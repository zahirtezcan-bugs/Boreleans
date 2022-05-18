using Boreleans.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;

try
{
    Host.CreateDefaultBuilder(args)
        .UseOrleans((context, silos) =>
        {
            silos.UseLocalhostClustering()
                 .ConfigureServices((context, services) =>
                 {
                     services.AddHostedService<ConsoleActivatorService>()
                             .AddSingleton<IMessenger, Messenger>();
                 });
        })
        .ConfigureLogging((context, logs) => logs.AddSimpleConsole())
        .Build()
        .Run();
}
catch (Exception exc)
{
    Console.Error.WriteLine(exc);
}