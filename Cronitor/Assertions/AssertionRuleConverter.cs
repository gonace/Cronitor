using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Assertions
{
    public class AssertionRuleConverter : JsonConverter<AssertionRule>
    {
        public override AssertionRule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new AssertionRule(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, AssertionRule value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}