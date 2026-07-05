using HauteCouture.Shared.Domain;

namespace HauteCouture.Shared.Databases.Postgres.Abstractions.Repositories;

/// <summary>
///     Defines write operations for a repository managing <typeparamref name="TEntity"/> instances.
/// </summary>
/// <typeparam name="TEntity">The entity type being managed.</typeparam>
public interface ICrudRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    ///     Persists a new <paramref name="entity"/> to the database.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created entity.</returns>
    Task<TEntity> CreateAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Persists a collection of <paramref name="entities"/> to the database in a single batch.
    /// </summary>
    /// <param name="entities">The entities to create.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created entities.</returns>
    Task<List<TEntity>> CreateRangeAsync(
        TEntity[] entities,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Persists all changes made to <paramref name="entity"/> to the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated entity.</returns>
    Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Persists all changes made to each entity in <paramref name="entities"/> to the database in a single batch.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated entities.</returns>
    Task<List<TEntity>> UpdateRangeAsync(
        TEntity[] entities,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Marks <paramref name="entity"/> as deleted (soft delete) and persists the change,
    ///     without physically removing the row. The entity must implement <see cref="ISoftDeletable"/>.
    /// </summary>
    /// <param name="entity">The entity to soft-delete.</param>
    /// <param name="deletedAt">The timestamp to record as the deletion time.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The soft-deleted entity.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <typeparamref name="TEntity"/> does not implement <see cref="ISoftDeletable"/>.
    /// </exception>
    Task<TEntity> SoftDeleteAsync(
        TEntity entity,
        DateTimeOffset deletedAt,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Removes <paramref name="entity"/> from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><see langword="true"/> if at least one row was affected; otherwise <see langword="false"/>.</returns>
    Task<bool> DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Removes all <paramref name="entities"/> from the database in a single batch.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><see langword="true"/> if at least one row was affected; otherwise <see langword="false"/>.</returns>
    Task<bool> DeleteRangeAsync(
        TEntity[] entities,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Removes <paramref name="entity"/> from the database and returns it to the caller.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The deleted entity.</returns>
    Task<TEntity> DeleteAndReturnAsync(
        TEntity entity,
        CancellationToken cancellationToken);
}