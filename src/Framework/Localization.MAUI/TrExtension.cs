using Localization.Shared.Models;

namespace Localization.MAUI;

/// <summary>
/// Localization markup extension for .NET MAUI
/// </summary>
[ContentProperty(nameof(Key))]
public sealed class TrExtension : IMarkupExtension<BindingBase>
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
    /// Optional formatting arguments
    /// </summary>
    public object[] Args { get; set; } = [];

    /// <summary>
    /// Provide value for XAML
    /// </summary>
    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        var locString = GetLocString(Namespace, Key, String);

        if (locString == null)
            return new Binding { Source = string.Empty };

        if (Args.Length == 0)
        {
            // Simple binding to the localized string
            return new Binding(nameof(LString.String))
            {
                Source = locString,
                Mode = BindingMode.OneWay
            };
        }
        else
        {
            // MultiBinding is not available in MAUI, so use a value converter
            return new Binding(nameof(LString.String))
            {
                Source = locString,
                Mode = BindingMode.OneWay,
                Converter = new TrFormatConverter(),
                ConverterParameter = Args
            };
        }
    }

    /// <inheritdoc />
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        => ProvideValue(serviceProvider);

    private static LString? GetLocString(string @namespace, string key, LString? locString)
    {
        if (locString != null)
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
