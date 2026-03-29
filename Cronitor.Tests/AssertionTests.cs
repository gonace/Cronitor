using Cronitor.Constants;
using Xunit;

namespace Cronitor.Tests
{
    public class AssertionTests
    {
        [Fact]
        public void MetricDurationLessThan()
        {
            var result = Assertion.Metric.Duration.LessThan("30s");

            Assert.Equal("metric.duration < 30s", result);
        }

        [Fact]
        public void MetricErrorCountLessThan()
        {
            var result = Assertion.Metric.ErrorCount.LessThan(5);

            Assert.Equal("metric.error_count < 5", result);
        }

        [Fact]
        public void MetricCountGreaterThan()
        {
            var result = Assertion.Metric.Count.GreaterThan(0);

            Assert.Equal("metric.count > 0", result);
        }

        [Fact]
        public void ResponseCodeEquals()
        {
            var result = Assertion.Response.Code.Equals(200);

            Assert.Equal("response.code = 200", result);
        }

        [Fact]
        public void ResponseTimeLessThan()
        {
            var result = Assertion.Response.Time.LessThan("2s");

            Assert.Equal("response.time < 2s", result);
        }

        [Fact]
        public void ResponseBodyContains()
        {
            var result = Assertion.Response.Body.Contains("healthy");

            Assert.Equal("response.body contains healthy", result);
        }

        [Fact]
        public void ResponseJsonGreaterThan()
        {
            var result = Assertion.Response.Json("user.count").GreaterThan(10);

            Assert.Equal("response.json user.count > 10", result);
        }

        [Fact]
        public void ResponseHeaderEquals()
        {
            var result = Assertion.Response.Header("X-Version").Equals("1.2.3");

            Assert.Equal("response.header X-Version = 1.2.3", result);
        }
    }
}
