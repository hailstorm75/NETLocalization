namespace Localization.Shared.DependencyInjection;

/// <summary>
/// Options for localization service registration.
/// </summary>
public sealed class LocalizationOptions
{
    /// <summary>
    /// Fallback culture key used when a translation for the target culture is missing.
    /// </summary>
    public string FallbackCulture { get; set; } = "en";

    /// <summary>
    /// Restricts selectable languages. Empty set means all loaded languages are allowed.
    /// </summary>
    public ISet<string> AllowedLanguages { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}
