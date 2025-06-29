using System.Text.Json.Serialization;
using Localization.Shared.Models;

namespace Localization.Shared.JSON;

[JsonSerializable(typeof(LString))]
[JsonSourceGenerationOptions(
    WriteIndented = false,
    Converters = [typeof(LStringJsonConverter)],
    DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class LStringJsonGenerationContext : JsonSerializerContext;