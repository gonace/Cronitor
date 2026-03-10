using Cronitor.Commands;
using Cronitor.Internals;
using System;
using System.Threading.Tasks;

namespace Cronitor.Abstractions
{
    public abstract class BaseClient<T> : IDisposable
    {
        private HttpClient _httpClient;

        protected BaseClient(Uri apiUri)
        {
            _httpClient = new HttpClient(apiUri);
        }

        protected BaseClient(Uri apiUri, string apiKey)
        {
            _httpClient = new HttpClient(apiUri, apiKey);
        }

        internal BaseClient(HttpClient client)
        {
            _httpClient = client;
        }


        protected void Send(Command command) =>
            SendAsync(command).GetAwaiter().GetResult();

        protected async Task SendAsync(Command command)
        {
            await _httpClient.SendAsync(command);
        }

        protected TResponse Send<TResponse>(BaseRequest request) =>
            SendAsync<TResponse>(request).GetAwaiter().GetResult();

        protected async Task<TResponse> SendAsync<TResponse>(BaseRequest request)
        {
            return await _httpClient.SendAsync<TResponse>(request);
        }

        protected void Send(BaseRequest request) =>
            SendAsync(request).GetAwaiter().GetResult();

        protected async Task SendAsync(BaseRequest request)
        {
            await _httpClient.SendAsync(request);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClient = null;
            GC.SuppressFinalize(this);
        }
    }
}