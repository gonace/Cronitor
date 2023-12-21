using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cronitor.Abstractions
{
    public abstract class BaseResponse<TResponse>
    {
        [JsonProperty("page")]
        public virtual int Page { get; set; } = 1;
        [JsonProperty("page_size")]
        public virtual int PageSize { get; set; } = 50;
        [JsonProperty("total_monitor_count")]
        public virtual int Total { get; set; }
        [JsonProperty("version")]
        public virtual string Version { get; set; }

        public abstract IEnumerable<TResponse> Data { get; set; }
    }
}
