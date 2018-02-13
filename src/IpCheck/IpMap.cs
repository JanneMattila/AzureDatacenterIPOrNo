using System.Net;

namespace IpCheck
{
    public struct IpMap
    {
        public IPNetwork IPNetwork { get; set; }
        public string Subnet { get; set; }
        public string Region { get; set; }
    }
}
