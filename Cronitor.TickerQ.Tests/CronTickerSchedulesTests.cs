using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;
using TickerQ.Utilities.Entities;
using Xunit;

namespace Cronitor.TickerQ.Tests;

public class CronTickerSchedulesTests
{
    private static readonly TimeZoneInfo Berlin = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

    [Fact]
    public void Should_give_a_cron_ticker_s_function_its_monitor_key_schedule_and_TickerQ_s_time_zone()
    {
        var cronTickers = new[] { CronTicker("reports/daily", "0 0 3 * * *") };

        var schedule = Assert.Single(CronTickerSchedules.Of(cronTickers, Berlin, NullLogger.Instance));

        Assert.Equal("reports-daily", schedule.MonitorKey);
        Assert.Equal(new[] { "0 3 * * *" }, schedule.Schedules);
        Assert.Equal("Europe/Berlin", schedule.Timezone);
    }

    [Fact]
    public void Should_give_a_monitor_the_schedules_of_all_cron_tickers_of_its_function()
    {
        var cronTickers = new[] { CronTicker("nightly-backup", "0 0 3 * * *"), CronTicker("nightly-backup", "0 0 15 * * SUN") };

        var schedule = Assert.Single(CronTickerSchedules.Of(cronTickers, TimeZoneInfo.Utc, NullLogger.Instance));

        Assert.Equal(new[] { "0 3 * * *", "0 15 * * 0" }, schedule.Schedules);
    }

    [Fact]
    public void Should_not_sync_a_monitor_with_a_cron_expression_Cronitor_would_read_differently_and_sync_the_others()
    {
        var logger = new FakeLogger<CronTickerSchedulesTests>();
        var cronTickers = new[] { CronTicker("payroll", "0 0 12 13 * FRI"), CronTicker("nightly-backup", "0 0 3 * * *") };

        var schedules = CronTickerSchedules.Of(cronTickers, TimeZoneInfo.Utc, logger);

        Assert.Equal(new[] { "nightly-backup" }, schedules.Select(schedule => schedule.MonitorKey));
        Assert.Equal(LogLevel.Information, Assert.Single(logger.Entries).Level);
    }

    [Fact]
    public void Should_not_sync_a_function_whose_name_is_too_long_for_a_monitor_key_and_sync_the_others()
    {
        var logger = new FakeLogger<CronTickerSchedulesTests>();
        var cronTickers = new[] { CronTicker(new string('x', 101), "0 0 3 * * *"), CronTicker("nightly-backup", "0 0 3 * * *") };

        var schedules = CronTickerSchedules.Of(cronTickers, TimeZoneInfo.Utc, logger);

        Assert.Equal(new[] { "nightly-backup" }, schedules.Select(schedule => schedule.MonitorKey));
        Assert.Equal(LogLevel.Information, Assert.Single(logger.Entries).Level);
    }

    [Fact]
    public void Should_not_sync_a_disabled_cron_ticker()
    {
        var cronTicker = CronTicker("nightly-backup", "0 0 3 * * *");
        cronTicker.IsEnabled = false;

        Assert.Empty(CronTickerSchedules.Of(new[] { cronTicker }, TimeZoneInfo.Utc, NullLogger.Instance));
    }

    [Fact]
    public void Should_not_sync_when_TickerQ_s_time_zone_has_no_IANA_name()
    {
        var logger = new FakeLogger<CronTickerSchedulesTests>();
        var custom = TimeZoneInfo.CreateCustomTimeZone("Somewhere", TimeSpan.FromHours(3), "Somewhere", "Somewhere");

        var schedules = CronTickerSchedules.Of(new[] { CronTicker("nightly-backup", "0 0 3 * * *") }, custom, logger);

        Assert.Empty(schedules);
        Assert.Equal(LogLevel.Information, Assert.Single(logger.Entries).Level);
    }

    private static CronTickerEntity CronTicker(string function, string expression) =>
        new CronTickerEntity { Id = Guid.NewGuid(), Function = function, Expression = expression };
}
