using HauteCouture.Example.Domain.Exceptions;

namespace HauteCouture.Example.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique identifier of a <c>Something</c>.
/// </summary>
public readonly record struct SomethingId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    public SomethingId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="SomethingId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw identifier.</param>
    /// <returns>The validated value object.</returns>
    public static SomethingId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new SomethingDomainException("ID cannot be empty.");
        }

        return new(id);
    }
}