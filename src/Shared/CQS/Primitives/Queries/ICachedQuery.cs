namespace HauteCouture.Shared.CQS.Primitives.Queries;

/// <summary>
///     Indicates that a query result can be cached.
/// </summary>
public interface ICachedQuery
{
    /// <summary>
    ///     Unique key used to store and retrieve the cached result.
    ///     </summary>
    string CacheKey { get; }

    /// <summary>
    ///     Cache entry lifetime. If <see langword="null"/>, 
    ///     the default expiration policy is used.
    /// </summary>
    TimeSpan? Expiration { get; }
}