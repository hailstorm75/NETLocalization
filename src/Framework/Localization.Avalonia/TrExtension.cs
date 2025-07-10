using System.Globalization;
using System;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Data;
using Localization.Shared.Models;
using Localization.Shared;

namespace Localization.Avalonia;

/// <summary>
/// Localization markup extension for Avalonia
/// </summary>
public sealed class TrExtension : MarkupExtension
{
    private readonly IBinding[] _args = [];

    #region Properties

    public string Key { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public LString? String { get; set; }
    public IValueConverter? Converter { get; set; }
    public object? ConverterParameter { get; set; }
    public CultureInfo? ConverterCulture { get; set; }

    #endregion

    #region Constructors

    private static IBinding SanitizeArgument(object argument)
        => argument switch
        {
            IBinding binding => binding,
            LString locString => new Binding { Path = nameof(LString.String), Source = locString },
            _ => new Binding { Source = argument }
        };

    public TrExtension() { }

    public TrExtension(object arg0)
    {
        _args = new[] { SanitizeArgument(arg0) };
    }

    public TrExtension(IBinding arg0, IBinding arg1)
    {
        _args = new[] { SanitizeArgument(arg0), SanitizeArgument(arg1) };
    }

    public TrExtension(IBinding arg0, IBinding arg1, IBinding arg2)
    {
        _args = new[] { SanitizeArgument(arg0), SanitizeArgument(arg1), SanitizeArgument(arg2) };
    }

    public TrExtension(IBinding arg0, IBinding arg1, IBinding arg2, IBinding arg3)
    {
        _args = new[] { SanitizeArgument(arg0), SanitizeArgument(arg1), SanitizeArgument(arg2), SanitizeArgument(arg3) };
    }

    public TrExtension(IBinding arg0, IBinding arg1, IBinding arg2, IBinding arg3, IBinding arg4)
    {
        _args = new[] { SanitizeArgument(arg0), SanitizeArgument(arg1), SanitizeArgument(arg2), SanitizeArgument(arg3), SanitizeArgument(arg4) };
    }

    #endregion

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var localizedString = GetLocString(Namespace, Key, String);
        if (localizedString is null)
            return string.Empty;

        if (_args.Length == 0)
            return BindingWithoutArguments(localizedString);

        return BindingWithArguments(localizedString);
    }

    private static LString? GetLocString(string @namespace, string key, LString? locString)
    {
        if (locString is not null)
            return locString;

        if (string.IsNullOrWhiteSpace(@namespace) || string.IsNullOrWhiteSpace(key))
            return null;

        if (CultureManager.GetTranslator()?.TryGetString(key, @namespace, out var localizedString) == true)
            return localizedString;

        return new LString
        {
            Namespace = @namespace,
            Key = key
        };
    }

    private object BindingWithArguments(LString localizedString)
    {
        // You must implement TrArgumentConverter for Avalonia (similar to WPF's IMultiValueConverter)
        var converter = new TrArgumentConverter
        {
            Arguments = _args,
            Data = localizedString,
            Converter = Converter,
            ConverterParameter = ConverterParameter,
            ConverterCulture = ConverterCulture ?? CultureInfo.CurrentCulture
        };

        var multiBinding = new MultiBinding
        {
            Converter = converter,
            Bindings =
            {
                new Binding { Path = nameof(LString.String), Source = localizedString }
            }
        };

        foreach (var arg in _args)
            multiBinding.Bindings.Add(arg);

        return multiBinding;
    }

    private static object BindingWithoutArguments(LString localizedString)
        => new Binding
        {
            Path = nameof(LString.String),
            Source = localizedString
        };
}
