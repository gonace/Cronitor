using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cronitor.Constants;
using Cronitor.Exceptions;
using Cronitor.Extensions;
using Cronitor.Requests;
using Newtonsoft.Json;

namespace Cronitor
{
    public class HttpClient : IDisposable
    {
        private readonly System.Net.Http.HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly Uri _apiUri;

        public HttpClient(string apiUrl, string apiKey, bool useHttps = true)
        {
            _apiKey = apiKey;

            var uri = new Uri(apiUrl);

            if (!useHttps)
                uri.AsHttp();

            _apiUri = uri;

            _httpClient = new System.Net.Http.HttpClient
            {
                BaseAddress = uri
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{apiKey}:")
                ));
        }

        //TODO: Send to FallbackUrl
        //TODO: Move Url parsing/building to command constant?
        public async Task SendAsync(Command command, string monitorKey)
        {
            var dictionary = new Dictionary<string, string>
            {
                { ":apiKey", _apiKey },
                { ":key", monitorKey },
                { ":command", command.ToString() }
            };

            var requestUri = _apiUri.Build(dictionary);

            var message = new HttpRequestMessage
            {
                Method = command.Method,
                RequestUri = requestUri,
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
            var task = _httpClient.SendAsync(message);

            await PerformRequestAsync(task);
        }

        //TODO: send to FallbackUrl
        public async Task SendAsync(Request request)
        {
            var requestUri = new Uri($"{_apiUri.Combine(request.ToUrl())}");

            var message = new HttpRequestMessage
            {
                Method = request.Method,
                RequestUri = requestUri,
                Content = request.Content
            };
            var task = _httpClient.SendAsync(message);

            await PerformRequestAsync(task);
        }

        //TODO: send to FallbackUrl
        public async Task<TReturn> SendAsync<TReturn>(Request request)
        {
            var requestUri = new Uri($"{_apiUri.Combine(request.ToUrl())}");

            var message = new HttpRequestMessage
            {
                Method = request.Method,
                RequestUri = requestUri,
                Content = request.Content
            };
            var task = _httpClient.SendAsync(message);

            return await PerformRequestAsync<TReturn>(task);
        }


        protected virtual async Task PerformRequestAsync(Task<HttpResponseMessage> request)
        {
            var response = await request;

            if (response.IsSuccessStatusCode)
                return;

            var details = response?.Content != null
                ? JsonConvert.DeserializeObject<Models.ApiException>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                : null;
            throw new ApiException(details, response.StatusCode);
        }

        protected virtual async Task<TReturn> PerformRequestAsync<TReturn>(Task<HttpResponseMessage> request)
        {
            var response = await request;

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<TReturn>(await response.Content.ReadAsStringAsync());

            var details = response?.Content != null
                ? JsonConvert.DeserializeObject<Models.ApiException>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                : null;
            throw new ApiException(details, response.StatusCode);
        }


        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
