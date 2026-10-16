using System;
using System.Globalization;

namespace Cronitor.Assertions;

public class AssertionBuilder
{
    private readonly string _assertion;
    private readonly string _key;

    public AssertionBuilder(string assertion, string key = null)
    {
        _assertion = assertion;
        _key = key;
    }

    public new AssertionRule Equals(object value) => Build("=", value);
    public AssertionRule LessThan(object value) => Build("<", value);
    public AssertionRule GreaterThan(object value) => Build(">", value);
    public AssertionRule Contains(object value) => Build("contains", value);

    private AssertionRule Build(string op, object value)
    {
        var text = Convert.ToString(value, CultureInfo.InvariantCulture);
        var assertion = _key != null
            ? $"{_assertion} {_key} {op} {text}"
            : $"{_assertion} {op} {text}";

        return new AssertionRule(assertion);
    }
}
