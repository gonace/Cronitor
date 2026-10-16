using Xunit;

namespace Cronitor.Hangfire.Tests;

public class HangfireCronTests
{
    [Theory]
    [InlineData("0 3 * * *", "0 3 * * *")]
    [InlineData("*/15 9-17 * * 1-5", "*/15 9-17 * * 1-5")]
    [InlineData("0 3 * JAN,JUL MON-FRI", "0 3 * JAN,JUL MON-FRI")]
    [InlineData("0 3 * * THU", "0 3 * * THU")]
    [InlineData("0 3 * FEB-NOV SUN-SAT", "0 3 * FEB-NOV SUN-SAT")]
    [InlineData("0 3 * * 5-7", "0 3 * * 5-7")]
    [InlineData("0 3 * * 0-7", "0 3 * * 0-7")]
    [InlineData("0 3 * * mon", "0 3 * * mon")]
    [InlineData("0 3 * jan *", "0 3 * jan *")]
    [InlineData("0 0 3 * * *", "0 3 * * *")]
    [InlineData("00 0 3 * * *", "0 3 * * *")]
    [InlineData("0 00 03 * * *", "00 03 * * *")]
    [InlineData("0\t3 * * *", "0 3 * * *")]
    [InlineData("  0 3 * * *", "0 3 * * *")]
    [InlineData("0 3 ? * MON", "0 3 * * MON")]
    [InlineData("0 3 1 * ?", "0 3 1 * *")]
    [InlineData("0 3 ? * ?", "0 3 * * *")]
    [InlineData("0 3 L * *", "0 3 L * *")]
    [InlineData("0 3 l * *", "0 3 l * *")]
    [InlineData("0 3 * * 1#2", "0 3 * * 1#2")]
    [InlineData("0 3 * * mon#2", "0 3 * * mon#2")]
    [InlineData("@daily", "0 0 * * *")]
    [InlineData("@HOURLY", "0 * * * *")]
    [InlineData("@weekly", "0 0 * * 0")]
    [InlineData("@monthly", "0 0 1 * *")]
    [InlineData("@yearly", "0 0 1 1 *")]
    [InlineData("@every_minute", "* * * * *")]
    public void Should_convert_a_cron_expression_Cronitor_reads_the_same(string cron, string expected)
    {
        var converted = HangfireCron.TryConvert(cron, out var schedule);

        Assert.True(converted);
        Assert.Equal(expected, schedule);
    }

    [Theory]
    [InlineData("*/30 * * * * *")]
    [InlineData("30 */5 * * * *")]
    [InlineData("30 0 3 * * *")]
    [InlineData("0-30 * * * * *")]
    [InlineData("0 3 15W * *")]
    [InlineData("0 3 15w * *")]
    [InlineData("0 3 LW * *")]
    [InlineData("0 3 L-3 * *")]
    [InlineData("0 3 * * 5L")]
    [InlineData("0 3 * * 5l")]
    [InlineData("0 22-1 * * *")]
    [InlineData("0 3 * NOV-FEB *")]
    [InlineData("0 3 * * FRI-MON")]
    [InlineData("0 3 * *")]
    [InlineData("0 0 3 * * * 2027")]
    [InlineData("@every_second")]
    [InlineData("")]
    [InlineData(null)]
    public void Should_not_convert_a_cron_expression_Cronitor_would_read_differently_or_not_at_all(string cron)
    {
        var converted = HangfireCron.TryConvert(cron, out var schedule);

        Assert.False(converted);
        Assert.Null(schedule);
    }

    [Theory]
    [InlineData("0 9 1 * MON")]
    [InlineData("0 3 15 * 1#2")]
    [InlineData("0 0 9 1 * MON")]
    [InlineData("0 3 1-7 * 1-5")]
    public void Should_not_convert_a_cron_expression_that_restricts_both_the_day_of_month_and_the_day_of_week(string cron)
    {
        var converted = HangfireCron.TryConvert(cron, out var schedule);

        Assert.False(converted);
        Assert.Null(schedule);
    }
}
