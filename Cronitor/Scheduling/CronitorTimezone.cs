using System;
#if !NET6_0_OR_GREATER
using System.Linq;
using TimeZoneConverter;
#endif

namespace Cronitor.Scheduling;

/// <summary>
/// The IANA name Cronitor expects for a monitor's <c>timezone</c>, such as <c>Europe/Berlin</c>, from a
/// <see cref="TimeZoneInfo"/>. On Windows, time zones have Windows names such as <c>W. Europe Standard Time</c>,
/// which are converted to the IANA name of the same zone.
/// </summary>
public static class CronitorTimezone
{
    /// <summary>
    /// The IANA name of the time zone. Throws an <see cref="ArgumentException"/> when it has none, such as a custom time zone.
    /// </summary>
    public static string Of(TimeZoneInfo timeZone)
    {
        if (TryGetIanaName(timeZone, out var ianaName))
        {
            return ianaName;
        }

        throw new ArgumentException($"The time zone '{timeZone.Id}' has no IANA name.", nameof(timeZone));
    }

    /// <summary>
    /// Gets the IANA name of the time zone, or returns false when it has none, such as a custom time zone.
    /// </summary>
    public static bool TryGetIanaName(TimeZoneInfo timeZone, out string ianaName)
    {
        if (timeZone == null)
        {
            throw new ArgumentNullException(nameof(timeZone));
        }

        // UTC is named UTC on every platform, rather than Etc/UTC on some.
        if (string.Equals(timeZone.Id, TimeZoneInfo.Utc.Id, StringComparison.Ordinal))
        {
            ianaName = "UTC";
            return true;
        }

        if (IsIanaName(timeZone.Id))
        {
            ianaName = timeZone.Id;
            return true;
        }

        if (TryConvertWindowsName(timeZone.Id, out ianaName))
        {
            return true;
        }

        ianaName = null;
        return false;
    }

#if NET6_0_OR_GREATER
    private static bool IsIanaName(string id) => TimeZoneInfo.TryConvertIanaIdToWindowsId(id, out _);

    private static bool TryConvertWindowsName(string id, out string ianaName) => TimeZoneInfo.TryConvertWindowsIdToIanaId(id, out ianaName);
#else
    // .NET Standard has no conversion of its own, so TimeZoneConverter's copy of the Unicode CLDR mapping is used.
    private static bool IsIanaName(string id) => TZConvert.KnownIanaTimeZoneNames.Contains(id, StringComparer.Ordinal);

    private static bool TryConvertWindowsName(string id, out string ianaName) => TZConvert.TryWindowsToIana(id, out ianaName);
#endif
}
