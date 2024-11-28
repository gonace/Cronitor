using Cronitor.Commands;
using Cronitor.Internals;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cronitor.Abstractions
{
    public abstract class BaseClient<T> : IDisposable
    {
        private HttpClient _httpClient;

        protected BaseClient(Uri apiUri)
        {
            _httpClient = new HttpClient(apiUri, _jsonSerializerOptions);
        }

        protected BaseClient(Uri apiUri, string apiKey)
        {
            _httpClient = new HttpClient(apiUri, apiKey, _jsonSerializerOptions);
        }

        protected BaseClient(Uri apiUri, string apiKey, JsonSerializerOptions serializerOptions)
        {
            _httpClient = new HttpClient(apiUri, apiKey, serializerOptions);
        }

        internal BaseClient(HttpClient client)
        {
            _httpClient = client;
        }


        public void Send(Command command)
        {
            Task.Run(async () => await SendAsync(command))
                .Wait();
        }

        public async Task SendAsync(Command command)
        {
            await _httpClient.SendAsync(command);
        }

        public TResponse Send<TResponse>(BaseRequest request)
        {
            return Task.Run(async () => await SendAsync<TResponse>(request)).Result;
        }

        public async Task<TResponse> SendAsync<TResponse>(BaseRequest request)
        {
            return await _httpClient.SendAsync<TResponse>(request);
        }

        public void Dispose()
        {
            _httpClient = null;
        }

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
    }
}