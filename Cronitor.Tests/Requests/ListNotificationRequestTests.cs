using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class ListNotificationRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new ListNotificationRequest();

            Assert.Equal("templates", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToGet()
        {
            var request = new ListNotificationRequest();

            Assert.Equal(HttpMethod.Get, request.Method);
        }

        [Fact]
        public void ShouldDefaultPageToOne()
        {
            var request = new ListNotificationRequest();

            Assert.Equal(1, request.Page);
        }

        [Fact]
        public void ShouldBuildUriWithDefaultPage()
        {
            var request = new ListNotificationRequest();

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/templates?page=1", uri.ToString());
        }

        [Fact]
        public void ShouldBuildUriWithCustomPage()
        {
            var request = new ListNotificationRequest { Page = 5 };

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/templates?page=5", uri.ToString());
        }
    }
}
