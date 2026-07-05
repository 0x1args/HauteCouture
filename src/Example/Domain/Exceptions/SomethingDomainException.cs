using HauteCouture.Shared.Domain;

namespace HauteCouture.Example.Domain.Exceptions;

/// <summary>
///     Thrown when a domain invariant or business rule specific to <c>Something</c> is violated.
/// </summary>
public class SomethingDomainException(string message)
    : DomainException(message);