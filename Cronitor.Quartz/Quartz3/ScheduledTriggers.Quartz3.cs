using Quartz;
using Quartz.Impl.Matchers;

namespace Cronitor.Quartz;

internal static partial class ScheduledTriggers
{
    private static GroupMatcher<JobKey> AnyJob() => GroupMatcher<JobKey>.AnyGroup();
}
