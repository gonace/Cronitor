using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests
{
    public class UnpauseMonitorRequest : BaseRequest
    {
        public sealed override string Endpoint { get; set; } = "monitors/:key/pause/:duration";
        public string Key { get; set; }
        public int Duration { get; set; } = 0;

        public UnpauseMonitorRequest(string monitorKey)
        {
            Key = monitorKey;
        }

        public override Uri ToUri()
        {
            var dictionary = new Dictionary<string, string>
            {
                { ":key", Key },
                { ":duration", Duration.ToString() }
            };

            return base.ToUri().Build(dictionary);
        }
    }
}
