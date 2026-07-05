using Microsoft.EntityFrameworkCore;

namespace HauteCouture.Shared.Databases.Postgres.Abstractions.Repositories;

/// <summary>
///     Defines an operation for executing pre-compiled EF Core queries
///     against the underlying <see cref="DbContext"/>.
/// </summary>
public interface ICompiledQueryRepository
{
    /// <summary>
    ///     Executes a pre-compiled query built via <see cref="EF.CompileAsyncQuery{TContext,TParam,TResult}"/>
    ///     against the underlying <see cref="DbContext"/>, returning <see langword="null"/> if no match is found.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the compiled query.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the compiled query.</typeparam>
    /// <param name="compiledQuery">The pre-compiled query delegate to execute.</param>
    /// <param name="argument">The argument value to bind to the compiled query.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<TResult?> ExecuteCompiledAsync<TArg, TResult>(
        Func<DbContext, TArg, CancellationToken, Task<TResult?>> compiledQuery,
        TArg argument,
        CancellationToken cancellationToken);
}