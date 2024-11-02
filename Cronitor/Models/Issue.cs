using Cronitor.Constants;
using Cronitor.Models.Issues;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cronitor.Models
{
    public class Issue
    {
        /// <summary>
        /// The unique identifier for this issue.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// The display name of this issue. Used to identify
        /// the issue in alerts, within the Cronitor application,
        /// and, if published, on your status page(s).
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// The key of the environment this issue is assigned to.
        /// To view your full list of current environment keys,
        ///
        /// see: https://cronitor.io/app/settings/environments
        /// </summary>
        [JsonPropertyName("environment")]
        public string Environment { get; set; }
        /// <summary>
        /// The severity of the issue. Used within the Cronitor
        /// application, and, if published, on your status page(s).
        /// </summary>
        [JsonPropertyName("severity")]
        public IssueSeverity Severity { get; set; }
        /// <summary>
        /// The current state of the issue. Used within the Cronitor
        /// application, and, if published, on your status page(s).
        /// </summary>
        [JsonPropertyName("state")]
        public IssueState State { get; set; }
        /// <summary>
        /// An ISO 8601 formatted timestamp of when the issue started,
        /// e.g. 2023-06-02T09:39:31Z
        /// </summary>
        [JsonPropertyName("started")]
        public string Started { get; set; }
        /// <summary>
        /// Must be a member of your Cronitor team.
        /// An email notification will be sent.
        /// </summary>
        [JsonPropertyName("assigned_to")]
        public string AssignedTo { get; set; }
        /// <summary>
        /// An optional list of status pages where this issue should be
        /// published. Each list entry should be the key of an active
        /// status page in your account.
        /// </summary>
        [JsonPropertyName("statuspages")]
        public IEnumerable<string> StatusPages { get; set; }
        /// <summary>
        /// An optional list of affected status page components that this
        /// issue should be associated with. Each list entry should be the
        /// key of an active status page component in your account.
        ///
        /// Note: The affected component must belong to a status page
        /// included in statuspages.
        /// </summary>
        [JsonPropertyName("affected_components")]
        public IEnumerable<string> AffectedComponents { get; set; }


        #region Read Only Attributes

        [JsonPropertyName("created_by")]
        public Author CreatedBy { get; set; }
        /// <summary>
        /// ISO 8601 formatted timestamp of when the issue was created.
        /// </summary>
        [JsonPropertyName("created")]
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// ISO 8601 formatted timestamp of when the issue was created.
        /// </summary>
        [JsonPropertyName("updated")]
        public DateTime? UpdatedAt { get; set; }

        #endregion
    }
}
