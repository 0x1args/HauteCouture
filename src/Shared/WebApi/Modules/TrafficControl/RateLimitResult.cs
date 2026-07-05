namespace HauteCouture.Shared.WebApi.Modules.TrafficControl;

/// <summary>
///     Result of a rate limit check.
/// </summary>
public sealed record RateLimitResult(
    bool IsAllowed,
    int CurrentCount,
    int MaxRequests,
    int Remaining,
    DateTimeOffset ResetsAt)
{
    /// <summary>
    ///     Creates a new instance of <see cref="RateLimitResult"/> representing an allowed request.
    /// </summary>
    public static RateLimitResult Allowed(int currentCount, int maxRequests, DateTimeOffset resetAt) =>
        new(true, currentCount, maxRequests, Math.Max(0, maxRequests - currentCount), resetAt);

    /// <summary>
    ///     Creates a new instance of <see cref="RateLimitResult"/> representing a blocked request.
    /// </summary>
    public static RateLimitResult Blocked(int currentCount, int maxRequests, DateTimeOffset resetAt) =>
        new(false, currentCount, maxRequests, 0, resetAt);
}