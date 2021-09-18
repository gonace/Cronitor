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

## Development


## Authors
- [@gonace](https://github.com/gonace)
- [@laget-se](https://github.com/laget-se)
