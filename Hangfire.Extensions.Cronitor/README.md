# Hangfire.Extensions.Cronitor
Reports every [Hangfire](https://www.hangfire.io) recurring job to [Cronitor](https://cronitor.io): run when the job starts, complete when it succeeds, and fail, with the exception's message, when it throws.

![Nuget](https://img.shields.io/nuget/v/Hangfire.Extensions.Cronitor)
![Nuget](https://img.shields.io/nuget/dt/Hangfire.Extensions.Cronitor)

## Install
```
dotnet add package Hangfire.Extensions.Cronitor
```
The package's major version matches Hangfire's: version 1 is for Hangfire 1 (1.8.25 or later).

## Setup
### Dependency injection
Register Cronitor with `.ConfigureCronitor()`, then call `UseCronitor(provider)` when adding Hangfire:
```c#
using Cronitor.Extensions;
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
Jobs that are not recurring, such as those started with `BackgroundJob.Enqueue()`, are not reported.

## Errors
Reporting never affects your jobs: reports are sent in the background, so a job never waits for Cronitor. If a report can't be sent, the job still runs and keeps its result, and the error is logged through the host's logging. When the application stops, it waits up to five seconds for reports that are still being sent, so the outcome of the last jobs isn't lost.

For everything else about the Cronitor client, such as logging and the API key, see the [Cronitor README](https://github.com/gonace/Cronitor#readme).
