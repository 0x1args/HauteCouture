## Service structure

This document describes the standard structure that every service added to the system must follow, the dependency rules between the layers of that structure, and how those layers connect to the shared code base in `Shared`. It explains the responsibility and intent of each layer so that the current shape of a service is understood, and so that new services can be built consistently with it. Every service in the system is built on three combined architectural approaches: Domain-Driven Design; Clean Architecture; CQS.

**Layer responsibilities at a glance**

| Layer | Responsibility |
|---|---|
| **Domain** | Owns the business domain: aggregates, entities, value objects and invariants. Has no dependency on any other layer. |
| I**nfrastructures.DataAccess** | Configures access to external data stores (e.g. EF Core `DbContext`s and entity configuration) and maps domain models to persisted data. |
| **Applications.Services** | Executes business use cases by orchestrating domain models against data-access abstractions, without depending on data-access configuration details. |
| **Applications.Handlers** | Entry point for use-case execution. Splits work into Commands and Queries, and owns validation, logging, caching, transactions, diagnostics, and authorization. |
| **Contracts** | Holds the artifacts used to communicate with the outside world: requests, responses, events, mappers, constants, and other shared helpers. |
| **Providers** | Composition root of the service. Registers and wires all dependencies from **Domain**, **Applications**, **Contracts**, **Infrastructures.DataAccess**, and the relevant `Shared` packages for consumption by the HostSide hosts. |
| **HostSide.WebApi** | The presentation layer. Exposes the service over HTTP and delegates all work to **Applications.Handlers**. |
| **HostSide.Migrations** | A standalone host dedicated to applying database migrations. |
| **HostSide.Consumers** *(optional)* | A standalone host that reacts to inbound events/messages. |
| **HostSide.BackgroundJobs** *(optional)* | A standalone host that runs scheduled or recurring background work. |
| **HostSide.SignalR** *(optional)* | A standalone host that handles real-time, persistent client connections. |


## 1. Domain

This layer is the core of every service: it contains all definitions of the business domain. It is designed strictly around rich domain models with explicitly defined invariants. Following DDD, there are two key concepts: Strategic DDD and Tactical DDD (described below) — and every subdomain modeled in the system (usually represented by a single service) must comply with both. This layer has no dependencies on anything else in the system.

### 1.1 Strategic DDD

Every service added to the system must be classified against a defined subdomain type, which directly determines the significance of that service within the overall system. The available classifications are:

| Name | Description |
|---|---|
| **Core** | Contains the core business logic — the logic that is the actual reason the business exists and differentiates it from competitors. |
| **Generic** | Contains logic that does not define the business itself and has no unique or differentiating solutions; it implements standardized, off-the-shelf approaches. |
| **Supporting** | Complements the core business logic and rounds out the overall business capability without being central to it. |

Every service is also tied to the concept of a Bounded Context, whose boundaries must not be violated. A bounded context may map one-to-one to a subdomain, or it may encompass several related subdomains.

### 1.2 Ubiquitous Language

Every service must be built around its own consistent Ubiquitous Language. This language differs from service to service, because the same word can carry a different meaning in different contexts. The language must be reflected literally in code: class names, method names, field names, and domain events must all speak the language of the business. A method name must mirror the action as the business describes it, and the same applies to domain events.

Synonyms within the same context are forbidden. For example, if the term `Remove` has already been adopted within a bounded context, `Delete` must not be used elsewhere in that same service to describe the same concept. Abbreviations and shortenings are not accepted for the sake of brevity, names must be complete and unambiguous to read.

### 1.3 Tactical DDD

Tactical DDD defines how the business logic identified above is technically implemented. It combines the following building blocks, all of which must be reflected in code to guarantee a correct implementation.

| Name | Description | Design rules |
|---|---|---|
| **Aggregate** | A cluster of related entities treated as a single unit of consistency. Has exactly one Aggregate Root, through which all external access and all changes must flow. | All properties must have private setters; state changes must occur strictly through public methods defined on the aggregate, which encapsulate all business-logic nuances. Constructors must be private, an aggregate is created exclusively through a factory method whose name must begin with `Create`. The factory method accepts primitive (raw) values, wraps them into Value Objects, and passes them to the constructor; wrapping every eligible property in a Value Object is preferred wherever it makes sense. All conversions and validations must happen inside the factory method, the constructor must remain "clean" (assignment only, no logic). The Aggregate Root controls the state of its related entities and is responsible for upholding the invariants of the entire cluster. |
| **Entity** | A domain object with a unique identity (an `Id`) that persists throughout its lifecycle, independent of attribute changes. | Follows the same rules as Aggregate: private setters, state changes only through defined methods, a private and "clean" constructor. Created through a `Create` factory method that accepts primitive values and wraps them into Value Objects. An Entity that is not meant to be accessed from outside its Aggregate must not be exposed other than through the Aggregate Root. |
| **Value Object** | A domain object with no identity of its own, defined entirely by its attributes; it is immutable and compared by value. Used to extract invariants and additional validation out of Entities/Aggregates. | Wraps a specific value together with all required checks and validation. The constructor must be private and "clean"; the wrapped property must have a private setter. The factory method must be strictly named `Of` and must contain all validation logic. Additional helper methods may be added where necessary. A Value Object is recommended to be modeled as a `readonly record struct` wherever this is feasible. |
| **Invariant** | A business rule that must always hold true (e.g. "an amount cannot be negative", "the date and time a model is accessed cannot precede the date and time it was created"). | — |
| **Domain Event** | A fact that occurred in the domain and matters to the business. It is conceptually raised by an Aggregate, but in our approach domain events are neither raised nor dispatched from within the aggregate itself — they are created and dispatched at the handler level. | Modeled as a `sealed record`, with all values supplied through the constructor. The name must end with `Event`. All events must be named in the past tense. |

