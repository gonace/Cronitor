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

        var problem = ProblemWith(value);
        if (problem != null)
        {
            throw new ArgumentException(problem, nameof(value));
        }

        _value = value;
    }

    public static bool TryCreate(string value, out MonitorKey key)
    {
        key = string.IsNullOrWhiteSpace(value) || ProblemWith(value) != null ? null : new MonitorKey(value);
        return key != null;
    }

    public static bool TryFromName(string name, out MonitorKey key) => TryCreate(name?.Replace('/', '-').Trim(), out key);

    private static string ProblemWith(string value)
    {
        if (value.IndexOf('/') >= 0)
        {
            return $"Cronitor doesn't accept '/' in a monitor key: '{value}'.";
        }

        if (HasIncompleteCharacter(value))
        {
            return $"A monitor key must be valid Unicode, but '{value}' has an incomplete character.";
        }

        if (CodePointsIn(value) > MaxLength)
        {
            return $"Cronitor accepts at most {MaxLength} characters in a monitor key: '{value}'.";
        }

        if (value.Trim() != value)
        {
            return $"Cronitor removes whitespace from the start and end of a monitor key, so it can't have any: '{value}'.";
        }

        return null;
    }

    private static bool HasIncompleteCharacter(string value)
    {
        for (var i = 0; i < value.Length; i++)
        {
            if (char.IsSurrogatePair(value, i))
            {
                i++;
            }
            else if (char.IsSurrogate(value[i]))
            {
                return true;
            }
        }

        return false;
    }

    private static int CodePointsIn(string value) => value.Length - value.Count(char.IsLowSurrogate);

    public override bool Equals(object obj) => obj is MonitorKey other && StringComparer.OrdinalIgnoreCase.Equals(_value, other._value);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_value);

    public override string ToString() => _value;

    public static implicit operator MonitorKey(string value) => new MonitorKey(value);

    public static implicit operator string(MonitorKey key) => key?.ToString();
}
