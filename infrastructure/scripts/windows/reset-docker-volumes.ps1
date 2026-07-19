# Removes every container AND every volume for the local Docker stack.
# This permanently deletes all local data.
# Each file is run as its own separate `docker compose` invocation, torn
# down in the reverse order they're started in run-docker-all.ps1.
# Expected location: infrastructure\scripts\windows\reset-docker-volumes.ps1
# Compose files live in: infrastructure\docker\
# Usage: .\reset-docker-volumes.ps1 [-Force] (skip the confirmation prompt)
param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"

if (-not $Force) {
    $confirm = Read-Host "This will permanently delete all local data. Type 'yes' to continue"
    if ($confirm -ne "yes") {
        Write-Host "Aborted, no changes made."
        exit 0
    }
}

$DockerDir = Join-Path $PSScriptRoot "..\..\docker"

try {
    Set-Location -Path $DockerDir
} catch {
    Write-Host "Error: failed to change directory to $DockerDir"
    exit 1
}

Write-Host "Removing all services and volumes..."

docker compose -f docker-compose.observability.yaml down -v
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: failed to remove observability"
    exit $LASTEXITCODE
}

docker compose -f docker-compose.infra.yaml down -v
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: failed to remove infra"
    exit $LASTEXITCODE
}

Write-Host "All services and volumes removed successfully"