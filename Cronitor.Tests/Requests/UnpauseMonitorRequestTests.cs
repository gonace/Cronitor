using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class UnpauseMonitorRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new UnpauseMonitorRequest("abc123");

            Assert.Equal("monitors/:key/pause/:duration", request.Endpoint);
        }

        [Fact]
        public void ShouldSetKey()
        {
            var request = new UnpauseMonitorRequest("abc123");

            Assert.Equal("abc123", request.Key);
        }

        [Fact]
        public void ShouldBuildUriWithDurationZero()
        {
            var request = new UnpauseMonitorRequest("abc123");

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/monitors/abc123/pause/0", uri.ToString());
        }
    }
}
