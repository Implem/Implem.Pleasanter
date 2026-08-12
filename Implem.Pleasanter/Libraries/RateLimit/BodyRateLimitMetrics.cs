using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Implem.Pleasanter.Libraries.RateLimit
{
    public static class BodyRateLimitMetrics
    {
        public const string MeterName = "Implem.Pleasanter.RateLimit";

        private static readonly Meter Meter = new Meter(MeterName);

        private static readonly Counter<long> Rejected =
            Meter.CreateCounter<long>("ratelimit.rejected");

        private static readonly Counter<long> WouldReject =
            Meter.CreateCounter<long>("ratelimit.would_reject");

        public static void AddRejected(string policy, string partitionKind)
            => Rejected.Add(
                1,
                new KeyValuePair<string, object>("policy", policy),
                new KeyValuePair<string, object>("partition", partitionKind),
                new KeyValuePair<string, object>("mode", "On"));

        public static void AddWouldReject(string policy, string partitionKind)
            => WouldReject.Add(
                1,
                new KeyValuePair<string, object>("policy", policy),
                new KeyValuePair<string, object>("partition", partitionKind),
                new KeyValuePair<string, object>("mode", "LogOnly"));
    }
}
