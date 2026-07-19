using HauteCouture.Shared.Domain;

namespace HauteCouture.TenantManagement.Domain.Exceptions;

/// <summary>
///     Thrown when a domain invariant or business rule specific to <c>TenantDomain</c> is violated.
/// </summary>
public sealed class TenantDomainException(string message)
    : DomainException(message);