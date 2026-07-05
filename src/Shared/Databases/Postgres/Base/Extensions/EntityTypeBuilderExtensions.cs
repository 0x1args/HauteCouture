using HauteCouture.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HauteCouture.Shared.Databases.Postgres.Extensions;

/// <summary>
///     Extensions for <see cref="EntityTypeBuilder{TEntity}"/>.
/// </summary>
public static class EntityTypeBuilderExtensions
{
    /// <param name="builder">Entity type builder.</param>
    extension<TEntity, TId>(EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditableEntity<TId>
        where TId : struct
    {
        /// <summary>
        ///     Configures the auditing properties inherited from <see cref="AuditableEntity{TId}"/>:
        ///     <see cref="AuditableEntity{TId}.CreatedAt"/>, <see cref="AuditableEntity{TId}.LastUpdatedAt"/>,
        ///     <see cref="AuditableEntity{TId}.IsDeleted"/>, and <see cref="AuditableEntity{TId}.DeletedAt"/>.
        /// </summary>
        /// <returns>The same <see cref="EntityTypeBuilder{TEntity}"/> instance, for chaining.</returns>
        public EntityTypeBuilder<TEntity> ConfigureAuditableEntity()
        {
            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.LastUpdatedAt)
                .IsRequired(false);

            builder.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.DeletedAt)
                .IsRequired(false);

            // Solution uses snake case naming convention for database columns.
            builder.HasIndex(x => x.IsDeleted)
                .HasFilter("is_deleted = false");

            return builder;
        }
    }
}