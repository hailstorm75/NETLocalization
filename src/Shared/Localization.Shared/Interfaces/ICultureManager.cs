using Localization.Shared.Models;
using System.Diagnostics.CodeAnalysis;

namespace Localization.Shared.Interfaces;

/// <summary>
/// Interface for culture/language orchestration.
/// </summary>
public interface ICultureManager
{
    /// <summary>
    /// Invoked when a language change is requested.
    /// </summary>
    event EventHandler<Language>? LanguageChanged;

    /// <summary>
    /// Invoked when the active culture has changed.
    /// </summary>
    event EventHandler<CultureChangedMessage>? CultureChanged;

    /// <summary>
    /// Current translator instance, if available.
    /// </summary>
    ITranslator? Translator { get; }

    /// <summary>
    /// Attaches a translator instance to this manager.
    /// </summary>
    /// <param name="translator">Translator instance</param>
    void SetTranslator(ITranslator translator);

    /// <summary>
    /// Changes the language of the application.
    /// </summary>
    /// <param name="language">Language to set</param>
    void SetLanguage(Language language);

    /// <summary>
    /// Notifies listeners that the effective culture changed.
    /// </summary>
    /// <param name="language">Effective language</param>
    void NotifyCultureChanged(Language language);

    /// <summary>
    /// Attempts to retrieve a localized string from the translator.
    /// </summary>
    /// <param name="namespace">Localized string namespace</param>
    /// <param name="key">Localized string key</param>
    /// <param name="result">Retrieved localized string</param>
    /// <returns><c>true</c> if a localized string was retrieved</returns>
    bool TryRetrieveString(string @namespace, string key, [NotNullWhen(true)] out LString? result);
}
