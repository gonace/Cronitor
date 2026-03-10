using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class PauseMonitorRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new PauseMonitorRequest("abc123");

            Assert.Equal("monitors/:key/pause", request.Endpoint);
        }

        [Fact]
        public void ShouldSetEndpointWithDuration()
        {
            var request = new PauseMonitorRequest("abc123", 60);

            Assert.Equal("monitors/:key/pause/:duration", request.Endpoint);
        }

        [Fact]
        public void ShouldSetKey()
        {
            var request = new PauseMonitorRequest("abc123");

            Assert.Equal("abc123", request.Key);
        }

        [Fact]
        public void ShouldSetDuration()
        {
            var request = new PauseMonitorRequest("abc123", 60);

            Assert.Equal(60, request.Duration);
        }

        [Fact]
        public void ShouldNotSetDurationWhenNotProvided()
        {
            var request = new PauseMonitorRequest("abc123");

            Assert.Null(request.Duration);
        }

        [Fact]
        public void ShouldBuildUriWithoutDuration()
        {
            var request = new PauseMonitorRequest("abc123");

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/monitors/abc123/pause", uri.ToString());
        }

        [Fact]
        public void ShouldBuildUriWithDuration()
        {
            var request = new PauseMonitorRequest("abc123", 60);

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/monitors/abc123/pause/60", uri.ToString());
        }
    }
}
