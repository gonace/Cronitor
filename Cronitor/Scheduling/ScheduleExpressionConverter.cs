using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Scheduling
{
    public class ScheduleExpressionConverter : JsonConverter<ScheduleExpression>
    {
        public override ScheduleExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return value != null ? new ScheduleExpression(value) : null;
        }

        public override void Write(Utf8JsonWriter writer, ScheduleExpression value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
