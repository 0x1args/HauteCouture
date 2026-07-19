using System.Text.RegularExpressions;
using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated branding settings for a tenant.
/// </summary>
public sealed partial record TenantBrandingSettings
{
    /// <summary>
    ///     The maximum allowed length for the logo URL.
    /// </summary>
    public const int MaxLogoUrlLength = 2048;

    /// <summary>
    ///     The maximum allowed length for the primary color hexadecimal value (including '#').
    /// </summary>
    public const int MaxPrimaryColorHexLength = 7;

    /// <summary>
    ///     The logo URL (optional).
    /// </summary>
    public string? LogoUrl { get; }

    /// <summary>
    ///     The primary color in hexadecimal format (optional).
    /// </summary>
    public string? PrimaryColorHex { get; }

    private TenantBrandingSettings(
        string? logoUrl,
        string? primaryColorHex)
    {
        LogoUrl = logoUrl;
        PrimaryColorHex = primaryColorHex;
    }

    /// <summary>
    ///     Creates a <see cref="TenantBrandingSettings"/> instance from the specified values.
    /// </summary>
    /// <param name="logoUrl">The logo URL (optional).</param>
    /// <param name="primaryColorHex">The primary color in hexadecimal format (optional).</param>
    /// <returns>The validated value object.</returns>
    public static TenantBrandingSettings Of(
        string? logoUrl,
        string? primaryColorHex)
    {
        if (!string.IsNullOrWhiteSpace(logoUrl))
        {
            logoUrl = logoUrl.Trim();

            if (logoUrl.Length > MaxLogoUrlLength)
            {
                throw new TenantException($"Tenant branding logo URL cannot exceed {MaxLogoUrlLength} characters.");
            }
            if (!Uri.IsWellFormedUriString(logoUrl, UriKind.Absolute))
            {
                throw new TenantException("Tenant branding logo URL must be a valid absolute URI.");
            }
        }

        primaryColorHex = ValidatePrimaryColorHex(primaryColorHex);
        return new(logoUrl, primaryColorHex);
    }

    /// <summary>
    ///     Gets the default branding settings (empty logo and no primary color).
    /// </summary>
    public static TenantBrandingSettings Default => Of(null, null);

    private static string? ValidatePrimaryColorHex(string? primaryColorHex)
    {
        if (string.IsNullOrWhiteSpace(primaryColorHex))
        {
            return string.Empty;
        }

        primaryColorHex = primaryColorHex.Trim();

        if (primaryColorHex.Length > MaxPrimaryColorHexLength)
        {
            throw new TenantException($"Primary color hex cannot exceed {MaxPrimaryColorHexLength} characters.");
        }
        if (!HexColorRegex().IsMatch(primaryColorHex))
        {
            throw new TenantException("Tenant branding primary color must be a valid hexadecimal color.");
        }

        return primaryColorHex;
    }

    [GeneratedRegex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")]
    private static partial Regex HexColorRegex();
}