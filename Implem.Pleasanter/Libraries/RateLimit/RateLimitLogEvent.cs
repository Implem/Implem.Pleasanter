using Implem.ParameterAccessor.Parts;
using Microsoft.AspNetCore.Http;

namespace Implem.Pleasanter.Libraries.RateLimit
{
    public enum RateLimitLogShape
    {
        Detail,

        PerKeySuppressed,

        GlobalSuppressed
    }

    public sealed class RateLimitLogEvent
    {
        public RateLimitLogShape Shape;

        public string EventType;

        public string Mode;

        public string PolicyName;

        public string Algorithm;

        public string Partition;

        public string PartitionKind;

        public string PartitionKey;

        public string RequestPath;

        public string Method;

        public string LoginId;

        public int RetryAfterSeconds;

        public bool WouldHaveRejected;

        public bool GlobalLimiterApplicable;

        public int? SuppressedCount;

        public int? SuppressedKeys;

        public int? WindowSeconds;

        public string Note;

        public HttpContext Context;

        public RateLimitPolicySettings Policy;
    }
}
