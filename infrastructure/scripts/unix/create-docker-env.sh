#!/usr/bin/env bash
# Creates a default .env file for local development.
# Expected location: infrastructure/scripts/unix/create-env.sh
# Output file: infrastructure/docker/.env

set -uo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
DOCKER_DIR="$SCRIPT_DIR/../../docker"
ENV_FILE="$DOCKER_DIR/.env"

if ! cd "$DOCKER_DIR"; then
    echo "Error: failed to change directory to $DOCKER_DIR"
    exit 1
fi

if [ -f "$ENV_FILE" ]; then
    echo ".env already exists"
    exit 0
fi

cat > "$ENV_FILE" <<EOF
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
EOF

echo ".env created successfully" 