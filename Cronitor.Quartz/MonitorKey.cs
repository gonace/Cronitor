using Quartz;
using System;

namespace Cronitor.Quartz;

internal static partial class MonitorKey
{
    internal static string Of(IJobDetail job, JobDataMap mergedData) => ChosenKeyIn(mergedData) ?? NameOf(job);

    internal static string Of(IJobDetail job, ITrigger trigger) => ChosenKeyIn(trigger.JobDataMap) ?? ChosenKeyIn(job.JobDataMap) ?? NameOf(job);

    private static string ChosenKeyIn(JobDataMap data) =>
        data.TryGetValue(CronitorJobBuilderExtensions.MonitorKey, out var value) && value is string monitorKey && !string.IsNullOrWhiteSpace(monitorKey)
            ? monitorKey
            : null;

    private static string NameOf(IJobDetail job)
    {
        var name = job.Key.Name;
        return IsGeneratedName(name) ? ClassNameOf(job) : name.Replace('/', '-');
    }

    /// <summary>
    /// Quartz names a job built without an identity with a new GUID on every start, and records nothing that tells
    /// such a name from a GUID set on purpose, so every GUID name counts as generated.
    /// </summary>
    private static bool IsGeneratedName(string name) => Guid.TryParseExact(name, "D", out _);
}
