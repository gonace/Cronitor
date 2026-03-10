using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class GetMonitorRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new GetMonitorRequest("abc123");

            Assert.Equal("monitors/:key", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToGet()
        {
            var request = new GetMonitorRequest("abc123");

            Assert.Equal(HttpMethod.Get, request.Method);
        }

        [Fact]
        public void ShouldSetKey()
        {
            var request = new GetMonitorRequest("abc123");

            Assert.Equal("abc123", request.Key);
        }

        [Fact]
        public void ShouldBuildUriWithKey()
        {
            var request = new GetMonitorRequest("abc123");

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/monitors/abc123", uri.ToString());
        }
    }
}
