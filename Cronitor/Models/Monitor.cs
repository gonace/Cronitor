using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using Cronitor.Assertions;
using Cronitor.Attributes;
using Cronitor.Scheduling;

namespace Cronitor.Models;

public class Monitor
{
    /// <summary>
    /// A monitor’s unique identifier. The key is used in making API requests to an individual monitor
    /// resource, e.g. https://cronitor.io/api/monitors/:monitorKey
    ///
    /// It is also the required identifier for sending telemetry events.https://cronitor.link/ping/API_KEY/:monitorKey
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }
    /// <summary>
    /// The display name of this monitor. Used to identify your monitor in alerts and within the Cronitor application.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
    /// <summary>
    ///  Monitors are continuously evaluated for both schedule and assertion violations. When Cronitor detects a problem, an alert is sent.
    ///
    /// The syntax for expressing most assertions is:
    ///
    ///  "{assertion} {operator} {value}"
    ///  "response.code = 200"
    ///
    /// Assertions that use an access key to a hash data structure — response.json and response.header assertions — are expressed as:
    ///  "{assertion_name} {key} {operator} {value}"
    ///  "response.json new_user.count > 10"
    ///  "response.header X-App-Version = 1.2.3"
    /// </summary>
    [JsonPropertyName("assertions")]
    public IEnumerable<AssertionRule> Assertions { get; set; }
    /// <summary>
    /// job and event: number of telemetry events with state='fail' to allow before sending an alert.
    /// check: number of consecutive failed requests allow before sending an alert.
    /// </summary>
    [JsonPropertyName("failure_tolerance")]
    public int? FailureTolerance { get; set; }
    /// <summary>
    /// The number of seconds that Cronitor should wait after detecting a failure before dispatching an alert.
    /// If the monitor recovers during the grace period, no alert will be sent.
    /// </summary>
    [JsonPropertyName("grace_seconds")]
    public int GraceSeconds { get; set; }
    /// <summary>
    /// Groups are user-defined collections of monitors, which are used to group monitors in the Cronitor application.
    /// Each group has a unique URL with the format https://cronitor.io/app/groups/:groupKey
    /// </summary>
    [JsonPropertyName("group")]
    public string Group { get; set; }
    /// <summary>
    /// A JSON serializable set of arbitrary key/value pairs that contain useful information about a job.
    ///
    /// For example, cronitor-kubernetes stores fields like backoffLimit and startingDeadlineSeconds.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object Metadata { get; set; }
    /// <summary>
    /// A useful place to provide additional context/troubleshooting information about the job/system being monitored.
    /// The note is sent in alerts (excluding SMS) and is accessible from the monitor details view in the Cronitor application.
    /// </summary>
    [JsonPropertyName("note")]
    public string Note { get; set; }
    /// <summary>
    /// Configure where alerts sent when Cronitor detects an error. Additionally, notify can be used to specify
    /// occurrence-based alerts - alerts based on receiving telemetry pings.
    ///
    /// All accounts have a default Notification List(key= "default"). If notify is left empty or omitted, the default list will be used.
    ///
    /// Array: An array of Notification List keys. Notification Lists are used for configuring how alerts are sent to your team.
    ///   "notify": [‘devops-channel’]
    ///
    /// Hash: To specify separate notification preferences, a hash containing the keys "alerts" and "events" can be used.
    ///
    /// The "alerts" key accepts an array of Notification List keys. The "events" key accepts a hash with keys corresponding to the
    /// lifecycle telemetry events - run, complete. If these keys are set to true, Cronitor will send a notification when the event occurs.
    ///   { "notify": { "alerts": [‘foo-bar’], "events": { "complete": true} }
    /// </summary>
    [JsonPropertyName("notify")]
    public IEnumerable<string> Notify { get; set; }
    /// <summary>
    /// Keys of the environments the monitor belongs to, e.g. "production".
    /// </summary>
    [JsonPropertyName("environments")]
    public IEnumerable<string> Environments { get; set; }
    /// <summary>
    /// Keys of the status pages that show this monitor's status.
    /// </summary>
    [JsonPropertyName("statuspages")]
    public IEnumerable<string> StatusPages { get; set; }
    /// <summary>
    /// How many alerts are sent in a row before alerting pauses until the monitor recovers (1–1000).
    /// </summary>
    [JsonPropertyName("consecutive_alert_threshold")]
    public int? ConsecutiveAlertThreshold { get; set; }
    /// <summary>
    /// The platform attribute is used in conjunction with the type attribute to tell Cronitor about where and how a monitor is being run beyond.
    /// </summary>
    [JsonPropertyName("platform")]
    public virtual string Platform { get; set; }
    /// <summary>
    /// Interval expression telling Cronitor how long to wait before sending follow-up alerts after a monitor fails (and does not recover).
    ///
    /// If an integer is provided, it will be interpreted as `X hours`
    ///
    /// After 10 alerts, Cronitor will mute alerting until the monitor recovers.
    /// </summary>
    [JsonPropertyName("realert_interval")]
    public string RealertInterval { get; set; }
    /// <summary>
    /// Schedule has different meanings depending on the monitor type.
    ///
    /// job and event: the schedule tells Cronitor when to expect telemetry events from your system. If events are not received on schedule, an alert is sent.
    ///
    /// An interval expression (‘every 5 minutes’) or a cron expression ('0 0 * * *’) must be used.
    ///
    /// check: the schedule is used to tell Cronitor how frequently to make requests to the resource being monitored.
    ///
    /// An interval expression must be used. The range of accepted values is 30 seconds to 1 hour.e.g. ‘every 2 minutes’
    /// </summary>
    [JsonPropertyName("schedules")]
    public IEnumerable<ScheduleExpression> Schedules { get; set; }
    /// <summary>
    /// A single schedule: reads the first of <see cref="Schedules"/>, and sets <see cref="Schedules"/> to just this one.
    /// </summary>
    [JsonIgnore]
    public ScheduleExpression Schedule
    {
        get => Schedules?.FirstOrDefault();
        set => Schedules = value == null ? null : new[] { value };
    }
    /// <summary>
    /// Number of missed scheduled executions to allow before sending an alert.
    /// </summary>
    [JsonPropertyName("schedule_tolerance")]
    public int? ScheduleTolerance { get; set; }
    /// <summary>
    /// Tags are user provided strings that can be used to associate and filter monitors in the Cronitor dashboard. Use tags however you would like.
    /// </summary>
    [JsonPropertyName("tags")]
    public IEnumerable<string> Tags { get; set; }
    /// <summary>
    /// The timezone your system is running in.
    /// </summary>
    [JsonPropertyName("timezone")]
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
    [JsonPropertyName("type")]
    public virtual string Type { get; set; }


    #region Read Only Attributes

    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("attributes")]
    public MonitorAttributes Attributes { get; protected set; }
    /// <summary>
    /// The display name of the monitor's group, e.g. "Database".
    /// </summary>
    [JsonIgnore]
    public string GroupName => Attributes?.GroupName;
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("latest_event")]
    public Event LatestEvent { get; private set; }
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("latest_events")]
    public IEnumerable<Event> LatestEvents { get; protected set; }
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("badges")]
    public MonitorBadges Badges { get; protected set; }
    /// <summary>
    /// Names of custom metrics seen in the last 7 days. Only returned when getting a single monitor.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("metric_names")]
    public IEnumerable<string> MetricNames { get; protected set; }
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("latest_issue")]
    public MonitorIssue LatestIssue { get; protected set; }
    [Obsolete("Cronitor's API sends latest_issue; use LatestIssue. Will be removed in the next major version (2027).")]
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("latest_incident")]
    public Incident LatestIncident { get; protected set; }
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("latest_invocations")]
    public dynamic LatestInvocations { get; protected set; }
    /// <summary>
    /// Whether the monitor is currently disabled.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; protected set; }
    /// <summary>
    /// Whether the monitor has received a telemetry event.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("initialized")]
    public bool? Initialized { get; protected set; }
    /// <summary>
    /// Whether the monitor is currently passing or failing.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("passing")]
    public bool? Passing { get; protected set; }
    /// <summary>
    /// Whether sending alerts is currently paused. Set it to create a monitor that starts paused.
    /// </summary>
    [JsonPropertyName("paused")]
    public bool? Paused { get; set; }
    /// <summary>
    /// Whether a job is running (only applicable for type: job).
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("running")]
    public bool? Running { get; protected set; }
    [Obsolete("Not part of Cronitor's documented Monitor API. Will be removed in the next major version (2027).")]
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("has_duration_history")]
    public bool? HasDurationHistory { get; protected set; }
    [Obsolete("Not part of Cronitor's documented Monitor API; set a check's frequency with Schedule. Will be removed in the next major version (2027).")]
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("request_interval_seconds")]
    public int? RequestIntervalSeconds { get; protected set; }
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("next_expected_at")]
    public long? ExpectedAt { get; protected set; }

    /// <summary>
    /// ISO 8601 formatted timestamp of when the monitor was created.
    /// </summary>
    [JsonInclude]
    [ResponseOnly]
    [JsonPropertyName("created")]
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

            if (property == null)
            {
                return this;
            }

            property.SetValue(this, value, null);
        }

        return this;
    }
}
