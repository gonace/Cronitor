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
    public Monitor Get(string key) => throw new NotSupportedException();
    public Task<Monitor> GetAsync(string key) => throw new NotSupportedException();
    public Monitor Clone(string key, string name = null) => throw new NotSupportedException();
    public Task<Monitor> CloneAsync(string key, string name = null) => throw new NotSupportedException();
    public IEnumerable<Monitor> Create(CreateMonitorRequest request) => throw new NotSupportedException();
    public Task<IEnumerable<Monitor>> CreateAsync(CreateMonitorRequest request) => throw new NotSupportedException();
    public void Delete(string key) => throw new NotSupportedException();
    public Task DeleteAsync(string key) => throw new NotSupportedException();
    public void Pause(string key, int? hours = null) => throw new NotSupportedException();
    public Task PauseAsync(string key, int? hours = null) => throw new NotSupportedException();
    public void Unpause(string key) => throw new NotSupportedException();
    public Task UnpauseAsync(string key) => throw new NotSupportedException();
    public IEnumerable<Activity> Activities(string key) => throw new NotSupportedException();
    public Task<IEnumerable<Activity>> ActivitiesAsync(string key) => throw new NotSupportedException();
    public IEnumerable<Alert> Alerts(string key) => throw new NotSupportedException();
    public Task<IEnumerable<Alert>> AlertsAsync(string key) => throw new NotSupportedException();
    public IEnumerable<Ping> Pings(string key) => throw new NotSupportedException();
    public Task<IEnumerable<Ping>> PingsAsync(string key) => throw new NotSupportedException();
}
