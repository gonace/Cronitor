﻿using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests.Monitors
{
    public class PauseRequest : BaseRequest
    {
        public sealed override string Endpoint { get; set; } = "monitors/:key/pause";
        public string MonitorKey { get; set; }
        public int? Duration { get; set; }

        public PauseRequest(string monitorKey)
        {
            MonitorKey = monitorKey;
        }

        public PauseRequest(string monitorKey, int duration)
        {
            MonitorKey = monitorKey;
            Duration = duration;
            Endpoint = $"{Endpoint}/:duration";
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
