using System.Collections.Generic;
using System.Text.Json.Serialization;
using Cronitor.Assertions;
using Xunit;

namespace Cronitor.Tests;

public class AssertionTests
{
    [Fact]
    public void Should_build_Metric_Duration_LessThan()
    {
        var result = Assertion.Metric.Duration.LessThan("30s");

        Assert.Equal("metric.duration < 30s", result.Value);
    }

    [Fact]
    public void Should_build_Metric_ErrorCount_LessThan()
    {
        var result = Assertion.Metric.ErrorCount.LessThan(5);

        Assert.Equal("metric.error_count < 5", result.Value);
    }

    [Fact]
    public void Should_build_Metric_Count_GreaterThan()
    {
        var result = Assertion.Metric.Count.GreaterThan(0);

        Assert.Equal("metric.count > 0", result.Value);
    }

    [Fact]
    public void Should_build_Response_Code_Equals()
    {
        var result = Assertion.Response.Code.Equals(200);

        Assert.Equal("response.code = 200", result.Value);
    }

    [Fact]
    public void Should_build_Response_Time_LessThan()
    {
        var result = Assertion.Response.Time.LessThan("2s");

        Assert.Equal("response.time < 2s", result.Value);
    }

    [Fact]
    public void Should_build_Response_Body_Contains()
    {
        var result = Assertion.Response.Body.Contains("healthy");

        Assert.Equal("response.body contains healthy", result.Value);
    }

    [Fact]
    public void Should_build_Response_Json_GreaterThan()
    {
        var result = Assertion.Response.Json("user.count").GreaterThan(10);

        Assert.Equal("response.json user.count > 10", result.Value);
    }

    [Fact]
    public void Should_build_Response_Header_Equals()
    {
        var result = Assertion.Response.Header("X-Version").Equals("1.2.3");

        Assert.Equal("response.header X-Version = 1.2.3", result.Value);
    }

    [Fact]
    public void Should_return_the_value_from_ToString()
    {
        var rule = Assertion.Metric.Duration.LessThan("30s");

        Assert.Equal("metric.duration < 30s", rule.ToString());
    }

    [Fact]
    public void Should_convert_implicitly_to_a_string()
    {
        string result = Assertion.Response.Code.Equals(200);

        Assert.Equal("response.code = 200", result);
    }

    [Fact]
    public void Should_convert_implicitly_from_a_string_to_an_AssertionRule()
    {
        AssertionRule rule = "metric.duration < 30s";

        Assert.Equal("metric.duration < 30s", rule.Value);
    }

    [Fact]
    public void Should_serialize_as_a_json_string()
    {
        var rules = new List<AssertionRule>
        {
            Assertion.Metric.Duration.LessThan("15min")
        };

        var json = Serializer.Serialize(new { assertions = rules });

        Assert.Equal("{\"assertions\":[\"metric.duration < 15min\"]}", json);
    }

    [Fact]
    public void Should_deserialize_from_a_json_string()
    {
        var json = "{\"assertions\":[\"metric.duration < 15min\"]}";

        var result = Serializer.Deserialize<AssertionContainer>(json);

        Assert.Single(result.Assertions);
        Assert.Equal("metric.duration < 15min", result.Assertions[0].Value);
    }

    private class AssertionContainer
    {
        [JsonPropertyName("assertions")]
        public List<AssertionRule> Assertions { get; set; }
    }
}
