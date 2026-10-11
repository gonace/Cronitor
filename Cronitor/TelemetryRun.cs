using Cronitor.Commands;
using Cronitor.Extensions;
using System;
using System.Threading.Tasks;

namespace Cronitor
{
    internal sealed class TelemetryRun : ITelemetryRun
    {
        private readonly TelemetriesClient _client;
        private readonly string _monitorKey;
        private readonly string _environment;
        private readonly string _series = Guid.NewGuid().ToString("N");
        private bool _finished;

        internal TelemetryRun(TelemetriesClient client, string monitorKey, string environment)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(monitorKey);

            _client = client;
            _monitorKey = monitorKey;
            _environment = environment;
        }

        internal static TelemetryRun Start(TelemetriesClient client, string monitorKey, string environment = null) =>
            StartAsync(client, monitorKey, environment).GetAwaiter().GetResult();

        internal static async Task<TelemetryRun> StartAsync(TelemetriesClient client, string monitorKey, string environment = null)
        {
            var run = new TelemetryRun(client, monitorKey, environment);
            await run.SendAsync(new RunCommand(), null);

            return run;
        }

        public void Complete(string message = null) =>
            CompleteAsync(message).GetAwaiter().GetResult();

        public async Task CompleteAsync(string message = null)
        {
            if (TryFinish())
            {
                await SendAsync(new CompleteCommand(), message);
            }
        }

        public void Fail(string message = null) =>
            FailAsync(message).GetAwaiter().GetResult();

        public async Task FailAsync(string message = null)
        {
            if (TryFinish())
            {
                await SendAsync(new FailCommand(), message);
            }
        }

        /// <summary>
        /// Reports the run as failed unless it was completed or failed already.
        /// Never throws, so it cannot replace an exception thrown by the job itself.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Fail();
            }
            catch (Exception)
            {
                // Telemetry must not mask the job's own outcome.
            }
        }

        private bool TryFinish()
        {
            if (_finished)
            {
                return false;
            }

            _finished = true;
            return true;
        }

        private Task SendAsync(Command command, string message)
        {
            command.WithMonitorKey(_monitorKey).WithSeries(_series);

            if (!string.IsNullOrWhiteSpace(message))
            {
                command.WithMessage(message);
            }

            if (!string.IsNullOrWhiteSpace(_environment))
            {
                command.WithEnvironment(_environment);
            }

            return _client.PingWithApiKeyAsync(command);
        }
    }
}
