using Microsoft.Extensions.Logging;
using Orleans;

namespace Boreleans.Grains
{
    internal class TestGrain : Grain, ITestGrain
    {
        private readonly ILogger<TestGrain> logger;

        public TestGrain(ILogger<TestGrain> logger)
        {
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

        public Task Run()
        {
            logger.LogInformation("FUNCTION: Run {grainReference}", GrainReference);
            return Task.CompletedTask;
        }
    }
}
