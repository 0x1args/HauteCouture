namespace HauteCouture.TenantManagement.Domain.Enums;

/// <summary>
///     Represents the verification status of a tenant's custom domain.
/// </summary>
public enum DomainVerificationStatus
{
    /// <summary>
    ///     Verification is pending. The domain cannot be used yet.
    /// </summary>
    Pending,

    /// <summary>
    ///     The domain has been successfully verified and can be used.
    /// </summary>
    Verified,

    /// <summary>
    ///     Domain verification failed. The domain cannot be used.
    /// </summary>
    Failed
}