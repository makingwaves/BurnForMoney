$SubscriptionName = "EmpBranding BFM MS Websites PL (45779)"
$Environment = "Dev"
Invoke-Expression "$PSScriptRoot\Deploy.ps1 -Environment '$Environment' -SubscriptionName '$SubscriptionName' -UpgradeDatabase 0"
Invoke-Expression "$PSScriptRoot\Upgrade-Local-Database.ps1"
