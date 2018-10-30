. "$PSScriptRoot\Utils.ps1"

function Add-IpRestriction {
	 Param(
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $false)] 
        $IpSecurityRestrictions,
		[Parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $false)] 
        $IpRestrictionName,
		[Parameter(Position = 2, Mandatory = $false, ValueFromPipeline = $false)] 
        $IpAddress,
        [Parameter(Position = 3, Mandatory = $true, ValueFromPipeline = $false)] 
        $APIVersion
    )

	if($IpSecurityRestrictions -eq $null){
		$IpSecurityRestrictions = @()
	}

	if (-not $IpAddress)
	{
		$IpAddress = Read-Host -Prompt "Input IP address of the $IpRestrictionName in the CIDR format, e.g.: 51.144.182.8/32"
	}

	$ips = $WebAppConfig.Properties.ipSecurityRestrictions
	$ips += @([PSCustomObject] @{ name = $IpRestrictionName; ipAddress = [string]$IpAddress; subnetMask = $null; priority = 100 })
    $WebAppConfig.properties.ipSecurityRestrictions = $ips

    Write-Status "Adding a new IP restriction... "
    $WebAppConfig | Set-AzureRmResource  -ApiVersion $APIVersion -Force | Out-Null
    Write-Succeed
    Write-Host "New allowed IP address $IpAddress has been added."
}

function Set-WebAppIPRestrictions {
    Param(
        [Parameter(Position = 0, Mandatory = $true, HelpMessage = "Api App name", ValueFromPipeline = $false)] 
        $ApiAppName,
		[Parameter(Position = 1, Mandatory = $true, HelpMessage = "Environment", ValueFromPipeline = $false)] 
        $Environment,
        [Parameter(Position = 2, Mandatory = $true, HelpMessage = "Resource group name", ValueFromPipeline = $false)] 
        $ResourceGroupName
    )
              
    If (!(Get-AzureRmContext)) {
        Write-Host "Please login to your Azure account"
        Login-AzureRmAccount
    }
 
    $APIVersion = ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).ApiVersions[0]
    $WebAppConfig = (Get-AzureRmResource -ResourceType Microsoft.Web/sites/config -ResourceName $ApiAppName -ResourceGroupName $ResourceGroupName -ApiVersion $APIVersion)
    $IpSecurityRestrictions = $WebAppConfig.Properties.ipSecurityRestrictions
 
	$IpRestrictionName = ('burnformoney-' + $Environment.ToLower())
    if ($IpSecurityRestrictions -and ($IpRestrictionName -in $IpSecurityRestrictions.name)) {
        "IP address restriction for application $IpRestrictionName is already added as allowed to access $ApiAppName."         
    }
    else {
		Add-IpRestriction -IpSecurityRestrictions $IpSecurityRestrictions -IpRestrictionName $IpRestrictionName -APIVersion $ApiVersion 
    }

	$IpRestrictionName = "MakingWavesOffice"
	if ($IpSecurityRestrictions -and ("MakingWavesOffice" -in $IpSecurityRestrictions.name)) {
        "IP address restriction for $IpRestrictionName is already added as allowed to access $ApiAppName."         
    }
    else {
		Add-IpRestriction -IpSecurityRestrictions $IpSecurityRestrictions -IpRestrictionName $IpRestrictionName -APIVersion $ApiVersion -IpAddress "89.174.102.0/26"
    }
}