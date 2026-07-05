namespace HauteCouture.Shared.WebApi.Modules.TrafficControl;

/// <summary>
///     Token bucket throttler for controlling the rate of requests while allowing for bursts.
/// </summary>
public interface IThrottler
{
    /// <summary>
    ///     Tries to consume a token for the request associated with the given key.
    /// </summary>
    /// <param name="key">Unique key built from IP + http method + path.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Time (in seconds) to wait before the next request can be processed, or null if the request is allowed.</returns>
    Task<double?> TryConsumeAsync(string key, CancellationToken cancellationToken);
}