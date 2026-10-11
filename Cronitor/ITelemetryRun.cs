using System;
using System.Threading.Tasks;

namespace Cronitor
{
    /// <summary>
    /// A single run of a job, reported to Cronitor as run → complete or fail.
    /// Disposing a run that was neither completed nor failed reports it as failed,
    /// so a forgotten <see cref="Complete"/> shows up instead of hiding failures.
    /// </summary>
    public interface ITelemetryRun : IDisposable
    {
        void Complete(string message = null);
        Task CompleteAsync(string message = null);
        void Fail(string message = null);
        Task FailAsync(string message = null);
    }
}
