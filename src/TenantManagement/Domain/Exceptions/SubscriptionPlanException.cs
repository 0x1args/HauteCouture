using HauteCouture.Shared.Domain;

namespace HauteCouture.TenantManagement.Domain.Exceptions;

/// <summary>
///     Thrown when a domain invariant or business rule specific to <c>SubscriptionPlan</c> is violated.
/// </summary>
public sealed class SubscriptionPlanException(string message)
    : DomainException(message);