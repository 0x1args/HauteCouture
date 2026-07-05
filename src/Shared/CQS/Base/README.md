## CQS

Contains the necessary tools for implementing the CQS (Command/Query Separation) pattern on top of MediatR, adding a configurable pipeline of cross-cutting behaviors around every command and query handler.

### Registration

To register all the tools in DI, add the `Shared.CQS` package to the registration point and call `AddCqs` with a configuration delegate. The assembly to scan for handlers, validators and event consumers must be specified via `FromAssembly`; every other option is opt-in and disabled by default. All registration examples:

```csharp
// Minimal registration.
services.AddCqs(cqs => cqs.FromAssembly<Program>());

// Registration with an explicit assembly reference instead of a marker type.
services.AddCqs(cqs => cqs.FromAssembly(typeof(Program).Assembly));

// Enable a specific subset of behaviors.
services.AddCqs(cqs => cqs
    .FromAssembly<Program>()
    .UseAuthorization()
    .UseValidation()
    .UseLogging());

// Enable every available behavior at once.
services.AddCqs(cqs => cqs
    .FromAssembly<Program>()
    .UseAllBehaviors());
```

Calling `AddCqs` always registers MediatR itself (AddMediatR) and scans the specified assembly for CQS handlers. The `UseX()` calls only set flags on `CqsOptions` — the actual `IPipelineBehavior<,>` registration for each one happens inside `AddCqs` in a fixed order, and each behavior can also be registered independently:

```csharp
services
    .AddDiagnosticBehavior()
    .AddAuthorizationBehavior()
    .AddValidationBehavior()
    .AddLoggingBehavior()
    .AddPerformanceBehavior()
    .AddCachingBehavior() 
    .AddTransactionBehavior();
```

### Pipeline order

Because `IPipelineBehavior<,>` instances execute in registration order, `AddCqs` registers them in the following sequence, which reflects a typical request lifecycle:

| Order | Behavior | Requires | Description |
|---|---|---|---|
| 1 | Diagnostic | UseDiagnostics() | Adds tracing (requires `DiagnosticHandler`) for OpenTelemetry. |
| 2 | Authorization | UseAuthorization() | Adds authorization (requires `AuthorizationHandler`). |
| 3 | Validation | UseValidation() | Adds validation of incoming requests using FluentValidation. Validation error messages are formatted as `Field '{PropertyName}': {ErrorMessage}` for each field. |
| 4 | Logging | UseLogging() | Adds structured logging before and after execution. Includes serialized request/response payloads as compact JSON in the logs. |
| 5 | Performance | UsePerformanceTracking() | Adds execution time tracking. If the execution time exceeds the fixed 600 ms threshold, a warning is logged. |
| 6 | Caching | UseCaching() | Adds automatic response caching for queries implementing the `ICachedQuery` interface. If a cached response exists, it is returned immediately. Otherwise, the request is executed normally and the result is stored in the cache. Cache invalidation requires manual intervention. |
| 7 | Transactional | UseTransactions() | Wraps the entire handler (`ICommandHandler`) in a transaction. Uses a retry strategy. |

Each behavior is registered as an open generic against `IPipelineBehavior<,>`, so it applies to all requests. Behaviors that only make sense for a subset of requests constrain themselves through generic type constraints (e.g. `CachingBehavior<TRequest, TResponse>` requires `TRequest : ICachedQuery`)

### Behaviors

The handler-based system differs somewhat from the conventional approach, where request processing is performed at the controller or Minimal API level. This system uses handlers as the primary execution unit for requests, while controllers are responsible only for delegating requests from the HTTP layer. As a result, all core request-processing logic is implemented at the handler level and is wrapped by behaviors. This includes request validation, logging, authorization, and diagnostics. To enable a specific behavior, the corresponding registration method must be called. Once registered, the system automatically includes the required logic in the behaviors pipeline.

### Usage

To start using CQS, add the `HauteCouture.Shared.CQS.Abstractions` package to the project containing handlers. This package provides two primary handler interfaces: `ICommandHandler<TCommand>` / `ICommandHandler<TCommand, TResponse>` and `IQueryHandler<TQuery, TResponse>`, which serve as the foundation for all requests.
In addition, the package includes several supporting components. `IAuthorizationHandler<TRequest>` (use `BaseRoleAuthorizationHandler<TRequest>`) is responsible for authorization. Within it, call the `RequireRole` method to apply role-based restrictions to the handler. There is also `IDiagnosticHandler<TRequest>` / `IDiagnosticHandler<TRequest, TResponse>` (use `BaseDiagnosticHandler<TRequest, TResponse>`), which is responsible for tracing the execution process. It provides a set of methods for adding tags, events, and baggage to the current activity. Finally, `ICorrelationIdAccessor` is available for retrieving the current correlation ID, enabling end-to-end request tracing across distributed systems.