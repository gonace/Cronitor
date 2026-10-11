using Cronitor.Abstractions;
using Cronitor.Attributes;
using Cronitor.Constants;
using System.Collections.Generic;

namespace Cronitor.Requests
{
    /// <summary>
    /// Lists issues, optionally filtered and ordered. Filters that are not set are not sent.
    /// </summary>
    public class ListIssueRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "issues";
        [QueryStringProperty("page")]
        public int Page { get; set; } = 1;
        /// <summary>
        /// Issues per page, up to 1000.
        /// </summary>
        [QueryStringProperty("pageSize")]
        public int? PageSize { get; set; }
        [QueryStringProperty("state")]
        public List<IssueState> States { get; } = new();
        [QueryStringProperty("severity")]
        public List<IssueSeverity> Severities { get; } = new();
        /// <summary>
        /// Keys of job monitors.
        /// </summary>
        [QueryStringProperty("job")]
        public List<string> Jobs { get; } = new();
        /// <summary>
        /// Keys of check monitors.
        /// </summary>
        [QueryStringProperty("check")]
        public List<string> Checks { get; } = new();
        /// <summary>
        /// Keys of heartbeat monitors.
        /// </summary>
        [QueryStringProperty("heartbeat")]
        public List<string> Heartbeats { get; } = new();
        /// <summary>
        /// Keys of site monitors.
        /// </summary>
        [QueryStringProperty("site")]
        public List<string> Sites { get; } = new();
        /// <summary>
        /// Keys of monitor groups.
        /// </summary>
        [QueryStringProperty("group")]
        public List<string> Groups { get; } = new();
        [QueryStringProperty("tag")]
        public List<string> Tags { get; } = new();
        [QueryStringProperty("type")]
        public List<MonitorType> Types { get; } = new();
        /// <summary>
        /// Keys of status page components.
        /// </summary>
        [QueryStringProperty("component")]
        public List<string> Components { get; } = new();
        /// <summary>
        /// Key of a status page.
        /// </summary>
        [QueryStringProperty("statuspage")]
        public string StatusPage { get; set; }
        [QueryStringProperty("env")]
        public string Environment { get; set; }
        /// <summary>
        /// Searches issue names and monitor names, keys and codes.
        /// </summary>
        [QueryStringProperty("search")]
        public string Search { get; set; }
        /// <summary>
        /// How far back to look, such as "24h", "7d" or "30d".
        /// </summary>
        [QueryStringProperty("time")]
        public string Time { get; set; }
        [QueryStringProperty("orderBy")]
        public IssueOrder OrderBy { get; set; }
        [QueryStringProperty("withStatusPageDetails", true)]
        public bool? WithStatusPageDetails { get; set; }
        [QueryStringProperty("withMonitorDetails", true)]
        public bool? WithMonitorDetails { get; set; }
        [QueryStringProperty("withAlertDetails", true)]
        public bool? WithAlertDetails { get; set; }
        [QueryStringProperty("withComponentDetails", true)]
        public bool? WithComponentDetails { get; set; }
    }
}
