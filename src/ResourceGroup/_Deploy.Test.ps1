$SubscriptionName = "EmpBranding BFM MS Websites PL (45779)"
$Environment = "Test"
Invoke-Expression "$PSScriptRoot\Deploy.ps1 -Environment '$Environment' -SubscriptionName '$SubscriptionName' -DeployCredentials 0 -UpgradeDatabase 0"