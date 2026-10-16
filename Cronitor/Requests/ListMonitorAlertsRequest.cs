using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests;

public class ListAlertsRequest : BaseRequest
{
    public override string Endpoint { get; set; } = "monitors/:key/alerts";
    public MonitorKey Key { get; set; }

    public ListAlertsRequest(MonitorKey monitorKey)
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
