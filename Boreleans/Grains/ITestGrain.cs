using Orleans;

namespace Boreleans.Grains;

internal interface ITestGrain : IGrainWithIntegerKey
{
    Task Run();
}
