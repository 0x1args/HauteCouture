using Microsoft.AspNetCore.Http;

namespace HauteCouture.Shared.WebApi.Helpers;

/// <summary>
///     Helper for managing correlation IDs in web requests.
/// </summary>
public static class CorrelationIdHelper
{
    /// <summary>
    ///     Maximum allowed length for a correlation ID value.
    /// </summary>
    private const int MaxLength = 128;

    /// <summary>
    ///     Default HTTP header name used to propagate the correlation ID.
    /// </summary>
    public const string DefaultHeaderName = "X-Correlation-Id";

    /// <summary>
    ///     Alternative header name used by some reverse proxies.
    /// </summary>
    public const string RequestIdHeaderName = "X-Request-Id";

    /// <summary>
    ///     Generates a new unique correlation ID.
    /// </summary>
    public static string Generate() => Guid.NewGuid().ToString("D");

    /// <summary>
    ///     Determines whether the specified value is a valid correlation ID.
    /// </summary>
    public static bool IsValid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value.Length <= MaxLength;

    /// <summary>
    ///     Attempts to read a valid correlation ID from the specified request header.
    /// </summary>
    public static string? TryReadFromHeader(this HttpRequest request, string headerName)
    {
        if (request.Headers.TryGetValue(headerName, out var value) && IsValid(value))
        {
            return value.ToString();
        }

        return null;
    }

    /// <summary>
    ///     Resolves the correlation ID from the request header if valid,
    /// otherwise generates a new one.
    /// </summary>
    public static string ResolveOrGenerate(this HttpRequest request, string headerName) =>
        request.TryReadFromHeader(headerName) ?? Generate();
}