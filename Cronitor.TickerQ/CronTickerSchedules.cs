using Cronitor.Scheduling;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TickerQ.Utilities.Entities;

namespace Cronitor.TickerQ;

internal static class CronTickerSchedules
{
    internal static IReadOnlyList<MonitorSchedule> Of(IEnumerable<CronTickerEntity> cronTickers, TimeZoneInfo timeZone, ILogger logger)
    {
        var schedules = new List<MonitorSchedule>();
        foreach (var monitor in cronTickers.Where(cronTicker => cronTicker.IsEnabled).GroupBy(cronTicker => MonitorKey.Of(cronTicker.Function)))
        {
            if (TryScheduleOf(monitor.Key, monitor.ToList(), timeZone, logger, out var schedule))
            {
                schedules.Add(schedule);
            }
        }

        return schedules;
    }

    private static bool TryScheduleOf(string monitorKey, IReadOnlyList<CronTickerEntity> cronTickers, TimeZoneInfo timeZone, ILogger logger, out MonitorSchedule schedule)
    {
        schedule = null;
        var expressions = new List<string>();
        foreach (var cronTicker in cronTickers)
        {
            if (!TickerQCron.TryConvert(cronTicker.Expression, out var expression))
            {
                logger.ScheduleNotSynced(monitorKey, $"Cronitor can't express the cron expression '{cronTicker.Expression}' of cron ticker {cronTicker.Id}");
                return false;
            }

            expressions.Add(expression);
        }

        if (!CronitorTimezone.TryGetIanaName(timeZone, out var ianaName))
        {
            logger.ScheduleNotSynced(monitorKey, $"TickerQ's time zone '{timeZone.Id}' has no IANA name");
            return false;
        }

        schedule = new MonitorSchedule(monitorKey, expressions.Distinct(), ianaName);
        return true;
    }
}
