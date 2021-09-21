using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using Cronitor.Attributes;
using Cronitor.Constants;
using Cronitor.Extensions;

namespace Cronitor.Commands
{
    public class Command
    {
        public HttpContent Content { get; set; }
        public HttpMethod Method { get; set; }
        public virtual string Endpoint { get; }
        public Uri Uri { get; set; }

        public string ApiKey { get; set; }
        public string MonitorKey { get; set; }

        [QueryString("env")]
        public string Environment { get; set; }
        [QueryString("host")]
        public string Host { get; set; }
        [QueryString("message")]
        public string Message { get; private set; }
        [QueryString("metric")]
        public string Metric { get; set; }
        [QueryString("series")]
        public string Series { get; set; }
        [QueryString("status_code")]
        public string Status { get; set; }


        protected Command()
        {
            Uri = new Uri(Urls.PrimaryBaseUrl);
        }

        public Command(HttpMethod method, string endpoint)
        {
            Method = method;
            Endpoint = endpoint;
            Uri = new Uri(Urls.PrimaryBaseUrl);
        }

        public Command WithApiKey(string apiKey)
        {
            ApiKey = apiKey;

            return this;
        }

        public Command WithContent(HttpContent content)
        {
            Content = content;

            return this;
        }

        public Command WithMetric(Metric metric, string value)
        {
            Metric = $"{metric}:{value}";

            return this;
        }

        public Command WithMetric(Metric metric, int value) => WithMetric(metric, value.ToString(CultureInfo.InvariantCulture));
        public Command WithMetric(Metric metric, decimal value) => WithMetric(metric, value.ToString(CultureInfo.InvariantCulture));
        public Command WithMetric(Metric metric, double value) => WithMetric(metric, value.ToString(CultureInfo.InvariantCulture));

        public Command WithEnvironment(string environment)
        {
            Environment = environment;

            return this;
        }

        public Command WithHost(string host)
        {
            Host = host;

            return this;
        }

        public Command WithMessage(string message)
        {
            if (message.StartsWith("'") &&
                message.EndsWith("'"))
            {
                Message = message;
            }
            else
            {
                Message = $"'{message}'";
            }

            return this;
        }

        public Command WithMonitorKey(string monitorKey)
        {
            MonitorKey = monitorKey;

            return this;
        }

        public Command WithSeries(string series)
        {
            Series = series;

            return this;
        }

        public Command WithStatus(string status)
        {
            Status = status;

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

            return Uri.Build(dictionary).AddQueryString(this.ToQueryString());
        }

        public string ToUrl()
        {
            return ToUri().ToString();
        }
    }
}