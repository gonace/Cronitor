# Cronitor.TickerQ
Reports every [TickerQ](https://tickerq.net) cron job to [Cronitor](https://cronitor.io): run when the job starts, complete when it succeeds, and fail, with the exception's message, when it fails.

![Nuget](https://img.shields.io/nuget/v/Cronitor.TickerQ)
![Nuget](https://img.shields.io/nuget/dt/Cronitor.TickerQ)

## Install
```
dotnet add package Cronitor.TickerQ
```
The package's major version matches TickerQ's, which ships one major per .NET version:

| Cronitor.TickerQ | TickerQ | .NET |
| --- | --- | --- |
| 10.x | 10.4.0 or later | .NET 10 |
| 9.x | 9.3.0 or later | .NET 9 |
| 8.x | 8.3.0 or later | .NET 8 |

`dotnet add package` picks the latest version, which is for .NET 10. On .NET 8 or 9, pick the version matching your TickerQ, for example `dotnet add package Cronitor.TickerQ --version 8.*`.

## Setup
Register Cronitor with `.ConfigureCronitor()`, then call `AddCronitorTickerQ()` after `AddTickerQ()`:
```c#
using Cronitor.Extensions;
using Cronitor.TickerQ;
using TickerQ.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureCronitor((context) => context.Configuration.GetValue<string>("Cronitor:ApiKey"));
builder.Services.AddTickerQ();
builder.Services.AddCronitorTickerQ();

var app = builder.Build();
app.UseTickerQ();
app.Run();
```
`AddCronitorTickerQ()` wraps the job handler `AddTickerQ()` registers, so it must come after it; otherwise it throws at startup.

## What is reported
Every cron job occurrence is reported once: run when it starts, then, depending on how TickerQ ends it:

| TickerQ ends the job as | Reported to Cronitor |
| --- | --- |
| Done | complete |
| Failed | fail, with the exception's message |
| Cancelled, for example when the application stops | fail, with "The job was cancelled" |
| Skipped, when the job throws TickerQ's `TerminateExecutionException` | complete, with the job's message |

TickerQ retries a failing job within the same occurrence, so a job that succeeds on a retry is reported as complete. One-off jobs, scheduled with TickerQ's time tickers, are not reported, since they have no schedule for Cronitor to monitor.

## Monitor keys
A cron job reports to the monitor with the same key as its function name, the name in `[TickerFunction("…")]`. Cron tickers that run the same function share its monitor. Cronitor doesn't accept `/` in a monitor key, so a `/` in the function name becomes `-`: a function `reports/daily` reports to `reports-daily`. Functions `reports/daily` and `reports-daily` would therefore share a monitor; rename one of them to keep them apart. A monitor key can be at most 100 characters: a function whose name is longer isn't reported or synced, and the reason is logged.

## Schedules
A monitor created by a job's first report has no schedule, so Cronitor alerts when the job fails but not when it doesn't run at all. To give each cron job's monitor the schedules of its cron tickers, add schedule sync after `AddTickerQ()`:
```c#
builder.Services.AddTickerQ();
builder.Services.AddCronitorTickerQ();
builder.Services.AddCronitorTickerQScheduleSync();
```
Once the application has started, the enabled cron tickers in TickerQ's store, both those from `[TickerFunction]` and those added with `ICronTickerManager`, are sent to Cronitor in one request, in the background. They are sent with TickerQ's time zone, `SchedulerTimeZone`, which is the machine's local time zone unless you set it. Only the schedules and time zone are set; everything else on the monitor, such as its name and alerts, is kept. Cron tickers added later get their schedules at the next start.

A schedule is only synced when Cronitor reads it the same way as TickerQ: a cron expression whose seconds aren't 0, that restricts both the day of the month and the day of the week (TickerQ then needs both to match, Cronitor either), that has a range running backwards, such as `22-2`, or that uses full day names, such as `Sunday`, isn't synced; the reason is logged. All the cron tickers of a function are synced together. Schedule sync needs dependency injection.

## Errors
Reporting never affects your jobs: reports are sent in the background, so a job never waits for Cronitor. If a report can't be sent, the job still runs, and the error is logged through the host's logging. When the application stops, TickerQ cancels the jobs still running; the outcome of those jobs, and of reports still being sent, is waited for up to five seconds after TickerQ has stopped, so it isn't lost.

For everything else about the Cronitor client, such as logging and the API key, see the [Cronitor README](https://github.com/gonace/Cronitor#readme).
