using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Abstractions
{
    public abstract class BaseResponse<TResponse>
    {
        [JsonPropertyName("page")]
        public virtual int Page { get; set; } = 1;
        [JsonPropertyName("page_size")]
        public virtual int PageSize { get; set; } = 50;
        [JsonPropertyName("total_monitor_count")]
        public virtual int Total { get; set; }
        [JsonPropertyName("version")]
        public virtual string Version { get; set; }

        public abstract IEnumerable<TResponse> Items { get; set; }
    }
}
