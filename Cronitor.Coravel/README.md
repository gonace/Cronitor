# Cronitor.Coravel
Reports [Coravel](https://docs.coravel.net) scheduled jobs to [Cronitor](https://cronitor.io): run when the job starts, complete when it succeeds, and fail, with the exception's message, when it throws.

![Nuget](https://img.shields.io/nuget/v/Cronitor.Coravel)
![Nuget](https://img.shields.io/nuget/dt/Cronitor.Coravel)

## Install
```
dotnet add package Cronitor.Coravel
```
The package's major version matches Coravel's: version 6 is for Coravel 6 (6.0.0 or later), on .NET 8, 9 and 10.

## Setup
Register Cronitor with `.ConfigureCronitor()` on the host builder and call `AddCronitorCoravel()` next to `AddScheduler()`. Then schedule each job you want reported with `ScheduleWithCronitor<TJob>()` instead of `Schedule<TJob>()`:
```c#
using Coravel;
using Cronitor.Coravel;
using Cronitor.Extensions;

var host = Host.CreateDefaultBuilder()
    .ConfigureCronitor((context) => context.Configuration.GetValue<string>("Cronitor:ApiKey"))
    .ConfigureServices(services =>
    {
        services.AddScheduler();
        services.AddCronitorCoravel();
        services.AddTransient<BackupJob>();
    })
    .Build();

host.Services.UseScheduler(scheduler =>
{
    scheduler.ScheduleWithCronitor<BackupJob>().DailyAt(3, 0);
});

await host.RunAsync();
```
`ScheduleWithCronitor<TJob>()` returns what `Schedule<TJob>()` returns, so the schedule and everything after it, such as `.PreventOverlapping()`, `.When()` and `.Zoned()`, work as usual. The job is created the same way too, so register it as you would for `Schedule<TJob>()`.

Jobs scheduled with `Schedule<TJob>()` are not reported, so you choose which jobs Cronitor monitors. Only invocable jobs can be reported: a job scheduled as a lambda, with `Schedule(() => …)`, has nothing that names it from one start of the application to the next.

## What is reported
Every run of the job is reported: run when it starts, then complete when it succeeds, or fail when it throws, with the exception's message. A job Coravel cancels when the application stops fails with "The job was cancelled". The job's exception still reaches Coravel, so Coravel's own error handling, such as `.OnError()`, works as before.

A run Coravel skips is not reported, since it never ran: a run skipped by `.PreventOverlapping()` because the previous run is still going, or by `.When()` because its condition is false.

## Monitor keys
A job reports to the monitor with the same key as its class name, `BackupJob` above. Give it a key of its own to report to a different monitor, or to keep two schedules of the same class apart:
```c#
scheduler.ScheduleWithCronitor<BackupJob>("nightly-backup").DailyAt(3, 0);
```
The key must follow Cronitor's [rules for monitor keys](https://github.com/gonace/Cronitor#telemetry), such as no `/` and at most 100 characters, so `ScheduleWithCronitor()` throws when given one that doesn't.

## Errors
Reporting never affects your jobs: the start of a run is reported in the background, so a job never waits for Cronitor. If a report can't be sent, the job still runs and keeps its result, and the error is logged through the host's logging. When the application stops, Coravel cancels the jobs still running and waits for them; reports still being sent are then waited for up to five seconds, so the outcome of the last jobs isn't lost.

Without `AddCronitorCoravel()`, a job scheduled with `ScheduleWithCronitor()` doesn't run: Coravel reports an error, through `.OnError()` and its log, that says to call it.

For everything else about the Cronitor client, such as logging and the API key, see the [Cronitor README](https://github.com/gonace/Cronitor#readme).
