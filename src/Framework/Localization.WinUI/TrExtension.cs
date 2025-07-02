using System.Globalization;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Localization.Shared.Models;

namespace Localization.WinUI;

/// <summary>
/// Localization markup extension for WinUI
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class TrExtension : MarkupExtension
{
    /// <summary>
    /// Localization text key
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Localization namespace
    /// </summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// Localized string
    /// </summary>
    public LString? String { get; set; }

    /// <summary>
    /// Converter to apply to the translated string
    /// </summary>
    public IValueConverter? Converter { get; set; }

    /// <summary>
    /// Parameter for the <see cref="Converter"/>
    /// </summary>
    public object? ConverterParameter { get; set; }

    /// <summary>
    /// Culture for the <see cref="Converter"/>
    /// </summary>
    public CultureInfo? ConverterCulture { get; set; }

    protected override object ProvideValue()
    {
        var localizedString = GetLocString(Namespace, Key, String);
        if (localizedString is null)
            return string.Empty;

        var binding = new Binding
        {
            Path = new Microsoft.UI.Xaml.PropertyPath(nameof(LString.String)),
            Source = localizedString,
            Mode = BindingMode.OneWay,
            Converter = Converter ?? new TrConverter(),
            ConverterParameter = ConverterParameter
        };

        return binding;
    }

    private static LString? GetLocString(string @namespace, string key, LString? locString)
    {
        if (locString is not null)
            return locString;

        if (string.IsNullOrWhiteSpace(@namespace) || string.IsNullOrWhiteSpace(key))
            return null;

        return new LString
        {
            Namespace = @namespace,
            Key = key
        };
    }
}


