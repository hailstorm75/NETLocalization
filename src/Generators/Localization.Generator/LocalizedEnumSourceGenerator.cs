using Localization.Generator.Translation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Localization.Generator;

#pragma warning disable RS2008

[Generator(LanguageNames.CSharp)]
public sealed class LocalizedEnumSourceGenerator : IIncrementalGenerator
{
    private const string ENUM_ATTRIBUTE_NAME = "Localization.Shared.Attributes.LocalizedEnumAttribute";
    private const string ENUM_FIELD_ATTRIBUTE_NAME = "Localization.Shared.Attributes.LocalizedEnumFieldAttribute";
    private const string PROVIDER_ATTRIBUTE_NAME = "Localization.Shared.Attributes.TranslationProviderAttribute";
    private const string L_ENUM_NAME = "Localization.Shared.Models.LEnum";
    private const string TOOL_NAME = "Localizations.Generator";
    private const string VERSION = "1.0.0";

    private static readonly DiagnosticDescriptor EMPTY_NAMESPACE = new(
        "LOCENUM001",
        "Localized enum namespace is missing",
        "Localized enum '{0}' must specify a non-empty localization namespace",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor INVALID_PROVIDER_TYPE = new(
        "LOCENUM002",
        "Localized enum field provider is invalid",
        "Provider type '{0}' referenced by enum field '{1}' must be marked with TranslationProviderAttribute",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MISSING_PROVIDER_KEY = new(
        "LOCENUM003",
        "Localized enum field key is missing",
        "Translation key '{0}' referenced by enum field '{1}' was not found in provider '{2}'",
        "Localization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private sealed record TranslationFile(string Name, TranslationsCollection Collection);
    private sealed record ProviderData(INamedTypeSymbol Symbol, string Source);
    private sealed record FieldData(IFieldSymbol Symbol, string Key, INamedTypeSymbol? ProviderType);
    private sealed record EnumData(INamedTypeSymbol Symbol, string Namespace, ImmutableArray<FieldData> Fields);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var translations = context.AdditionalTextsProvider
            .Where(static text => text.Path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            .Select(static (text, _) =>
            {
                var data = text.GetText()?.ToString();
                if (data is null)
                    return null;

                return ParserHelper.TryParse(data, out var collection)
                    ? new TranslationFile(Path.GetFileNameWithoutExtension(text.Path), collection)
                    : null;
            })
            .Where(static file => file is not null)
            .Collect();

        var providers = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: PROVIDER_ATTRIBUTE_NAME,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (context, _) =>
                {
                    var symbol = (INamedTypeSymbol)context.TargetSymbol;
                    var attribute = context.Attributes.FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == PROVIDER_ATTRIBUTE_NAME);
                    var source = attribute?.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    return string.IsNullOrEmpty(source) ? null : new ProviderData(symbol, source!);
                })
            .Where(static provider => provider is not null)
            .Collect();

        var enums = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: ENUM_ATTRIBUTE_NAME,
                predicate: static (node, _) => node is EnumDeclarationSyntax,
                transform: static (context, _) =>
                {
                    var symbol = (INamedTypeSymbol)context.TargetSymbol;
                    var attribute = context.Attributes.FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == ENUM_ATTRIBUTE_NAME);
                    var @namespace = attribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;

                    var fields = symbol.GetMembers()
                        .OfType<IFieldSymbol>()
                        .Where(static field => field.HasConstantValue)
                        .Select(static field =>
                        {
                            var fieldAttribute = field.GetAttributes()
                                .FirstOrDefault(static attr => attr.AttributeClass?.ToDisplayString() == ENUM_FIELD_ATTRIBUTE_NAME);
                            if (fieldAttribute is null)
                                return new FieldData(field, field.Name, null);

                            if (fieldAttribute.ConstructorArguments.Length == 2)
                            {
                                var provider = fieldAttribute.ConstructorArguments[0].Value as INamedTypeSymbol;
                                var key = fieldAttribute.ConstructorArguments[1].Value?.ToString();
                                return new FieldData(field, string.IsNullOrWhiteSpace(key) ? field.Name : key!, provider);
                            }

                            var explicitKey = fieldAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                            return new FieldData(field, string.IsNullOrWhiteSpace(explicitKey) ? field.Name : explicitKey!, null);
                        })
                        .ToImmutableArray();

                    return new EnumData(symbol, @namespace, fields);
                })
            .Collect();

