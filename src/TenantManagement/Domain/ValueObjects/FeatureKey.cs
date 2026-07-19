using HauteCouture.TenantManagement.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique key of a feature.
/// </summary>
public readonly partial record struct FeatureKey
{
    private static readonly Regex Pattern = KeyRegex();

    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 100;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private FeatureKey(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="FeatureKey"/> from the specified raw value.
    /// </summary>
    /// <param name="name">The raw feature key.</param>
    /// <returns>The validated value object.</returns>
    public static FeatureKey Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new PlanFeatureException("Feature key cannot be empty.");
        }

        var normalizedKey = name.Trim().ToLowerInvariant();

        if (normalizedKey.Length > MaxLength)
        {
            throw new PlanFeatureException($"Feature key cannot exceed {MaxLength} characters.");
        }
        if (!Pattern.IsMatch(normalizedKey))
        {
            throw new PlanFeatureException("Feature key has an invalid format.");
        }

        return new(normalizedKey);
    }

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex KeyRegex();
}