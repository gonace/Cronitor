using System.Text.Json.Serialization;

namespace Cronitor.Assertions
{
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
}