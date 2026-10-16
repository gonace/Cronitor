using Xunit;

namespace Cronitor.TickerQ.Tests;

public class TickerQCronTests
{
    [Theory]
    [InlineData("0 0 3 * * *", "0 3 * * *")]
    [InlineData("00 0 3 * * *", "0 3 * * *")]
    [InlineData(" 0 0 3 * * *", "0 3 * * *")]
    [InlineData("0  0 3 * * * ", "0 3 * * *")]
    [InlineData("0 0 3 01 * *", "0 3 1 * *")]
    [InlineData("0 0 3 * * 0-6", "0 3 * * 0-6")]
    [InlineData("0 0 3 * * 1/2", "0 3 * * 1/2")]
    [InlineData("0 0 3 * * SUN", "0 3 * * 0")]
    [InlineData("0 0 3 * * mon-fri", "0 3 * * 1-5")]
    [InlineData("0 0 3 * * MON/2", "0 3 * * 1/2")]
    [InlineData("0 0 3 * oct *", "0 3 * 10 *")]
    [InlineData("0 0 3 * JAN,feb *", "0 3 * 1,2 *")]
    [InlineData("0 0 3 1/5 * *", "0 3 1/5 * *")]
    [InlineData("0 0/15 * * * *", "0/15 * * * *")]
    [InlineData("0 5/15 * * * *", "5/15 * * * *")]
    [InlineData("0 */15 * * * *", "*/15 * * * *")]
    [InlineData("0 1,2-4,10/20 * * * *", "1,2-4,10/20 * * * *")]
    public void Should_convert_a_cron_expression_Cronitor_reads_the_same(string cron, string expected)
    {
        var converted = TickerQCron.TryConvert(cron, out var schedule);

        Assert.True(converted);
        Assert.Equal(expected, schedule);
    }

    [Theory]
    [InlineData("30 0 3 * * *")]
    [InlineData("0 3 * * *")]
    [InlineData("0 0 12 13 * FRI")]
    [InlineData("0 0 3 */1 * MON")]
    [InlineData("0 0 22-2 * * *")]
    [InlineData("0 0 3 * * FRI-MON")]
    [InlineData("0 0 3 * NOV-FEB *")]
    [InlineData("0 0/0 3 * * *")]
    [InlineData("0 */0 * * * *")]
    [InlineData("0 0 3 * * Sunday")]
    [InlineData("0 0 3 * * *,1")]
    [InlineData("0 0 3 * * 7")]
    [InlineData("0 60 3 * * *")]
    [InlineData("0 -5 * * * *")]
    [InlineData("0 0 3 L * *")]
    [InlineData("0 0 3 ? * *")]
    [InlineData("0 0 3 * * 1#2")]
    [InlineData("0 0 3 15W * *")]
    [InlineData("0 0 3\t* * *")]
    [InlineData("")]
    [InlineData(null)]
    public void Should_not_convert_a_cron_expression_Cronitor_would_read_differently_or_TickerQ_never_runs(string cron)
    {
        var converted = TickerQCron.TryConvert(cron, out var schedule);

        Assert.False(converted);
        Assert.Null(schedule);
    }
}
