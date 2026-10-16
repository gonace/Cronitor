using Cronitor.Abstractions;
using Cronitor.Extensions;
using System;
using System.Collections.Generic;

namespace Cronitor.Requests;

public class GetMonitorRequest : BaseRequest
{
    public override string Endpoint { get; set; } = "monitors/:key";
    public MonitorKey Key { get; set; }

    public GetMonitorRequest(MonitorKey monitorKey)
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
