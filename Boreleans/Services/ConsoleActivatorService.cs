using Boreleans.Grains;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boreleans.Services
{
    internal class ConsoleActivatorService : BackgroundService
    {
        private readonly IGrainFactory grains;
        private readonly ILogger<ConsoleActivatorService> logger;

        public ConsoleActivatorService(IGrainFactory grains,
                                       ILogger<ConsoleActivatorService> logger)
        {
            this.grains = grains;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellation)
        {
            try
            {
                logger.LogInformation("Please start typing commands");
                string? command = null;

                while (!cancellation.IsCancellationRequested)
                {
                    var readLineTask = Task.Run(() => command = Console.ReadLine());
                    await WhenTaskOrCanceled(readLineTask, cancellation);

                    if (command != null)
                    {
                        await RunCommand(command, cancellation);
                    }
                }
            }
            catch (OperationCanceledException)
            { }
            catch (Exception exc)
            {
                logger.LogError(exc, "Unexpected error while trying to get input from user");
            }
            finally
            {
                logger.LogInformation("Closed console activator");
            }
        }

        private async Task RunCommand(string command, CancellationToken cancellation)
        {
            try
            {
                switch (command)
                {
                    case "run":
                        await CallRun(cancellation);
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    default:
                        logger.LogError("Unknown command '{command}'", command);
                        break;
                }
            }
            catch (OperationCanceledException)
            { }
            catch (Exception exc)
            {
                logger.LogError(exc, "Unexpected error while trying to run '{}'", command);
            }
        }

        private async Task CallRun(CancellationToken cancellation)
        {
            var testGrain = grains.GetGrain<ITestGrain>(0);
            await WhenTaskOrCanceled(testGrain.Run(), cancellation);
        }

        private async Task WhenTaskOrCanceled(Task task, CancellationToken cancellation)
        {
            var cancellationTcs = new TaskCompletionSource();
            using var canceler = cancellation.Register(() => cancellationTcs.TrySetCanceled());

            await Task.WhenAny(task, cancellationTcs.Task);
        }
    }
}
