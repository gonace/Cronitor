using Cronitor.Scheduling;
using System;
using Xunit;

namespace Cronitor.Tests;

public class CronitorTimezoneTests
{
    [Fact]
    public void Should_convert_a_Windows_time_zone_to_its_IANA_name()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

        var ianaName = CronitorTimezone.Of(timeZone);

        Assert.Equal("Europe/Berlin", ianaName);
    }

    [Fact]
    public void Should_keep_the_name_of_a_time_zone_that_already_has_an_IANA_name()
    {
        var timeZone = TimeZoneInfo.CreateCustomTimeZone("Europe/Stockholm", TimeSpan.FromHours(1), "Stockholm", "Stockholm");

        var ianaName = CronitorTimezone.Of(timeZone);

        Assert.Equal("Europe/Stockholm", ianaName);
    }

    [Fact]
    public void Should_name_UTC_UTC()
    {
        var ianaName = CronitorTimezone.Of(TimeZoneInfo.Utc);

        Assert.Equal("UTC", ianaName);
    }

    [Fact]
    public void Should_not_find_an_IANA_name_for_a_time_zone_that_has_none()
    {
        var timeZone = TimeZoneInfo.CreateCustomTimeZone("Office hours", TimeSpan.FromHours(3), "Office hours", "Office hours");

        var found = CronitorTimezone.TryGetIanaName(timeZone, out var ianaName);

        Assert.False(found);
        Assert.Null(ianaName);
    }

    [Fact]
    public void Should_throw_from_Of_for_a_time_zone_that_has_no_IANA_name()
    {
        var timeZone = TimeZoneInfo.CreateCustomTimeZone("Office hours", TimeSpan.FromHours(3), "Office hours", "Office hours");

        var exception = Assert.Throws<ArgumentException>(() => CronitorTimezone.Of(timeZone));

        Assert.Contains("Office hours", exception.Message);
    }

    /// <summary>
    /// The fallback for a Windows app without ICU, where .NET's own conversion fails.
    /// </summary>
    [Fact]
    public void Should_convert_a_Windows_name_with_the_mapping_alone()
    {
        var converted = CronitorTimezone.TryConvertWindowsNameWithMapping("W. Europe Standard Time", out var ianaName);

        Assert.True(converted);
        Assert.Equal("Europe/Berlin", ianaName);
    }

    [Fact]
    public void Should_recognise_an_IANA_name_with_the_mapping_alone()
    {
        var recognised = CronitorTimezone.IsIanaNameInMapping("Europe/Stockholm");

        Assert.True(recognised);
    }

    [Fact]
    public void Should_throw_when_the_time_zone_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => CronitorTimezone.Of(null));
    }
}
