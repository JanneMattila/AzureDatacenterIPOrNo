using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IpCheck.FunctionApp.Tests
{
    public class IpCheckBaseTests
    {
        private const string WebAppAddressEnvirontVariableName = "Custom_WebAppUri";
        protected const string LocalAppAddress = "http://localhost:7071/";
        protected const string IpCheckApiPath = "/api/IpCheck";

        protected Uri BaseAddress { get; private set; }
        protected HttpClient Client { get; private set; }

        public IpCheckBaseTests()
        {
            var baseAddressOverride = Environment.GetEnvironmentVariable(WebAppAddressEnvirontVariableName);
            if (string.IsNullOrEmpty(baseAddressOverride))
            {
                // Use local development address as default url
                baseAddressOverride = LocalAppAddress;
            }

            BaseAddress = new Uri(baseAddressOverride);

            Client = new HttpClient
            {
                BaseAddress = BaseAddress
            };
        }
    }
}
