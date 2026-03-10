using System.Collections.Generic;
using System.ComponentModel;

namespace Implem.ParameterAccessor.Parts
{
    public class McpServer
    {
        public bool Enabled;

        public bool ReadOnlyMode;

        public McpRateLimitSettings RateLimit;

        public int LogExportLimit { get; set; } = 10000;

        public McpLoggingSettings Logging;
    }

    public class McpLoggingSettings
    {
        [DefaultValue(true)]
        public bool EnableLoggingToDatabase { get; set; } = true;

        public bool EnableLoggingToFile { get; set; }

        public List<string> NotLoggingIp { get; set; }

        [DefaultValue(65536)]
        public int ResponseDataMaxLength { get; set; } = 65536;
    }

    public class McpRateLimitSettings
    {
        public McpFixedWindowSettings FixedWindow;

        public McpSlidingWindowSettings SlidingWindow;

        public McpTokenBucketSettings TokenBucket;

        public McpConcurrencySettings Concurrency;

        public bool AnyEnabled =>
            (FixedWindow?.Enabled ?? false) ||
            (SlidingWindow?.Enabled ?? false) ||
            (TokenBucket?.Enabled ?? false) ||
            (Concurrency?.Enabled ?? false);
    }

    public class McpFixedWindowSettings
    {
        public bool Enabled;

        public int PermitLimit;

        public int WindowSeconds;
    }

    public class McpSlidingWindowSettings
    {
        public bool Enabled;

        public int PermitLimit;

        public int WindowSeconds;

        public int SegmentsPerWindow;
    }

    public class McpTokenBucketSettings
    {
        public bool Enabled;

        public int TokenLimit;

        public int TokensPerPeriod;

        public int ReplenishmentPeriodSeconds;
    }

    public class McpConcurrencySettings
    {
        public bool Enabled;

        public int PermitLimit;
    }
}
