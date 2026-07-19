#!/usr/bin/env bash
# Starts only the observability stack.
# Expected location: infrastructure/scripts/unix/run-docker-observability.sh
# Compose files live in: infrastructure/docker/
set -uo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
DOCKER_DIR="$SCRIPT_DIR/../../docker"

if ! cd "$DOCKER_DIR"; then
    echo "Error: failed to change directory to $DOCKER_DIR"
    exit 1
fi

echo "Starting observability..."
if docker compose -f docker-compose.observability.yaml up -d; then
    echo "Observability started successfully"
else
    echo "Error: failed to start observability"
    exit 1
fi