using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Tests.Helpers;

/// <summary>
/// Replaces the network: answers every request with a fixed response
/// and records the requests it receives.
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _body;
    private readonly List<HttpRequestMessage> _requests = new List<HttpRequestMessage>();

    public FakeHttpMessageHandler(HttpStatusCode statusCode = HttpStatusCode.OK, string body = "")
    {
        _statusCode = statusCode;
        _body = body;
    }

    public IReadOnlyList<HttpRequestMessage> Requests => _requests;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _requests.Add(request);

        var response = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_body, Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }
}
