using Localization.Shared.Interfaces;
using Localization.Shared.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Globalization;

namespace Localization.Shared;

/// <summary>
/// Translation service
/// </summary>
public class Translator : ITranslator, IDisposable
{
  #region Fields

  private readonly ILogger<Translator> _logger;
  private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, TranslationSet>> _translations = new(StringComparer.OrdinalIgnoreCase);
  private readonly Dictionary<string, Language> _loadedLanguages = new(2, StringComparer.OrdinalIgnoreCase);
  private readonly Dictionary<object, LEnum> _enums = new();

  #endregion

  #region Properties

  /// <inheritdoc />
  public string CurrentCulture { get; private set; } = "en";

  /// <inheritdoc />
  public string FallbackCulture { get; init; } = "en";

  /// <inheritdoc />
  public IReadOnlyCollection<Language> LoadedLanguages => _loadedLanguages.Values;

  /// <inheritdoc />
  public ISet<string> AllowedLanguages { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

  #endregion

  /// <summary>
  /// Default constructor
  /// </summary>
  public Translator(ILogger<Translator> logger)
  {
    _logger = logger;
    
    CultureManager.LanguageChanged += LanguageChanged;
  }

  #region Methods

  private void LanguageChanged(object? sender, Language e) => ChangeCulture(e);

  /// <inheritdoc />
  public LEnum GetEnum(object? enumField)
  {
    if (enumField is null)
      return LEnum.INVALID;

    if (_enums.TryGetValue(enumField, out var locEnum))
      return locEnum;

    locEnum = new LEnum(enumField);
    _enums[enumField] = locEnum;

    return locEnum;
  }

  /// <inheritdoc />
  public void RegisterTranslations(TranslationSet translations)
  {
    using var scope = _logger.BeginScope(nameof(RegisterTranslations));
    
    TranslatorLogger.LogRegisteringTranslation(translations.Namespace, translations.Key, _logger);

    if (!_translations.TryGetValue(translations.Namespace, out var localizations))
      _translations[translations.Namespace] = new ConcurrentDictionary<string, TranslationSet>(StringComparer.OrdinalIgnoreCase)
      {
        [translations.Key] = translations
      };
    else
    {
#if DEBUG
      if (localizations.ContainsKey(translations.Key))
        TranslatorLogger.LogDuplicateRegisteringTranslation(translations.Namespace, translations.Key, _logger);
#endif

      localizations[translations.Key] = translations;
      RegisterLanguages(translations);
    }
  }

  /// <inheritdoc />
  public void RegisterTranslations(IEnumerable<TranslationSet> translationSets)
  {
    foreach (var set in translationSets)
      RegisterTranslations(set);
  }

  /// <inheritdoc />
  public bool IsLocalizationKnown(string key, string @namespace)
    => _translations.TryGetValue(@namespace, out var localizations)
       && localizations.ContainsKey(key);

  private void RegisterLanguages(TranslationSet set)
  {
    foreach (var language in set.Translations.Keys)
      RegisterLanguage(language);
  }

  private void RegisterLanguage(string key)
  {
    if (_loadedLanguages.ContainsKey(key))
      return;

    _loadedLanguages.Add(key, key);

    TranslatorLogger.LogDiscoveredCulture(key, _logger);
  }

  /// <inheritdoc />
  public string Translate(string key, string @namespace, string culture)
  {
    using var scope = _logger.BeginScope(nameof(Translate));
    
    if (!_translations.TryGetValue(@namespace, out var localization))
    {
      TranslatorLogger.LogNamespaceNotFound(@namespace, _logger);
      return "#INVALID LOCALIZATION NAMESPACE#";
    }
    if (!localization.TryGetValue(key, out var translationHolder))
    {
      TranslatorLogger.LogKeyNotFound(key, @namespace, _logger);
      return "#INVALID LOCALIZATION KEY#";
    }

    if (!translationHolder.Translations.TryGetValue(culture, out var localizedString))
    {
      TranslatorLogger.LogCultureNotFound(culture, key, @namespace, FallbackCulture, _logger);
      return '#' + translationHolder.Translations[FallbackCulture];
    }

    return localizedString;
  }

  /// <inheritdoc />
  public string Translate(string key, string @namespace) => Translate(key, @namespace, CurrentCulture);

  /// <inheritdoc />
  public string TranslateArgs(string key, string @namespace, object arg) => TranslateArgs(key, @namespace, CurrentCulture, arg);

  /// <inheritdoc />
  public string TranslateArgs(string key, string @namespace, params object[] args) => TranslateArgs(key, @namespace, CurrentCulture, args);

  /// <inheritdoc />
  public string TranslateArgs(string key, string @namespace, string culture, object arg)
  {
    var localizedString = Translate(key, @namespace, culture);
    var formattedString = string.Format(localizedString, arg);

    return formattedString;
  }

  /// <inheritdoc />
  public string TranslateArgs(string key, string @namespace, string culture, params object[] args)
  {
    var localizedString = Translate(key, @namespace, culture);
    var formattedString = string.Format(localizedString, args);

    return formattedString;
  }

  /// <inheritdoc />
  public void ChangeCulture(Language language)
  {
    using var scope = _logger.BeginScope(nameof(CurrentCulture));

    if (AllowedLanguages.Count > 0 && !AllowedLanguages.Contains(language))
    {
      TranslatorLogger.LogAttemptToChangeCultureToDisallowedCulture(language, _logger);
      return;
    }

    var newCulture = new CultureInfo(language).TwoLetterISOLanguageName;
    if (CurrentCulture.Equals(newCulture, StringComparison.OrdinalIgnoreCase))
      return;

    TranslatorLogger.LogChangingCulture(CurrentCulture, newCulture, _logger);
    CurrentCulture = newCulture;

    CultureManager.InternallyNotifyCultureChanged(language);
  }

  /// <inheritdoc />
  public void Dispose()
  {
    CultureManager.LanguageChanged -= LanguageChanged;
    _translations.Clear();
    _loadedLanguages.Clear();
    _enums.Clear();

    GC.SuppressFinalize(this);
  }

  #endregion
}
