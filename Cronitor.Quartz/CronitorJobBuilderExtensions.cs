using Quartz;
using System;

namespace Cronitor.Quartz;

public static class CronitorJobBuilderExtensions
{
    /// <summary>
    /// The <see cref="JobDataMap"/> key holding the Cronitor monitor key a job reports to.
    /// </summary>
    public const string MonitorKey = "Cronitor.MonitorKey";

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

        if (string.IsNullOrWhiteSpace(monitorKey))
        {
            throw new ArgumentException("A monitor key is required.", nameof(monitorKey));
        }

        if (monitorKey.Contains("/"))
        {
            throw new ArgumentException($"Cronitor doesn't accept '/' in a monitor key: '{monitorKey}'.", nameof(monitorKey));
        }

        job.UsingJobData(MonitorKey, monitorKey);

        return job;
    }
}
