using System;
using System.Threading.Tasks;
using Cronitor.Constants;
using Cronitor.Requests;

namespace Cronitor
{
    public class BaseClient
    {
        protected string ApiKey;
        protected Uri BaseUri;
        protected bool UseHttps = true;

        public BaseClient(string apiKey)
        {
            ApiKey = apiKey;
        }

        public BaseClient(string apiKey, bool useHttps)
        {
            ApiKey = apiKey;
            UseHttps = useHttps;
        }


        public void Send(Command command)
        {
            Task.Run(async () => await SendAsync(command))
                .Wait();
        }

        public async Task SendAsync(Command command)
        {
            using (var client = new HttpClient(BaseUri, ApiKey, UseHttps))
            {
                await client.SendAsync(command);
            }
        }

        public TResponse Send<TResponse>(Request request)
        {
            return Task.Run(async () => await SendAsync<TResponse>(request)).Result;
        }

        public async Task<TResponse> SendAsync<TResponse>(Request request)
        {
            using (var client = new HttpClient(BaseUri, ApiKey, UseHttps))
            {
                return await client.SendAsync<TResponse>(request);
            }
        }
    }
}