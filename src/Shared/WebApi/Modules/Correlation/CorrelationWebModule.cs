using HauteCouture.Shared.CQS.Abstractions.Handlers;
using HauteCouture.Shared.WebApi.Modules.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HauteCouture.Shared.WebApi.Modules.Correlation;

/// <summary>
///     Web module responsible for setting up correlation ID 
///     handling across the application.
/// </summary>
public sealed class CorrelationWebModule : IWebModule
{
    /// <inheritdoc />
    public int Order => WebModuleOrder.Correlation;

    /// <inheritdoc />
    public void MountServices(WebModuleContext context)
    {
        context.Services
            .AddHttpContextAccessor()
            .AddScoped<CorrelationIdAccessor>() // Required for middleware.
            .AddScoped<ICorrelationIdAccessor>(sp =>
                sp.GetRequiredService<CorrelationIdAccessor>()); // Required for CQS handlers.
    }

    /// <inheritdoc />
    public void MountPipeline(IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
    }
}