using Localization.Shared.Attributes;
using Localization.Shared.Interfaces;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Localization.Shared.Models;

/// <summary>
/// Represents a localized wrapper around an enum field.
/// Provides a localized string for the enum value and notifies listeners when the string changes.
/// </summary>
/// <remarks>
/// Instances subscribe to culture change notifications and automatically update their localized string.
/// Use LEnum.INVALID to represent an invalid or placeholder enum value.
/// </remarks>
[DebuggerDisplay("{String} - {_enumType.FullName,nq}")]
public sealed class LEnum : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
  
    #region Fields

    /// <summary>
    /// Instance of LocEnum that represents an invalid enum
    /// </summary>
    public static readonly LEnum INVALID = new("#INVALID ENUM#");
    private readonly ITranslator? _translator;
    private readonly Type _enumType;
    private string _string = string.Empty;

    #endregion
  
    #region Properties

    /// <summary>
    /// Localized string representation of the enum field
    /// </summary>
    public string String
    {
        get => _string;
        set => SetField(ref _string, value);
    }

    /// <summary>
    /// Wrapped enum field
    /// </summary>
    public object EnumField { get; }
  
    #endregion
  
    #region Constructors

    private LEnum(string text)
    {
        _enumType = typeof(Enum);
        _translator = null;
        EnumField = null!;
        String = text;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public LEnum(object enumField)
        : this(enumField, CultureManager.GetTranslator())
    {
    }

    internal LEnum(object enumField, ITranslator? translator)
    {
        _enumType = enumField.GetType();
        _translator = translator;
        EnumField = enumField;

        if (!_enumType.IsEnum || !_enumType.IsEnumDefined(enumField))
            throw new NotSupportedException("The supplied object is not an enum field");

        String = TranslateEnumField(_translator?.CurrentCulture ?? string.Empty);

        CultureManager.InternalCultureChanged += InternalCultureChangeHandler;
    }
  
    #endregion

    #region Methods

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private string TranslateEnumField(string culture)
    {
        if (_translator is null)
            return "#TRANSLATOR MISSING#";

        var enumName = Enum.GetName(_enumType, EnumField)!;
        var enumAttribute = _enumType.GetCustomAttribute<LocalizedEnumAttribute>(false);
        if (enumAttribute is null || string.IsNullOrWhiteSpace(enumAttribute.Namespace))
            return "#" + enumName;

        var memberInfos = _enumType.GetMember(enumName);
        var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == _enumType);
        if (enumValueMemberInfo is null)
            return "#" + enumName;

        var enumFieldAttribute = enumValueMemberInfo.GetCustomAttribute<LocalizedEnumFieldAttribute>(false);
        var key = enumFieldAttribute?.Key;
        if (string.IsNullOrWhiteSpace(key))
            key = enumName;

        return _translator.Translate(key, enumAttribute.Namespace, culture);
    }

    private void InternalCultureChangeHandler(object? recipient, CultureChangedMessage message)
    {
        if (_translator is null)
            return;

        String = TranslateEnumField(message.Value);
    }

    /// <inheritdoc />
    public override string ToString() => String;

    #endregion
}
