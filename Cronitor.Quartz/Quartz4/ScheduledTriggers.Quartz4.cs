using Quartz;

namespace Cronitor.Quartz;

internal static partial class ScheduledTriggers
{
    private static GroupMatcher<JobKey> AnyJob() => GroupMatcher<JobKey>.AnyGroup();
}