### 1.4 Additional guidelines

- Domain models must signal invariant violations by throwing an exception.
- Domain models must expose an empty, private constructor for the ORM (EF Core).
- The Domain layer must not contain any abstractions for services or repositories, those belong to outer layers.
- All Aggregate and Entity identifiers must be implemented as strongly-typed IDs (e.g. `UserId`, `MemberId`), never as "bare" `Guid`/`int` values.
- Date and time values must be obtained through an external abstraction rather than a direct call to `DateTime.UtcNow`, to keep domain logic testable and deterministic.
- Domain methods must avoid `bool` flag parameters. If such a need arises, a second, separate method should be introduced instead.
- Domain models that require change tracking must be audited using the tooling provided in `Shared.Domain` (`AuditableEntity`).

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Domain/README.md

## 2. Infrastructures.DataAccess

This layer is responsible for configuring access to external data stores. When using EF Core, this is where `DbContext` classes are created and domain models and `DbContext`s are configured. The layer depends directly on `Domain` and configures domain models that use Value Objects via the `HasConversion` method, which defines how a Value Object is unwrapped into a primitive value when written to the database, and how a primitive value is wrapped back into a Value Object when read from it.

Under this approach, the `Applications.Services` layer must operate on Value Objects themselves when performing computations, and use `.Value` only when reading the underlying primitive. To keep the code clean, it is recommended to create a static `CustomModelBuilder` class that owns all configuration logic and is invoked from `OnModelCreating` in the `DbContext`. All tooling for working with external data stores is available in `Shared.Databases`.

**Reference documentation:** https://github.com/0x1args/HauteCouture/tree/main/src/Shared/Databases (select the database you need and open the `Base` folder, which contains a `README.md` with the relevant documentation).

## 3. Applications

This layer acts as the orchestrator between **Infrastructures.DataAccess** and **Domain**. Where **Domain** is responsible for defining business logic, **Applications** is responsible purely for executing it and connecting it to the infrastructure. In the designed structure, the **Applications** layer is split into two sublayers: **Services** and **Handlers**.

### 3.1 Applications.Services

This sublayer is the key point where everything comes together into a finished operation that can be handed off for execution. It is responsible for working with domain models and connecting them to various external stores or services. This layer depends directly on **Domain**, but, by design, it does **not** depend on **Infrastructures.DataAccess**, so that configuration logic is not mixed with business-execution logic. Instead, it depends on the corresponding abstraction layer from `Shared.Databases.<DatabaseName>.Abstractions`.

In this design, services interact only with data-access abstractions that encapsulate the underlying `DbContext` (in the case of EF Core). This keeps the **Services** sublayer unaware of any configuration concerns and lets it perform its work with the minimum necessary knowledge of the infrastructure.

### 3.2 Applications.Handlers

The **Handlers** sublayer is the primary unit of execution within the system. It delegates calls to services, or creates and dispatches domain events. It splits operations into Commands and Queries so each can be handled through its own pipeline. All request validation, logging, caching, transaction management, diagnostics, and authorization take place at this level. This keeps **HostSide.WebApi** responsible only for the HTTP concerns and for delegating calls further into the handlers.

### 3.3 Additional guidelines

- Service names must follow the format `<AggregateRoot>Service`.
- Handler names must follow the format `<UseCase>CommandHandler` for commands, and `<UseCase>QueryHandler` for queries.
- Services must depend on the final `IRepository<TEntity>` interface. No additional repository abstractions should be created on top of it.
- All responses must be returned using the `<Projection>Response` naming format, with no `Dto` suffix.
- Services should only log the final, mutating operation. Read-only (data-retrieval) operations must not be logged.
- Services must signal operation failures by throwing the corresponding exceptions defined in `Shared.Common.Exceptions`.
- Methods responsible for retrieving data must project the result directly into the `<Projection>Response` shape as part of the SQL query itself (e.g. via a `Select` projection), rather than materializing an entity first and mapping it afterward.
- Handlers must never contain logging logic, logging belongs to the `Services` sublayer.
- Methods in services that create a new entity must return its `Guid`.
- Do not use `DateTime.UtcNow` or `DateTimeOffset.UtcNow` to obtain the current time; use the injected `TimeProvider` instead.

