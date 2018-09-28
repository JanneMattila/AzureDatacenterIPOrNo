using IpCheck.Models;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IpCheck
{
    public class OnlinePublicIpListLoader : IPublicIpListLoader
    {
        private const string ListUrl = "https://www.microsoft.com/en-us/download/details.aspx?id=41653";
        private const string DownloadUrl = "https://download.microsoft.com/download/0/1/8/018E208D-54F8-44CD-AA26-CD7BC9524A8C/{0}.xml";
        private const string FileName = "PublicIPs.xml";

        public async Task<IpAddressSource> LoadAsync()
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync(ListUrl).ConfigureAwait(false);
            var fileStartIndex = response.IndexOf("PublicIPs_");
            var fileEndIndex = response.IndexOf(".xml", fileStartIndex);
            var file = response.Substring(fileStartIndex, fileEndIndex - fileStartIndex);
            var downloadLink = string.Format(DownloadUrl, file);
            var xml = await client.GetStringAsync(downloadLink).ConfigureAwait(false);

            using (var stringReader = new StringReader(xml))
            using (var reader = XmlReader.Create(stringReader))
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

                return new IpAddressSource()
                {
                    Source = file,
                    Addresses = ipAddresses
                };
            }
        }
    }
}
