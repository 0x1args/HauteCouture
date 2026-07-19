using HauteCouture.TenantManagement.Contracts.Responses;

namespace HauteCouture.TenantManagement.Applications.Services.Abstractions;

public interface ITenantService
{
    Task<Guid> CreateAsync(
        string name, 
        string slug,
        CancellationToken cancellationToken);

    Task ActivateAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task SuspendAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task ReactivateAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task DeactivateAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task UpdateSettingsAsync(
        Guid tenantId,
        string timeZone,
        string defaultLocate,
        string logoUrl,
        string primaryColorHex,
        CancellationToken cancellationToken);

    Task<Guid> CreateDomainAsync(
        Guid tenantId,
        string domainName,
        CancellationToken cancellationToken);

    Task VerifyDomainAsync(
        Guid tenantId,
        string domainName,
        CancellationToken cancellationToken);

    Task MarkDomainFailedAsync(
        Guid tenantId,
        string domainName,
        CancellationToken cancellationToken);

    Task RemoveDomainAsync(
        Guid tenantId,
        string domainName,
        CancellationToken cancellationToken);

    Task<TenantResponse> GetTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task<TenantResponse> GetTenantBySlugAsync(
        string slug,
        CancellationToken cancellationToken);

    Task<bool> IsActivateAsync(
        Guid tenantId,
        CancellationToken cancellationToken);
}