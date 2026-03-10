using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class ListMonitorActivitiesRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new ListActivitiesRequest("abc123");

            Assert.Equal("monitors/:key/activity", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToGet()
        {
            var request = new ListActivitiesRequest("abc123");

            Assert.Equal(HttpMethod.Get, request.Method);
        }

        [Fact]
        public void ShouldSetKey()
        {
            var request = new ListActivitiesRequest("abc123");

            Assert.Equal("abc123", request.Key);
        }

        [Fact]
        public void ShouldBuildUriWithKey()
        {
            var request = new ListActivitiesRequest("abc123");

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/monitors/abc123/activity", uri.ToString());
        }
    }
}
