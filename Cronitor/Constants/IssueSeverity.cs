using System;
using System.Text.Json.Serialization;

namespace Cronitor.Constants;

[JsonConverter(typeof(IssueSeverityConverter))]
public class IssueSeverity
{
    public static readonly IssueSeverity MissingData = new IssueSeverity("missing_data");
    public static readonly IssueSeverity Operational = new IssueSeverity("operational");
    public static readonly IssueSeverity Maintenance = new IssueSeverity("maintenance");
    public static readonly IssueSeverity DegradedPerformance = new IssueSeverity("degraded_performance");
    public static readonly IssueSeverity MinorOutage = new IssueSeverity("minor_outage");
    public static readonly IssueSeverity Outage = new IssueSeverity("outage");

    public string Severity { get; set; }


    public IssueSeverity(string severity)
    {
        Severity = severity;
    }

    public override string ToString()
    {
        return Severity;
    }

    public override bool Equals(object obj) => obj is IssueSeverity other && string.Equals(Severity, other.Severity, StringComparison.Ordinal);

    public override int GetHashCode() => Severity != null ? StringComparer.Ordinal.GetHashCode(Severity) : 0;

    public static bool operator ==(IssueSeverity left, IssueSeverity right) => Equals(left, right);

    public static bool operator !=(IssueSeverity left, IssueSeverity right) => !Equals(left, right);
}
