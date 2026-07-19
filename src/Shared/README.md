## Shared

`Shared` is the collection of cross-cutting, reusable packages consumed by every service in the system. It exists to keep the architecture flexible and to avoid re-implementing the same infrastructure and cross-cutting concerns (persistence access, request pipelines, authorization, exceptions, pagination, HTTP wiring) inside each service separately. Before implementing something new inside a service, always check whether it is already solved here. This document is a general, high-level map of everything under `src/Shared`. Each package below carries its own, more detailed documentation, this file exists so that map can be navigated without having to browse the folder tree first.

### Structure

```
src/Shared/
├── Common/
│   ├── Authorization/
│   ├── Exceptions/
│   └── Pagination/
├── CQS/
│   ├── Abstractions/
│   ├── Base/
│   └── Primitives/
├── Databases/
│   └── Postgres/
│       ├── Abstractions/
│       ├── Base/
│       └── Migrations/
├── Domain/
└── WebApi/
```

## Packages

### Common

`Common` is a family of small, focused, framework-agnostic packages shared across every layer of every service. 

#### Authorization

Provides the components required for authorization across the system, centered on `ICurrentUserSession` — the current authenticated user's context (user, tenant, roles, session, IP, user agent), available anywhere it's needed. It also defines the platform's shared `UserRole` enumeration. The package is intentionally agnostic of the authentication provider — the hosting application is responsible for constructing the session from its own auth mechanism (currently Keycloak) and registering it via `AddUserSessions`.

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Common/Authorization/README.md

#### Exceptions

Provides a unified, standardized set of exceptions for signaling failures from any application layer, split into three families: Client, Server, Integration. Standardizing on this set lets a single exception-handling middleware (see `WebApi` → `ExceptionHandling` module) map any thrown exception to a consistent HTTP response.

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Common/Exceptions/README.md

#### Pagination

Provides the shared building blocks for paginated results: `PagedFilter` (the incoming page request), `OffsetPagedList` (offset-based result set, paired with `Databases.Postgres`'s `PageAsync`), and `CursorPagedList` (keyset/cursor-based result set, paired with `Databases.Postgres`'s `ToKeysetPageAsync`).

### CQS

Implements the CQS (Command/Query Separation) pattern on top of MediatR, adding a configurable pipeline of cross-cutting behaviors: Diagnostic, Authorization, Validation, Logging, Performance, Caching, and Transactional around every command handler. Services build their handlers against `ICommandHandler<TCommand>` / `ICommandHandler<TCommand, TResponse>` and `IQueryHandler<TQuery, TResponse>` from `CQS.Abstractions`, register everything via `AddCqs`, and opt individual behaviors in as needed. This is precisely what constitutes the **Applications.Handlers** sublevel (see `service-structure.md`).

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/CQS/Base/README.md

### Databases

#### Postgres

Provides the tooling for working with PostgreSQL through EF Core: a pooled `DbContext` registered via `AddPostgres<TDbContext, TConfigurator>`, a ready-to-use `IRepository<TEntity, TId>` covering both CRUD and querying (in general, it is recommended to use only a single, unified interface), offset pagination (`PageAsync`) and keyset pagination (`ToKeysetPageAsync`) for large tables, `ITransactionalScope` (the same mechanism `CQS`'s `TransactionalBehavior` wraps every command in), and shared model-building conventions (snake_case naming, `DateTimeKind` normalization). The `Migrations` project under this package is what backs the dedicated **HostSide.Migrations** host described in `service-structure.md`.

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Databases/Postgres/Base/README.md

### Domain

Provides the shared building blocks that support the tactical DDD side of every service's `Domain` layer: `AuditableEntity<TId>` (change-tracking with `MarkAsCreated`/`MarkAsUpdated`/`MarkAsDeleted` internally and `Remove`/`Restore` externally, plus a matching `ConfigureAuditableEntity<TEntity, TId>` EF Core extension so every service configures auditing the same way), `ISoftDeletable`, and the base `DomainException` that every domain-level invariant violation should derive from. This package must only be referenced by a service's own `Domain` layer, and by nothing else.

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Domain/README.md

### WebApi

Provides the tooling for building a service's HTTP layer: a modular, opt-in pipeline of self-contained infrastructure concerns (`IWebModule`: Caching, Logging, Correlation, ExceptionHandling, TrafficControl, HealthChecks, OpenTelemetry, Authorization, and Swagger), a reflection-based endpoint registration system (`IEndpoint` + `AddEndpoints`/`MapEndpoints`), and per-endpoint rate limiting (`WithRateLimit`, sliding window) and throttling (`WithThrottle`, token bucket) as `RouteHandlerBuilder` extensions. This is the package behind every service's **HostSide.WebApi** host.

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/WebApi/README.md