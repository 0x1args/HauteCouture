using HauteCouture.Shared.Common.Pagination;
using System.Linq.Expressions;

namespace HauteCouture.Shared.Databases.Postgres.Abstractions.Repositories;

/// <summary>
///     Defines read operations for a repository managing <typeparamref name="TEntity"/> instances.
/// </summary>
/// <typeparam name="TEntity">The entity type being queried.</typeparam>
/// <typeparam name="TId">The type of the entity's primary key.</typeparam>
public interface IQueryRepository<TEntity, in TId>
    where TEntity : class
    where TId : struct
{
    /// <summary>
    ///     Returns an <see cref="IQueryable{T}"/> over the entire <typeparamref name="TEntity"/> set,
    ///     allowing callers to compose further LINQ operators before execution.
    /// </summary>
    IQueryable<TEntity> AsQueryable();

    /// <summary>
    ///     Returns an <see cref="IQueryable{T}"/> filtered by <paramref name="predicate"/>,
    ///     allowing callers to compose further LINQ operators before execution.
    /// </summary>
    /// <param name="predicate">The filter expression to apply.</param>
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    ///     Finds a single <typeparamref name="TEntity"/> by its primary key,
    ///     returning <see langword="null"/> if no match is found.
    /// </summary>
    /// <param name="id">The primary key value to look up.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<TEntity?> FindAsync(
        TId id,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns a single page of <typeparamref name="TEntity"/> records from the entire set.
    /// </summary>
    /// <param name="filter">Paging parameters (page number and page size).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<OffsetPagedList<TEntity>> PageAsync(
        PagedFilter filter,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns a single page of <typeparamref name="TEntity"/> records matching <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">The filter expression to apply before paging.</param>
    /// <param name="filter">Paging parameters (page number and page size).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<OffsetPagedList<TEntity>> PageAsync(
        Expression<Func<TEntity, bool>> predicate,
        PagedFilter filter,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns <see langword="true"/> if at least one <typeparamref name="TEntity"/> satisfies
    ///     <paramref name="predicate"/>; otherwise <see langword="false"/>.
    /// </summary>
    /// <param name="predicate">The filter expression to evaluate.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns the total number of <typeparamref name="TEntity"/> records in the set.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<long> CountAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Returns the number of <typeparamref name="TEntity"/> records satisfying <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">The filter expression to apply before counting.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<long> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Executes a raw parameterized SQL statement directly against the database.
    ///     Use only when the operation cannot be expressed through LINQ.
    /// </summary>
    /// <param name="sql">The raw SQL statement to execute.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <param name="parameters">Parameters to bind to the SQL statement.</param>
    Task ExecuteRawSqlAsync(
        string sql,
        CancellationToken cancellationToken,
        params object[] parameters);
}