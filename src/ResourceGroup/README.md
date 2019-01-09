> The project is responsible for the automated creation of Azure resources.

### How it works?

User need to run `Deploy.{environment}.ps1` script to bootstrap environment. Those scripts run `Deploy.ps1` script that can accept few parameters:

* Environment: `dev`, `test` and `prod` are accepted,
* Subscription name,
* DeployCredentials - defines whether credentials deployment should be included,
* DeployArm - defines whether ARM scripts should be deployed,
* UpgradeDatabase - defined whether SQL database migration should be included.

### Requirements
* [PowerShell](https://github.com/PowerShell/PowerShell). Linux/MacOs requires an additional installation of `Az` module is required. Open PowerShell console (`pwsh`), type `Install-Module -Name Az` and after installation is complete, enable AzureRM aliases: `Enable-AzureRmAlias`.
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest)

### How to deploy Azure resources?

* Open PowerShell console (`pwsh` in the terminal).
* Log in to your Azure account: `az login`.
* Run ARM script (`~\src\ResourceGroup`) for selected environment, e.g.: `_Deploy.Test.ps1`.

> Warning:
`.DeployCredentials.ps1` script is currently not supported by MacOs/Linux platform.


> The [copy](https://github.com/makingwaves/BurnForMoney/wiki/How-to-deploy-infrastructure) of this document should be available in the Wiki.