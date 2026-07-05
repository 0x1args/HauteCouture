namespace HauteCouture.Shared.WebApi.Modules.TrafficControl;

/// <summary>
///     Configuration options for throttling.
/// </summary>
public sealed class ThrottleOptions
{
    public const string SectionName = "Throttle";

    /// <summary>Maximum number of requests allowed per second. </summary>
    public int RequestsPerSecond { get; init; } = 50;

    /// <summary>
    ///     How many seconds of burst traffic to allow.
    ///     BucketCapacity = RequestsPerSecond * BurstSeconds.
    ///     Example: 50 RPS * 2s = 100 token bucket, allowing a brief spike before throttling.
    /// </summary>
    public double BurstSeconds { get; init; } = 1.0;

    /// <summary>Http status code returned when the bucket is empty. </summary>
    public int StatusCode { get; init; } = 429;

    /// <summary>Whether to add Retry-After header indicating how long to wait. </summary>
    public bool AddRetryAfterHeader { get; init; } = true;
}