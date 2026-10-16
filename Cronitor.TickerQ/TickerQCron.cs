using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cronitor.TickerQ;

/// <summary>
/// Translates TickerQ's cron expressions, which NCrontab parses with a seconds field, into ones Cronitor reads the same way, or rejects them.
/// NCrontab: https://github.com/atifaziz/NCrontab/wiki/Crontab-Expression (reversed ranges are swapped, both day fields must match)
/// Cronitor: https://cronitor.io/docs/monitors-api (schedules); a match of either day field fires, verified against the API.
/// </summary>
internal static class TickerQCron
{
    private const int DayOfMonthField = 3;
    private const int DayOfWeekField = 5;

    private static readonly IReadOnlyDictionary<string, int> MonthNumbers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["JAN"] = 1, ["FEB"] = 2, ["MAR"] = 3, ["APR"] = 4, ["MAY"] = 5, ["JUN"] = 6,
        ["JUL"] = 7, ["AUG"] = 8, ["SEP"] = 9, ["OCT"] = 10, ["NOV"] = 11, ["DEC"] = 12,
    };

    private static readonly IReadOnlyDictionary<string, int> DayNumbers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["SUN"] = 0, ["MON"] = 1, ["TUE"] = 2, ["WED"] = 3, ["THU"] = 4, ["FRI"] = 5, ["SAT"] = 6,
    };

    private static readonly IReadOnlyDictionary<string, int> NoNames = new Dictionary<string, int>();

    private static readonly IReadOnlyList<Field> ScheduleFields = new[]
    {
        new Field(0, 59, NoNames),
        new Field(0, 23, NoNames),
        new Field(1, 31, NoNames),
        new Field(1, 12, MonthNumbers),
        new Field(0, 6, DayNumbers),
    };

    internal static bool TryConvert(string cron, out string schedule)
    {
        schedule = null;
        if (string.IsNullOrWhiteSpace(cron))
        {
            return false;
        }

        var fields = cron.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length != 6 || !IsZero(fields[0]) || IsRestricted(fields[DayOfMonthField]) && IsRestricted(fields[DayOfWeekField]))
        {
            return false;
        }

        var converted = new string[ScheduleFields.Count];
        for (var i = 0; i < converted.Length; i++)
        {
            if (!ScheduleFields[i].TryConvert(fields[i + 1], out converted[i]))
            {
                return false;
            }
        }

        schedule = string.Join(" ", converted);
        return true;
    }

    private static bool IsZero(string field) => field == "0" || field == "00";

    private static bool IsRestricted(string field) => field != "*";

    private sealed class Field
    {
        private readonly int _min;
        private readonly int _max;
        private readonly IReadOnlyDictionary<string, int> _names;

        internal Field(int min, int max, IReadOnlyDictionary<string, int> names)
        {
            _min = min;
            _max = max;
            _names = names;
        }

        internal bool TryConvert(string field, out string converted)
        {
            converted = null;
            var elements = field.Split(',');
            if (elements.Length > 1 && elements.Any(element => element.StartsWith("*", StringComparison.Ordinal)))
            {
                return false;
            }

            var convertedElements = new List<string>();
            foreach (var element in elements)
            {
                if (!TryConvertElement(element, out var convertedElement))
                {
                    return false;
                }

                convertedElements.Add(convertedElement);
            }

            converted = string.Join(",", convertedElements);
            return true;
        }

        private bool TryConvertElement(string element, out string converted)
        {
            converted = null;
            var parts = element.Split('/');
            if (parts.Length > 2 || !TryConvertBase(parts[0], out var convertedBase))
            {
                return false;
            }

            if (parts.Length == 1)
            {
                converted = convertedBase;
                return true;
            }

            if (!int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var step) || step < 1)
            {
                return false;
            }

            converted = $"{convertedBase}/{step.ToString(CultureInfo.InvariantCulture)}";
            return true;
        }

        private bool TryConvertBase(string value, out string converted)
        {
            converted = null;
            if (value == "*")
            {
                converted = value;
                return true;
            }

            var bounds = value.Split('-');
            if (bounds.Length > 2 || !TryNumberOf(bounds[0], out var first))
            {
                return false;
            }

            if (bounds.Length == 1)
            {
                converted = first.ToString(CultureInfo.InvariantCulture);
                return true;
            }

            if (!TryNumberOf(bounds[1], out var last) || first > last)
            {
                return false;
            }

            converted = $"{first.ToString(CultureInfo.InvariantCulture)}-{last.ToString(CultureInfo.InvariantCulture)}";
            return true;
        }

        private bool TryNumberOf(string value, out int number)
        {
            if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out number) && !_names.TryGetValue(value, out number))
            {
                return false;
            }

            return number >= _min && number <= _max;
        }
    }
}
