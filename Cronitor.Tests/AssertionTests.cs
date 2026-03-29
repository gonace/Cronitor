using Cronitor.Constants;
using Cronitor.Serialization;
using System.Collections.Generic;
using Xunit;

namespace Cronitor.Tests
{
    public class AssertionTests
    {
        [Fact]
        public void MetricDurationLessThan()
        {
            var result = Assertion.Metric.Duration.LessThan("30s");

            Assert.Equal("metric.duration < 30s", result.Value);
        }

        [Fact]
        public void MetricErrorCountLessThan()
        {
            var result = Assertion.Metric.ErrorCount.LessThan(5);

            Assert.Equal("metric.error_count < 5", result.Value);
        }

        [Fact]
        public void MetricCountGreaterThan()
        {
            var result = Assertion.Metric.Count.GreaterThan(0);

            Assert.Equal("metric.count > 0", result.Value);
        }

        [Fact]
        public void ResponseCodeEquals()
        {
            var result = Assertion.Response.Code.Equals(200);

            Assert.Equal("response.code = 200", result.Value);
        }

        [Fact]
        public void ResponseTimeLessThan()
        {
            var result = Assertion.Response.Time.LessThan("2s");

            Assert.Equal("response.time < 2s", result.Value);
        }

        [Fact]
        public void ResponseBodyContains()
        {
            var result = Assertion.Response.Body.Contains("healthy");

            Assert.Equal("response.body contains healthy", result.Value);
        }

        [Fact]
        public void ResponseJsonGreaterThan()
        {
            var result = Assertion.Response.Json("user.count").GreaterThan(10);

            Assert.Equal("response.json user.count > 10", result.Value);
        }

        [Fact]
        public void ResponseHeaderEquals()
        {
            var result = Assertion.Response.Header("X-Version").Equals("1.2.3");

            Assert.Equal("response.header X-Version = 1.2.3", result.Value);
        }

        [Fact]
        public void ToStringReturnsValue()
        {
            var rule = Assertion.Metric.Duration.LessThan("30s");

            Assert.Equal("metric.duration < 30s", rule.ToString());
        }

        [Fact]
        public void ImplicitStringConversion()
        {
            string result = Assertion.Response.Code.Equals(200);

            Assert.Equal("response.code = 200", result);
        }

        [Fact]
        public void ImplicitAssertionRuleFromString()
        {
            AssertionRule rule = "metric.duration < 30s";

            Assert.Equal("metric.duration < 30s", rule.Value);
        }

        [Fact]
        public void SerializesAsJsonString()
        {
            var rules = new List<AssertionRule>
            {
                Assertion.Metric.Duration.LessThan("15min")
            };

            var json = Serializer.Serialize(new { assertions = rules });

            Assert.Equal("{\"assertions\":[\"metric.duration < 15min\"]}", json);
        }

        [Fact]
        public void DeserializesFromJsonString()
        {
            var json = "{\"assertions\":[\"metric.duration < 15min\"]}";

            var result = Serializer.Deserialize<AssertionContainer>(json);

            Assert.Single(result.Assertions);
            Assert.Equal("metric.duration < 15min", result.Assertions[0].Value);
        }

        private class AssertionContainer
        {
            [System.Text.Json.Serialization.JsonPropertyName("assertions")]
            public List<AssertionRule> Assertions { get; set; }
        }
    }
}
