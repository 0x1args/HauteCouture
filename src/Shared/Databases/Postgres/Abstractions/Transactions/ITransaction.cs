namespace HauteCouture.Shared.Databases.Postgres.Abstractions.Transactions;

/// <summary>
///     Represents an active database transaction that can be committed, rolled back, or disposed.
/// </summary>
public interface ITransaction : IAsyncDisposable
{
    /// <summary>
    ///     Uniquely identifies this transaction instance.
    /// </summary>
    Guid TransactionId { get; }

    /// <summary>
    ///     Indicates whether the transaction is still active.
    ///     Returns <see langword="false"/> after a successful <see cref="CommitAsync"/> or
    ///     <see cref="RollbackAsync"/>, or after disposal.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    ///     Commits all changes made within this transaction to the database.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the transaction is no longer active.</exception>
    Task CommitAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Discards all changes made within this transaction and reverts the database to its prior state.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the transaction is no longer active.</exception>
    Task RollbackAsync(CancellationToken cancellationToken);
}