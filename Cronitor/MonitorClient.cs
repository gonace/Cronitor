using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Requests.Monitor;
using Cronitor.Responses.Monitor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cronitor
{
    public class MonitorClient : BaseClient
    {
        public MonitorClient()
            : base(Urls.ApiUrl)
        {
        }

        public MonitorClient(string apiKey)
            : base(Urls.ApiUrl, apiKey)
        {
        }

        public MonitorClient(string apiKey, bool useHttps)
            : base(Urls.ApiUrl, apiKey, useHttps)
        {
        }

        internal MonitorClient(HttpClient client)
            : base(client)
        {
        }


        public Pageable<Monitor> Find(int page = 1) =>
            Task.Run(async () => await FindAsync()).Result;

        public async Task<Pageable<Monitor>> FindAsync(int page = 1)
        {
            var request = new FindRequest
            {
                Page = page
            };

            return await SendAsync<Pageable<Monitor>>(request);
        }

        public Monitor Get(string key) =>
            Task.Run(async () => await GetAsync(key)).Result;

        public async Task<Monitor> GetAsync(string key)
        {
            var request = new GetRequest(key);

            return await SendAsync<Monitor>(request);
        }

        public IEnumerable<Monitor> Create(CreateRequest request) =>
            Task.Run(async () => await CreateAsync(request)).Result;

        public async Task<IEnumerable<Monitor>> CreateAsync(CreateRequest request)
        {
            var response = await SendAsync<CreateResponse>(request);

            return response?.Monitors;
        }

        public IEnumerable<Monitor> Update(UpdateRequest request) =>
            Task.Run(async () => await UpdateAsync(request)).Result;

        public async Task<IEnumerable<Monitor>> UpdateAsync(UpdateRequest request)
        {
            var response = await SendAsync<UpdateResponse>(request);

            return response.Monitors;
        }

        public void Delete(string key) =>
            Task.Run(async () => await DeleteAsync(key))
                .Wait();

        public async Task DeleteAsync(string key)
        {
            var request = new DeleteRequest(key);

            await SendAsync<Task>(request);
        }

        public void Pause(string key, int? hours = null) =>
            Task.Run(async () => await PauseAsync(key, hours))
                .Wait();

        public async Task PauseAsync(string key, int? hours = null)
        {
            var request = hours != null ? new PauseRequest(key, hours.Value) : new PauseRequest(key);

            await SendAsync<Task>(request);
        }

        public void Unpause(string key) =>
            Task.Run(async () => await UnpauseAsync(key))
                .Wait();

        public async Task UnpauseAsync(string key)
        {
            var request = new UnpauseRequest(key);

            await SendAsync<Task>(request);
        }

        public IEnumerable<Activity> Activities(string key) =>
            Task.Run(async () => await ActivitiesAsync(key)).Result;

        public async Task<IEnumerable<Activity>> ActivitiesAsync(string key)
        {
            var request = new GetActivitiesRequest(key);

            return await SendAsync<IEnumerable<Activity>>(request);
        }

        public IEnumerable<Alert> Alerts(string key) =>
            Task.Run(async () => await AlertsAsync(key)).Result;

        public async Task<IEnumerable<Alert>> AlertsAsync(string key)
        {
            var request = new GetAlertsRequest(key);
            var response = await SendAsync<Dictionary<string, IEnumerable<Alert>>>(request);

            return response.FirstOrDefault().Value;
        }

        public IEnumerable<Ping> Pings(string key) =>
            Task.Run(async () => await PingsAsync(key)).Result;

        public async Task<IEnumerable<Ping>> PingsAsync(string key)
        {
            var request = new GetPingsRequest(key);
            var response = await SendAsync<Dictionary<string, IEnumerable<Ping>>>(request);

            return response.FirstOrDefault().Value;
        }
    }
}
