using Quartz;

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
}
