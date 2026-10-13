using Quartz;
using System;

namespace Cronitor.Quartz;

/// <remarks>
/// This part is the same for every Quartz major; <c>WithCronitorMonitorKey</c>, whose job configurator type differs
/// between majors, is in the <c>Quartz3</c> and <c>Quartz4</c> folders.
/// </remarks>
public static partial class CronitorJobBuilderExtensions
{
    /// <summary>
    /// The <see cref="JobDataMap"/> key holding the Cronitor monitor key a job reports to.
    /// </summary>
    public const string MonitorKey = "Cronitor.MonitorKey";

    /// <summary>
    /// Throws unless the key can be used as given: it must not be blank, and Cronitor doesn't accept <c>/</c> in a monitor key.
    /// </summary>
    private static void ValidateMonitorKey(string monitorKey)
    {
        if (string.IsNullOrWhiteSpace(monitorKey))
        {
            throw new ArgumentException("A monitor key is required.", nameof(monitorKey));
        }

        if (monitorKey.IndexOf('/') >= 0)
        {
            throw new ArgumentException($"Cronitor doesn't accept '/' in a monitor key: '{monitorKey}'.", nameof(monitorKey));
        }
    }
}
