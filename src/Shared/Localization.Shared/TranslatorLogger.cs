using Microsoft.Extensions.Logging;

namespace Localization.Shared;

internal static partial class TranslatorLogger
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Registering translation for namespace '{Namespace}' with key '{Key}'")]
    public static partial void LogRegisteringTranslation(
        string @namespace,
        string key,
        ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Duplicate registering of a translation for namespace '{Namespace}' with key '{Key}'")]
    public static partial void LogDuplicateRegisteringTranslation(
        string @namespace,
        string key,
        ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "The translation namespace '{Namespace}' was not found")]
    public static partial void LogNamespaceNotFound(
        string @namespace,
        ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "The translation key '{Key}' was not found in namespace '{Namespace}'")]
    public static partial void LogKeyNotFound(
        string key,
        string @namespace,
        ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "The translation culture '{Culture}' was not found for key '{Key}' in namespace '{Namespace}'. Using default culture: '{DefaultCulture}'")]
    public static partial void LogCultureNotFound(
        string culture,
        string key,
        string @namespace,
        string defaultCulture,
        ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Attempt to change culture to disallowed culture '{Culture}' was blocked")]
    public static partial void LogAttemptToChangeCultureToDisallowedCulture(
        string culture,
        ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Changing application culture from '{CurrentCulture}' to '{Culture}'")]
    public static partial void LogChangingCulture(
        string currentCulture,
        string culture,
        ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Registering discovered culture '{Culture}'")]
    public static partial void LogDiscoveredCulture(
        string culture,
        ILogger logger);
}
