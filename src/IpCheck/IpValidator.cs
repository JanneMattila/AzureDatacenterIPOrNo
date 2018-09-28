using IpCheck.Models;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IpCheck
{
    public class IpValidator : IIpValidator
    {
        private DateTime _lastUpdated = DateTime.MinValue;
        private string _source;
        private AzurePublicIpAddresses _ipAddresses;
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private readonly IPublicIpListLoader _publicIpListLoader;

        public IpValidator(IPublicIpListLoader publicIpListLoader)
        {
            _publicIpListLoader = publicIpListLoader;
        }

        public async Task<AzurePublicIpAddresses> LoadAsync()
        {
            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);

            try
            {
                if (_lastUpdated > DateTime.UtcNow.AddDays(-1))
                {
                    return _ipAddresses;
                }

                var list = await _publicIpListLoader.LoadAsync().ConfigureAwait(false);
                _source = list.Source;
                _ipAddresses = list.Addresses;
                _lastUpdated = DateTime.UtcNow;
                return _ipAddresses;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<IpValidationResult> TryParseAsync(string ip)
        {
            if (!IPAddress.TryParse(ip, out IPAddress ipAddress))
            {
                return new IpValidationResult
                {
                    StatusCode = 400,
                    Error = "IPAddress Validation failed."
                };
            }
            try
            {
                AzurePublicIpAddresses ipAddresses = await LoadAsync().ConfigureAwait(false);
                foreach (var region in ipAddresses.Region)
                {
                    foreach (var ipRange in region.IpRange)
                    {
                        if (ipRange.IPNetwork.Contains(ipAddress))
                        {
                            return new IpValidationResult
                            {
                                StatusCode = 200,
                                Region = region.Name,
                                IpRange = ipRange.Subnet,
                                Ip = ipAddress.ToString(),
                                Source = _source
                            };
                        }
                    }
                }

                return new IpValidationResult
                {
                    StatusCode = 200,
                    Region = IpValidatorConstants.NonAzureIpAddress,
                    Ip = ipAddress.ToString()
                };
            }
            catch (Exception ex)
            {
                return new IpValidationResult
                {
                    StatusCode = 500,
                    Error = ex.Message
                };
            }
        }
    }
}
