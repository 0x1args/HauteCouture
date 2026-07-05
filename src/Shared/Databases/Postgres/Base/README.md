## Postgres

Contains the necessary tools for working with a PostgreSQL relational database using [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) as the primary ORM in the form of extensions, adding another layer of abstractions.

### Registration

To register all the tools in DI, add the `Shared.Databases.Postgres` package to the registration point and call the appropriate method from a set of overloads. All overloads can be found here. In short, you can add the built-in tools (which is recommended), or if needed provide your own Repository or set the required DbContext Pool Size (128 by default). All methods register a `DbContextPool`. All registration examples:

```csharp
// Pooled DbContext + built-in IRepository.
services.AddPostgres<AppDbContext, AppDbContextConfigurator>();

// Pooled DbContext + built-in IRepository with an explicit pool size.
services.AddPostgres<AppDbContext, AppDbContextConfigurator>(poolSize: 256);

// Pooled DbContext + custom repository registration.
services.AddPostgres<AppDbContext, AppDbContextConfigurator>(repositories =>
{
    repositories.AddScoped(typof(IRepository<>), typof(MyRepository));
});

// Pooled DbContext + custom repository registration with an explicit pool size.
services.AddPostgres<AppDbContext, AppDbContextConfigurator>(poolSize: 256, repositories =>
{
    repositories.AddScoped(typof(IRepository<>), typof(MyRepository));
});
```

The DbContext configurator applies settings with default values, and the DbContext configuration is fully open to changes through virtual properties and methods — simply override them as needed. What the configurator sets up by default: `CommandTimeoutSeconds` (60s); `MaxBatchSize` (256); `MaxRetryCount` (3); `MaxRetryDelay` (10s); `SlowQueryThreshold` (2s); `EnableSensitiveDataLogging` (false); `EnableDetailedErrors` (false).

| Property | Default | Description |
|---|---|---|
| ConnectionString | — | Connection string name from configuration *(required)* |
| CommandTimeoutSeconds | 60 | Maximum command execution time before timeout |
| MaxBatchSize | 256 | Maximum number of SQL statements in a single round-trip during SaveChangesAsync |
| MaxRetryCount | 3 | Number of retry attempts on transient failures |
| MaxRetryDelay | 10s | Maximum delay between retry attempts |
| SlowQueryThreshold | 2s | Minimum query execution time to be logged as slow |
| EnableSensitiveDataLogging | false | Logging of parameter values in SQL |
| EnableDetailedErrors | false | Detailed EF Core error messages |

The configurator also applies snake_case naming conventions for all tables and columns and enables retry-on-failure for transient connection failures.

### Repository

By default, the generic `IRepository<TEntity, TId>` is used for data access. It combines the `ICrudRepository<TEntity>` interface (for data modification operations) and the `IQueryRepository<TEntity, TId>` interface (for data retrieval operations). All data modification methods automatically call `SaveChangesAsync`, removing the need for an explicit UnitOfWork. The repository can be used directly in handlers or services without the need to create entity-specific implementations with repetitive logic. To use this interface, add the `Shared.Databases.Postgres.Abstractions` package.

### Pagination

`IQueryRepository<TEntity, TId>` contains two `PageAsync` overloads for offset-based pagination using `PagedFilter`. Under the hood, two round-trips to the database are performed: one for the total record count (`COUNT`), and one for the page items (`SKIP`/`TAKE`). For large tables where pagination depth is unpredictable — keyset pagination is recommended, which eliminates performance degradation on deep pages.

Keyset pagination uses the last seen value as a cursor instead of counting rows to skip, which allows the database to perform an index seek directly to the required position. This provides constant query performance regardless of how deep into the dataset the caller has navigated. The approach also eliminates the `COUNT` round-trip entirely — instead, `pageSize + 1` records are fetched, and if more than `pageSize` are returned, it means there is a next page.

```csharp
// Simple case, unique sort key.
var page = await repository
    .Where(o => o.TenantId == tenantId && o.Id > lastSeenId)
    .OrderBy(o => o.Id)
    .AsNoTracking()
    .ToKeysetPageAsync(pageSize, cancellationToken);

// Non-unique sort key, composite cursor with tie-breaker.
var page = await repository
    .Where(o => o.TenantId == tenantId
        && (o.CreatedAt > lastSeenDate
            || (o.CreatedAt == lastSeenDate && o.Id > lastSeenId)))
    .OrderBy(o => o.CreatedAt)
    .ThenBy(o => o.Id)
    .AsNoTracking()
    .ToKeysetPageAsync(pageSize, cancellationToken);
```

Keyset pagination is available as a `ToKeysetPageAsync` extension method on `IQueryable<T>` from `QueryableExtensions`. The caller is responsible for applying the cursor filter and `ORDER BY` before calling the method. The sort key must be unique; for non-unique sort columns, add the primary key as a tie-breaker both in the `WHERE` predicate and in `ORDER BY`.

| | Offset (`PageAsync`) | Keyset (`ToKeysetPageAsync`) |
|---|---|---|
| Round-trips | 2 (`COUNT` + data) | 1 (data only) |
| Performance at depth | Degrades linearly | Constant |
| Arbitrary page jump | ✓ | ✗ |
| Total record count | ✓ | ✗ |

### Transactions

There is an `ITransactionalScope` with a single `BeginTransactionAsync` method for working with transactions. If a transaction is already open on the current `DbContext` — a wrapper over the existing one is returned rather than a new nested transaction. By default, `ITransactionalScope` is used in `TransactionalBehavior` from the `Shared.CQS` package — all command handlers are automatically wrapped in a transaction, so manual transaction management is not needed in the vast majority of cases.

### Converters

All `DateTimeOffset` and `DateTimeOffset?` properties in the model can be normalized to a uniform `DateTimeKind` via `SetDefaultDateTimeKind` on `ModelBuilder`. Converters are cached statically through `ConcurrentDictionary`, so the overhead during model configuration is minimal.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
}
```