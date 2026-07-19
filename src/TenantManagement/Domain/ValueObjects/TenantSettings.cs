using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated settings for a tenant.
/// </summary>
public sealed record TenantSettings
{
    /// <summary>
    ///     The maximum allowed length for the time zone string.
    /// </summary>
    public const int MaxTimeZoneLength = 100;

    /// <summary>
    ///     The maximum allowed length for the default locale string.
    /// </summary>
    public const int MaxDefaultLocaleLength = 50;

    /// <summary>
    ///     The tenant's time zone (e.g., "UTC").
    /// </summary>
    public string TimeZone { get; }

    /// <summary>
    ///     The default locale (e.g., "en-US").
    /// </summary>
    public string DefaultLocale { get; }

    /// <summary>
    ///     The branding settings.
    /// </summary>
    public TenantBrandingSettings Branding { get; }

    /// <summary>
    ///     The feature flags dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, bool> FeatureFlags { get; }

    private TenantSettings(
        string timeZone,
        string defaultLocale,
        TenantBrandingSettings branding,
        IReadOnlyDictionary<string, bool> featureFlags)
    {
        TimeZone = timeZone;
        DefaultLocale = defaultLocale;
        Branding = branding;
        FeatureFlags = featureFlags;
    }

    /// <summary>
    ///     Creates a <see cref="TenantSettings"/> instance from the specified values.
    /// </summary>
    /// <param name="timeZone">The tenant's time zone (e.g., "UTC").</param>
    /// <param name="defaultLocale">The default locale (e.g., "en-US").</param>
    /// <param name="branding">The branding settings (optional).</param>
    /// <param name="featureFlags">The feature flags dictionary (optional).</param>
    /// <returns>The validated value object.</returns>
    public static TenantSettings Of(
        string timeZone,
        string defaultLocale,
        TenantBrandingSettings? branding = null,
        IReadOnlyDictionary<string, bool>? featureFlags = null)
    {
        if (string.IsNullOrWhiteSpace(timeZone))
        {
            throw new TenantException("Tenant time zone cannot be empty.");
        }

        var trimmedTimeZone = timeZone.Trim();

        if (trimmedTimeZone.Length > MaxTimeZoneLength)
        {
            throw new TenantException($"Tenant time zone cannot exceed {MaxTimeZoneLength} characters.");
        }
        if (string.IsNullOrWhiteSpace(defaultLocale))
        {
            throw new TenantException("Tenant default locale cannot be empty.");
        }

        var trimmedDefaultLocale = defaultLocale.Trim();

        if (trimmedDefaultLocale.Length > MaxDefaultLocaleLength)
        {
            throw new TenantException($"Tenant default locale cannot exceed {MaxDefaultLocaleLength} characters.");
        }

        return new(
            trimmedTimeZone,
            trimmedDefaultLocale,
            branding ?? TenantBrandingSettings.Default,
            featureFlags ?? new Dictionary<string, bool>());
    }

    /// <summary>
    ///     Gets the default tenant settings (UTC time zone, en-US locale, empty branding, and no feature flags).
    /// </summary>
    public static TenantSettings Default() => Of(
        timeZone: "UTC",
        defaultLocale: "en-US");
}