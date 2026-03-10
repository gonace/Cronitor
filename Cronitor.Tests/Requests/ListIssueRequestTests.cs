using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class ListIssueRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new ListIssueRequest();

            Assert.Equal("issues", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToGet()
        {
            var request = new ListIssueRequest();

            Assert.Equal(HttpMethod.Get, request.Method);
        }

        [Fact]
        public void ShouldDefaultPageToOne()
        {
            var request = new ListIssueRequest();

            Assert.Equal(1, request.Page);
        }

        [Fact]
        public void ShouldBuildUriWithDefaultPage()
        {
            var request = new ListIssueRequest();

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues?page=1", uri.ToString());
        }

        [Fact]
        public void ShouldBuildUriWithCustomPage()
        {
            var request = new ListIssueRequest { Page = 2 };

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues?page=2", uri.ToString());
        }
    }
}
