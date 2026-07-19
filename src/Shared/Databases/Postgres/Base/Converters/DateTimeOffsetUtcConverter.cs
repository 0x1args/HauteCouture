using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HauteCouture.Shared.Databases.Postgres.Converters;

/// <summary>
///     Converter to handle DateTimeKind for DateTimeOffset values.
/// </summary>
public sealed class DateTimeOffsetUtcConverter(
    DateTimeKind kind,
    ConverterMappingHints? mappingHints = null)
    : ValueConverter<DateTimeOffset, DateTimeOffset>(
        v => v.ToUniversalTime(),
        v => kind == DateTimeKind.Local ? v.ToLocalTime() : v.ToUniversalTime(),
        mappingHints)
{
    /// <summary>
    ///     Convertor for UTC DateTimeKind.
    /// </summary>
    public static readonly DateTimeOffsetUtcConverter Utc = new(DateTimeKind.Utc);

    /// <summary>
    ///     Convertor for UTC Local. 
    /// </summary>
    public static readonly DateTimeOffsetUtcConverter Local = new(DateTimeKind.Local);

    /// <summary>
    ///     Convertor for UTC Unspecified. 
    /// </summary>
    public static readonly DateTimeOffsetUtcConverter Unspecified = new(DateTimeKind.Unspecified);
}

/// <summary>
///     Converter to handle DateTimeKind for nullable DateTimeOffset values.
/// </summary>
public sealed class NullableDateTimeOffsetUtcConverter(
    DateTimeKind kind,
    ConverterMappingHints? mappingHints = null)
    : ValueConverter<DateTimeOffset?, DateTimeOffset?>(
        v => v.HasValue ? v.Value.ToUniversalTime() : null,
        v => v.HasValue
            ? (kind == DateTimeKind.Local ? v.Value.ToLocalTime() : v.Value.ToUniversalTime())
            : null,
        mappingHints)
{
    /// <summary>
    ///     Convertor for UTC DateTimeKind.
    /// </summary>
    public static readonly NullableDateTimeOffsetUtcConverter Utc = new(DateTimeKind.Utc);

    /// <summary>
    ///     Convertor for UTC Local.
    /// </summary>
    public static readonly NullableDateTimeOffsetUtcConverter Local = new(DateTimeKind.Local);

    /// <summary>
    ///     Convertor for UTC Unspecified.
    /// </summary>
    public static readonly NullableDateTimeOffsetUtcConverter Unspecified = new(DateTimeKind.Unspecified);
}