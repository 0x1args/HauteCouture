namespace HauteCouture.Shared.WebApi.Modules.TrafficControl;

/// <summary>
///     Bucket state for token bucket algorithm.
/// </summary>
public sealed record BucketState(
    double Tokens,
    double LastRefillAt);