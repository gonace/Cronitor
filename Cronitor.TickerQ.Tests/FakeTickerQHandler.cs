using System;
using System.Threading;
using System.Threading.Tasks;
using TickerQ.Utilities.Interfaces;
using TickerQ.Utilities.Models;

namespace Cronitor.TickerQ.Tests;

/// <summary>
/// Stands in for TickerQ's own handler: runs the given job body, which sets the status TickerQ would end the job with.
/// </summary>
internal sealed class FakeTickerQHandler : ITickerExecutionTaskHandler
{
    private readonly Func<InternalFunctionContext, CancellationToken, Task> _job;
    private int _calls;

    public FakeTickerQHandler(Func<InternalFunctionContext, CancellationToken, Task> job) => _job = job;

    public int Calls => _calls;

    public Task ExecuteTaskAsync(InternalFunctionContext context, bool isDue, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _calls);
        return _job(context, cancellationToken);
    }
}
