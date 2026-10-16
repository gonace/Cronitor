using Cronitor.Exceptions;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Scheduling;
using Cronitor.Tests.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

public class ScheduleSyncTests
{
    private const string Rejected = """{"bad-job":{"schedules":["Invalid schedule expression: not a cron"]}}""";

    private readonly FakeLoggerFactory _logs = new FakeLoggerFactory();

    [Fact]
    public async Task Should_send_each_monitor_as_a_job_with_only_its_schedules_and_timezone()
    {
        var monitors = new FakeMonitorsClient();

        await Sync(monitors, new MonitorSchedule("nightly-backup", new[] { "0 3 * * *" }, "Europe/Berlin"), new MonitorSchedule("cleanup", new[] { "every 5 minutes" }));

        var body = Assert.Single(await monitors.Bodies());
        Assert.Equal("""{"monitors":[{"key":"nightly-backup","schedules":["0 3 * * *"],"timezone":"Europe/Berlin","type":"job"},{"key":"cleanup","schedules":["every 5 minutes"],"type":"job"}]}""", body);
        Assert.Contains(_logs.Entries, entry => entry.Level == LogLevel.Information && entry.Message.Contains("2 monitors"));
    }

    [Fact]
    public async Task Should_send_nothing_when_there_are_no_schedules()
    {
        var monitors = new FakeMonitorsClient();

        await Sync(monitors);

        Assert.Empty(monitors.Requests);
    }

    [Fact]
    public async Task Should_send_the_others_once_more_without_a_monitor_Cronitor_rejects()
    {
        var monitors = new FakeMonitorsClient(Fails(HttpStatusCode.BadRequest, Rejected), Succeeds());

        await Sync(monitors, new MonitorSchedule("nightly-backup", new[] { "0 3 * * *" }), new MonitorSchedule("bad-job", new[] { "not a cron" }));

        var bodies = await monitors.Bodies();
        Assert.Equal(2, bodies.Count);
        Assert.Contains("\"bad-job\"", bodies[0]);
        Assert.Equal("""{"monitors":[{"key":"nightly-backup","schedules":["0 3 * * *"],"type":"job"}]}""", bodies[1]);
        var warning = Assert.Single(_logs.Entries, entry => entry.Level == LogLevel.Warning);
        Assert.Contains("bad-job", warning.Message);
        Assert.Contains("Invalid schedule expression", warning.Message);
    }

    [Fact]
    public async Task Should_not_send_again_when_Cronitor_rejects_every_monitor()
    {
        var monitors = new FakeMonitorsClient(Fails(HttpStatusCode.BadRequest, Rejected));

        await Sync(monitors, new MonitorSchedule("bad-job", new[] { "not a cron" }));

        Assert.Single(monitors.Requests);
        Assert.Single(_logs.Entries, entry => entry.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task Should_log_and_stop_when_the_second_attempt_fails_too()
    {
        var monitors = new FakeMonitorsClient(Fails(HttpStatusCode.BadRequest, Rejected), Fails(HttpStatusCode.InternalServerError));

        await Sync(monitors, new MonitorSchedule("nightly-backup", new[] { "0 3 * * *" }), new MonitorSchedule("bad-job", new[] { "not a cron" }));

        Assert.Equal(2, monitors.Requests.Count);
        Assert.Single(_logs.Entries, entry => entry.Level == LogLevel.Error);
    }

    [Theory]
    [InlineData(429, "")]
    [InlineData(400, """{"detail":"Bad request"}""")]
    public async Task Should_log_and_not_send_again_after_any_other_error(int statusCode, string response)
    {
        var monitors = new FakeMonitorsClient(Fails((HttpStatusCode)statusCode, response));

        await Sync(monitors, new MonitorSchedule("nightly-backup", new[] { "0 3 * * *" }));

        Assert.Single(monitors.Requests);
        var error = Assert.Single(_logs.Entries, entry => entry.Level == LogLevel.Error);
        Assert.IsType<ApiException>(error.Exception);
    }

    [Fact]
    public async Task Should_log_and_not_throw_when_Cronitor_cannot_be_reached()
    {
        var monitors = new FakeMonitorsClient(_ => throw new HttpRequestException("No such host"));

        await Sync(monitors, new MonitorSchedule("nightly-backup", new[] { "0 3 * * *" }));

        var error = Assert.Single(_logs.Entries, entry => entry.Level == LogLevel.Error);
        Assert.IsType<HttpRequestException>(error.Exception);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("reports/daily")]
    public void Should_not_accept_a_monitor_key_Cronitor_does_not_accept(string monitorKey)
    {
        Assert.ThrowsAny<ArgumentException>(() => new MonitorSchedule(monitorKey, new[] { "0 3 * * *" }));
    }

    [Fact]
    public void Should_not_accept_a_monitor_without_schedules()
    {
        Assert.Throws<ArgumentException>(() => new MonitorSchedule("nightly-backup", Array.Empty<string>()));
    }

    private Task Sync(IMonitorsClient monitors, params MonitorSchedule[] schedules) =>
        new ScheduleSync(monitors, _logs.CreateLogger<ScheduleSync>()).SyncAsync(schedules);

    private static Func<UpdateMonitorRequest, Task<IEnumerable<Monitor>>> Succeeds() =>
        _ => Task.FromResult(Enumerable.Empty<Monitor>());

    private static Func<UpdateMonitorRequest, Task<IEnumerable<Monitor>>> Fails(HttpStatusCode statusCode, string response = "") =>
        _ => throw new ApiException(new ApiError(), statusCode, response);

    /// <summary>
    /// Records the update requests it receives and answers them in turn with the given answers.
    /// </summary>
    private sealed class FakeMonitorsClient : IMonitorsClient
    {
        private readonly Queue<Func<UpdateMonitorRequest, Task<IEnumerable<Monitor>>>> _answers;

        public FakeMonitorsClient(params Func<UpdateMonitorRequest, Task<IEnumerable<Monitor>>>[] answers)
        {
            _answers = new Queue<Func<UpdateMonitorRequest, Task<IEnumerable<Monitor>>>>(answers);
        }

        public List<UpdateMonitorRequest> Requests { get; } = new List<UpdateMonitorRequest>();

        public async Task<IReadOnlyList<string>> Bodies()
        {
            var bodies = new List<string>();
            foreach (var request in Requests)
            {
                bodies.Add(await request.Content.ReadAsStringAsync());
            }

            return bodies;
        }

        public Task<IEnumerable<Monitor>> UpdateAsync(UpdateMonitorRequest request)
        {
            Requests.Add(request);
            return _answers.Count > 0 ? _answers.Dequeue()(request) : Succeeds()(request);
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
}
