using Cronitor.Scheduling;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cronitor.Quartz;

internal static class TriggerSchedules
{
    internal static IReadOnlyList<MonitorSchedule> Of(IEnumerable<(IJobDetail Job, ITrigger Trigger)> triggers, ILogger logger)
    {
        var schedules = new List<MonitorSchedule>();
        foreach (var monitor in ByMonitorKey(triggers, logger))
        {
            if (TryScheduleOf(monitor.Key, monitor.ToList(), logger, out var schedule))
            {
                schedules.Add(schedule);
            }
        }

        return schedules;
    }

    private static IEnumerable<IGrouping<MonitorKey, ITrigger>> ByMonitorKey(IEnumerable<(IJobDetail Job, ITrigger Trigger)> triggers, ILogger logger)
    {
        var keyed = new List<(MonitorKey MonitorKey, ITrigger Trigger)>();
        foreach (var (job, trigger) in triggers)
        {
            if (job.TryGetMonitorKey(trigger, out var monitorKey))
            {
                keyed.Add((monitorKey, trigger));
            }
            else
            {
                logger.ScheduleNotSynced(job.Key.ToString(), $"the monitor key of job {job.Key}, chosen or from its name, can't be blank, contain '/' or be longer than 100 characters");
            }
        }

        return keyed.GroupBy(entry => entry.MonitorKey, entry => entry.Trigger);
    }

    private static bool TryScheduleOf(MonitorKey monitorKey, IReadOnlyList<ITrigger> triggers, ILogger logger, out MonitorSchedule schedule)
    {
        schedule = null;
        var expressions = new List<string>();
        var timeZones = new List<TimeZoneInfo>();
        foreach (var trigger in triggers)
        {
            if (!TryExpressionOf(trigger, out var expression, out var timeZone, out var reason))
            {
                logger.ScheduleNotSynced(monitorKey, $"{reason} (trigger {trigger.Key})");
                return false;
            }

            expressions.Add(expression);
            if (timeZone != null)
            {
                timeZones.Add(timeZone);
            }
        }

        var timeZoneIds = timeZones.Select(timeZone => timeZone.Id).Distinct().ToList();
        if (timeZoneIds.Count > 1)
        {
            logger.ScheduleNotSynced(monitorKey, $"its cron triggers run in different time zones: {string.Join(", ", timeZoneIds)}");
            return false;
        }

        string ianaName = null;
        if (timeZones.Count > 0 && !CronitorTimezone.TryGetIanaName(timeZones[0], out ianaName))
        {
            logger.ScheduleNotSynced(monitorKey, $"the time zone '{timeZones[0].Id}' has no IANA name");
            return false;
        }

        schedule = new MonitorSchedule(monitorKey, expressions.Distinct(), ianaName);
        return true;
    }

    private static bool TryExpressionOf(ITrigger trigger, out string expression, out TimeZoneInfo timeZone, out string reason)
    {
        expression = null;
        timeZone = null;
        reason = null;
        if (trigger.CalendarName != null)
        {
            reason = $"the trigger skips the times its calendar '{trigger.CalendarName}' excludes, which Cronitor can't express";
            return false;
        }

        if (trigger.EndTimeUtc != null)
        {
            reason = $"the trigger stops at {trigger.EndTimeUtc.Value.ToString("u", CultureInfo.InvariantCulture)}, after which Cronitor would keep expecting runs";
            return false;
        }

        switch (trigger)
        {
            case ICronTrigger cronTrigger when QuartzCron.TryConvert(cronTrigger.CronExpressionString, out expression):
                timeZone = cronTrigger.TimeZone;
                return true;
            case ICronTrigger cronTrigger:
                reason = $"Cronitor can't express the cron expression '{cronTrigger.CronExpressionString}'";
                return false;
            case ISimpleTrigger simpleTrigger when simpleTrigger.RepeatCount >= 0:
                reason = "a simple trigger that stops repeating has no recurring schedule";
                return false;
            case ISimpleTrigger simpleTrigger when QuartzInterval.TryConvert(simpleTrigger.RepeatInterval, out expression):
                return true;
            case ISimpleTrigger simpleTrigger:
                reason = $"Cronitor can't express the interval {simpleTrigger.RepeatInterval}";
                return false;
            default:
                reason = $"Cronitor can't express a {trigger.GetType().Name}";
                return false;
        }
    }
}
