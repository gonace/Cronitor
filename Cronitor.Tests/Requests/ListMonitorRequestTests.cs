using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class ListMonitorRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new ListMonitorRequest();

            Assert.Equal("monitors", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToGet()
        {
            var request = new ListMonitorRequest();

            Assert.Equal(HttpMethod.Get, request.Method);
        }

        [Fact]
        public void ShouldDefaultPageToOne()
        {
            var request = new ListMonitorRequest();

            Assert.Equal(1, request.Page);
        }

        [Fact]
        public void ShouldBuildUriWithDefaultPage()
        {
            var request = new ListMonitorRequest();

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/monitors?page=1", uri.ToString());
        }

        [Fact]
        public void ShouldBuildUriWithCustomPage()
        {
            var request = new ListMonitorRequest { Page = 3 };

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/monitors?page=3", uri.ToString());
        }
    }
}
