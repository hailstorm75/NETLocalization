using System;
using System.Collections.Generic;
using System.Linq;
using Localization.Shared;
using Localization.Shared.Models;

namespace Localization.Avalonia;

/// <summary>
/// Extension methods for working with localized enums in Avalonia.
/// </summary>
public static class LocalizedEnumExtensions
{
    /// <summary>
    /// Converts an enum value to a localized enum wrapper.
    /// </summary>
    /// <typeparam name="TEnum">Enum type.</typeparam>
    /// <param name="value">Enum value.</param>
    /// <returns>Localized enum wrapper.</returns>
    public static LEnum ToLEnum<TEnum>(this TEnum value)
        where TEnum : struct, Enum
        => CultureManager.GetTranslator()?.GetEnum(value) ?? LEnum.INVALID;

    /// <summary>
    /// Converts a nullable enum value to a localized enum wrapper.
    /// </summary>
    /// <typeparam name="TEnum">Enum type.</typeparam>
    /// <param name="value">Enum value.</param>
    /// <returns>Localized enum wrapper.</returns>
    public static LEnum ToLEnum<TEnum>(this TEnum? value)
        where TEnum : struct, Enum
        => value.HasValue
            ? value.Value.ToLEnum()
            : LEnum.INVALID;

    /// <summary>
    /// Gets all enum values as localized enum wrappers.
    /// </summary>
    /// <typeparam name="TEnum">Enum type.</typeparam>
    /// <returns>Localized enum wrappers.</returns>
    public static IReadOnlyList<LEnum> GetLocalizedValues<TEnum>()
        where TEnum : struct, Enum
        => Enum.GetValues<TEnum>()
            .Select(static value => value.ToLEnum())
            .ToArray();

    /// <summary>
    /// Gets all enum values from an enum type as localized enum wrappers.
    /// </summary>
    /// <param name="enumType">Enum type. Nullable enum types are unwrapped.</param>
    /// <returns>Localized enum wrappers.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="enumType"/> is not an enum type.</exception>
    public static IReadOnlyList<LEnum> GetLocalizedValues(this Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);

        var actualEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
        if (!actualEnumType.IsEnum)
            throw new ArgumentException("Type must be an enum.", nameof(enumType));

        var translator = CultureManager.GetTranslator();
        if (translator is null)
            return Array.Empty<LEnum>();

        return Enum.GetValues(actualEnumType)
            .OfType<object>()
            .Select(translator.GetEnum)
            .ToArray();
    }

    /// <summary>
    /// Converts a localized enum wrapper back to its enum value.
    /// </summary>
    /// <param name="localizedEnum">Localized enum wrapper.</param>
    /// <returns>Wrapped enum value, or <c>null</c> for invalid/null values.</returns>
    public static object? ToEnumValue(this LEnum? localizedEnum)
        => ReferenceEquals(localizedEnum, LEnum.INVALID)
            ? null
            : localizedEnum?.EnumField;
}
