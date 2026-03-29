using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class DeleteIssueRequestTests
    {
        [Fact]
        public void ShouldSetEndpoint()
        {
            var request = new DeleteIssueRequest("issue-001");

            Assert.Equal("issues/:key", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToDelete()
        {
            var request = new DeleteIssueRequest("issue-001");

            Assert.Equal(HttpMethod.Delete, request.Method);
        }

        [Fact]
        public void ShouldSetKey()
        {
            var request = new DeleteIssueRequest("issue-001");

            Assert.Equal("issue-001", request.Key);
        }

        [Fact]
        public void ShouldBuildUriWithKey()
        {
            var request = new DeleteIssueRequest("issue-001");

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues/issue-001", uri.ToString());
        }
    }
}