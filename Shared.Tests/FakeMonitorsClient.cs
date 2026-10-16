using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cronitor.Shared.Tests;

public class FakeMonitorsClient : IMonitorsClient
{
    private readonly TaskCompletionSource<bool> _updated = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

    public List<string> Updates { get; } = new List<string>();

    public Task Updated => _updated.Task;

    public Task UpdateGate { get; set; } = Task.CompletedTask;

    public async Task<IEnumerable<Monitor>> UpdateAsync(UpdateMonitorRequest request)
    {
        await UpdateGate;
        Updates.Add(await request.Content.ReadAsStringAsync());
        _updated.TrySetResult(true);
        return Enumerable.Empty<Monitor>();
    }

    public IEnumerable<Monitor> Update(UpdateMonitorRequest request) => throw new NotSupportedException();
    public ListMonitorResponse List(int page = 1) => throw new NotSupportedException();
    public Task<ListMonitorResponse> ListAsync(int page = 1) => throw new NotSupportedException();
    public ListMonitorResponse List(ListMonitorRequest request) => throw new NotSupportedException();
    public Task<ListMonitorResponse> ListAsync(ListMonitorRequest request) => throw new NotSupportedException();
    public Monitor Get(MonitorKey key) => throw new NotSupportedException();
    public Task<Monitor> GetAsync(MonitorKey key) => throw new NotSupportedException();
    public Monitor Clone(MonitorKey key, string name = null) => throw new NotSupportedException();
    public Task<Monitor> CloneAsync(MonitorKey key, string name = null) => throw new NotSupportedException();
    public IEnumerable<Monitor> Create(CreateMonitorRequest request) => throw new NotSupportedException();
    public Task<IEnumerable<Monitor>> CreateAsync(CreateMonitorRequest request) => throw new NotSupportedException();
    public void Delete(MonitorKey key) => throw new NotSupportedException();
    public Task DeleteAsync(MonitorKey key) => throw new NotSupportedException();
    public void Pause(MonitorKey key, int? hours = null) => throw new NotSupportedException();
    public Task PauseAsync(MonitorKey key, int? hours = null) => throw new NotSupportedException();
    public void Unpause(MonitorKey key) => throw new NotSupportedException();
    public Task UnpauseAsync(MonitorKey key) => throw new NotSupportedException();
    public IEnumerable<Activity> Activities(MonitorKey key) => throw new NotSupportedException();
    public Task<IEnumerable<Activity>> ActivitiesAsync(MonitorKey key) => throw new NotSupportedException();
    public IEnumerable<Alert> Alerts(MonitorKey key) => throw new NotSupportedException();
    public Task<IEnumerable<Alert>> AlertsAsync(MonitorKey key) => throw new NotSupportedException();
    public IEnumerable<Ping> Pings(MonitorKey key) => throw new NotSupportedException();
    public Task<IEnumerable<Ping>> PingsAsync(MonitorKey key) => throw new NotSupportedException();
}
