using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;

namespace Cronitor.Models
{
    public class Monitor
    {
        /// <summary>
        /// A monitor’s unique identifier. The key is used in making API requests to an individual monitor
        /// resource, e.g. https://cronitor.io/api/monitors/:monitorKey
        ///
        /// It is also the required identifier for sending telemetry events.https://cronitor.link/ping/API_KEY/:monitorKey
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }
        /// <summary>
        /// The display name of this monitor. Used to identify your monitor in alerts and within the Cronitor application.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        ///  Monitors are continuously evaluated for both schedule and assertion violations. When Cronitor detects a problem, an alert is sent.
        ///
        /// The syntax for expressing most assertions is:
        ///
        ///  "{assertion} {operator} {value}"
        ///  "metric.duration < 5 min"
        ///  "response.code = 200"
        ///
        /// Assertions that use an access key to a hash data structure — response.json and response.header assertions — are expressed as:
        ///  "{assertion_name} {key} {operator} {value}"
        ///  "response.json new_user.count > 10"
        ///  "response.header X-App-Version = 1.2.3"
        /// </summary>
        [JsonProperty("assertions")]
        public IEnumerable<string> Assertions { get; set; }
        /// <summary>
        /// job & event: number of telemetry events with state='fail' to allow before sending an alert.
        /// check: number of consecutive failed requests allow before sending an alert.
        /// </summary>
        [JsonProperty("failure_tolerance")]
        public int? FailureTolerance { get; set; }
        /// <summary>
        /// The number of seconds that Cronitor should wait after detecting a failure before dispatching an alert.
        /// If the monitor recovers during the grace period no alert will be sent.
        /// </summary>
        [JsonProperty("grace_seconds")]
        public int GraceSeconds { get; set; }
        /// <summary>
        /// Groups are user-defined collections of monitors, that are used to group monitors in the Cronitor application.
        /// Each group has a unique URL with the format https://cronitor.io/app/groups/:groupKey
        /// </summary>
        [JsonProperty("group")]
        public string Group { get; set; }
        /// <summary>
        /// A JSON serializable set of arbitrary key/value pairs that contain useful information about a job.
        ///
        /// For example, cronitor-kubernetes stores fields like backoffLimit and startingDeadlineSeconds.
        /// </summary>
        [JsonProperty("metadata")]
        public object Metadata { get; set; }
        /// <summary>
        /// A useful place to provide additional context/troubleshooting information about the job/system being monitored.
        /// The note is sent in alerts (excluding SMS), and are accessible from the monitor details view in the Cronitor application.
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; }
        /// <summary>
        /// Configure where alerts sent when Cronitor detects an error. Additionally, notify can be used to specify
        /// occurrence based alerts - alerts based on receiving telemetry pings.
        ///
        /// All accounts have a default Notification List(key= "default"). If notify is left empty or omitted, the default list will be used.
        ///
        /// Array: An array of Notification List keys. Notification Lists are used for configuring how alerts are sent to your team.
        ///   "notify": [‘devops-channel’]
        ///
        /// Hash: To specify separate notification preferences a hash containing the keys "alerts" and "events" can be used.
        ///
        /// The "alerts" key accepts an array of Notification List keys. The "events" key accepts a hash with keys corresponding to the
        /// lifecycle telemetry events - run, complete.If these keys are set to true, Cronitor will send a notification when the event occurs.
        ///   { "notify": { "alerts": [‘foo-bar’], "events": { "complete": true} }
        /// </summary>
        [JsonProperty("notify")]
        public IEnumerable<string> Notify { get; set; }
        /// <summary>
        /// The platform attribute is used in conjunction with the type attribute to tell Cronitor about where and how a monitor is being run beyond.
        /// </summary>
        [JsonProperty("platform")]
        public virtual string Platform { get; set; }
        /// <summary>
        /// Interval expression telling Cronitor how long to wait before sending follow up alerts after a monitor fails (and does not recover).
        ///
        /// If an integer is provided it will be interpreted as `X hours`
        ///
        /// After 10 alerts, Cronitor will mute alerting until the monitor recovers.
        /// </summary>
        [JsonProperty("realert_interval")]
        public string AlertInterval { get; set; }
        /// <summary>
        /// Schedule has different meanings depending on the monitor type.
        ///
        /// job & event: the schedule tells Cronitor when to expect telemetry events from your system. If events are not received on schedule, an alert is sent.
        ///
        /// An interval expression (‘every 5 minutes’) or a cron expression(‘0 0 * * *’) must be used.
        ///
        /// check: the schedule is used to tell Cronitor how frequently to make requests to the resource being monitored.
        ///
        /// An interval expression must be used.The range of accepted values is 30 seconds to 1 hour.e.g. ‘every 2 minutes’.
        /// </summary>
        [JsonProperty("schedule")]
        public string Schedule { get; set; }
        /// <summary>
        /// Number of missed scheduled executions to allow before sending an alert.
        /// </summary>
        [JsonProperty("schedule_tolerance")]
        public int? ScheduleTolerance { get; set; }
        /// <summary>
        /// Tags are user provided strings that can be used to associate and filter monitors in the Cronitor dashboard. Use tags however you would like.
        /// </summary>
        [JsonProperty("tags")]
        public IEnumerable<string> Tags { get; set; }
        /// <summary>
        /// The timezone your system is running in.
        /// </summary>
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        /// <summary>
        /// The type attribute determines which of Cronitor's monitoring capabilities are used to assess the health of your system.
        /// Based on the type provided, other attributes may be required — e.g. the checks type requires the request attribute to be set.
        ///
        /// jobs monitor the execution of a backend job(cron job, windows scheduled task, etc).
        /// This involves tracking the lifecycle of the job - the start_time, end_time and exit_state of the job.
        ///
        /// events monitor the health of a system via telemetry events.There is no lifecycle to measure, the occurrence (or absence)
        /// of healthy/unhealthy events, as well as data passed as custom metrics are used to determine the health of an event monitor.
        ///
        /// checks monitor websites, APIs, proxy servers, cloud storage providers(e.g.S3) or any other HTTP/TCP/UDP networked device.
        /// </summary>
        [JsonProperty("type")]
        public virtual string Type { get; set; }

        //TODO: Is these used? (documented here: https://cronitor.io/docs/monitor-api-v3)
        ///// <summary>
        ///// Where/how you wish to be contacted when a monitor's alerting is triggered. The following key/value pairs are all options,
        ///// at least one of which must not be empty. Note: When extending notification template(s), passing an empty array will
        ///// overload the templated notification settings for that key.
        ///// </summary>
        //[JsonProperty("notifications")]
        //public dynamic Notifications { get; set; }
        ///// <summary>
        ///// when creating a monitor you must specify the rules that will trigger alerts to be sent.
        ///// </summary>
        //[JsonProperty("rules")]
        //public dynamic Rules { get; protected set; }


        #region Read Only Attributes

        [JsonProperty("group_name")]
        public string GroupName { get; protected set; }
        [JsonProperty("latest_event")]
        public Event LatestEvent { get; private set; }
        [JsonProperty("latest_events")]
        public IEnumerable<Event> LatestEvents { get; protected set; }
        [JsonProperty("latest_incident")]
        public Incident LatestIncident { get; protected set; }
        [JsonProperty("latest_invocations")]
        public dynamic LatestInvocations { get; protected set; }
        /// <summary>
        /// Whether the monitor is currently disabled.
        /// </summary>
        [JsonProperty("disabled")]
        public bool? Disabled { get; protected set; }
        /// <summary>
        /// Whether the monitor has received a telemetry event.
        /// </summary>
        [JsonProperty("initialized")]
        public bool? Initialized { get; protected set; }
        /// <summary>
        /// Whether the monitor is currently passing or failing.
        /// </summary>
        [JsonProperty("passing")]
        public bool? Passing { get; protected set; }
        /// <summary>
        /// Whether sending alerts is currently paused.
        /// </summary>
        [JsonProperty("paused")]
        public bool? Paused { get; protected set; }
        /// <summary>
        /// Whether a job is running (only applicable for type: job).
        /// </summary>
        [JsonProperty("running")]
        public bool? Running { get; protected set; }
        [JsonProperty("has_duration_history")]
        public bool? HasDurationHistory { get; protected set; }
        [JsonProperty("request_interval_seconds")]
        public int? RequestIntervalSeconds { get; protected set; }
        [JsonProperty("next_expected_at")]
        public long? ExpectedAt { get; protected set; }
        /// <summary>
        /// ISO 8601 formatted timestamp of when the monitor was created.
        /// </summary>
        [JsonProperty("created")]
        public DateTime CreatedAt { get; protected set; }

        #endregion


        [JsonConstructor]
        private Monitor()
        {
        }

        public Monitor(string key)
        {
            Key = key;
        }

        public Monitor With(Expression<Func<Monitor, object>> expression, object value)
        {
            if (expression.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(this, value, null);
                }
            }

            return this;
        }

        protected static string GenerateKey()
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ123456790";

            using (var crypto = new RNGCryptoServiceProvider())
            {
                var tokenBuffer = new byte[6];
                crypto.GetBytes(tokenBuffer);
                var chars = new char[6];
                var count = allowedChars.Length;

                for (var i = 0; i < 6; i++)
                {
                    chars[i] = allowedChars[tokenBuffer[i] % count];
                }
                return new string(chars);
            }
        }
    }
}
