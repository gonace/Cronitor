using System;
using System.Text.Json.Serialization;

namespace Cronitor.Constants;

[JsonConverter(typeof(IssueStateConverter))]
public class IssueState
{
    public static readonly IssueState Unresolved = new IssueState("unresolved");
    public static readonly IssueState Investigating = new IssueState("investigating");
    public static readonly IssueState Identified = new IssueState("identified");
    public static readonly IssueState Monitoring = new IssueState("monitoring");
    public static readonly IssueState Resolved = new IssueState("resolved");
    public static readonly IssueState Update = new IssueState("update");

    public string State { get; set; }


    public IssueState(string state)
    {
        State = state;
    }

    public override string ToString()
    {
        return State;
    }

    public override bool Equals(object obj) => obj is IssueState other && string.Equals(State, other.State, StringComparison.Ordinal);

    public override int GetHashCode() => State != null ? StringComparer.Ordinal.GetHashCode(State) : 0;

    public static bool operator ==(IssueState left, IssueState right) => Equals(left, right);

    public static bool operator !=(IssueState left, IssueState right) => !Equals(left, right);
}
