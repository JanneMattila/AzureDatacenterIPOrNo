<#
.SYNOPSIS
This cmdlet lets you verify if IP address is Azure datacenter 
IP Address or no. This information is based from publicly available
document: Microsoft Azure Datacenter IP Ranges
from: https://www.microsoft.com/en-us/download/details.aspx?id=56519

.DESCRIPTION
Use this cmdlet for verifying if IP address is from known Azure
datacenter IP address space.

.PARAMETER IP
IP address to check if it's part of the know Azure Datacenter IP Ranges.

.PARAMETER UpdateCache
Fetch updated IP address range file from internet.

.EXAMPLE
Get-AzureDatacenterIPOrNo -IP 40.82.192.123

.EXAMPLE
Get-AzureDatacenterIPOrNo -IP 40.82.192.123 -UpdateCache

.LINK
https://www.microsoft.com/en-us/download/details.aspx?id=56519

#>
function Get-AzureDatacenterIPOrNo
(
    [Parameter(Mandatory = $true, HelpMessage = "IP address to check if it's part of the know Azure Datacenter IP ranges.")] 
    [string] $IP,

    [Parameter(HelpMessage = "Update cache file.")] 
    [switch] $UpdateCache = $false
) {
    $ErrorActionPreference = "Stop"

    Add-Type -Path $PSScriptRoot\System.Net.IPNetwork.dll
    $ipAddress = [System.Net.IPAddress]::Parse($IP)

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
    foreach ($serviceTag in $json.values) {
        foreach ($range in $serviceTag.properties.addressPrefixes) {
            $ipNetwork = [System.Net.IPNetwork]::Parse($range)
            if ($ipNetwork.Contains($ipAddress)) {
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
