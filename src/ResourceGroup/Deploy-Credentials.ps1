function CreateKeyVault {
	Param(
		[string] [Parameter(Mandatory=$true)] $Environment,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupName,
		[string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
		[string] [Parameter(Mandatory=$true)] $KeyVaultName,
		[string] [Parameter(Mandatory=$true)] $UserAccountId
	)

	if (-not (Get-AzureRmKeyVault -VaultName $KeyVaultName))
	{
		Write-Host "Creating KeyVault: [$KeyVaultName]"
		New-AzureRmKeyVault -VaultName $KeyVaultName -ResourceGroupName $ResourceGroupName -Location $ResourceGroupLocation -EnabledForTemplateDeployment -EnableSoftDelete
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
		[string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
		[string] [Parameter(Mandatory=$true)] $UserAccountId
	)

	Write-Host "Deploying credentials"
	$KeyVaultName = "burnformoneykv" + $Environment.ToLower();
	CreateKeyVault -Environment $Environment `
					-ResourceGroupName $ResourceGroupName `
					-ResourceGroupLocation $ResourceGroupLocation `
					-KeyVaultName $KeyVaultName `
					-UserAccountId $UserAccountId

	Set-AzureRmKeyVaultAccessPolicy -VaultName $KeyVaultName -EmailAddress $UserAccountId -PermissionsToKeys decrypt,sign,get,unwrapKey -PermissionsToSecrets Get, Set, List, Delete

	$secrets = "sqlServerPassword", "stravaAccessTokensEncryptionKey", "stravaClientId", "stravaClientSecret", "sendGridApiKey"

	for ($i=0; $i -lt $secrets.length; $i++) {
		AddNewSecret -SecretName $secrets[$i] `
				-KeyVaultName $KeyVaultName
	}
}