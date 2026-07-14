using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.RateLimit
{
    public sealed class BodyRateLimitMetricsSnapshotService : BackgroundService
    {
        private sealed class Bucket
        {
            public long Count;
        }

        private static readonly NLog.Logger DefaultLogger = NLog.LogManager.GetLogger("ratelimitmetrics");

        private readonly ConcurrentDictionary<string, Bucket> _counts = new ConcurrentDictionary<string, Bucket>();
        private readonly MeterListener _listener = new MeterListener();
        private readonly TimeSpan _interval;
        private readonly Func<DateTime> _now;
        private readonly Action<IReadOnlyDictionary<string, long>, DateTime, DateTime> _emit;
        private DateTime _windowStart;

        public BodyRateLimitMetricsSnapshotService()
            : this(TimeSpan.FromSeconds(60), () => DateTime.UtcNow, emit: null)
        {
        }

        internal BodyRateLimitMetricsSnapshotService(
            TimeSpan interval,
            Func<DateTime> nowProvider,
            Action<IReadOnlyDictionary<string, long>, DateTime, DateTime> emit)
        {
            _interval = interval;
            _now = nowProvider ?? (() => DateTime.UtcNow);
            _emit = emit ?? EmitToLog;
            _windowStart = _now();
            _listener.InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == BodyRateLimitMetrics.MeterName)
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            };
            _listener.SetMeasurementEventCallback<long>((_, measurement, tags, _) =>
            {
                var bucket = _counts.GetOrAdd(BuildKey(tags), _ => new Bucket());
                Interlocked.Add(ref bucket.Count, measurement);
            });
            _listener.Start();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(_interval, stoppingToken);
                    EmitSnapshot();
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                EmitSnapshot();
            }
        }

        internal void EmitSnapshot()
        {
            var now = _now();
            var counts = new Dictionary<string, long>();
            foreach (var kv in _counts)
            {
                var n = Interlocked.Exchange(ref kv.Value.Count, 0);
                if (n > 0)
                {
                    counts[kv.Key] = n;
                }
            }
            var windowStart = _windowStart;
            _windowStart = now;
            if (counts.Count == 0)
            {
                return;
            }
            _emit(counts, windowStart, now);
        }

        private static string BuildKey(ReadOnlySpan<KeyValuePair<string, object>> tags)
        {
            string policy = null, partition = null, mode = null;
            foreach (var tag in tags)
            {
                switch (tag.Key)
                {
                    case "policy": policy = tag.Value as string; break;
                    case "partition": partition = tag.Value as string; break;
                    case "mode": mode = tag.Value as string; break;
                }
            }
            return string.Concat(policy, "_", partition, "_", mode);
        }

        private static void EmitToLog(IReadOnlyDictionary<string, long> counts, DateTime start, DateTime end)
        {
            var logEvent = new NLog.LogEventInfo(NLog.LogLevel.Info, "ratelimitmetrics", "RateLimitMetricsSnapshot");
            logEvent.Properties["EventType"] = "RateLimitMetricsSnapshot";
            logEvent.Properties["WindowStartedAt"] = start.ToString("O");
            logEvent.Properties["WindowEndedAt"] = end.ToString("O");
            logEvent.Properties["Counts"] = counts;
            DefaultLogger.Log(logEvent);
        }

        public override void Dispose()
        {
            _listener.Dispose();
            base.Dispose();
        }
    }
}
