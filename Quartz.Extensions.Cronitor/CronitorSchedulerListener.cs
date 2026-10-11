using Quartz.Listener;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz;

/// <summary>
/// Waits, when the scheduler shuts down, for the reports a <see cref="CronitorJobListener"/> is still sending,
/// so the outcome of the last jobs reaches Cronitor before the application stops. It waits at most five seconds.
/// Quartz calls it after the running jobs have finished. <c>UseCronitor()</c> registers it; without dependency
/// injection, add it next to the job listener with <c>ListenerManager.AddSchedulerListener</c>.
/// </summary>
public class CronitorSchedulerListener : SchedulerListenerSupport
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

    public override Task SchedulerShutdown(CancellationToken cancellationToken = default) =>
        _jobListener.WaitForPendingReports(_limit, cancellationToken);
}
