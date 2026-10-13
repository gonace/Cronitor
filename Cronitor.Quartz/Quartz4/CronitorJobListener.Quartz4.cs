using Quartz;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Quartz;

/// <summary>
/// The methods Quartz 4 calls, with Quartz 4's signatures. Every <see cref="IJobListener"/> method has a default
/// implementation in Quartz 4, so a method with the wrong signature would compile and simply never be called; they
/// are implemented explicitly, which turns a wrong signature into a compile error.
/// </summary>
public partial class CronitorJobListener : IJobListener
{
    ValueTask IJobListener.JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        OnJobToBeExecuted(context);
        return default;
    }

    ValueTask IJobListener.JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken)
    {
        OnJobWasExecuted(context, jobException);
        return default;
    }

    private static string ClassNameOf(IJobDetail job) => job.JobType.Type.Name;
}
