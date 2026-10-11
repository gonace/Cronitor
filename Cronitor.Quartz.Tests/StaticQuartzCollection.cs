using Xunit;

namespace Cronitor.Quartz.Tests;

/// <summary>
/// Serializes tests that run Quartz schedulers, since Quartz 3 keeps its logging in global state
/// that <c>AddQuartz</c> replaces.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public class StaticQuartzCollection
{
    public const string Name = "Static Quartz";
}
