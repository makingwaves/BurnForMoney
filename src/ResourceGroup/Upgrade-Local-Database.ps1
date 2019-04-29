. "$PSScriptRoot\Upgrade-Database.ps1"

Upgrade-Database -ConnectionString "Data Source=localhost;Initial Catalog=BurnForMoney;Integrated Security=True" -ScriptsPath "$PSScriptRoot\SqlScripts\"