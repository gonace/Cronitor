using System.Net.Http;
using Cronitor.Requests;
using Xunit;

namespace Cronitor.Tests.Requests;

public class GetIssueRequestTests
{
    [Fact]
    public void Should_set_Endpoint()
    {
        var request = new GetIssueRequest("issue-001");

        Assert.Equal("issues/:key", request.Endpoint);
    }

    [Fact]
    public void Should_set_Method_to_Get()
    {
        var request = new GetIssueRequest("issue-001");

        Assert.Equal(HttpMethod.Get, request.Method);
    }

    [Fact]
    public void Should_set_Key()
    {
        var request = new GetIssueRequest("issue-001");

        Assert.Equal("issue-001", request.Key);
    }

    [Fact]
    public void Should_build_the_uri_with_the_key()
    {
        var request = new GetIssueRequest("issue-001");

        var uri = request.ToUri();

        Assert.Equal("https://cronitor.io/api/issues/issue-001", uri.ToString());
    }
}
