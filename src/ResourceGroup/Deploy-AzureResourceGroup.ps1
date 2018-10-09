#Requires -Version 3.0

Param(
    [switch] $ValidateOnly,
	[string] [Parameter(Mandatory=$true)] $Environment
)

. "$PSScriptRoot\Deploy-Credentials.ps1"
. "$PSScriptRoot\Upgrade-Database.ps1"

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

$SubscriptionName = "Making Waves - Search and Collaboration O365";
$ResourceGroupName = "BurnForMoney-$Environment";
$ResourceGroupLocation= 'West Europe';
$TemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "Template.json"))
$TemplateParametersFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "Template.$Environment.parameters.json"))

try {
	Select-AzureRmSubscription -SubscriptionName  $SubscriptionName
}
catch{
	Login-AzureRmAccount;
	Select-AzureRmSubscription -SubscriptionName  $SubscriptionName
}

# Create or update the resource group using the specified template file and template parameters file
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force

if ($ValidateOnly) {
    $ErrorMessages = Format-ValidationOutput (Test-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName `
                                                                                  -TemplateFile $TemplateFile `
                                                                                  -TemplateParameterFile $TemplateParametersFile `
                                                                                  @OptionalParameters)
    if ($ErrorMessages) {
        Write-Output '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.'
    }
    else {
        Write-Output '', 'Template is valid.'
    }
}
else {
	DeployCredentials -Environment $Environment `
		-ResourceGroupName $ResourceGroupName `
		-ResourceGroupLocation $ResourceGroupLocation
	

    New-AzureRmResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                       -ResourceGroupName $ResourceGroupName `
                                       -TemplateFile $TemplateFile `
                                       -TemplateParameterFile $TemplateParametersFile `
                                       @OptionalParameters `
                                       -Force -Verbose `
                                       -ErrorVariable ErrorMessages

    if ($ErrorMessages) {
        Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
    }
	else {
		$keyVaultName = "burnformoneykv" + $Environment.ToLower()
		$connectionStringSecret = Get-AzureKeyVaultSecret -VaultName $keyVaultName -Name "SQLConnectionString"

		Upgrade-Database -ConnectionString $connectionStringSecret.SecretValueText -ScriptsPath "$PSScriptRoot\SqlScripts\"
	}
}

Read-Host "Press ENTER to continue"