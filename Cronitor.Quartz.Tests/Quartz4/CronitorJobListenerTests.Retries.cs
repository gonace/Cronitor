using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Quartz.Tests;

/// <summary>
/// Quartz 4 retries a failed job when its trigger has a retry policy, which Quartz 3 can't.
/// </summary>
public partial class CronitorJobListenerTests
{
    [Fact]
    public async Task Should_report_each_attempt_as_a_run_and_end_the_fail_message_of_a_retry_with_its_attempt()
    {
        var telemetries = new FakeTelemetriesClient();
        var listener = new CronitorJobListener(telemetries);
        FlakyJob.Reset(failures: 2);
        var scheduler = await NewScheduler();
        AddJobListener(scheduler, listener);
        scheduler.ListenerManager.AddSchedulerListener(new CronitorSchedulerListener(listener));
        await scheduler.Start(TestContext.Current.CancellationToken);

        var job = JobBuilder.Create<FlakyJob>().WithIdentity("nightly-backup").Build();
        var trigger = TriggerBuilder.Create().StartNow().WithRetryPolicy(RetryPolicy.Fixed(2, TimeSpan.FromSeconds(1))).Build();
        await scheduler.ScheduleJob(job, trigger, cancellationToken: TestContext.Current.CancellationToken);
        await Task.WhenAny(FlakyJob.Succeeded, Task.Delay(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));
        await scheduler.Shutdown(true, TestContext.Current.CancellationToken);

        Assert.Equal(new[]
        {
            "run nightly-backup", "fail nightly-backup: Disk is full",
            "run nightly-backup", "fail nightly-backup: Disk is full (attempt 2)",
            "run nightly-backup", "complete nightly-backup",
        }, telemetries.Sent);
    }

    /// <summary>
    /// Fails a chosen number of times, then succeeds.
    /// </summary>
    private sealed class FlakyJob : IJob
    {
        private static int _failuresLeft;
        private static TaskCompletionSource<bool> _succeeded = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public static Task Succeeded => _succeeded.Task;

        public static void Reset(int failures)
        {
            _failuresLeft = failures;
            _succeeded = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            if (Interlocked.Decrement(ref _failuresLeft) >= 0)
            {
                throw new InvalidOperationException("Disk is full");
            }

            _succeeded.TrySetResult(true);
            return default;
        }
    }
}
