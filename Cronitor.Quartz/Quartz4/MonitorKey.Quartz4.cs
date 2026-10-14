using Quartz;

namespace Cronitor.Quartz;

internal static partial class MonitorKey
{
    private static string ClassNameOf(IJobDetail job) => job.JobType.Type.Name;
}
