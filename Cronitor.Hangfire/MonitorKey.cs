namespace Cronitor.Hangfire;

internal static class MonitorKey
{
    internal static string Of(string recurringJobId) => recurringJobId.Replace('/', '-');
}
