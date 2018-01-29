Param (
    [string] $ResourceGroupName = "ipcheck-local-rg",
    [string] $Location = "North Europe",
    [string] $Template = "$PSScriptRoot\azuredeploy.json"
)

$ErrorActionPreference = "Stop"

$timestamp = (Get-Date).ToString("yyyy-MM-dd-HH-mm-ss")
$deploymentName = "Local-$timestamp"

if ([string]::IsNullOrEmpty($env:RELEASE_DEFINITIONNAME))
{
    Write-Host (@"
Not executing inside VSTS Release Management.
Make sure you have done "Login-AzureRmAccount" and
"Select-AzureRmSubscription -SubscriptionName name"
so that script continues to work correctly for you.
"@)
}
else
{
	$deploymentName = $env:RELEASE_RELEASENAME
}

if ((Get-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location -ErrorAction SilentlyContinue) -eq $null)
{
    Write-Warning "Resource group '$ResourceGroupName' doesn't exist and it will be created."
    New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location -Verbose
}

$result = New-AzureRmResourceGroupDeployment `
	-DeploymentName $deploymentName `
    -ResourceGroupName $ResourceGroupName `
    -TemplateFile $Template `
    -Verbose

if ($result.Outputs.webAppName -eq $null -or
    $result.Outputs.webAppUri -eq $null)
{
    Throw "Template deployment didn't return web app information correctly and therefore deployment is cancelled."
}

$result

$webAppName = $result.Outputs.webAppName.value
$webAppUri = $result.Outputs.webAppUri.value
Write-Host "##vso[task.setvariable variable=Custom.WebAppName;]$webAppName"
Write-Host "##vso[task.setvariable variable=Custom.WebAppUri;]$webAppUri"

Write-Host "Validating that site is up and running..."
$running = 0
for ($i = 0; $i -lt 60; $i++)
{
    try 
    {
        $request = Invoke-WebRequest -Uri $webAppUri -UseBasicParsing -DisableKeepAlive -ErrorAction SilentlyContinue
        Write-Host "Site status code $($request.StatusCode)."

        if ($request.StatusCode -eq 200)
        {
            Write-Host "Site is up and running."
            $running++
        }
    }
    catch
    {
        Start-Sleep -Seconds 3
    }

    if ($running -eq 10)
    {
        return
    }
}

Throw "Site didn't respond on defined time."