using System.Text.Json;
using Cronitor.Abstractions;
using Cronitor.Commands;
using Cronitor.Constants;
using Cronitor.Internals;
using System.Threading.Tasks;

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
            : base(Urls.PrimaryBaseUrl)
        {
        }

        public TelemetriesClient(string apiKey)
            : base(Urls.ApiUrl, apiKey)
        {
        }

        public TelemetriesClient(string apiKey, JsonSerializerOptions jsonSerializerOptions)
            : base(Urls.ApiUrl, apiKey, jsonSerializerOptions)
        {
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
            Task.Run(async () => await RunAsync(monitorKey, message, environment))
                .Wait();

        public async Task RunAsync(string monitorKey, string message = null, string environment = null)
        {
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
            Task.Run(async () => await CompleteAsync(monitorKey, message, environment))
                .Wait();

        public async Task CompleteAsync(string monitorKey, string message = null, string environment = null)
        {
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
            Task.Run(async () => await FailAsync(monitorKey, message, environment))
                .Wait();

        public async Task FailAsync(string monitorKey, string message = null, string environment = null)
        {
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
            Task.Run(async () => await TickAsync(monitorKey, message, environment))
                .Wait();

        public async Task TickAsync(string monitorKey, string message = null, string environment = null)
        {
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
            Task.Run(async () => await PingAsync(command))
                .Wait();

        public async Task PingAsync(Command command) =>
            await SendAsync(command);
    }
}