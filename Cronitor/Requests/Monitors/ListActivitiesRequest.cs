using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests.Monitors
{
    public class ListActivitiesRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "monitors/:key/activity";
        public string Key { get; set; }

        public ListActivitiesRequest(string monitorKey)
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
