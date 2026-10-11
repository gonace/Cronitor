using Cronitor.Abstractions;
using Cronitor.Constants;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Cronitor.Tests;

// Concrete implementation for testing BaseRequest
internal class TestRequest : BaseRequest
{
    public override string Endpoint { get; set; } = "test-endpoint";
}

public class BaseRequestTests
{
    [Fact]
    public void Should_set_Content_with_SetContent()
    {
        var request = new TestRequest();
        var content = new StringContent("test content", Encoding.UTF8, "application/json");

        request.SetContent(content);

        Assert.NotNull(request.Content);
        Assert.Same(content, request.Content);
    }

    [Fact]
    public void Should_return_itself_from_SetContent()
    {
        var request = new TestRequest();
        var content = new StringContent("test content", Encoding.UTF8, "application/json");

        var result = request.SetContent(content);

        Assert.NotNull(result);
        Assert.Same(request, result);
    }

    [Fact]
    public void Should_chain_SetContent()
    {
        var request = new TestRequest();
        var content = new StringContent("test content", Encoding.UTF8, "application/json");

        var result = request.SetContent(content).SetContent(content);

        Assert.NotNull(result);
        Assert.Same(request, result);
    }

    [Fact]
    public void Should_combine_the_base_url_and_endpoint_in_ToUri()
    {
        var request = new TestRequest();

        var uri = request.ToUri();

        Assert.NotNull(uri);
        Assert.Contains("test-endpoint", uri.ToString());
        Assert.StartsWith(Urls.DefaultApiUrl.ToString(), uri.ToString());
    }

    [Fact]
    public void Should_return_the_uri_as_a_string_from_ToUrl()
    {
        var request = new TestRequest();

        var url = request.ToUrl();

        Assert.NotNull(url);
        Assert.IsType<string>(url);
        Assert.Contains("test-endpoint", url);
        Assert.StartsWith(Urls.DefaultApiUrl.ToString(), url);
    }

    [Fact]
    public void Should_return_the_same_from_ToUrl_as_from_ToUri()
    {
        var request = new TestRequest();

        var url = request.ToUrl();
        var uriString = request.ToUri().ToString();

        Assert.Equal(uriString, url);
    }

    [Fact]
    public void Should_default_Method_to_Get()
    {
        var request = new TestRequest();

        Assert.Equal(HttpMethod.Get, request.Method);
    }

    [Fact]
    public void Should_allow_setting_Method()
    {
        var request = new TestRequest
        {
            Method = HttpMethod.Post
        };

        Assert.Equal(HttpMethod.Post, request.Method);
    }

    [Fact]
    public void Should_allow_setting_Endpoint()
    {
        var request = new TestRequest
        {
            Endpoint = "custom-endpoint"
        };

        Assert.Equal("custom-endpoint", request.Endpoint);
    }

    [Fact]
    public void Should_default_Content_to_null()
    {
        var request = new TestRequest();

        Assert.Null(request.Content);
    }

    [Fact]
    public void Should_allow_setting_Content_directly()
    {
        var request = new TestRequest();
        var content = new StringContent("test", Encoding.UTF8, "application/json");

        request.Content = content;

        Assert.NotNull(request.Content);
        Assert.Same(content, request.Content);
    }
}
