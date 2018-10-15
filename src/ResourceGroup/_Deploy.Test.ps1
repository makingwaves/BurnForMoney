$SubscriptionName = "Making Waves - Search and Collaboration O365"
$Environment = "Test"
Invoke-Expression "$PSScriptRoot\Deploy.ps1 -Environment '$Environment' -SubscriptionName '$SubscriptionName'"