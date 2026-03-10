using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class UpdateIssueRequestTests
    {
        [Theory]
        [JsonData("UpdateIssueRequest.json")]
        public async Task ShouldCreateUpdateIssueRequestAsync(string expected)
        {
            var issue = new Issue
            {
                Key = "issue-001",
                Name = "Database Outage",
                Environment = "Production",
                Severity = IssueSeverity.Outage,
                State = IssueState.Investigating
            };
            var request = new UpdateIssueRequest("issue-001", issue);

#if NET8_0_OR_GREATER
            var actual = await request.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
#else
            var actual = await request.Content.ReadAsStringAsync();
#endif
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldSetEndpoint()
        {
            var issue = new Issue { Key = "test" };
            var request = new UpdateIssueRequest("test", issue);

            Assert.Equal("issues/:key", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToPut()
        {
            var issue = new Issue { Key = "test" };
            var request = new UpdateIssueRequest("test", issue);

            Assert.Equal(HttpMethod.Put, request.Method);
        }

        [Fact]
        public void ShouldSetKey()
        {
            var issue = new Issue { Key = "test" };
            var request = new UpdateIssueRequest("issue-001", issue);

            Assert.Equal("issue-001", request.Key);
        }

        [Fact]
        public void ShouldBuildUriWithKey()
        {
            var issue = new Issue { Key = "test" };
            var request = new UpdateIssueRequest("issue-001", issue);

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues/issue-001", uri.ToString());
        }
    }
}
