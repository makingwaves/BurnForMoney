function Set-WebAppIPRestrictions {
    Param(
        [Parameter(Position = 0, Mandatory = $true, HelpMessage = "WebApp name", ValueFromPipeline = $false)] 
        $ApiAppName,
		[Parameter(Position = 1, Mandatory = $true, HelpMessage = "WebApp name", ValueFromPipeline = $false)] 
        $ReactAppName,
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
 
	Write-Output $IpSecurityRestrictions
	Write-Output ($IpSecurityRestrictions -eq -not $null)
    if ($IpSecurityRestrictions -and ($ReactAppName -in $IpSecurityRestrictions.name)) {
        "IP address restriction for application $ReactAppName is already added as allowed to $ApiAppName."         
    }
    else {
		if($IpSecurityRestrictions -eq $null){
			$IpSecurityRestrictions = @()
		}

		$IPAddress = Read-Host -Prompt "Input IP address of the $ReactAppName app service, e.g.: 51.144.182.8/32"

		$ips = $WebAppConfig.Properties.ipSecurityRestrictions
		$ips += @([PSCustomObject] @{ name = $ReactAppName; ipAddress = [string]$IPAddress; subnetMask = $null })
        $WebAppConfig.properties.ipSecurityRestrictions = $ips

        $WebAppConfig | Set-AzureRmResource  -ApiVersion $APIVersion -Force | Out-Null
        Write-Host "New restricted IP address $IPAddress has been added to WebApp $ApiAppName"
    }
}