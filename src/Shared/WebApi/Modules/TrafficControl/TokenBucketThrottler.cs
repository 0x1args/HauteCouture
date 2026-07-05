using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace HauteCouture.Shared.WebApi.Modules.TrafficControl;

/// <summary>
///     Token bucket throttler implementation that allows for bursts of requests 
///     while maintaining an average rate over time.
/// </summary>
public sealed class TokenBucketThrottler(
    IDistributedCache cache,
    IOptions<ThrottleOptions> options) : IThrottler
{
    private readonly ThrottleOptions _options = options.Value;
    private double _bucketCapacity => _options.RequestsPerSecond *  _options.BurstSeconds;
    private double _refillRate => _options.RequestsPerSecond;

    /// <inheritdoc/>
    public async Task<double?> TryConsumeAsync(string key, CancellationToken cancellationToken)
    {
        var cacheKey = $"tb:{key}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;

        var state = await GetBucketStateAsync(cacheKey, cancellationToken) 
            ?? new BucketState(_bucketCapacity, now);

        // Refill tokens based on time elapsed since last request.
        var elapsed = Math.Max(0, now - state.LastRefillAt);
        var refilled = Math.Min(_bucketCapacity, state.Tokens + elapsed * _refillRate);

        if (refilled < 1.0)
        {
            // Not enough tokens, calculate wait time.
            var waitSeconds = (1.0 - refilled) / _refillRate;
            return Math.Ceiling(waitSeconds);
        }

        var newState = new BucketState(refilled - 1.0, now);
        await SetBucketStateAsync(cacheKey, newState, cancellationToken);

        return null;
    }

    private async Task<BucketState?> GetBucketStateAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var bytes = await cache.GetAsync(key, cancellationToken);
        
        if (bytes is null or { Length: 0 })
        {
            return null;
        }

        return JsonSerializer.Deserialize<BucketState>(bytes)!;
    }

    private async Task SetBucketStateAsync(
        string key,
        BucketState state, 
        CancellationToken cancellationToken)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(state);
        var ttlSeconds = (int)Math.Ceiling(_bucketCapacity / _refillRate) + 5;

        await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
        }, cancellationToken);
    }
}