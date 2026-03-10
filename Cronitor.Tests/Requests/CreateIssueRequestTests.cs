using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests.Requests
{
    public class CreateIssueRequestTests
    {
        [Theory]
        [JsonData("CreateIssueRequest.json")]
        public async Task ShouldCreateIssueRequestAsync(string expected)
        {
            var issue = new Issue
            {
                Key = "issue-001",
                Name = "Database Outage",
                Environment = "Production",
                Severity = IssueSeverity.Outage,
                State = IssueState.Unresolved
            };
            var request = new CreateIssueRequest(issue);

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
            var request = new CreateIssueRequest(issue);

            Assert.Equal("issues", request.Endpoint);
        }

        [Fact]
        public void ShouldSetHttpMethodToPost()
        {
            var issue = new Issue { Key = "test" };
            var request = new CreateIssueRequest(issue);

            Assert.Equal(HttpMethod.Post, request.Method);
        }

        [Fact]
        public void ShouldBuildUri()
        {
            var issue = new Issue { Key = "test" };
            var request = new CreateIssueRequest(issue);

            var uri = request.ToUri();

            Assert.Equal("https://cronitor.io/api/issues", uri.ToString());
        }
    }
}
