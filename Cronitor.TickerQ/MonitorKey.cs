namespace Cronitor.TickerQ;

internal static class MonitorKey
{
    internal static string Of(string functionName) => functionName.Replace('/', '-');
}
