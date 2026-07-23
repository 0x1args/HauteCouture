---
name: code-reviewer
description: Checklist and guidelines for conducting code reviews on the HauteCouture project. Verifies compliance with the project's domain, architecture, naming, security, and performance conventions defined in the Agents instructions.
metadata:
    author: 0x1args
    version: 1.0.0
---

## Code reviewer

This skill describes the checklist to follow when reviewing code in this project. The goal isn't just to check that the code compiles and works, but to verify it against every architectural, domain, naming, and performance convention defined in the `Agents instructions` (Clean Architecture, DDD, CQRS, naming conventions, the preferred approaches in section 4.2, and the prohibitions in section 4.3). The reviewer should go through each section below and flag any item that isn't satisfied, along with an explanation and the exact location in the code (file/line) where the issue was found. Prefer linking a comment to a concrete line over a general remark, it's actionable and easy for the author to resolve.

### Checklist

#### 1. Overall implementation

- [ ] Does this change actually accomplish what it's supposed to do?
- [ ] Can the solution be simplified?
- [ ] Does this change introduce unnecessary compile-time or run-time dependencies?
- [ ] Does it use a framework, API, library, or service that the project avoids (see 4.2, libraries with excessive allocations, AutoMapper, etc.)?
- [ ] Would adding or swapping a library/service meaningfully improve the solution?
- [ ] Is the code at the right layer of abstraction (Domain / Applications / Infrastructures / Contracts / HostSide, Provider)?
- [ ] Is the code modular enough?
- [ ] Are Clean Architecture boundaries respected, does the HTTP layer (headers, status codes, DTOs) leak into the layers above it?
- [ ] Does similar functionality already exist in `Shared`? If so, why isn't it reused instead of being reimplemented?
- [ ] Are there GoF patterns or C#-specific idioms that would meaningfully improve the code, without overusing patterns for their own sake?
- [ ] Are SOLID principles followed (Single Responsibility, Open-Closed, Liskov Substitution, Interface Segregation, Dependency Injection)?
- [ ] Are Bounded Context boundaries respected, and is the code consistent with the project's Ubiquitous Language?

#### 2. Domain layer (DDD)

- [ ] Is the domain model rich rather than anemic?
- [ ] Does all business logic live in the domain, rather than leaking into Application-layer services?
- [ ] Are domain models, methods, and value objects named in the language of the business, without `Entity`/`Model`/`Aggregate`/`ValueObject`/`VO` suffixes?
- [ ] Are value objects named so it's clear which domain model they belong to (e.g. `MemberId`)?
- [ ] Are domain exceptions named `<DomainModel>Exception`?
- [ ] Are factory methods (`Create`/`Of`) used instead of public constructors where appropriate?
- [ ] Are domain events `sealed record`s, created and published from the handler, not from the service or the domain model itself?
- [ ] Are events kept coarse-grained (one business event = one event) rather than fragmented per consumer?

#### 3. Architectural layers and CQRS

- [ ] Do services stay unaware of the `DbContext`?
- [ ] Do handlers avoid `IRepository<TEntity>`, `DbContext`, or any ORM/data-access specifics directly, delegating instead to a service?
- [ ] Is `ISender` used to dispatch commands/queries, rather than `IMediator`?
- [ ] Is validation absent from the service layer (validation belongs to the command/query, run through the pipeline, not to the `<UseCase>Request`)?
- [ ] Is caching absent from the service layer (cacheable queries should implement `IPagedQuery` and go through the caching pipeline)?
- [ ] Are operations left unwrapped by manual transactions at the service layer (transactions belong to the command-handler pipeline)?
- [ ] Do endpoints follow REST conventions rather than a use-case style (`users/createUser`)?
- [ ] Is data read from HTTP headers via a dedicated helper, rather than inline in application code?
- [ ] Is AutoMapper (or similar) avoided in favor of hand-written extension methods for mapping?

#### 4. Naming and file structure

- [ ] `DbContext` → `<ProjectName>DbContext`; model configurations → `<Entity>Configuration`; configurator → `<DbContext>Configurator`?
- [ ] Services → `<DomainModel>Service`, no plural suffixes; method names match the use case?
- [ ] Commands/queries → `<UseCase>Command`/`<UseCase>Query`; handlers → `<UseCase>CommandHandler`/`<UseCase>QueryHandler`?
- [ ] Contracts free of `Dto`; requests → `<UseCase>Request`; responses → `<Projection>Response`; mappers → `<Model>Mapper`?
- [ ] Presentation-layer endpoint/handler pair → `<UseCase>Endpoint` / `<UseCase>Handler`; middleware → `*Middleware`?
- [ ] Extension files → `<Name>Extensions`; DI registration files → `<Name>Registrar`; DI methods start with `Add`, pipeline extensions start with `Use`?
- [ ] Tests → `<ClassName>UnitTests` / `<ClassName>IntegrationTests`; test names follow `<Method>_Should<ExpectedBehavior>_When<Condition>`; AAA structure with separating comments?
- [ ] Is the code placed in the correct file/folder/layer given the project structure?
- [ ] Is `#region`/`#endregion` avoided as a way of organizing code?

#### 5. Logic errors and bugs

- [ ] Can you think of a use case where the code doesn't behave as intended?
- [ ] Can you think of inputs or external events that could break the code?
- [ ] Are edge cases handled (nulls, empty collections, boundary values, concurrent access)?

#### 6. Error handling and logging

