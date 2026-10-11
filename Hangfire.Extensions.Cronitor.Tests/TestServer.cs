using Hangfire.Common;
using Hangfire.InMemory;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Hangfire.Extensions.Cronitor.Tests;

/// <summary>
/// Runs jobs on a real Hangfire server with in-memory storage.
/// </summary>
public static class TestServer
{
    /// <summary>
    /// Adds a recurring job, triggers it, and waits until Hangfire has recorded whether it succeeded or failed.
    /// </summary>
    public static async Task<InMemoryStorage> RunRecurringJob(string recurringJobId, Expression<Action> job, IJobFilterProvider filters)
    {
        var storage = new InMemoryStorage();

        using (Start(storage, filters))
        {
            var recurringJobs = new RecurringJobManager(storage);
            recurringJobs.AddOrUpdate(recurringJobId, job, Cron.Never());
            recurringJobs.Trigger(recurringJobId);

            await WaitUntil(() => HasFinished(storage));
        }

        return storage;
    }

    public static BackgroundJobServer Start(JobStorage storage, IJobFilterProvider filters)
    {
        var options = new BackgroundJobServerOptions
        {
            WorkerCount = 1,
            SchedulePollingInterval = TimeSpan.FromMilliseconds(100),
            FilterProvider = filters
        };

        return new BackgroundJobServer(options, storage);
    }

    public static async Task WaitUntil(Func<bool> condition)
    {
        var stopwatch = Stopwatch.StartNew();
        while (!condition() && stopwatch.Elapsed < TimeSpan.FromSeconds(10))
        {
            await Task.Delay(50, TestContext.Current.CancellationToken);
        }
    }

    private static bool HasFinished(JobStorage storage)
    {
        var monitoring = storage.GetMonitoringApi();

        return monitoring.SucceededListCount() + monitoring.FailedCount() > 0;
    }
}

/// <summary>
/// Jobs for the tests. Hangfire runs them from their method expression, so they are static.
/// </summary>
public static class TestJobs
{
    public static void Succeed()
    {
    }

    public static void Throw() => throw new InvalidOperationException("Disk is full");
}
