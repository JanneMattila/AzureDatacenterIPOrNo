using System.Collections.Generic;
using System.Xml.Serialization;

namespace IpCheck.Models
{
    public class AzurePublicIpAddressesRegion
    {
        [XmlElement]
        public List<AzurePublicIpAddressesRegionIpRange> IpRange { get; set; }

        [XmlAttribute]
        public string Name { get; set; }
    }
}
