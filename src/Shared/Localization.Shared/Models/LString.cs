﻿using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Localization.Shared.Interfaces;
using Localization.Shared.JSON;

namespace Localization.Shared.Models;

/// <summary>
/// Localized string
/// </summary>
/// <remarks>
/// The provided string is localized based on the current culture, which is managed by the <see cref="CultureManager"/>.
/// <para/>
/// Constant <see cref="LString"/> instances are not affected by culture changes and will always return the same value (see <see cref="CreateConstant"/>)
/// </remarks>
[JsonConverter(typeof(LStringJsonConverter))]
[DebuggerDisplay("{String} - {Namespace,nq}:{Key,nq}")]
public sealed class LString : INotifyPropertyChanged, IEquatable<LString>, IComparable<LString>, IDisposable
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    #region Fields

    private static readonly ITranslator? TRANSLATOR = CultureManager.GetTranslator();
    private readonly LString? _formattingSource;
    private readonly LString[] _formattingArgs = [];
    private readonly bool _isConstant;
    private string? _string;

    #endregion

    #region Properties

    /// <summary>
    /// Localization namespace
    /// </summary>
    public required string Namespace { get; init; } = string.Empty;

    /// <summary>
    /// Localization key
    /// </summary>
    public required string Key { get; init; } = string.Empty;

    /// <summary>
    /// Localization unique identifier
    /// </summary>
    public string Identifier => $"{Namespace}:{Key}";

    /// <summary>
    /// Determines whether the string is empty
    /// </summary>
    public bool IsEmpty => !((_isConstant && !string.IsNullOrEmpty(_string)) || (!string.IsNullOrEmpty(Namespace) && !string.IsNullOrEmpty(Key)));

    /// <summary>
    /// Localized string
    /// </summary>
    public string String
    {
        get
        {
            if (TRANSLATOR is null)
                return string.Empty;

            if (!IsEmpty)
                return _string ??= TRANSLATOR.Translate(Key, Namespace);

            if (_isConstant)
                return _string ??= "INVALID CONSTANT";
            if (_formattingSource is not null)
                return string.Format(_formattingSource.String, _formattingArgs.Select(static object (s) => s.String).ToArray());

            return string.Empty;
        }
        private set => SetField(ref _string, value);
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    public LString()
    {
        _isConstant = false;
        CultureManager.InternalCultureChanged += InternalCultureChangedHandler;
    }

    /// <summary>
    /// Constant value constructor
    /// </summary>
    /// <param name="constString">Constant string value</param>
    private LString(string constString)
    {
        String = constString;
        Namespace = string.Empty;
        Key = string.Empty;

        _isConstant = true;
    }

    private LString(LString formatted, IEnumerable<LString> args)
    {
        _formattingSource = formatted;
        _formattingArgs = args.ToArray();
        _isConstant = false;

        CultureManager.InternalCultureChanged += InternalCultureChangedHandler;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Splits a localized string identifier into its namespace and key components
    /// </summary>
    /// <param name="identifier">Identifier in the format "Namespace:Key"</param>
    /// <returns>Tuple containing the namespace and key</returns>
    public static (string @namespace, string key) SplitIdentifier(string identifier)
    {
        var parts = identifier.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid identifier format. Identifier must be in the format 'Namespace:Key'.");

        return (parts[0], parts[1]);
    }
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Create a constant localized string
    /// </summary>
    /// <param name="value">Constant value</param>
    /// <returns>Created string</returns>
    public static LString CreateConstant(string value)
        => new(value)
        {
            Namespace = string.Empty,
            Key = string.Empty
        };

    /// <summary>
    /// Creates a formatted localized string
    /// </summary>
    /// <param name="format">Format source</param>
    /// <param name="args">Formatting arguments</param>
    /// <returns></returns>
    public static LString CreateFormatted(LString format, params LString[] args)
        => new(format, args)
        {
            Namespace = string.Empty,
            Key = string.Empty
        };

    /// <summary>
    /// Formats the localized string with supplied <paramref name="args"/>
    /// </summary>
    /// <param name="args">Formatting arguments</param>
    /// <returns>Formatted string</returns>
    public string Format(params object[] args) => string.Format(this, args);

    /// <summary>
    /// Formats the localized string with supplied <paramref name="arg"/>
    /// </summary>
    /// <param name="arg">Formatting argument</param>
    /// <returns>Formatted string</returns>
    public string Format(object arg) => string.Format(this, arg);

    private void InternalCultureChangedHandler(object? sender, CultureChangedMessage message)
    {
        if (TRANSLATOR is null)
            return;

        if (!IsEmpty)
            String = TRANSLATOR.Translate(Key, Namespace, message.Value);
        else if (_formattingSource is not null)
            String = string.Format(
                TRANSLATOR.Translate(_formattingSource.Key, _formattingSource.Namespace, message.Value),
                _formattingArgs.Select(s => TRANSLATOR.Translate(s.Key, s.Namespace, message.Value) as object).ToArray());
    }

    /// <inheritdoc />
    public bool Equals(LString? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        
        if (other is null)
            return false;

        if (_isConstant && other._isConstant)
            return String.Equals(other.String, StringComparison.Ordinal);

        return Namespace.Equals(other.Namespace) && Key.Equals(other.Key);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is LString other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => _isConstant
            ? HashCode.Combine(String)
            : HashCode.Combine(Namespace, Key);

    /// <inheritdoc />
    public int CompareTo(LString? other)
    {
        if (other is null)
            return 1; // This instance is greater than null

        var namespaceComparison = string.Compare(Namespace, other.Namespace, StringComparison.Ordinal);
        return namespaceComparison != 0
            ? namespaceComparison
            : string.Compare(Key, other.Key, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override string ToString() => String;

    /// <summary>
    /// Implicit conversion to <c>string</c>
    /// </summary>
    /// <param name="localized">Localized string instance</param>
    /// <returns>Conversion result</returns>
    public static implicit operator string(LString localized) => localized.String;

    /// <inheritdoc />
    public void Dispose()
    {
        CultureManager.InternalCultureChanged -= InternalCultureChangedHandler;
        _string = null;
    }

    #endregion
}
