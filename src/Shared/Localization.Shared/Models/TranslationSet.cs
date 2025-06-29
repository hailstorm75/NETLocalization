using System.Collections.Generic;

namespace Localization.Shared.Models;

public sealed class TranslationSet
{
    /// <summary>
    /// Namespace for separating translations into logical groups
    /// </summary>
    /// <example>
    /// <c>Dialogs</c> would be a namespace for dialog-related translations
    /// </example>
    public required string Namespace { get; init; }

    /// <summary>
    /// Key identifying the translation
    /// </summary>
    /// <example>
    /// <c>DialogTitle</c> would be a key for a dialog title translation
    /// </example>
    public required string Key { get; init; }

    /// <summary>
    /// Culture-keyed translations
    /// </summary>
    /// <remarks>
    /// The keys in this dictionary are lowercase RFC 4646 language tags (e.g., "en-US", "fr-FR") mapped to their respective localized strings
    /// </remarks>
    public required IReadOnlyDictionary<string, string> Translations { get; init; }
}