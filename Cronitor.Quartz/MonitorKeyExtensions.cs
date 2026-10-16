using Quartz;
using System;

namespace Cronitor.Quartz;

internal static partial class MonitorKeyExtensions
{
    internal static bool TryGetMonitorKey(this IJobDetail job, JobDataMap mergedData, out MonitorKey key) =>
        TryCreateMonitorKey(job, ChosenKeyIn(mergedData), out key);

    internal static bool TryGetMonitorKey(this IJobDetail job, ITrigger trigger, out MonitorKey key) =>
        TryCreateMonitorKey(job, ChosenKeyIn(trigger.JobDataMap) ?? ChosenKeyIn(job.JobDataMap), out key);

    private static bool TryCreateMonitorKey(IJobDetail job, string chosenKey, out MonitorKey key) =>
        chosenKey != null ? MonitorKey.TryCreate(chosenKey, out key) : MonitorKey.TryFromName(NameOf(job), out key);

    private static string ChosenKeyIn(JobDataMap data) =>
        data.TryGetValue(CronitorJobBuilderExtensions.MonitorKey, out var value) && value is string monitorKey && !string.IsNullOrWhiteSpace(monitorKey)
            ? monitorKey
            : null;

    private static string NameOf(IJobDetail job) => IsGeneratedName(job.Key.Name) ? ClassNameOf(job) : job.Key.Name;

    /// <summary>
    /// Quartz names a job built without an identity with a new GUID on every start, and records nothing that tells
    /// such a name from a GUID set on purpose, so every GUID name counts as generated.
    /// </summary>
    private static bool IsGeneratedName(string name) => Guid.TryParseExact(name, "D", out _);
}
