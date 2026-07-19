using HauteCouture.TenantManagement.Applications.Services.Abstractions;
using HauteCouture.TenantManagement.Contracts.Responses;

namespace HauteCouture.TenantManagement.Applications.Services;

public sealed class TenantService : ITenantService
{
    public Task ActivateAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Guid> CreateAsync(string name, string slug, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Guid> CreateDomainAsync(Guid tenantId, string domainName, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task DeactivateAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<TenantResponse> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<TenantResponse> GetTenantBySlugAsync(string slug, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<bool> IsActivateAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task MarkDomainFailedAsync(Guid tenantId, string domainName, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task ReactivateAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task RemoveDomainAsync(Guid tenantId, string domainName, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task SuspendAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task UpdateSettingsAsync(Guid tenantId, string timeZone, string defaultLocate, string logoUrl, string primaryColorHex, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task VerifyDomainAsync(Guid tenantId, string domainName, CancellationToken cancellationToken) => throw new NotImplementedException();
}