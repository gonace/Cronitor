using Hangfire.Common;
using Hangfire.InMemory;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Hangfire.Extensions.Cronitor.Tests;

/// <summary>
/// Runs jobs on a real Hangfire server, with in-memory storage, and checks what the filter reports to Cronitor.
/// </summary>
public class CronitorJobFilterTests
{
    [Fact]
    public async Task Should_send_run_and_complete_when_a_recurring_job_succeeds()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries));

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_with_the_message_when_a_recurring_job_throws()
    {
        var telemetries = new FakeTelemetriesClient();

        await RunRecurringJob("nightly-backup", () => TestJobs.Throw(), new CronitorJobFilter(telemetries));

        Assert.Equal(new[] { "run nightly-backup", "fail nightly-backup: Disk is full" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_not_report_a_job_that_is_not_recurring()
    {
        var telemetries = new FakeTelemetriesClient();
        var storage = new InMemoryStorage();

        using (TestServer.Start(storage, new JobFilterCollection { new CronitorJobFilter(telemetries) }))
        {
            new BackgroundJobClient(storage).Enqueue(() => TestJobs.Succeed());
            await TestServer.WaitUntil(() => storage.GetMonitoringApi().SucceededListCount() == 1);
        }

        Assert.Empty(telemetries.Sent);
    }

    [Fact]
    public async Task Should_still_run_the_job_and_log_the_error_when_the_filter_fails_before_it()
    {
        var telemetries = new FakeTelemetriesClient { ThrowOnStart = true };
        var logger = new FakeLogger<CronitorJobFilter>();

        var storage = await RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries, logger));

        Assert.Equal(1, storage.GetMonitoringApi().SucceededListCount());
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Start failed", entry.Exception.Message);
    }

    [Fact]
    public async Task Should_keep_the_job_succeeded_and_log_the_error_when_the_filter_fails_after_it()
    {
        var telemetries = new FakeTelemetriesClient { ThrowOnFinish = true };
        var logger = new FakeLogger<CronitorJobFilter>();

        var storage = await RunRecurringJob("nightly-backup", () => TestJobs.Succeed(), new CronitorJobFilter(telemetries, logger));

        Assert.Equal(1, storage.GetMonitoringApi().SucceededListCount());
        Assert.Equal(0, storage.GetMonitoringApi().FailedCount());
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Finish failed", entry.Exception.Message);
    }

    private static Task<InMemoryStorage> RunRecurringJob(string recurringJobId, Expression<Action> job, CronitorJobFilter filter) =>
        TestServer.RunRecurringJob(recurringJobId, job, new JobFilterCollection { filter });
}
