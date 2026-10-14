using Cronitor.Scheduling;
using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Hangfire;

internal sealed class CronitorScheduleSyncService : IHostedService
{
    private readonly JobStorage _storage;
    private readonly ScheduleSync _sync;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<CronitorScheduleSyncService> _logger;
    private CancellationTokenRegistration _applicationStarted;

    public CronitorScheduleSyncService(JobStorage storage, ScheduleSync sync, IHostApplicationLifetime lifetime, ILogger<CronitorScheduleSyncService> logger = null)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _sync = sync ?? throw new ArgumentNullException(nameof(sync));
        _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        _logger = logger ?? NullLogger<CronitorScheduleSyncService>.Instance;
    }

    internal Task Synced { get; private set; } = Task.CompletedTask;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _applicationStarted = _lifetime.ApplicationStarted.Register(() => Synced = Task.Run(SyncAsync, CancellationToken.None));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _applicationStarted.Dispose();
        return Task.CompletedTask;
    }

    private async Task SyncAsync()
    {
        IReadOnlyList<MonitorSchedule> schedules;
        try
        {
            using (var connection = _storage.GetConnection())
            {
                schedules = RecurringJobSchedules.Of(connection.GetRecurringJobs(), _logger);
            }
        }
        catch (Exception exception)
        {
            _logger.RecurringJobsNotRead(exception);
            return;
        }

        await _sync.SyncAsync(schedules).ConfigureAwait(false);
    }
}
