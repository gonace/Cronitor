using Cronitor.Abstractions;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cronitor.Extensions;

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
            : base(Urls.DefaultApiUrl)
        {
        }

        public MonitorsClient(string apiKey)
            : base(Urls.DefaultApiUrl, apiKey)
        {
        }

        internal MonitorsClient(HttpClient client)
            : base(client)
        {
        }


        public ListMonitorResponse List(int page = 1) =>
            ListAsync(page).GetAwaiter().GetResult();

        public async Task<ListMonitorResponse> ListAsync(int page = 1)
        {
            var request = new ListMonitorRequest
            {
                Page = page
            };

            return await SendAsync<ListMonitorResponse>(request);
        }

        public Monitor Get(string key) =>
            GetAsync(key).GetAwaiter().GetResult();

        public async Task<Monitor> GetAsync(string key)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

            var request = new GetMonitorRequest(key);

            return await SendAsync<Monitor>(request);
        }

        public IEnumerable<Monitor> Create(CreateMonitorRequest request) =>
            CreateAsync(request).GetAwaiter().GetResult();

        public async Task<IEnumerable<Monitor>> CreateAsync(CreateMonitorRequest request)
        {
            var response = await SendAsync<CreateMonitorResponse>(request);

            return response?.Monitors;
        }

        public IEnumerable<Monitor> Update(UpdateMonitorRequest request) =>
            UpdateAsync(request).GetAwaiter().GetResult();

        public async Task<IEnumerable<Monitor>> UpdateAsync(UpdateMonitorRequest request)
        {
            var response = await SendAsync<UpdateMonitorResponse>(request);

            return response.Monitors;
        }

        public void Delete(string key) =>
            DeleteAsync(key).GetAwaiter().GetResult();

        public async Task DeleteAsync(string key)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

            var request = new DeleteMonitorRequest(key);

            await SendAsync(request);
        }

        public void Pause(string key, int? hours = null) =>
            PauseAsync(key, hours).GetAwaiter().GetResult();

        public async Task PauseAsync(string key, int? hours = null)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

            var request = new PauseMonitorRequest(key, hours);

            await SendAsync(request);
        }

        public void Unpause(string key) =>
            UnpauseAsync(key).GetAwaiter().GetResult();

        public async Task UnpauseAsync(string key)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

            var request = new UnpauseMonitorRequest(key);

            await SendAsync(request);
        }

        public IEnumerable<Activity> Activities(string key) =>
            ActivitiesAsync(key).GetAwaiter().GetResult();

        public async Task<IEnumerable<Activity>> ActivitiesAsync(string key)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

            var request = new ListActivitiesRequest(key);

            return await SendAsync<IEnumerable<Activity>>(request);
        }

        public IEnumerable<Alert> Alerts(string key) =>
            AlertsAsync(key).GetAwaiter().GetResult();

        public async Task<IEnumerable<Alert>> AlertsAsync(string key)
        {
            var request = new ListAlertsRequest(key);
            var response = await SendAsync<Dictionary<string, IEnumerable<Alert>>>(request);

            return response?.FirstOrDefault().Value;
        }

        public IEnumerable<Ping> Pings(string key) =>
            PingsAsync(key).GetAwaiter().GetResult();

        public async Task<IEnumerable<Ping>> PingsAsync(string key)
        {
            var request = new ListPingsRequest(key);
            var response = await SendAsync<Dictionary<string, IEnumerable<Ping>>>(request);

            return response?.FirstOrDefault().Value;
        }
    }
}
