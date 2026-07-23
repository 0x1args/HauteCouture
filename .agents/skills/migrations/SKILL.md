---
name: efcore-migrations
description: Guidance for managing EF Core migrations in .NET solution, project structure (dedicated Migrations host project per service), creating/applying migrations via the CLI and running migrations across environments.
globs: ["src/**/Migrations/**/*.cs", "src/**/*.csproj"]
---

## Purpose

This skill provides guidance for creating, organizing, and applying EF Core migrations in a modular, multi-tenant solution. It covers the recommended project layout (isolating migrations and design-time tooling into a dedicated per-module host project).

### 1. Project structure

For each service `<ServiceName>`, migrations live in a dedicated console/host project: `src/<ServiceName>/Hosts/Migrations/<YourSolution>.<ServiceName>.Hosts.Migrations.csproj`

#### Required packages in the Migrations project

- `Shared.Databases.Postgres.Migrations` — shared infrastructure (`IMigrationExecutor`, `AddMigrationExecutor<TContext>()`, common conventions).
- `Microsoft.Extensions.Hosting` — configures the project as a generic host, so it can build a `HostBuilder`, resolve DI, read configuration per-environment, and run as a standalone console app.
- `Microsoft.EntityFrameworkCore.Design` — required for `dotnet ef` design-time tooling (scaffolding, migration generation). This package should only live in the Migrations project, never in the main API/service runtime project, to avoid shipping design-time tooling in production binaries.

#### Why a separate project

- Keeps `dotnet ef` design-time dependencies out of the deployed application.
- Allows running migrations independently of the API (e.g., as a Kubernetes Job / init container, a CI/CD pipeline step, or manually per environment).
- Each module owns and versions its own migration history independently, consistent with modular/microservice boundaries.

### 2. Creating migrations

```bash
dotnet ef migrations add <MigrationName> \
  --context AppDbContext \
  --project src\Modules\<ModuleName>\Hosts\Migrations\<YourSolution>.Modules.<ModuleName>.Hosts.Migrations.csproj \
  --startup-project src\Modules\<ModuleName>\Hosts\Migrations\<YourSolution>.Modules.<ModuleName>.Hosts.Migrations.csproj \
  --output-dir Migrations
```

- `--context` — the `DbContext` the migration applies to; required whenever a project could contain (or reference) more than one context.
- `--project` — where the migration files (`.cs`, model snapshot) get written; should be the Migrations project itself.
- `--startup-project` — the project that gets built/run to provide configuration and DI context for the design-time model; typically the same Migrations project since it's configured as a host with its own connection-string/configuration setup.
- `--output-dir Migrations` — keeps migration files under a consistent, predictable folder for every module.

#### Naming conventions

- Use PascalCase, action-oriented names: `AddTenantIdToOrders`, `InitialCreate`, `CreateAppointmentsTable`.
- One logical schema change per migration where practical, easier to review, easier to roll back a single change if needed.
- Never rename or hand-edit an already-applied (in any shared environment) migration's `Up`/`Down`; create a new corrective migration instead.

#### Removing a migration

```bash
dotnet ef migrations remove --context AppDbContext --project <MigrationsProject> --startup-project <MigrationsProject>
```

Only safe if the migration has not been applied to any shared environment.

### 3. Registration & application at startup

#### DI registration

```csharp
var assembly = Assembly.GetExecutingAssembly();

services
    .AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(
            GetConnectionString(configuration),
            optionsBuilder => optionsBuilder.MigrationsAssembly(assembly));
    })
    .AddMigrationExecutor<AppDbContext>();
```

- `MigrationsAssembly(assembly)` is required whenever migrations live in a project other than the one containing the `DbContext` (which is the case here, migrations sit in the dedicated Migrations project).
- `AddMigrationExecutor<AppDbContext>()` registers the shared `IMigrationExecutor` abstraction, keeping the "how to apply migrations" logic centralized and consistent across modules instead of each module hand-rolling its own `context.Database.MigrateAsync()` call.

#### Applying migrations

Resolve `IMigrationExecutor` and call `MigrateAsync`, typically as an explicit step in the Migrations host's `Main`/startup pipeline, run once per deployment/environment rather than on every API instance boot:

```csharp
using var host = CreateHostBuilder(args).Build();

var executor = host.Services.GetRequiredService<IMigrationExecutor>();
await executor.MigrateAsync();
```

### 4. Practices for safe migration execution

- Do not delete or edit existing migrations; you may analyze their contents and report if the code generator made a mistake in the schema configuration, and you may also create a new migration that fixes an incorrect schema structure.
- Never run `EnsureCreated()` alongside migrations, they're mutually exclusive strategies; `EnsureCreated()` bypasses the migrations history table entirely and will desync schema tracking.
- Don't apply migrations from every API instance on startup in production. With multiple replicas (Kubernetes, load-balanced instances), concurrent `MigrateAsync()` calls can race. Instead:
  - Run migrations as a separate one-off step (CI/CD pipeline stage, Kubernetes `Job`/init container, or a dedicated migration invocation) before the API replicas start/scale up.
  - For local development / docker-compose single-instance setups, running `MigrateAsync()` at startup is acceptable for convenience.
- EF Core's migration history table (`__EFMigrationsHistory`) already tracks applied migrations, so re-running `MigrateAsync()` against an up-to-date database is a safe no-op, but avoid relying on this as a substitute for controlled deployment ordering.
- Backward compatibility during rollout: for zero-downtime deployments, prefer additive migrations first (add nullable columns, new tables) deployed *before* the code that depends on them, and destructive migrations (drop column, rename, tighten constraints) only after the old code path is fully retired — avoids a window where running app code doesn't match the current schema.
- Review generated SQL before applying to a shared/production environment:
```bash
  dotnet ef migrations script --context AppDbContext --project <MigrationsProject> --startup-project <MigrationsProject> --idempotent -o migration.sql
```
  The `--idempotent` flag wraps each migration in a check against the history table, making the script safe to re-run.