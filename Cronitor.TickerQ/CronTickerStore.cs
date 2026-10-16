using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TickerQ.Utilities.Entities;
using TickerQ.Utilities.Interfaces;

namespace Cronitor.TickerQ;

/// <summary>
/// Reads the cron tickers from TickerQ's store, whichever entity types the application registered TickerQ with.
/// </summary>
internal abstract class CronTickerStore
{
    internal abstract Task<IReadOnlyList<CronTickerEntity>> ReadAsync(CancellationToken cancellationToken);

    internal static CronTickerStore Of(IServiceProvider provider, Type persistenceProviderType)
    {
        var storeType = typeof(CronTickerStoreFor<,>).MakeGenericType(persistenceProviderType.GetGenericArguments());
        return (CronTickerStore)Activator.CreateInstance(storeType, provider.GetRequiredService(persistenceProviderType));
    }
}

internal sealed class CronTickerStoreFor<TTimeTicker, TCronTicker> : CronTickerStore
    where TTimeTicker : TimeTickerEntity<TTimeTicker>, new()
    where TCronTicker : CronTickerEntity, new()
{
    private readonly ITickerPersistenceProvider<TTimeTicker, TCronTicker> _persistence;

    public CronTickerStoreFor(ITickerPersistenceProvider<TTimeTicker, TCronTicker> persistence)
    {
        _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
    }

    internal override async Task<IReadOnlyList<CronTickerEntity>> ReadAsync(CancellationToken cancellationToken) =>
        await _persistence.GetCronTickers(_ => true, cancellationToken).ConfigureAwait(false);
}
