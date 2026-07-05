using HauteCouture.Example.Domain.Exceptions;

namespace HauteCouture.Example.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique display name of a <c>Something</c>.
/// </summary>
public readonly record struct SomethingName
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 100;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    public SomethingName(string name) => Value = name;

    /// <summary>
    ///     Creates a <see cref="SomethingName"/> from the specified raw value.
    /// </summary>
    /// <param name="name">The raw name.</param>
    /// <returns>The validated value object.</returns>
    public static SomethingName Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new SomethingDomainException("Name cannot be empty.");
        }
        if (name.Length > MaxLength)
        {
            throw new SomethingDomainException($"Name cannot exceed {MaxLength} characters.");
        }

        return new(name);
    }
}