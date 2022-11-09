using Cronitor.Commands;
using Cronitor.Exceptions;
using Cronitor.Extensions;
using Cronitor.Requests;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cronitor
{
    public class HttpClient : IDisposable
    {
        private readonly string _apiKey;
        private readonly Uri _apiUri;

        public HttpClient(Uri apiUri)
        {
            _apiUri = apiUri;
        }

        public HttpClient(Uri apiUri, string apiKey, bool useHttps = true)
        {
            _apiKey = apiKey;
            _apiUri = useHttps ? apiUri.AsHttps() : apiUri.AsHttp();
        }

        protected HttpClient()
        {
        }

        //TODO: send to FallbackUrl
        public virtual async Task SendAsync(Command command)
        {
            using (var httpClient = GetHttpClient())
            {
                var requestUri = command.ToUri();

                var message = new HttpRequestMessage
                {
                    Method = command.Method,
                    RequestUri = requestUri
                };
                var task = httpClient.SendAsync(message);

                await PerformRequestAsync(task);
            }
        }

        public virtual async Task SendAsync(Request request)
        {
            using (var httpClient = GetHttpClient())
            {
                var requestUri = request.ToUri();

                var message = new HttpRequestMessage
                {
                    Method = request.Method,
                    RequestUri = requestUri,
                    Content = request.Content
                };
                var task = httpClient.SendAsync(message);

                await PerformRequestAsync(task);
            }
        }

        public virtual async Task<TReturn> SendAsync<TReturn>(Request request)
        {
            using (var httpClient = GetHttpClient())
            {
                var requestUri = request.ToUri();

                var message = new HttpRequestMessage
                {
                    Method = request.Method,
                    RequestUri = requestUri,
                    Content = request.Content
                };
                var task = httpClient.SendAsync(message);

                return await PerformRequestAsync<TReturn>(task);
            }
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

        private System.Net.Http.HttpClient GetHttpClient()
        {
            var httpClient = new System.Net.Http.HttpClient
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
            //TODO What can we do here?
        }
    }
}
