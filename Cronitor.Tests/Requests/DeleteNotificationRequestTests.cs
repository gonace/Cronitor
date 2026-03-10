using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class DeleteNotificationRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new DeleteNotificationRequest("default");

            Assert.Equal("templates/:key", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToDelete()
        {
            var request = new DeleteNotificationRequest("default");

            Assert.Equal(HttpMethod.Delete, request.Method);
        }

        [Fact]
        public void ShouldSetKey()
        {
            var request = new DeleteNotificationRequest("default");

            Assert.Equal("default", request.Key);
        }

        [Fact]
        public void ShouldBuildUriWithKey()
        {
            var request = new DeleteNotificationRequest("devops-alerts");

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/templates/devops-alerts", uri.ToString());
        }
    }
}
