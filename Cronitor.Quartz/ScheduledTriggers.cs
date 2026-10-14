using Quartz;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cronitor.Quartz;

internal static partial class ScheduledTriggers
{
    internal static async Task<IReadOnlyList<(IJobDetail Job, ITrigger Trigger)>> ReadAsync(ISchedulerFactory schedulers)
    {
        var triggers = new List<(IJobDetail Job, ITrigger Trigger)>();
        foreach (var scheduler in await schedulers.GetAllSchedulers().ConfigureAwait(false))
        {
            foreach (var jobKey in await scheduler.GetJobKeys(AnyJob()).ConfigureAwait(false))
            {
                var job = await scheduler.GetJobDetail(jobKey).ConfigureAwait(false);
                if (job == null)
                {
                    continue;
                }

                foreach (var trigger in await scheduler.GetTriggersOfJob(jobKey).ConfigureAwait(false))
                {
                    triggers.Add((job, trigger));
                }
            }
        }

        return triggers;
    }
}
