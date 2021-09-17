using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cronitor.Constants;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Requests.Monitor;
using Cronitor.Responses.Monitor;

namespace Cronitor
{
    public class MonitorClient : BaseClient
    {
        public MonitorClient(string apiKey)
            : base(apiKey)
        {
            BaseUri = new Uri(Urls.ApiUrl);
        }

        public MonitorClient(string apiKey, bool useHttps)
            : base(apiKey, useHttps)
        {
            BaseUri = new Uri(Urls.ApiUrl);
        }

        public Pageable<Monitor> Find(int page = 1)
        {
            return Task.Run(async () => await FindAsync()).Result;
        }

        public async Task<Pageable<Monitor>> FindAsync(int page = 1)
        {
            var request = new FindRequest
            {
                Page = page
            }; 

            return await SendAsync<Pageable<Monitor>>(request);
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
            Task.Run(async () => await DeleteAsync(request))
                .Wait(); ;
        }

        public async Task DeleteAsync(DeleteRequest request)
        {
            await SendAsync<Task>(request);
        }

        public IEnumerable<Activity> Activities(string monitorKey)
        {
            return Task.Run(async () => await ActivitiesAsync(monitorKey)).Result;
        }

        public async Task<IEnumerable<Activity>> ActivitiesAsync(string monitorKey)
        {
            var request = new GetActivitiesRequest(monitorKey);

            return await SendAsync<IEnumerable<Activity>>(request);
        }

        public IEnumerable<Alert> Alerts(string monitorKey)
        {
            return Task.Run(async () => await AlertsAsync(monitorKey)).Result;
        }

        public async Task<IEnumerable<Alert>> AlertsAsync(string monitorKey)
        {
            var request = new GetAlertsRequest(monitorKey);
            var response = await SendAsync<Dictionary<string, IEnumerable<Alert>>>(request);

            return response.FirstOrDefault().Value;
        }

        public IEnumerable<Ping> Pings(string monitorKey)
        {
            return Task.Run(async () => await PingsAsync(monitorKey)).Result;
        }

        public async Task<IEnumerable<Ping>> PingsAsync(string monitorKey)
        {
            var request = new GetPingsRequest(monitorKey);
            var response = await SendAsync<Dictionary<string, IEnumerable<Ping>>>(request);

            return response.FirstOrDefault().Value;
        }
    }
}
