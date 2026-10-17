using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Cronitor.Hangfire.Tests;

/// <summary>
/// A storage connection that fails whenever it's asked anything, to show that code reads only what Hangfire already
/// fetched with the job.
/// </summary>
internal sealed class UnreachableStorageConnection : JobStorageConnection
{
    public override IWriteOnlyTransaction CreateWriteTransaction() => throw Unreachable();
    public override IDisposable AcquireDistributedLock(string resource, TimeSpan timeout) => throw Unreachable();
    public override string CreateExpiredJob(Job job, IDictionary<string, string> parameters, DateTime createdAt, TimeSpan expireIn) => throw Unreachable();
    public override IFetchedJob FetchNextJob(string[] queues, CancellationToken cancellationToken) => throw Unreachable();
    public override void SetJobParameter(string id, string name, string value) => throw Unreachable();
    public override string GetJobParameter(string id, string name) => throw Unreachable();
    public override JobData GetJobData(string jobId) => throw Unreachable();
    public override StateData GetStateData(string jobId) => throw Unreachable();
    public override void AnnounceServer(string serverId, ServerContext context) => throw Unreachable();
    public override void RemoveServer(string serverId) => throw Unreachable();
    public override void Heartbeat(string serverId) => throw Unreachable();
    public override int RemoveTimedOutServers(TimeSpan timeOut) => throw Unreachable();
    public override HashSet<string> GetAllItemsFromSet(string key) => throw Unreachable();
    public override string GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore) => throw Unreachable();
    public override void SetRangeInHash(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs) => throw Unreachable();
    public override Dictionary<string, string> GetAllEntriesFromHash(string key) => throw Unreachable();

    private static InvalidOperationException Unreachable() => new InvalidOperationException("Storage was asked although the job's parameters were already fetched.");
}
