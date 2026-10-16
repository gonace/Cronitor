using Quartz;
using System;

namespace Cronitor.Quartz;

public static partial class CronitorJobBuilderExtensions
{
    /// <summary>
    /// Reports this job to the Cronitor monitor with the given key, instead of the job's name.
    /// Works on a <see cref="JobBuilder"/> and on the <see cref="IJobConfigurator"/> of <c>AddJob</c> and <c>ScheduleJob</c>.
    /// </summary>
    public static TJob WithCronitorMonitorKey<TJob>(this TJob job, MonitorKey monitorKey) where TJob : IJobConfigurator
    {
        if (job == null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        if (monitorKey == null)
        {
            throw new ArgumentNullException(nameof(monitorKey));
        }

        job.UsingJobData(MonitorKey, monitorKey.ToString());

        return job;
    }
}
