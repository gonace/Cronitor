using System;
using System.Collections.Generic;
using Cronitor.Extensions;

namespace Cronitor.Requests.Monitor
{
    public class GetPingsRequest : Request
    {
        public override string Endpoint { get; set; } = "monitors/:key/pings";
        public string MonitorKey { get; set; }

        public GetPingsRequest(string monitorKey)
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
