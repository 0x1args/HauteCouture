#!/usr/bin/env bash
# Starts only the core infrastructure.
# Expected location: infrastructure/scripts/unix/run-docker-infra.sh
# Compose files live in: infrastructure/docker/
set -uo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
DOCKER_DIR="$SCRIPT_DIR/../../docker"

if ! cd "$DOCKER_DIR"; then
    echo "Error: failed to change directory to $DOCKER_DIR"
    exit 1
fi

echo "Starting infra..."
if docker compose -f docker-compose.infra.yaml up -d; then
    echo "Infra started successfully"
else
    echo "Error: failed to start infra"
    exit 1
fi