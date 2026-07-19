using HauteCouture.TenantManagement.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated URL-friendly slug for a tenant.
/// </summary>
public readonly partial record struct TenantSlug
{
    private static readonly Regex Pattern = SlugRegex();

    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 253;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private TenantSlug(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="TenantSlug"/> from the specified raw value.
    /// </summary>
    /// <param name="slug">The raw tenant slug.</param>
    /// <returns>The validated value object.</returns>
    public static TenantSlug Of(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new TenantException("Tenant slug cannot be empty.");
        }

        var normalizedSlug = slug.Trim().ToLowerInvariant();

        if (normalizedSlug.Length > MaxLength)
        {
            throw new TenantException($"Tenant slug cannot exceed {MaxLength} characters.");
        }
        if (!Pattern.IsMatch(normalizedSlug))
        {
            throw new TenantException("Tenant slug has an invalid format.");
        }

        return new(normalizedSlug);
    }

    [GeneratedRegex("^[a-z0-9](?:[a-z0-9-]{1,61}[a-z0-9])?$")]
    private static partial Regex SlugRegex();
}