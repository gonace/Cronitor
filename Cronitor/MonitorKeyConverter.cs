using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor;

internal sealed class MonitorKeyConverter : JsonConverter<MonitorKey>
{
    public override MonitorKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) // skipcq: CS-R1138 Signature is defined by JsonConverter<T>.
    {
        var value = reader.GetString();
        return value != null ? MonitorKey.FromCronitor(value) : null;
    }

    public override void Write(Utf8JsonWriter writer, MonitorKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
