using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated description of a feature.
/// </summary>
public readonly record struct FeatureDescription
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 255;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private FeatureDescription(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="FeatureDescription"/> from the specified raw value.
    /// </summary>
    /// <param name="reason">The raw feature description.</param>
    /// <returns>The validated value object.</returns>
    public static FeatureDescription Of(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new PlanFeatureException("Feature description cannot be empty.");
        }

        var normalizedDescription = reason.Trim();

        if (normalizedDescription.Length > MaxLength)
        {
            throw new PlanFeatureException($"Feature description cannot exceed {MaxLength} characters.");
        }

        return new(normalizedDescription);
    }
}