using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Scheduling
{
    [JsonConverter(typeof(ScheduleExpressionConverter))]
    public class ScheduleExpression
    {
        public string Value { get; }

        public ScheduleExpression(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static implicit operator string(ScheduleExpression expression) => expression?.Value;
        public static implicit operator ScheduleExpression(string value) => new ScheduleExpression(value);
    }

    public class ScheduleExpressionConverter : JsonConverter<ScheduleExpression>
    {
        public override ScheduleExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new ScheduleExpression(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, ScheduleExpression value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
