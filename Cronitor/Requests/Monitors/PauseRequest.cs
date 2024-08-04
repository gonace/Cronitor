using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests.Monitors
{
    public class PauseRequest : BaseRequest
    {
        public sealed override string Endpoint { get; set; } = "monitors/:key/pause";
        public string Key { get; set; }
        public int? Duration { get; set; }

        public PauseRequest(string monitorKey)
        {
            Key = monitorKey;
        }

        public PauseRequest(string monitorKey, int duration)
        {
            Key = monitorKey;
            Duration = duration;
            Endpoint = $"{Endpoint}/:duration";
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
