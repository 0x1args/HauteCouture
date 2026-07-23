# Creates a default .env file for local development.
# Expected location: infrastructure\scripts\windows\create-env.ps1
# Output file: infrastructure\docker\.env

$ErrorActionPreference = "Stop"

$DockerDir = Join-Path $PSScriptRoot "..\..\docker"
$EnvFile = Join-Path $DockerDir ".env"

try {
    Set-Location -Path $DockerDir
} catch {
    Write-Host "Error: failed to change directory to $DockerDir"
    exit 1
}

if (Test-Path $EnvFile) {
    Write-Host ".env already exists"
    exit 0
}

@"
# Postgres
POSTGRES_DB=hautecouture
POSTGRES_USER=postgres
POSTGRES_PASSWORD=p4ssw0rd

# pgAdmin
PGADMIN_DEFAULT_EMAIL=admin@noreply.com
PGADMIN_DEFAULT_PASSWORD=postgres

# Redis
REDIS_PASSWORD=p4ssw0rd
REDIS_PORT=6379

# Seq
SEQ_ADMIN_PASSWORD=p4ssw0rd!
"@ | Set-Content -Path $EnvFile -Encoding UTF8

Write-Host ".env created successfully"