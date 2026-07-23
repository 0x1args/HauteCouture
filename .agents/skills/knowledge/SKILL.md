---
name: hautecouture-knowledge
description: Entry-point knowledge skill for the HauteCouture project. Project is a multi-tenant SaaS platform for service-based businesses built on microservices, DDD, Clean Architecture, and CQRS
globs: ["**/*"]
---

## Purpose

HauteCouture is a personal project: a multi-tenant SaaS platform for service-based businesses (salons, workshops, retail shops, etc.), built to publicly showcase clean microservice architecture rather than as a production product. This skill orients an agent quickly, what the project is, its stack, and where to look for details, without duplicating the project's own docs.

Always prefer reading the actual referenced doc for specifics (endpoint details, DI wiring, per-service behavior) — this skill is an index, not a replacement.

### Core facts

- Stack: ASP.NET Core / .NET 10, YARP gateway, PostgreSQL (write) + MongoDB (read, CQRS), EF Core 10, FluentValidation, Redis, Kafka + MassTransit, Keycloak, Stripe, MinIO, Serilog/Seq/OpenTelemetry/Grafana/Prometheus/Jaeger, xUnit/Testcontainers/Respawn, Docker/Docker Compose (+ Kubernetes, Terraform dirs).
- Patterns: Microservices, DDD, Clean Architecture, CQRS, Event-Driven Architecture, Outbox/Inbox, Saga, Circuit Breaker, API Gateway.
- Docs language: English (public GitHub audience). Conversation with the maintainer is in Ukrainian.

### Where to find things

Central index: `docs/docs-structure.md`, always the first stop for navigating documentation.

| Need | Location |
|---|---|
| Big-picture architecture, diagrams, sync/async interaction, security topology | `docs/architecture.md` *(not written yet)* |
| Baseline structure/recommendations every service must follow | `docs/service-structure.md` |
| Per-service specifics (purpose, domain, endpoints, stack, jobs, dependencies) | `<Service>/HostSide/README.md` (per service) |
| Cross-cutting reusable packages map | `src/Shared/README.md` |
| Individual `Shared` package details | `src/Shared/<Layer>/Base/README.md` |
| Tests (Unit/Integration, coverage tracking) | `tests/README.md` |
| Local Docker setup | `infrastructure/docker/README.md` |
| Kubernetes manifests | `infrastructure/kubernetes/README.md` *(not written yet)* |
| Dev automation scripts | `infrastructure/scripts/README.md` |
| Terraform/IaC | `infrastructure/terraform/README.md` *(not written yet)* |
| AI agent skills/personas | `/.agents/skills/`, `/.agents/agents/`, entry point `AGENTS.md` |

### Shared module map (`src/Shared`)

- `Common/Authorization` — `ICurrentUserSession`, shared `UserRole`, provider-agnostic (Keycloak wired by host app).
- `Common/Exceptions` — unified Client/Server/Integration exception families feeding one exception-handling middleware.
- `Common/Pagination` — `PagedFilter`, `OffsetPagedList`, `CursorPagedList`.
- `CQS/*` — CQS on MediatR; pipeline behaviors (Diagnostic, Authorization, Validation, Logging, Performance, Caching, Transactional); `ICommandHandler`/`IQueryHandler` from `CQS.Abstractions`; this is the **Applications.Handlers** layer.
- `Databases/Postgres/*` — pooled `DbContext` via `AddPostgres`, `IRepository<TEntity, TId>`, `PageAsync`/`ToKeysetPageAsync`, `ITransactionalScope`, snake_case + `DateTimeKind` conventions; `Migrations` subproject backs each service's **HostSide.Migrations** host.
- `Domain` — `AuditableEntity<TId>`, `ISoftDeletable`, base `DomainException`; referenced only by a service's own `Domain` layer.
- `WebApi` — `IWebModule` pipeline (Caching, Logging, Correlation, ExceptionHandling, TrafficControl, HealthChecks, OpenTelemetry, Authorization, Swagger), `IEndpoint`/`AddEndpoints`/`MapEndpoints`, `WithRateLimit`/`WithThrottle`; backs **HostSide.WebApi**.

### Conventions to respect when generating code or docs

- Before adding new cross-cutting logic to a service, check whether `Shared` already solves it.
- `Shared` packages split into **Abstractions** (contracts a service depends on) and **Base** (implementation); docs live in **Base**.
- Per-service docs live at the **HostSide** level and must cover: purpose/responsibilities, domain overview, external-facing endpoints, internal-only endpoints, tech stack, background jobs/consumers, third-party libraries, external dependencies.
- File naming: `.yaml` not `.yml`; watch for the `docs-structure.md` typo variant `dosc-structure.md`.
- Docs written in English; explanatory/reference tone, not tutorial/onboarding.