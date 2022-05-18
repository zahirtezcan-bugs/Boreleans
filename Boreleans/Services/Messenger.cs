using Boreleans.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Boreleans.Services
{
    internal class Messenger : IMessenger, IPooledObjectPolicy<CancelingTaskSource<int>>
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(2);
        private ObjectPool<CancelingTaskSource<int>> taskSources;

        public Messenger()
        {
            taskSources = new DefaultObjectPool<CancelingTaskSource<int>>(this);
        }

        public async ValueTask<MessageResult<int>> SendMessage(int parameter)
        {
            var taskSource = taskSources.Get();
            try
            {
                short token = taskSource.Initialize(DefaultTimeout);

                //here send a message (parameter) to external component
                //and register (taskSource, token) pair in a table of requests,
                //and when a response is received call taskSource.TrySetResult(token) via table-of-requests.
                //this way we can handle multiple ongoing messages to the external component

                //here we only wait on the task until timeout
                var result = await taskSource.GetTask(token);
                return result;
            }
            finally 
            {
                taskSources.Return(taskSource);
            }
        }

        #region IPooledObjectPolicy<CancelingTaskSource> Implementation

        CancelingTaskSource<int> IPooledObjectPolicy<CancelingTaskSource<int>>.Create()
        {
            return new CancelingTaskSource<int>();
        }

        bool IPooledObjectPolicy<CancelingTaskSource<int>>.Return(CancelingTaskSource<int> obj)
        {
            return obj.Reset();
        } 

        #endregion
    }
}
