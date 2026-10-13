using Coravel.Invocable;
using Coravel.Scheduling.Schedule.Interfaces;
using System;

namespace Cronitor.Coravel;

public static class CronitorSchedulerExtensions
{
    /// <summary>
    /// Schedules the job like <c>Schedule&lt;TInvocable&gt;()</c> and reports each run to the Cronitor monitor with the
    /// job's class name as its key: run when it starts, then complete when it succeeds or fail when it throws. Runs
    /// Coravel skips, because of <c>PreventOverlapping</c> or <c>When</c>, are not reported.
    /// </summary>
    public static IScheduleInterval ScheduleWithCronitor<TInvocable>(this IScheduler scheduler) where TInvocable : IInvocable =>
        scheduler.ScheduleWithCronitor<TInvocable>(typeof(TInvocable).Name);

    /// <summary>
    /// Schedules the job like <c>Schedule&lt;TInvocable&gt;()</c> and reports each run to the Cronitor monitor with
    /// the given key. The key can't contain <c>/</c>, which Cronitor doesn't accept in a monitor key.
    /// </summary>
    public static IScheduleInterval ScheduleWithCronitor<TInvocable>(this IScheduler scheduler, string monitorKey) where TInvocable : IInvocable
    {
        if (scheduler == null)
        {
            throw new ArgumentNullException(nameof(scheduler));
        }

        if (string.IsNullOrWhiteSpace(monitorKey))
        {
            throw new ArgumentException("A monitor key is required.", nameof(monitorKey));
        }

        if (monitorKey.IndexOf('/') >= 0)
        {
            throw new ArgumentException($"Cronitor doesn't accept '/' in a monitor key: '{monitorKey}'.", nameof(monitorKey));
        }

        return scheduler.ScheduleWithParams<CronitorInvocable<TInvocable>>(monitorKey);
    }
}
