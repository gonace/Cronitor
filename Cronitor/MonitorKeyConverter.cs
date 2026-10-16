using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor;

internal sealed class MonitorKeyConverter : JsonConverter<MonitorKey>
{
    private readonly Func<string, MonitorKey> _create;

    public MonitorKeyConverter()
        : this(value => new MonitorKey(value))
    {
    }

    private MonitorKeyConverter(Func<string, MonitorKey> create)
    {
        _create = create;
    }

    internal static MonitorKeyConverter ForCronitorResponses() => new MonitorKeyConverter(MonitorKey.FromCronitor);

    public override MonitorKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) // skipcq: CS-R1138 Signature is defined by JsonConverter<T>.
    {
        var value = reader.GetString();
        return value != null ? _create(value) : null;
    }

    public override void Write(Utf8JsonWriter writer, MonitorKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
