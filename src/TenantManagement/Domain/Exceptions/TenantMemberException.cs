using HauteCouture.Shared.Domain;

namespace HauteCouture.TenantManagement.Domain.Exceptions;

/// <summary>
///     Thrown when a domain invariant or business rule specific to <c>TenantMember</c> is violated.
/// </summary>
public sealed class TenantMemberException(string message)
    : DomainException(message);