using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace IpCheck.FunctionApp
{
    public static class IpCheckFunction
    {
        private static IpValidator _validator = new IpValidator();

        [FunctionName("IpCheck")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, TraceWriter log)
        {
            string ip = req.Query["ip"];

            log.Info($"IpCheck processing request with ip: {ip}");
            if (_validator.TryParse(ip, out IpValidationResult result))
            {
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult(result);
        }
    }
}
