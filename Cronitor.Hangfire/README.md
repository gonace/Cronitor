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
Configure the static `Cronitor` class first, then call `UseCronitor()`:
```c#
using Cronitor.Hangfire;
using Hangfire;

Cronitor.Cronitor.Configure("apiKey");

GlobalConfiguration.Configuration
    .UseSqlServerStorage("connectionString")
    .UseCronitor();
```

## Monitor keys
A recurring job reports to the monitor with the same key as its ID, so name your recurring jobs after their Cronitor monitors:
```c#
RecurringJob.AddOrUpdate("nightly-backup", () => Backup.Run(), Cron.Daily(3));
```
Cronitor doesn't accept `/` in a monitor key, so a `/` in the ID becomes `-`: a recurring job `reports/daily` reports to `reports-daily`.

Jobs that are not recurring, such as those started with `BackgroundJob.Enqueue()`, are not reported.

## Retries
When a recurring job fails, Hangfire retries it by default, often minutes or hours later. Each attempt is reported as its own run, and a retry's fail message ends with its attempt, for example `Disk is full (attempt 2)`. A job that fails twice and then succeeds is therefore reported as two failed runs followed by a completed one.

## Errors
Reporting never affects your jobs: the start of a job is reported in the background, and the worker waits at most five seconds for its end to be reported. If Cronitor is slower than that, the worker moves on and the report continues in the background, so a report still pending when the application stops can be lost. If a report can't be sent, the job still runs and keeps its result, and the error is logged through the host's logging.

For everything else about the Cronitor client, such as logging and the API key, see the [Cronitor README](https://github.com/gonace/Cronitor#readme).
