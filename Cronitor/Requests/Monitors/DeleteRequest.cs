﻿using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Cronitor.Requests.Monitors
{
    public class DeleteRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "monitors/:key";
        public override HttpMethod Method => HttpMethod.Delete;
        public string Key { get; set; }

        public DeleteRequest(string monitorKey)
        {
            Key = monitorKey;
        }

        public override Uri ToUri()
        {
            var dictionary = new Dictionary<string, string>
            {
                { ":key", Key }
            };

            return base.ToUri().Build(dictionary);
        }
    }
}
