using Cronitor.Scheduling;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cronitor.Hangfire;

internal static class RecurringJobSchedules
{
    private const string DefaultTimeZoneId = "UTC";

    internal static IReadOnlyList<MonitorSchedule> Of(IEnumerable<RecurringJobDto> recurringJobs, ILogger logger)
    {
        var schedules = new List<MonitorSchedule>();
        foreach (var monitor in ByMonitorKey(recurringJobs.Where(job => !job.Removed), logger))
        {
            if (TryScheduleOf(monitor.Key, monitor.ToList(), logger, out var schedule))
            {
                schedules.Add(schedule);
            }
        }

        return schedules;
    }

    private static IEnumerable<IGrouping<MonitorKey, RecurringJobDto>> ByMonitorKey(IEnumerable<RecurringJobDto> recurringJobs, ILogger logger)
    {
        var keyed = new List<(MonitorKey MonitorKey, RecurringJobDto RecurringJob)>();
        foreach (var recurringJob in recurringJobs)
        {
            if (MonitorKey.TryFromName(recurringJob.Id, out var monitorKey))
            {
                keyed.Add((monitorKey, recurringJob));
            }
            else
            {
                logger.ScheduleNotSynced(recurringJob.Id, "the ID of the recurring job can't be a monitor key, which can't be blank or longer than 100 characters");
            }
        }

        return keyed.GroupBy(entry => entry.MonitorKey, entry => entry.RecurringJob);
    }

    private static bool TryScheduleOf(MonitorKey monitorKey, IReadOnlyList<RecurringJobDto> recurringJobs, ILogger logger, out MonitorSchedule schedule)
    {
        schedule = null;
        var expressions = new List<string>();
        foreach (var recurringJob in recurringJobs)
        {
            if (!HangfireCron.TryConvert(recurringJob.Cron, out var expression))
            {
                logger.ScheduleNotSynced(monitorKey, $"Cronitor can't express the cron expression '{recurringJob.Cron}' of recurring job {recurringJob.Id}");
                return false;
            }

            expressions.Add(expression);
        }

        var timeZoneIds = recurringJobs.Select(job => job.TimeZoneId ?? DefaultTimeZoneId).Distinct().ToList();
        if (timeZoneIds.Count > 1)
        {
            logger.ScheduleNotSynced(monitorKey, $"its recurring jobs run in different time zones: {string.Join(", ", timeZoneIds)}");
            return false;
        }

        if (!TryGetIanaName(timeZoneIds[0], out var timezone))
        {
            logger.ScheduleNotSynced(monitorKey, $"the time zone '{timeZoneIds[0]}' has no IANA name");
            return false;
        }

        schedule = new MonitorSchedule(monitorKey, expressions.Distinct(), timezone);
        return true;
    }

    private static bool TryGetIanaName(string timeZoneId, out string ianaName)
    {
        var timeZone = FindTimeZone(timeZoneId);
        if (timeZone == null)
        {
            ianaName = null;
            return false;
        }

        return CronitorTimezone.TryGetIanaName(timeZone, out ianaName);
    }

    private static TimeZoneInfo FindTimeZone(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return null;
        }
        catch (InvalidTimeZoneException)
        {
            return null;
        }
    }
}
