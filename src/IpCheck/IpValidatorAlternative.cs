using IpCheck.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace IpCheck
{
    public class IpValidatorAlternative : IIpValidator
    {
        private List<IpMap> _ipMapList;

        public void Initialize()
        {
            // From: https://www.microsoft.com/en-us/download/details.aspx?id=41653
            var ipMapList = new List<IpMap>();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream("IpCheck.PublicIPs.xml");

            using (var reader = XmlReader.Create(resourceStream))
            {
                var serializer = new XmlSerializer(typeof(AzurePublicIpAddresses));
                var ipAddresses = serializer.Deserialize(reader) as AzurePublicIpAddresses;
                foreach (var region in ipAddresses.Region)
                {
                    foreach (var ipRange in region.IpRange)
                    {
                        ipMapList.Add(new IpMap
                        {
                            IPNetwork = ipRange.IPNetwork,
                            Region = region.Name,
                            Subnet = ipRange.Subnet
                        });
                    }
                }

                _ipMapList = ipMapList;
            }
        }

        public bool TryParse(string ip, out IpValidationResult result)
        {
            if (!IPAddress.TryParse(ip, out IPAddress ipAddress))
            {
                result = new IpValidationResult
                {
                    Error = "IPAddress Validation failed."
                };
                return false;
            }
            try
            {
                result = null;

                foreach (var ipRange in _ipMapList)
                {
                    if (IPNetwork.Contains(ipRange.IPNetwork, ipAddress))
                    {
                        result = new IpValidationResult
                        {
                            Region = ipRange.Region,
                            IpRange = ipRange.Subnet,
                            Ip = ipAddress.ToString()
                        };
                        return true;
                    }
                }

                result = new IpValidationResult
                {
                    Region = IpValidatorConstants.NonAzureIpAddress,
                    Ip = ipAddress.ToString()
                };
                return true;
            }
            catch (Exception ex)
            {
                result = new IpValidationResult
                {
                    Error = ex.Message
                };
                return false;
            }
        }
    }
}
