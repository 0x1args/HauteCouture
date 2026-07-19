using HauteCouture.Shared.Domain;

namespace HauteCouture.TenantManagement.Domain.Exceptions;

/// <summary>
///     Thrown when a domain invariant or business rule specific to <c>Tenant</c> is violated.
/// </summary>
public sealed class TenantException(string message)
    : DomainException(message);