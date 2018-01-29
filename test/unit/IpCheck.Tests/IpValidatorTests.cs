using System;
using Xunit;

namespace IpCheck.Tests
{
    [Trait("type", "unit")]
    public class IpValidatorTests
    {
        private IpValidator _validator = new IpValidator();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("-")]
        [InlineData("a")]
        public void InvalidInputTest(string ip)
        {
            // Arrange
            var expectedParse = false;

            // Act
            var actualParse = _validator.TryParse(ip, out IpValidationResult result);

            // Assert
            Assert.Equal(expectedParse, actualParse);
            Assert.NotNull(result);
            Assert.NotNull(result.Error);
        }

        [Theory]
        [InlineData("13.70.64.1", "australiaeast", "13.70.64.0/18")]
        [InlineData("20.36.0.200", "uswest2", "20.36.0.0/19")]
        [InlineData("192.168.0.1", IpValidator.NonAzureIpAddress, null)]
        [InlineData("10.168.0.1", IpValidator.NonAzureIpAddress, null)]
        public void IpCheckTest(string ip, string expectedRegion, string expectedIpRange)
        {
            // Arrange
            var expectedParse = true;

            // Act
            var actualParse = _validator.TryParse(ip, out IpValidationResult result);

            // Assert
            Assert.Equal(expectedParse, actualParse);
            Assert.NotNull(result);
            Assert.Equal(expectedRegion, result.Region);
            Assert.Equal(expectedIpRange, result.IpRange);
            Assert.Equal(ip, result.Ip);
        }
    }
}
