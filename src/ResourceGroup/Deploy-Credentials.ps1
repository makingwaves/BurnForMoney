. "$PSScriptRoot\Utils.ps1"

function CreateKeyVault {
	Param(
		[string] [Parameter(Mandatory=$true)] $Environment,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupName,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
		[string] [Parameter(Mandatory=$true)] $KeyVaultName
	)

	if (-not (Get-AzureRmKeyVault -VaultName $KeyVaultName))
	{
		Write-Status "Creating KeyVault: [$KeyVaultName]... "
		New-AzureRmKeyVault -VaultName $KeyVaultName -ResourceGroupName $ResourceGroupName -Location $ResourceGroupLocation -EnabledForTemplateDeployment -EnableSoftDelete
		Write-Succeed
	}
}

function AddNewSecret {
	Param(
		[string] [Parameter(Mandatory=$true)] $SecretName,
		[string] [Parameter(Mandatory=$true)] $KeyVaultName
	)

	if (-not (Get-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $SecretName))
	{
		Write-Status "Adding a new secret [$SecretName]... "
		$Credentials = Get-Credential -Message "Provide password for $SecretName [put anything as an username]. Password can be found in the KeePass database."
		Set-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $SecretName -SecretValue $Credentials.Password
		Write-Succeed
	}
}

function DeployCredentials {
	Param(
		[string] [Parameter(Mandatory=$true)] $Environment,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupName,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
		[string] [Parameter(Mandatory=$true)] $KeyVaultName
	)

	try {
		Write-Host "Deploying credentials..."
		CreateKeyVault -Environment $Environment `
						-ResourceGroupName $ResourceGroupName `
						-ResourceGroupLocation $ResourceGroupLocation `
						-KeyVaultName $KeyVaultName

		$accountId = (Get-AzureRmContext).Account.Id
		Write-Status "Setting KeyVault access policy for user: $accountId"
		Set-AzureRmKeyVaultAccessPolicy -VaultName $KeyVaultName -EmailAddress $accountId -PermissionsToKeys decrypt,sign,get,unwrapKey -PermissionsToSecrets Get, Set, List, Delete
		Write-Succeed

		$secrets = "sqlServerPassword", "stravaAccessTokensEncryptionKey", "stravaClientId", "stravaClientSecret", "sendGridApiKey"

		for ($i=0; $i -lt $secrets.length; $i++) {
			AddNewSecret -SecretName $secrets[$i] `
					-KeyVaultName $KeyVaultName
		}
	}
    Catch {
        Write-Fail
        throw
    }
}