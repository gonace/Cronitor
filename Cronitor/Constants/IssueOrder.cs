namespace Cronitor.Constants;

/// <summary>
/// The order of an issue list.
/// </summary>
public class IssueOrder
{
    public static readonly IssueOrder StartedAscending = new IssueOrder("started");
    public static readonly IssueOrder StartedDescending = new IssueOrder("-started");
    public static readonly IssueOrder RelevanceAscending = new IssueOrder("relevance");
    public static readonly IssueOrder RelevanceDescending = new IssueOrder("-relevance");

    private IssueOrder(string key)
    {
        Key = key;
    }

    public string Key { get; }

    public override string ToString()
    {
        return Key;
    }
}
