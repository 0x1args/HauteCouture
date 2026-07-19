namespace HauteCouture.TenantManagement.Domain.Enums;

/// <summary>
///     Specifies the lifecycle status of a tenant invitation.
/// </summary>
public enum InvitationStatus
{
    /// <summary>
    ///     The invitation has been issued and is awaiting a response.
    /// </summary>
    Pending,

    /// <summary>
    ///     The invitation has been accepted by its recipient.
    /// </summary>
    Accepted,

    /// <summary>
    ///     The invitation was revoked before it could be accepted.
    /// </summary>
    Revoked,

    /// <summary>
    ///     The invitation was not accepted before its expiration date.
    /// </summary>
    Expired
}