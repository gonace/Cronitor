using System;
using System.Collections.Generic;
using System.Linq;

namespace Cronitor.Scheduling;

/// <summary>
/// The schedules of one monitor, as <see cref="ScheduleSync"/> sends them to Cronitor: cron expressions such as
/// <c>0 3 * * *</c> or intervals such as <c>every 5 minutes</c>, evaluated in <see cref="Timezone"/>.
/// </summary>
public sealed class MonitorSchedule
{
    /// <param name="monitorKey">The key of the monitor.</param>
    /// <param name="schedules">One or more schedule expressions.</param>
    /// <param name="timezone">The IANA name of the time zone the schedules are evaluated in, see
    /// <see cref="CronitorTimezone"/>; null to keep the monitor's own, which is enough for intervals.</param>
    public MonitorSchedule(MonitorKey monitorKey, IEnumerable<string> schedules, string timezone = null)
    {
        var expressions = (schedules ?? throw new ArgumentNullException(nameof(schedules))).ToList();
        if (expressions.Count == 0 || expressions.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("At least one schedule is required, and none can be empty.", nameof(schedules));
        }

        MonitorKey = monitorKey ?? throw new ArgumentNullException(nameof(monitorKey));
        Schedules = expressions.AsReadOnly();
        Timezone = timezone;
    }

    public MonitorKey MonitorKey { get; }

    public IReadOnlyList<string> Schedules { get; }

    public string Timezone { get; }
}
