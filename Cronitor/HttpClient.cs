using System;
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

        public HttpClient(Uri apiUrl, string apiKey, bool useHttps = true)
        {
            if (!useHttps)
                apiUrl.AsHttp();

            _httpClient = new System.Net.Http.HttpClient
            {
                BaseAddress = apiUrl
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{apiKey}:")
                ));
        }

        //TODO: send to FallbackUrl
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
        
        public async Task SendAsync(Request request)
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
        
        public async Task<TReturn> SendAsync<TReturn>(Request request)
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
