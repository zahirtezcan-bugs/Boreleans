using Boreleans.Services;
using System.Threading.Tasks.Sources;

namespace Boreleans.Tasks
{
    internal class CancelingTaskSource<T> : IValueTaskSource<MessageResult<T>>
    {
        private ManualResetValueTaskSourceCore<MessageResult<T>> source;
        private CancellationTokenSource timeoutCts;

        public CancelingTaskSource()
        {
            source = new ManualResetValueTaskSourceCore<MessageResult<T>>();
            timeoutCts = InitializeCancellation();
        }

        public short Initialize(TimeSpan timeout, bool runContinuationsAsync = false)
        {
            source.RunContinuationsAsynchronously = runContinuationsAsync;
            timeoutCts.CancelAfter(timeout);

            return source.Version;
        }

        private CancellationTokenSource InitializeCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Token.Register(TimeoutCallback, this);

            return cts;
        }

        private void TimeoutCallback(object? state)
        {
            if (state is CancelingTaskSource<T> taskSource)
            {
                taskSource.TrySetTimeout();
            }
        }

        public bool Reset()
        {
            bool isCtsReset;
            try
            {
                isCtsReset = timeoutCts.TryReset();
            }
            catch
            {
                isCtsReset = false;
            }

            if (!isCtsReset)
            {
                timeoutCts = InitializeCancellation();
            }

            source.Reset();
            return true;
        }

        public MessageResult<T> GetResult(short token)
        {
            return source.GetResult(token);
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return source.GetStatus(token);
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            source.OnCompleted(continuation, state, token, flags);
        }

        public ValueTask<MessageResult<T>> GetTask(short token)
        {
            var status = source.GetStatus(token);
            if (status == ValueTaskSourceStatus.Pending)
            {
                return new ValueTask<MessageResult<T>>(this, source.Version);
            }

            return ValueTask.FromResult(source.GetResult(token));
        }

        public void TrySetSuccessful(short token, T result)
        {
            try
            {
                if (source.Version == token)
                {
                    source.SetResult(new MessageResult<T>
                    {
                        Error = MessageError.Successful,
                        Value = result
                    });
                }
            }
            catch
            { }
        }

        private void TrySetTimeout()
        {
            try
            {
                source.SetResult(new MessageResult<T>
                {
                    Error = MessageError.Timeout
                });
            }
            catch
            { }
        }
    }
}
