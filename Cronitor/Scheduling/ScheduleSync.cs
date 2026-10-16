using Cronitor.Constants;
using Cronitor.Exceptions;
using Cronitor.Internals;
using Cronitor.Models;
using Cronitor.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cronitor.Scheduling;

/// <summary>
/// Gives monitors their schedules in Cronitor, so Cronitor can alert when a job doesn't run. Each monitor is sent as a
/// job with only its schedules and timezone, so everything else set in Cronitor, such as its name and alerts, is kept;
/// a monitor that doesn't exist yet is created.
/// Syncing never throws: it runs next to the jobs it describes, so every problem is logged instead.
/// </summary>
public sealed class ScheduleSync
{
    private readonly IMonitorsClient _monitors;
    private readonly ILogger<ScheduleSync> _logger;

    public ScheduleSync(IMonitorsClient monitors, ILogger<ScheduleSync> logger = null)
    {
        _monitors = monitors ?? throw new ArgumentNullException(nameof(monitors));
        _logger = logger ?? NullLogger<ScheduleSync>.Instance;
    }

    /// <summary>
    /// Sends the schedules in one request. Cronitor rejects the whole request when it rejects one monitor, so the
    /// rejected monitors are logged and the others sent once more without them. Any other error is logged, and the
    /// schedules are not sent again.
    /// </summary>
    public async Task SyncAsync(IEnumerable<MonitorSchedule> schedules)
    {
        var toSync = (schedules ?? throw new ArgumentNullException(nameof(schedules))).ToList();
        if (toSync.Count == 0)
        {
            return;
        }

        try
        {
            await UpdateAsync(toSync).ConfigureAwait(false);
        }
        catch (ApiException exception) when (exception.StatusCode == HttpStatusCode.BadRequest && RejectedMonitors(exception.Response, toSync) is { Count: > 0 } rejected)
        {
            foreach (var monitor in rejected)
            {
                _logger.ScheduleRejected(monitor.Key, monitor.Value);
            }

            await RetryWithoutAsync(toSync, rejected).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.SchedulesNotSynced(exception, toSync.Count);
        }
    }

    private async Task RetryWithoutAsync(IReadOnlyCollection<MonitorSchedule> schedules, IReadOnlyDictionary<string, string> rejected)
    {
        var remaining = schedules.Where(schedule => !rejected.ContainsKey(schedule.MonitorKey)).ToList();
        if (remaining.Count == 0)
        {
            return;
        }

        try
        {
            await UpdateAsync(remaining).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.SchedulesNotSynced(exception, remaining.Count);
        }
    }

    private async Task UpdateAsync(IReadOnlyCollection<MonitorSchedule> schedules)
    {
        await _monitors.UpdateAsync(new UpdateMonitorRequest(schedules.Select(ToMonitor))).ConfigureAwait(false);
        _logger.SchedulesSynced(schedules.Count);
    }

    private static Monitor ToMonitor(MonitorSchedule schedule) =>
        new Monitor(schedule.MonitorKey)
        {
            Type = MonitorType.Job.ToString(),
            Schedules = schedule.Schedules.Select(expression => new ScheduleExpression(expression)).ToList(),
            Timezone = schedule.Timezone,
        };

    /// <summary>
    /// The sent monitors a rejected request names, with Cronitor's reasons, from a response such as
    /// <c>{"nightly-backup": {"schedules": ["Invalid schedule expression: …"]}}</c>. Empty if it names none of them,
    /// as for an ordinary error such as <c>{"detail": "…"}</c>.
    /// </summary>
    private static IReadOnlyDictionary<string, string> RejectedMonitors(string response, IEnumerable<MonitorSchedule> sent)
    {
        var sentKeys = new HashSet<string>(sent.Select(schedule => schedule.MonitorKey.ToString()), StringComparer.Ordinal);
        var rejected = new Dictionary<string, string>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(response))
        {
            return rejected;
        }

        try
        {
            using (var document = JsonDocument.Parse(response))
            {
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return rejected;
                }

                foreach (var monitor in document.RootElement.EnumerateObject().Where(property => sentKeys.Contains(property.Name)))
                {
                    rejected[monitor.Name] = monitor.Value.GetRawText();
                }
            }
        }
        catch (JsonException)
        {
        }

        return rejected;
    }
}
