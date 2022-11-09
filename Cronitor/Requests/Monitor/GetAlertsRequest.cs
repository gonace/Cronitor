using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests.Monitor
{
    public class GetAlertsRequest : Request
    {
        public override string Endpoint { get; set; } = "monitors/:key/alerts";
        public string MonitorKey { get; set; }

        public GetAlertsRequest(string monitorKey)
        {
            MonitorKey = monitorKey;
        }

        public override Uri ToUri()
        {
            var dictionary = new Dictionary<string, string>
            {
                { ":key", MonitorKey }
            };

            return base.ToUri().Build(dictionary);
        }
    }
}
