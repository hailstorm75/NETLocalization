using Localization.Shared;
using Localization.Shared.DependencyInjection;
using Localization.Shared.Interfaces;
using Localization.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace UT.Shared;

public class CultureManagerTests
{
    [Fact]
    public void AddLocalization_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLocalization(_ => { });
        using var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<ICultureManager>().ShouldNotBeNull();
        provider.GetService<ITranslator>().ShouldNotBeNull();
    }

    [Fact]
    public void SetLanguage_Throws_WhenTranslatorIsMissing()
    {
        // Arrange
        var cultureManager = new CultureManager();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => cultureManager.SetLanguage("en"));
    }

    [Fact]
    public void SetLanguage_FallsBack_WhenLanguageIsNotAllowed()
    {
        // Arrange
        var cultureManager = new CultureManager();
        var translator = new Translator(Microsoft.Extensions.Logging.Abstractions.NullLogger<Translator>.Instance, cultureManager)
        {
            AllowedLanguages = new HashSet<string>()
        };
        translator.ChangeCulture("de");
        translator.AllowedLanguages.Add("en");

        // Act
        cultureManager.SetLanguage(new Language { Key = "de", DisplayName = "German" });

        // Assert
        translator.CurrentCulture.ShouldBe("en");
    }
}
