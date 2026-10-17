using Quartz;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Quartz;

/// <summary>
/// The methods Quartz 3 calls, with Quartz 3's signatures.
/// </summary>
public partial class CronitorJobListener : IJobListener
{
    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        OnJobToBeExecuted(context);
        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
    {
        OnJobWasExecuted(context, jobException);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Quartz 3 doesn't retry failed jobs, so every run is a first attempt.
    /// </summary>
    private static int RetryOf(IJobExecutionContext context) => 0;

    /// <summary>
    /// Quartz 3 puts the scheduler in standby before it cancels the running jobs on shutdown, and only marks it shut
    /// down after them.
    /// </summary>
    private static bool IsShuttingDown(IScheduler scheduler) => scheduler.InStandbyMode;
}
