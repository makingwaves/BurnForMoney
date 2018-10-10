# Burn for money

## Documentation

https://makingwaves.sharepoint.com/sites/BurnforMoney

## Supported systems

|                 |  Strava  | Endomondo | Runkeeper |
| :-------------: | :------: | :-------: | :-------: |
|       Run       | &#x2714; | &#x2718;  | &#x2718;  |
|      Ride       | &#x2714; | &#x2718;  | &#x2718;  |
|      Walk       | &#x2714; | &#x2718;  | &#x2718;  |
|  Winter sports  | &#x2714; | &#x2718;  | &#x2718;  |
|  Water sports   | &#x2714; | &#x2718;  | &#x2718;  |
|   Team sports   | &#x2718; | &#x2718;  | &#x2718;  |
|       Gym       | &#x2714; | &#x2718;  | &#x2718;  |
|      Hike       | &#x2714; | &#x2718;  | &#x2718;  |
| Fitness / Dance | &#x2714; | &#x2718;  | &#x2718;  |
|      Other      | &#x2714; | &#x2718;  | &#x2718;  |

## Installation

### Requirements

* [Azure Developer Tools](https://azure.microsoft.com/en-us/tools/)
* [Azure Storage Emulator](https://go.microsoft.com/fwlink/?LinkId=717179&clcid=0x409)
* [PowerShell](https://www.microsoft.com/web/handlers/webpi.ashx/getinstaller/WindowsAzurePowershellGet.3f.3f.3fnew.appids)
* [Azure Functions and Web Jobs Tools](https://marketplace.visualstudio.com/items?itemName=VisualStudioWebandAzureTools.AzureFunctionsandWebJobsTools) - Visual Studio only
* [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools) - recommended for Mac OS / Linux users, optional for Windows users
* IDE: [Visual Studio 2017](https://visualstudio.microsoft.com/downloads/) / [Visual Studio Code](https://visualstudio.microsoft.com/downloads/)

### Design

* UX: https://makingwaves.invisionapp.com/share/Y5OB37AQ349
* Design: https://makingwaves.invisionapp.com/share/TXNW58VVRU2#/screens/318202851_bfm_Home

### Environments

* **Test:**
  * Website: https://burnformoney-test.azurewebsites.net/
  * Api: https://burnformoneyfunc-test.azurewebsites.net/
* **Production:**
  - Website: https://bfm.makingwaves.pl/
  - Api: https://burnformoneyfunc-prod.azurewebsites.net/

### Azure functions

* Documentation: https://docs.microsoft.com/en-us/azure/azure-functions/


### Strava API

* Rate limits: 600 requests every 15 minutes, 30000 daily
* Documentation: https://developers.strava.com/docs/
* Authentication: https://developers.strava.com/docs/authentication/
* API: https://developers.strava.com/docs/reference/

### Safeguarding passwords

All passwords are stored in the [KeePass](https://keepass.info/) database, included in version control system. This database should store all credentials used by BurnForMoney projects (Azure services, APIs, etc.).

Application passwords are deployed by ARM scripts to [Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/) storage and should be retrieved only from this service. All apllications must be properly authorised to have an access to Key Vault services.

* Documentation: https://docs.microsoft.com/en-us/azure/key-vault/
* Access: In order to get secrets from Azure Key Vault, user/application must be authorized. User can be authorized via email address, but applications must be registered via [Azure Managed Service Identity](https://docs.microsoft.com/en-us/azure/app-service/app-service-managed-service-identity#creating-an-app-with-an-identity). All access policies should be added to ARM script (section Microsoft.KeyVault/vaults/accessPolicies).
* Azure Key Vault is additionally protected by a '[soft-delete](https://docs.microsoft.com/en-us/azure/key-vault/key-vault-ovw-soft-delete)' feature that allows to recover an entire resource in case of accidental deletion.

### Automation

Azure Resource Manager is used to automate environment creation. In order to create an environment from scratch, run: `Deploy.{envCode}.ps1` script that is located in `BurnForMoney.ResourceGroup` project. The script will ask only for credentials to Azure services and they can be retrieved from KeePass database. 

* Documentation: https://docs.microsoft.com/en-us/azure/azure-resource-manager/
* Snippets: https://github.com/Azure/azure-quickstart-templates





### Diagrams

#### User authorization - Strava

![User authorization - Strava](https://raw.githubusercontent.com//makingwaves/BurnForMoney/master/docs/User authorization - Strava.png)