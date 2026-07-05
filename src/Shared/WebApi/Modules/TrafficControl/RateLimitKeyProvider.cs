using Microsoft.AspNetCore.Http;

namespace HauteCouture.Shared.WebApi.Modules.TrafficControl;

/// <summary>
///     Provides methods to generate rate limit keys.
/// </summary>
public static class RateLimitKeyProvider
{
    /// <summary>
    ///     Generates a rate limit key based on current http context.
    /// </summary>
    /// <param name="httpContext">Http context.</param>
    /// <returns>Generated rate limit key.</returns>
    public static string FromHttpContext(HttpContext httpContext)
    {
        var ipAddress = GetClientIp(httpContext);
        var method = httpContext.Request.Method.ToUpperInvariant();
        var path = httpContext.Request.Path.Value?.ToLowerInvariant() ?? "/";

        return $"{ipAddress}:{method}:{path}";
    }

    /// <summary>
    ///     Generates a rate limit key based on provided parts.
    /// </summary>
    /// <param name="identifier">Identifier.</param>
    /// <param name="method">Http method.</param>
    /// <param name="path">Request path.</param>
    /// <returns>Generated rate limit key.</returns>
    public static string FromParts(string identifier, string method, string path) =>
        $"{identifier}:{method.ToUpperInvariant()}:{path.ToLowerInvariant()}";

    private static string GetClientIp(HttpContext httpContext)
    {
        var forwaded = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(forwaded))
        {
            var firstIp = forwaded?.Split(',', StringSplitOptions.TrimEntries).FirstOrDefault();

            if(!string.IsNullOrEmpty(firstIp))
            {
                return firstIp;
            }
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}