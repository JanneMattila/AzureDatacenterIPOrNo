<#
.SYNOPSIS
This cmdlet lets you verify if IP address is Azure datacenter 
IP Address or no. This information is based from publicly available
document: Microsoft Azure Datacenter IP Ranges
from: https://www.microsoft.com/en-us/download/details.aspx?id=41653

.DESCRIPTION
Use this cmdlet for verifying if IP address is from known Azure
datacenter IP address space.

NOTE: This script uses "IPInRange" function from PSMailTools repository:
https://github.com/omniomi/PSMailTools
Original source url:
https://raw.githubusercontent.com/omniomi/PSMailTools/v0.2.0/src/Private/spf/IPInRange.ps1

.PARAMETER IP
IP address to check if it's part of the know Azure Datacenter IP Ranges.

.PARAMETER UpdateCache
Fetch updated IP address range file from internet.

.EXAMPLE
Get-AzureDatacenterIPOrNo -IP 40.82.192.123

.EXAMPLE
Get-AzureDatacenterIPOrNo -IP 40.82.192.123 -UpdateCache

.LINK
https://www.microsoft.com/en-us/download/details.aspx?id=41653

#>
function Get-AzureDatacenterIPOrNo
(
    [Parameter(Mandatory = $true, HelpMessage = "IP address to check if it's part of the know Azure Datacenter IP ranges.")] 
    [string] $IP,

    [Parameter(HelpMessage = "Update cache file.")] 
    [switch] $UpdateCache = $false
) {
    # Below "IPInRange" function from PSMailTools:
    # https://github.com/omniomi/PSMailTools
    #
    # IPInRange:
    # https://raw.githubusercontent.com/omniomi/PSMailTools/v0.2.0/src/Private/spf/IPInRange.ps1
    function IPInRange {
        [cmdletbinding()]
        [outputtype([System.Boolean])]
        param(
            # IP Address to find.
            [parameter(Mandatory,
                Position = 0)]
            [validatescript( {
                    ([System.Net.IPAddress]$_).AddressFamily -eq 'InterNetwork'
                })]
            [string]
            $IPAddress,

            # Range in which to search using CIDR notation. (ippaddr/bits)
            [parameter(Mandatory,
                Position = 1)]
            [validatescript( {
                    $IP = ($_ -split '/')[0]
                    $Bits = ($_ -split '/')[1]

                    (([System.Net.IPAddress]($IP)).AddressFamily -eq 'InterNetwork')

                    if (-not($Bits)) {
                        throw 'Missing CIDR notiation.'
                    }
                    elseif (-not(0..32 -contains [int]$Bits)) {
                        throw 'Invalid CIDR notation. The valid bit range is 0 to 32.'
                    }
                })]
            [alias('CIDR')]
            [string]
            $Range
        )

        # Split range into the address and the CIDR notation
        [String]$CIDRAddress = $Range.Split('/')[0]
        [int]$CIDRBits = $Range.Split('/')[1]

        # Address from range and the search address are converted to Int32 and the full mask is calculated from the CIDR notation.
        [int]$BaseAddress = [System.BitConverter]::ToInt32((([System.Net.IPAddress]::Parse($CIDRAddress)).GetAddressBytes()), 0)
        [int]$Address = [System.BitConverter]::ToInt32(([System.Net.IPAddress]::Parse($IPAddress).GetAddressBytes()), 0)
        [int]$Mask = [System.Net.IPAddress]::HostToNetworkOrder(-1 -shl ( 32 - $CIDRBits))

        # Determine whether the address is in the range.
        if (($BaseAddress -band $Mask) -eq ($Address -band $Mask)) {
            $true
        }
        else {
            $false
        }
    }

    $cacheFileName = "$PSScriptRoot\PublicIPs.json"
    $cacheSource = "$PSScriptRoot\CacheSource.txt"
    if ((Test-Path -Path $cacheFileName) -eq $false -or
        $UpdateCache) {
        if ($UpdateCache) {
            Write-Host "Updating cache..."
        }
        else {
            Write-Warning "Cache file not found. Downloading it from..."
        }
    
        $url = "https://www.microsoft.com/en-us/download/details.aspx?id=56519"
        $response = Invoke-WebRequest $url -UseBasicParsing
        $fileStartIndex = $response.Content.IndexOf("ServiceTags_Public_")
        $fileEndIndex = $response.Content.IndexOf(".json", $fileStartIndex)
        $file = $response.Content.Substring($fileStartIndex, $fileEndIndex - $fileStartIndex)
        Set-Content $cacheSource -Value $file
        $downloadLink = "https://download.microsoft.com/download/7/1/D/71D86715-5596-4529-9B13-DA13A5DE5B63/$file.json"
        Invoke-WebRequest $downloadLink -UseBasicParsing -OutFile $cacheFileName
    }

    $json = Get-Content $cacheFileName | ConvertFrom-Json
    $source = Get-Content $cacheSource
    foreach ($serviceTag in $json.values.serviceTags) {
        foreach ($range in $serviceTag.properties.addressPrefixes) {
            $isInside = IPInRange -IPAddress $IP -Range $range
            if ($isInside) {
                return new-object psobject -property @{
                    Region  = $serviceTag.properties.region
                    IpRange = $range
                    Ip      = $IP
                    Source  = $source
                }
            }
        }
    }

    return new-object psobject -property @{
        Region = "Not Public Azure IP"
        Ip     = $IP
    }
}

Export-ModuleMember -Function Get-AzureDatacenterIPOrNo
