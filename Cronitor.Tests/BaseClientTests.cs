using Cronitor.Abstractions;
using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

// Concrete implementation of BaseClient for testing
internal class TestClient : BaseClient<TestClient>
{
    public TestClient(Uri apiUri, string apiKey) : base(apiUri, apiKey) { }
    internal TestClient(HttpClient client) : base(client) { }


    public void ExposedSend(Command command) =>
        SendAsync(command).GetAwaiter().GetResult();

    public async Task ExposedSendAsync(Command command)
    {
        await SendAsync(command);
    }

    public TResponse ExposedSend<TResponse>(BaseRequest request) =>
        SendAsync<TResponse>(request).GetAwaiter().GetResult();

    public async Task<TResponse> ExposedSendAsync<TResponse>(BaseRequest request)
    {
        return await SendAsync<TResponse>(request);
    }
}

public class BaseClientTests : BaseTest
{
    [Fact]
    public void ShouldConstructWithUriAndApiKey()
    {
        var client = new TestClient(Urls.DefaultApiUrl, ApiKey);

        Assert.NotNull(client);
    }

    [Fact]
    public void ShouldConstructWithHttpClient()
    {
        var httpClient = new HttpClient(Urls.TelemetryBaseUrl, ApiKey, new FakeHttpMessageHandler());
        var client = new TestClient(httpClient);

        Assert.NotNull(client);
    }

    [Fact]
    public async Task ShouldSendCommandAsync()
    {
        var handler = new FakeHttpMessageHandler();
        var command = new RunCommand()
            .WithApiKey(ApiKey)
            .WithMonitorKey(MonitorKey);
        var client = new TestClient(new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler));

        await client.ExposedSendAsync(command);

        var request = Assert.Single(handler.Requests);
        Assert.Equal(command.ToUri(), request.RequestUri);
    }

    [Fact]
    public void ShouldSendCommandSync()
    {
        var handler = new FakeHttpMessageHandler();
        var command = new RunCommand()
            .WithApiKey(ApiKey)
            .WithMonitorKey(MonitorKey);
        var client = new TestClient(new HttpClient(Urls.TelemetryBaseUrl, ApiKey, handler));

        client.ExposedSend(command);

        var request = Assert.Single(handler.Requests);
        Assert.Equal(command.ToUri(), request.RequestUri);
    }

    [Fact]
    public void ShouldDisposeWithoutError()
    {
        var client = new TestClient(Urls.DefaultApiUrl, ApiKey);

        client.Dispose();

        // Test passes if no exception is thrown
    }

    [Fact]
    public void ShouldDisposeUsingUsingStatement()
    {
        using (var client = new TestClient(Urls.DefaultApiUrl, ApiKey))
        {
            Assert.NotNull(client);
        }
        // Test passes if no exception is thrown during disposal
    }
}
