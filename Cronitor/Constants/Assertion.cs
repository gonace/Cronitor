using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cronitor.Constants
{
    public static class Assertion
    {
        public static MetricAssertion Metric => new MetricAssertion();
        public static ResponseAssertion Response => new ResponseAssertion();
    }

    public class MetricAssertion
    {
        public AssertionBuilder Duration => new AssertionBuilder("metric.duration");
        public AssertionBuilder Count => new AssertionBuilder("metric.count");
        public AssertionBuilder ErrorCount => new AssertionBuilder("metric.error_count");
    }

    public class ResponseAssertion
    {
        public AssertionBuilder Code => new AssertionBuilder("response.code");
        public AssertionBuilder Time => new AssertionBuilder("response.time");
        public AssertionBuilder Body => new AssertionBuilder("response.body");

        public AssertionBuilder Json(string key) => new AssertionBuilder("response.json", key);
        public AssertionBuilder Header(string key) => new AssertionBuilder("response.header", key);
    }

    public class AssertionBuilder
    {
        private readonly string _assertion;
        private readonly string _key;

        public AssertionBuilder(string assertion, string key = null)
        {
            _assertion = assertion;
            _key = key;
        }

        public new AssertionRule Equals(object value) => Build("=", value);
        public AssertionRule LessThan(object value) => Build("<", value);
        public AssertionRule GreaterThan(object value) => Build(">", value);
        public AssertionRule Contains(object value) => Build("contains", value);

        private AssertionRule Build(string op, object value)
        {
            var assertion = _key != null
                ? $"{_assertion} {_key} {op} {value}"
                : $"{_assertion} {op} {value}";

            return new AssertionRule(assertion);
        }
    }

    [JsonConverter(typeof(AssertionRuleConverter))]
    public class AssertionRule
    {
        public string Value { get; }

        public AssertionRule(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static implicit operator string(AssertionRule rule) => rule?.Value;
        public static implicit operator AssertionRule(string value) => new AssertionRule(value);
    }

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
