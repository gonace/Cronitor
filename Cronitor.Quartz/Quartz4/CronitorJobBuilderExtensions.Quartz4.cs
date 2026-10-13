using Quartz;
using System;

namespace Cronitor.Quartz;

public static partial class CronitorJobBuilderExtensions
{
    /// <summary>
    /// Reports this job to the Cronitor monitor with the given key, instead of the job's name.
    /// Works on a <see cref="JobBuilder{T}"/>, and keeps it one so the chain can end with <c>Build()</c>.
    /// The key can't contain <c>/</c>, which Cronitor doesn't accept in a monitor key.
    /// </summary>
    public static JobBuilder<TJob> WithCronitorMonitorKey<TJob>(this JobBuilder<TJob> job, string monitorKey) where TJob : IJob
    {
        AddMonitorKey(job, monitorKey);
        return job;
    }

    /// <summary>
    /// Reports this job to the Cronitor monitor with the given key, instead of the job's name.
    /// Works on the <see cref="IJobConfigurator{T}"/> of <c>AddJob</c> and <c>ScheduleJob</c>.
    /// The key can't contain <c>/</c>, which Cronitor doesn't accept in a monitor key.
    /// </summary>
    public static IJobConfigurator<TJob> WithCronitorMonitorKey<TJob>(this IJobConfigurator<TJob> job, string monitorKey) where TJob : IJob
    {
        AddMonitorKey(job, monitorKey);
        return job;
    }

    private static void AddMonitorKey<TJob>(IJobConfigurator<TJob> job, string monitorKey) where TJob : IJob
    {
        if (job == null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        ValidateMonitorKey(monitorKey);
        job.UsingJobData(MonitorKey, monitorKey);
    }
}
