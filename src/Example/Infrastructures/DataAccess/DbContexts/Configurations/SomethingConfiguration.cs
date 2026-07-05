using HauteCouture.Example.Domain.Entities;
using HauteCouture.Example.Domain.ValueObjects;
using HauteCouture.Shared.Databases.Postgres.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HauteCouture.Example.Infrastuctures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="Something"/>.
/// </summary>
public sealed class SomethingConfiguration : IEntityTypeConfiguration<Something>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Something> builder)
    {
        builder.ToTable("Somethings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new SomethingId(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(s => s.Name)
            .HasConversion(
                name => name.Value,
                value => new SomethingName(value))
            .HasMaxLength(SomethingName.MaxLength)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasConversion(
                description => description.Value,
                value => new SomethingDescription(value))
            .HasMaxLength(SomethingDescription.MaxLength)
            .IsRequired();

        builder.Property(s => s.Price)
            .HasConversion(
                price => price.Value,
                value => new SomethingPrice(value))
            .IsRequired();

        builder.Property(s => s.UserId)
            .HasConversion(
                userId => userId.Value,
                value => new SomethingUserId(value))
            .IsRequired();

        builder.ConfigureAuditableEntity<Something, SomethingId>();
    }
}