using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Implem.ParameterAccessor.Parts
{
    public class RateLimit
    {
        public string Mode { get; set; }

        public string[] ApplyPaths { get; set; }

        public string[] ExcludePaths { get; set; }

        public RateLimitKeyResolver KeyResolver { get; set; }

        public RateLimitExclusions Exclusions { get; set; }

        public RateLimitPolicySettings GlobalLimiter { get; set; }

        public Dictionary<string, RateLimitPolicySettings> Policies { get; set; }

        public RateLimitRejectedResponse RejectedResponse { get; set; }

        [JsonIgnore]
        public bool AnyActive =>
            GlobalLimiter?.EffectiveMode(Mode) == "On"
            || (Policies?.Values.Any(p => p.EffectiveMode(Mode) == "On") ?? false);

        [JsonIgnore]
        public bool AnyLogOnly =>
            GlobalLimiter?.EffectiveMode(Mode) == "LogOnly"
            || (Policies?.Values.Any(p => p.EffectiveMode(Mode) == "LogOnly") ?? false);

        [JsonIgnore]
        public List<string> ValidationWarnings { get; } = new List<string>();

        private static readonly HashSet<string> ValidTopLevelModes =
            new(System.StringComparer.Ordinal) { "Off", "LogOnly", "On" };

        private static readonly HashSet<string> ValidPolicyModes =
            new(System.StringComparer.Ordinal) { "Inherit", "Off", "LogOnly", "On" };

        private static readonly HashSet<string> ValidAlgorithms =
            new(System.StringComparer.Ordinal) { "TokenBucket", "FixedWindow", "Concurrency", "SlidingWindow" };

        private static readonly HashSet<string> ValidPartitions =
            new(System.StringComparer.Ordinal) { "User", "Ip", "Auto", "ApiKey" };

        public void Normalize()
        {
            ValidationWarnings.Clear();
            if (Mode == null || !ValidTopLevelModes.Contains(Mode))
            {
                ValidationWarnings.Add(
                    $"RateLimit.Mode='{Mode ?? "<null>"}' is invalid. Falling back to 'Off'.");
                Mode = "Off";
            }
            ApplyPaths = NormalizePaths(nameof(ApplyPaths), ApplyPaths);
            ExcludePaths = NormalizePaths(nameof(ExcludePaths), ExcludePaths);
            NormalizePolicy("GlobalLimiter", GlobalLimiter);
            if (Policies != null)
            {
                foreach (var kv in Policies)
                {
                    NormalizePolicy($"Policies.{kv.Key}", kv.Value);
                }
            }
        }

        private string[] NormalizePaths(string name, string[] paths)
        {
            if (paths == null) return null;
            var valid = new List<string>();
            foreach (var p in paths)
            {
                if (string.IsNullOrEmpty(p)) continue;
                if (p[0] != '/')
                {
                    ValidationWarnings.Add($"{name} entry '{p}' must start with '/'. Ignored.");
                    continue;
                }
                valid.Add(p);
            }
            return valid.ToArray();
        }

        private void NormalizePolicy(string policyName, RateLimitPolicySettings policy)
        {
            if (policy == null) return;
            var failSafe = false;
            if (policy.Mode == null || !ValidPolicyModes.Contains(policy.Mode))
            {
                ValidationWarnings.Add(
                    $"{policyName}.Mode='{policy.Mode ?? "<null>"}' is invalid.");
                failSafe = true;
            }
            if (policy.Algorithm == null || !ValidAlgorithms.Contains(policy.Algorithm))
            {
                ValidationWarnings.Add(
                    $"{policyName}.Algorithm='{policy.Algorithm ?? "<null>"}' is invalid.");
                failSafe = true;
            }
            if (policy.Partition == null || !ValidPartitions.Contains(policy.Partition))
            {
                ValidationWarnings.Add(
                    $"{policyName}.Partition='{policy.Partition ?? "<null>"}' is invalid.");
                failSafe = true;
            }
            if (policy.QueueLimit < 0)
            {
                ValidationWarnings.Add(
                    $"{policyName}.QueueLimit must be >= 0 (got QueueLimit={policy.QueueLimit}).");
                failSafe = true;
            }
            switch (policy.Algorithm)
            {
                case "TokenBucket":
                    if (policy.TokenLimit <= 0 || policy.TokensPerPeriod <= 0 || policy.ReplenishmentPeriodSeconds <= 0)
                    {
                        ValidationWarnings.Add(
                            $"{policyName} (TokenBucket) requires TokenLimit>0, TokensPerPeriod>0, ReplenishmentPeriodSeconds>0 "
                            + $"(got TokenLimit={policy.TokenLimit}, TokensPerPeriod={policy.TokensPerPeriod}, "
                            + $"ReplenishmentPeriodSeconds={policy.ReplenishmentPeriodSeconds}).");
                        failSafe = true;
                    }
                    break;
                case "FixedWindow":
                    if (policy.PermitLimit <= 0 || policy.WindowSeconds <= 0)
                    {
                        ValidationWarnings.Add(
                            $"{policyName} (FixedWindow) requires PermitLimit>0, WindowSeconds>0 "
                            + $"(got PermitLimit={policy.PermitLimit}, WindowSeconds={policy.WindowSeconds}).");
                        failSafe = true;
                    }
                    break;
                case "SlidingWindow":
                    if (policy.PermitLimit <= 0 || policy.WindowSeconds <= 0 || policy.SegmentsPerWindow <= 0)
                    {
                        ValidationWarnings.Add(
                            $"{policyName} (SlidingWindow) requires PermitLimit>0, WindowSeconds>0, SegmentsPerWindow>0 "
                            + $"(got PermitLimit={policy.PermitLimit}, WindowSeconds={policy.WindowSeconds}, "
                            + $"SegmentsPerWindow={policy.SegmentsPerWindow}).");
                        failSafe = true;
                    }
                    break;
                case "Concurrency":
                    if (policy.PermitLimit <= 0)
                    {
                        ValidationWarnings.Add(
                            $"{policyName} (Concurrency) requires PermitLimit>0 "
                            + $"(got PermitLimit={policy.PermitLimit}).");
                        failSafe = true;
                    }
                    break;
            }
            if (failSafe)
            {
                ValidationWarnings.Add($"{policyName} falls back to Mode='Off'.");
                policy.Mode = "Off";
            }
        }
    }

    public class RateLimitKeyResolver
    {
        public string[] Order { get; set; }
    }

    public class RateLimitExclusions
    {
        public string[] LoginIds { get; set; }
    }

    public class RateLimitPolicySettings
    {
        public string Mode { get; set; }

        public string Algorithm { get; set; }

        public string Partition { get; set; }

        public int TokenLimit { get; set; }
        public int TokensPerPeriod { get; set; }
        public int ReplenishmentPeriodSeconds { get; set; }

        public int PermitLimit { get; set; }
        public int WindowSeconds { get; set; }
        public int SegmentsPerWindow { get; set; }

        public int QueueLimit { get; set; }

        public string EffectiveMode(string topLevelMode) =>
            Mode == "Inherit" ? topLevelMode : Mode;
    }

    public class RateLimitRejectedResponse
    {
        public bool IncludeRetryAfter { get; set; }

        public bool LogRejected { get; set; }
    }
}
