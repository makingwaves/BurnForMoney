# Burn for money

### Installation

### Requirements

* [Azure Storage Emulator](https://go.microsoft.com/fwlink/?LinkId=717179&clcid=0x409)
* [PowerShell](https://www.microsoft.com/web/handlers/webpi.ashx/getinstaller/WindowsAzurePowershellGet.3f.3f.3fnew.appids)

### Azure functions

* Documentation: https://docs.microsoft.com/en-us/azure/azure-functions/


### Strava API

* Authorization: https://www.strava.com/oauth/authorize?client_id=25708&response_type=code&redirect_uri=http://localhost&approval_prompt=force
* Rate limits: 600 requests every 15 minutes, 30000 daily
* Documentation: https://developers.strava.com/docs/
* Authentication: https://developers.strava.com/docs/authentication/
* API: https://developers.strava.com/docs/reference/

### Safeguarding passwords

All passwords are stored in the [KeePass](https://keepass.info/) database, included in version control system. This database should store all credentials used by BurnForMoney projects (Azure services, APIs, etc.).

Application passwords are deployed by ARM scripts to [Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/) storage and should be retrieved only from this service. All apllications must be properly authorised to have an access to Key Vault services.

* Documentation: https://docs.microsoft.com/en-us/azure/key-vault/

### Automation

Azure Resource Manager is used to automate environment creation. In order to create an environment from scratch, run: `Deploy.{envCode}.ps1` script that is located in `BurnForMoney.ResourceGroup` project. The script will ask only for credentials to Azure services and they can be retrieved from KeePass database. 

* Documentation: https://docs.microsoft.com/en-us/azure/azure-resource-manager/
* Snippets: https://github.com/Azure/azure-quickstart-templates

