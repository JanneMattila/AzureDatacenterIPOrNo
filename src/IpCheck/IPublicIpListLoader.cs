﻿using IpCheck.Models;
using System.Threading.Tasks;

namespace IpCheck
{
    public interface IPublicIpListLoader
    {
        Task<IpAddressSource> LoadAsync();
    }
}