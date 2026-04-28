using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Localization.Shared.Models;

namespace Localization.Avalonia;

/// <summary>
/// Converts enum values to localized enum wrappers and back.
/// </summary>
public sealed class TrEnumConverter : MarkupExtension, IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is null
            ? LEnum.INVALID
            : Shared.LocalizationRuntime.GetTranslator()?.GetEnum(value) ?? LEnum.INVALID;

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is LEnum localizedEnum ? localizedEnum.ToEnumValue() : value;

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
