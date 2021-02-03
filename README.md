# Azure Datacenter IP Or No

[![AzureDatacenterIPOrNo](https://img.shields.io/powershellgallery/v/AzureDatacenterIPOrNo.svg?style=flat-square&label=AzureDatacenterIPOrNo)](https://www.powershellgallery.com/packages/AzureDatacenterIPOrNo/)

This repository contains PowerShell Module `AzureDatacenterIPOrNo` with contains cmdlet: `Get-AzureDatacenterIPOrNo` for checking if given
IP is Azure Datacenter address.

## Usage

You can install this module directly from PowerShell gallery:

```powershell
Install-Module -Name AzureDatacenterIPOrNo
```

Then you can use it for verifying the possible Azure IP address:

```powershell
# Update the Azure IP address list from internet
> Get-AzureDatacenterIPOrNo -IP 13.104.157.130 -UpdateCache

Updating cache...

Ip             Region    IpRange           Source
--             ------    -------           ------
13.104.157.130 westindia 13.104.157.128/25 ServiceTags_Public_20210125

# Make another request
> Get-AzureDatacenterIPOrNo -IP 13.107.246.10

Region              Ip
------              --
Not Public Azure IP 13.107.246.10
```
