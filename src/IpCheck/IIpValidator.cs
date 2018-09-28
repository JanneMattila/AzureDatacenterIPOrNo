﻿using IpCheck.Models;
using System.Threading.Tasks;

namespace IpCheck
{
    public interface IIpValidator
    {
        Task<AzurePublicIpAddresses> LoadAsync();
        Task<IpValidationResult> TryParseAsync(string ip);
    }
}