#!/usr/bin/env bash
# Stops every local-dev compose file.
# Each file is run as its own separate `docker compose` invocation, stopped
# in the reverse order they're started in run-docker-all.sh.
# Expected location: infrastructure/scripts/unix/stop-docker-all.sh
# Compose files live in: infrastructure/docker/
set -uo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
DOCKER_DIR="$SCRIPT_DIR/../../docker"

if ! cd "$DOCKER_DIR"; then
    echo "Error: failed to change directory to $DOCKER_DIR"
    exit 1
fi

echo "Stopping all services..."

if ! docker compose -f docker-compose.observability.yaml down; then
    echo "Error: failed to stop observability"
    exit 1
fi

if ! docker compose -f docker-compose.infra.yaml down; then
    echo "Error: failed to stop infra"
    exit 1
fi

echo "All services stopped successfully"