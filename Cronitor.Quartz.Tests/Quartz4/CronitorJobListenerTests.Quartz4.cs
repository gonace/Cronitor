using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Quartz.Tests;

/// <summary>
/// The parts of the listener tests that use Quartz 4's API: creating a scheduler and the test jobs. Quartz 4 has no
/// <c>StdSchedulerFactory</c>, so each scheduler comes from dependency injection, like in an application.
/// </summary>
public partial class CronitorJobListenerTests
{
    /// <summary>
    /// A scheduler of its own for each test, so tests don't share jobs or listeners. Its service provider lives as long
    /// as the scheduler, which each test shuts down.
    /// </summary>
    private static async Task<IScheduler> NewScheduler()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddQuartz();

        return await services.BuildServiceProvider().GetRequiredService<ISchedulerFactory>().GetScheduler(TestContext.Current.CancellationToken);
    }

    /// <summary>
    /// A scheduler of its own that cancels the jobs still running when it's shut down without waiting for them.
    /// </summary>
    private static async Task<IScheduler> NewSchedulerThatInterruptsJobsOnShutdown()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddQuartz(new NameValueCollection { ["quartz.scheduler.interruptJobsOnShutdown"] = "true" }, _ => { });

        return await services.BuildServiceProvider().GetRequiredService<ISchedulerFactory>().GetScheduler(TestContext.Current.CancellationToken);
    }

    private static void AddJobListener(IScheduler scheduler, IJobListener listener) => scheduler.ListenerManager.AddJobListener(listener, [Matchers.AllJobs()]);

    /// <summary>
    /// Shuts the scheduler down, waiting for running jobs, as a task the caller can wait for with a time limit.
    /// </summary>
    private static Task ShutdownAsTask(IScheduler scheduler) => scheduler.Shutdown(true, TestContext.Current.CancellationToken).AsTask();

    private sealed class SucceedingJob : IJob
    {
        private static TaskCompletionSource<bool> _ran = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public static Task Ran => _ran.Task;

        public static void Reset() => _ran = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            _ran.TrySetResult(true);
            return default;
        }
    }

    private sealed class FailingJob : IJob
    {
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken) => throw new InvalidOperationException("Disk is full");
    }

    private sealed class RefiredOnceJob : IJob
    {
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken) =>
            context.RefireCount == 0 ? throw new JobExecutionException("Network is down") { RefireImmediately = true } : new ValueTask();
    }

    private sealed class RefiredOnceThenFailingJob : IJob
    {
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            if (context.RefireCount == 0)
            {
                throw new JobExecutionException("Network is down") { RefireImmediately = true };
            }

            throw new InvalidOperationException("Disk is full");
        }
    }

    private sealed class RefiredThenRekeyingJob : IJob
    {
        private static TaskCompletionSource<bool> _rekeyed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public static Task Rekeyed => _rekeyed.Task;

        public static void Reset() => _rekeyed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            if (context.RefireCount == 0)
            {
                throw new JobExecutionException("Network is down") { RefireImmediately = true };
            }

            context.MergedJobDataMap[CronitorJobBuilderExtensions.MonitorKey] = "other-key";
            _rekeyed.TrySetResult(true);
            return default;
        }
    }

    private sealed class InterruptibleJob : IJob
    {
        private static TaskCompletionSource<bool> _started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public static Task Started => _started.Task;

        public static void Reset() => _started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            _started.TrySetResult(true);
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        }
    }
}
