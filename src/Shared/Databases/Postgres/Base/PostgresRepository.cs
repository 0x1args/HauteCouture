using HauteCouture.Shared.Common.Pagination;
using HauteCouture.Shared.Databases.Postgres.Abstractions.Repositories;
using HauteCouture.Shared.Databases.Postgres.Extensions;
using HauteCouture.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HauteCouture.Shared.Databases.Postgres;

/// <summary>
///     Generic repository for data access operations.
///     Default implementation of <see cref="IRepository{TEntity, TId}"/>.
/// </summary>
public sealed class PostgresRepository<TEntity, TId>
    : IRepository<TEntity, TId>
    where TEntity : class
    where TId : struct
{
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public PostgresRepository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = dbContext.Set<TEntity>();
    }

    /// <inheritdoc />
    public async Task<TEntity> CreateAsync(
        TEntity entity, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<List<TEntity>> CreateRangeAsync(
        TEntity[] entities, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Length == 0)
        {
            return [];
        }

        await _dbSet.AddRangeAsync(entities, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return [.. entities];
    }

    /// <inheritdoc />
    public async Task<TEntity> UpdateAsync(
        TEntity entity, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = _dbContext.Entry(entity);
        
        if (entry.State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        entry.State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<List<TEntity>> UpdateRangeAsync(
        TEntity[] entities,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Length == 0)
        {
            return [];
        }

        foreach (var entity in entities)
        {
            var entry = _dbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return [.. entities];
    }

    /// <inheritdoc />
    public async Task<TEntity> SoftDeleteAsync(
        TEntity entity,
        DateTimeOffset deletedAt,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (entity is not ISoftDeletable softDeletable)
        {
            throw new InvalidOperationException(
                $"Entity of type '{typeof(TEntity).Name}' does not support soft delete. " +
                $"It must implement '{nameof(ISoftDeletable)}'.");
        }

        softDeletable.Delete(deletedAt);
        var entry = _dbContext.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        entry.State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Remove(entity);
        var affected = await _dbContext.SaveChangesAsync(cancellationToken);

        return affected > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteRangeAsync(TEntity[] entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Length == 0)
        {
            return false;
        }

        _dbSet.RemoveRange(entities);
        var affected = await _dbContext.SaveChangesAsync(cancellationToken);

        return affected > 0;
    }

    /// <inheritdoc />
    public async Task<TEntity> DeleteAndReturnAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public IQueryable<TEntity> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }

    /// <inheritdoc />
    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _dbSet.Where(predicate);
    }

    /// <inheritdoc />
    public async Task<TEntity?> FindAsync(TId id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OffsetPagedList<TEntity>> PageAsync(
        PagedFilter filter,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await _dbSet.ToPagedListAsync(filter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OffsetPagedList<TEntity>> PageAsync(
        Expression<Func<TEntity, bool>> predicate,
        PagedFilter filter,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(filter);

        return await _dbSet
            .Where(predicate)
            .ToPagedListAsync(filter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> CountAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.LongCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _dbSet.LongCountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ExecuteRawSqlAsync(
        string sql,
        CancellationToken cancellationToken,
        params object[] parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TResult?> ExecuteCompiledAsync<TArg, TResult>(
        Func<DbContext, TArg, CancellationToken, Task<TResult?>> compiledQuery,
        TArg argument,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(compiledQuery);
        return compiledQuery(_dbContext, argument, cancellationToken);
    }
}