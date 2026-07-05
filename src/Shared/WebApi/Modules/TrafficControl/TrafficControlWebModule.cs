using HauteCouture.Shared.WebApi.Modules.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HauteCouture.Shared.WebApi.Modules.TrafficControl;

/// <summary>
///     Web module responsible for setting up request throttling and 
///     rate limiting across the application.
/// </summary>
public sealed class TrafficControlWebModule : IWebModule
{
    /// <inheritdoc />
    public int Order => WebModuleOrder.TrafficControl;

    /// <inheritdoc />
    public void MountServices(WebModuleContext context)
    {
        context.Services
            .Configure<RateLimitOptions>(
                context.Configuration.GetSection(RateLimitOptions.SectionName))
            .Configure<ThrottleOptions>(
                context.Configuration.GetSection(ThrottleOptions.SectionName))
            .AddSingleton<IRateLimiter, SlidingWindowRateLimiter>()
            .AddSingleton<IThrottler, TokenBucketThrottler>();
    }

    /// <inheritdoc />
    public void MountPipeline(IApplicationBuilder app)
    {
    }
}