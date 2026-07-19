#!/usr/bin/env bash
# Starts every local-dev compose file.
# Each file is run as its own separate `docker compose` invocation.
# Expected location: infrastructure/scripts/unix/run-docker-all.sh
# Compose files live in: infrastructure/docker/
set -uo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
DOCKER_DIR="$SCRIPT_DIR/../../docker"

if ! cd "$DOCKER_DIR"; then
    echo "Error: failed to change directory to $DOCKER_DIR"
    exit 1
fi

echo "Starting all services..."

if ! docker compose -f docker-compose.infra.yaml up -d; then
    echo "Error: failed to start infra"
    exit 1
fi

if ! docker compose -f docker-compose.observability.yaml up -d; then
    echo "Error: failed to start observability"
    exit 1
fi

echo "All services started successfully"