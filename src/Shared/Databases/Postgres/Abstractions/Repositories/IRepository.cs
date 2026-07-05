namespace HauteCouture.Shared.Databases.Postgres.Abstractions.Repositories;

/// <summary>
///     Unified repository contract combining read and write operations
///     for <typeparamref name="TEntity"/> instances identified by <typeparamref name="TId"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type being managed.</typeparam>
/// <typeparam name="TId">The type of the entity's primary key.</typeparam>
public interface IRepository<TEntity, TId>
    : ICrudRepository<TEntity>, IQueryRepository<TEntity, TId>, ICompiledQueryRepository
    where TEntity : class
    where TId : struct
{
}