using HauteCouture.Example.Contracts.Responses;
using HauteCouture.Shared.CQS.Primitives.Queries;

namespace HauteCouture.Example.Applications.Handlers.Queries.GetSomething;

/// <summary>
///     Query to retrieve a single <c>Something</c> by its identifier.
/// </summary>
public sealed record GetSomethingQuery(
    Guid SomethingId)
    : IQuery<SomethingResponse>, ICachedQuery
{
    private const string CacheKeyPrefix = "something:";
    private static readonly TimeSpan? ExpirationTime = TimeSpan.FromMinutes(2);

    /// <inheritdoc />
    public string CacheKey => BuildCacheKey(SomethingId);

    /// <inheritdoc />
    public TimeSpan? Expiration => ExpirationTime;

    /// <summary>
    ///     Builds the cache key for invalidating the cache entry during updates.
    /// </summary>
    public static string BuildCacheKey(Guid somethingId) => $"{CacheKeyPrefix}{somethingId:N}";
}