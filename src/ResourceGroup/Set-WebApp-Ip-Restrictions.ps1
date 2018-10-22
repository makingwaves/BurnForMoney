. "$PSScriptRoot\Utils.ps1"

function Set-WebAppIPRestrictions {
    Param(
        [Parameter(Position = 0, Mandatory = $true, HelpMessage = "Api App name", ValueFromPipeline = $false)] 
        $ApiAppName,
		[Parameter(Position = 1, Mandatory = $true, HelpMessage = "Web App name", ValueFromPipeline = $false)] 
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
 
    if ($IpSecurityRestrictions -and ($ReactAppName -in $IpSecurityRestrictions.name)) {
        "IP address restriction for application $ReactAppName is already added as allowed to $ApiAppName."         
    }
    else {
		if($IpSecurityRestrictions -eq $null){
			$IpSecurityRestrictions = @()
		}

		$IPAddress = Read-Host -Prompt "Input IP address of the $ReactAppName app service in the CIDR format, e.g.: 51.144.182.8/32"

		$ips = $WebAppConfig.Properties.ipSecurityRestrictions
		$ips += @([PSCustomObject] @{ name = $ReactAppName; ipAddress = [string]$IPAddress; subnetMask = $null; priority = 100 })
        $WebAppConfig.properties.ipSecurityRestrictions = $ips

        Write-Status "Adding a new IP restriction... "
        $WebAppConfig | Set-AzureRmResource  -ApiVersion $APIVersion -Force | Out-Null
        Write-Succeed
        Write-Host "New allowed IP address $IPAddress has been added to WebApp $ApiAppName"
    }
}