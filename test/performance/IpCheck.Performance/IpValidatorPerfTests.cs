using BenchmarkDotNet.Attributes;

namespace IpCheck.Performance
{
    [Config(typeof(IpCheckBenchmarkConfig))]
    public class IpValidatorPerfTests
    {
        private readonly IIpValidator _validatorBase = new IpValidator();
        private readonly IIpValidator _validatorAlternative = new IpValidatorAlternative();

        [GlobalSetup]
        public void Setup()
        {
            _validatorBase.Initialize();
            _validatorAlternative.Initialize();
        }

        [Benchmark(Baseline = true)]
        public bool Base()
        {
            return _validatorBase.TryParse("13.14.15.26", out IpValidationResult result);
        }

        [Benchmark()]
        public bool Alternative()
        {
            return _validatorAlternative.TryParse("13.14.15.26", out IpValidationResult result);
        }
    }
}