        var pipeline = translations.Combine(providers).Combine(enums);
        context.RegisterSourceOutput(pipeline, static (ctx, data) => Execute(ctx, data.Left.Left, data.Left.Right, data.Right));
    }

    private static void Execute(
        SourceProductionContext context,
        ImmutableArray<TranslationFile?> translationFiles,
        ImmutableArray<ProviderData?> providers,
        ImmutableArray<EnumData> enums)
    {
        var translationsBySource = translationFiles
            .Where(static file => file is not null)
            .ToDictionary(static file => file!.Name, static file => file!.Collection, StringComparer.OrdinalIgnoreCase);

        var providerSet = providers
            .Where(static provider => provider is not null)
            .Select(static provider => provider!)
            .ToArray();

        foreach (var enumData in enums)
        {
            if (string.IsNullOrWhiteSpace(enumData.Namespace))
                context.ReportDiagnostic(Diagnostic.Create(EMPTY_NAMESPACE, enumData.Symbol.Locations.FirstOrDefault(), enumData.Symbol.Name));

            foreach (var field in enumData.Fields.Where(static field => field.ProviderType is not null))
            {
                var provider = providerSet.FirstOrDefault(candidate => SymbolEqualityComparer.Default.Equals(candidate.Symbol, field.ProviderType));
                if (provider is null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        INVALID_PROVIDER_TYPE,
                        field.Symbol.Locations.FirstOrDefault(),
                        field.ProviderType!.ToDisplayString(),
                        field.Symbol.Name));
                    continue;
                }

                if (translationsBySource.TryGetValue(provider.Source, out var translations)
                    && !translations.Any(translation => translation.Key.Equals(field.Key, StringComparison.OrdinalIgnoreCase)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        MISSING_PROVIDER_KEY,
                        field.Symbol.Locations.FirstOrDefault(),
                        field.Key,
                        field.Symbol.Name,
                        provider.Symbol.ToDisplayString()));
                }
            }

            var source = GenerateExtensions(enumData);
            context.AddSource($"{GetSafeHintName(enumData.Symbol)}.LocalizedEnum.g.cs", source);
        }
    }

    private static string GenerateExtensions(EnumData enumData)
    {
        var baseTextWriter = new StringWriter();
        var indentWriter = new IndentedTextWriter(baseTextWriter, "    ");
        var enumType = enumData.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var className = $"{NormalizeIdentifier(enumData.Symbol.Name)}LocalizationExtensions";
        var @namespace = enumData.Symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : enumData.Symbol.ContainingNamespace.ToDisplayString();

        indentWriter.WriteLine("// <auto-generated />");
        indentWriter.WriteLine("#pragma warning disable 1591");
        indentWriter.WriteLine("#nullable enable");
        indentWriter.WriteLineNoTabs(string.Empty);

        if (@namespace is not null)
        {
            indentWriter.WriteLine($"namespace {@namespace}");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
        }

        indentWriter.WriteLine($"[global::System.CodeDom.Compiler.GeneratedCode(\"{TOOL_NAME}\", \"{VERSION}\")]");
        indentWriter.WriteLine($"public static partial class {className}");
        indentWriter.WriteLine("{");
        indentWriter.Indent++;

        indentWriter.WriteLine($"public static global::{L_ENUM_NAME}? ToLEnum(this {enumType} value)");
        indentWriter.Indent++;
        indentWriter.WriteLine("=> global::Localization.Shared.LocalizationRuntime.GetTranslator()?.GetEnum(value);");
        indentWriter.Indent--;

        indentWriter.WriteLineNoTabs(string.Empty);

        indentWriter.WriteLine($"public static global::System.Collections.Generic.IEnumerable<global::{L_ENUM_NAME}> GetLocalizedValues()");
        indentWriter.WriteLine("{");
        indentWriter.Indent++;
        foreach (var field in enumData.Fields)
            indentWriter.WriteLine($"yield return global::Localization.Shared.LocalizationRuntime.GetTranslator()?.GetEnum({enumType}.{field.Symbol.Name}) ?? global::{L_ENUM_NAME}.INVALID;");
        indentWriter.Indent--;
        indentWriter.WriteLine("}");

        indentWriter.Indent--;
        indentWriter.WriteLine("}");

        if (@namespace is not null)
        {
            indentWriter.Indent--;
            indentWriter.WriteLine("}");
        }

        return baseTextWriter.ToString();
    }

    private static string NormalizeIdentifier(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "_";

        var result = new char[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            result[i] = char.IsLetterOrDigit(c) || c == '_' ? c : '_';
            if (i == 0 && char.IsDigit(result[i]))
                result[i] = '_';
        }

        return new string(result);
    }

    private static string GetSafeHintName(INamedTypeSymbol symbol)
        => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", string.Empty)
            .Replace(".", "_")
            .Replace("+", "_");
}
