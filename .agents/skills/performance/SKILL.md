---
name: aspnetcore-performance
description: Reviews and optimizes code for memory allocations, GC pressure, ThreadPool health, async/await correctness, Span/Memory usage, EF Core query efficiency, and JIT-friendly patterns. Use during code review, hot-path refactoring, and latency/throughput diagnostics.
globs: ["src/**/*.cs"]
---

## Purpose

The goal is to detect and eliminate performance bottlenecks: unnecessary allocations, GC pressure, blocking calls, inefficient LINQ/EF Core queries, thread pool starvation, and incorrect async usage. Every recommendation must include the "why" and, where possible, a measurable effect (allocations/op, ns/op, RPS).

- Code must have high throughput and minimal GC load, this is the guiding metric for any optimization decision in this domain.
- Prefer explicit, measured trade-offs over "clever" micro-optimizations applied uniformly.

### Operating principles

1. Don't optimize blindly. Before concluding, suggest profiling (BenchmarkDotNet, dotnet-trace, dotnet-counters, dotnet-gcdump) or explain why the optimization is obvious without measurement (allocation in a hot loop, N+1 query, etc.).
2. Readability vs performance. Micro-optimizations (stackalloc, aggressive inlining, ref struct) should only be applied to proven hot paths (high RPS endpoints, tight loops), not to ordinary business logic.
3. Always state the trade-off: memory vs CPU, latency vs throughput, code complexity vs gain.
4. Prefer measurable, idiomatic .NET patterns over exotic `unsafe` tricks unless the hot path truly justifies it.

### 1. Memory & GC

- Minimize allocations in hot paths: avoid LINQ (`Select`, `Where`, `ToList` allocate iterators/collections), string concatenation via `+` in loops, and boxing of value types.
- Do not use string concatenation via `+`; prefer string interpolation** (`$"{a}-{b}"`) or `StringBuilder` for loops: interpolation compiles to efficient `string.Format`/`DefaultInterpolatedStringHandler` paths and avoids intermediate allocations better than chained `+`.
- Avoid intermediate string allocations. Chained string operations (`.Trim().ToLower().Replace(...)`) each allocate a new string, combine logic or operate on spans where possible.
- Do not use `Substring`, `IndexOf`, `ToCharArray` where a `ReadOnlySpan<char>`-based alternative exists, `Substring` allocates a new string, `ToCharArray` allocates a new array. Use `AsSpan()`, `Slice()`, and span-based `IndexOf`/comparison overloads instead.
- Avoid creating temporary arrays where possible, prefer `Span<T>`, `stackalloc`, or pooled buffers over `new T[n]` in hot code.
- Avoid boxing/unboxing, always use generic collections and generic methods (`List<T>`, `Dictionary<TKey,TValue>`, generic constraints) instead of non-generic (`ArrayList`, `Hashtable`) or `object`-typed APIs.
- Use `struct` for small, short-lived, immutable data (< 16–24 bytes) to avoid heap allocations, but be careful with by-value copying of larger structs (copying can be more expensive than a reference).
- `readonly struct` + `in` parameters avoid defensive copies.
- `ref struct` (e.g. `Span<T>`) is stack-only, cannot escape to the heap, and is incompatible with async methods/lambdas/iterators.
- Keep objects small so they don't end up on the LOH (objects ≥ 85,000 bytes go to the Large Object Heap and fragment it, leaving "holes" that can't be reused). At the same time, don't over-produce short-lived small objects either, that pressures Gen0 GC. Balance object size and allocation rate; use pooling for large, reusable buffers/objects instead of repeatedly allocating and discarding them.
- Account for hidden allocations: closures capturing variables, `params` arrays, `IEnumerable` iterators, async state machines, and interpolated strings falling back to `string.Format` can all allocate invisibly; inspect with a memory profiler or `dotnet-counters`/BenchmarkDotNet's `MemoryDiagnoser`.
- `ArrayPool<T>.Shared` for temporary buffers (serialization, I/O, parsing).
- `Microsoft.Extensions.ObjectPool` (`ObjectPool<T>`, `DefaultObjectPoolProvider`) for reusing expensive objects (e.g. `StringBuilder`).
- Use pooled buffers in general for any short-lived-but-large buffer to reduce Gen0/LOH churn.
- Always `try/finally` when returning to a pool, otherwise you lose the benefit and the object is simply collected normally.
- In high-load / proven hot-path scenarios, prefer `Span<T>` and `stackalloc` to eliminate allocations entirely.
- `Span<T>` cannot be used in `async` methods, iterators (`yield`), or class fields, use `Memory<T>` there instead.
- For buffers larger than ~1KB, use `ArrayPool<T>` rather than `stackalloc` (stack overflow risk).

### 2. Concurrency

- Never block ThreadPool threads with synchronous waits (`.Result`, `.Wait()`, `Task.Run(() => syncCode).Wait()`) — this causes thread pool starvation.
- Don't burden the ThreadPool with genuinely long-running operations. Move such work into background services (`IHostedService`/`BackgroundService`) running as separate hosts/processes with their own scheduling, so they don't degrade the Web API's request-handling throughput.
- For CPU-bound work, `Task.Run` is appropriate. For I/O-bound work, do NOT use `Task.Run`, use native async I/O; wrapping I/O in `Task.Run` just wastes a pool thread waiting.
- In multithreaded service code, avoid `lock`. Prefer optimistic concurrency (e.g. row versioning/`RowVersion` + retry, `Interlocked` compare-and-swap patterns) or pessimistic locking at the appropriate layer (e.g. database-level locks/transactions) instead of in-process `lock` statements, which serialize access and don't compose well with async code (never `lock` around an `await`).
- Alternatives to manual locking:
  - `SemaphoreSlim` — async-compatible mutual exclusion when synchronization is unavoidable.
  - `Interlocked` — atomic counters/CAS without blocking.
  - `System.Threading.Channels` — for producer/consumer instead of manual `lock` + `Queue`.
  - `ConcurrentDictionary`/`ConcurrentQueue` — lock-free/fine-grained-locking collections.
