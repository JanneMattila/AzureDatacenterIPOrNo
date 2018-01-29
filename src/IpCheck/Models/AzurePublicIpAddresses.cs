using System.Collections.Generic;
using System.Xml.Serialization;

namespace IpCheck.Models
{
    [XmlRoot]
    public class AzurePublicIpAddresses
    {
        [XmlElement]
        public List<AzurePublicIpAddressesRegion> Region { get; set; }
    }
}
