using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Constants;

/// <summary>
/// Reads and writes an <see cref="IssueState"/> as the plain string Cronitor uses, e.g. "resolved".
/// </summary>
public class IssueStateConverter : JsonConverter<IssueState>
{
    public override IssueState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) // skipcq: CS-R1138 Signature is defined by JsonConverter<T>.
    {
        var value = reader.GetString();
        return value != null ? new IssueState(value) : null;
    }

    public override void Write(Utf8JsonWriter writer, IssueState value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.State);
    }
}
