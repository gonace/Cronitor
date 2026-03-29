using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Scheduling
{
    public class ScheduleExpressionConverter : JsonConverter<ScheduleExpression>
    {
#pragma warning disable CS-R1138 // Parameter order is dictated by JsonConverter<T> base class
        public override ScheduleExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
#pragma warning restore CS-R1138
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
