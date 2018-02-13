using IpCheck.Models;
using System;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace IpCheck
{
    public class IpValidator : IIpValidator
    {
        private static Lazy<AzurePublicIpAddresses> ipAddressLoader = new Lazy<AzurePublicIpAddresses>(() =>
        {
            // From: https://www.microsoft.com/en-us/download/details.aspx?id=41653
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
#pragma warning disable S1481
                        // Unused local variable due to forced initialization.
                        var ipNetwork = ipRange.IPNetwork;
#pragma warning restore S1481
                    }
                }

                return ipAddresses;
            }
        });

        public void Initialize()
        {
#pragma warning disable S1481
            // Unused local variable due to forced initialization.
            AzurePublicIpAddresses ipAddresses = ipAddressLoader.Value;
#pragma warning restore S1481
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
