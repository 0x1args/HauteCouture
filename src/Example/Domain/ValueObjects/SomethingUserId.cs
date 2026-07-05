using HauteCouture.Example.Domain.Exceptions;

namespace HauteCouture.Example.Domain.ValueObjects;

/// <summary>
///     Represents the validated identifier of the user owning a <c>Something</c>.
/// </summary>
public readonly record struct SomethingUserId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    public SomethingUserId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="SomethingUserId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw identifier.</param>
    /// <returns>The validated value object.</returns>
    public static SomethingUserId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new SomethingDomainException("User ID cannot be empty.");
        }

        return new(id);
    }
}