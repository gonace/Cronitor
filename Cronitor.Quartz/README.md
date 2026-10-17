# Cronitor.Quartz
Reports every [Quartz.NET](https://www.quartz-scheduler.net) job to [Cronitor](https://cronitor.io): run when the job starts, complete when it succeeds, and fail, with the exception's message, when it throws.

![Nuget](https://img.shields.io/nuget/v/Cronitor.Quartz)
![Nuget](https://img.shields.io/nuget/dt/Cronitor.Quartz)

## Install
```
dotnet add package Cronitor.Quartz
```
The package's major version matches Quartz's:

| Cronitor.Quartz | Quartz | .NET |
| --- | --- | --- |
| 4.x | 4.0.0 or later | .NET 10 |
| 3.x | 3.22.4 or later | .NET 8, 9 and 10, and .NET Standard 2.0 |

`dotnet add package` picks the latest version, which is for Quartz 4. On Quartz 3, pick version 3: `dotnet add package Cronitor.Quartz --version 3.*`.

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
The setup is the same for Quartz 3 and 4. `AddQuartzHostedService()` comes from Quartz's own `Quartz.Extensions.Hosting` package, which in Quartz 4 is part of the `Quartz` package itself.

## Monitor keys
A job reports to the monitor with the same key as the job's name. Use `.WithCronitorMonitorKey()` to report to a different monitor, as above.

Cronitor doesn't accept `/` in a monitor key, so a `/` in a job's name becomes `-`: a job named `reports/daily` reports to `reports-daily`. Jobs named `reports/daily` and `reports-daily` would therefore share a monitor; give one of them `.WithCronitorMonitorKey()` to keep them apart. A key given to `.WithCronitorMonitorKey()` is used exactly as given, so it must follow Cronitor's [rules for monitor keys](https://github.com/gonace/Cronitor#telemetry); it throws if it doesn't. A monitor key can be at most 100 characters: a job whose key would be longer, or breaks Cronitor's rules in another way, isn't reported or synced, and the reason is logged.

A job scheduled without a name, such as `q.ScheduleJob<BackupJob>(trigger => …)` without `WithIdentity`, gets a new random name from Quartz on every start. Such a job reports under its class name instead, `BackupJob` here, so it keeps reporting to the same monitor. Two unnamed jobs of the same class share that monitor; give them names, or monitor keys, to keep them apart.

Quartz can't tell its own random names from a name you set to a GUID yourself, so a job you name with a GUID also reports under its class name. Give such jobs a monitor key of their own:
```c#
job => job.WithIdentity(tenantId.ToString()).WithCronitorMonitorKey($"sync-{tenantId}")
```

## Retries
Quartz 4 retries a failed job when its trigger, its job class or the scheduler has a retry policy, such as `.WithRetryPolicy(RetryPolicy.Fixed(3, TimeSpan.FromMinutes(5)))`. Each attempt is reported as its own run, and a retry's fail message ends with its attempt, for example `Disk is full (attempt 2)`. A job that fails twice and then succeeds is therefore reported as two failed runs followed by a completed one. Quartz 3 has no retry policies.

In both Quartz 3 and 4, a job that throws a `JobExecutionException` with `RefireImmediately = true` is run again at once. Quartz doesn't tell listeners why that attempt failed, so it's reported as a failed run with the message `The job failed and Quartz ran it again`, and the next attempt as a run of its own.

A job that Quartz interrupts with `Interrupt()` fails with `The job was interrupted`. A job interrupted because the scheduler shuts down isn't reported as failed: its run is abandoned with the message `The service went offline`, so a deploy or restart doesn't make the monitor fail, and Cronitor keeps the run open until the job runs again. Quartz only interrupts running jobs on shutdown when `quartz.scheduler.interruptJobsOnShutdown` (or, when it waits for jobs, `quartz.scheduler.interruptJobsOnShutdownWithWait`) is set; otherwise it lets them finish. The job has to stop when its cancellation token is cancelled. Quartz 3 doesn't say why a job was interrupted, so a job interrupted while the scheduler is paused with `Standby()` is reported as abandoned as well.

## Schedules
A monitor created by a job's first report has no schedule, so Cronitor alerts when the job fails but not when it doesn't run at all. To give each job's monitor the schedules of its triggers, add schedule sync:
```c#
services.AddQuartzHostedService();
services.AddCronitorQuartzScheduleSync();
```
Once the application has started, the triggers of every scheduler are sent to Cronitor in one request, in the background. Only the schedules and time zone are set; everything else on the monitor, such as its name and alerts, is kept. Jobs and triggers scheduled later get their schedules at the next start.

A cron trigger is synced with its own time zone, and a simple trigger that repeats forever as an interval, such as `every 5 minutes`. A schedule is only synced when Cronitor reads it the same way as Quartz: a cron expression whose seconds aren't 0, that's limited to certain years, or that uses something Cronitor doesn't support, such as the nearest weekday (`15W`), isn't synced, and neither is a simple trigger that stops repeating, or a trigger with a calendar or an end time, since Cronitor can't leave out the times a calendar excludes or stop expecting runs at an end time; the reason is logged. A paused trigger is left out, so a job gets the schedules of its other triggers, and a job whose triggers are all paused isn't synced. All the triggers of a monitor are synced together, as long as their cron triggers share a time zone. Schedule sync needs dependency injection.

## Errors
Reporting never affects your jobs: reports are sent in the background, so a job never waits for Cronitor. If a report can't be sent, the job still runs, and the error is logged through the host's logging. When the scheduler shuts down, it waits up to five seconds for reports that are still being sent, so the outcome of the last jobs isn't lost.

Jobs that are still running when the application stops are only reported if Quartz waits for them to finish. Quartz doesn't by default; to make it wait, set `WaitForJobsToComplete`:
```c#
services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
```

For everything else about the Cronitor client, such as logging and the API key, see the [Cronitor README](https://github.com/gonace/Cronitor#readme).
