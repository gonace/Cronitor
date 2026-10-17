using System;
using System.Threading.Tasks;

namespace Cronitor;

/// <summary>
/// A single run of a job, reported to Cronitor as run → complete or fail.
/// Disposing a run that was neither completed, failed nor abandoned reports it as failed,
/// so a forgotten <see cref="Complete"/> shows up instead of hiding failures.
/// Complete, Fail and Abandon return <c>true</c> when the event was delivered, and <c>false</c>
/// when it could not be delivered or the run had already ended.
/// </summary>
public interface ITelemetryRun : IDisposable
{
    bool Complete(string message = null);
    Task<bool> CompleteAsync(string message = null);
    bool Fail(string message = null);
    Task<bool> FailAsync(string message = null);

    /// <summary>
    /// Stops reporting the run without completing or failing it, for a run that was stopped before it could end, such as
    /// when the service went offline. The message is reported as a tick, so the monitor keeps its state, and Cronitor
    /// keeps the run open until the job's next run.
    /// </summary>
    bool Abandon(string message);

    /// <inheritdoc cref="Abandon"/>
    Task<bool> AbandonAsync(string message);
}
