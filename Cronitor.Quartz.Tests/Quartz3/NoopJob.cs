using Quartz;
using System.Threading.Tasks;

namespace Cronitor.Quartz.Tests;

public sealed class NoopJob : IJob
{
    public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
}
