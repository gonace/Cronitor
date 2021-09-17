using System.Collections.Generic;
using System.Threading.Tasks;
using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Requests.Monitor;
using Cronitor.Responses.Monitor;

namespace Cronitor
{
    public class MonitorClient
    {
        private readonly string _apiKey;
        private readonly bool _useHttps = true;

        public MonitorClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public MonitorClient(string apiKey, bool useHttps)
        {
            _apiKey = apiKey;
            _useHttps = useHttps;
        }


        public Pageable<Monitor> Find(FindRequest request)
        {
            return Task.Run(async () => await FindAsync(request)).Result;
        }

        public async Task<Pageable<Monitor>> FindAsync(FindRequest request)
        {
            return await SendAsync<Pageable<Monitor>>(request);
        }
         
        public Monitor Get(string monitorKey)
        {
            return Task.Run(async () => await GetAsync(monitorKey)).Result;
        }

        public async Task<Monitor> GetAsync(string monitorKey)
        {
            var request = new GetRequest(monitorKey);

            return await SendAsync<Monitor>(request);
        }

        public IEnumerable<Monitor> Create(CreateRequest request)
        {
            return Task.Run(async () => await CreateAsync(request)).Result;
        }

        public async Task<IEnumerable<Monitor>> CreateAsync(CreateRequest request)
        {
            var response = await SendAsync<CreateResponse>(request);

            return response.Monitors;
        }

        public IEnumerable<Monitor> Update(UpdateRequest request)
        {
            return Task.Run(async () => await UpdateAsync(request)).Result;
        }

        public async Task<IEnumerable<Monitor>> UpdateAsync(UpdateRequest request)
        {
            var response = await SendAsync<UpdateResponse>(request);

            return response.Monitors;
        }

        public void Delete(DeleteRequest request)
        {
            Task.Run(async () => await DeleteAsync(request));
        }

        public async Task DeleteAsync(DeleteRequest request)
        {
            await SendAsync<Task>(request);
        }
        
        public TResponse Send<TResponse>(Requests.Request request)
        {
            return Task.Run(async () => await SendAsync<TResponse>(request)).Result;
        }

        public async Task<TResponse> SendAsync<TResponse>(Requests.Request request)
        {
            using (var client = new HttpClient(Urls.PrimaryApiUrl, _apiKey, _useHttps))
            {
                return await client.SendAsync<TResponse>(request);
            }
        }
    }
}
