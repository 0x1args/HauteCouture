using HauteCouture.Shared.Databases.Postgres.Converters;
using HauteCouture.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace HauteCouture.Shared.Databases.Postgres.Extensions;

/// <summary>
///     Extensions for <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesInfoCache = new();

    /// <param name="modelBuilder">Model builder.</param>
    extension(ModelBuilder modelBuilder)
    {
        /// <summary>
        ///     Sets the default <see cref="DateTimeKind"/> for all <see cref="DateTimeOffset"/> properties in the model.
        /// </summary>
        /// <param name="kind">DateTimeKind to enforce.</param>
        /// <returns>The same <see cref="ModelBuilder"/> instance, for chaining.</returns>
        public ModelBuilder SetDefaultDateTimeKind(DateTimeKind kind)
        {
            var converter = kind switch
            {
                DateTimeKind.Utc => DateTimeOffsetUtcConverter.Utc,
                DateTimeKind.Local => DateTimeOffsetUtcConverter.Local,
                DateTimeKind.Unspecified => DateTimeOffsetUtcConverter.Unspecified,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, $"Invalid '{nameof(DateTimeKind)}' specified.")
            };

            var nullableConverter = kind switch
            {
                DateTimeKind.Utc => NullableDateTimeOffsetUtcConverter.Utc,
                DateTimeKind.Local => NullableDateTimeOffsetUtcConverter.Local,
                DateTimeKind.Unspecified => NullableDateTimeOffsetUtcConverter.Unspecified,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, $"Invalid '{nameof(DateTimeKind)}' specified.")
            };

            return modelBuilder
                .UseValueConverterForType(typeof(DateTimeOffset), converter)
                .UseValueConverterForType(typeof(DateTimeOffset?), nullableConverter);
        }

        /// <summary>
        ///     Applies a global query filter to all entities inheriting from <see cref="AuditableEntity{TId}"/>,
        ///     excluding soft-deleted rows (<c>IsDeleted == false</c>) from default queries.
        /// </summary>
        /// <returns>The same <see cref="ModelBuilder"/> instance, for chaining.</returns>
        public ModelBuilder ApplySoftDeleteQueryFilter()
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!IsAuditableEntity(entityType.ClrType))
                {
                    continue;
                }

                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeletedProperty = Expression.Property(parameter, nameof(AuditableEntity<Guid>.IsDeleted));
                var notDeleted = Expression.Not(isDeletedProperty);
                var lambda = Expression.Lambda(notDeleted, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }

            return modelBuilder;
        }

        internal ModelBuilder UseValueConverterForType(Type type, ValueConverter converter)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = GetPropertiesOfType(entityType.ClrType, type);

                foreach (var property in properties)
                {
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(converter);
                }
            }

            return modelBuilder;
        }
    }

    private static bool IsAuditableEntity(Type type)
    {
        var current = type;

        while (current is not null && current != typeof(object))
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(AuditableEntity<>))
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static PropertyInfo[] GetPropertiesOfType(Type entityType, Type propertyType)
    {
        var allProperties = PropertiesInfoCache.GetOrAdd(
            entityType,
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

        return allProperties
            .Where(p => p.PropertyType == propertyType)
            .ToArray();
    }
}