using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests.Monitors
{
    public class GetRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "monitors/:key";
        public string MonitorKey { get; set; }

        public GetRequest(string monitorKey)
        {
            MonitorKey = monitorKey;
        }

        public override Uri ToUri()
        {
            var dictionary = new Dictionary<string, string>
            {
                { ":key", MonitorKey },
            };

            return base.ToUri().Build(dictionary);
        }
    }
}
