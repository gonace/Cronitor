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
}
