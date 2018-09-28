using IpCheck.Models;

namespace IpCheck
{
    public class IpAddressSource
    {
        public string Source { get; set; }

        public AzurePublicIpAddresses Addresses { get; set; }
    }
}
