using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IpCheck.FunctionApp.Tests
{
    [Trait("type", "release")]
    public class IpCheckFunctionTests : IpCheckBaseTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("?ip=")]
        [InlineData("?ip= ")]
        [InlineData("?ip=-")]
        [InlineData("?ip=a")]
        [InlineData("")]
        public async Task InvalidInputTestAsync(string query)
        {
            // Arrange
            var expectedHttpStatus = HttpStatusCode.BadRequest;

            // Act
            var response = await Client.GetAsync($"{IpCheckApiPath}{query}");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(expectedHttpStatus, response.StatusCode);
        }


        [Theory]
        [InlineData("13.70.64.1", "australiaeast", "13.70.64.0/18")]
        [InlineData("20.36.0.200", "uswest2", "20.36.0.0/19")]
        [InlineData("192.168.0.1", "Non-Azure IP", null)]
        [InlineData("10.168.0.1", "Non-Azure IP", null)]
        public async Task IpCheckTestAsync(string ip, string expectedRegion, string expectedIpRange)
        {
            // Arrange
            var expectedHttpStatus = HttpStatusCode.OK;

            // Act
            var response = await Client.GetAsync($"{IpCheckApiPath}?ip={ip}");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(expectedHttpStatus, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(json);
            Assert.Equal(expectedRegion, data["region"].Value<string>());
            Assert.Equal(expectedIpRange, data["ipRange"]?.Value<string>());
            Assert.Equal(ip, data["ip"].Value<string>());
            Assert.Null(data["error"]);
        }
    }
}
