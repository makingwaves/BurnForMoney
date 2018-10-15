$SubscriptionName = "Making Waves - Search and Collaboration O365"
$Environment = "Prod"
Invoke-Expression "$PSScriptRoot\Deploy.ps1 -Environment '$Environment' -SubscriptionName '$SubscriptionName'"