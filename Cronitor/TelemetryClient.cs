using System.Threading.Tasks;
using Cronitor.Commands;
using Cronitor.Constants;

namespace Cronitor
{
    public class TelemetryClient : BaseClient
    {
        private readonly string _apiKey;

        public TelemetryClient(string apiKey)
            : base(Urls.PrimaryBaseUrl, apiKey)
        {
            _apiKey = apiKey;
        }

        public TelemetryClient(string apiKey, bool useHttps)
            : base(Urls.PrimaryBaseUrl, apiKey, useHttps)
        {
            _apiKey = apiKey;
        }

        public TelemetryClient(string apiKey, HttpClient client)
            : base(client)
        {
            _apiKey = apiKey;
        }

        public void Run(string monitorKey, string message = null, string environment = null)
        {
            Task.Run(async () => await RunAsync(monitorKey, message, environment))
                .Wait();
        }

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

        public void Complete(string monitorKey)
        {
            Task.Run(async () => await CompleteAsync(monitorKey))
                .Wait();
        }

        public async Task CompleteAsync(string monitorKey)
        {
            var command = new CompleteCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey);

            await PingAsync(command);
        }

        public void Complete(string monitorKey, string message)
        {
            Task.Run(async () => await CompleteAsync(monitorKey, message))
                .Wait();
        }

        public async Task CompleteAsync(string monitorKey, string message)
        {
            var command = new CompleteCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey)
                .WithMessage(message);

            await PingAsync(command);
        }

        public void Fail(string monitorKey)
        {
            Task.Run(async () => await FailAsync(monitorKey))
                .Wait();
        }

        public async Task FailAsync(string monitorKey)
        {
            var command = new FailCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey);

            await PingAsync(command);
        }

        public void Fail(string monitorKey, string message)
        {
            Task.Run(async () => await FailAsync(monitorKey, message))
                .Wait();
        }

        public async Task FailAsync(string monitorKey, string message)
        {
            var command = new FailCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey)
                .WithMessage(message);

            await PingAsync(command);
        }

        public void Tick(string monitorKey)
        {
            Task.Run(async () => await TickAsync(monitorKey))
                .Wait();
        }

        public async Task TickAsync(string monitorKey)
        {
            var command = new TickCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey);

            await PingAsync(command);
        }

        public void Tick(string monitorKey, string message)
        {
            Task.Run(async () => await TickAsync(monitorKey, message))
                .Wait();
        }

        public async Task TickAsync(string monitorKey, string message)
        {
            var command = new TickCommand()
                .WithApiKey(_apiKey)
                .WithMonitorKey(monitorKey)
                .WithMessage(message);

            await PingAsync(command);
        }


        public void Ping(Command command)
        {
            Task.Run(async () => await PingAsync(command))
                .Wait();
        }

        public async Task PingAsync(Command command)
        {
            await SendAsync(command);
        }
    }
}