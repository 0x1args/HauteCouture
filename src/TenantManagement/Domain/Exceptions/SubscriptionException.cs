using HauteCouture.Shared.Domain;

namespace HauteCouture.TenantManagement.Domain.Exceptions;

/// <summary>
///     Thrown when a domain invariant or business rule specific to <c>Subscription</c> is violated.
/// </summary>
public sealed class SubscriptionException(string message)
    : DomainException(message);