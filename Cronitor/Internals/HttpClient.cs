﻿using Cronitor.Abstractions;
using Cronitor.Commands;
using Cronitor.Exceptions;
using Cronitor.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cronitor.Internals
{
    internal class HttpClient : IDisposable
    {
        private readonly string _apiKey;
        private readonly Uri _apiUri;

        internal HttpClient(Uri apiUri)
        {
            _apiUri = apiUri;
        }

        internal HttpClient(Uri apiUri, string apiKey, bool useHttps = true)
        {
            _apiKey = apiKey;
            _apiUri = useHttps ? apiUri.AsHttps() : apiUri.AsHttp();
        }

        internal HttpClient()
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

        public virtual async Task SendAsync(BaseRequest request)
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

        public virtual async Task<TReturn> SendAsync<TReturn>(BaseRequest request)
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
                ? JsonSerializer.Deserialize<Models.ApiException>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    })
                : null;
            throw new ApiException(details, response.StatusCode);
        }

        protected virtual async Task<TReturn> PerformRequestAsync<TReturn>(Task<HttpResponseMessage> request)
        {
            var response = await request;

            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<TReturn>(await response.Content.ReadAsStringAsync());

            var details = response?.Content != null
                ? JsonSerializer.Deserialize<Models.ApiException>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
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
