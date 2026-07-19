using HauteCouture.TenantManagement.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated token of an invitation.
/// </summary>
public readonly partial record struct InvitationToken
{
    private static readonly Regex Pattern = HexRegex();

    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 64;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private InvitationToken(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="InvitationToken"/> from the specified raw value.
    /// </summary>
    /// <param name="token">The raw invitation token.</param>
    /// <returns>The validated value object.</returns>
    public static InvitationToken Of(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new TenantInvitationException("Invitation token cannot be empty.");
        }

        var normalizedToken = token.Trim();

        if (!Pattern.IsMatch(normalizedToken))
        {
            throw new TenantInvitationException("Invitation token must be a valid hexadecimal string.");
        }
        if (normalizedToken.Length != MaxLength)
        {
            throw new TenantInvitationException($"Invitation token cannot exceed {MaxLength} characters.");
        }

        return new(normalizedToken);
    }

    /// <summary>
    ///     Generates a new invitation token.
    /// </summary>
    /// <returns>A newly generated token.</returns>
    public static InvitationToken Generate()
    {
        return new(Convert.ToHexString(RandomNumberGenerator.GetBytes(32)));
    }

    [GeneratedRegex("^[A-F0-9]{64}$", RegexOptions.Compiled)]
    private static partial Regex HexRegex();
}