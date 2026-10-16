using Cronitor.Constants;
using Cronitor.Exceptions;
using Cronitor.Models;
using Cronitor.Requests;
using Cronitor.Responses;
using Cronitor.Scheduling;
using Cronitor.Tests.Builders;
using Cronitor.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Tests;

public class MonitorsClientTests : BaseTest
{
    private const string MonitorsUri = "https://cronitor.io/api/monitors";
    private const string MonitorUri = MonitorsUri + "/" + MonitorKey;

    [Fact]
    public void Should_construct_with_an_api_key()
    {
        var client = new MonitorsClient(ApiKey);

        Assert.NotNull(client);
    }

    [Fact]
    public void Should_dispose_the_MonitorsClient()
    {
        using (var client = new MonitorsClient(ApiKey))
        {
            Assert.NotNull(client);
        }
        // Test passes if no exception is thrown during disposal
    }

    [Theory]
    [JsonData("ListMonitors.json")]
    public void Should_execute_List(string json)
    {
        var handler = new FakeHttpMessageHandler(body: json);
        var monitorsClient = CreateClient(handler);

        var result = monitorsClient.List();

        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(50, result.PageSize);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorsUri + "?page=1");
    }

    [Fact]
    public async Task Should_execute_ListAsync()
    {
        var response = new ListMonitorResponse { Items = new List<Monitor> { Make.Check.Build(), Make.Heartbeat.Build(), Make.Job.Build() } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.ListAsync();

        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(1, result.Page);
        Assert.Equal(50, result.PageSize);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorsUri + "?page=1");
    }

    [Fact]
    public void Should_execute_List_with_filters()
    {
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(new ListMonitorResponse { Items = new List<Monitor> { Make.Job.Build() } }));
        var monitorsClient = CreateClient(handler);

        var result = monitorsClient.List(new ListMonitorRequest { Types = { MonitorType.Job }, States = { MonitorState.Failing } });

        Assert.Single(result.Items);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorsUri + "?page=1&type=job&state=failing");
    }

    [Fact]
    public async Task Should_execute_ListAsync_with_filters()
    {
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(new ListMonitorResponse { Items = new List<Monitor> { Make.Job.Build() } }));
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.ListAsync(new ListMonitorRequest { Environment = "production", Sort = MonitorSort.CreatedDescending });

        Assert.Single(result.Items);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorsUri + "?page=1&env=production&sort=-created");
    }

    [Fact]
    public void Should_request_api_version_2025_11_28()
    {
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(new ListMonitorResponse { Items = new List<Monitor>() }));
        var monitorsClient = CreateClient(handler);

        monitorsClient.List();

        var request = Assert.Single(handler.Requests);
        Assert.Equal(new[] { "2025-11-28" }, request.Headers.GetValues("Cronitor-Version"));
    }

    [Fact]
    public async Task Should_require_a_request_in_ListAsync()
    {
        var monitorsClient = CreateClient(new FakeHttpMessageHandler());

        await Assert.ThrowsAsync<ArgumentNullException>(() => monitorsClient.ListAsync((ListMonitorRequest)null));
    }

    [Fact]
    public void Should_execute_Get()
    {
        var response = new Monitor(MonitorKey);
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var monitorsClient = CreateClient(handler);

        var result = monitorsClient.Get(MonitorKey);

        Assert.NotNull(result);
        Assert.Equal(response.Key, result.Key);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri);
    }

    [Fact]
    public async Task Should_execute_GetAsync()
    {
        var response = new Monitor(MonitorKey);
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.GetAsync(MonitorKey);

        Assert.NotNull(result);
        Assert.Equal(response.Key, result.Key);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri);
    }

    [Fact]
    public void Should_execute_Create()
    {
        var notify = new List<string>
        {
            "developers",
            "administrators"
        };
        var monitor = Make.Job
            .Notify(notify)
            .Schedule(Schedule.Every(60).Seconds)
            .Build();
        var response = new CreateMonitorResponse { Monitors = new List<Monitor> { monitor } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var monitorsClient = CreateClient(handler);

        var result = monitorsClient.Create(new CreateMonitorRequest(monitor));

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
    }

    [Fact]
    public async Task Should_execute_CreateAsync()
    {
        var monitor = Make.Job.Build();
        var response = new CreateMonitorResponse { Monitors = new List<Monitor> { monitor } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.CreateAsync(new CreateMonitorRequest(monitor));

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
    }

    [Fact]
    public void Should_execute_Update()
    {
        var monitor = Make.Job.Key(MonitorKey).Build();
        var response = new UpdateMonitorResponse { Monitors = new List<Monitor> { monitor } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var monitorsClient = CreateClient(handler);

        var result = monitorsClient.Update(new UpdateMonitorRequest(monitor));

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Single(result);
        AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
    }

    [Fact]
    public async Task Should_execute_UpdateAsync()
    {
        var monitor = Make.Job.Key(MonitorKey).Build();
        var response = new UpdateMonitorResponse { Monitors = new List<Monitor> { monitor } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.UpdateAsync(new UpdateMonitorRequest(monitor));

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Single(result);
        AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
    }

    [Fact]
    public void Should_execute_Delete()
    {
        var handler = new FakeHttpMessageHandler();
        var monitorsClient = CreateClient(handler);

        monitorsClient.Delete(MonitorKey);

        AssertRequest.Sent(handler, HttpMethod.Delete, MonitorUri);
    }

    [Fact]
    public async Task Should_execute_DeleteAsync()
    {
        var handler = new FakeHttpMessageHandler();
        var monitorsClient = CreateClient(handler);

        await monitorsClient.DeleteAsync(MonitorKey);

        AssertRequest.Sent(handler, HttpMethod.Delete, MonitorUri);
    }

    [Fact]
    public void Should_execute_Pause()
    {
        var handler = new FakeHttpMessageHandler();
        var monitorsClient = CreateClient(handler);

        monitorsClient.Pause(MonitorKey);

        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause");
    }

    [Fact]
    public async Task Should_execute_PauseAsync()
    {
        var handler = new FakeHttpMessageHandler();
        var monitorsClient = CreateClient(handler);

        await monitorsClient.PauseAsync(MonitorKey);

        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause");
    }

    [Fact]
    public void Should_execute_Pause_with_hours()
    {
        var handler = new FakeHttpMessageHandler();
        var monitorsClient = CreateClient(handler);

        monitorsClient.Pause(MonitorKey, 24);

        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause/24");
    }

    [Fact]
    public async Task Should_execute_PauseAsync_with_hours()
    {
        var handler = new FakeHttpMessageHandler();
        var monitorsClient = CreateClient(handler);

        await monitorsClient.PauseAsync(MonitorKey, 24);

        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause/24");
    }

    [Fact]
    public void Should_execute_Unpause()
    {
        var handler = new FakeHttpMessageHandler();
        var monitorsClient = CreateClient(handler);

        monitorsClient.Unpause(MonitorKey);

        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause/0");
    }

    [Fact]
    public async Task Should_execute_UnpauseAsync()
    {
        var handler = new FakeHttpMessageHandler();
        var monitorsClient = CreateClient(handler);

        await monitorsClient.UnpauseAsync(MonitorKey);

        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pause/0");
    }

    [Fact]
    public void Should_execute_Activities()
    {
        var activities = new List<Activity> { new Activity() };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(activities));
        var monitorsClient = CreateClient(handler);

        var result = monitorsClient.Activities(MonitorKey);

        Assert.NotNull(result);
        Assert.Single(result);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/activity");
    }

    [Fact]
    public async Task Should_execute_ActivitiesAsync()
    {
        var activities = new List<Activity> { new Activity() };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(activities));
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.ActivitiesAsync(MonitorKey);

        Assert.NotNull(result);
        Assert.Single(result);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/activity");
    }

    [Theory]
    [JsonData("ListMonitorAlerts.json")]
    public void Should_execute_Alerts(string json)
    {
        var handler = new FakeHttpMessageHandler(body: json);
        var monitorsClient = CreateClient(handler);

        var result = monitorsClient.Alerts(MonitorKey);

        Assert.Equal(2, result.Count());
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/alerts");
    }

    [Theory]
    [JsonData("ListMonitorAlerts.json")]
    public async Task Should_execute_AlertsAsync(string json)
    {
        var handler = new FakeHttpMessageHandler(body: json);
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.AlertsAsync(MonitorKey);

        Assert.Equal(2, result.Count());
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/alerts");
    }

    [Theory]
    [JsonData("ListMonitorAlerts.json")]
    public void Should_read_alerts_from_a_real_response(string json)
    {
        var handler = new FakeHttpMessageHandler(body: json);
        var monitorsClient = CreateClient(handler);

        var alert = monitorsClient.Alerts(MonitorKey).Last();

        Assert.Equal("alert", alert.Type);
        Assert.Equal("slack", alert.Channel);
        Assert.Equal("#alerts", alert.Destination);
        Assert.Equal("nightly-backup", alert.Monitor.Name);
        Assert.Equal("abc123", alert.Monitor.Key);
        Assert.Equal("3ebaf133531bc718", alert.Incident);
        Assert.Null(alert.NotificationList);
        Assert.True(alert.Delivered);
        Assert.Equal(new DateTimeOffset(2026, 6, 15, 18, 54, 28, TimeSpan.Zero).AddTicks(8255160), alert.CreatedAt);
        Assert.Equal("e9776de559f8d3f3595582f992515442", alert.GroupByKey);
    }

    [Fact]
    public void Should_execute_Pings()
    {
        var pings = new List<Ping> { new Ping() };
        var response = new Dictionary<string, IEnumerable<Ping>> { { MonitorKey, pings } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var monitorsClient = CreateClient(handler);

        var result = monitorsClient.Pings(MonitorKey);

        Assert.NotNull(result);
        Assert.Single(result);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pings");
    }

    [Theory]
    [JsonData("ListMonitorActivity.json")]
    public void Should_read_activities_from_a_real_response(string json)
    {
        var handler = new FakeHttpMessageHandler(body: json);
        var monitorsClient = CreateClient(handler);

        var activities = monitorsClient.Activities(MonitorKey).ToList();

        Assert.Equal(2, activities.Count);
        Assert.Equal("complete", activities[0].Event);
        Assert.Equal(1791718744.454m, activities[0].Timestamp);
        Assert.Equal(2.018m, activities[0].Duration);
        Assert.Equal("worker-01", activities[0].Host);
        Assert.Equal(42m, activities[0].Count);
        Assert.Equal(1m, activities[0].ErrorCount);
        Assert.Null(activities[1].Count);
        Assert.Null(activities[1].ErrorCount);
    }

    [Theory]
    [JsonData("ListMonitorPings.json")]
    public void Should_read_pings_from_a_real_response(string json)
    {
        var handler = new FakeHttpMessageHandler(body: json);
        var monitorsClient = CreateClient(handler);

        var pings = monitorsClient.Pings(MonitorKey).ToList();

        Assert.Equal(2, pings.Count);
        Assert.Equal("complete", pings[0].Event);
        Assert.Equal(1791714304.604m, pings[0].Timestamp);
        Assert.Equal(2.555m, pings[0].Duration);
        Assert.Equal("worker-01", pings[0].Host);
        Assert.Equal(42m, pings[0].Count);
        Assert.Equal(1m, pings[0].ErrorCount);
        Assert.Null(pings[1].Count);
        Assert.Null(pings[1].ErrorCount);
    }

    [Fact]
    public async Task Should_execute_PingsAsync()
    {
        var pings = new List<Ping> { new Ping() };
        var response = new Dictionary<string, IEnumerable<Ping>> { { MonitorKey, pings } };
        var handler = new FakeHttpMessageHandler(body: Serializer.Serialize(response));
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.PingsAsync(MonitorKey);

        Assert.NotNull(result);
        Assert.Single(result);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pings");
    }

    [Fact]
    public async Task Should_return_null_from_CreateAsync_when_the_response_is_null()
    {
        var handler = new FakeHttpMessageHandler(body: "null");
        var monitorsClient = CreateClient(handler);

        var monitor = Make.Job.Build();
        var result = await monitorsClient.CreateAsync(new CreateMonitorRequest(monitor));

        Assert.Null(result);
        AssertRequest.Sent(handler, HttpMethod.Put, MonitorsUri);
    }

    [Fact]
    public async Task Should_return_null_from_AlertsAsync_when_the_response_is_null()
    {
        var handler = new FakeHttpMessageHandler(body: "null");
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.AlertsAsync(MonitorKey);

        Assert.Null(result);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/alerts");
    }

    [Fact]
    public async Task Should_return_no_alerts_from_AlertsAsync_when_there_are_none()
    {
        var handler = new FakeHttpMessageHandler(body: "[]");
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.AlertsAsync(MonitorKey);

        Assert.Empty(result);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/alerts");
    }

    [Fact]
    public async Task Should_return_null_from_PingsAsync_when_the_response_is_null()
    {
        var handler = new FakeHttpMessageHandler(body: "null");
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.PingsAsync(MonitorKey);

        Assert.Null(result);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pings");
    }

    [Fact]
    public async Task Should_return_null_from_PingsAsync_when_the_dictionary_is_empty()
    {
        var handler = new FakeHttpMessageHandler(body: "{}");
        var monitorsClient = CreateClient(handler);

        var result = await monitorsClient.PingsAsync(MonitorKey);

        Assert.Null(result);
        AssertRequest.Sent(handler, HttpMethod.Get, MonitorUri + "/pings");
    }

    [Fact]
    public async Task Should_reject_an_invalid_monitor_key_without_sending_anything()
    {
        var handler = new FakeHttpMessageHandler();
        var monitorsClient = CreateClient(handler);

        await Assert.ThrowsAsync<ArgumentException>(() => monitorsClient.GetAsync(" nightly-backup"));

        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task Should_get_a_monitor_by_a_key_read_from_Cronitor_even_if_it_breaks_the_rules()
    {
        var handler = new FakeHttpMessageHandler(body: "{\"key\":\"reports/daily\"}");
        var monitorsClient = CreateClient(handler);
        var monitor = Serializer.Deserialize<Monitor>("{\"key\":\"reports/daily\"}");

        await monitorsClient.GetAsync(monitor.Key);

        AssertRequest.Sent(handler, HttpMethod.Get, MonitorsUri + "/reports%2Fdaily");
    }

    [Fact]
    public async Task Should_throw_ApiException_from_GetAsync_when_the_monitor_is_not_found()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.NotFound, "{\"detail\":\"Monitor not found\"}");
        var monitorsClient = CreateClient(handler);

        var exception = await Assert.ThrowsAsync<ApiException>(() => monitorsClient.GetAsync(MonitorKey));

        Assert.Equal("Monitor not found (NotFound)", exception.Message);
    }

    [Fact]
    public async Task Should_throw_ApiException_from_DeleteAsync_when_unauthorized()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.Unauthorized, "{\"detail\":\"Invalid API key\"}");
        var monitorsClient = CreateClient(handler);

        var exception = await Assert.ThrowsAsync<ApiException>(() => monitorsClient.DeleteAsync(MonitorKey));

        Assert.Equal("Invalid API key (Unauthorized)", exception.Message);
    }

    [Fact]
    public async Task Should_keep_the_status_code_and_response_of_a_rejected_update_in_the_ApiException()
    {
        const string response = """{"nightly-backup":{"schedules":["Invalid schedule expression: not a cron"]}}""";
        var monitorsClient = CreateClient(new FakeHttpMessageHandler(HttpStatusCode.BadRequest, response));

        var exception = await Assert.ThrowsAsync<ApiException>(() => monitorsClient.UpdateAsync(new UpdateMonitorRequest(Make.Job.Build())));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal(response, exception.Response);
    }

    [Theory]
    [InlineData(429, "")]
    [InlineData(502, "<html>Bad Gateway</html>")]
    [InlineData(400, "null")]
    public async Task Should_keep_the_status_code_and_response_of_an_error_whose_body_is_not_an_error_object(int statusCode, string response)
    {
        var monitorsClient = CreateClient(new FakeHttpMessageHandler((HttpStatusCode)statusCode, response));

        var exception = await Assert.ThrowsAsync<ApiException>(() => monitorsClient.UpdateAsync(new UpdateMonitorRequest(Make.Job.Build())));

        Assert.Equal((HttpStatusCode)statusCode, exception.StatusCode);
        Assert.Equal(response, exception.Response);
    }

    private static MonitorsClient CreateClient(FakeHttpMessageHandler handler)
    {
        return new MonitorsClient(new Internals.HttpClient(Urls.DefaultApiUrl, ApiKey, handler));
    }
}
