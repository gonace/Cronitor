using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cronitor.Coravel.Tests;

/// <summary>
/// Runs jobs scheduled with <c>ScheduleWithCronitor</c> on a real Coravel scheduler and checks what is reported to Cronitor.
/// </summary>
public class ScheduleWithCronitorTests
{
    private static readonly TimeSpan Limit = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Should_send_run_and_complete_when_a_job_succeeds()
    {
        var telemetries = new FakeTelemetriesClient();

        await using (await TestHost.StartAsync(telemetries, scheduler => scheduler.ScheduleWithCronitor<SucceedingJob>().EverySecond().Once()))
        {
            await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));
        }

        Assert.Equal(new[] { "run SucceedingJob", "complete SucceedingJob" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_send_run_and_fail_with_the_message_and_still_pass_the_exception_to_Coravel_when_a_job_throws()
    {
        var telemetries = new FakeTelemetriesClient();

        await using (var host = await TestHost.StartAsync(telemetries, scheduler => scheduler.ScheduleWithCronitor<FailingJob>().EverySecond().Once()))
        {
            await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));
            await host.StopAsync();

            Assert.Equal(new[] { "run FailingJob", "fail FailingJob: Disk is full" }, telemetries.Sent);
            Assert.Equal("Disk is full", Assert.Single(host.Errors).Message);
        }
    }

    [Fact]
    public async Task Should_report_to_the_monitor_key_given_to_ScheduleWithCronitor()
    {
        var telemetries = new FakeTelemetriesClient();

        await using (await TestHost.StartAsync(telemetries, scheduler => scheduler.ScheduleWithCronitor<SucceedingJob>("nightly-backup").EverySecond().Once()))
        {
            await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));
        }

        Assert.Equal(new[] { "run nightly-backup", "complete nightly-backup" }, telemetries.Sent);
    }

    [Theory]
    [InlineData("reports/daily")]
    [InlineData(" ")]
    public void Should_throw_when_ScheduleWithCronitor_is_given_a_monitor_key_Cronitor_does_not_accept(string monitorKey)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScheduler();
        using (var provider = services.BuildServiceProvider())
        {
            var scheduler = provider.GetRequiredService<IScheduler>();

            Assert.ThrowsAny<ArgumentException>(() => scheduler.ScheduleWithCronitor<SucceedingJob>(monitorKey));
        }
    }

    [Fact]
    public void Should_throw_when_ScheduleWithCronitor_is_given_a_monitor_key_longer_than_100_characters()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScheduler();
        using (var provider = services.BuildServiceProvider())
        {
            var scheduler = provider.GetRequiredService<IScheduler>();

            Assert.Throws<ArgumentException>(() => scheduler.ScheduleWithCronitor<SucceedingJob>(new string('x', 101)));
        }
    }

    [Theory]
    [InlineData(" nightly-backup")]
    [InlineData("nightly-backup ")]
    public void Should_throw_when_ScheduleWithCronitor_is_given_a_monitor_key_with_surrounding_whitespace(string monitorKey)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScheduler();
        using (var provider = services.BuildServiceProvider())
        {
            var scheduler = provider.GetRequiredService<IScheduler>();

            Assert.Throws<ArgumentException>(() => scheduler.ScheduleWithCronitor<SucceedingJob>(monitorKey));
        }
    }

    [Fact]
    public void Should_throw_when_ScheduleWithCronitor_is_given_no_monitor_key()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScheduler();
        using (var provider = services.BuildServiceProvider())
        {
            var scheduler = provider.GetRequiredService<IScheduler>();

            Assert.Throws<ArgumentNullException>(() => scheduler.ScheduleWithCronitor<SucceedingJob>((MonitorKey)null));
        }
    }

    /// <summary>
    /// Coravel's own events report a run it skips because the previous run is still going as a run that started and
    /// ended, which is why Cronitor.Coravel wraps the job instead.
    /// </summary>
    [Fact]
    public async Task Should_only_report_the_runs_that_happened_when_PreventOverlapping_skips_runs()
    {
        var telemetries = new FakeTelemetriesClient();
        int runs;

        await using (var host = await TestHost.StartAsync(telemetries, scheduler => scheduler.ScheduleWithCronitor<SlowJob>().EverySecond().PreventOverlapping("slow")))
        {
            await host.Jobs.WaitForRunsOf<SlowJob>(2);
            await host.StopAsync();
            runs = host.Jobs.RunsOf<SlowJob>();
        }

        Assert.True(runs >= 2, $"The job ran {runs} times.");
        Assert.Equal(runs, telemetries.Sent.Count(sent => sent == "run SlowJob"));
        Assert.Equal(runs, telemetries.Sent.Count(sent => sent == "complete SlowJob"));
    }

    [Fact]
    public async Task Should_not_report_a_run_that_When_skips()
    {
        var telemetries = new FakeTelemetriesClient();

        await using (var host = await TestHost.StartAsync(telemetries, scheduler =>
        {
            scheduler.ScheduleWithCronitor<FailingJob>().EverySecond().When(() => Task.FromResult(false));
            scheduler.ScheduleWithCronitor<SucceedingJob>().EverySecond().Once();
        }))
        {
            await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));
            await host.StopAsync();

            Assert.Equal(0, host.Jobs.RunsOf<FailingJob>());
        }

        Assert.Equal(new[] { "run SucceedingJob", "complete SucceedingJob" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_pass_Coravel_s_cancellation_to_a_cancellable_job_and_abandon_the_run_when_the_application_stops()
    {
        var telemetries = new FakeTelemetriesClient();

        await using (var host = await TestHost.StartAsync(telemetries, scheduler => scheduler.ScheduleWithCronitor<CancellableJob>().EverySecond().Once()))
        {
            await Task.WhenAny(host.Jobs.Started, Task.Delay(Limit, TestContext.Current.CancellationToken));
            await host.StopAsync();
            await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));
        }

        Assert.Equal(new[] { "run CancellableJob", "abandon CancellableJob: The service went offline" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_run_the_job_without_waiting_for_the_start_to_be_reported()
    {
        var startGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var telemetries = new FakeTelemetriesClient { StartGate = startGate.Task };

        await using (var host = await TestHost.StartAsync(telemetries, scheduler => scheduler.ScheduleWithCronitor<SucceedingJob>().EverySecond().Once()))
        {
            await Task.WhenAny(host.Jobs.Started, Task.Delay(Limit, TestContext.Current.CancellationToken));
            var sentWhileStartWasHeld = telemetries.Sent.ToArray();
            startGate.SetResult(true);
            await Task.WhenAny(telemetries.Finished, Task.Delay(Limit, TestContext.Current.CancellationToken));

            Assert.True(host.Jobs.Started.IsCompleted, "The job waited for the start report.");
            Assert.Empty(sentWhileStartWasHeld);
        }

        Assert.Equal(new[] { "run SucceedingJob", "complete SucceedingJob" }, telemetries.Sent);
    }

    [Fact]
    public async Task Should_still_run_the_job_and_log_the_error_when_the_start_report_fails()
    {
        var telemetries = new FakeTelemetriesClient { ThrowOnStart = true };
        var logger = new FakeLogger<CronitorReporter>();

        await using (var host = await TestHost.StartAsync(
            telemetries,
            scheduler => scheduler.ScheduleWithCronitor<SucceedingJob>().EverySecond().Once(),
            services => services.AddCronitorCoravel().AddSingleton<ILogger<CronitorReporter>>(logger)))
        {
            await host.Jobs.WaitForRunsOf<SucceedingJob>(1);
            await host.StopAsync();

            Assert.Equal(1, host.Jobs.RunsOf<SucceedingJob>());
            Assert.Empty(host.Errors);
        }

        Assert.Empty(telemetries.Sent);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Start failed", entry.Exception.Message);
    }

    [Fact]
    public async Task Should_tell_Coravel_to_call_AddCronitorCoravel_when_it_was_not_called()
    {
        var telemetries = new FakeTelemetriesClient();

        await using (var host = await TestHost.StartAsync(telemetries, scheduler => scheduler.ScheduleWithCronitor<SucceedingJob>().EverySecond().Once(), _ => { }))
        {
            var limit = DateTime.UtcNow.Add(Limit);
            while (host.Errors.IsEmpty && DateTime.UtcNow < limit)
            {
                await Task.Delay(50, TestContext.Current.CancellationToken);
            }

            Assert.Contains("AddCronitorCoravel()", Assert.Single(host.Errors).Message);
        }
    }

    [Fact]
    public void Should_throw_when_Cronitor_is_not_configured()
    {
        var services = new ServiceCollection();
        services.AddCronitorCoravel();

        using (var provider = services.BuildServiceProvider())
        {
            var exception = Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<CronitorReporter>());

            Assert.Contains("ITelemetriesClient", exception.Message);
        }
    }
}
