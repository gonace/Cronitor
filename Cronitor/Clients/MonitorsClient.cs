using Cronitor.Abstractions;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cronitor.Clients
{
    public interface IMonitorsClient
    {
        ListMonitorResponse List(int page = 1);
        Task<ListMonitorResponse> ListAsync(int page = 1);
        Monitor Get(string key);
        Task<Monitor> GetAsync(string key);
        IEnumerable<Monitor> Create(CreateMonitorRequest request);
        Task<IEnumerable<Monitor>> CreateAsync(CreateMonitorRequest request);
        IEnumerable<Monitor> Update(UpdateMonitorRequest request);
        Task<IEnumerable<Monitor>> UpdateAsync(UpdateMonitorRequest request);
        void Delete(string key);
        Task DeleteAsync(string key);
        void Pause(string key, int? hours = null);
        Task PauseAsync(string key, int? hours = null);
        void Unpause(string key);
        Task UnpauseAsync(string key);

        IEnumerable<Activity> Activities(string key);
        Task<IEnumerable<Activity>> ActivitiesAsync(string key);
        IEnumerable<Alert> Alerts(string key);
        Task<IEnumerable<Alert>> AlertsAsync(string key);
        IEnumerable<Ping> Pings(string key);
        Task<IEnumerable<Ping>> PingsAsync(string key);
    }

    public class MonitorsClient : BaseClient<MonitorsClient>, IMonitorsClient
    {
        public MonitorsClient()
            : base(Urls.ApiUrl)
        {
        }

        public MonitorsClient(string apiKey)
            : base(Urls.ApiUrl, apiKey)
        {
        }

        public MonitorsClient(string apiKey, bool useHttps)
            : base(Urls.ApiUrl, apiKey, useHttps)
        {
        }

        internal MonitorsClient(HttpClient client)
            : base(client)
        {
        }


        public ListMonitorResponse List(int page = 1) =>
            Task.Run(async () => await ListAsync(page)).Result;

        public async Task<ListMonitorResponse> ListAsync(int page = 1)
        {
            var request = new ListMonitorRequest
            {
                Page = page
            };

            return await SendAsync<ListMonitorResponse>(request);
        }

        public Monitor Get(string key) =>
            Task.Run(async () => await GetAsync(key)).Result;

        public async Task<Monitor> GetAsync(string key)
        {
            var request = new GetMonitorRequest(key);

            return await SendAsync<Monitor>(request);
        }

        public IEnumerable<Monitor> Create(CreateMonitorRequest request) =>
            Task.Run(async () => await CreateAsync(request)).Result;

        public async Task<IEnumerable<Monitor>> CreateAsync(CreateMonitorRequest request)
        {
            var response = await SendAsync<CreateMonitorResponse>(request);

            return response?.Monitors;
        }

        public IEnumerable<Monitor> Update(UpdateMonitorRequest request) =>
            Task.Run(async () => await UpdateAsync(request)).Result;

        public async Task<IEnumerable<Monitor>> UpdateAsync(UpdateMonitorRequest request)
        {
            var response = await SendAsync<UpdateMonitorResponse>(request);

            return response.Monitors;
        }

        public void Delete(string key) =>
            Task.Run(async () => await DeleteAsync(key))
                .Wait();

        public async Task DeleteAsync(string key)
        {
            var request = new DeleteMonitorRequest(key);

            await SendAsync<Task>(request);
        }

        public void Pause(string key, int? hours = null) =>
            Task.Run(async () => await PauseAsync(key, hours))
                .Wait();

        public async Task PauseAsync(string key, int? hours = null)
        {
            var request = hours != null ? new PauseMonitorRequest(key, hours.Value) : new PauseMonitorRequest(key);

            await SendAsync<Task>(request);
        }

        public void Unpause(string key) =>
            Task.Run(async () => await UnpauseAsync(key))
                .Wait();

        public async Task UnpauseAsync(string key)
        {
            var request = new UnpauseMonitorRequest(key);

            await SendAsync<Task>(request);
        }

        public IEnumerable<Activity> Activities(string key) =>
            Task.Run(async () => await ActivitiesAsync(key)).Result;

        public async Task<IEnumerable<Activity>> ActivitiesAsync(string key)
        {
            var request = new ListActivitiesRequest(key);

            return await SendAsync<IEnumerable<Activity>>(request);
        }

        public IEnumerable<Alert> Alerts(string key) =>
            Task.Run(async () => await AlertsAsync(key)).Result;

        public async Task<IEnumerable<Alert>> AlertsAsync(string key)
        {
            var request = new ListAlertsRequest(key);
            var response = await SendAsync<Dictionary<string, IEnumerable<Alert>>>(request);

            return response.FirstOrDefault().Value;
        }

        public IEnumerable<Ping> Pings(string key) =>
            Task.Run(async () => await PingsAsync(key)).Result;

        public async Task<IEnumerable<Ping>> PingsAsync(string key)
        {
            var request = new ListPingsRequest(key);
            var response = await SendAsync<Dictionary<string, IEnumerable<Ping>>>(request);

            return response.FirstOrDefault().Value;
        }
    }
}
