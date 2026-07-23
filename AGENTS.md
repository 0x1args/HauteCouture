## Agents instructions

This file describes the instructions for any AI model integrated into the project, so that the model always has an entry point into any context with an already prepared knowledge base.

### 1. Core information about the technology stack

The project is a multitenant SaaS platform built on a microservices architecture.

| Area | Technologies |
|---|---|
| **Backend** | ASP.NET Core + .NET 10, EF Core, PostgreSQL, MongoDB, MassTransit, Kafka, Keycloak, S3 (Minio) |
| **Observability** | Serilog, Seq, OpenTelemetry, Grafana, Prometheus, Jaeger |
| **Containerization** | Docker, Docker Compose |

### 2. Core information about the architectural patterns applied

| Pattern | Application |
|---|---|
| Microservices | An ASP.NET Core Web API per service |
| Domain-Driven Design | Strategic DDD and Tactical DDD in every service |
| Clean Architecture | In every service (Domain, Infrastructure/DataAccess, Applications.Services, Applications.Handlers, Contracts, Providers, HostSide) |
| CQRS | In some services, a pragmatic asynchronous variant with two databases: write-side Postgres, read-side MongoDB |
| Event-Driven Architecture | Via MassTransit and Kafka |
| Outbox / Inbox patterns | Via MassTransit |
| Saga pattern | Via MassTransit |
| Circuit Breaker | Via Polly and manual extensions |
| API Gateway | An off-the-shelf solution |

### 3. Core paths in the project

| Path | What is located there |
|---|---|
| `/src` | All of the project's source code, where all services are located; the `.slnx` file is located at `src/Solution`, and the remaining services are relative to `/src`, for example `/src/ExampleService` |
| `/infrastructure/docker` | Configuration for running services and their dependencies (databases, message brokers, etc.) locally via Docker/Docker Compose |
| `/infrastructure/scripts` | Automation scripts that support environment setup and day-to-day development tasks (for example, starting or stopping Docker containers) |
| `/infrastructure/kubernetes` | Manifests and configuration used to orchestrate the services in a cluster, for environments beyond local development |
| `/infrastructure/terraform` | Infrastructure as Code (IaC) configuration used to provision and manage cloud infrastructure in a consistent, reproducible, and version-controlled manner |
| `/tests` | All tests, split into Unit and Integration, and further organized by the module they correspond to |
| `/docs` | Description of the architecture, service structure, tests, documentation structure, and their location |

### 4. General rules and conventions

This section is broken down into a number of smaller subsections, organized so it's clear what must be followed when developing a specific area of the project, what to pay attention to, how to organize structure, which naming conventions to follow, and which approaches to prefer.

#### 4.1 Naming conventions

- At the domain layer, all domain models must clearly reflect the object of the business domain, without any suffixes like `Entity`, `Model`, or `Aggregate`. All methods must be clearly named in the language of the business. All Value Objects must be named in a way that makes clear which domain model they belong to (for example, `MemberId`, `UserId`), also without any `ValueObject`/`VO` suffix. Domain-level exceptions must be named in the format `<DomainModel>Exception`.
- At the infrastructure layer, the `DbContext` must be named in the format `<ProjectName>DbContext`; all model configurations must be named `<Entity>Configuration`; the configurator for the `DbContext` must be named in the format `<DbContext>Configurator`.
- At the application layer, services must be named `<DomainModel>Service`, with method names matching the use case; no plural suffixes in service names. For handler primitives, naming should follow the format `<UseCase>Command`/`<UseCase>Query`, and for the handlers themselves `<UseCase>CommandHandler`/`<UseCase>QueryHandler`.
- At the contracts layer, any naming containing `Dto` is strictly forbidden. Requests should be named in the format `<UseCase>Request`, and responses `<Projection>Response`. Mappers must be named in the format `<Model>Mapper`. All classes containing constants must have a meaningful name (without any `Const` suffix for the values themselves); constants must be named in PascalCase.
- At the presentation layer, always prefer Minimal API, defining two classes: the first named `<UseCase>Endpoint` and the second `<UseCase>Handler`. Make an exception at the HTTP level for naming asynchronous methods, since the operation itself is often already understood as asynchronous based on the interface it returns and what it does (an analogy being that in MediatR some operations are asynchronous without an `Async` suffix, or another example being MassTransit or Wolverine). Middleware must have a `Middleware` suffix.
- Files with regular extension methods must be named `<Name>Extensions`; files with DI registration must be named `<Name>Registrar`. DI methods must start with `Add` (for example, `AddPostgres`, `AddExampleService`, `AddApplications`, `AddInfrastructure`); methods that are extensions for the ASP.NET Core pipeline must start with `Use` (for example, `UseSwagger`, `UseAuthentication`, `UseExampleService`).
- Tests must have class names in the corresponding form `<ClassName>UnitTests` or `<ClassName>IntegrationTests`. Test names themselves must follow the format `<Method>_Should<ExpectedBehavior>_When<Condition>` and follow the AAA pattern, with each part separated by a comment.

