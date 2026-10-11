using Xunit;

namespace Cronitor.Tests.Helpers;

/// <summary>
/// Serializes tests that configure the static <see cref="Cronitor"/> class,
/// since they share its global state.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public class StaticCronitorCollection
{
    public const string Name = "Static Cronitor";
}
