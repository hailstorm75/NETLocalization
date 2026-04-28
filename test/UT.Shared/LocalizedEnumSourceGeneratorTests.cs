using Localization.Generator;
using Localization.Shared.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using System.Text;
using Xunit;

namespace UT.Shared;

public sealed class LocalizedEnumSourceGeneratorTests
{
    [Fact]
    public void Generator_EmitsLocalizedEnumHelpers()
    {
        // Arrange
        var source = """
                     using Localization.Shared.Attributes;

                     namespace TestProject;

                     [TranslationProvider("ProviderStrings")]
                     public sealed partial class Provider;

                     [LocalizedEnum("Test")]
                     public enum Status
                     {
                         Draft,
                         [LocalizedEnumField(typeof(Provider), nameof(Provider.Archived))]
                         Archived
                     }
                     """;
        var additionalText = new TestAdditionalText("ProviderStrings.xml", """
            <translations namespace="Test">
              <set key="Draft" description="Draft">
                <item lang="en">Draft</item>
              </set>
              <set key="Archived" description="Archived">
                <item lang="en">Archived</item>
              </set>
            </translations>
            """);
        // Act
        var result = RunGenerator(source, additionalText);
        // Assert
        result.GeneratedTrees.Select(tree => tree.GetText().ToString()).ShouldContain(text =>
            text.Contains("StatusLocalizationExtensions", StringComparison.Ordinal)
            && text.Contains("ToLEnum(this global::TestProject.Status value)", StringComparison.Ordinal)
            && text.Contains("GetLocalizedValues()", StringComparison.Ordinal));
    }

    [Fact]
    public void Generator_ReportsInvalidProviderType()
    {
        // Arrange
        var source = """
                     using Localization.Shared.Attributes;

                     namespace TestProject;

                     public sealed class NotProvider;

                     [LocalizedEnum("Test")]
                     public enum Status
                     {
                         [LocalizedEnumField(typeof(NotProvider), "Draft")]
                         Draft
                     }
                     """;
        // Act
        var result = RunGenerator(source);
        // Assert
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Id == "LOCENUM002");
    }

    [Fact]
    public void Generator_ReportsMissingProviderKey()
    {
        // Arrange
        var source = """
                     using Localization.Shared.Attributes;

                     namespace TestProject;

                     [TranslationProvider("ProviderStrings")]
                     public sealed partial class Provider;

                     [LocalizedEnum("Test")]
                     public enum Status
                     {
                         [LocalizedEnumField(typeof(Provider), "Missing")]
                         Missing
                     }
                     """;
        var additionalText = new TestAdditionalText("ProviderStrings.xml", """
            <translations namespace="Test">
              <set key="Other" description="Other">
                <item lang="en">Other</item>
              </set>
            </translations>
            """);
        // Act
        var result = RunGenerator(source, additionalText);
        // Assert
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Id == "LOCENUM003");
    }

    private static GeneratorDriverRunResult RunGenerator(string source, params AdditionalText[] additionalTexts)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .Cast<MetadataReference>()
            .ToList();
        references.Add(MetadataReference.CreateFromFile(typeof(LocalizedEnumAttribute).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            "GeneratorTests",
            [CSharpSyntaxTree.ParseText(source)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [new LocalizedEnumSourceGenerator().AsSourceGenerator()],
            additionalTexts: additionalTexts,
            parseOptions: CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        return driver.GetRunResult();
    }

    private sealed class TestAdditionalText(string path, string text) : AdditionalText
    {
        public override string Path { get; } = path;

        public override SourceText GetText(CancellationToken cancellationToken = default)
            => SourceText.From(text, Encoding.UTF8);
    }
}
