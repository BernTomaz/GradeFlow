param(
    [string]$ProjectName = "gradeflowsecure",
    [string]$OutputDirectory = "artifacts/backups"
)

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$container = "${ProjectName}-sqlserver-1"
$backupDir = "/var/opt/mssql/backup"
$backupFile = "GradeFlow-$timestamp.bak"
$saPassword = $env:MSSQL_SA_PASSWORD

if (-not $saPassword -and (Test-Path ".env")) {
    $saPassword = (Get-Content ".env" | Where-Object { $_ -like "MSSQL_SA_PASSWORD=*" } | Select-Object -First 1) -replace "^MSSQL_SA_PASSWORD=", ""
}

if (-not $saPassword) {
    throw "MSSQL_SA_PASSWORD not found in environment or .env."
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
docker exec $container mkdir -p $backupDir
docker exec $container /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $saPassword -C -Q "BACKUP DATABASE [GradeFlow] TO DISK = N'$backupDir/$backupFile' WITH INIT, COMPRESSION"
docker cp "${container}:${backupDir}/${backupFile}" (Join-Path $OutputDirectory $backupFile)

Write-Host "Backup saved to $(Join-Path $OutputDirectory $backupFile)"
