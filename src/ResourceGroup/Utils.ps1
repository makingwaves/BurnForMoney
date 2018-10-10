function Write-Status{
    [cmdletbinding()]
    param (
        [Parameter(Mandatory=$true)]
        [Object]$message
    )
    Write-Host "$message... " -NoNewline
}

function Write-Succeed{
    Write-Host "[Succeed]" -ForegroundColor Green
}

function Write-Fail{
    Write-Host "[Fail]" -ForegroundColor Red
}