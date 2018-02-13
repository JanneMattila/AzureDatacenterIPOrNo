using BenchmarkDotNet.Running;

namespace IpCheck.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<IpValidatorPerfTests>();
        }
    }
}
