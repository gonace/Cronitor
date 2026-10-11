using System;
using System.Threading.Tasks;

namespace Cronitor
{
    /// <summary>
    /// A single run of a job, reported to Cronitor as run → complete or fail.
    /// Disposing a run that was neither completed nor failed reports it as failed,
    /// so a forgotten <see cref="Complete"/> shows up instead of hiding failures.
    /// Complete and Fail return <c>true</c> when the event was delivered, and <c>false</c>
    /// when it could not be delivered or the run was already completed or failed.
    /// </summary>
    public interface ITelemetryRun : IDisposable
    {
        bool Complete(string message = null);
        Task<bool> CompleteAsync(string message = null);
        bool Fail(string message = null);
        Task<bool> FailAsync(string message = null);
    }
}
