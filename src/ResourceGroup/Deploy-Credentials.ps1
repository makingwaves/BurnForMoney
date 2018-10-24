. "$PSScriptRoot\Utils.ps1"

[Reflection.Assembly]::LoadWithPartialName("System.Web")

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
		[string] [Parameter(Mandatory=$true)] $KeyVaultName,
		[Security.SecureString] [Parameter(Mandatory=$false)] $Password
	)

	if (-not (Get-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $SecretName))
	{
		Write-Status "Adding a new secret [$SecretName]... "
		if (-not $Password)
		{
			$Credentials = Get-Credential -Message "Provide password for $SecretName [put anything as an username]."
			$Password = $Credentials.Password
		} 

		Set-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $SecretName -SecretValue $Password
		Write-Succeed
	}
}

function AddNewAccountSecret {
	Param(
		[string] [Parameter(Mandatory=$true)] $SecretName,
		[string] [Parameter(Mandatory=$true)] $KeyVaultName
	)

	$userNameSecretName = "$SecretName--username"
	$passwordSecretName = "$SecretName--password"

	if (-not (Get-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $userNameSecretName))
	{
		Write-Status "Adding a new secret [$SecretName]... "
		$Credentials = Get-Credential -Message "Provide account credentials for $SecretName."

		Set-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $userNameSecretName -SecretValue (ConvertTo-SecureString –String $Credentials.UserName –AsPlainText -Force)
		Set-AzureKeyVaultSecret -VaultName $KeyVaultName -Name $passwordSecretName -SecretValue $Credentials.Password
		Write-Succeed
	}
}

function GenerateEncryptionKey {
    param(
        $length = 32,
        $characters = ‘ABCDEFGHKLMNPRSTUVWXYZ1234567890’
    )
    $random = 1..$length | ForEach-Object { Get-Random -Maximum $characters.length }

    $private:ofs= “”
    $password = [String]$characters[$random]
    return $password
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

		$accounts = "accounts--gmail", "accounts--sendgrid", "accounts--strava", "accounts--contentful"
		for ($i=0; $i -lt $accounts.length; $i++) {
			AddNewAccountSecret -SecretName $accounts[$i] `
				-KeyVaultName $KeyVaultName
		}

		$userSecrets = "sendGrid--ApiKey", "strava--ClientId", "strava--clientSecret"
		for ($i=0; $i -lt $userSecrets.length; $i++) {
			AddNewSecret -SecretName $userSecrets[$i] `
				-KeyVaultName $KeyVaultName
		}

		AddNewSecret -SecretName "sqlServerPassword" `
			-KeyVaultName $KeyVaultName `
			-Password (ConvertTo-SecureString –String ([System.Web.Security.Membership]::GeneratePassword(20,5)) –AsPlainText -Force)

		AddNewSecret -SecretName "strava--AccessTokensEncryptionKey" `
			-KeyVaultName $KeyVaultName `
			-Password (ConvertTo-SecureString –String (GenerateEncryptionKey) –AsPlainText -Force)
	}
    Catch {
        Write-Fail
        throw
    }
}