namespace HauteCouture.TenantManagement.Domain.Enums;

/// <summary>
///     Specifies the lifecycle status of a billing invoice.
/// </summary>
public enum InvoiceStatus
{
    /// <summary>
    ///     The invoice has been issued and is awaiting payment.
    /// </summary>
    Open,

    /// <summary>
    ///     The invoice has been paid in full.
    /// </summary>
    Paid,

    /// <summary>
    ///     A payment attempt for the invoice was unsuccessful.
    /// </summary>
    Failed,

    /// <summary>
    ///     The invoice was voided by the external billing provider.
    /// </summary>
    VoidedByProvider
}