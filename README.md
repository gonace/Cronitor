# Cronitor API Client
Cronitor is a service for heartbeat-style monitoring of anything that can send an HTTP request. It's particularly well suited for monitoring cron jobs, Jenkins jobs, or any other scheduled task.

![Nuget](https://img.shields.io/nuget/v/Cronitor)
![Nuget](https://img.shields.io/nuget/dt/Cronitor)

## Supported APIs
This .NET library provides a simple abstraction for the pinging of a Cronitor monitor. For a better understanding of the API this library talks to, please see the documentation, links below.
* [Activity API](https://cronitor.io/docs/activity-api)
* [Monitor API](https://cronitor.io/docs/monitor-api)
* [Notifications API](https://cronitor.io/docs/template-api)
* [Telemetry API](https://cronitor.io/docs/telemetry-api)

## Install
You can download the cronitor client nuget.
[https://www.nuget.org/packages/Cronitor](https://www.nuget.org/packages/Cronitor)

## Usage
```c#
public class SomeClass
{
    private readonly TelemetryClient _client;

    public SomeClass()
    {
        _client = new TelemetryClient("apiKey");
    }

    public void SomeMethod()
    {
        # Begin / ping a monitor
        _client.Run("monitorKey");
        # Begin / ping a monitor asynchronous
        _client.RunAsync("monitorKey");


        # Complete a monitor
        _client.Complete("monitorKey");
        # Complete a monitor asynchronous
        _client.CompleteAsync("monitorKey");
        

        # Complete a monitor
        _client.Fail("monitorKey");
        # Complete a monitor asynchronous
        _client.FailAsync("monitorKey");


        # Tick a monitor
        _client.Tick("monitorKey");
        # Tick a monitor asynchronous
        _client.TickAsync("monitorKey");
    }
}
```

### Message
> Each `Command`-method in `TelemetryClient` can be sent with a message.
```c#
public class SomeClass
{
    private readonly TelemetryClient _client;

    public SomeClass()
    {
        _client = new TelemetryClient("apiKey");
    }

    public void SomeMethod()
    {
        # Begin / ping a monitor
        _client.Run("monitorKey", "message");
        # Begin / ping a monitor asynchronous
        _client.RunAsync("monitorKey", "message");


        # Complete a monitor
        _client.Complete("monitorKey", "message");
        # Complete a monitor asynchronous
        _client.CompleteAsync("monitorKey", "message");
        

        # Complete a monitor
        _client.Fail("monitorKey", "message");
        # Complete a monitor asynchronous
        _client.FailAsync("monitorKey", "message");


        # Tick a monitor
        _client.Tick("monitorKey", "message");
        # Tick a monitor asynchronous
        _client.TickAsync("monitorKey", "message");
    }
}
```

### Advanced
> You can choose to use `Ping` and/or `PingAsync` to send a `Command` (`CompleteCommand`, `FailCommand`, `RunCommand` and `TickCommand`) with extended properties! To find more about telemetry event enrichers please read the [documentation](https://cronitor.io/docs/telemetry-api#parameters)!

#### Required
##### `WithApiKey`
> The Cronitor API uses API keys to authenticate requests. Your API keys can be found on your account [settings](https://cronitor.io/settings) page.
##### `WithMonitorKey`
> A monitor’s unique identifier. The key is used in making API requests to an individual monitor resource.

#### Optional
##### `WithEnvironment` (`environment` or `env`)
> The environment the telemetry event is being sent from. Use this for monitors that are running in multiple environments (e.g. staging and production). Alerting can be configured per environment.
##### `WithHost` (`host`)
> The hostname of the server sending the telemetry event.
##### `WithMessage` (`message`)
> A url-encoded message of up to 2000 characters.
##### `WithMetric` (`metric`)
> Performance related metrics. Must be one of:
  `Metric.Count` (`count:*`) - record counts of important events.
  `Metric.Duration` (`duration:*`) - the duration of the job/task being monitored.
  `Metric.Errors` (`error_count:*`) - the number of errors that occurred.
##### `WithSeries` (`series`)
> A unique user-supplied ID to collate related pings, i.e. matching state=run and state=complete|fail to one another. If a job is pinging very frequently (every 2-3s or faster), it will greatly improve matching accurracy.
##### `WithStatus` (`series`)
> Exit code returned from a background job.

#### Example
```c#
    var command = new CompleteCommand()
        .WithApiKey("apiKey")
        .WithMonitorKey("monitorKey")
        .WithEnvironment("Production")
        .WithHost("127.0.0.1")
        .WithMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit.")
        .WithMetric(Metric.Count, new decimal(99.99))
        .WithSeries("3de5db91-9c02-4e95-b8a9-9a2442702336")
        .WithStatus(0);
    
    # Ping a monitor
    _client.Ping(command);
    # Ping a monitor asynchronous
    _client.PingAsync(command);
```

## Development


## Authors
- [@gonace](https://github.com/gonace)
