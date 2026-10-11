using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Constants
{
    /// <summary>
    /// Reads and writes an <see cref="IssueSeverity"/> as the plain string Cronitor uses, e.g. "outage".
    /// </summary>
    public class IssueSeverityConverter : JsonConverter<IssueSeverity>
    {
        public override IssueSeverity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) // skipcq: CS-R1138 Signature is defined by JsonConverter<T>.
        {
            var value = reader.GetString();
            return value != null ? new IssueSeverity(value) : null;
        }

        public override void Write(Utf8JsonWriter writer, IssueSeverity value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Severity);
        }
    }
}
