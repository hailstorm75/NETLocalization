using Localization.Shared.Attributes;
using Localization.Shared.Interfaces;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Localization.Shared.Models;

[DebuggerDisplay("{String} - {_enumType.FullName,nq}")]
public sealed class LEnum : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
  
    #region Fields

    /// <summary>
    /// Instance of LocEnum that represents an invalid enum
    /// </summary>
    public static readonly LEnum INVALID = new("#INVALID ENUM#");
    private static readonly ITranslator? TRANSLATOR = CultureManager.GetTranslator();

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
        EnumField = null!;
        String = text;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public LEnum(object enumField)
    {
        _enumType = enumField.GetType();
        EnumField = enumField;

        if (!_enumType.IsEnum || !_enumType.IsEnumDefined(enumField))
            throw new NotSupportedException("The supplied object is not an enum field");

        if (TRANSLATOR is null)
            throw new InvalidOperationException($"The {nameof(ITranslator)} instance is not initialized.");

        String = TranslateEnumField(TRANSLATOR.CurrentCulture);

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
        if (TRANSLATOR is null)
            return "#TRANSLATOR MISSING#";

        var enumName = Enum.GetName(_enumType, EnumField)!;
        var memberInfos = _enumType.GetMember(enumName);
        var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == _enumType);
        if (enumValueMemberInfo is null)
            return "#" + enumName;

        var valueAttributes = enumValueMemberInfo.GetCustomAttribute(typeof(LocalizedEnumFieldAttribute), false);
        if (valueAttributes is not LocalizedEnumFieldAttribute locEnumAttribute)
            return "#" + enumName;

        return string.IsNullOrEmpty(locEnumAttribute.Key)
            ? TRANSLATOR.Translate(enumName, locEnumAttribute.Namespace, culture)
            : string.Empty;
    }

    private void InternalCultureChangeHandler(object? recipient, CultureChangedMessage message)
    {
        if (TRANSLATOR is null)
            return;

        String = TranslateEnumField(TRANSLATOR.CurrentCulture);
    }

    /// <inheritdoc />
    public override string ToString() => String;

    #endregion
}