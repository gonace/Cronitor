using Quartz;
using Xunit;

namespace Cronitor.Quartz.Tests;

public class MonitorKeyExtensionsTests
{
    [Fact]
    public void Should_use_the_job_s_chosen_key_when_the_trigger_has_a_blank_one()
    {
        var job = JobBuilder.Create<NoopJob>().WithIdentity("payroll").UsingJobData(CronitorJobBuilderExtensions.MonitorKey, "monthly-payroll").Build();
        var mergedData = new JobDataMap { [CronitorJobBuilderExtensions.MonitorKey] = "" };

        var found = job.TryGetMonitorKey(mergedData, out var key);

        Assert.True(found);
        Assert.Equal("monthly-payroll", key.ToString());
    }
}
