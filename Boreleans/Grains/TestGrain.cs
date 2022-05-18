using Boreleans.Services;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Boreleans.Grains
{
    internal class TestGrain : Grain, ITestGrain
    {
        private readonly IMessenger messenger;
        private readonly ILogger<TestGrain> logger;

        public TestGrain(IMessenger messenger, ILogger<TestGrain> logger)
        {
            this.messenger = messenger;
            this.logger = logger;
        }

        public override Task OnActivateAsync()
        {
            logger.LogInformation("Activating {grainReference}", GrainReference);
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            logger.LogInformation("Deactivating {grainReference}", GrainReference);
            return base.OnDeactivateAsync();
        }

        public async Task Run()
        {
            try
            {
                logger.LogInformation("FUNCTION: Run {grainReference}", GrainReference);

                var result = await messenger.SendMessage(0x0000dead);

                logger.LogInformation("Run returned {result}", result);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Unexpected error while trying to wait for the task in Run");
            }
        }

        public async Task RunInterleaved()
        {
            try
            {
                logger.LogInformation("FUNCTION: RunInterleaved {grainReference}", GrainReference);

                var result = await messenger.SendMessage(0x0000beef);

                logger.LogInformation("RunInterleaved returned {result}", result);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Unexpected error while trying to wait for the task in RunInterleaved");
            }
        }
    }
}
