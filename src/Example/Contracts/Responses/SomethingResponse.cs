namespace HauteCouture.Example.Contracts.Responses;

/// <summary>
///     Response payload representing a <c>Something</c>.
/// </summary>
public sealed record SomethingResponse(
    Guid id,
    string name,
    string description,
    decimal price);