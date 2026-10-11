using System.Net.Http;
using System.Threading.Tasks;
using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Tests.Helpers;
using Xunit;

namespace Cronitor.Tests.Requests;

public class CreateIssueRequestTests
{
    [Theory]
    [JsonData("CreateIssueRequest.json")]
    public async Task Should_create_a_CreateIssueRequest(string expected)
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
    public void Should_set_Endpoint()
    {
        var issue = new Issue { Key = "test" };
        var request = new CreateIssueRequest(issue);

        Assert.Equal("issues", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Post()
    {
        var issue = new Issue { Key = "test" };
        var request = new CreateIssueRequest(issue);

        Assert.Equal(HttpMethod.Post, request.Method);
    }

    [Fact]
    public void Should_build_the_uri()
    {
        var issue = new Issue { Key = "test" };
        var request = new CreateIssueRequest(issue);

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/issues", uri.ToString());
    }
}
