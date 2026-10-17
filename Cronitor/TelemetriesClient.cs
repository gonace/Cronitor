using Cronitor.Abstractions;
using Cronitor.Commands;
using Cronitor.Exceptions;
using Cronitor.Internals;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Cronitor.Extensions;
using HttpClient = Cronitor.Internals.HttpClient;

namespace Cronitor;

/// <summary>
/// Sends telemetry events to Cronitor. Telemetry never breaks the job it monitors:
/// transient failures are retried, and if an event still cannot be delivered the
/// methods return <c>false</c> instead of throwing. Programming errors, such as a
/// missing monitor key, still throw.
/// </summary>
public interface ITelemetriesClient
{
    bool Run(MonitorKey monitorKey, string message = null, string environment = null);
    Task<bool> RunAsync(MonitorKey monitorKey, string message = null, string environment = null);
    bool Complete(MonitorKey monitorKey, string message = null, string environment = null);
    Task<bool> CompleteAsync(MonitorKey monitorKey, string message = null, string environment = null);
    bool Fail(MonitorKey monitorKey, string message = null, string environment = null);
    Task<bool> FailAsync(MonitorKey monitorKey, string message = null, string environment = null);
    bool Tick(MonitorKey monitorKey, string message = null, string environment = null);
    Task<bool> TickAsync(MonitorKey monitorKey, string message = null, string environment = null);
    bool Ping(Command command);
    Task<bool> PingAsync(Command command);

    /// <summary>
    /// Starts a job run and reports it as running.
    /// Call <see cref="ITelemetryRun.Complete"/> when the job succeeds; disposing the run without it reports a failure.
    /// </summary>
    ITelemetryRun Start(MonitorKey monitorKey, string environment = null);
    /// <inheritdoc cref="Start"/>
    Task<ITelemetryRun> StartAsync(MonitorKey monitorKey, string environment = null);
}

public class TelemetriesClient : BaseClient<TelemetriesClient>, ITelemetriesClient
{
    private readonly string _apiKey;
    private readonly ILogger<TelemetriesClient> _logger;

    public TelemetriesClient(string apiKey)
        : this(apiKey, NullLoggerFactory.Instance)
    {
    }

    /// <summary>
    /// Logs an error through <paramref name="loggerFactory"/> whenever a telemetry event cannot be delivered.
    /// </summary>
    public TelemetriesClient(string apiKey, ILoggerFactory loggerFactory)
        : this(apiKey, HttpClient.ForTelemetry(apiKey), loggerFactory)
    {
    }

    internal TelemetriesClient(string apiKey, HttpClient client)
        : this(apiKey, client, NullLoggerFactory.Instance)
    {
    }

    internal TelemetriesClient(string apiKey, HttpClient client, ILoggerFactory loggerFactory)
        : base(client)
    {
        if (loggerFactory == null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }

        _apiKey = apiKey;
        _logger = loggerFactory.CreateLogger<TelemetriesClient>();
    }


    public bool Run(MonitorKey monitorKey, string message = null, string environment = null) =>
        RunAsync(monitorKey, message, environment).GetAwaiter().GetResult();

    public async Task<bool> RunAsync(MonitorKey monitorKey, string message = null, string environment = null)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(monitorKey);

        var command = new RunCommand()
            .WithApiKey(_apiKey)
            .WithMonitorKey(monitorKey);

        if (!string.IsNullOrWhiteSpace(message))
        {
            command.WithMessage(message);
        }

        if (!string.IsNullOrWhiteSpace(environment))
        {
            command.WithEnvironment(environment);
        }

        return await PingAsync(command).ConfigureAwait(false);
    }

    public bool Complete(MonitorKey monitorKey, string message = null, string environment = null) =>
        CompleteAsync(monitorKey, message, environment).GetAwaiter().GetResult();

    public async Task<bool> CompleteAsync(MonitorKey monitorKey, string message = null, string environment = null)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(monitorKey);

        var command = new CompleteCommand()
            .WithApiKey(_apiKey)
            .WithMonitorKey(monitorKey);

        if (!string.IsNullOrWhiteSpace(message))
        {
            command.WithMessage(message);
        }

        if (!string.IsNullOrWhiteSpace(environment))
        {
            command.WithEnvironment(environment);
        }

        return await PingAsync(command).ConfigureAwait(false);
    }

    public bool Fail(MonitorKey monitorKey, string message = null, string environment = null) =>
        FailAsync(monitorKey, message, environment).GetAwaiter().GetResult();

    public async Task<bool> FailAsync(MonitorKey monitorKey, string message = null, string environment = null)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(monitorKey);

        var command = new FailCommand()
            .WithApiKey(_apiKey)
            .WithMonitorKey(monitorKey);

        if (!string.IsNullOrWhiteSpace(message))
        {
            command.WithMessage(message);
        }

        if (!string.IsNullOrWhiteSpace(environment))
        {
            command.WithEnvironment(environment);
        }

        return await PingAsync(command).ConfigureAwait(false);
    }

    public bool Tick(MonitorKey monitorKey, string message = null, string environment = null) =>
        TickAsync(monitorKey, message, environment).GetAwaiter().GetResult();

    public async Task<bool> TickAsync(MonitorKey monitorKey, string message = null, string environment = null)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(monitorKey);

        var command = new TickCommand()
            .WithApiKey(_apiKey)
            .WithMonitorKey(monitorKey);

        if (!string.IsNullOrWhiteSpace(message))
        {
            command.WithMessage(message);
        }

        if (!string.IsNullOrWhiteSpace(environment))
        {
            command.WithEnvironment(environment);
        }

        return await PingAsync(command).ConfigureAwait(false);
    }

    public bool Ping(Command command) =>
        PingAsync(command).GetAwaiter().GetResult();

    public async Task<bool> PingAsync(Command command)
    {
        try
        {
            await SendAsync(command).ConfigureAwait(false);
            return true;
        }
        catch (Exception exception) when (IsDeliveryFailure(exception))
        {
            _logger.TelemetryNotDelivered(exception, command.Endpoint, command.MonitorKey);
            return false;
        }
    }

    public ITelemetryRun Start(MonitorKey monitorKey, string environment = null) =>
        TelemetryRun.Start(this, monitorKey, environment);

    public async Task<ITelemetryRun> StartAsync(MonitorKey monitorKey, string environment = null) =>
        await TelemetryRun.StartAsync(this, monitorKey, environment).ConfigureAwait(false);

    /// <summary>
    /// Sends <paramref name="command"/> authenticated with this client's API key.
    /// </summary>
    internal Task<bool> PingWithApiKeyAsync(Command command) =>
        PingAsync(command.WithApiKey(_apiKey));

    /// <summary>
    /// Whether <paramref name="exception"/> means the ping could not be delivered to Cronitor,
    /// as opposed to a programming error such as a missing monitor key.
    /// </summary>
    private static bool IsDeliveryFailure(Exception exception)
    {
        return exception is HttpRequestException
            || exception is ApiException
            || exception is TaskCanceledException
            || exception is TimeoutException
            || exception is JsonException
            || exception is ObjectDisposedException;
    }
}
