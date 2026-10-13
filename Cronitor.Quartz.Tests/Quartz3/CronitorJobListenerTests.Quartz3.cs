using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Quartz.Tests;

/// <summary>
/// The parts of the listener tests that use Quartz 3's API: creating a scheduler and the test jobs.
/// </summary>
public partial class CronitorJobListenerTests
{
    /// <summary>
    /// A scheduler of its own for each test, so tests don't share jobs or listeners.
    /// </summary>
    private static Task<IScheduler> NewScheduler()
    {
        var properties = new NameValueCollection { ["quartz.scheduler.instanceName"] = Guid.NewGuid().ToString("N") };

        return new StdSchedulerFactory(properties).GetScheduler(TestContext.Current.CancellationToken);
    }

    private static void AddJobListener(IScheduler scheduler, IJobListener listener) => scheduler.ListenerManager.AddJobListener(listener);

    /// <summary>
    /// Shuts the scheduler down, waiting for running jobs, as a task the caller can wait for with a time limit.
    /// </summary>
    private static Task ShutdownAsTask(IScheduler scheduler) => scheduler.Shutdown(true, TestContext.Current.CancellationToken);

    private sealed class SucceedingJob : IJob
    {
        private static TaskCompletionSource<bool> _ran = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public static Task Ran => _ran.Task;

        public static void Reset() => _ran = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Execute(IJobExecutionContext context)
        {
            _ran.TrySetResult(true);
            return Task.CompletedTask;
        }
    }

    private sealed class FailingJob : IJob
    {
        public Task Execute(IJobExecutionContext context) => throw new InvalidOperationException("Disk is full");
    }

    private sealed class InterruptibleJob : IJob
    {
        private static TaskCompletionSource<bool> _started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public static Task Started => _started.Task;

        public static void Reset() => _started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task Execute(IJobExecutionContext context)
        {
            _started.TrySetResult(true);
            await Task.Delay(TimeSpan.FromSeconds(30), context.CancellationToken);
        }
    }
}
