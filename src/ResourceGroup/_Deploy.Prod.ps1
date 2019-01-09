$SubscriptionName = "EmpBranding BFM MS Websites PL (45779)"
$Environment = "Prod"
Invoke-Expression "$PSScriptRoot\Deploy.ps1 -Environment '$Environment' -SubscriptionName '$SubscriptionName' -DeployCredentials 0 -UpgradeDatabase 0"