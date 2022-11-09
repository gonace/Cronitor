using Cronitor.Commands;
using Cronitor.Requests;
using System;
using System.Threading.Tasks;

namespace Cronitor
{
    public class BaseClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public BaseClient(Uri baseUri)
        {
            _httpClient = new HttpClient(baseUri);
        }

        public BaseClient(Uri baseUri, string apiKey)
        {
            _httpClient = new HttpClient(baseUri, apiKey);
        }

        public BaseClient(Uri baseUri, string apiKey, bool useHttps)
        {
            _httpClient = new HttpClient(baseUri, apiKey, useHttps);
        }

        public BaseClient(HttpClient client)
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

        public TResponse Send<TResponse>(Request request)
        {
            return Task.Run(async () => await SendAsync<TResponse>(request)).Result;
        }

        public async Task<TResponse> SendAsync<TResponse>(Request request)
        {
            return await _httpClient.SendAsync<TResponse>(request);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}