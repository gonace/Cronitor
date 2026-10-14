using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using System;
using System.Linq;
using Xunit;

namespace Cronitor.Quartz.Tests;

public class TriggerSchedulesTests
{
    private static readonly TimeZoneInfo Berlin = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

    [Fact]
    public void Should_give_a_cron_trigger_s_job_its_monitor_key_schedule_and_time_zone()
    {
        var job = Job("reports/daily");

        var schedule = Assert.Single(TriggerSchedules.Of(new[] { (job, CronTrigger(job, "0 0 3 ? * 2", Berlin)) }, NullLogger.Instance));

        Assert.Equal("reports-daily", schedule.MonitorKey);
        Assert.Equal(new[] { "0 3 * * 1" }, schedule.Schedules);
        Assert.Equal("Europe/Berlin", schedule.Timezone);
    }

    [Fact]
    public void Should_give_a_simple_trigger_that_repeats_forever_an_interval_without_a_time_zone()
    {
        var job = Job("cleanup");

        var schedule = Assert.Single(TriggerSchedules.Of(new[] { (job, SimpleTrigger(job, minutes: 5)) }, NullLogger.Instance));

        Assert.Equal(new[] { "every 5 minutes" }, schedule.Schedules);
        Assert.Null(schedule.Timezone);
    }

    [Fact]
    public void Should_report_to_the_monitor_key_chosen_with_WithCronitorMonitorKey()
    {
        var job = JobBuilder.Create<NoopJob>().WithIdentity("backup").WithCronitorMonitorKey("nightly-backup").Build();

        var schedule = Assert.Single(TriggerSchedules.Of(new[] { (job, CronTrigger(job, "0 0 3 * * ?", TimeZoneInfo.Utc)) }, NullLogger.Instance));

        Assert.Equal("nightly-backup", schedule.MonitorKey);
    }

    [Fact]
    public void Should_report_to_the_monitor_key_on_the_trigger_over_the_one_on_the_job()
    {
        var job = JobBuilder.Create<NoopJob>().WithIdentity("backup").WithCronitorMonitorKey("nightly-backup").Build();
        var trigger = TriggerBuilder.Create().ForJob(job).UsingJobData(CronitorJobBuilderExtensions.MonitorKey, "weekly-backup").WithCronSchedule("0 0 3 ? * 1", schedule => schedule.InTimeZone(TimeZoneInfo.Utc)).Build();

        var schedule = Assert.Single(TriggerSchedules.Of(new[] { (job, trigger) }, NullLogger.Instance));

        Assert.Equal("weekly-backup", schedule.MonitorKey);
    }

    [Fact]
    public void Should_give_a_monitor_the_schedules_of_all_its_triggers()
    {
        var job = Job("nightly-backup");

        var schedule = Assert.Single(TriggerSchedules.Of(new[] { (job, CronTrigger(job, "0 0 3 * * ?", Berlin)), (job, SimpleTrigger(job, minutes: 120)) }, NullLogger.Instance));

        Assert.Equal(new[] { "0 3 * * *", "every 2 hours" }, schedule.Schedules);
        Assert.Equal("Europe/Berlin", schedule.Timezone);
    }

    [Fact]
    public void Should_not_sync_a_monitor_whose_cron_triggers_run_in_different_time_zones()
    {
        var logger = new FakeLogger<TriggerSchedulesTests>();
        var job = Job("nightly-backup");

        var schedules = TriggerSchedules.Of(new[] { (job, CronTrigger(job, "0 0 3 * * ?", Berlin)), (job, CronTrigger(job, "0 0 4 * * ?", TimeZoneInfo.Utc)) }, logger);

        Assert.Empty(schedules);
        Assert.Equal(LogLevel.Information, Assert.Single(logger.Entries).Level);
    }

    [Fact]
    public void Should_not_sync_a_monitor_with_a_trigger_Cronitor_would_read_differently_and_sync_the_others()
    {
        var logger = new FakeLogger<TriggerSchedulesTests>();
        var payroll = Job("payroll");
        var backup = Job("nightly-backup");

        var schedules = TriggerSchedules.Of(new[] { (payroll, CronTrigger(payroll, "0 0 3 15W * ?", TimeZoneInfo.Utc)), (backup, CronTrigger(backup, "0 0 3 * * ?", TimeZoneInfo.Utc)) }, logger);

        Assert.Equal(new[] { "nightly-backup" }, schedules.Select(schedule => schedule.MonitorKey));
        Assert.Equal(LogLevel.Information, Assert.Single(logger.Entries).Level);
    }

    [Fact]
    public void Should_not_sync_a_simple_trigger_that_stops_repeating()
    {
        var job = Job("nightly-backup");
        var trigger = TriggerBuilder.Create().ForJob(job).WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromMinutes(5)).WithRepeatCount(3)).Build();

        Assert.Empty(TriggerSchedules.Of(new[] { (job, trigger) }, NullLogger.Instance));
    }

    [Fact]
    public void Should_not_sync_a_trigger_of_another_kind()
    {
        var job = Job("nightly-backup");
        var trigger = TriggerBuilder.Create().ForJob(job).WithCalendarIntervalSchedule(schedule => schedule.WithInterval(1, IntervalUnit.Day)).Build();

        Assert.Empty(TriggerSchedules.Of(new[] { (job, trigger) }, NullLogger.Instance));
    }

    private static IJobDetail Job(string name) => JobBuilder.Create<NoopJob>().WithIdentity(name).Build();

    private static ITrigger CronTrigger(IJobDetail job, string cron, TimeZoneInfo timeZone) =>
        TriggerBuilder.Create().ForJob(job).WithCronSchedule(cron, schedule => schedule.InTimeZone(timeZone)).Build();

    private static ITrigger SimpleTrigger(IJobDetail job, int minutes) =>
        TriggerBuilder.Create().ForJob(job).WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromMinutes(minutes)).RepeatForever()).Build();
}
