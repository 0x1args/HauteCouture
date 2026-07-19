using HauteCouture.Shared.Domain;

namespace HauteCouture.TenantManagement.Domain.Exceptions;

/// <summary>
///     Thrown when a domain invariant or business rule specific to <c>Invoice</c> is violated.
/// </summary>
public sealed class InvoiceException(string message)
    : DomainException(message);