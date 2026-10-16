using Coravel.Invocable;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Coravel;

/// <summary>
/// The invocable <c>ScheduleWithCronitor</c> schedules in place of <typeparamref name="TInvocable"/>: it runs the job
/// and reports the run to Cronitor. Coravel only creates and invokes it for a run that happens, so runs Coravel skips,
/// because of <c>PreventOverlapping</c> or <c>When</c>, are not reported. Coravel creates it with the monitor key as
/// a parameter and resolves the job from the job's own scope, as it would for the job itself.
/// </summary>
internal sealed class CronitorInvocable<TInvocable> : IInvocable, ICancellableInvocable where TInvocable : IInvocable
{
    private readonly MonitorKey _monitorKey;
    private readonly TInvocable _invocable;
    private readonly CronitorReporter _reporter;

    public CronitorInvocable(MonitorKey monitorKey, IServiceProvider services)
    {
        _monitorKey = monitorKey;
        _reporter = services.GetService<CronitorReporter>()
            ?? throw new InvalidOperationException("Cronitor.Coravel is not registered. Call services.AddCronitorCoravel() when you add Coravel's scheduler.");
        _invocable = services.GetRequiredService<TInvocable>();
    }

    public CancellationToken CancellationToken { get; set; }

    public Task Invoke()
    {
        if (_invocable is ICancellableInvocable cancellable)
        {
            cancellable.CancellationToken = CancellationToken;
        }

        return _reporter.RunAsync(_monitorKey, _invocable.Invoke);
    }
}
