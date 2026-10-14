using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cronitor.Quartz;

/// <summary>
/// Translates Quartz's cron expressions into ones Cronitor reads the same way, or rejects them.
/// Quartz: https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html (days of week 1 = Sunday)
/// Cronitor: https://cronitor.io/docs/monitors-api (schedules); L, n#k, steps and names verified against the API.
/// </summary>
internal static class QuartzCron
{
    private const int DayOfMonthField = 2;
    private const int MonthField = 3;
    private const int DayOfWeekField = 4;

    private static readonly IReadOnlyDictionary<string, int> MonthNumbers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["JAN"] = 1, ["FEB"] = 2, ["MAR"] = 3, ["APR"] = 4, ["MAY"] = 5, ["JUN"] = 6,
        ["JUL"] = 7, ["AUG"] = 8, ["SEP"] = 9, ["OCT"] = 10, ["NOV"] = 11, ["DEC"] = 12,
    };

    private static readonly IReadOnlyDictionary<string, int> DayNumbers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["SUN"] = 1, ["MON"] = 2, ["TUE"] = 3, ["WED"] = 4, ["THU"] = 5, ["FRI"] = 6, ["SAT"] = 7,
    };

    private static readonly IReadOnlyDictionary<string, int> NoNames = new Dictionary<string, int>();

    private static readonly Regex Range = new Regex(@"([A-Za-z0-9]+)-([A-Za-z0-9]+)", RegexOptions.CultureInvariant);
    private static readonly Regex DayOfWeekNumber = new Regex(@"(?<![/#\d])\d+", RegexOptions.CultureInvariant);

    internal static bool TryConvert(string cron, out string schedule)
    {
        schedule = null;
        if (string.IsNullOrWhiteSpace(cron))
        {
            return false;
        }

        var fields = cron.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length == 7 && fields[6] == "*")
        {
            fields = fields.Take(6).ToArray();
        }

        if (fields.Length != 6 || !IsZero(fields[0]))
        {
            return false;
        }

        fields = fields.Skip(1).ToArray();
        if (fields.Where(HasReversedRange).Any() || IsRestricted(fields[DayOfMonthField]) && IsRestricted(fields[DayOfWeekField]))
        {
            return false;
        }

        var dayOfMonth = UnspecifiedAsAny(fields[DayOfMonthField]);
        var dayOfWeek = UnspecifiedAsAny(fields[DayOfWeekField]);
        if (!IsSupportedDayOfMonth(dayOfMonth) || !IsSupportedDayOfWeek(dayOfWeek))
        {
            return false;
        }

        schedule = string.Join(" ", fields[0], fields[1], dayOfMonth, fields[MonthField], FromSundayIsOne(dayOfWeek));
        return true;
    }

    private static bool IsZero(string field) => field == "0" || field == "00";

    private static bool IsRestricted(string field) => field != "*" && field != "?";

    private static bool HasReversedRange(string field, int position) =>
        Range.Matches(field).Cast<Match>().Any(range => ValueOf(range.Groups[1].Value, position) > ValueOf(range.Groups[2].Value, position));

    private static int? ValueOf(string token, int position)
    {
        if (int.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out var number))
        {
            return number;
        }

        return NamedNumbersOf(position).TryGetValue(token, out var named) ? named : null;
    }

    private static IReadOnlyDictionary<string, int> NamedNumbersOf(int position) => position switch
    {
        MonthField => MonthNumbers,
        DayOfWeekField => DayNumbers,
        _ => NoNames,
    };

    private static string UnspecifiedAsAny(string field) => field == "?" ? "*" : field;

    private static bool IsSupportedDayOfMonth(string field)
    {
        var upper = field.ToUpperInvariant();
        return upper == "L" || upper.IndexOfAny(new[] { 'L', 'W' }) < 0;
    }

    private static bool IsSupportedDayOfWeek(string field) => field.ToUpperInvariant().IndexOf('L') < 0;

    private static string FromSundayIsOne(string dayOfWeek) =>
        DayOfWeekNumber.Replace(dayOfWeek, day => (int.Parse(day.Value, CultureInfo.InvariantCulture) - 1).ToString(CultureInfo.InvariantCulture));
}
