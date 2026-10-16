using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Cronitor;

[JsonConverter(typeof(MonitorKeyConverter))]
public sealed class MonitorKey : IEquatable<MonitorKey>
{
    private const int MaxLength = 100;

    private readonly string _value;

    public MonitorKey(string value)
        : this(value, validate: true)
    {
    }

    private MonitorKey(string value, bool validate)
    {
        _value = validate ? Validated(value) : value;
    }

    internal static MonitorKey FromCronitor(string value) => new MonitorKey(value, validate: false);

    public static bool TryFromName(string name, out MonitorKey key)
    {
        key = null;
        if (name == null)
        {
            return false;
        }

        var candidate = name.Replace('/', '-').Trim();
        if (ProblemWith(candidate) != null)
        {
            return false;
        }

        key = new MonitorKey(candidate, validate: false);
        return true;
    }

    private static string Validated(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var problem = ProblemWith(value);
        if (problem != null)
        {
            throw new ArgumentException(problem, nameof(value));
        }

        return value;
    }

    private static string ProblemWith(string value)
    {
        if (value.Trim().Length == 0)
        {
            return "A monitor key is required.";
        }

        if (value.IndexOf('/') >= 0)
        {
            return $"Cronitor doesn't accept '/' in a monitor key: '{value}'.";
        }

        if (CodePointsIn(value) > MaxLength)
        {
            return $"Cronitor accepts at most {MaxLength} characters in a monitor key: '{value}'.";
        }

        if (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[value.Length - 1]))
        {
            return $"Cronitor removes whitespace from the start and end of a monitor key, so it can't have any: '{value}'.";
        }

        return null;
    }

    private static int CodePointsIn(string value) => value.Length - value.Count(char.IsLowSurrogate);

    public bool Equals(MonitorKey other) => other is not null && StringComparer.OrdinalIgnoreCase.Equals(_value, other._value);

    public override bool Equals(object obj) => Equals(obj as MonitorKey);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_value);

    public override string ToString() => _value;

    public static implicit operator MonitorKey(string value) => value == null ? null : new MonitorKey(value);

    public static implicit operator string(MonitorKey key) => key?._value;

    public static bool operator ==(MonitorKey left, MonitorKey right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(MonitorKey left, MonitorKey right) => !(left == right);
}