- [ ] Is error handling done correctly for the layer it's in (domain exceptions raised in the domain, etc.)?
- [ ] Should any logging or diagnostic information be added or removed?
- [ ] Is logging limited to process finalization rather than every step, so it doesn't burden memory with useless string allocations?
- [ ] Are error messages user-friendly?
- [ ] Is there enough log detail for easy debugging, and is it integrated with `ActivitySource`/the correlation ID?

#### 7. Dependencies and compatibility

- [ ] Were documentation, configuration, or README files updated as required by this change?
- [ ] Are there impacts on other parts of the system or on backward compatibility (event contracts, public APIs)?
- [ ] If the change touches the EF Core schema, are existing migrations left untouched, with a new migration created on the HostSide instead?

#### 8. Security and data privacy

- [ ] Does the code introduce any security vulnerabilities?
- [ ] Are authentication and authorization handled correctly (`ICurrentUserSession`, `AuthorizationBehavior`, `RequireUserId()`)?
- [ ] Is input validated, sanitized, and escaped to prevent XSS or SQL injection?
- [ ] Is sensitive data (personal data, payment information) stored and handled securely?
- [ ] Is the right encryption used where needed?
- [ ] Does the change avoid leaking secrets (keys, passwords, tokens, connection strings)?
- [ ] Is data retrieved from external APIs or libraries checked for security issues before use?

#### 9. Performance and .NET internals

- [ ] Is `CancellationToken` passed through every asynchronous call?
- [ ] Is `Guid.CreateVersion7()` used instead of `Guid.NewGuid()`?
- [ ] Do queries return a projection rather than the full entity when only specific data is needed?
- [ ] Is `.AsNoTracking()` applied to read-only queries?
- [ ] Is eager loading combined with `AsSplitQuery()` used where multiple `Include`s are present, to avoid N+1 and cartesian explosion?
- [ ] Are compiled queries used for frequently repeated query shapes?
- [ ] Is query caching/parameterization done correctly (a variable is passed rather than a literal, so the query compiles once)?
- [ ] Does every `string` property in EF Core configurations have `MaxLength` set?
- [ ] Is `ExecuteUpdateAsync`/`ExecuteDeleteAsync` used instead of looping over large record sets?
- [ ] Are the right columns indexed, with composite indexes in the correct column order?
- [ ] Is pagination used wherever a potentially large result set is returned, instead of full in-memory materialization?
- [ ] Is boxing/unboxing avoided in favor of generic versions?
- [ ] Are classes with no descendants marked `sealed`?
- [ ] Are commands, queries, requests, responses, and events modeled as `record`s?
- [ ] Is `Task` used by default for asynchronous operations, with `ValueTask` reserved for cases where it's actually justified?
- [ ] Is `IHttpClientFactory` used instead of instantiating `HttpClient` directly?
- [ ] Is string concatenation via `+` avoided in favor of interpolation, and are unnecessary intermediate string allocations (`Substring`, `IndexOf`, `ToCharArray`) avoided?
- [ ] Are unnecessary temporary arrays avoided?
- [ ] Are objects kept small enough to avoid landing on the LOH, and is short-lived object churn kept in balance to avoid GC pressure?
- [ ] Is object pooling / pooled buffers used for large objects?
- [ ] Are `Span`/`stackalloc` used in high-throughput hot paths?
- [ ] Is the code written to be JIT-friendly (aggressive inlining where it genuinely helps)?
- [ ] Is reflection used sparingly, avoiding unnecessary overhead?
- [ ] Is `ConcurrentDictionary` used for caching where the same computed results are likely to be reused?
- [ ] In multithreaded contexts, is `lock` avoided in favor of optimistic or pessimistic concurrency control?
- [ ] Are long-running operations moved off the `ThreadPool` into dedicated background hosts?
- [ ] Is deadlock risk ruled out, and do transactions uphold ACID?
- [ ] Is date/time obtained via `TimeProvider` as close as possible to the actual action, rather than read directly inside the domain?

#### 10. Usability and accessibility

- [ ] Is the proposed solution well designed from a usability standpoint?
- [ ] Is the API well documented?
- [ ] Is the API intuitive to use?

#### 11. Testability and tests

- [ ] Is the code testable (can dependencies be mocked easily)?
- [ ] Have automated tests been added or updated to cover this change?
- [ ] Do the existing tests adequately cover the change (unit/integration)?
- [ ] Are there additional cases, inputs, or edge cases worth testing?
- [ ] Do integration tests account for the required infrastructure (Docker: Postgres, MongoDB, Kafka, etc.)?

#### 12. Readability

- [ ] Is the code easy to understand?
- [ ] Which parts were confusing, and why?
- [ ] Could readability improve by splitting into smaller methods?
- [ ] Could readability improve with better method/variable names?
- [ ] Is the data flow understandable?
- [ ] Are there outdated or redundant comments?
- [ ] Could some comments be removed by making the code self-explanatory instead?
- [ ] Is there any commented-out code that should be deleted?

#### 13. Pre-merge post-check

- [ ] Does the whole solution build: `cd src/Solution && dotnet build HauteCouture.slnx`?
- [ ] Do unit and integration tests pass: `dotnet test HauteCouture.slnx` (with Docker running for integration tests)?
- [ ] If there are errors or warnings, has their root cause been identified along with a proposed fix?

#### 14. Expert opinion

- [ ] Should a specialist (security, performance, DevOps) review this change before it's accepted?
- [ ] Does this change affect other teams or services, and should they be looped into the review?
