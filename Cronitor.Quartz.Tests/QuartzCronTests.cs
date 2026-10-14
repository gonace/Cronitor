using Xunit;

namespace Cronitor.Quartz.Tests;

public class QuartzCronTests
{
    [Theory]
    [InlineData("0 0 3 * * ?", "0 3 * * *")]
    [InlineData("0 0 3 ? * *", "0 3 * * *")]
    [InlineData("0 0 3 * * *", "0 3 * * *")]
    [InlineData("0 0 3 ? * 1", "0 3 * * 0")]
    [InlineData("0 0 3 ? * 2", "0 3 * * 1")]
    [InlineData("0 0 3 ? * 7", "0 3 * * 6")]
    [InlineData("0 0 3 ? * 2-6", "0 3 * * 1-5")]
    [InlineData("0 0 3 ? * 1-7", "0 3 * * 0-6")]
    [InlineData("0 0 3 ? * 2,4,6", "0 3 * * 1,3,5")]
    [InlineData("0 0 3 ? * 2/2", "0 3 * * 1/2")]
    [InlineData("0 0 3 ? * 2#2", "0 3 * * 1#2")]
    [InlineData("0 0 3 ? * MON-FRI", "0 3 * * MON-FRI")]
    [InlineData("0 0 3 ? * MON#2", "0 3 * * MON#2")]
    [InlineData("0 0 3 ? JAN MON", "0 3 * JAN MON")]
    [InlineData("0 0 3 L * ?", "0 3 L * *")]
    [InlineData("0 0 3 1/5 * ?", "0 3 1/5 * *")]
    [InlineData("0 0/15 * * * ?", "0/15 * * * *")]
    [InlineData("0 5/15 * * * ?", "5/15 * * * *")]
    [InlineData("0 */15 * * * ?", "*/15 * * * *")]
    [InlineData("00 0 3 * * ?", "0 3 * * *")]
    [InlineData("0 00 03 * * ?", "00 03 * * *")]
    [InlineData("0 0 3 * * ? *", "0 3 * * *")]
    [InlineData("  0 0 3 * * ?", "0 3 * * *")]
    [InlineData("0\t0 3 * * ?", "0 3 * * *")]
    public void Should_convert_a_cron_expression_Cronitor_reads_the_same(string cron, string expected)
    {
        var converted = QuartzCron.TryConvert(cron, out var schedule);

        Assert.True(converted);
        Assert.Equal(expected, schedule);
    }

    [Theory]
    [InlineData("30 0 3 * * ?")]
    [InlineData("0 0 3 1 * MON")]
    [InlineData("0 0 3 ? * L")]
    [InlineData("0 0 3 ? * 6L")]
    [InlineData("0 0 3 LW * ?")]
    [InlineData("0 0 3 15W * ?")]
    [InlineData("0 0 3 L-3 * ?")]
    [InlineData("0 0 3 ? * FRI-MON")]
    [InlineData("0 0 22-2 * * ?")]
    [InlineData("0 0 3 ? NOV-FEB *")]
    [InlineData("0 0 3 * * ? 2027")]
    [InlineData("0 0 3 * * ? 2026-2030")]
    [InlineData("0 3 * * *")]
    [InlineData("0 0 3 * * ? 2027 1")]
    [InlineData("")]
    [InlineData(null)]
    public void Should_not_convert_a_cron_expression_Cronitor_would_read_differently_or_not_at_all(string cron)
    {
        var converted = QuartzCron.TryConvert(cron, out var schedule);

        Assert.False(converted);
        Assert.Null(schedule);
    }
}
