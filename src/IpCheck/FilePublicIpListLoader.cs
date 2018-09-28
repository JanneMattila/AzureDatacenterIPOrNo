using IpCheck.Models;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IpCheck
{
    public class FilePublicIpListLoader : IPublicIpListLoader
    {
        public async Task<AzurePublicIpAddresses> LoadAsync()
        {
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

                return await Task.FromResult(ipAddresses).ConfigureAwait(false);
            }
        }
    }
}
