$SubscriptionName = "Making Waves - Search and Collaboration O365"
$Environment = "Dev"
Invoke-Expression "$PSScriptRoot\Deploy.ps1 -Environment '$Environment' -SubscriptionName '$SubscriptionName' -UpgradeDatabase 0"
Invoke-Expression "$PSScriptRoot\Upgrade-Local-Database.ps1"
