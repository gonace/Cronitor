using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Exceptions;
using Cronitor.Tests.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using HttpClient = Cronitor.Internals.HttpClient;

namespace Cronitor.Tests;

public class HttpClientTests : BaseTest
{
    [Fact]
    public void Should_construct_with_a_uri_and_an_api_key()
    {
        var httpClient = new HttpClient(Urls.DefaultApiUrl, ApiKey);

        Assert.NotNull(httpClient);
    }

    [Fact]
    public void Should_convert_http_to_https_when_constructed_with_an_api_key()
    {
        var httpUri = new Uri("http://cronitor.io/api/");
        var httpClient = new HttpClient(httpUri, ApiKey);
        var apiUriField = typeof(HttpClient).GetField("_apiUri", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var actualUri = (Uri)apiUriField.GetValue(httpClient);

        Assert.NotNull(actualUri);
        Assert.Equal("https", actualUri.Scheme);
        Assert.Equal("cronitor.io", actualUri.Host);
    }

    [Fact]
    public void Should_limit_telemetry_requests_to_thirty_seconds_in_total()
    {
        var httpClient = HttpClient.ForTelemetry(ApiKey);

        Assert.Equal(TimeSpan.FromSeconds(30), httpClient.Timeout);
    }

    [Fact]
    public async Task Should_send_a_command_to_its_uri()
    {
        var handler = new FakeHttpMessageHandler();
        var httpClient = new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler);
        var command = new RunCommand()
            .WithApiKey(ApiKey)
            .WithMonitorKey(MonitorKey);

        await httpClient.SendAsync(command);

        var request = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Equal($"https://cronitor.link/p/{ApiKey}/{MonitorKey}/run", request.RequestUri.ToString());
    }

    [Fact]
    public async Task Should_authenticate_with_the_api_key()
    {
        var handler = new FakeHttpMessageHandler();
        var httpClient = new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler);
        var command = new RunCommand()
            .WithApiKey(ApiKey)
            .WithMonitorKey(MonitorKey);

        await httpClient.SendAsync(command);

        var authorization = Assert.Single(handler.Requests).Headers.Authorization;
        Assert.Equal("Basic", authorization.Scheme);
        Assert.Equal(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ApiKey}:")), authorization.Parameter);
    }

    [Fact]
    public async Task Should_throw_ApiException_when_the_response_is_not_successful()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.NotFound, "{\"detail\":\"Monitor not found\"}");
        var httpClient = new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler);
        var command = new RunCommand()
            .WithApiKey(ApiKey)
            .WithMonitorKey(MonitorKey);

        var exception = await Assert.ThrowsAsync<ApiException>(() => httpClient.SendAsync(command));

        Assert.Equal("Monitor not found (NotFound)", exception.Message);
    }
}
