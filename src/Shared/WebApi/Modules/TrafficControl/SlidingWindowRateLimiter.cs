using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace HauteCouture.Shared.WebApi.Modules.TrafficControl;

/// <summary>
///     Sliding window rate limiter implementation.
/// </summary>
public sealed class SlidingWindowRateLimiter(
    IDistributedCache cache,
    IOptions<RateLimitOptions> options) : IRateLimiter
{
    private readonly RateLimitOptions _options = options.Value;

    /// <inheritdoc/>
    public async Task<RateLimitResult> CheckAsync(string key, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var (currentWindow, previousWindow) = GetWindows(now);

        var currentKey = BuildCacheKey(key, currentWindow);
        var previousKey = BuildCacheKey(key, previousWindow);

        var (currentCount, previousCount) = await GetCountsAsync(
            currentKey,
            previousKey,
            cancellationToken);

        var overlapWeight = ComputeOverlapWeight(now);
        var effectiveCount = currentCount + (int)(previousCount * overlapWeight);

        // Calculate when the current window expires for Retry-After / X-RateLimit-Reset.
        var windowStart = now.ToUnixTimeSeconds() / _options.WindowSizeInSeconds * _options.WindowSizeInSeconds;
        var resetAt = DateTimeOffset.FromUnixTimeSeconds(windowStart + _options.WindowSizeInSeconds);

        if (effectiveCount >= _options.MaxRequests)
        {
            return RateLimitResult.Blocked(currentCount, _options.MaxRequests, resetAt);
        }

        await IncrementAsync(currentKey, currentCount, cancellationToken);
        return RateLimitResult.Allowed(effectiveCount + 1, _options.MaxRequests, resetAt);
    }

    private async Task<(int current, int previous)> GetCountsAsync(
        string key,
        string previousKey,
        CancellationToken cancellationToken)
    {
        var currentTask = GetCounterAsync(key, cancellationToken);
        var previousTask = GetCounterAsync(previousKey, cancellationToken);

        await Task.WhenAll(currentTask, previousTask);
        return (await currentTask, await previousTask);
    }

    private async Task<int> GetCounterAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var bytes = await cache.GetAsync(key, cancellationToken);

        if (bytes is null or { Length: 0 })
        {
            return 0;
        }

        return JsonSerializer.Deserialize<int>(bytes);
    }

    private async Task IncrementAsync(
        string key,
        int currentCount, 
        CancellationToken cancellationToken)
    {
        var newCount = currentCount + 1;
        var bytes = JsonSerializer.SerializeToUtf8Bytes(newCount);

        // TTL = 2 windows so the previous window entry is still readable when we cross the boundary.
        await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.WindowSizeInSeconds * 2)
        }, cancellationToken);
    }

    private (long current, long previous) GetWindows(DateTimeOffset now)
    {
        var unixSeconds = now.ToUnixTimeSeconds();
        var currentWindow = unixSeconds / _options.WindowSizeInSeconds;

        return (currentWindow, currentWindow - 1);
    }

    private double ComputeOverlapWeight(DateTimeOffset now)
    {
        var windowsSizeInSeconds = _options.WindowSizeInSeconds;

        // k = 1 - (elapsedInCurrentWindow / windowSize).
        // At the very start of a window -> k ≈ 1.0 (previous window still counts relevant).
        // At the very end of a window -> k ≈ 0.0 (previous window is almost irrelevant).
        var unixSeconds = now.ToUnixTimeSeconds();
        var windowStart = (unixSeconds / windowsSizeInSeconds) * windowsSizeInSeconds;
        var elapsed = unixSeconds - windowStart;
        var weight = 1.0 - ((double)elapsed / windowsSizeInSeconds);

        return Math.Clamp(weight, 0.0, 1.0);
    }

    private static string BuildCacheKey(string key, long window) => 
        $"rl:{key}:{window}";
}