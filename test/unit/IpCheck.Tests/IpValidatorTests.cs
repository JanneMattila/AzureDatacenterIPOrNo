using Xunit;

namespace IpCheck.Tests
{
    [Trait("type", "unit")]
    public class IpValidatorTests : IpValidatorBaseTests
    {
        public IpValidatorTests() : base(new IpValidator(new FilePublicIpListLoader()))
        {
        }
    }
}
