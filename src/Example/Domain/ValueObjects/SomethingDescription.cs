using HauteCouture.Example.Domain.Exceptions;

namespace HauteCouture.Example.Domain.ValueObjects;

/// <summary>
///     Represents the validated descriptive text of a <c>Something</c>.
/// </summary>
public readonly record struct SomethingDescription
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 500;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    public SomethingDescription(string description) => Value = description;

    /// <summary>
    ///     Creates a <see cref="SomethingDescription"/> from the specified raw value.
    /// </summary>
    /// <param name="description">The raw description text.</param>
    /// <returns>The validated value object.</returns>
    public static SomethingDescription Of(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new SomethingDomainException("Description cannot be empty.");
        }
        if (description.Length > MaxLength)
        {
            throw new SomethingDomainException($"Description cannot exceed {MaxLength} characters.");
        }

        return new(description);
    }
}