using Microsoft.Extensions.Hosting;
using System.Threading;

namespace Cronitor.Shared.Tests;

public sealed class TestLifetime : IHostApplicationLifetime
{
    private readonly CancellationTokenSource _started = new CancellationTokenSource();

    public CancellationToken ApplicationStarted => _started.Token;

    public CancellationToken ApplicationStopping => CancellationToken.None;

    public CancellationToken ApplicationStopped => CancellationToken.None;

    public void NotifyStarted() => _started.Cancel();

    public void StopApplication()
    {
    }
}
