using Localization.Shared.Interfaces;
using Localization.Shared.Models;
using System.Diagnostics.CodeAnalysis;

namespace Localization.Shared;

/// <summary>
/// Singleton manager for handling culture and language changes in the application
/// </summary>
public static class CultureManager
{
    private static ITranslator? TRANSLATOR;

    #region Events

    internal static event EventHandler<CultureChangedMessage>? InternalCultureChanged;

    /// <summary>
    /// Invoked by the <see cref="CultureManager"/> when the language is changed
    /// </summary>
    /// <remarks>
    /// For internal use only or for custom implementations of <see cref="ITranslator"/>.
    /// <para/>
    /// Use <see cref="SetLanguage(Language)"/> to change the language.
    /// </remarks>
    public static event EventHandler<Language>? LanguageChanged;

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the <see cref="CultureManager"/> with the provided service provider
    /// </summary>
    /// <param name="serviceProvider">Instance of the service provider</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is <c>null</c></exception>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="serviceProvider"/> does not provide the required <see cref="ITranslator"/> service</exception>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        if (serviceProvider is null)
            throw new ArgumentNullException(nameof(serviceProvider), "Service provider cannot be null.");

        var service = serviceProvider.GetService(typeof(ITranslator));
        if (service is not ITranslator translator)
            throw new InvalidOperationException($"The '{nameof(ITranslator)}' service is not registered in the service provider.");

        TRANSLATOR = translator;
    }
    
    /// <summary>
    /// Initializes the <see cref="CultureManager"/> with the provided translator instance
    /// </summary>
    /// <param name="translator">Translator instance to initialize with</param>
    public static void Initialize(ITranslator translator)
        => TRANSLATOR = translator;

    /// <summary>
    /// Get the current translator instance
    /// </summary>
    /// <returns>Current translator instance. <c>null</c> if the <see cref="CultureManager"/> is not initialized</returns>
    public static ITranslator? GetTranslator() => TRANSLATOR;

    /// <summary>
    /// Changes the language of the application
    /// </summary>
    /// <param name="language">Language to set</param>
    /// <exception cref="InvalidOperationException">Thrown when the <see cref="CultureManager"/> is not initialized</exception>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="language"/> is <c>null</c></exception>
    /// <exception cref="ArgumentException">Throw when the <paramref name="language"/> parameter is invalid</exception>
    public static void SetLanguage(Language language)
    {
        if (TRANSLATOR is null)
            throw new InvalidOperationException($"The {nameof(CultureManager)} is not initialized. Please call '{nameof(Initialize)}' first.");
        if (language is null)
            throw new ArgumentNullException(nameof(language), $"The {nameof(language)} parameter cannot be null.");
        if (string.IsNullOrEmpty(language.Key))
            throw new ArgumentException($"The language '{nameof(language.Key)}' is invalid.", nameof(language));

        // If the language is not allowed...
        if (TRANSLATOR.AllowedLanguages.Count > 0 && !TRANSLATOR.AllowedLanguages.Contains(language.Key))
            // Fallback to the default culture
            language = TRANSLATOR.FallbackCulture;

        // Set the current culture
        LanguageChanged?.Invoke(null, language);
    }

    /// <summary>
    /// Notifies localized elements about a culture change
    /// </summary>
    internal static void InternallyNotifyCultureChanged(Language language) => InternalCultureChanged?.Invoke(null, new CultureChangedMessage(language.Key));

    /// <summary>
    /// Attempts to retrieve a localized string from the translator
    /// </summary>
    /// <param name="namespace">Localized string namespace</param>
    /// <param name="key">Localized string key</param>
    /// <param name="result">Retrieved localized string. Set to <c>null</c> if no string was found</param>
    /// <returns><c>true</c> if the retrieval was a success</returns>
    internal static bool TryRetrieveString(string @namespace, string key, [NotNullWhen(true)] out LString? result)
    {
        if (TRANSLATOR is not null)
            return TRANSLATOR.TryGetString(key, @namespace, out result);

        result = null;
        return false;
    }

    #endregion
}

/// <summary>
/// Message for notifying about a culture change
/// </summary>
/// <param name="Value">Value of the new culture</param>
public sealed record CultureChangedMessage(string Value);
