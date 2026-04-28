using Localization.Shared.Interfaces;
using Localization.Shared.Models;
using System.Diagnostics.CodeAnalysis;

namespace Localization.Shared;

/// <summary>
/// Runtime access helpers for non-DI-created surfaces such as XAML markup extensions and generated static helpers.
/// </summary>
public static class LocalizationRuntime
{
    /// <summary>
    /// Gets the currently active translator.
    /// </summary>
    /// <returns>Translator instance, if available</returns>
    public static ITranslator? GetTranslator()
        => LocalizationAmbient.TryGetTranslator(out var translator) ? translator : null;

    /// <summary>
    /// Attempts to get the active culture manager.
    /// </summary>
    /// <param name="cultureManager">Resolved culture manager</param>
    /// <returns><c>true</c> if an active culture manager was found</returns>
    public static bool TryGetCultureManager([NotNullWhen(true)] out ICultureManager? cultureManager)
        => LocalizationAmbient.TryGetCultureManager(out cultureManager);

    /// <summary>
    /// Gets the active culture manager.
    /// </summary>
    /// <returns>Active culture manager</returns>
    public static ICultureManager GetRequiredCultureManager()
        => LocalizationAmbient.GetRequiredCultureManager();

    /// <summary>
    /// Attempts to retrieve a localized string from the runtime translator.
    /// </summary>
    /// <param name="namespace">Localized string namespace</param>
    /// <param name="key">Localized string key</param>
    /// <param name="result">Retrieved localized string</param>
    /// <returns><c>true</c> if successful</returns>
    public static bool TryRetrieveString(string @namespace, string key, [NotNullWhen(true)] out LString? result)
    {
        if (TryGetCultureManager(out var cultureManager))
            return cultureManager.TryRetrieveString(@namespace, key, out result);

        result = null;
        return false;
    }
}
