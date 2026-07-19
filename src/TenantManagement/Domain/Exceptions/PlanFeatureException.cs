using HauteCouture.Shared.Domain;

namespace HauteCouture.TenantManagement.Domain.Exceptions;

/// <summary>
///     Thrown when a domain invariant or business rule specific to <c>PlanFeature</c> is violated.
/// </summary>
public sealed class PlanFeatureException(string message)
    : DomainException(message);