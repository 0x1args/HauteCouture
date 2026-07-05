using HauteCouture.Example.Domain.ValueObjects;
using HauteCouture.Shared.Domain;

namespace HauteCouture.Example.Domain.Entities;

/// <summary>
///     Represents a purchasable item for demonstration purposes.
/// </summary>
public sealed class Something : AuditableEntity<SomethingId>
{
    /// <summary>
    ///     The unique display name.
    /// </summary>
    public SomethingName Name { get; private set; }

    /// <summary>
    ///     The descriptive text.
    /// </summary>
    public SomethingDescription Description { get; private set; }

    /// <summary>
    ///     The monetary price.
    /// </summary>
    public SomethingPrice Price { get; private set; }

    /// <summary>
    ///     The identifier of the user who owns the entity.
    /// </summary>
    public SomethingUserId UserId { get; private set; }

    /// <summary>
    ///     Reserved for EF Core materialization.
    /// </summary>
    private Something()
    {
    }

    private Something(
        SomethingId id,
        SomethingName name,
        SomethingDescription description,
        SomethingPrice price,
        SomethingUserId userId)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        UserId = userId;
    }

    /// <summary>
    ///     Creates a new <see cref="Something"/> with the specified attributes.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="name">The unique display name.</param>
    /// <param name="description">The descriptive text.</param>
    /// <param name="price">The monetary price.</param>
    /// <param name="userId">The identifier of the owning user.</param>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    /// <returns>The created entity.</returns>
    public static Something Create(
        Guid id,
        string name,
        string description,
        decimal price,
        Guid userId,
        DateTimeOffset createdAt)
    {
        var something = new Something(
            SomethingId.Of(id),
            SomethingName.Of(name),
            SomethingDescription.Of(description),
            SomethingPrice.Of(price),
            SomethingUserId.Of(userId));

        something.MarkAsCreated(createdAt);

        return something;
    }

    /// <summary>
    ///     Updates the display name.
    /// </summary>
    /// <param name="newName">The new display name.</param>
    /// <param name="updatedAt">The timestamp to record as the update time.</param>
    public void UpdateName(string newName, DateTimeOffset updatedAt)
    {
        Name = SomethingName.Of(newName);
        MarkAsUpdated(updatedAt);
    }

    /// <summary>
    ///     Updates the descriptive text.
    /// </summary>
    /// <param name="newDescription">The new descriptive text.</param>
    /// <param name="updatedAt">The timestamp to record as the update time.</param>
    public void UpdateDescription(string newDescription, DateTimeOffset updatedAt)
    {
        Description = SomethingDescription.Of(newDescription);
        MarkAsUpdated(updatedAt);
    }

    /// <summary>
    ///     Updates the monetary price.
    /// </summary>
    /// <param name="price">The new price.</param>
    /// <param name="updatedAt">The timestamp to record as the update time.</param>
    public void UpdatePrice(decimal price, DateTimeOffset updatedAt)
    {
        Price = SomethingPrice.Of(price);
        MarkAsUpdated(updatedAt);
    }
}