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
        foreach (var monitor in ByMonitorKey(cronTickers.Where(cronTicker => cronTicker.IsEnabled), logger))
        {
            if (TryScheduleOf(monitor.Key, monitor.ToList(), timeZone, logger, out var schedule))
            {
                schedules.Add(schedule);
            }
        }

        return schedules;
    }

    private static IEnumerable<IGrouping<MonitorKey, CronTickerEntity>> ByMonitorKey(IEnumerable<CronTickerEntity> cronTickers, ILogger logger)
    {
        var keyed = new List<(MonitorKey MonitorKey, CronTickerEntity CronTicker)>();
        foreach (var cronTicker in cronTickers)
        {
            if (MonitorKey.TryFromName(cronTicker.Function, out var monitorKey))
            {
                keyed.Add((monitorKey, cronTicker));
            }
            else
            {
                logger.ScheduleNotSynced(cronTicker.Function, $"the function name of cron ticker {cronTicker.Id} can't be a monitor key, which can't be blank or longer than 100 characters");
            }
        }

        return keyed.GroupBy(entry => entry.MonitorKey, entry => entry.CronTicker);
    }

    private static bool TryScheduleOf(MonitorKey monitorKey, IReadOnlyList<CronTickerEntity> cronTickers, TimeZoneInfo timeZone, ILogger logger, out MonitorSchedule schedule)
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
