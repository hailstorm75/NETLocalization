using Bogus;
using Localization.Shared;
using Localization.Shared.Attributes;
using Localization.Shared.Interfaces;
using Localization.Shared.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace UT.Shared;

public sealed class LEnumTests
{
    private enum TestEnum { Value1, Value2 }

    [LocalizedEnum<TestProvider>]
    private enum LocalizedTestEnum
    {
        Value1,

        [LocalizedEnumField("ExplicitValue")]
        Value2,

        [LocalizedEnumField(typeof(TestProvider), nameof(TestProvider.StrongValue))]
        Value3
    }

    private enum UnlocalizedTestEnum
    {
        Value1
    }

    private sealed class TestProvider : ITranslationProvider
    {
        public static LString StrongValue { get; } = new()
        {
            Namespace = GetNamespace(),
            Key = nameof(StrongValue)
        };

        public static string GetNamespace() => "EnumTests";
    }

    [Fact]
    public void LEnum_Invalid_IsSingleton()
    {
        // Arrange & Act
        var invalid1 = LEnum.INVALID;
        var invalid2 = LEnum.INVALID;
        // Assert
        invalid1.ShouldBeSameAs(invalid2);
    }

    [Fact]
    public void LEnum_Constructor_SetsEnumField()
    {
        // Arrange
        var faker = new Faker();
        var enumValue = TestEnum.Value1;
        // Act
        var lEnum = new LEnum(enumValue);
        // Assert
        lEnum.EnumField.ShouldBe(enumValue);
    }

    [Fact]
    public void LEnum_UsesEnumFieldName_WhenFieldAttributeIsMissing()
    {
        // Arrange
        var translator = CreateTranslator();
        translator.RegisterTranslations(CreateSet(nameof(LocalizedTestEnum.Value1), "Default"));
        // Act
        var lEnum = translator.GetEnum(LocalizedTestEnum.Value1);
        // Assert
        lEnum.String.ShouldBe("Default");
    }

    [Fact]
    public void LEnum_UsesExplicitLiteralKey()
    {
        // Arrange
        var translator = CreateTranslator();
        translator.RegisterTranslations(CreateSet("ExplicitValue", "Explicit"));
        // Act
        var lEnum = translator.GetEnum(LocalizedTestEnum.Value2);
        // Assert
        lEnum.String.ShouldBe("Explicit");
    }

    [Fact]
    public void LEnum_UsesProviderScopedKey()
    {
        // Arrange
        var translator = CreateTranslator();
        translator.RegisterTranslations(CreateSet(nameof(TestProvider.StrongValue), "Strong"));
        // Act
        var lEnum = translator.GetEnum(LocalizedTestEnum.Value3);
        // Assert
        lEnum.String.ShouldBe("Strong");
    }

    [Fact]
    public void LEnum_ReturnsInvalidMarker_WhenEnumAttributeIsMissing()
    {
        // Arrange
        var translator = CreateTranslator();
        // Act
        var lEnum = translator.GetEnum(UnlocalizedTestEnum.Value1);
        // Assert
        lEnum.String.ShouldBe("#Value1");
    }

    [Fact]
    public void LEnum_UpdatesString_WhenCultureChanges()
    {
        // Arrange
        var translator = CreateTranslator();
        translator.RegisterTranslations(new TranslationSet
        {
            Source = new LString { Namespace = "EnumTests", Key = nameof(LocalizedTestEnum.Value1) },
            Translations = new Dictionary<string, string>
            {
                ["en"] = "English",
                ["de"] = "German"
            }
        });
        var lEnum = translator.GetEnum(LocalizedTestEnum.Value1);
        // Act
        translator.ChangeCulture(new Language { Key = "de", DisplayName = "German" });
        // Assert
        lEnum.String.ShouldBe("German");
    }

    [Fact]
    public void LEnum_IsCachedByTranslator()
    {
        // Arrange
        var translator = CreateTranslator();
        translator.RegisterTranslations(CreateSet(nameof(LocalizedTestEnum.Value1), "Default"));
        // Act
        var first = translator.GetEnum(LocalizedTestEnum.Value1);
        var second = translator.GetEnum(LocalizedTestEnum.Value1);
        // Assert
        second.ShouldBeSameAs(first);
    }

    private static Translator CreateTranslator()
    {
        var translator = new Translator(Substitute.For<ILogger<Translator>>());
        CultureManager.Initialize(translator);

        return translator;
    }

    private static TranslationSet CreateSet(string key, string value)
        => new()
        {
            Source = new LString { Namespace = "EnumTests", Key = key },
            Translations = new Dictionary<string, string> { ["en"] = value }
        };
}
