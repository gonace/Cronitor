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
        foreach (var monitor in recurringJobs.Where(job => !job.Removed).GroupBy(job => MonitorKey.Of(job.Id)))
        {
            if (TryScheduleOf(monitor.Key, monitor.ToList(), logger, out var schedule))
            {
                schedules.Add(schedule);
            }
        }

        return schedules;
    }

    private static bool TryScheduleOf(string monitorKey, IReadOnlyList<RecurringJobDto> recurringJobs, ILogger logger, out MonitorSchedule schedule)
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
        try
        {
            return CronitorTimezone.TryGetIanaName(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId), out ianaName);
        }
        catch (Exception exception) when (exception is TimeZoneNotFoundException || exception is InvalidTimeZoneException)
        {
            ianaName = null;
            return false;
        }
    }
}
