using Cronitor.Abstractions;
using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Internals;
using System.Threading.Tasks;
using Cronitor.Extensions;

namespace Cronitor
{
    public interface ITelemetriesClient
    {
        void Run(string monitorKey, string message = null, string environment = null);
        Task RunAsync(string monitorKey, string message = null, string environment = null);
        void Complete(string monitorKey, string message = null, string environment = null);
        Task CompleteAsync(string monitorKey, string message = null, string environment = null);
        void Fail(string monitorKey, string message = null, string environment = null);
        Task FailAsync(string monitorKey, string message = null, string environment = null);
        void Tick(string monitorKey, string message = null, string environment = null);
        Task TickAsync(string monitorKey, string message = null, string environment = null);
        void Ping(Command command);
        Task PingAsync(Command command);

        /// <summary>
        /// Starts a job run and reports it as running.
        /// Call <see cref="ITelemetryRun.Complete"/> when the job succeeds; disposing the run without it reports a failure.
        /// </summary>
        ITelemetryRun Start(string monitorKey, string environment = null);
        /// <inheritdoc cref="Start"/>
        Task<ITelemetryRun> StartAsync(string monitorKey, string environment = null);
    }

    public class TelemetriesClient : BaseClient<TelemetriesClient>, ITelemetriesClient
    {
        private readonly string _apiKey;

        public TelemetriesClient(string apiKey)
            : base(HttpClient.ForTelemetry(apiKey))
        {
            _apiKey = apiKey;
        }

        internal TelemetriesClient(string apiKey, HttpClient client)
            : base(client)
        {
            _apiKey = apiKey;
        }


        public void Run(string monitorKey, string message = null, string environment = null) =>
            RunAsync(monitorKey, message, environment).GetAwaiter().GetResult();

        public async Task RunAsync(string monitorKey, string message = null, string environment = null)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(monitorKey);

            var command = new RunCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey);

            if (!string.IsNullOrWhiteSpace(message))
            {
                command.WithMessage(message);
            }

            if (!string.IsNullOrWhiteSpace(environment))
            {
                command.WithEnvironment(environment);
            }

            await PingAsync(command);
        }

        public void Complete(string monitorKey, string message = null, string environment = null) =>
            CompleteAsync(monitorKey, message, environment).GetAwaiter().GetResult();

        public async Task CompleteAsync(string monitorKey, string message = null, string environment = null)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(monitorKey);

            var command = new CompleteCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey);

            if (!string.IsNullOrWhiteSpace(message))
            {
                command.WithMessage(message);
            }

            if (!string.IsNullOrWhiteSpace(environment))
            {
                command.WithEnvironment(environment);
            }

            await PingAsync(command);
        }

        public void Fail(string monitorKey, string message = null, string environment = null) =>
            FailAsync(monitorKey, message, environment).GetAwaiter().GetResult();

        public async Task FailAsync(string monitorKey, string message = null, string environment = null)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(monitorKey);

            var command = new FailCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey);

            if (!string.IsNullOrWhiteSpace(message))
            {
                command.WithMessage(message);
            }

            if (!string.IsNullOrWhiteSpace(environment))
            {
                command.WithEnvironment(environment);
            }

            await PingAsync(command);
        }

        public void Tick(string monitorKey, string message = null, string environment = null) =>
            TickAsync(monitorKey, message, environment).GetAwaiter().GetResult();

        public async Task TickAsync(string monitorKey, string message = null, string environment = null)
        {
            ArgumentHelper.ThrowIfNullOrWhiteSpace(monitorKey);

            var command = new TickCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey);

            if (!string.IsNullOrWhiteSpace(message))
            {
                command.WithMessage(message);
            }

            if (!string.IsNullOrWhiteSpace(environment))
            {
                command.WithEnvironment(environment);
            }

            await PingAsync(command);
        }

        public void Ping(Command command) =>
            PingAsync(command).GetAwaiter().GetResult();

        public async Task PingAsync(Command command) =>
            await SendAsync(command);

        public ITelemetryRun Start(string monitorKey, string environment = null) =>
            TelemetryRun.Start(this, monitorKey, environment);

        public async Task<ITelemetryRun> StartAsync(string monitorKey, string environment = null) =>
            await TelemetryRun.StartAsync(this, monitorKey, environment);

        /// <summary>
        /// Sends <paramref name="command"/> authenticated with this client's API key.
        /// </summary>
        internal Task PingWithApiKeyAsync(Command command) =>
            PingAsync(command.WithApiKey(_apiKey));
    }
}