using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests.Monitors
{
    public class UnpauseRequest : Request
    {
        public sealed override string Endpoint { get; set; } = "monitors/:key/pause/:duration";
        public string MonitorKey { get; set; }
        public int Duration { get; set; } = 0;

        public UnpauseRequest(string monitorKey)
        {
            MonitorKey = monitorKey;
        }

        public override Uri ToUri()
        {
            var dictionary = new Dictionary<string, string>
            {
                { ":key", MonitorKey },
                { ":duration", Duration.ToString() }
            };

            return base.ToUri().Build(dictionary);
        }
    }
}
