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

        public HttpClient(string apiUrl, string apiKey, bool useHttps = true)
        {
            var uri = new Uri(apiUrl);

            if (!useHttps)
                uri.AsHttp();

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

        public async Task SendAsync(Command command)
        {
            for (var i = 0; i < 8; i++)
            {
                var message = new HttpRequestMessage
                {
                    Method = command.Method,
                    //RequestUri = new Uri(requestUri),
                    Content = command.Content
                };

                await _httpClient.SendAsync(message);
            }
        }

        public async Task SendAsync(Request request)
        {
            var message = new HttpRequestMessage
            {
                Method = request.Method,
                RequestUri = new Uri(request.Endpoint),
                Content = request.Content
            };
            var task = _httpClient.SendAsync(message);

            await PerformRequestAsync(task);
        }

        public async Task<TReturn> SendAsync<TReturn>(Request request)
        {
            var message = new HttpRequestMessage
            {
                Method = request.Method,
                RequestUri = new Uri($"{Urls.PrimaryApiUrl}{request.ToUrl()}"),
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
