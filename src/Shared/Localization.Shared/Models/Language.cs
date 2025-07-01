using System.Diagnostics;
using System.Globalization;

namespace Localization.Shared.Models;

/// <summary>
/// Model representing an available language
/// </summary>
[DebuggerDisplay("{Key,nq} = {DisplayName,nq}")]
public sealed record Language
{
    #region Properties

    /// <summary>
    /// RFC 4646 key for the given language
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Native language name
    /// </summary>
    public required string DisplayName { get; init; }

    #endregion

    #region Operators

    /// <summary>
    /// Implicit conversion from <see cref="string"/> to <see cref="Language"/>
    /// </summary>
    /// <param name="key">ISO639-1 two-letter language key</param>
    /// <returns>The language instance</returns>
    public static implicit operator Language(string key)
        => new()
        {
            Key = key,
            DisplayName = GetDisplayName(CultureInfo.CreateSpecificCulture(key))
        };

    /// <summary>
    /// Implicit conversion from <see cref="CultureInfo"/> to <see cref="Language"/>
    /// </summary>
    /// <param name="culture">Culture instance</param>
    /// <returns>The language instance</returns>
    public static implicit operator Language(CultureInfo culture)
        => new()
        {
            Key = culture.IetfLanguageTag,
            DisplayName = GetDisplayName(culture)
        };

    /// <summary>
    /// Implicit conversion from <see cref="Language"/> to <see cref="string"/>
    /// </summary>
    /// <param name="language">The language instance</param>
    /// <returns>ISO639-1 two-letter language key</returns>
    public static implicit operator string(Language language)
        => language.Key;

    private static string GetDisplayName(CultureInfo info)
    {
        var nativeName = info.NativeName;
        var beforeBracket = nativeName.IndexOf(" (", StringComparison.Ordinal);
        if (beforeBracket == -1)
            return nativeName;

        var trimmed = nativeName.AsSpan()[..beforeBracket];
        if (char.IsUpper(trimmed[0]))
            return trimmed.ToString();

        Span<char> buffer = stackalloc char[trimmed.Length];
        trimmed.CopyTo(buffer);
        buffer[0] = char.ToUpper(buffer[0]);

        return buffer.ToString();
    }

    #endregion
}
