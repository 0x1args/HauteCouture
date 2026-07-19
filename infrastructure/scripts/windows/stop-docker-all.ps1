# Stops every local-dev compose file.
# Each file is run as its own separate `docker compose` invocation, stopped
# in the reverse order they're started in run-docker-all.ps1.
# Expected location: infrastructure\scripts\windows\stop-docker-all.ps1
# Compose files live in: infrastructure\docker\
$ErrorActionPreference = "Stop"

$DockerDir = Join-Path $PSScriptRoot "..\..\docker"

try {
    Set-Location -Path $DockerDir
} catch {
    Write-Host "Error: failed to change directory to $DockerDir"
    exit 1
}

Write-Host "Stopping all services..."

docker compose -f docker-compose.observability.yaml down
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: failed to stop observability"
    exit $LASTEXITCODE
}

docker compose -f docker-compose.infra.yaml down
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: failed to stop infra"
    exit $LASTEXITCODE
}

Write-Host "All services stopped successfully"