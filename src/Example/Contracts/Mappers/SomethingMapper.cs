using HauteCouture.Example.Contracts.Responses;
using HauteCouture.Example.Domain.Entities;

namespace HauteCouture.Example.Contracts.Mappers;

/// <summary>
///     Provides mapping extensions from <see cref="Something"/> to its contract representations.
/// </summary>
public static class SomethingMapper
{
    /// <summary>
    ///     Projects a <see cref="Something"/> entity into a <see cref="SomethingResponse"/>.
    /// </summary>
    /// <param name="something">The entity to project.</param>
    /// <returns>The projected response.</returns>
    public static SomethingResponse ToResponse(this Something something) =>
        new(something.Id.Value,
            something.Name.Value,
            something.Description.Value,
            something.Price.Value);
}