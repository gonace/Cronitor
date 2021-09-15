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
            Task.Run(() => RunAsync(monitorKey));
        }

        public async Task RunAsync(string monitorKey)
        {
            await PingAsync(monitorKey, Command.Run);
        }

        public void Complete(string monitorKey)
        {
            Task.Run(() => CompleteAsync(monitorKey));
        }

        public async Task CompleteAsync(string monitorKey)
        {
            await PingAsync(monitorKey, Command.Complete);
        }

        public void Fail(string monitorKey)
        {
            Task.Run(() => FailAsync(monitorKey));
        }

        public async Task FailAsync(string monitorKey)
        {
            await PingAsync(monitorKey, Command.Fail);
        }

        public void Tick(string monitorKey)
        {
            Task.Run(() => TickAsync(monitorKey));
        }

        public async Task TickAsync(string monitorKey)
        {
            await PingAsync(monitorKey, Command.Tick);
        }

        public void Pause(string monitorKey)
        {
            Task.Run(() => PauseAsync(monitorKey));
        }

        public async Task PauseAsync(string monitorKey)
        {
            await PingAsync(monitorKey, Command.Pause);
        }

        public void Unpause(string monitorKey)
        {
            Task.Run(() => UnpauseAsync(monitorKey));
        }

        public async Task UnpauseAsync(string monitorKey)
        {
            await PingAsync(monitorKey, Command.Unpause);
        }


        public void Ping(string monitorKey, Command command)
        {
            Task.Run(() => PingAsync(monitorKey, command));
        }

        public async Task PingAsync(string monitorKey, Command command)
        {
            using (var client = new HttpClient(Urls.PrimaryBaseUrl, _apiKey, _useHttps))
            {
                await client.SendAsync(command);
            }
        }
    }
}
