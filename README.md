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
> For the full documentation please read our [wiki](https://github.com/gonace/Cronitor/wiki), [telemetry wiki](https://github.com/gonace/Cronitor/wiki/Telemetry)!

### Simple
> If you're using .NET (former .NET Core) and utilizing the default hosting and startup pattern (`Microsoft.Extensions.Hosting`) you use [`Cronitor.Extensions.Hosting`](https://github.com/gonace/Cronitor.Extensions.Hosting).

The easiest way to use the client is to run `.Configure()`, this will create a static instance (`Cronitor`) of the client that can be used throughout your application.

#### Examples
```c#
using Cronitor.Models;
using Cronitor.Requests;

Cronitor.Configure("apiKey")

var monitor = new Monitor();
var request = new CreateRequest(monitor);
var response = Cronitor.Monitors.Create(request);
```

### Advanced
If you only need access to one (or a few) clients you're able to configure each client individually.

#### Examples
```c#
using Cronitor.Models;
using Cronitor.Requests;

public class SomeClass
{
    private readonly ITelemetriesClient _client;

    public SomeClass()
    {
        _client = new TelemetriesClient("apiKey");
    }

    public Monitor Create()
    {
        var monitor = new Monitor();
        var request = new CreateRequest(monitor);
        var response = Cronitor.Monitors.Create(request);

        return response;
    }

    public async Task<Monitor> CreateAsync()
    {
        var monitor = new Monitor();
        var request = new CreateRequest(monitor);
        var response = await Cronitor.Monitors.CreateAsync(request);

        return resposne;
    }
}
```

## Development
### TODO
* Implement Timezone constant (if not too big of a hassle to maintain)
* Implement cron expression-language (if found as needed?)
* Implement cronitor `assertions`-language

## Contributing
Pull requests and features are happily considered! By participating in this project you agree to abide by the [Code of Conduct](http://contributor-covenant.org/version/2/0).

### To contribute

Fork, then clone the repo:
```
git clone git@github.com:your-username/Cronitor.git
```
Push to your fork and [submit a pull request](https://github.com/gonace/Cronitor/compare/)
