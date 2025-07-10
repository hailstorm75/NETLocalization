using Localization.Shared.Models;
using Localization.Shared;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;

namespace Localization.WPF;

/// <summary>
/// Localization markup extension for WPF
/// </summary>
public sealed class TrExtension
    : MarkupExtension
{
    private readonly BindingBase[] _args = [];

    #region Properties

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

    #endregion

    #region Constructors

    private static BindingBase SanitizeArgument(object argument)
        => argument switch
        {
            BindingBase binding => binding,
            LString locString => new Binding(nameof(LString.String)) { Source = locString },
            _ => new Binding { Source = argument }
        };

    /// <summary>
    /// Default constructor
    /// </summary>
    public TrExtension()
    {
    }

    /// <summary>
    /// Single argument constructor
    /// </summary>
    /// <param name="arg0">Localization formatting parameter</param>
    public TrExtension(object arg0)
    {
        _args =
        [
            SanitizeArgument(arg0)
        ];
    }

    /// <summary>
    /// Double argument constructor
    /// </summary>
    /// <param name="arg0">First localization formatting parameter</param>
    /// <param name="arg1">Second localization formatting parameter</param>
    public TrExtension(BindingBase arg0, BindingBase arg1)
    {
        _args =
        [
            SanitizeArgument(arg0),
            SanitizeArgument(arg1)
        ];
    }

    /// <summary>
    /// Triple argument constructor
    /// </summary>
    /// <param name="arg0">First localization formatting parameter</param>
    /// <param name="arg1">Second localization formatting parameter</param>
    /// <param name="arg2">Third localization formatting parameter</param>
    public TrExtension(BindingBase arg0, BindingBase arg1, BindingBase arg2)
    {
        _args =
        [
            SanitizeArgument(arg0),
            SanitizeArgument(arg1),
            SanitizeArgument(arg2)
        ];
    }

    /// <summary>
    /// Quadruple argument constructor
    /// </summary>
    /// <param name="arg0">First localization formatting parameter</param>
    /// <param name="arg1">Second localization formatting parameter</param>
    /// <param name="arg2">Third localization formatting parameter</param>
    /// <param name="arg3">Fourth localization formatting parameter</param>
    public TrExtension(BindingBase arg0, BindingBase arg1, BindingBase arg2, BindingBase arg3)
    {
        _args =
        [
            SanitizeArgument(arg0),
            SanitizeArgument(arg1),
            SanitizeArgument(arg2),
            SanitizeArgument(arg3)
        ];
    }

    /// <summary>
    /// Quintuple argument constructor
    /// </summary>
    /// <param name="arg0">First localization formatting parameter</param>
    /// <param name="arg1">Second localization formatting parameter</param>
    /// <param name="arg2">Third localization formatting parameter</param>
    /// <param name="arg3">Fourth localization formatting parameter</param>
    /// <param name="arg4">Fifth localization formatting parameter</param>
    public TrExtension(BindingBase arg0, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4)
    {
        _args =
        [
            SanitizeArgument(arg0),
            SanitizeArgument(arg1),
            SanitizeArgument(arg2),
            SanitizeArgument(arg3),
            SanitizeArgument(arg4)
        ];
    }

    #endregion

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget service)
            return this;

        var dependencyProperty = service.TargetProperty as DependencyProperty;
        var dependencyObject = service.TargetObject as DependencyObject;

        var localizedString = GetLocString(Namespace, Key, String);
        if (localizedString is null)
            return this;

        if (_args.Length == 0)
            return BindingWithoutArguments(serviceProvider, localizedString, dependencyObject, dependencyProperty);

        return BindingWithArguments(serviceProvider, localizedString, dependencyObject, dependencyProperty);
    }

    private static LString? GetLocString(string @namespace, string key, LString? locString)
    {
        if (locString is not null)
            return locString;

        if (string.IsNullOrWhiteSpace(@namespace))
            return null;
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (CultureManager.GetTranslator()?.TryGetString(key, @namespace, out var localizedString) == true)
            return localizedString;

        return new LString
        {
            Namespace = @namespace,
            Key = key
        };
    }

    private object BindingWithArguments(IServiceProvider serviceProvider, LString localizedString, DependencyObject? dependencyObject, DependencyProperty? dependencyProperty)
    {
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
            Mode = BindingMode.OneWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            NotifyOnSourceUpdated = true
        };

        multiBinding.Bindings.Add(new Binding(nameof(LString.String))
        {
            Source = localizedString
        });
        foreach (var bindingBase in _args)
        {
            if (bindingBase is Binding)
            {
                multiBinding.Bindings.Add(bindingBase);
            }
            else if (bindingBase is MultiBinding multiBindingArg)
            {
                foreach (var binding in multiBindingArg.Bindings)
                    multiBinding.Bindings.Add(binding);
            }
        }

        BindingOperations.SetBinding(dependencyObject!, dependencyProperty!, multiBinding);

        return multiBinding.ProvideValue(serviceProvider);
    }

    private static object BindingWithoutArguments(IServiceProvider serviceProvider, LString localizedString, DependencyObject? dependencyObject, DependencyProperty? dependencyProperty)
    {
        var binding = new Binding(nameof(LString.String))
        {
            Source = localizedString
        };

        if (dependencyObject is not null)
            BindingOperations.SetBinding(dependencyObject, dependencyProperty!, binding);

        return binding.ProvideValue(serviceProvider);
    }
}
