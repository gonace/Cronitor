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
using System.Threading.Tasks;

namespace Cronitor.Internals
{
    internal class HttpClient : IDisposable
    {
        private static readonly HttpClientHandler SharedHandler = new HttpClientHandler();
        private static readonly TimeSpan TelemetryTimeout = TimeSpan.FromSeconds(30);
        
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

            await PerformRequestAsync(task);
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

            await PerformRequestAsync(task);
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

            return await PerformRequestAsync<TReturn>(task);
        }


        private async Task PerformRequestAsync(Task<HttpResponseMessage> request)
        {
            var response = await request;
            if (response.IsSuccessStatusCode)
                return;

            var details = Serializer.Deserialize<ApiError>(await response.Content.ReadAsStringAsync());
            throw new ApiException(details, response.StatusCode);
        }

        private async Task<TReturn> PerformRequestAsync<TReturn>(Task<HttpResponseMessage> request)
        {
            var response = await request;
            if (response.IsSuccessStatusCode)
                return Serializer.Deserialize<TReturn>(await response.Content.ReadAsStringAsync());

            var details = Serializer.Deserialize<ApiError>(await response.Content.ReadAsStringAsync());
            throw new ApiException(details, response.StatusCode);
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

            return httpClient;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
