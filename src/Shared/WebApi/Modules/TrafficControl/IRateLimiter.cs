namespace HauteCouture.Shared.WebApi.Modules.TrafficControl;

/// <summary>
///     Window rate limiter for controlling the number of requests allowed within a specified time window.
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    ///     Checks if the request associated with the given key is allowed based on the rate limiting rules.
    /// </summary>
    /// <param name="key">Unique key built from IP + http method + path.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Rate limit result indicating whether the request is allowed.</returns>
    Task<RateLimitResult> CheckAsync(string key, CancellationToken cancellationToken);
}