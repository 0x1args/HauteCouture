using HauteCouture.Example.Applications.AppServices.Abstractions;
using HauteCouture.Example.Contracts.Responses;
using HauteCouture.Shared.CQS.Abstractions.Handlers;

namespace HauteCouture.Example.Applications.Handlers.Queries.GetSomething;

/// <summary>
///     Handles <see cref="GetSomethingQuery"/>.
/// </summary>
public sealed class GetSomethingQueryHandler(
    ISomethingService somethingService)
    : IQueryHandler<GetSomethingQuery, SomethingResponse>
{
    /// <inheritdoc />
    public async Task<SomethingResponse> Handle(
        GetSomethingQuery query,
        CancellationToken cancellationToken)
    {
        return await somethingService.GetSomethingAsync(
            query.SomethingId,
            cancellationToken);
    }
}