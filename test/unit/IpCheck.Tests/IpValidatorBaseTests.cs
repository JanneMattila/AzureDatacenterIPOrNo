using System.Threading.Tasks;
using Xunit;

namespace IpCheck.Tests
{
    [Trait("type", "unit")]
    public abstract class IpValidatorBaseTests
    {
        private readonly IIpValidator _validator;

        protected IpValidatorBaseTests(IIpValidator validator)
        {
            _validator = validator;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("-")]
        [InlineData("a")]
        public async Task InvalidInputTest(string ip)
        {
            // Arrange
            await _validator.LoadAsync();
            var expectedStatusCode = 400;

            // Act
            var actual = await _validator.TryParseAsync(ip);

            // Assert
            Assert.Equal(expectedStatusCode, actual.StatusCode);
            Assert.NotNull(actual);
            Assert.NotNull(actual.Error);
        }

        [Theory]
        [InlineData("13.70.64.1", "australiaeast", "13.70.64.0/18")]
        [InlineData("20.36.0.200", "uswest2", "20.36.0.0/19")]
        [InlineData("192.168.0.1", IpValidatorConstants.NonAzureIpAddress, null)]
        [InlineData("10.168.0.1", IpValidatorConstants.NonAzureIpAddress, null)]
        public async Task IpCheckTest(string ip, string expectedRegion, string expectedIpRange)
        {
            // Arrange
            await _validator.LoadAsync();
            var expectedStatusCode = 200;

            // Act
            var actual = await _validator.TryParseAsync(ip);

            // Assert
            Assert.Equal(expectedStatusCode, actual.StatusCode);
            Assert.NotNull(actual);
            Assert.Equal(expectedRegion, actual.Region);
            Assert.Equal(expectedIpRange, actual.IpRange);
            Assert.Equal(ip, actual.Ip);
        }
    }
}
