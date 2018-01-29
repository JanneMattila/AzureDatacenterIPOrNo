using IpCheck.Models;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace IpCheck
{
    public class IpValidator
    {
        public const string NonAzureIpAddress = "Non-Azure IP";

        private static Lazy<AzurePublicIpAddresses> ipAddressLoader = new Lazy<AzurePublicIpAddresses>(() =>
        {
            // From: https://www.microsoft.com/en-us/download/details.aspx?id=41653
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream("IpCheck.PublicIPs.xml");

            using (var reader = XmlReader.Create(resourceStream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AzurePublicIpAddresses));
                var ipAddresses = serializer.Deserialize(reader) as AzurePublicIpAddresses;
                foreach (var region in ipAddresses.Region)
                {
                    foreach (var ipRange in region.IpRange)
                    {
                        var ipNetwork = ipRange.IPNetwork;
                    }
                }

                return ipAddresses;
            }
        });

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
                AzurePublicIpAddresses ipAddresses = ipAddressLoader.Value;
                result = null;

                foreach (var region in ipAddresses.Region)
                {
                    foreach (var ipRange in region.IpRange)
                    {
                        if (IPNetwork.Contains(ipRange.IPNetwork, ipAddress))
                        {
                            result = new IpValidationResult
                            {
                                Region = region.Name,
                                IpRange = ipRange.Subnet,
                                Ip = ipAddress.ToString()
                            };
                            return true;
                        }
                    }
                }

                result = new IpValidationResult
                {
                    Region = NonAzureIpAddress,
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
