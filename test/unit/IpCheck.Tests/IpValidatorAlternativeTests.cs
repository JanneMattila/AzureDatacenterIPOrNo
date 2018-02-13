using Xunit;

namespace IpCheck.Tests
{
    [Trait("type", "unit")]
    public class IpValidatorAlternativeTests : IpValidatorBaseTests
    {
        public IpValidatorAlternativeTests() : base(new IpValidatorAlternative())
        {
        }
    }
}
