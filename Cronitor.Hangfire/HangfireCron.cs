using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cronitor.Hangfire;

/// <summary>
/// Translates Hangfire's cron expressions, as parsed by Cronos, into ones Cronitor reads the same way, or rejects them.
/// Hangfire: https://github.com/HangfireIO/Cronos#cron-format
/// Cronitor: https://cronitor.io/docs/monitors-api (schedules); L and n#k accepted and 15W rejected, verified against the API.
/// </summary>
internal static class HangfireCron
{
    private const int MonthField = 3;
    private const int DayOfWeekField = 4;

    private static readonly IReadOnlyDictionary<string, string> Macros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["@yearly"] = "0 0 1 1 *",
        ["@annually"] = "0 0 1 1 *",
        ["@monthly"] = "0 0 1 * *",
        ["@weekly"] = "0 0 * * 0",
        ["@daily"] = "0 0 * * *",
        ["@midnight"] = "0 0 * * *",
        ["@hourly"] = "0 * * * *",
        ["@every_minute"] = "* * * * *",
    };

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

    private static readonly Regex Range = new Regex(@"([A-Za-z0-9]+)-([A-Za-z0-9]+)", RegexOptions.CultureInvariant);

    internal static bool TryConvert(string cron, out string schedule)
    {
        schedule = null;
        if (string.IsNullOrWhiteSpace(cron))
        {
            return false;
        }

        if (Macros.TryGetValue(cron.Trim(), out schedule))
        {
            return true;
        }

        var fields = cron.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length == 6 && IsZero(fields[0]))
        {
            fields = fields.Skip(1).ToArray();
        }

        if (fields.Length != 5 || fields.Where(HasReversedRange).Any())
        {
            return false;
        }

        var dayOfMonth = UnspecifiedAsAny(fields[2]);
        var dayOfWeek = UnspecifiedAsAny(fields[DayOfWeekField]);
        if (!IsSupportedDayOfMonth(dayOfMonth) || !IsSupportedDayOfWeek(dayOfWeek) || IsRestricted(dayOfMonth) && IsRestricted(dayOfWeek))
        {
            return false;
        }

        schedule = string.Join(" ", fields[0], fields[1], dayOfMonth, fields[MonthField], dayOfWeek);
        return true;
    }

    private static bool IsZero(string field) => field == "0" || field == "00";

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

    /// <summary>
    /// When both the day of the month and the day of the week are restricted, Hangfire needs both to match and Cronitor either.
    /// </summary>
    private static bool IsRestricted(string field) => field != "*";

    private static bool IsSupportedDayOfMonth(string field)
    {
        var upper = field.ToUpperInvariant();
        return upper == "L" || upper.IndexOfAny(new[] { 'L', 'W' }) < 0;
    }

    private static bool IsSupportedDayOfWeek(string field) => field.ToUpperInvariant().IndexOf('L') < 0;
}
