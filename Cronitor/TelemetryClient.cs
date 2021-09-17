using System;
using System.Threading.Tasks;
using Cronitor.Commands;
using Cronitor.Constants;

namespace Cronitor
{
    public class TelemetryClient : BaseClient
    {
        public TelemetryClient(string apiKey)
            : base(apiKey)
        {
            BaseUri = new Uri(Urls.PrimaryBaseUrl);
        }

        public TelemetryClient(string apiKey, bool useHttps)
            : base(apiKey, useHttps)
        {
            BaseUri = new Uri(Urls.PrimaryBaseUrl);
        }

        public void Run(string monitorKey)
        {
            Task.Run(async () => await RunAsync(monitorKey))
                .Wait();
        }

        public async Task RunAsync(string monitorKey)
        {
            var command = new RunCommand()
                .WithApiKey(ApiKey)
                .WithMonitorKey(monitorKey);

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
                .WithApiKey(ApiKey)
                .WithMonitorKey(monitorKey);

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
                .WithApiKey(ApiKey)
                .WithMonitorKey(monitorKey);

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
                .WithApiKey(ApiKey)
                .WithMonitorKey(monitorKey);

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
