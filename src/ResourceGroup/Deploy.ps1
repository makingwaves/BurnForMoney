#Requires -Version 3.0

Param(
    [switch] $ValidateOnly,
	[string] [Parameter(Mandatory=$true)] $Environment,
	[string] [Parameter(Mandatory=$true)] $SubscriptionName
)

. "$PSScriptRoot\Deploy-Credentials.ps1"
. "$PSScriptRoot\Upgrade-Database.ps1"
. "$PSScriptRoot\Utils.ps1"
. "$PSScriptRoot\Set-WebApp-Ip-Restrictions.ps1"

try {
    [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.AddUserAgent("VSAzureTools-$UI$($host.name)".replace(' ','_'), '3.0.0')
} catch { }

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3

function Format-ValidationOutput {
    param ($ValidationOutput, [int] $Depth = 0)
    Set-StrictMode -Off
    return @($ValidationOutput | Where-Object { $_ -ne $null } | ForEach-Object { @('  ' * $Depth + ': ' + $_.Message) + @(Format-ValidationOutput @($_.Details) ($Depth + 1)) })
}

$OptionalParameters = New-Object -TypeName Hashtable

$ResourceGroupName = "BurnForMoney-$Environment";
$ResourceGroupLocation= 'West Europe';
$TemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "Template.json"))
$TemplateParametersFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "Template.$Environment.parameters.json"))

Write-Status "Selecting Azure subscription... "
try {
	Select-AzureRmSubscription -SubscriptionName  $SubscriptionName
}
catch{
	Login-AzureRmAccount;
	Select-AzureRmSubscription -SubscriptionName  $SubscriptionName
}
Write-Succeed

# Create or update the resource group using the specified template file and template parameters file
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force

if ($ValidateOnly) {
	Write-Status "Running Azure deployment in ValidateOnly mode... "
    $ErrorMessages = Format-ValidationOutput (Test-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName `
                                                                                  -TemplateFile $TemplateFile `
                                                                                  -TemplateParameterFile $TemplateParametersFile `
                                                                                  @OptionalParameters)
    if ($ErrorMessages) {
		Write-Succeed
        Write-Output '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.'
    }
    else {
		Write-Fail
        Write-Output '', 'Template is valid.'
    }
}

else {
	$KeyVaultName = "burnformoneykv" + $Environment.ToLower();

	DeployCredentials -Environment $Environment `
		-ResourceGroupName $ResourceGroupName `
		-ResourceGroupLocation $ResourceGroupLocation `
		-KeyVaultName $KeyVaultName
	
	Write-Status "Processing a new group deployment... "
    New-AzureRmResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                       -ResourceGroupName $ResourceGroupName `
                                       -TemplateFile $TemplateFile `
                                       -TemplateParameterFile $TemplateParametersFile `
                                       @OptionalParameters `
                                       -Force -Verbose `
                                       -ErrorVariable ErrorMessages
	Write-Succeed

    if (-Not $ErrorMessages) {
		$connectionStringSecret = Get-AzureKeyVaultSecret -VaultName $KeyVaultName -Name "SQLConnectionString"
		Upgrade-Database -ConnectionString $connectionStringSecret.SecretValueText -ScriptsPath "$PSScriptRoot\SqlScripts\"

		Set-WebAppIPRestrictions -ApiAppName ('burnformoneyfunc-' + $Environment.ToLower()) -ReactAppName ('burnformoney-' + $Environment.ToLower()) -ResourceGroupName $ResourceGroupName
    }
	else {
		Write-Fail
        Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
	}
}

Read-Host "Press ENTER to continue"