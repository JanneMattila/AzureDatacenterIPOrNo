using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpCheck.Performance
{
    public class IpValidatorPerfTests
    {
        private IpValidator _validator = new IpValidator();

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark(Baseline = true)]
        public bool Base()
        {
            return _validator.TryParse("13.14.15.26", out IpValidationResult result);
        }
    }
}
