using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Internals;

/// <summary>
/// Gives the static <see cref="CronitorClient"/> the host's logging once the host starts.
/// UseCronitor configures Cronitor while the host is still being built, before its
/// <see cref="ILoggerFactory"/> exists, so this completes the setup when it does.
/// </summary>
internal sealed class CronitorLoggingHostedService : IHostedService
{
    private readonly Action<ILoggerFactory> _configure;
    private readonly ILoggerFactory _loggerFactory;

    public CronitorLoggingHostedService(Action<ILoggerFactory> configure, ILoggerFactory loggerFactory)
    {
        _configure = configure;
        _loggerFactory = loggerFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _configure(_loggerFactory);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
