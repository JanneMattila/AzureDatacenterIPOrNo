# Azure Datacenter IP Or No

Example [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview) application to demonstrate 
API for showing if IP Address is from [Microsoft Azure Datacenter IP Ranges](https://www.microsoft.com/en-us/download/details.aspx?id=41653).

## How this app works?

During local development API is exposed from url:

`http://localhost:7071/api/IpCheck`

You can pass on `?ip=<ip>` to then check if the given `<ip>` is one of the
Azure Datacenter IPs. Example:

Request: 

`http://localhost:7071/api/IpCheck?ip=191.239.0.100`

Response:
```json
{
    "region": "uswest",
    "ipRange": "191.239.0.0/18",
    "ip": "191.239.0.100"
}
```

Response fields:
* `"region"`: name of the [Azure Region](https://azure.microsoft.com/en-us/regions/) from which the IP is
* `"ipRange"`: IP range from the `PublicIPs_yyyyMMdd.xml` file which contains given IP
* `"ip"`: IP address that was checked

If IP is not part of any of the defined Azure Region IP Range then you get following response:

```json
{
    "region": "Non-Azure IP",
    "ip": "192.168.0.1"
}
```

If IP cannot be be parsed of there is some other error then you get following response (with `400 BadRequest` as HTTP Status):
```json
{
    "error": "IPAddress Validation failed."
}
```

## Repository folder structure

Here's the brief description of the repository folder structure:
* `deploy`: Contains deployment script and [Azure Resource Manager Template](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates) for the application infrastructure
* `src`
* `test`
  * `performance`
  * `release`
  * `unit`

## How do I deploy this myself?

Here's example PowerShell commands if you want to deploy the infrastructure yourself:

```powershell
# Login to Azure
Login-AzureRmAccount

# Select correct deployment subscription
Select-AzureRmSubscription -SubscriptionName <subsription_name>

# Go to deploy folder
cd .\deploy\

# Launch deployment
.\deploy.ps1
```

Use then e.g. Visual Studio for deploying the actual application.

If PowerShell is not your thing then you can use these buttons to do the deployments:

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FJanneMattila%2FAzureDatacenterIPOrNo%2Fmaster%2Fdeploy%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FJanneMattila%2FAzureDatacenterIPOrNo%2Fmaster%2Fdeploy%2Fazuredeploy.json" target="_blank">
    <img src="http://armviz.io/visualizebutton.png"/>
</a>
