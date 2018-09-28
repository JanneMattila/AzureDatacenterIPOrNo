using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IpCheck.FunctionApp
{
    public static class IpCheckFunction
    {
        private static IIpValidator _validator = new IpValidator(new OnlinePublicIpListLoader());

        static IpCheckFunction()
        {
            _validator.LoadAsync();
        }

        [FunctionName("Ping")]
        public static IActionResult Ping([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, ILogger log)
        {
            return new OkResult();
        }

        [FunctionName("IpCheck")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, ILogger log)
        {
            string ip = req.Query["ip"];

            log.LogInformation($"IpCheck processing request with ip: {ip}");
            var result = await _validator.TryParseAsync(ip);
            return new ObjectResult(result)
            {
                StatusCode = result.StatusCode
            };
        }
    }
}