#### 4.2 Preferred approaches

- Always pass a `CancellationToken` everywhere.
- Prefer `Guid.CreateVersion7` over `Guid.NewGuid`.
- Always return a projection in the response.
- Minimal API instead of controllers.
- Keep `Program.cs` clean in the Web API; extract logic into extension methods.
- Prefer `DbContext` pooling.
- Use compiled queries for repeated query patterns.
- Use query caching and parameterization correctly; pass a variable into the query instead of a literal value, so that instead of N compilations there is only one.
- Retrieve a projection instead of the full entity, fetching only the data that is actually needed.
- For queries with many includes, watch out for cartesian explosion and apply `AsSplitQuery`.
- By default, EF Core configures strings as `nvarchar(max)`; always set `MaxLength` when configuring models.
- Use `ExecuteUpdateAsync` and `ExecuteDeleteAsync` when a large amount of data needs to be changed instead of doing it in a loop.
- Always apply eager loading to avoid N+1 problems.
- For read-only queries, apply `.AsNoTracking()`.
- Index columns correctly; use composite indexes with the correct column order so ranges of data are filtered efficiently.
- Keep request handlers as clean as possible; they should only delegate the call further to the handlers.
- Don't log everything, log only the finalization of a process, so as not to clutter memory with unnecessary string allocations that carry no useful information.
- Cache queries that are likely to be requested frequently.
- Always return `Task` for asynchronous operations; where possible, try to use `ValueTask`, but this decision must be justified so it doesn't lead to redundant allocations from a later conversion to `Task`.
- By default, mark classes that will not have descendants as `sealed`.
- Configure commands, queries, requests, responses, events, or any other DTOs as `record`.
- Do not use `HttpClient` directly; always prefer `IHttpClientFactory`.
- Strictly preserve separation of responsibilities between layers; the HTTP layer must not leak into the layers above it.
- Do not materialize all data in memory; use pagination when returning large amounts of data.
- Every module being designed should be designed to be extensible; try to follow SOLID principles when writing code.
- Where possible, write pure functions; extract frequently used blocks; if a piece of logic is repeated across many classes, extract it into a static helper class.
- Apply GoF patterns where it's possible and appropriate; don't overuse patterns.
- Avoid inheritance as an OOP principle where possible.
- Avoid boxing/unboxing; always use the generic versions.
- Extract all magic values into constants; extract endpoint names passed to `WithName` into a separate constants file, and do the same for `WithTags`, place these in the Contracts layer.
- Do not use string concatenation via `+`. Prefer interpolation.
- Avoid intermediate string allocations.
- Do not use `Substring`, `IndexOf`, `ToCharArray`.
- Try not to create temporary arrays where possible.
- Try to keep objects small so they don't end up on the LOH (to avoid problems with fragmented memory regions where an object can't fit and therefore sits idle); also don't overuse short-lived objects so as not to put pressure on the GC, maintain a balance; use object pooling for large objects to avoid excessive allocations. In general, use pooled buffers; account for hidden allocations.
- In scenarios that fall under the high-load category, try to use `Span` and `stackalloc` for optimizations.
- Try to write code that is amenable to JIT optimizations; use aggressive inlining where appropriate.
- Code must have high throughput and minimal load on the GC.
- Don't overuse reflection due to its slowness.
- If there's reason to suspect that the same results will be reused during an operation, use `ConcurrentDictionary` for caching.
- In multithreaded contexts, do not use `lock` in services; prefer optimistic or pessimistic locking instead.
- Don't burden the `ThreadPool` with operations that will genuinely take a long time; move such operations into background services as separate hosts with their own `ThreadPool`, so they don't degrade the Web API's performance.
- Avoid deadlock situations; adhere to the ACID acronym so that transactions execute correctly and fully ensure data consistency and correctness.
- Pass date and time as close as possible to the actual action, doing so in services via `TimeProvider`; all domain-level methods that need a date and time must receive it as a parameter.
- Do not use libraries that have been found to have excessive allocations; find alternatives for them; if the solution is not complex, write your own implementation.
- Don't write a solution from scratch, first check whether it's already implemented in `Shared`.
- Avoid pseudo-code; code must correspond to production-ready solutions, adapted for reasonable load and high RPS.

#### 4.3 What not to do

