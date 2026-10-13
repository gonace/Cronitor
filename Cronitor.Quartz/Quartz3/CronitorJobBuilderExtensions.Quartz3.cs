using Quartz;
using System;

namespace Cronitor.Quartz;

public static partial class CronitorJobBuilderExtensions
{
    /// <summary>
    /// Reports this job to the Cronitor monitor with the given key, instead of the job's name.
    /// Works on a <see cref="JobBuilder"/> and on the <see cref="IJobConfigurator"/> of <c>AddJob</c> and <c>ScheduleJob</c>.
    /// The key can't contain <c>/</c>, which Cronitor doesn't accept in a monitor key.
    /// </summary>
    public static TJob WithCronitorMonitorKey<TJob>(this TJob job, string monitorKey) where TJob : IJobConfigurator
    {
        if (job == null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        ValidateMonitorKey(monitorKey);
        job.UsingJobData(MonitorKey, monitorKey);

        return job;
    }
}
