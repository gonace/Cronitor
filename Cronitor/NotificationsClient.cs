namespace Cronitor
{
    public class NotificationsClient
    {
        private readonly string _apiKey;
        private readonly bool _useHttps = true;

        public NotificationsClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public NotificationsClient(string apiKey, bool useHttps)
        {
            _apiKey = apiKey;
            _useHttps = useHttps;
        }
    }
}
