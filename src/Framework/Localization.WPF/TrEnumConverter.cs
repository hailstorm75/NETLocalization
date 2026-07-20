using Localization.Shared;
using Localization.Shared.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Localization.WPF;

/// <summary>
/// Converts enum values to localized enum wrappers and back.
/// </summary>
public sealed class TrEnumConverter : MarkupExtension, IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => CultureManager.GetTranslator()?.GetEnum(value) ?? LEnum.INVALID;

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is LEnum localizedEnum ? localizedEnum.EnumField : value;

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
