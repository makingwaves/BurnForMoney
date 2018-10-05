function CreateKeyVault {
	Param(
		[string] [Parameter(Mandatory=$true)] $Environment,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupName,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
		[string] [Parameter(Mandatory=$true)] $KeyVaultName
	)

	if (-not (Get-AzureRmKeyVault -VaultName $KeyVaultName))
	{
		Write-Host "Creating KeyVault: [$KeyVaultName]"
		New-AzureRmKeyVault -VaultName $KeyVaultName -ResourceGroupName $ResourceGroupName -Location $ResourceGroupLocation -EnabledForTemplateDeployment -EnableSoftDelete

		Set-AzureRmKeyVaultAccessPolicy -VaultName $KeyVaultName -EmailAddress 'pawel.maga@makingwaves.com' -PermissionsToKeys decrypt,sign,get,unwrapKey -PermissionsToSecrets Get, Set, List, Delete
	}
}

function AddNewSecret {
	Param(
		[string] [Parameter(Mandatory=$true)] $SecretName,
		[string] [Parameter(Mandatory=$true)] $KeyVaultName
	)

	if (-not (Get-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $SecretName))
	{
		Write-Host "Adding a new secret [$SecretName]"
		$Credentials = Get-Credential -Message "Provide password for $SecretName [put anything as an username]. Password can be found in the KeePass database."
		Set-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $SecretName -SecretValue $Credentials.Password
	}
}

function DeployCredentials {
	Param(
		[string] [Parameter(Mandatory=$true)] $Environment,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupName,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupLocation
	)

	Write-Host "Deploying credentials"
	$KeyVaultName = "burnformoneykv" + $Environment.ToLower();
	CreateKeyVault -Environment $Environment `
					-ResourceGroupName $ResourceGroupName `
					-ResourceGroupLocation $ResourceGroupLocation `
					-KeyVaultName $KeyVaultName

	$secrets = "sqlServerPassword", "stravaAccessTokensEncryptionKey", "stravaClientId", "stravaClientSecret", "sendGridApiKey"

	for ($i=0; $i -lt $secrets.length; $i++) {
		AddNewSecret -SecretName $secrets[$i] `
				-KeyVaultName $KeyVaultName
	}
}