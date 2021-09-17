using System.Threading.Tasks;
using Cronitor.Constants;

namespace Cronitor
{
    public class TelemetryClient
    {
        private readonly string _apiKey;
        private readonly bool _useHttps = true;

        public TelemetryClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public TelemetryClient(string apiKey, bool useHttps)
        {
            _apiKey = apiKey;
            _useHttps = useHttps;
        }

        public void Run(string monitorKey)
        {
            Task.Run(async () => await RunAsync(monitorKey))
                .Wait();
        }

        public async Task RunAsync(string monitorKey)
        {
            await PingAsync(Command.Run, monitorKey);
        }

        public void Complete(string monitorKey)
        {
            Task.Run(async () => await CompleteAsync(monitorKey))
                .Wait();
        }

        public async Task CompleteAsync(string monitorKey)
        {
            await PingAsync(Command.Complete, monitorKey);
        }

        public void Fail(string monitorKey)
        {
            Task.Run(async () => await FailAsync(monitorKey))
                .Wait();
        }

        public async Task FailAsync(string monitorKey)
        {
            await PingAsync(Command.Fail, monitorKey);
        }

        public void Tick(string monitorKey)
        {
            Task.Run(async () => await TickAsync(monitorKey))
                .Wait();
        }

        public async Task TickAsync(string monitorKey)
        {
            await PingAsync(Command.Tick, monitorKey);
        }


        public void Ping(Command command, string monitorKey)
        {
            Task.Run(async () => await PingAsync(command, monitorKey))
                .Wait();
        }

        public async Task PingAsync(Command command, string monitorKey)
        {
            using (var client = new HttpClient(Urls.PrimaryBaseUrl, _apiKey, _useHttps))
            {
                await client.SendAsync(command, monitorKey);
            }
        }
    }
}
