using System;
using System.Threading;

namespace Cronitor.Tests.Helpers;

/// <summary>
/// Runs code on its own thread with a synchronization context that, like a UI thread or classic ASP.NET, only
/// continues awaited work on that thread. While the code blocks the thread, that work never continues.
/// </summary>
public static class SingleThreadedContext
{
    /// <summary>
    /// Returns false if the code didn't finish within the limit.
    /// </summary>
    public static bool Run(Action code, TimeSpan limit)
    {
        Exception failure = null;
        var thread = new Thread(() =>
        {
            SynchronizationContext.SetSynchronizationContext(new BlockedThreadContext());
            try
            {
                code();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true
        };

        thread.Start();
        var finished = thread.Join(limit);
        if (failure != null)
        {
            throw new InvalidOperationException("The code threw on the single-threaded context.", failure);
        }

        return finished;
    }

    private sealed class BlockedThreadContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback callback, object state)
        {
        }
    }
}
