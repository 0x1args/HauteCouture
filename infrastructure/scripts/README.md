## Scripts

`scripts` exists to automate the repetitive parts of working with the local environment, bringing up and tearing down the Docker stack described in `infrastructure/docker`, and any other setup or maintenance task that's cleaner as a single command than as a long-form one to remember and retype. It's a place for convenience: turning a multi-step or easy-to-get-wrong sequence into one script, once, so it doesn't have to be re-derived every time. This documenatiob is meant to be the centralized explanation for that place: what belongs here, how it's organized, and what any script added to it is expected to do, so the reasoning behind the tooling lives in one place rather than being spread across the scripts themselves.

### 1. Structure

Scripts are split by platform rather than kept as one folder, since a shell script and its PowerShell equivalent are two different files by necessity, not two variants of the same file:

```
scripts/
├── unix/       # .sh — macOS and Linux (bash)
└── windows/    # .ps1 — Windows (PowerShell)
```

Every script that exists for one platform is expected to have a matching script for the other, sharing the same base name (example: `run-infra.sh` / `run-infra.ps1`) and performing the same operation with the same observable behavior: the same inputs, the same success/failure output, the same exit-code semantics. The two folders should be read as one logical set of tools with two entry points, not as two independently evolving sets of tooling.

### 2. Conventions

Any script added here  is expected to follow the same handful of rules:

- Resolve paths relative to the script's own location.
- Never report success after a failure.
- Be safe to re-run.
- Keep status output minimal and consistent.
- Name scripts by the action they perform, using an imperative verb first (`run-`, `create-`, `reset-`, …) followed by what it acts on, so the intent is clear from the filename alone without opening the file.

### 3. Available scripts

| Script | Platform | Description |
|---|---|---|
| `run-all.sh` / `run-all.ps1` | unix / windows | Starts every part of the local Docker stack (`infra` + `observability`), each as its own `docker compose` project. |
| `run-infra.sh` / `run-infra.ps1` | unix / windows | Starts only the core infrastructure. |
| `run-observability.sh` / `run-observability.ps1` | unix / windows | Starts only the observability stack. |
| `stop-docker-all.sh` / `stop-docker-all.ps1` | unix / windows | Stops every part of the local Docker stack, in the reverse order `run-docker-all` starts them. |
| `reset-docker-volumes.sh` / `reset-docker-volumes.ps1` | unix / windows | Destructive. Stops every part of the stack and deletes its volumes, permanently erases all local data (Postgres tables, Seq logs, etc.). Prompts for confirmation unless run with `-y`/`--force` (`-Force` on Windows). |