using HauteCouture.Example.Applications.Services.Abstractions;
using HauteCouture.Example.Contracts.Responses;
using HauteCouture.Example.Domain.Entities;
using HauteCouture.Example.Domain.ValueObjects;
using HauteCouture.Shared.Common.Exceptions.Client;
using HauteCouture.Shared.Databases.Postgres.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HauteCouture.Example.Applications.Services;

/// <summary>
///     Default implementation of <see cref="ISomethingService"/>.
/// </summary>
public sealed class SomethingService(
    IRepository<Something, Guid> somethingRepository,
    TimeProvider timeProvider,
    ILogger<SomethingService> logger) : ISomethingService
{
    /// <inheritdoc />
    public async Task<Guid> CreateSomethingAsync(
        string name,
        string description,
        decimal price,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var somethingName = SomethingName.Of(name);

        var exists = await somethingRepository.ExistsAsync(
            s => s.Name == somethingName,
            cancellationToken);

        if (exists)
        {
            logger.LogWarning("Something with name {Name} already exists.", name);
            throw new ConflictException("Something with the specified name already exists.");
        }

        var something = Something.Create(
            Guid.CreateVersion7(),
            name,
            description,
            price,
            userId,
            timeProvider.GetUtcNow());

        await somethingRepository.CreateAsync(something, cancellationToken);
        logger.LogInformation("Something with ID {Id} created successfully.", something.Id);

        return something.Id.Value;
    }

    /// <inheritdoc />
    public async Task DeleteSomethingAsync(
        Guid somethingId,
        CancellationToken cancellationToken)
    {
        var something = await GetSometingByIdAsync(somethingId, cancellationToken);
        await somethingRepository.SoftDeleteAsync(something, timeProvider.GetUtcNow(), cancellationToken);

        logger.LogInformation("Something with ID {Id} soft-deleted successfully.", somethingId);
    }

    /// <inheritdoc />
    public async Task<SomethingResponse> GetSomethingAsync(
        Guid somethingId,
        CancellationToken cancellationToken)
    {
        var sid = SomethingId.Of(somethingId);

        return await somethingRepository.ExecuteCompiledAsync(
            SomethingCompiledQueries.GetResponseByIdAsync,
            sid,
            cancellationToken)
            ?? throw new NotFoundException($"Something with ID '{somethingId}' was not found.");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SomethingResponse>> GetSomethingPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return [];
    }

    /// <inheritdoc />
    public async Task UpdateSomethingDescriptionAsync(
        Guid somethingId,
        string newDescription,
        CancellationToken cancellationToken)
    {
        var something = await GetSometingByIdAsync(somethingId, cancellationToken);

        something.UpdateDescription(newDescription, timeProvider.GetUtcNow());
        await somethingRepository.UpdateAsync(something, cancellationToken);

        // Simulate some processing time for demonstration purposes (slow query simulation).
        await Task.Delay(1200, cancellationToken);

        logger.LogInformation("Something with ID {Id} updated successfully.", somethingId);
    }

    private async Task<Something> GetSometingByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var sid = SomethingId.Of(id);

        return await somethingRepository
            .Where(s => s.Id == sid)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Something with ID '{id}' was not found.");
    }
}