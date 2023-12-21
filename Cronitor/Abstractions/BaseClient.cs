using Cronitor.Commands;
using Cronitor.Internals;
using System;
using System.Threading.Tasks;

namespace Cronitor.Abstractions
{
    public abstract class BaseClient<T> : IDisposable
    {
        private readonly HttpClient _httpClient;

        protected BaseClient(Uri baseUri)
        {
            _httpClient = new HttpClient(baseUri);
        }

        protected BaseClient(Uri baseUri, string apiKey)
        {
            _httpClient = new HttpClient(baseUri, apiKey);
        }

        protected BaseClient(Uri baseUri, string apiKey, bool useHttps)
        {
            _httpClient = new HttpClient(baseUri, apiKey, useHttps);
        }

        private protected BaseClient(HttpClient client)
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
            _httpClient?.Dispose();
        }
    }
}