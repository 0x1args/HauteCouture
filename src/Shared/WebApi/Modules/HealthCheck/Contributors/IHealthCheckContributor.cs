using Microsoft.Extensions.DependencyInjection;

namespace HauteCouture.Shared.WebApi.Modules.HealthCheck.Contributors;

/// <summary>
///     Represents an opt-in contributor that registers a named health check probe
///     into the <see cref="IHealthChecksBuilder"/>.
/// </summary>
public interface IHealthCheckContributor
{
    /// <summary>
    ///     Registers the health check probe into the builder.
    /// </summary>
    /// <param name="builder">Health checks builder.</param>
    void Register(IHealthChecksBuilder builder);
}