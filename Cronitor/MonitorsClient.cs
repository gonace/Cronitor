using Cronitor.Abstractions;
using Cronitor.Constants;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cronitor.Extensions;

namespace Cronitor;

public interface IMonitorsClient
{
    ListMonitorResponse List(int page = 1);
    Task<ListMonitorResponse> ListAsync(int page = 1);
    /// <summary>
    /// Lists monitors matching the filters on <paramref name="request"/>.
    /// </summary>
    ListMonitorResponse List(ListMonitorRequest request);
    /// <inheritdoc cref="List(ListMonitorRequest)"/>
    Task<ListMonitorResponse> ListAsync(ListMonitorRequest request);
    Monitor Get(MonitorKey key);
    Task<Monitor> GetAsync(MonitorKey key);
    Monitor Clone(MonitorKey key, string name = null);
    Task<Monitor> CloneAsync(MonitorKey key, string name = null);
    IEnumerable<Monitor> Create(CreateMonitorRequest request);
    Task<IEnumerable<Monitor>> CreateAsync(CreateMonitorRequest request);
    IEnumerable<Monitor> Update(UpdateMonitorRequest request);
    Task<IEnumerable<Monitor>> UpdateAsync(UpdateMonitorRequest request);
    void Delete(MonitorKey key);
    Task DeleteAsync(MonitorKey key);
    void Pause(MonitorKey key, int? hours = null);
    Task PauseAsync(MonitorKey key, int? hours = null);
    void Unpause(MonitorKey key);
    Task UnpauseAsync(MonitorKey key);

    IEnumerable<Activity> Activities(MonitorKey key);
    Task<IEnumerable<Activity>> ActivitiesAsync(MonitorKey key);
    IEnumerable<Alert> Alerts(MonitorKey key);
    Task<IEnumerable<Alert>> AlertsAsync(MonitorKey key);
    IEnumerable<Ping> Pings(MonitorKey key);
    Task<IEnumerable<Ping>> PingsAsync(MonitorKey key);
}

public class MonitorsClient : BaseClient<MonitorsClient>, IMonitorsClient
{
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

    public Task<ListMonitorResponse> ListAsync(int page = 1) =>
        ListAsync(new ListMonitorRequest { Page = page });

    public ListMonitorResponse List(ListMonitorRequest request) =>
        ListAsync(request).GetAwaiter().GetResult();

    public async Task<ListMonitorResponse> ListAsync(ListMonitorRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await SendAsync<ListMonitorResponse>(request).ConfigureAwait(false);
    }

    public Monitor Get(MonitorKey key) =>
        GetAsync(key).GetAwaiter().GetResult();

    public async Task<Monitor> GetAsync(MonitorKey key)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

        var request = new GetMonitorRequest(key);

        return await SendAsync<Monitor>(request).ConfigureAwait(false);
    }

    public IEnumerable<Monitor> Create(CreateMonitorRequest request) =>
        CreateAsync(request).GetAwaiter().GetResult();

    public async Task<IEnumerable<Monitor>> CreateAsync(CreateMonitorRequest request)
    {
        var response = await SendAsync<CreateMonitorResponse>(request).ConfigureAwait(false);

        return response?.Monitors;
    }

    public Monitor Clone(MonitorKey key, string name = null) =>
        CloneAsync(key, name).GetAwaiter().GetResult();

    public async Task<Monitor> CloneAsync(MonitorKey key, string name = null)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

        var request = new CloneMonitorRequest(key, name);

        return await SendAsync<Monitor>(request).ConfigureAwait(false);
    }

    public IEnumerable<Monitor> Update(UpdateMonitorRequest request) =>
        UpdateAsync(request).GetAwaiter().GetResult();

    public async Task<IEnumerable<Monitor>> UpdateAsync(UpdateMonitorRequest request)
    {
        var response = await SendAsync<UpdateMonitorResponse>(request).ConfigureAwait(false);

        return response.Monitors;
    }

    public void Delete(MonitorKey key) =>
        DeleteAsync(key).GetAwaiter().GetResult();

    public async Task DeleteAsync(MonitorKey key)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

        var request = new DeleteMonitorRequest(key);

        await SendAsync(request).ConfigureAwait(false);
    }

    public void Pause(MonitorKey key, int? hours = null) =>
        PauseAsync(key, hours).GetAwaiter().GetResult();

    public async Task PauseAsync(MonitorKey key, int? hours = null)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

        var request = new PauseMonitorRequest(key, hours);

        await SendAsync(request).ConfigureAwait(false);
    }

    public void Unpause(MonitorKey key) =>
        UnpauseAsync(key).GetAwaiter().GetResult();

    public async Task UnpauseAsync(MonitorKey key)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

        var request = new UnpauseMonitorRequest(key);

        await SendAsync(request).ConfigureAwait(false);
    }

    public IEnumerable<Activity> Activities(MonitorKey key) =>
        ActivitiesAsync(key).GetAwaiter().GetResult();

    public async Task<IEnumerable<Activity>> ActivitiesAsync(MonitorKey key)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(key);

        var request = new ListActivitiesRequest(key);

        return await SendAsync<IEnumerable<Activity>>(request).ConfigureAwait(false);
    }

    public IEnumerable<Alert> Alerts(MonitorKey key) =>
        AlertsAsync(key).GetAwaiter().GetResult();

    public async Task<IEnumerable<Alert>> AlertsAsync(MonitorKey key)
    {
        var request = new ListAlertsRequest(key);

        return await SendAsync<IEnumerable<Alert>>(request).ConfigureAwait(false);
    }

    public IEnumerable<Ping> Pings(MonitorKey key) =>
        PingsAsync(key).GetAwaiter().GetResult();

    public async Task<IEnumerable<Ping>> PingsAsync(MonitorKey key)
    {
        var request = new ListPingsRequest(key);
        var response = await SendAsync<Dictionary<string, IEnumerable<Ping>>>(request).ConfigureAwait(false);

        return response?.FirstOrDefault().Value;
    }
}
