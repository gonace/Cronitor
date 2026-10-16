using Cronitor.Abstractions;
using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Exceptions;
using Cronitor.Extensions;
using Cronitor.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cronitor.Internals;

internal class HttpClient : IDisposable
{
    /// <summary>
    /// Shared by every client so connections are reused. On .NET 8 and later, each connection is replaced after two
    /// minutes, as Microsoft recommends for a long-lived handler, so a change in Cronitor's DNS is picked up.
    /// </summary>
#if NET
    internal static readonly HttpMessageHandler SharedHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) };
#else
    internal static readonly HttpMessageHandler SharedHandler = new HttpClientHandler();
#endif
    private static readonly TimeSpan TelemetryTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// The Cronitor API version the models are written for. Without it, Cronitor answers with 2020-10-01.
    /// </summary>
    private const string ApiVersion = "2025-11-28";

    private readonly string _apiKey;
    private readonly Uri _apiUri;
    private readonly System.Net.Http.HttpClient _httpClient;

    internal HttpClient(Uri apiUri, string apiKey)
        : this(apiUri, apiKey, SharedHandler)
    {
    }

    /// <summary>
    /// Sends requests through the given <see cref="HttpMessageHandler"/>.
    /// Intended for tests, to replace the network.
    /// </summary>
    internal HttpClient(Uri apiUri, string apiKey, HttpMessageHandler handler)
    {
        _apiKey = apiKey;
        _apiUri = apiUri.AsHttps();
        _httpClient = CreateHttpClient(handler);
    }

    /// <summary>
    /// The longest a request may take in total, including retries.
    /// </summary>
    internal TimeSpan Timeout => _httpClient.Timeout;

    /// <summary>
    /// Creates the client for telemetry pings, which retries transient failures
    /// and gives up after 30s in total, as Microsoft.Extensions.Http.Resilience does.
    /// </summary>
    internal static HttpClient ForTelemetry(string apiKey) =>
        ForTelemetry(apiKey, SharedHandler);

    /// <summary>
    /// Creates the client for telemetry pings, sending through <paramref name="network"/>.
    /// Intended for tests, to replace the network.
    /// </summary>
    internal static HttpClient ForTelemetry(string apiKey, HttpMessageHandler network)
    {
        var client = new HttpClient(Urls.TelemetryBaseUrl, apiKey, new RetryingHttpMessageHandler(network));
        client._httpClient.Timeout = TelemetryTimeout;

        return client;
    }

    public async Task SendAsync(Command command)
    {
        var requestUri = command.ToUri();
        var message = new HttpRequestMessage
        {
            Method = command.Method,
            RequestUri = requestUri
        };
        var task = _httpClient.SendAsync(message);

        await PerformRequestAsync(task).ConfigureAwait(false);
    }

    public async Task SendAsync(BaseRequest request)
    {
        var requestUri = request.ToUri();
        var message = new HttpRequestMessage
        {
            Method = request.Method,
            RequestUri = requestUri,
            Content = request.Content
        };
        var task = _httpClient.SendAsync(message);

        await PerformRequestAsync(task).ConfigureAwait(false);
    }

    public async Task<TReturn> SendAsync<TReturn>(BaseRequest request)
    {
        var requestUri = request.ToUri();
        var message = new HttpRequestMessage
        {
            Method = request.Method,
            RequestUri = requestUri,
            Content = request.Content
        };
        var task = _httpClient.SendAsync(message);

        return await PerformRequestAsync<TReturn>(task).ConfigureAwait(false);
    }


    private async Task PerformRequestAsync(Task<HttpResponseMessage> request)
    {
        var response = await request.ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw await ErrorOfAsync(response).ConfigureAwait(false);
    }

    private async Task<TReturn> PerformRequestAsync<TReturn>(Task<HttpResponseMessage> request)
    {
        var response = await request.ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            return Serializer.Deserialize<TReturn>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        throw await ErrorOfAsync(response).ConfigureAwait(false);
    }

    private static async Task<ApiException> ErrorOfAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return new ApiException(DetailOf(body), response.StatusCode, body);
    }

    private static ApiError DetailOf(string body)
    {
        try
        {
            return Serializer.Deserialize<ApiError>(body) ?? new ApiError();
        }
        catch (JsonException)
        {
            return new ApiError();
        }
    }

    private System.Net.Http.HttpClient CreateHttpClient(HttpMessageHandler handler)
    {
        var httpClient = new System.Net.Http.HttpClient(handler, disposeHandler: false)
        {
            BaseAddress = _apiUri
        };

        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_apiKey}:")));
        httpClient.DefaultRequestHeaders.Add("Cronitor-Version", ApiVersion);

        return httpClient;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
