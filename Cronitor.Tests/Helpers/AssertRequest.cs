using System.Net.Http;
using Xunit;

namespace Cronitor.Tests.Helpers;

public static class AssertRequest
{
    /// <summary>
    /// Asserts that <paramref name="handler"/> received exactly one request, with the given method and URI.
    /// </summary>
    public static void Sent(FakeHttpMessageHandler handler, HttpMethod method, string uri)
    {
        var request = Assert.Single(handler.Requests);
        Assert.Equal(method, request.Method);
        Assert.Equal(uri, request.RequestUri.ToString());
    }
}
