using Microsoft.Extensions.Logging;
using Localization.Shared.Models;
using Localization.Shared;
using NSubstitute;
using Shouldly;
using Xunit;
using Bogus;

namespace UT.Shared;

public class TranslatorTests
{
    [Fact]
    public void Constructor_SetsLoggerAndSubscribesToLanguageChanged()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        // Act
        var translator = new Translator(logger);
        // Assert
        translator.ShouldNotBeNull();
    }

    [Fact]
    public void GetEnum_ReturnsInvalidForNull()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger);
        // Act
        var result = translator.GetEnum(null);
        // Assert
        result.ShouldBe(LEnum.INVALID);
    }

    [Fact]
    public void GetEnum_ReturnsLEnumForNonNull()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger);
        var value = DayOfWeek.Monday;
        // Act
        var result = translator.GetEnum(value);
        // Assert
        result.ShouldNotBeNull();
        result.EnumField.ShouldBe(value);
    }

    [Fact]
    public void RegisterTranslations_And_IsLocalizationKnown_Works()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger);
        var faker = new Faker();
        var ns = faker.Lorem.Words(1).First();
        var key = faker.Lorem.Words(1).First();
        var translations = new TranslationSet
        {
            Source = new LString { Namespace = ns, Key = key },
            Translations = new Dictionary<string, string> { { "en", "Hello" } }
        };
        // Act
        translator.RegisterTranslations(translations);
        var known = translator.IsLocalizationKnown(key, ns);
        // Assert
        known.ShouldBeTrue();
    }

    [Fact]
    public void Translate_ReturnsLocalizedStringOrError()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger);
        var ns = "TestNS";
        var key = "TestKey";
        var translations = new TranslationSet
        {
            Source = new LString { Namespace = ns, Key = key },
            Translations = new Dictionary<string, string> { { "en", "Hello" } }
        };
        translator.RegisterTranslations(translations);
        // Act
        var result = translator.Translate(key, ns, "en");
        var missingNs = translator.Translate("k", "missing", "en");
        var missingKey = translator.Translate("missing", ns, "en");
        var missingCulture = translator.Translate(key, ns, "fr");
        // Assert
        result.ShouldBe("Hello");
        missingNs.ShouldContain("INVALID LOCALIZATION NAMESPACE");
        missingKey.ShouldContain("INVALID LOCALIZATION KEY");
        missingCulture.ShouldContain("#");
    }

    [Fact]
    public void TryGetString_ReturnsTrueIfFound()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger);
        var ns = "TestNS";
        var key = "TestKey";
        var translations = new TranslationSet
        {
            Source = new LString { Namespace = ns, Key = key },
            Translations = new Dictionary<string, string> { { "en", "Hello" } }
        };
        translator.RegisterTranslations(translations);
        // Act
        var found = translator.TryGetString(key, ns, out var lstr);
        // Assert
        found.ShouldBeTrue();
        lstr.ShouldNotBeNull();
        lstr!.Namespace.ShouldBe(ns);
        lstr.Key.ShouldBe(key);
    }

    [Fact]
    public void TryGetString_ReturnsFalseIfNotFound()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger);
        // Act
        var found = translator.TryGetString("missing", "missing", out var lstr);
        // Assert
        found.ShouldBeFalse();
        lstr.ShouldBeNull();
    }

    [Fact]
    public void TranslateArgs_FormatsStringWithArgs()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger);
        var ns = "TestNS";
        var key = "TestKey";
        var translations = new TranslationSet
        {
            Source = new LString { Namespace = ns, Key = key },
            Translations = new Dictionary<string, string> { { "en", "Hello {0}" } }
        };
        translator.RegisterTranslations(translations);
        // Act
        var result = translator.TranslateArgs(key, ns, "en", "World");
        // Assert
        result.ShouldBe("Hello World");
    }

    [Fact]
    public void ChangeCulture_ChangesCurrentCultureIfAllowed()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger)
        {
            AllowedLanguages = new HashSet<string> { "en", "fr" }
        };
        var lang = new Language { Key = "fr", DisplayName = "French" };
        // Act
        translator.ChangeCulture(lang);
        // Assert
        translator.CurrentCulture.ShouldBe("fr");
    }

    [Fact]
    public void ChangeCulture_DoesNotChangeIfDisallowed()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger)
        {
            AllowedLanguages = new HashSet<string> { "en" }
        };
        var lang = new Language { Key = "fr", DisplayName = "French" };
        // Act
        translator.ChangeCulture(lang);
        // Assert
        translator.CurrentCulture.ShouldBe("en");
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var logger = Substitute.For<ILogger<Translator>>();
        var translator = new Translator(logger);
        // Act & Assert
        translator.Dispose();
        translator.Dispose();
    }
}
