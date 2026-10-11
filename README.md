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
Cronitor.Configure("apiKey");

using (var run = await Cronitor.StartAsync("monitorKey"))
{
    // ... do the work
    await run.CompleteAsync();
}
```

## Setup
### Static
Configure the static `Cronitor` class once, then use it anywhere in your application.

Without a host:
```c#
Cronitor.Configure("apiKey");
```

With the .NET Generic Host (`Microsoft.Extensions.Hosting`):
```c#
await Host.CreateDefaultBuilder()
    .UseCronitor((context) => context.Configuration.GetValue<string>("Cronitor:ApiKey"))
    .Build()
    .RunAsync();
```

### Dependency injection
`.ConfigureCronitor()` registers `ITelemetriesClient`, `IMonitorsClient`, `IIssuesClient` and `INotificationsClient` in the service container instead. It does not configure the static `Cronitor` class.
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

All events except `Ping` take a monitor key and an optional `message` and `environment`.

Telemetry never breaks the job it monitors: transient failures are retried, and if an event still can't be delivered the method returns `false` instead of throwing.

Events that can't be delivered are logged as errors, see [Logging](#logging).

### Using block
`Cronitor.Start()` (or `ITelemetriesClient.Start()`) reports the run and returns an `ITelemetryRun`. Call `Complete()` when the job succeeds; if the run is disposed without it (for example because an exception was thrown), it's reported as failed.
```c#
using (var run = await Cronitor.StartAsync("monitorKey", environment: "production"))
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
    await Cronitor.RunAsync("monitorKey");
    // ... do the work
    await Cronitor.CompleteAsync("monitorKey");
}
catch (Exception ex)
{
    await Cronitor.FailAsync("monitorKey", ex.Message, environment: "production");
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

await Cronitor.PingAsync(command);
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

Without a host, pass an `ILoggerFactory`, either to the static `Cronitor` class or to a client:
```c#
using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

Cronitor.Configure("apiKey", loggerFactory);
// or
var telemetriesClient = new TelemetriesClient("apiKey", loggerFactory);
```
`AddConsole()` comes from the `Microsoft.Extensions.Logging.Console` package; any logging provider works. Without an `ILoggerFactory`, nothing is logged.

## Management
Monitors, issues and notifications are managed through `Cronitor.Monitors`, `Cronitor.Issues` and `Cronitor.Notifications` (or the injected `IMonitorsClient`, `IIssuesClient` and `INotificationsClient`). For example, creating a job monitor:
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

var monitors = await Cronitor.Monitors.CreateAsync(new CreateMonitorRequest(job));
```

`Assertion` and `Schedule` are fluent builders, so you don't have to write raw assertion or cron strings. See the [wiki](https://github.com/gonace/Cronitor/wiki) for all operations and options.

## Quartz.NET
`Quartz.Extensions.Cronitor` reports every Quartz.NET job to Cronitor. See its [README](Quartz.Extensions.Cronitor/README.md) for installation and setup.

## Hangfire
`Hangfire.Extensions.Cronitor` reports every Hangfire recurring job to Cronitor. See its [README](Hangfire.Extensions.Cronitor/README.md) for installation and setup.

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
