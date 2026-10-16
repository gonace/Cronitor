using Quartz;
using System;

namespace Cronitor.Quartz.Tests;

/// <summary>
/// The parts of the trigger schedule tests that use Quartz 4's API.
/// </summary>
public partial class TriggerSchedulesTests
{
    private static ITrigger CronTriggerWithCalendar(IJobDetail job, string calendar) =>
        TriggerBuilder.Create().ForJob(job).WithCronSchedule("0 0 3 * * ?", schedule => schedule.InTimeZone(TimeZoneInfo.Utc)).WithCalendarName(calendar).Build();
}
