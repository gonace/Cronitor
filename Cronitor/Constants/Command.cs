using System;
using System.Collections.Generic;
using System.Net.Http;
using Cronitor.Extensions;

namespace Cronitor.Constants
{
    public class Command
    {
        public static readonly Command Run = new Command(HttpMethod.Get, "run");
        public static readonly Command Complete = new Command(HttpMethod.Get, "complete");
        public static readonly Command Fail = new Command(HttpMethod.Get, "fail");
        public static readonly Command Tick = new Command(HttpMethod.Get, "tick");

        public HttpContent Content { get; set; }
        public HttpMethod Method { get; set; }
        public string Endpoint { get; }
        public Uri Uri { get; set; }

        public string ApiKey { get; set; }
        public string MonitorKey { get; set; }

        public Command(HttpMethod method, string endpoint)
        {
            Method = method;
            Endpoint = endpoint;
            Uri = new Uri(Urls.PrimaryBaseUrl);
        }

        public Command SetApiKey(string apiKey)
        {
            ApiKey = apiKey;

            return this;
        }

        public Command SetContent(HttpContent content)
        {
            Content = content;

            return this;
        }

        public Command SetMethod(HttpMethod method)
        {
            Method = method;

            return this;
        }

        public Command SetMonitorKey(string monitorKey)
        {
            MonitorKey = monitorKey;

            return this;
        }

        public override string ToString()
        {
            return Endpoint;
        }

        public Uri ToUri()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new ArgumentException(nameof(ApiKey));
            if (string.IsNullOrWhiteSpace(MonitorKey))
                throw new ArgumentException(nameof(MonitorKey));

            var dictionary = new Dictionary<string, string>
            {
                { ":apiKey", ApiKey },
                { ":key", MonitorKey },
                { ":command", ToString() }
            };

            return Uri.Build(dictionary);
        }

        public string ToUrl()
        {
            return ToUri().ToString();
        }
    }
}
