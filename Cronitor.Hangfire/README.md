# Cronitor.Hangfire
Reports every [Hangfire](https://www.hangfire.io) recurring job to [Cronitor](https://cronitor.io): run when the job starts, complete when it succeeds, and fail, with the exception's message, when it throws.

![Nuget](https://img.shields.io/nuget/v/Cronitor.Hangfire)
![Nuget](https://img.shields.io/nuget/dt/Cronitor.Hangfire)

## Install
```
dotnet add package Cronitor.Hangfire
```
The package's major version matches Hangfire's: version 1 is for Hangfire 1 (1.8.25 or later).

## Setup
### Dependency injection
Register Cronitor with `.ConfigureCronitor()`, then call `UseCronitor(provider)` when adding Hangfire:
```c#
using Cronitor.Extensions;
using Cronitor.Hangfire;
using Hangfire;

await Host.CreateDefaultBuilder()
    .ConfigureCronitor((context) => context.Configuration.GetValue<string>("Cronitor:ApiKey"))
    .ConfigureServices((context, services) =>
    {
        services.AddHangfire((provider, configuration) => configuration
            .UseSqlServerStorage(context.Configuration.GetConnectionString("Hangfire"))
            .UseCronitor(provider));
        services.AddHangfireServer();
    })
    .Build()
    .RunAsync();
```
`AddHangfire()` and `AddHangfireServer()` come from Hangfire's own `Hangfire.NetCore` package (or `Hangfire.AspNetCore`).

### Without dependency injection
Configure the static `CronitorClient` first, then call `UseCronitor()`:
```c#
using Cronitor;
using Cronitor.Hangfire;
using Hangfire;

CronitorClient.Configure("apiKey");

GlobalConfiguration.Configuration
    .UseSqlServerStorage("connectionString")
    .UseCronitor();
```

## Monitor keys
A recurring job reports to the monitor with the same key as its ID, so name your recurring jobs after their Cronitor monitors:
```c#
RecurringJob.AddOrUpdate("nightly-backup", () => Backup.Run(), Cron.Daily(3));
```
Cronitor doesn't accept `/` in a monitor key, so a `/` in the ID becomes `-`: a recurring job `reports/daily` reports to `reports-daily`. Recurring jobs `reports/daily` and `reports-daily` would therefore share a monitor; rename one of them to keep them apart. A monitor key can be at most 100 characters: a recurring job whose key would be longer isn't reported or synced, and the reason is logged.

Jobs that are not recurring, such as those started with `BackgroundJob.Enqueue()`, are not reported.

## Retries
When a recurring job fails, Hangfire retries it by default, often minutes or hours later. Each attempt is reported as its own run, and a retry's fail message ends with its attempt, for example `Disk is full (attempt 2)`. A job that fails twice and then succeeds is therefore reported as two failed runs followed by a completed one.

## Schedules
A monitor created by a job's first report has no schedule, so Cronitor alerts when the job fails but not when it doesn't run at all. To give each recurring job's monitor the job's schedule and time zone, add schedule sync:
```c#
services.AddHangfireServer();
services.AddCronitorHangfireScheduleSync();
```
Once the application has started, the recurring jobs in Hangfire's storage are sent to Cronitor in one request, in the background. Only the schedule and time zone are set; everything else on the monitor, such as its name and alerts, is kept. Recurring jobs added later get their schedule at the next start.

A schedule is only synced when Cronitor reads it the same way as Hangfire. A cron expression with a seconds field is synced without it when the seconds are 0; one Cronitor doesn't support, such as the nearest weekday (`15W`), isn't synced, and the reason is logged. Recurring jobs that share a monitor key share its schedules, as long as they run in the same time zone. Schedule sync needs dependency injection.

## Errors
Reporting never affects your jobs: the start of a job is reported in the background, and the worker waits at most five seconds for its end to be reported. If Cronitor is slower than that, the worker moves on and the report continues in the background, so a report still pending when the application stops can be lost. If a report can't be sent, the job still runs and keeps its result, and the error is logged through the host's logging.

For everything else about the Cronitor client, such as logging and the API key, see the [Cronitor README](https://github.com/gonace/Cronitor#readme).