**Reference documentation (Handlers & CQS):** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/CQS/Base/README.md

**Reference documentation (Exceptions):** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Common/Exceptions/README.md


## 4. Contracts

This is a supporting layer that contains everything needed for the service to communicate with the outside world: requests, responses, and events. It may also contain supporting components such as DTO mappers, helpers, and shared constants or options used across **HostSide** hosts. Because this layer defines the public "shape" of the service's API and event surface, it must remain independent of any specific persistence or execution technology, it should only depend on **Domain**-level types where strictly necessary (e.g. for mapping) and on `Shared` contract utilities.

## 5. Providers

This is the layer responsible for supplying and wiring all key components (dependencies and configuration) that make up a service's logic into the individual sublayers under **HostSide**. It acts as the single composition point of dependency provisioning for the service. This layer depends on **Domain**, **Applications**, **Contracts**, **Infrastructures.DataAccess**, and the relevant `Shared` layers, and is responsible for registering all DI services. Every **HostSide** host consumes this layer through a single, well-known extension method (e.g. `AddServiceName()`) and seperate layer-based methods (e.g `AddInfrastructures()`, `AddApplications()`), so that adding or removing a host never requires duplicating registration logic.

## 6. HostSide

`HostSide` is the umbrella layer that groups together the individual application hosts of a service — separately deployable entry points, each with its own configuration per environment. Splitting the service into multiple hosts keeps background or auxiliary work from competing for the resources and threads of the main service, and gives the system flexibility to scale each part independently, including running multiple instances of a specific host.

### 6.1 HostSide.WebApi

This is the layer treated as the Presentation layer. It is responsible for everything related to the HTTP side of the service and is the first layer to participate in handling a request. It must limit itself strictly to the concerns of its own layer and delegate all further work to the handlers. This is where request models are populated, middleware is applied, the HTTP context is used, endpoints are defined, and all other HTTP-related work happens.

#### 6.1.1 Additional guidelines

- All CUD (Create/Update/Delete) operations must accept a `record` named using the `<UseCase>Request` format, with no `Dto` suffix. This type must carry the `[FromBody]` attribute.
- All R (Read) operations must accept parameters in plain form, following standard REST conventions (route/query parameters).
- Minimal API is the preferred approach: for each use case, create two classes: a `<UseCase>Endpoint` implementing the `IEndpoint` interface, and a corresponding `<UseCase>Handler` (a static class with a `Handle` method). Every endpoint must be documented.
- The body of an endpoint handler must remain as thin as possible: build the corresponding command or query and delegate the call further down the chain, nothing else.
- The ready-made functionality from `Shared.WebApi` must be used. Where needed, that package's tooling supports extension with service-specific components.
- `Program.cs` must remain as minimal as possible; all setup logic must be moved into extension methods.
- DI extension methods must be placed under a `Registration` folder.
- All endpoints must follow REST API conventions.
- The HTTP layer must not leak into higher layers, all data crossing the boundary must be packaged into dedicated structures (requests/commands/queries), never passed as raw HTTP primitives.

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/WebApi/README.md

### 6.2 HostSide.Migrations

For convenience during development and for greater deployment flexibility, migrations are extracted into a separate host that applies pending migrations every time it starts up. This host must be run whenever new changes need to be introduced into the database schema, and it can be scheduled or triggered as a distinct step in CI/CD.

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Databases/Postgres/Migrations/README.md

### 6.3 HostSide.Consumers *(optional)*

Consumers that react to events are deliberately extracted into their own host so that they do not degrade the **WebApi** host with background work. Consumers are always active and continuously consume a share of resources; as system load grows, this could otherwise compete directly with the WebApi's own throughput, which is why this separation was chosen. Consumer names must follow the `<Event>Consumer` naming format.

### 6.4 HostSide.BackgroundJobs *(optional)*

This host is used to isolate scheduled and recurring background work, for example, periodic cleanup tasks, batch processing, or timed synchronization jobs from the request-handling path of **WebApi**, following the same reasoning as **HostSide.Consumers**: predictable resource usage, independent scaling, and independent deployment. Job classes must be named using the `<UseCase>Job` format, and each job should encapsulate a single, well-defined use case rather than combining multiple unrelated pieces of work. Scheduling configuration (interval, cron expression, or trigger) for a job belongs to this host's own configuration and must not be hardcoded inside **Applications**.

### 6.5 HostSide.SignalR *(optional)*

This host is used to isolate real-time, persistent client connections (e.g. WebSocket/SignalR hubs) from the request/response-oriented **WebApi** host. Persistent connections have a fundamentally different resource and scaling profile, one connection stays open for the lifetime of a client session, so keeping them on a separate host prevents that load from affecting ordinary HTTP throughput and allows this host to be scaled independently. Hub classes must be named using the `<Feature>Hub` format. A hub should stay as thin as **ostSide.WebApi** endpoints do: it receives an inbound client call or an inbound domain event (typically forwarded from **HostSide.Consumers**) and pushes it to the relevant clients, without containing business logic of its own.