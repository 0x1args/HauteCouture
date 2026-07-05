using HauteCouture.Shared.WebApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HauteCouture.Shared.WebApi.Modules.Correlation;

/// <summary>
///     Middleware responsible for managing correlation IDs across HTTP requests.
/// </summary>
public sealed class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    private const string CorrelationIdKey = "CorrelationId";

    /// <inheritdoc cref="IMiddleware.InvokeAsync"/>
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Resolve(context.Request);

        var accessor = context.RequestServices.GetRequiredService<CorrelationIdAccessor>();
        accessor.CorrelationId = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.TryAdd(
                CorrelationIdHelper.DefaultHeaderName,
                correlationId.ToString("D"));
            return Task.CompletedTask;
        });

        using (logger.BeginScope(new Dictionary<string, object>
        {
            [CorrelationIdKey] = correlationId
        }))
        {
            await next(context);
        }
    }

    /// <summary>
    ///     Resolves the correlation ID from the request header.
    /// </summary>
    private static Guid Resolve(HttpRequest request)
    {
        var fromHeader = request.TryReadFromHeader(CorrelationIdHelper.DefaultHeaderName);

        if (fromHeader is not null && Guid.TryParse(fromHeader, out var parsed))
        {
            return parsed;
        }

        var generated = Guid.NewGuid();
        return generated;
    }
}