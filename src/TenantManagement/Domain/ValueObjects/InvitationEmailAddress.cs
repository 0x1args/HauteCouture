using HauteCouture.TenantManagement.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated email address of an invitation.
/// </summary>
public readonly partial record struct InvitationEmailAddress
{
    private static readonly Regex Pattern = EmailRegex();

    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 254;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private InvitationEmailAddress(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="InvitationEmailAddress"/> from the specified raw value.
    /// </summary>
    /// <param name="emailAddress">The raw email address.</param>
    /// <returns>The validated value object.</returns>
    public static InvitationEmailAddress Of(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            throw new TenantInvitationException("Email address cannot be empty.");
        }

        var normalizedEmailAddress = emailAddress.Trim();

        if (normalizedEmailAddress.Length > MaxLength)
        {
            throw new TenantInvitationException($"Email address cannot exceed {MaxLength} characters.");
        }

        if (!Pattern.IsMatch(normalizedEmailAddress))
        {
            throw new TenantInvitationException("Email address has an invalid format.");
        }

        return new(normalizedEmailAddress);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}