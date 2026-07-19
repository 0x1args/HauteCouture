using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Contracts.Responses;

namespace HauteCouture.TenantManagement.Applications.Services.Abstractions;

public interface IInvoiceService
{
    Task<Guid> CreateFromProviderAsync(
        Guid tenantId,
        Guid subscriptionId, 
        decimal amount,
        string currency,
        string providerInvoiceId,
        CancellationToken cancellationToken);

    Task MarkPaidAsync(
        string providerInvoiceId,
        string pdfUrl,
        CancellationToken cancellationToken);

    Task MarkFailedAsync(
        string providerInvoiceId,
        CancellationToken cancellationToken);

    Task<OffsetPagedList<InvoiceResponse>> GetTenantInvoicesPageAsync(
        Guid tenantId,
        CancellationToken cancellationToken);
} 