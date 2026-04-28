using Localization.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Localization.Shared.DependencyInjection;

/// <summary>
/// Extension methods for registering localization services.
/// </summary>
public static class LocalizationServiceCollectionExtensions
{
    /// <summary>
    /// Registers localization services and runtime access wiring.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configure">Options configuration callback</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddLocalization(this IServiceCollection services, Action<LocalizationOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new LocalizationOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddSingleton<ICultureManager, CultureManager>();
        services.AddSingleton<ITranslator>(serviceProvider =>
        {
            var logger = serviceProvider.GetService<ILogger<Translator>>() ?? NullLogger<Translator>.Instance;
            var cultureManager = serviceProvider.GetRequiredService<ICultureManager>();
            var localizationOptions = serviceProvider.GetRequiredService<LocalizationOptions>();

            var translator = new Translator(logger, cultureManager)
            {
                FallbackCulture = localizationOptions.FallbackCulture,
                AllowedLanguages = new HashSet<string>(localizationOptions.AllowedLanguages, StringComparer.OrdinalIgnoreCase)
            };

            return translator;
        });

        return services;
    }

}
