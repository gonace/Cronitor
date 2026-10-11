using Xunit;

namespace Hangfire.Extensions.Cronitor.Tests;

/// <summary>
/// Serializes tests that register filters through Hangfire's global configuration, since they share its global filter list.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public class GlobalJobFiltersCollection
{
    public const string Name = "Global Hangfire job filters";
}
