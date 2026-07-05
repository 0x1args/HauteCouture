using HauteCouture.Shared.Databases.Postgres.Abstractions.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HauteCouture.Shared.Databases.Postgres;

/// <summary>
///     Manages the transactional boundary for the current <see cref="DbContext"/> unit of work.
///     Default implementation of <see cref="ITransactionalScope"/>.
/// </summary>
internal sealed class PostgresTransactionalScope : ITransactionalScope
{
    private readonly DbContext _dbContext;

    /// <inheritdoc />
    public bool HasActiveTransaction => _dbContext.Database.CurrentTransaction is not null;

    public PostgresTransactionalScope(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var database = _dbContext.Database;

        if (database.CurrentTransaction is IDbContextTransaction existing)
        {
            return new PostgresTransaction(existing);
        }

        var transaction = await database.BeginTransactionAsync(cancellationToken);
        return new PostgresTransaction(transaction);
    }

    /// <inheritdoc />
    public IExecutionStrategy CreateExecutionStrategy()
    {
        return _dbContext.Database.CreateExecutionStrategy();
    }
}