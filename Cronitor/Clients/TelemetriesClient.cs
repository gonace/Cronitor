using Cronitor.Abstractions;
using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Internals;
using System.Threading.Tasks;
using Cronitor.Extensions;

namespace Cronitor.Clients
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
    }

    public class TelemetriesClient : BaseClient<TelemetriesClient>, ITelemetriesClient
    {
        private readonly string _apiKey;

        public TelemetriesClient()
            : base(Urls.TelemetryBaseUrl)
        {
        }

        public TelemetriesClient(string apiKey)
            : base(Urls.TelemetryBaseUrl, apiKey)
        {
            _apiKey = apiKey;
        }

        internal TelemetriesClient(string apiKey, HttpClient client)
            : base(client)
        {
            _apiKey = apiKey;
        }

        internal TelemetriesClient(HttpClient client)
            : base(client)
        {
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
    }
}