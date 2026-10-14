using Quartz;
using System.Threading;
using System.Threading.Tasks;

namespace Cronitor.Quartz.Tests;

public sealed class NoopJob : IJob
{
    public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken) => default;
}
