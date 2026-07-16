param(
    [string]$Output = "artifacts/database/gradeflow-migrations.sql"
)

$outputDirectory = Split-Path -Parent $Output
if ($outputDirectory) {
    New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
}

dotnet build src/GradeFlow.Api/GradeFlow.Api.csproj -m:1

dotnet ef migrations script `
    --idempotent `
    --no-build `
    --project src/GradeFlow.Infrastructure `
    --startup-project src/GradeFlow.Api `
    --output $Output
