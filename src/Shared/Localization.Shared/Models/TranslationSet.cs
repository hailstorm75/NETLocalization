using Localization.Shared.Attributes;
using Localization.Shared.Interfaces;
using System.Diagnostics;

namespace Localization.Shared.Models;

/// <summary>
/// Set of translations for a specific <see cref="Namespace"/> and <see cref="Key"/>
/// </summary>
/// <remarks>
/// Translation sets are provided by classes marked with <see cref="TranslationProviderAttribute"/> and are consumed by the <see cref="ITranslator"/> implementation
/// </remarks>
[DebuggerDisplay("{Namespace,nq}:{Key,nq}, {Translations.Count} Translations")]
public sealed class TranslationSet
{
    /// <summary>
    /// Namespace for separating translations into logical groups
    /// </summary>
    /// <example>
    /// <c>Dialogs</c> would be a namespace for dialog-related translations
    /// </example>
    public string Namespace => Source.Namespace;

    /// <summary>
    /// Key identifying the translation
    /// </summary>
    /// <example>
    /// <c>DialogTitle</c> would be a key for a dialog title translation
    /// </example>
    public string Key => Source.Key;

    /// <summary>
    /// Culture-keyed translations
    /// </summary>
    /// <remarks>
    /// The keys in this dictionary are lowercase RFC 4646 language tags (e.g., "en-US", "fr-FR") mapped to their respective localized strings
    /// </remarks>
    public required IReadOnlyDictionary<string, string> Translations { get; init; }

    /// <summary>
    /// Localized string utilizing this translation set
    /// </summary>
    public required LString Source { get; init; }
}
