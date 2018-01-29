using System.Net;
using System.Xml.Serialization;

namespace IpCheck.Models
{
    public class AzurePublicIpAddressesRegionIpRange
    {
        private IPNetwork _ipNetwork;

        [XmlAttribute]
        public string Subnet { get; set; }

        public IPNetwork IPNetwork
        {
            get
            {
                return _ipNetwork ?? (_ipNetwork = IPNetwork.Parse(Subnet));
            }
        }
    }
}
