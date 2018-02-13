using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace IpCheck.Performance
{
    public class IpCheckBenchmarkConfig : ManualConfig
    {
        public IpCheckBenchmarkConfig()
        {
            Add(Job.Default.With(CsProjCoreToolchain.NetCoreApp20).WithId(".NET Core 2.0"));
        }
    }
}
