#!/usr/bin/env bash
# Removes every container AND every volume for the local Docker stack.
# This permanently deletes all local data.
# Each file is run as its own separate `docker compose` invocation, torn
# down in the reverse order they're started in run-docker-all.sh.
# Expected location: infrastructure/scripts/unix/reset-docker-volumes.sh
# Compose files live in: infrastructure/docker/
# Usage: ./reset-docker-volumes.sh [-y|--force] (skip the confirmation prompt)
set -uo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
DOCKER_DIR="$SCRIPT_DIR/../../docker"

FORCE=false
for arg in "$@"; do
    case "$arg" in
        -y|--force) FORCE=true ;;
    esac
done

if [ "$FORCE" != true ]; then
    read -r -p "This will permanently delete all local data. Type 'yes' to continue: " CONFIRM
    if [ "$CONFIRM" != "yes" ]; then
        echo "Aborted, no changes made."
        exit 0
    fi
fi

if ! cd "$DOCKER_DIR"; then
    echo "Error: failed to change directory to $DOCKER_DIR"
    exit 1
fi

echo "Removing all services and volumes..."

if ! docker compose -f docker-compose.observability.yaml down -v; then
    echo "Error: failed to remove observability"
    exit 1
fi

if ! docker compose -f docker-compose.infra.yaml down -v; then
    echo "Error: failed to remove infra"
    exit 1
fi

echo "All services and volumes removed successfully"