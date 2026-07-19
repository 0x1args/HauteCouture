## WebApi

Contains the necessary tools for building the HTTP layer of a service: a modular pipeline of self-contained infrastructure concerns (`IWebModule`), a reflection-based endpoint registration system (`IEndpoint`), request throttling/rate limiting, global exception handling, and aggregated health checks.

### Registration

To register all the tools in DI, add the `Shared.WebApi` package to the registration point and call `AddWebModules` with a configuration delegate, passing the ASP.NET Core `IConfiguration`, `IWebHostEnvironment`, and `IHostBuilder`, some modules (e.g. `LoggingWebModule`) need to configure the host itself and not just the DI container. Every module is opt-in and disabled by default:

```csharp
// Enable a specific subset of modules.
builder.Services.AddWebModules(
    builder.Configuration,
    builder.Environment,
    builder.Host,
    web => web
        .UseLogging()
        .UseCorrelation()
        .UseExceptionHandling()
        .UseHealthChecks(hc => hc
            .UseApi()
            .UsePostgres(new PostgresHealthCheckOptions())
            .UseRedis(new RedisHealthCheckOptions())));

// Enable every built-in module at once.
builder.Services.AddWebModules(
    builder.Configuration,
    builder.Environment,
    builder.Host,
    web => web.UseAllModules());

// Discover additional IWebModule implementations by scanning assemblies,
// and/or register explicit module instances alongside the built-in ones.
builder.Services.AddWebModules(
    builder.Configuration,
    builder.Environment,
    builder.Host,
    web => web
        .UseLogging()
        .FromAssemblies(typeof(Program).Assembly)
        .WithModules(new MyCustomModule()));
```

Modules discovered via `FromAssemblies` or supplied via `WithModules` are combined with the built-in ones enabled through `UseX()`, deduplicated by type, and sorted by `IWebModule.Order` before `MountServices` runs, the registration method call order does not matter, only each module's `Order` value does.

Each built-in module can also be registered independently of `AddWebModules`, without a `WebApiOptions` delegate:

```csharp
services
    .AddLoggingModule(configuration, environment, host)
    .AddCorrelationModule(configuration, environment, host)
    .AddCachingModule(configuration, environment, host)
    .AddTrafficControlModule(configuration, environment, host)
    .AddExceptionHandlingModule(configuration, environment, host);
```

These convenience methods append to the same underlying `IReadOnlyList<IWebModule>` singleton that `AddWebModules` populates (guarding against duplicate registrations of the same module type), so the two registration styles can be freely mixed.

Once services are registered, mount the middleware pipeline for every registered module in a single call, in your `Program.cs`, after `app` is built:

```csharp
app.UseWebModules();
```

This resolves the registered `IReadOnlyList<IWebModule>` and invokes `MountPipeline` on each module in `Order` sequence.

### Modules

### Modules

| Order | Module | Requires | Description |
|---|---|---|---|
| 50 | Caching | UseCaching | Configures the Redis-backed distributed cache used by other modules. |
| 100 | Logging | UseLogging | Configures Serilog and request logging middleware. |
| 200 | Correlation | UseCorrelation | Assigns and propagates a correlation ID for each request. |
| 300 | ExceptionHandling | UseExceptionHandling | Maps unhandled exceptions to standardized `ProblemDetails` responses. |
| 400 | OpenTelemetry | UseOpenTelemetry *(Not implemented yet)* | Will add distributed tracing/metrics export via OpenTelemetry. |
| 500 | TrafficControl | UseTrafficControl | Registers the sliding window rate limiter used by `WithRateLimit()`. Registers the token bucket throttler used by `WithThrottle()`. |
| 600 | Authorization | UseAuthorization *(Not implemented yet)* | Will add authentication/authorization middleware setup. |
| 700 | Swagger | UseSwagger *(Not implemented yet)* | Will add Swagger/OpenAPI generation and UI. |
| 800 | HealthChecks | UseHealthChecks | Exposes the aggregated `/health` endpoint. |


### Rate limiting and throttling (endpoint-level)

Two independent mechanisms are available as `RouteHandlerBuilder` extension methods, applied per-endpoint rather than globally:

```csharp
app.MapGet("/orders/{id}", GetOrder)
    .WithRateLimit()
    .WithThrottle();

app.MapPost("/orders", CreateOrder)
    .WithRateLimit(maxRequests: 10, windowSeconds: 60)
    .WithThrottle(rps: 5, burstSeconds: 2.0);
```

`WithRateLimit` (sliding window) counts requests across two adjacent fixed windows and blends them with a linear overlap weight, approximating a true sliding window without storing a timestamp per request. When the global overload is used, `X-RateLimit-Limit`/`-Remaining`/`-Reset` headers are added only if `RateLimitOptions.AddRateLimitHeaders` is set; the per-endpoint overload always adds them. Exceeding the limit throws `TooManyRequestsException`, which `GlobalExceptionHandler` maps to `429 Too Many Requests`.

`WithThrottle` (token bucket) allows short bursts up to `RequestsPerSecond * BurstSeconds` tokens while enforcing an average rate of `RequestsPerSecond` over time, refilling continuously based on elapsed time rather than on a fixed schedule. When tokens are exhausted, it computes the wait time until at least one token is available and throws `TooManyRequestsException`; a `Retry-After` header (seconds, rounded up) is set on the response in both overloads.

Both limiters key requests via `RateLimitKeyProvider.FromHttpContext` and share the distributed cache registered by `CachingWebModule` — bucket/window state is stored with a TTL slightly longer than its own window so state remains valid across boundaries.

### Endpoints

`IEndpoint` is the contract for a single HTTP route (or a small group of related routes): implementers register themselves via `MapEndpoint(IEndpointRouteBuilder)`. 

```csharp
services.AddEndpoints(typeof(Program).Assembly);
```

`AddEndpoints` scans the given assembly for non-abstract classes implementing `IEndpoint` and registers them as transient `IEndpoint` services via `TryAddEnumerable`, so calling it more than once (e.g. across multiple assemblies) does not produce duplicate registrations for the same type.

```csharp
app.MapEndpoints();

var api = app.MapGroup("/api/v1");
app.MapEndpoints(api);
```

`MapEndpoints` resolves all registered `IEndpoint` instances and calls `MapEndpoint` on each, targeting either the `WebApplication` itself or a supplied `RouteGroupBuilder`.