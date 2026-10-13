using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Quartz;

/// <summary>
/// Waits, when the scheduler shuts down, for the reports a <see cref="CronitorJobListener"/> is still sending,
/// so the outcome of the last jobs reaches Cronitor before the application stops. It waits at most five seconds,
/// and only for reports already being sent: jobs still running are only covered when Quartz waits for them to
/// finish, with <c>Shutdown(true)</c> or <c>WaitForJobsToComplete</c> in Quartz's hosting options.
/// <c>UseCronitor()</c> registers it; without dependency injection, add it next to the job listener with
/// <c>ListenerManager.AddSchedulerListener</c>. Like <see cref="CronitorJobListener"/>, it implements its method
/// explicitly, so a signature that doesn't match Quartz 4's is a compile error instead of a method never called.
/// </summary>
public class CronitorSchedulerListener : ISchedulerListener
{
    private readonly CronitorJobListener _jobListener;
    private readonly TimeSpan _limit;

    public CronitorSchedulerListener(CronitorJobListener jobListener)
        : this(jobListener, TimeSpan.FromSeconds(5))
    {
    }

    internal CronitorSchedulerListener(CronitorJobListener jobListener, TimeSpan limit)
    {
        _jobListener = jobListener ?? throw new ArgumentNullException(nameof(jobListener));
        _limit = limit;
    }

    ValueTask ISchedulerListener.SchedulerShutdown(IScheduler scheduler, CancellationToken cancellationToken) =>
        new ValueTask(_jobListener.WaitForPendingReportsAsync(_limit, cancellationToken));
}
