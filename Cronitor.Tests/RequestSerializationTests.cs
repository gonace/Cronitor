using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Models.Monitors;
using Xunit;

namespace Cronitor.Tests
{
    public class RequestSerializationTests
    {
        [Fact]
        public void ShouldSendTheUrlAndRegionsOfACheck()
        {
            var check = new Check("api-health", new Request("https://example.com/health", new[] { Region.California }));

            var json = Serializer.Serialize(check);

            Assert.Contains("\"url\":\"https://example.com/health\"", json);
            Assert.Contains("\"regions\":[\"us-west-1\"]", json);
        }

        [Fact]
        public void ShouldReadTheUrlAndRegionsOfACheck()
        {
            const string json = "{\"type\":\"check\",\"key\":\"api-health\",\"request\":{\"url\":\"https://example.com/health\",\"regions\":[\"us-west-1\"]}}";

            var check = Serializer.Deserialize<Check>(json);

            Assert.Equal("https://example.com/health", check.Request.Url);
            Assert.Equal(new[] { "us-west-1" }, check.Request.Regions);
        }
    }
}
