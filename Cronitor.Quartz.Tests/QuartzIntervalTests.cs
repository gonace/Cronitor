using System;
using Xunit;

namespace Cronitor.Quartz.Tests;

public class QuartzIntervalTests
{
    [Theory]
    [InlineData(1, "every 1 minutes")]
    [InlineData(5, "every 5 minutes")]
    [InlineData(90, "every 90 minutes")]
    [InlineData(120, "every 2 hours")]
    [InlineData(1440, "every 1 days")]
    [InlineData(2880, "every 2 days")]
    public void Should_convert_an_interval_of_whole_minutes(int minutes, string expected)
    {
        var converted = QuartzInterval.TryConvert(TimeSpan.FromMinutes(minutes), out var schedule);

        Assert.True(converted);
        Assert.Equal(expected, schedule);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(90)]
    [InlineData(0)]
    public void Should_not_convert_an_interval_that_is_not_whole_minutes(int seconds)
    {
        var converted = QuartzInterval.TryConvert(TimeSpan.FromSeconds(seconds), out var schedule);

        Assert.False(converted);
        Assert.Null(schedule);
    }
}
