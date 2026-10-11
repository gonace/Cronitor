using Cronitor.Abstractions;
using Cronitor.Attributes;
using Cronitor.Constants;
using System.Collections.Generic;

namespace Cronitor.Requests
{
    /// <summary>
    /// Lists monitors, optionally filtered and sorted. Filters that are not set are not sent.
    /// </summary>
    public class ListMonitorRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "monitors";
        [QueryStringProperty("page")]
        public int Page { get; set; } = 1;
        [QueryStringProperty("pageSize")]
        public int? PageSize { get; set; }
        [QueryStringProperty("type")]
        public List<MonitorType> Types { get; } = new();
        [QueryStringProperty("tag")]
        public List<string> Tags { get; } = new();
        [QueryStringProperty("state")]
        public List<MonitorState> States { get; } = new();
        [QueryStringProperty("group")]
        public string Group { get; set; }
        [QueryStringProperty("env")]
        public string Environment { get; set; }
        /// <summary>
        /// Searches monitor names and keys.
        /// </summary>
        [QueryStringProperty("search")]
        public string Search { get; set; }
        [QueryStringProperty("sort")]
        public MonitorSort Sort { get; set; }
    }
}
