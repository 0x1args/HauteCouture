using HauteCouture.Example.Contracts.Responses;

namespace HauteCouture.Example.Applications.AppServices.Abstractions;

/// <summary>
///     Defines application-level operations for managing <c>Something</c> entities.
/// </summary>
public interface ISomethingService
{
    /// <summary>
    ///     Creates a new <c>Something</c> with the specified attributes.
    /// </summary>
    /// <param name="name">The unique display name.</param>
    /// <param name="description">The descriptive text.</param>
    /// <param name="price">The monetary price.</param>
    /// <param name="userId">The identifier of the user creating the entity.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The identifier of the created entity.</returns>
    Task<Guid> CreateSomethingAsync(
        string name,
        string description,
        decimal price,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Updates the description of an existing <c>Something</c>.
    /// </summary>
    /// <param name="somethingId">The identifier of the entity to update.</param>
    /// <param name="newDescription">The new descriptive text.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task UpdateSomethingDescriptionAsync(
        Guid somethingId,
        string newDescription,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Soft-deletes an existing <c>Something</c>.
    /// </summary>
    /// <param name="somethingId">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task DeleteSomethingAsync(
        Guid somethingId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a single <c>Something</c> by its identifier.
    /// </summary>
    /// <param name="somethingId">The identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The matching entity, projected as a response.</returns>
    Task<SomethingResponse> GetSomethingAsync(
        Guid somethingId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a single page of <c>Something</c> records.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The matching entities for the requested page, projected as responses.</returns>
    Task<IEnumerable<SomethingResponse>> GetSomethingPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}