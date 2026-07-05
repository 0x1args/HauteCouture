using HauteCouture.Example.Domain.Exceptions;

namespace HauteCouture.Example.Domain.ValueObjects;

/// <summary>
///     Represents the validated monetary price of a <c>Something</c>.
/// </summary>
public readonly record struct SomethingPrice
{
    /// <summary>
    ///     The maximum allowed value.
    /// </summary>
    public const decimal MaxValue = 1_000_000m;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public decimal Value { get; }

    public SomethingPrice(decimal price) => Value = price;

    /// <summary>
    ///     Creates a <see cref="SomethingPrice"/> from the specified raw value.
    /// </summary>
    /// <param name="price">The raw price.</param>
    /// <returns>The validated value object.</returns>
    public static SomethingPrice Of(decimal price)
    {
        if (price < 0)
        {
            throw new SomethingDomainException("Price cannot be negative.");
        }
        if (price > MaxValue)
        {
            throw new SomethingDomainException($"Price cannot exceed {MaxValue}.");
        }

        return new(price);
    }
}