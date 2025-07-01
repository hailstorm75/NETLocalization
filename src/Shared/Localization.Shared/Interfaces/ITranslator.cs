using Localization.Shared.Models;

namespace Localization.Shared.Interfaces;

/// <summary>
/// Interface for translation services
/// </summary>
public interface ITranslator
{
    #region Properties

    /// <summary>
    /// Default fallback culture key
    /// </summary>
    string FallbackCulture { get; init; }

    /// <summary>
    /// Languages that have been loaded
    /// </summary>
    IReadOnlyCollection<Language> LoadedLanguages { get; }

    /// <summary>
    /// Language keys that are allowed to be provided for selection
    /// </summary>
    /// <remarks>
    /// Leaving this empty means that all loaded languages are allowed
    /// </remarks>
    ISet<string> AllowedLanguages { get; init; }

    /// <summary>
    /// Currently active culture
    /// </summary>
    string CurrentCulture { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Registers a <paramref name="translations"/>
    /// </summary>
    /// <param name="translations">Translation set to register</param>
    /// <remarks>
    /// If the <paramref name="translations"/> is already registered, it will be overwritten
    /// </remarks>
    void RegisterTranslations(TranslationSet translations);

    /// <summary>
    /// Registers multiple <paramref name="translationSets"/>
    /// </summary>
    /// <param name="translationSets">Translation sets to register</param>
    /// <remarks>
    /// If a <see cref="TranslationSet"/> is already registered, it will be overwritten
    /// </remarks>
    void RegisterTranslations(IEnumerable<TranslationSet> translationSets);

    /// <summary>
    /// Determines whether the given <paramref name="key"/> is known in the given <paramref name="namespace"/>.
    /// </summary>
    /// <param name="key">Translation set key</param>
    /// <param name="namespace">Translation set namespace</param>
    /// <returns>True if the translation set is known</returns>
    bool IsLocalizationKnown(string key, string @namespace);

    /// <summary>
    /// Translates a string with the given <paramref name="key"/> and <paramref name="namespace"/>.
    /// </summary>
    /// <param name="key">Translation set key</param>
    /// <param name="namespace">Translation set namespace</param>
    /// <returns>Localized string</returns>
    string Translate(string key, string @namespace);

    /// <summary>
    /// Translates a string with the given <paramref name="key"/>, <paramref name="namespace"/> to the specified culture <paramref name="culture"/>.
    /// </summary>
    /// <param name="key">Translation set key</param>
    /// <param name="namespace">Translation set namespace</param>
    /// <param name="culture">Target culture key in the RFC 4646 format</param>
    /// <returns>Localized string</returns>
    string Translate(string key, string @namespace, string culture);

    /// <summary>
    /// Attempts to retrieve a localized string with the given <paramref name="key"/> and <paramref name="namespace"/>.
    /// </summary>
    /// <param name="key">Translation set key</param>
    /// <param name="namespace">Translation set namespace</param>
    /// <param name="result">Source localized string</param>
    /// <returns><c>true</c> if the retrieval was a success</returns>
    bool TryGetString(string key, string @namespace, out LString? result);

    /// <summary>
    /// Translates a parametrized string with the given <paramref name="key"/> and <paramref name="namespace"/>
    /// </summary>
    /// <param name="key">Translation set key</param>
    /// <param name="namespace">Translation set namespace</param>
    /// <param name="arg">Argument to supply to the parametrized string</param>
    /// <returns>Localized string</returns>
    string TranslateArgs(string key, string @namespace, object arg);

    /// <summary>
    /// Translates a parametrized string with the given <paramref name="key"/> and <paramref name="namespace"/>
    /// </summary>
    /// <param name="key">Translation set key</param>
    /// <param name="namespace">Translation set namespace</param>
    /// <param name="args">Arguments to supply to the parametrized string</param>
    /// <returns>Localized string</returns>
    string TranslateArgs(string key, string @namespace, params object[] args);

    /// <summary>
    /// Translates a parametrized string with the given <paramref name="key"/> and <paramref name="namespace"/> to the specified <paramref name="culture"/>.
    /// </summary>
    /// <param name="key">Translation set key</param>
    /// <param name="namespace">Translation set namespace</param>
    /// <param name="culture">Target culture</param>
    /// <param name="arg">Argument to supply to the parametrized string</param>
    /// <returns>Localized string</returns>
    string TranslateArgs(string key, string @namespace, string culture, object arg);

    /// <summary>
    /// Translates a parametrized string with the given <paramref name="key"/> and <paramref name="namespace"/> to the specified <paramref name="culture"/>.
    /// </summary>
    /// <param name="key">Translation set key</param>
    /// <param name="namespace">Translation set namespace</param>
    /// <param name="culture">Target culture</param>
    /// <param name="args">Arguments to supply to the parametrized string</param>
    /// <returns>Localized string</returns>
    string TranslateArgs(string key, string @namespace, string culture, params object[] args);

    /// <summary>
    /// Changes the current culture to the specified <paramref name="language"/>
    /// </summary>
    /// <param name="language">Target culture</param>
    void ChangeCulture(Language language);

    /// <summary>
    /// Retrieves a caches or provides a new <see cref="LEnum"/> for the given <paramref name="enumField"/>
    /// </summary>
    /// <param name="enumField">Enum field</param>
    /// <returns>Localized enum field</returns>
    LEnum GetEnum(object? enumField);

    #endregion
}
