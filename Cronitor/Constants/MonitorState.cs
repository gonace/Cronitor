namespace Cronitor.Constants;

/// <summary>
/// A monitor state to filter monitor lists by. Use the constructor for states not listed here.
/// </summary>
public class MonitorState
{
    public static readonly MonitorState Failing = new MonitorState("failing");
    public static readonly MonitorState Paused = new MonitorState("paused");

    public MonitorState(string key)
    {
        Key = key;
    }

    public string Key { get; }

    public override string ToString()
    {
        return Key;
    }
}
