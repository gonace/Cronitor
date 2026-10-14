using System;
using System.Globalization;

namespace Cronitor.Quartz;

internal static class QuartzInterval
{
    private const int MinutesPerHour = 60;
    private const int MinutesPerDay = 24 * MinutesPerHour;

    internal static bool TryConvert(TimeSpan interval, out string schedule)
    {
        schedule = null;
        if (interval < TimeSpan.FromMinutes(1) || interval.Ticks % TimeSpan.TicksPerMinute != 0)
        {
            return false;
        }

        schedule = InLargestWholeUnit((long)interval.TotalMinutes);
        return true;
    }

    private static string InLargestWholeUnit(long minutes)
    {
        if (minutes % MinutesPerDay == 0)
        {
            return Every(minutes / MinutesPerDay, "days");
        }

        if (minutes % MinutesPerHour == 0)
        {
            return Every(minutes / MinutesPerHour, "hours");
        }

        return Every(minutes, "minutes");
    }

    private static string Every(long count, string unit) => string.Format(CultureInfo.InvariantCulture, "every {0} {1}", count, unit);
}
