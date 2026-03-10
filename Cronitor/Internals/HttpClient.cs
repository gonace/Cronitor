using Cronitor.Abstractions;
using Cronitor.Commands;
using Cronitor.Exceptions;
using Cronitor.Extensions;
using Cronitor.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cronitor.Serialization;

namespace Cronitor.Internals
{
    internal class HttpClient : IDisposable
    {
        private static readonly HttpClientHandler SharedHandler = new HttpClientHandler();
        
        private readonly string _apiKey;
        private readonly Uri _apiUri;
        private readonly System.Net.Http.HttpClient _httpClient;

        internal HttpClient()
        {
            _httpClient = CreateHttpClient();
        }

        internal HttpClient(Uri apiUri)
        {
            _apiUri = apiUri;
            _httpClient = CreateHttpClient();
        }

        internal HttpClient(Uri apiUri, string apiKey)
        {
            _apiKey = apiKey;
            _apiUri = apiUri.AsHttps();
            _httpClient = CreateHttpClient();
        }

        //TODO: send to FallbackUrl
        public virtual async Task SendAsync(Command command)
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

        public virtual async Task SendAsync(BaseRequest request)
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

        public virtual async Task<TReturn> SendAsync<TReturn>(BaseRequest request)
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

        private System.Net.Http.HttpClient CreateHttpClient()
        {
            var httpClient = new System.Net.Http.HttpClient(SharedHandler, disposeHandler: false)
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
