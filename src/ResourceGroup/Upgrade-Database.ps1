. "$PSScriptRoot\Utils.ps1"

function Upgrade-Database {
	Param(
		[string] [Parameter(Mandatory=$true)] $ConnectionString,
		[string] [Parameter(Mandatory=$true)] $ScriptsPath
	)

	$dbUpLocation = Get-DbUp

    Write-Host "Found location of DbUp library: $dbUpLocation"

	Add-Type -Path $dbUpLocation

	$dbUp = [DbUp.DeployChanges]::To
	$dbUp = [SqlServerExtensions]::SqlDatabase($dbUp, $ConnectionString)
	$dbUp = [StandardExtensions]::WithScriptsFromFileSystem($dbUp, $ScriptsPath)
	$dbUp = [SqlServerExtensions]::JournalToSqlTable($dbUp, 'dbo', 'SchemaVersions')
	$dbUp = [StandardExtensions]::LogToConsole($dbUp)

	Write-Status "Upgrading database... "
	$upgradeResult = $dbUp.Build().PerformUpgrade()
	Write-Succeed
}

function Get-DbUp {
    [CmdletBinding()]
    param
    (
        $Url="https://www.nuget.org/api/v2/package/dbup-core/"
    )

    $dbUpTempPath ="$PSScriptRoot\dll\dbup"
    $dbUpZipLocation = "$PSScriptRoot\dll\DbUp.zip"

    try{
        Write-Status "Deleting old packages... "
        Remove-Item $dbUpZipLocation -Force -ErrorAction SilentlyContinue
        Remove-Item $dbUpTempPath -Force -Recurse -ErrorAction SilentlyContinue
        Write-Succeed

        Write-Status "Downloading package... "
        Invoke-WebRequest $url -OutFile $dbUpZipLocation
        Write-Succeed
        
        Write-Status "Expand archive... " 
        Expand-Archive $dbUpZipLocation -DestinationPath $dbUpTempPath -Force
        Write-Succeed

        Write-Status "Locating DbUp... "
        $dbupPath = Get-ChildItem -Path $dbUpTempPath -Filter "dbup-core.dll" -Recurse -ErrorAction SilentlyContinue -Force |
                    Select-Object -First 1  | 
                    ForEach-Object { $_.FullName }
        if (!$dbupPath) {
            throw [System.IO.FileNotFoundException] "DbUp.dll location cannot be found"
        }
        Write-Succeed
        
        return $dbupPath;
    }
    Catch {
        Write-Fail
        throw
    }
}