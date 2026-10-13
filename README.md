# Cronitor API Client
Cronitor is a service for heartbeat-style monitoring of anything that can send an HTTP request. It's particularly well suited for monitoring cron jobs, Jenkins jobs, or any other scheduled task.

![Nuget](https://img.shields.io/nuget/v/Cronitor)
![Nuget](https://img.shields.io/nuget/dt/Cronitor)

This .NET library has two parts:
* **Telemetry**: report when your jobs run, complete or fail ([Telemetry API](https://cronitor.io/docs/telemetry-api)).
* **Management**: create, read, update and delete monitors, issues and notifications ([Monitor API](https://cronitor.io/docs/monitor-api), [Issues API](https://cronitor.io/docs/issues-api), [Notifications API](https://cronitor.io/docs/template-api), [Activity API](https://cronitor.io/docs/activity-api)).

> For the full documentation, please read our [wiki](https://github.com/gonace/Cronitor/wiki).

## Install
```
dotnet add package Cronitor
```
Or download it from [NuGet](https://www.nuget.org/packages/Cronitor).

## Quick start
```c#
CronitorClient.Configure("apiKey");

using (var run = await CronitorClient.StartAsync("monitorKey"))
{
    // ... do the work
    await run.CompleteAsync();
}
```

## Setup
### Static
Configure the static `CronitorClient` once, then use it anywhere in your application. It used to be called `Cronitor`; that name still works but is obsolete, since it shares its name with the namespace and can only be called as `Cronitor.Cronitor`.

Without a host:
```c#
CronitorClient.Configure("apiKey");
```

With the .NET Generic Host (`Microsoft.Extensions.Hosting`):
```c#
await Host.CreateDefaultBuilder()
    .UseCronitor((context) => context.Configuration.GetValue<string>("Cronitor:ApiKey"))
    .Build()
    .RunAsync();
```

### Dependency injection
`.ConfigureCronitor()` registers `ITelemetriesClient`, `IMonitorsClient`, `IIssuesClient` and `INotificationsClient` in the service container instead. It does not configure the static `CronitorClient`.
```c#
await Host.CreateDefaultBuilder()
    .ConfigureCronitor((context) => context.Configuration.GetValue<string>("Cronitor:ApiKey"))
    .Build()
    .RunAsync();
```

Both `.UseCronitor()` and `.ConfigureCronitor()` also accept the API key as a plain string.

## Telemetry
Telemetry events report the state of a job to a monitor. Each event has a sync and an async (`…Async`) method:

| Method | Event |
|---|---|
| `Run` | The job has started |
| `Complete` | The job has finished successfully |
| `Fail` | The job has failed |
| `Tick` | A heartbeat, for monitors without a start/end |
| `Ping` | Sends a custom `Command` |

All events except `Ping` take a monitor key and an optional `message` and `environment`. A monitor that doesn't exist yet is created by its first event. Monitor keys may contain any character except `/`, which Cronitor doesn't accept in a monitor key; an event for such a key can't be delivered.

Telemetry never breaks the job it monitors: transient failures are retried, and if an event still can't be delivered the method returns `false` instead of throwing.

Events that can't be delivered are logged as errors, see [Logging](#logging).

### Using block
`CronitorClient.Start()` (or `ITelemetriesClient.Start()`) reports the run and returns an `ITelemetryRun`. Call `Complete()` when the job succeeds; if the run is disposed without it (for example because an exception was thrown), it's reported as failed.
```c#
using (var run = await CronitorClient.StartAsync("monitorKey", environment: "production"))
{
    // ... do the work
    await run.CompleteAsync();
}
```
Every run gets its own `series`, so overlapping runs of the same monitor are matched correctly. Use `run.Fail(message)` to report a failure with a reason.

### Static
```c#
try
{
    await CronitorClient.RunAsync("monitorKey");
    // ... do the work
    await CronitorClient.CompleteAsync("monitorKey");
}
catch (Exception ex)
{
    await CronitorClient.FailAsync("monitorKey", ex.Message, environment: "production");
    throw;
}
```

### Dependency injection
```c#
public class SomeJob
{
    private readonly ITelemetriesClient _telemetriesClient;

    public SomeJob(ITelemetriesClient telemetriesClient)
    {
        _telemetriesClient = telemetriesClient;
    }

    public async Task ExecuteAsync()
    {
        using (var run = await _telemetriesClient.StartAsync("monitorKey"))
        {
            // ... do the work
            await run.CompleteAsync();
        }
    }
}
```

### BackgroundService
A `BackgroundService` usually repeats its work in a loop. Report each pass through the loop as a run of its own:
```c#
public class CleanupService : BackgroundService
{
    private readonly ITelemetriesClient _telemetries;
    private readonly ILogger<CleanupService> _logger;

    public CleanupService(ITelemetriesClient telemetries, ILogger<CleanupService> logger)
    {
        _telemetries = telemetries;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
        do
        {
            using (var run = await _telemetries.StartAsync("cleanup"))
            {
                try
                {
                    // ... do the work
                    await run.CompleteAsync();
                }
                catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
                {
                    await run.FailAsync(exception.Message);
                    _logger.LogError(exception, "Cleanup failed");
                }
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
```
Register it next to Cronitor:
```c#
await Host.CreateDefaultBuilder()
    .ConfigureCronitor((context) => context.Configuration.GetValue<string>("Cronitor:ApiKey"))
    .ConfigureServices(services => services.AddHostedService<CleanupService>())
    .Build()
    .RunAsync();
```
* A failed pass doesn't stop the service: the exception is reported with its message and logged, and the loop continues. Without the `catch`, an exception in `ExecuteAsync` stops the whole application by default.
* A pass interrupted by shutdown is reported as failed, since its run is disposed without being completed.
* Cronitor only alerts when the work stops if the monitor has a schedule. Give it the loop's interval, in the Cronitor dashboard or with `Schedule.Every(5).Minutes` through [Management](#management). `PeriodicTimer` starts a pass every five minutes, as long as each pass takes less than that; a pass that runs longer is followed by the next one right away. `Task.Delay` instead waits five minutes after each pass, so the interval grows with the time the work takes.

### Custom commands
Use `Ping` when you need more than a message and environment, such as metrics, host or series:
```c#
using Cronitor.Commands;
using Cronitor.Constants;

var command = new CompleteCommand()
    .WithApiKey("apiKey")
    .WithMonitorKey("monitorKey")
    .WithHost("worker-01")
    .WithMetric(Metric.Count, 42)
    .WithMetric(Metric.Duration, 12.5);

await CronitorClient.PingAsync(command);
```

### Logging
Events that can't be delivered, even after retries, are logged as errors through `ILogger` (`Microsoft.Extensions.Logging`). With the console logger, an entry starts like this, followed by the exception:
```
fail: Cronitor.TelemetriesClient[1]
      Could not deliver run event for monitor nightly-backup to Cronitor
```

With the .NET Generic Host, both `.ConfigureCronitor()` and `.UseCronitor()` use the host's logging automatically, so there is nothing to set up. For `.UseCronitor()`, logging applies from when the host starts:
```c#
await Host.CreateDefaultBuilder()
    .UseCronitor("apiKey")
    .Build()
    .RunAsync();
```

Without a host, pass an `ILoggerFactory`, either to the static `CronitorClient` or to a client:
```c#
using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

CronitorClient.Configure("apiKey", loggerFactory);
// or
var telemetriesClient = new TelemetriesClient("apiKey", loggerFactory);
```
`AddConsole()` comes from the `Microsoft.Extensions.Logging.Console` package; any logging provider works. Without an `ILoggerFactory`, nothing is logged.

## Management
Monitors, issues and notifications are managed through `CronitorClient.Monitors`, `CronitorClient.Issues` and `CronitorClient.Notifications` (or the injected `IMonitorsClient`, `IIssuesClient` and `INotificationsClient`). For example, creating a job monitor:
```c#
using Cronitor.Assertions;
using Cronitor.Models.Monitors;
using Cronitor.Requests;
using Cronitor.Scheduling;

var job = new Job("nightly-backup")
{
    Schedule = Schedule.Cron.Daily(hour: 3),
    Assertions = new[]
    {
        Assertion.Metric.Duration.LessThan("30m"),
    }
};

var monitors = await CronitorClient.Monitors.CreateAsync(new CreateMonitorRequest(job));
```

`Assertion` and `Schedule` are fluent builders, so you don't have to write raw assertion or cron strings. See the [wiki](https://github.com/gonace/Cronitor/wiki) for all operations and options.

## Integrations
These packages report a scheduler's jobs to Cronitor without any reporting code in the jobs themselves. See each package's README for installation and setup.

| Package | Scheduler | Reports |
| --- | --- | --- |
| [`Cronitor.Quartz`](Cronitor.Quartz/README.md) | [Quartz.NET](https://www.quartz-scheduler.net) | every job |
| [`Cronitor.Hangfire`](Cronitor.Hangfire/README.md) | [Hangfire](https://www.hangfire.io) | every recurring job |
| [`Cronitor.TickerQ`](Cronitor.TickerQ/README.md) | [TickerQ](https://tickerq.net) | every cron job |
| [`Cronitor.Coravel`](Cronitor.Coravel/README.md) | [Coravel](https://docs.coravel.net) | every job scheduled with `ScheduleWithCronitor` |

## Development
### Suggestions
* Implement Timezone constant (if not too big of a hassle to maintain)


## Contributing
Pull requests and features are happily considered! By participating in this project you agree to abide by the [Code of Conduct](http://contributor-covenant.org/version/2/0).

### To contribute

Fork, then clone the repo:
```
git clone git@github.com:your-username/Cronitor.git
```
Push to your fork and [submit a pull request](https://github.com/gonace/Cronitor/compare/)
