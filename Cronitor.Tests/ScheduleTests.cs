using Cronitor.Scheduling;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xunit;

namespace Cronitor.Tests;

public class ScheduleTests
{
    [Fact]
    public void Should_build_Cron_EveryMinute()
    {
        var result = Schedule.Cron.EveryMinute();

        Assert.Equal("* * * * *", result.Value);
    }

    [Fact]
    public void Should_build_Cron_EveryHour_at_minute_zero()
    {
        var result = Schedule.Cron.EveryHour();

        Assert.Equal("0 * * * *", result.Value);
    }

    [Fact]
    public void Should_build_Cron_EveryHour_at_a_minute()
    {
        var result = Schedule.Cron.EveryHour(minute: 30);

        Assert.Equal("30 * * * *", result.Value);
    }

    [Fact]
    public void Should_build_Cron_Daily_at_midnight()
    {
        var result = Schedule.Cron.Daily();

        Assert.Equal("0 0 * * *", result.Value);
    }

    [Fact]
    public void Should_build_Cron_Daily_at_an_hour()
    {
        var result = Schedule.Cron.Daily(hour: 3);

        Assert.Equal("0 3 * * *", result.Value);
    }

    [Fact]
    public void Should_build_Cron_Daily_at_an_hour_and_minute()
    {
        var result = Schedule.Cron.Daily(hour: 0, minute: 35);

        Assert.Equal("35 0 * * *", result.Value);
    }

    [Fact]
    public void Should_build_Cron_Weekly_on_a_day()
    {
        var result = Schedule.Cron.Weekly(day: 1);

        Assert.Equal("0 0 * * 1", result.Value);
    }

    [Fact]
    public void Should_build_Cron_Weekly_on_a_day_at_a_time()
    {
        var result = Schedule.Cron.Weekly(day: 1, hour: 8, minute: 30);

        Assert.Equal("30 8 * * 1", result.Value);
    }

    [Fact]
    public void Should_build_Cron_Monthly_on_a_day()
    {
        var result = Schedule.Cron.Monthly(day: 15);

        Assert.Equal("0 0 15 * *", result.Value);
    }

    [Fact]
    public void Should_build_Cron_Monthly_on_a_day_at_a_time()
    {
        var result = Schedule.Cron.Monthly(day: 1, hour: 6, minute: 15);

        Assert.Equal("15 6 1 * *", result.Value);
    }

    [Fact]
    public void Should_build_Cron_Yearly_on_a_date()
    {
        var result = Schedule.Cron.Yearly(month: 1, day: 1);

        Assert.Equal("0 0 1 1 *", result.Value);
    }

    [Fact]
    public void Should_build_Cron_Yearly_on_a_date_at_a_time()
    {
        var result = Schedule.Cron.Yearly(month: 6, day: 15, hour: 12, minute: 30);

        Assert.Equal("30 12 15 6 *", result.Value);
    }

    [Fact]
    public void Should_build_a_raw_Cron_Expression()
    {
        var result = Schedule.Cron.Expression("0 0 * * *");

        Assert.Equal("0 0 * * *", result.Value);
    }

    [Fact]
    public void Should_build_Every_Seconds()
    {
        var result = Schedule.Every(60).Seconds;

        Assert.Equal("every 60 seconds", result.Value);
    }

    [Fact]
    public void Should_build_Every_Minutes()
    {
        var result = Schedule.Every(5).Minutes;

        Assert.Equal("every 5 minutes", result.Value);
    }

    [Fact]
    public void Should_build_Every_Hours()
    {
        var result = Schedule.Every(2).Hours;

        Assert.Equal("every 2 hours", result.Value);
    }

    [Fact]
    public void Should_build_Every_Days()
    {
        var result = Schedule.Every(1).Days;

        Assert.Equal("every 1 days", result.Value);
    }

    [Fact]
    public void Should_build_a_custom_cron_with_Create()
    {
        var result = Schedule.Cron.Create()
            .Minute(30)
            .Hour(3)
            .DayOfMonth(15)
            .Month(6)
            .DayOfWeek(1)
            .Build();

        Assert.Equal("30 3 15 6 1", result.Value);
    }

    [Fact]
    public void Should_use_defaults_in_a_custom_cron()
    {
        var result = Schedule.Cron.Create()
            .Minute(0)
            .Hour(3)
            .Build();

        Assert.Equal("0 3 * * *", result.Value);
    }

    [Fact]
    public void Should_build_a_custom_cron_with_a_step()
    {
        var result = Schedule.Cron.Create()
            .MinuteEvery(15)
            .Build();

        Assert.Equal("*/15 * * * *", result.Value);
    }

    [Fact]
    public void Should_build_a_custom_cron_with_a_range()
    {
        var result = Schedule.Cron.Create()
            .Minute(0)
            .HourRange(9, 17)
            .DayOfWeekRange(1, 5)
            .Build();

        Assert.Equal("0 9-17 * * 1-5", result.Value);
    }

    [Fact]
    public void Should_convert_a_custom_cron_to_a_string()
    {
        var result = Schedule.Cron.Create()
            .Minute(0)
            .Hour(12)
            .ToString();

        Assert.Equal("0 12 * * *", result);
    }

    [Fact]
    public void Should_return_the_value_from_ToString()
    {
        var expression = Schedule.Cron.Daily(hour: 3);

        Assert.Equal("0 3 * * *", expression.ToString());
    }

    [Fact]
    public void Should_convert_implicitly_to_a_string()
    {
        string result = Schedule.Every(5).Minutes;

        Assert.Equal("every 5 minutes", result);
    }

    [Fact]
    public void Should_convert_implicitly_from_a_string_to_a_ScheduleExpression()
    {
        ScheduleExpression expression = "0 0 * * *";

        Assert.Equal("0 0 * * *", expression.Value);
    }

    [Fact]
    public void Should_serialize_as_a_json_string()
    {
        var container = new ScheduleContainer { Schedule = Schedule.Cron.Daily() };

        var json = Serializer.Serialize(container);

        Assert.Equal("{\"schedule\":\"0 0 * * *\"}", json);
    }

    [Fact]
    public void Should_deserialize_from_a_json_string()
    {
        var json = "{\"schedule\":\"0 3 * * *\"}";

        var result = Serializer.Deserialize<ScheduleContainer>(json);

        Assert.Equal("0 3 * * *", result.Schedule.Value);
    }

    private class ScheduleContainer
    {
        [JsonPropertyName("schedule")]
        public ScheduleExpression Schedule { get; set; }
    }
}
