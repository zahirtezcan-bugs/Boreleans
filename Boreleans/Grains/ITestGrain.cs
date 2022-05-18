using Orleans;
using Orleans.Concurrency;

namespace Boreleans.Grains;

internal interface ITestGrain : IGrainWithIntegerKey
{
    Task Run();

    [AlwaysInterleave]
    Task RunInterleaved();
}
