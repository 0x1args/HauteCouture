namespace HauteCouture.Example.Contracts.Requests;

/// <summary>
///     Request payload for creating a new <c>Something</c>.
/// </summary>
public sealed record CreateSomethingRequest(
    string Name,
    string Description,
    decimal Price);