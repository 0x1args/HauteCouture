using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Applications.Services.Abstractions;
using HauteCouture.TenantManagement.Contracts.Responses;

namespace HauteCouture.TenantManagement.Applications.Services;

public sealed class InvoiceService : IInvoiceService
{
    public Task<Guid> CreateFromProviderAsync(Guid tenantId, Guid subscriptionId, decimal amount, string currency, string providerInvoiceId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<OffsetPagedList<InvoiceResponse>> GetTenantInvoicesPageAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task MarkFailedAsync(string providerInvoiceId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task MarkPaidAsync(string providerInvoiceId, string pdfUrl, CancellationToken cancellationToken) => throw new NotImplementedException();
}