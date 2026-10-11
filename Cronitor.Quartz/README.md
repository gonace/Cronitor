# Cronitor.Quartz
Reports every [Quartz.NET](https://www.quartz-scheduler.net) job to [Cronitor](https://cronitor.io): run when the job starts, complete when it succeeds, and fail, with the exception's message, when it throws.

![Nuget](https://img.shields.io/nuget/v/Cronitor.Quartz)
![Nuget](https://img.shields.io/nuget/dt/Cronitor.Quartz)

## Install
```
dotnet add package Cronitor.Quartz
```
The package's major version matches Quartz's: version 3 is for Quartz 3 (3.22.4 or later).

## Setup
Register Cronitor with `.ConfigureCronitor()`, then call `UseCronitor()` when adding Quartz:
```c#
using Cronitor.Extensions;
using Cronitor.Quartz;
using Quartz;

await Host.CreateDefaultBuilder()
    .ConfigureCronitor((context) => context.Configuration.GetValue<string>("Cronitor:ApiKey"))
    .ConfigureServices(services =>
    {
        services.AddQuartz(q =>
        {
            q.UseCronitor();

            q.ScheduleJob<BackupJob>(
                trigger => trigger.WithCronSchedule("0 0 3 * * ?"),
                job => job.WithIdentity("backup").WithCronitorMonitorKey("nightly-backup"));
        });
        services.AddQuartzHostedService();
    })
    .Build()
    .RunAsync();
```
`AddQuartzHostedService()` comes from Quartz's own `Quartz.Extensions.Hosting` package.

## Monitor keys
A job reports to the monitor with the same key as the job's name. Use `.WithCronitorMonitorKey()` to report to a different monitor, as above.

## Errors
Reporting never affects your jobs: reports are sent in the background, so a job never waits for Cronitor. If a report can't be sent, the job still runs, and the error is logged through the host's logging. When the scheduler shuts down, it waits up to five seconds for reports that are still being sent, so the outcome of the last jobs isn't lost.

Jobs that are still running when the application stops are only reported if Quartz waits for them to finish. Quartz doesn't by default; to make it wait, set `WaitForJobsToComplete`:
```c#
services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
```

For everything else about the Cronitor client, such as logging and the API key, see the [Cronitor README](https://github.com/gonace/Cronitor#readme).