- Do not delete or edit existing migrations; you may analyze their contents and report if the code generator made a mistake in the schema configuration, and you may also create a new migration that fixes an incorrect schema structure.
- Do not use anemic models when developing domain models.
- Never start design at the infrastructure layer, always start with the domain.
- Do not mix the responsibilities of layers; they must remain strict.
- Do not mix code style and naming, follow the Ubiquitous Language; if `Remove` has been chosen as the term, then no `Delete` anywhere.
- Do not send a huge number of events without need; every event must have its own logical purpose.
- Do not use an approach with a huge number of `EntityRepository` classes and a custom Unit of Work; the project has its own approach based on `IRepository<TEntity>` (described further below).
- Do not treat the Applications layer as the layer where business logic lives, it must live in the domain layer; also do not mix the logic of deciding what to do with the logic of doing it.
- Services must not know about the `DbContext`.
- In handlers, do not use `IRepository<TEntity>`, `DbContext`, or any ORM/data-access specifics whatsoever; handlers must delegate the call to a service.
- Never send events from services, a service must not know about them; instead, use the handler as the place where events are created and sent.
- Do not create many small events for specific consumers; a producer must not adapt to consumers and must not know about them. One business event = one event, and that event can be large.
- Do not use AutoMapper or similar libraries; instead, write your own extension methods in the corresponding static classes.
- Do not cache data at the service layer; follow the single responsibility principle: if you want to request data from the cache, ask for it, don't create it. The project has a separate caching pipeline, it's enough for a query to implement the `IPagedQuery` interface.
- Do not use the `IMediator` interface when a command/query should be sent; instead, follow the single responsibility principle by using the `ISender` interface.
- Do not read data directly from an HTTP header in the code; instead, create a separate helper class that provides the needed value read from the header.
- Do not wrap operations in transactions at the service layer; the project has a separate transactional pipeline for command handlers.
- No validation at the service layer; the project also has a separate pipeline for validation, which automatically invokes the validator for the corresponding request.
- Do not write validation for `<UseCase>Request`; instead, write it for the corresponding command/query, which contains all of the request's assembled values.
- Do not write use-case-style endpoints such as `users/createUser`, they must all strictly follow REST API principles.
- Do not create migrations at the data access layer; create them on the HostSide, in a separate console application dedicated to running migrations.
- Try not to use the built-in registration mechanisms for specific components from various libraries, since they can be slow, as they scan a large number of unnecessary things; instead, write your own registration mechanism.
- Do not create classes for the sake of having classes; a large number of classes is unnecessary load on the GC.
- Do not use `#region`/`#endregion` as a means of organizing large blocks of code.

### 5. Recommended implementation flow

The recommended flow corresponds to the order in which the operations are listed below.

- Get familiar with the existing domain.
- Define the Ubiquitous Language, or adopt the existing one and follow it.
- Define the Bounded Contexts and strictly respect their boundaries.
- Identify and classify the necessary subdomain.
- Proceed to tactical DDD: write rich domain models; if working within an existing subdomain, first get familiar with the existing domain base.
- Configure the `DbContext` and write the corresponding configuration for the models; create migrations and apply them.
- Understand how the current microservice should interact with others, determine which events should be sent, and place those events in the Contracts layer.
- Proceed to writing services within the current microservice, implementing the business logic.
- Identify the necessary use cases and write handlers for them; the handlers should send and create the previously defined events.
- Proceed to the Web API layer, define all necessary endpoints, and register all dependencies.
- Provide all necessary code; where relevant, you may note observations about the existing code and suggestions for possible modernization, do not modernize the code without prior coordination.
- Provide recommendations on which areas of the code can be tested, and supply all necessary tests.

### 6. Post-check after writing code

Once the code has been written, a few steps should be performed:

- Build the whole solution, located at `src/Solution/HauteCouture.slnx`. First, navigate to the directory with `cd src/Solution`, then run the build with the command `dotnet build HauteCouture.slnx`.
- Next, run the unit tests, located along the same path, using the command `dotnet test HauteCouture.slnx` (if there are integration tests among them, make sure Docker is running).
- If errors or warnings occur, report them and determine their causes and potential solutions.

### 7. All available skills

All available skills are located at `.agents/skills`:

- `devops/SKILL.md` — Guidance for local and cluster environment setup: Docker/Docker Compose, Kubernetes manifests, Terraform provisioning, and the automation scripts under `/infrastructure`.
- `knowledge/SKILL.md` — Entry point into the project's architecture knowledge base and where to find the relevant docs under `/docs` and `/src/Shared`.
- `migrations/SKILL.md` — Rules and workflow for creating, reviewing, and applying EF Core migrations from the HostSide console app, without editing or deleting existing ones.
- `performance/SKILL.md` — Checklist of the project's performance conventions: allocation-conscious code, GC/LOH awareness, Span/stackalloc usage and throughput-oriented patterns.
- `testing/SKILL.md` — Conventions for writing and organizing unit and integration tests, naming, the AAA structure, and running the test suite via `dotnet test`.

### 8. Where to find project knowledge when needed

- Knowledge of the system architecture is located at `docs/architecture`.
- Knowledge of service structure and design, along with additional recommendations for their development, is located at `docs/service-structure`.
- Knowledge of and links to all documentation is located at `docs/docs-structure`.
- Knowledge about Shared is located at `src/Shared/README.md`; use links there as the reference location for the documentation files needed for a specific area within Shared.
- Knowledge about Docker and Docker Compose is located at `infrastructure/docker/README.md`.
- Knowledge about scripts is located at `infrastructure/scripts/README.md`.