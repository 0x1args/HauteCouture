namespace HauteCouture.Example.Contracts.Requests;

/// <summary>
///     Request payload for updating the description of an existing <c>Something</c>.
/// </summary>
public sealed record UpdateSomethingDescriptionRequest(
    string NewDescription);