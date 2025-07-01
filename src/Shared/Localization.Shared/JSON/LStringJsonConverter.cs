using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Localization.Shared.Models;

namespace Localization.Shared.JSON;

/// <summary>
/// JSON converter for <see cref="LString"/> instances
/// </summary>
public sealed class LStringJsonConverter : JsonConverter<LString>
{
    /// <inheritdoc />
    public override LString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var obj = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
        if (obj is null)
            throw new JsonException();

        var @namespace = obj[nameof(LString.Namespace)]?.GetValue<string>();
        var key = obj[nameof(LString.Key)]?.GetValue<string>();

        if (@namespace is null || key is null)
            throw new JsonException();

        if (CultureManager.TryRetrieveString(@namespace, key, out var cached))
            return cached;

        return new LString
        {
            Namespace = @namespace,
            Key = key
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, LString value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        {
            writer.WriteString(nameof(LString.Namespace), value.Namespace);
            writer.WriteString(nameof(LString.Key), value.Key);
        }
        writer.WriteEndObject();
    }
}
