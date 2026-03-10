using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class GetNotificationRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new GetNotificationRequest("default");

            Assert.Equal("templates/:key", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToGet()
        {
            var request = new GetNotificationRequest("default");

            Assert.Equal(HttpMethod.Get, request.Method);
        }

        [Fact]
        public void ShouldSetKey()
        {
            var request = new GetNotificationRequest("default");

            Assert.Equal("default", request.Key);
        }

        [Fact]
        public void ShouldBuildUriWithKey()
        {
            var request = new GetNotificationRequest("devops-alerts");

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/templates/devops-alerts", uri.ToString());
        }
    }
}
