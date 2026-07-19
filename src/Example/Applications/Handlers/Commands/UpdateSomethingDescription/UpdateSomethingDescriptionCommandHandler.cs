using HauteCouture.Example.Applications.Handlers.Queries.GetSomething;
using HauteCouture.Example.Applications.Services.Abstractions;
using HauteCouture.Shared.CQS.Abstractions.Handlers;
using Microsoft.Extensions.Caching.Distributed;

namespace HauteCouture.Example.Applications.Handlers.Commands.UpdateSomethingDescription;

/// <summary>
///     Handles <see cref="UpdateSomethingDescriptionCommand"/>, invalidating the associated
///     <see cref="GetSomethingQuery"/> cache entry after the update completes.
/// </summary>
public sealed class UpdateSomethingDescriptionCommandHandler(
    ISomethingService somethingService,
    IDistributedCache distributedCache)
    : ICommandHandler<UpdateSomethingDescriptionCommand>
{
    /// <inheritdoc />
    public async Task Handle(
        UpdateSomethingDescriptionCommand command,
        CancellationToken cancellationToken)
    {
        await somethingService.UpdateSomethingDescriptionAsync(
            command.SomethingId,
            command.Request.NewDescription,
            cancellationToken);

        await distributedCache.RemoveAsync(
            GetSomethingQuery.BuildCacheKey(command.SomethingId),
            cancellationToken);
    }
}