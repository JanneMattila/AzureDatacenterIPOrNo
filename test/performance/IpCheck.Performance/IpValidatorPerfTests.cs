using BenchmarkDotNet.Attributes;
using System.Threading.Tasks;

namespace IpCheck.Performance
{
    [Config(typeof(IpCheckBenchmarkConfig))]
    public class IpValidatorPerfTests
    {
        private readonly IIpValidator _validatorBase = new IpValidator(new FilePublicIpListLoader());

        [GlobalSetup]
        public async Task Setup()
        {
            await _validatorBase.LoadAsync();
        }

        [Benchmark(Baseline = true)]
        public async Task Base()
        {
            await _validatorBase.TryParseAsync("13.14.15.26");
        }
    }
}
