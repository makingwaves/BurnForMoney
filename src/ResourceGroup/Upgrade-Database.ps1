. "$PSScriptRoot\Utils.ps1"

function Upgrade-Database {
	Param(
		[string] [Parameter(Mandatory=$true)] $ConnectionString,
		[string] [Parameter(Mandatory=$true)] $ScriptsPath
	)

	$dbUpLocation = "$PSScriptRoot/packages/dbup"
    $dbUpDllLocation = "$dbUpLocation/dll"

	Recreate-DbUp-Directories -DbUpLocation $dbUpLocation -DbUpDllLocation $dbUpDllLocation
	$dbUpCoreLocation = Get-DbUp -PackageName "dbup-core" -DbUpLocation $dbUpLocation -DbUpDllLocation $dbUpDllLocation
	$dbUpSqlServerLocation = Get-DbUp -PackageName "dbup-sqlserver" -DbUpLocation $dbUpLocation -DbUpDllLocation $dbUpDllLocation

	Add-Type -Path $dbUpCoreLocation
	Add-Type -Path $dbUpSqlServerLocation

	$dbUp = [DbUp.DeployChanges]::To
	$dbUp = [SqlServerExtensions]::SqlDatabase($dbUp, $ConnectionString)
	$dbUp = [StandardExtensions]::WithScriptsFromFileSystem($dbUp, $ScriptsPath)
	$dbUp = [SqlServerExtensions]::JournalToSqlTable($dbUp, 'dbo', 'SchemaVersions')
	$dbUp = [StandardExtensions]::LogToConsole($dbUp)

	Write-Status "Upgrading database... "
	$upgradeResult = $dbUp.Build().PerformUpgrade()
	Write-Succeed
}

function Create-Directory-If-Does-Not-Exists {
	Param(
		[string] [Parameter(Mandatory=$true)] $DirectoryPath
	)
	If(!(Test-Path $DirectoryPath))
	{
		New-Item -ItemType Directory -Force -Path $DirectoryPath
	}
}

function Recreate-DbUp-Directories {
	Param(
		[string] [Parameter(Mandatory=$true)] $DbUpLocation,
		[string] [Parameter(Mandatory=$true)] $DbUpDllLocation
	)
	Write-Status "Deleting old packages... "
    Remove-Item $DbUpLocation -Force -Recurse -ErrorAction SilentlyContinue
    Write-Succeed

	Write-Status "Creating a new directory structure... "
	Create-Directory-If-Does-Not-Exists -DirectoryPath $DbUpLocation
	Create-Directory-If-Does-Not-Exists -DirectoryPath $DbUpDllLocation
    Write-Succeed
}


function Get-DbUp {
    [CmdletBinding()]
    Param(
		[string] [Parameter(Mandatory=$true)] $PackageName,
		[string] [Parameter(Mandatory=$true)] $DbUpLocation,
		[string] [Parameter(Mandatory=$true)] $DbUpDllLocation,
        $NugetUrl = "https://www.nuget.org/api/v2/package/"
    )

    try{
        Write-Status "Downloading package... "
        Invoke-WebRequest "$NugetUrl/$PackageName" -OutFile "$DbUpLocation/$PackageName.zip"
        Write-Succeed
        
        Write-Status "Expand archive... " 
        Expand-Archive "$DbUpLocation/$PackageName.zip" -DestinationPath $DbUpDllLocation -Force
        Write-Succeed

        Write-Status "Locating package... "
        $packagePath = Get-ChildItem -Path $DbUpDllLocation -Filter "$PackageName.dll" -Recurse -ErrorAction SilentlyContinue -Force |
                    Select-Object -First 1 | 
                    ForEach-Object { $_.FullName } |
					Select-Object -Last 1
        if (!$packagePath) {
            throw [System.IO.FileNotFoundException] "$PackageName location cannot be found"
        }
        Write-Succeed
        
		Write-Host "Found $PackageName location: $packagePath"
        return $packagePath;
    }
    Catch {
        Write-Fail
        throw
    }
}