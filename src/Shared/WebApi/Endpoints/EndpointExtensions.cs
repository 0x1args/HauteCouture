using HauteCouture.Shared.Common.Exceptions.Client;
using HauteCouture.Shared.WebApi.Modules.TrafficControl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HauteCouture.Shared.WebApi.Endpoints;

/// <summary>
///     Extensions for endpoint configuration.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    ///     Applies the global <see cref="RateLimitOptions"/> rate limiter to this endpoint.
    /// </summary>
    public static RouteHandlerBuilder WithRateLimit(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var limiter = context.HttpContext.RequestServices.GetRequiredService<IRateLimiter>();
            var rateLimitOptions = context.HttpContext.RequestServices
                .GetRequiredService<IOptions<RateLimitOptions>>()
                .Value;

            var key = RateLimitKeyProvider.FromHttpContext(context.HttpContext);
            var result = await limiter.CheckAsync(key, context.HttpContext.RequestAborted);

            if (rateLimitOptions.AddRateLimitHeaders)
            {
                ApplyRateLimitHeaders(context.HttpContext, result);
            }

            if (!result.IsAllowed)
            {
                var retryAfterSeconds = (int)Math.Ceiling((result.ResetsAt - DateTimeOffset.UtcNow).TotalSeconds);
                throw new TooManyRequestsException(BuildRateLimitMessage(retryAfterSeconds));
            }

            return await next(context);
        });

        return builder;
    }

    /// <summary>
    ///     Applies a per-endpoint rate limiter that overrides the global options.
    /// </summary>
    public static RouteHandlerBuilder WithRateLimit(
        this RouteHandlerBuilder builder,
        int maxRequests,
        int windowSeconds = 60)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

            var localOptions = Options.Create(new RateLimitOptions
            {
                MaxRequests = maxRequests,
                WindowSizeInSeconds = windowSeconds,
                AddRateLimitHeaders = true
            });

            var limiter = new SlidingWindowRateLimiter(cache, localOptions);
            var key = RateLimitKeyProvider.FromHttpContext(context.HttpContext);
            var result = await limiter.CheckAsync(key, context.HttpContext.RequestAborted);

            ApplyRateLimitHeaders(context.HttpContext, result);

            if (!result.IsAllowed)
            {
                var retryAfterSeconds = (int)Math.Ceiling((result.ResetsAt - DateTimeOffset.UtcNow).TotalSeconds);
                throw new TooManyRequestsException(BuildRateLimitMessage(retryAfterSeconds));
            }

            return await next(context);
        });

        return builder;
    }

    /// <summary>
    ///     Applies the global <see cref="ThrottleOptions"/> throttler to this endpoint.
    /// </summary>
    public static RouteHandlerBuilder WithThrottle(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var throttler = context.HttpContext.RequestServices.GetRequiredService<IThrottler>();
            var throttleOptions = context.HttpContext.RequestServices
                .GetRequiredService<IOptions<ThrottleOptions>>().Value;

            var key = RateLimitKeyProvider.FromHttpContext(context.HttpContext);
            var waitSeconds = await throttler.TryConsumeAsync(key, context.HttpContext.RequestAborted);

            if (waitSeconds.HasValue)
            {
                if (throttleOptions.AddRetryAfterHeader)
                {
                    context.HttpContext.Response.Headers["Retry-After"] = Math.Ceiling(waitSeconds.Value).ToString();
                }

                throw new TooManyRequestsException(BuildThrottleMessage(waitSeconds.Value));
            }

            return await next(context);
        });

        return builder;
    }

    /// <summary>
    ///     Applies a per-endpoint throttler that overrides the global options.
    /// </summary>
    public static RouteHandlerBuilder WithThrottle(
        this RouteHandlerBuilder builder,
        int rps,
        double burstSeconds = 1.0)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

            var localOptions = Options.Create(new ThrottleOptions
            {
                RequestsPerSecond = rps,
                BurstSeconds = burstSeconds,
                AddRetryAfterHeader = true
            });

            var throttler = new TokenBucketThrottler(cache, localOptions);
            var key = RateLimitKeyProvider.FromHttpContext(context.HttpContext);
            var waitSeconds = await throttler.TryConsumeAsync(key, context.HttpContext.RequestAborted);

            if (waitSeconds.HasValue)
            {
                context.HttpContext.Response.Headers["Retry-After"] = Math.Ceiling(waitSeconds.Value).ToString();
                throw new TooManyRequestsException(BuildThrottleMessage(waitSeconds.Value));
            }

            return await next(context);
        });

        return builder;
    }

    private static void ApplyRateLimitHeaders(HttpContext httpContext, RateLimitResult result)
    {
        httpContext.Response.Headers["X-RateLimit-Limit"] = result.MaxRequests.ToString();
        httpContext.Response.Headers["X-RateLimit-Remaining"] = result.Remaining.ToString();
        httpContext.Response.Headers["X-RateLimit-Reset"] =
            result.ResetsAt.ToUnixTimeSeconds().ToString();
    }

    private static string BuildRateLimitMessage(double retryAfterSeconds) => 
        $"You have exceeded the allowed number of requests. Please try again in {retryAfterSeconds} seconds.";

    private static string BuildThrottleMessage(double retryAfterSeconds) =>
        $"You are sending requests too fast. Please try again in {retryAfterSeconds} seconds.";
}