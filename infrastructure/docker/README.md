## Docker

Docker and Docker Compose are used for containerization and local development. To keep each compose file focused and reasonably sized, and to have each one map to a clear category of concern, the setup is split into three files:

- **`docker-compose.infra.yaml`** — core infrastructure.
- **`docker-compose.observability.yaml`** — the observability stack.
- **`docker-compose.services.yaml`** — everything on the `HostSide` of a service: WebApi hosts, consumers, background jobs, and migrations.

### 1. Configuring secrets

Before starting anything, create an `.env` file in this folder (`infrastructure/docker`) with the values below (usernames, passwords, tokens, etc.):

#### 1.1 Postgres
```
POSTGRES_DB={your_db}
POSTGRES_USER={your_username}
POSTGRES_PASSWORD={your_password}
```

#### 1.2 pgAdmin
```
PGADMIN_DEFAULT_EMAIL={your_email}
PGADMIN_DEFAULT_PASSWORD={your_password}
```

#### 1.3 Redis
```
REDIS_PASSWORD={your_password}
```

#### 1.4 Seq
```
SEQ_ADMIN_PASSWORD={your_password}
```

### 2. Quick start

`infrastructure/scripts/` contains the scripts that simplify starting the stack, split by platform: `windows/` (PowerShell) and `unix/` (bash), use whichever matches your machine. See `infrastructure/scripts/README.md` for the full list and the conventions they follow; the ones relevant here are:

| Script | Starts |
|---|---|
| `run-all` | Everything: infra + observability (services once added) |
| `run-infra` | Only `docker-compose.infra.yaml` |
| `run-observability` | Only `docker-compose.observability.yaml` |

For most cases, running `run-all` is the right starting point (it brings up the whole local stack in one command).

**Documenation reference: https://github.com/0x1args/HauteCouture/blob/main/infrastructure/scripts/README.md**

### 3. Stack overview

| Service | Container | Image | Port(s) | Purpose |
|---|---|---|---|---|
| `postgres` | `hautecouture-postgres` | `postgres:17-alpine` | `5434 → 5432` | Primary relational database used by every service. |
| `pgadmin` | `hautecouture-pgadmin` | `dpage/pgadmin4:9.11` | `5050 → 80` | Web UI for inspecting and managing the Postgres instance. |
| `redis` | `hautecouture-redis` | `redis:8.0.4-alpine` | `6379 → 6379` | Distributed cache backing `Shared.WebApi`'s caching module and the CQS caching behavior. |
| `seq` | `hautecouture-seq` | `datalust/seq:2025.2` | `5341 → 80` | Structured log ingestion and UI (the Serilog sink every service logs to). |

### 4. Stopping the stack

To stop the containers, run the `stop-docker-all` script corresponding to your platform from `infrastructure/scripts`. If you need to stop all services and clear all volumes (local data), run the `reset-docker-volumes` script. This is destructive and permanent: it prompts for confirmation before proceeding, unless run with `-y`/`--force` (`-Force` on Windows).