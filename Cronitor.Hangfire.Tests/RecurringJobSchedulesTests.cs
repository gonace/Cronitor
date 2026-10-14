using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;
using Xunit;

namespace Cronitor.Hangfire.Tests;

public class RecurringJobSchedulesTests
{
    [Fact]
    public void Should_give_a_recurring_job_its_monitor_key_schedule_and_time_zone()
    {
        var recurringJobs = new[] { RecurringJob("reports/daily", "0 3 * * *", "W. Europe Standard Time") };

        var schedule = Assert.Single(RecurringJobSchedules.Of(recurringJobs, NullLogger.Instance));

        Assert.Equal("reports-daily", schedule.MonitorKey);
        Assert.Equal(new[] { "0 3 * * *" }, schedule.Schedules);
        Assert.Equal("Europe/Berlin", schedule.Timezone);
    }

    [Fact]
    public void Should_use_UTC_for_a_recurring_job_without_a_time_zone()
    {
        var recurringJobs = new[] { RecurringJob("nightly-backup", "0 3 * * *", timeZoneId: null) };

        var schedule = Assert.Single(RecurringJobSchedules.Of(recurringJobs, NullLogger.Instance));

        Assert.Equal("UTC", schedule.Timezone);
    }

    [Fact]
    public void Should_combine_the_schedules_of_recurring_jobs_that_report_to_the_same_monitor()
    {
        var recurringJobs = new[] { RecurringJob("reports/daily", "0 3 * * *"), RecurringJob("reports-daily", "0 15 * * *") };

        var schedule = Assert.Single(RecurringJobSchedules.Of(recurringJobs, NullLogger.Instance));

        Assert.Equal(new[] { "0 3 * * *", "0 15 * * *" }, schedule.Schedules);
    }

    [Fact]
    public void Should_not_sync_a_monitor_whose_recurring_jobs_run_in_different_time_zones()
    {
        var logger = new FakeLogger<RecurringJobSchedulesTests>();
        var recurringJobs = new[] { RecurringJob("reports/daily", "0 3 * * *", "UTC"), RecurringJob("reports-daily", "0 15 * * *", "W. Europe Standard Time") };

        var schedules = RecurringJobSchedules.Of(recurringJobs, logger);

        Assert.Empty(schedules);
        Assert.Equal(LogLevel.Information, Assert.Single(logger.Entries).Level);
    }

    [Fact]
    public void Should_not_sync_a_monitor_with_a_cron_expression_Cronitor_would_read_differently_and_sync_the_others()
    {
        var logger = new FakeLogger<RecurringJobSchedulesTests>();
        var recurringJobs = new[] { RecurringJob("payroll", "0 3 15W * *"), RecurringJob("nightly-backup", "0 3 * * *") };

        var schedules = RecurringJobSchedules.Of(recurringJobs, logger);

        Assert.Equal(new[] { "nightly-backup" }, schedules.Select(schedule => schedule.MonitorKey));
        Assert.Equal(LogLevel.Information, Assert.Single(logger.Entries).Level);
    }

    [Fact]
    public void Should_not_sync_a_monitor_whose_time_zone_is_unknown()
    {
        var recurringJobs = new[] { RecurringJob("nightly-backup", "0 3 * * *", "Somewhere/Nowhere") };

        Assert.Empty(RecurringJobSchedules.Of(recurringJobs, NullLogger.Instance));
    }

    [Fact]
    public void Should_not_sync_a_removed_recurring_job()
    {
        var recurringJob = RecurringJob("nightly-backup", "0 3 * * *");
        recurringJob.Removed = true;

        Assert.Empty(RecurringJobSchedules.Of(new[] { recurringJob }, NullLogger.Instance));
    }

    private static RecurringJobDto RecurringJob(string id, string cron, string timeZoneId = "UTC") =>
        new RecurringJobDto { Id = id, Cron = cron, TimeZoneId = timeZoneId };
}
