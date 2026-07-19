using HauteCouture.Example.Contracts.Mappers;
using HauteCouture.Example.Contracts.Responses;
using HauteCouture.Example.Domain.Entities;
using HauteCouture.Example.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HauteCouture.Example.Applications.Services;

/// <summary>
///     Pre-compiled EF Core queries for <see cref="Something"/>,
///     used to bypass per-call expression tree compilation on hot-path reads.
/// </summary>
public static class SomethingCompiledQueries
{
    /// <summary>
    ///     Retrieves a single <see cref="Something"/> by its identifier, projected as a response,
    ///     returning <see langword="null"/> if no match is found.
    /// </summary>
    public static readonly Func<DbContext, SomethingId, CancellationToken, Task<SomethingResponse?>> GetResponseByIdAsync =
        EF.CompileAsyncQuery((DbContext context, SomethingId id, CancellationToken _) =>
            context.Set<Something>()
                .Where(s => s.Id == id)
                .Select(s => s.ToResponse())
                .FirstOrDefault());
}