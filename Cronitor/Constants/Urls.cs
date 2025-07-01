using System;

namespace Cronitor.Constants
{
    public static class Urls
    {
        public static Uri DefaultApiUrl = new Uri("https://cronitor.io/api/");
        /// <summary>
        /// The Telemetry API is hosted on a separate domain, cronitor.link, and uses
        /// an API key embedded in the URL to authenticate requests.
        ///
        /// <see href="https://cronitor.io/docs/api#api-authentication">API Authentication</see>
        /// </summary>
        public static Uri TelemetryBaseUrl = new Uri("https://cronitor.link/p/:apiKey/:key/:command");
    }
}
