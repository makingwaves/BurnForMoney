. "$PSScriptRoot\Upgrade-Database.ps1"

Upgrade-Database -ConnectionString "Data Source=(LocalDB)\.;Initial Catalog=BurnForMoney;Integrated Security=True" -ScriptsPath "$PSScriptRoot\SqlScripts\"