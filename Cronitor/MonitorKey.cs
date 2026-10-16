using Cronitor.Extensions;
using System;
using System.Linq;

namespace Cronitor;

public sealed class MonitorKey
{
    private const int MaxLength = 100;

    private readonly string _value;

    public MonitorKey(string value)
    {
        ArgumentHelper.ThrowIfNullOrWhiteSpace(value);

        if (value.IndexOf('/') >= 0)
        {
            throw new ArgumentException($"Cronitor doesn't accept '/' in a monitor key: '{value}'.", nameof(value));
        }

        if (CodePointsIn(value) > MaxLength)
        {
            throw new ArgumentException($"Cronitor accepts at most {MaxLength} characters in a monitor key: '{value}'.", nameof(value));
        }

        if (value.Trim() != value)
        {
            throw new ArgumentException($"Cronitor removes whitespace from the start and end of a monitor key, so it can't have any: '{value}'.", nameof(value));
        }

        _value = value;
    }

    public static bool TryFromName(string name, out MonitorKey key)
    {
        key = null;
        var candidate = name?.Replace('/', '-').Trim();
        if (string.IsNullOrWhiteSpace(candidate) || CodePointsIn(candidate) > MaxLength)
        {
            return false;
        }

        key = new MonitorKey(candidate);
        return true;
    }

    private static int CodePointsIn(string value) => value.Length - value.Count(char.IsLowSurrogate);

    public override string ToString() => _value;

    public static implicit operator MonitorKey(string value) => new MonitorKey(value);

    public static implicit operator string(MonitorKey key) => key?.ToString();
}
