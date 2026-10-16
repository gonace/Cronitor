using Cronitor.Models;
using Cronitor.Models.Monitors;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor;

/// <summary>
/// Writes each monitor as its own type, and reads it as the type Cronitor names in its "type", so what only a
/// subclass has, such as a check's request, is sent and read too.
/// </summary>
internal sealed class MonitorConverter : JsonConverter<Monitor>
{
    private JsonSerializerOptions _withoutThis;

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Monitor);

    public override Monitor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) // skipcq: CS-R1138 Signature is defined by JsonConverter<T>.
    {
        using (var document = JsonDocument.ParseValue(ref reader))
        {
            return (Monitor)document.RootElement.Deserialize(TypeOf(document.RootElement), WithoutThis(options));
        }
    }

    public override void Write(Utf8JsonWriter writer, Monitor value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), WithoutThis(options));
    }

    private static Type TypeOf(JsonElement monitor) =>
        (monitor.TryGetProperty("type", out var type) ? type.GetString() : null) switch
        {
            "job" => typeof(Job),
            "heartbeat" => typeof(Heartbeat),
            "check" => typeof(Check),
            _ => typeof(Monitor)
        };

    private JsonSerializerOptions WithoutThis(JsonSerializerOptions options)
    {
        if (_withoutThis == null)
        {
            var copy = new JsonSerializerOptions(options);
            copy.Converters.Remove(this);
            _withoutThis = copy;
        }

        return _withoutThis;
    }
}