- If the same computed result is likely to be reused within an operation or across requests, cache it in a `ConcurrentDictionary`** (or a proper distributed/memory cache abstraction) rather than recomputing it.
- Avoid `Thread.Sleep` in async contexts, use `Task.Delay`.
- Avoid deadlocks. Never mix sync-over-async (`.Result`/`.Wait()` on code that also awaits back onto the same context) without full understanding of the synchronization context. Establish a consistent lock-acquisition order when multiple locks are unavoidable.
- Always return `Task` for asynchronous operations. Where appropriate, use `ValueTask`, but this choice must be justified: `ValueTask` avoids an allocation only when the method frequently completes synchronously (cache hit, already-available result). If a `ValueTask` will later be converted back to `Task` (e.g. via `.AsTask()`) or awaited multiple times / stored, you lose the benefit and add complexity for nothing, in that case just use `Task`.
- `IAsyncEnumerable<T>` for streaming large result sets without buffering everything in memory (`await foreach`).
- Use `Task.WhenAll` for independent parallel I/O operations instead of sequential `await`s.

### 3. EF Core

- Prefer `DbContext` pooling (`AddDbContextPool<TContext>`) over `AddDbContext` for high-throughput APIs, it reuses `DbContext` instances and avoids repeated construction/DI overhead, as long as the context has no per-request mutable state that isn't reset.
- Use compiled queries for repeated query patterns: caches the LINQ --> SQL translation for queries executed very frequently with the same shape.
- Use query caching and parameterization correctly. Pass variables into the query instead of embedding literal values, so EF Core's query cache produces one cached plan instead of a new compilation per distinct literal (e.g. `ctx.Users.Where(u => u.Id == id)` with `id` as a parameter, not a hardcoded constant baked in via string interpolation or constant folding).
- Retrieve a projection instead of the full entity: use `.Select()` to fetch only the columns actually needed. This reduces data transferred, memory usage, and avoids unnecessary change-tracking overhead.
- For read-only queries, apply `.AsNoTracking()`, skips change tracking, reducing memory and CPU cost.
- Always apply eager loading to avoid N+1 problems, use `.Include()`/`.ThenInclude()` explicitly rather than relying on lazy loading triggered per-item in a loop.
- For queries with many includes, watch out for cartesian explosion (multiple one-to-many `Include`s multiply row counts) apply `.AsSplitQuery()` to issue separate SQL queries per collection instead of one exploding join.
- EF Core defaults string columns to `nvarchar(max)`. Always set `MaxLength` (via Fluent API `.HasMaxLength(n)`) when configuring models; this affects storage, indexing efficiency, and prevents unbounded column sizes.
- Use `ExecuteUpdateAsync`/`ExecuteDeleteAsync` for bulk changes instead of loading entities into memory, mutating them in a loop, and calling `SaveChanges`, this executes a single set-based SQL statement.
- Index columns correctly. Use composite indexes with the correct column order (most selective / equality-filtered columns first, range-filtered columns after) so range queries can still use the index efficiently.
- Do not materialize all data in memory, use pagination (`Skip`/`Take`, keyset/cursor pagination for large datasets) instead of loading entire tables.
- `DbContext` is short-lived and scoped per request, don't hold it long-term or share it across threads.
- Watch the generated SQL (`.ToQueryString()` or logging), LINQ expressions that can't be translated silently execute client-side with a major performance penalty.
- Adhere to ACID properties when using transactions (`ctx.Database.BeginTransactionAsync()` or ambient transactions), ensure atomicity and consistency for multi-step writes, and keep transaction scope as short as possible to reduce lock contention and deadlock risk.

### 4. JIT, inlining & general code shape

- Write JIT-friendly code: keep hot methods small, avoid virtual dispatch and excessive polymorphism in tight loops, avoid unnecessary try/catch in hot paths (exception handling regions can inhibit some optimizations).
- Use `[MethodImpl(MethodImplOptions.AggressiveInlining)]` as a hint for small, frequently-called methods (getters, small hot-path helpers), the JIT may still decline for large methods.
- By default, mark classes that will not have descendants as `sealed`. This allows the JIT to devirtualize calls and inline more aggressively, and communicates design intent.
- Avoid inheritance as an OOP principle where possible, prefer composition. Deep inheritance hierarchies add virtual dispatch overhead and hurt inlining.
- Don't overuse reflection, `Type.GetProperty`, `Activator.CreateInstance`, dynamic invocation, etc. are slow relative to direct calls.

### 5. Diagnostics & tooling

| Tool | Purpose |
|---|---|
| `BenchmarkDotNet` | Microbenchmarks, allocations/op, precise measurements |
| `dotnet-trace` | CPU/ETW tracing, analysis in PerfView/Speedscope |
| `dotnet-counters` | Live monitoring of GC, ThreadPool, exceptions, contention |
| `dotnet-gcdump` | Heap dumps, memory leak analysis |
| `dotnet-dump` | Deadlock/hang analysis |

#### Key dotnet-counters metrics
- `gc-heap-size`, `gen-0-gc-count`, `gen-2-gc-count` — GC pressure
- `threadpool-thread-count`, `threadpool-queue-length` — starvation signals
- `monitor-lock-contention-count` — lock contention