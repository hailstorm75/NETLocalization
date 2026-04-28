using Localization.Shared.Interfaces;
using Localization.Shared.Models;
using System.Diagnostics.CodeAnalysis;

namespace Localization.Shared;

/// <summary>
/// Singleton manager for handling culture and language changes in the application.
/// </summary>
public sealed class CultureManager : ICultureManager
{
    private ITranslator? _translator;

    /// <inheritdoc />
    public event EventHandler<CultureChangedMessage>? CultureChanged;

    /// <inheritdoc />
    public event EventHandler<Language>? LanguageChanged;

    /// <inheritdoc />
    public ITranslator? Translator => _translator;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public CultureManager() => LocalizationAmbient.Register(this);

    /// <inheritdoc />
    public void SetTranslator(ITranslator translator)
        => _translator = translator ?? throw new ArgumentNullException(nameof(translator), "Translator cannot be null.");

    /// <inheritdoc />
    public void SetLanguage(Language language)
    {
        if (_translator is null)
            throw new InvalidOperationException($"The {nameof(CultureManager)} is not initialized. Please ensure localization services are resolved.");
        if (language is null)
            throw new ArgumentNullException(nameof(language), $"The {nameof(language)} parameter cannot be null.");
        if (string.IsNullOrEmpty(language.Key))
            throw new ArgumentException($"The language '{nameof(language.Key)}' is invalid.", nameof(language));

        if (_translator.AllowedLanguages.Count > 0 && !_translator.AllowedLanguages.Contains(language.Key))
            language = _translator.FallbackCulture;

        LanguageChanged?.Invoke(this, language);
    }

    /// <inheritdoc />
    public void NotifyCultureChanged(Language language)
        => CultureChanged?.Invoke(this, new CultureChangedMessage(language.Key));

    /// <inheritdoc />
    public bool TryRetrieveString(string @namespace, string key, [NotNullWhen(true)] out LString? result)
    {
        if (_translator is not null)
            return _translator.TryGetString(key, @namespace, out result);

        result = null;
        return false;
    }
}

/// <summary>
/// Message for notifying about a culture change.
/// </summary>
/// <param name="Value">Value of the new culture</param>
public sealed record CultureChangedMessage(string Value);
