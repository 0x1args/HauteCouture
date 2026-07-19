using HauteCouture.Shared.Databases.Postgres.Extensions;
using HauteCouture.TenantManagement.Domain.Aggregates;
using HauteCouture.TenantManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="TenantInvitation"/>.
/// </summary>
public sealed class TenantInvitationConfiguration : IEntityTypeConfiguration<TenantInvitation>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TenantInvitation> builder)
    {
        builder.ToTable("TenantInvitations");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(
                id => id.Value,
                value => InvitationId.Of(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(i => i.Email)
            .HasConversion(
                email => email.Value,
                value => InvitationEmailAddress.Of(value))
            .HasMaxLength(InvitationEmailAddress.MaxLength)
            .IsRequired();

        builder.Property(i => i.ProposedRole)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(i => i.Token)
            .HasConversion(
                token => token.Value,
                value => InvitationToken.Of(value))
            .HasMaxLength(InvitationToken.MaxLength)
            .IsRequired();

        builder.Property(i => i.InvitedByMemberId)
            .HasConversion(
                id => id.Value,
                value => InviterMemberId.Of(value))
            .IsRequired();

        builder.Property(i => i.ExpiresAt)
            .HasConversion(
                date => date.Value,
                value => InvitationExpirationDate.Of(value))
            .IsRequired();

        builder.HasIndex(i => i.Token)
           .IsUnique();

        builder.HasIndex(i => i.Email);

        builder.ConfigureAuditableEntity<TenantInvitation, InvitationId>();
    }
}