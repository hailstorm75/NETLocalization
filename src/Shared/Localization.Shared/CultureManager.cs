using System;
using Localization.Shared.Interfaces;
using Localization.Shared.Models;

namespace Localization.Shared;

public static class CultureManager
{
    #region Fields

    private static ITranslator? _translator;
    private static IServiceProvider? _serviceProvider;

    #endregion

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
        _serviceProvider = serviceProvider;
        if (_serviceProvider is null)
            throw new ArgumentNullException(nameof(serviceProvider), "Service provider cannot be null.");

        var service = _serviceProvider.GetService(typeof(ITranslator));
        if (service is not ITranslator translator)
            throw new InvalidOperationException($"The '{nameof(ITranslator)}' service is not registered in the service provider.");

        _translator = translator;
    }

    /// <summary>
    /// Get the current translator instance
    /// </summary>
    /// <returns>Current translator instance. <c>null</c> if the <see cref="CultureManager"/> is not initialized</returns>
    public static ITranslator? GetTranslator() => _translator;

    /// <summary>
    /// Changes the language of the application
    /// </summary>
    /// <param name="language">Language to set</param>
    /// <exception cref="InvalidOperationException">Thrown when the <see cref="CultureManager"/> is not initialized</exception>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="language"/> is <c>null</c></exception>
    /// <exception cref="ArgumentException">Throw when the <paramref name="language"/> parameter is invalid</exception>
    public static void SetLanguage(Language language)
    {
        if (_translator is null)
            throw new InvalidOperationException($"The {nameof(CultureManager)} is not initialized. Please call '{nameof(Initialize)}' first.");
        if (language is null)
            throw new ArgumentNullException(nameof(language), $"The {nameof(language)} parameter cannot be null.");
        if (string.IsNullOrEmpty(language.Key))
            throw new ArgumentException($"The language '{nameof(language.Key)}' is invalid.", nameof(language));

        // If the language is not allowed...
        if (!_translator.AllowedLanguages.Contains(language.Key))
            // Fallback to the default culture
            language = _translator.FallbackCulture;

        // Set the current culture
        LanguageChanged?.Invoke(null, language);
    }

    /// <summary>
    /// Notifies localized elements about a culture change
    /// </summary>
    internal static void InternallyNotifyCultureChanged(Language language) => InternalCultureChanged?.Invoke(null, new CultureChangedMessage(language.Key));

    #endregion
}

/// <summary>
/// Message for notifying about a culture change
/// </summary>
/// <param name="Value">Value of the new culture</param>
public sealed record CultureChangedMessage(string Value);