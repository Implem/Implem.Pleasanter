using Implem.DefinitionAccessor;
using Implem.ParameterAccessor.Parts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.RateLimit
{
    public class BodyRateLimitObserver
    {
        private static readonly Logger Logger = LogManager.GetLogger("ratelimitlogs");

        private readonly RequestDelegate _next;

        private readonly ParameterAccessor.Parts.RateLimit _rl;

        private readonly (RateLimitPolicySettings Policy, PartitionedRateLimiter<HttpContext> Limiter)? _globalObserver;

        private readonly Dictionary<string, (RateLimitPolicySettings Policy, PartitionedRateLimiter<HttpContext> Limiter)> _policyObservers;

        public BodyRateLimitObserver(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _rl = Parameters.RateLimit;
            (_globalObserver, _policyObservers) = BuildObservers(_rl);
        }

        internal BodyRateLimitObserver(RequestDelegate next, ParameterAccessor.Parts.RateLimit rl)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _rl = rl;
            (_globalObserver, _policyObservers) = BuildObservers(_rl);
        }

        private static (
            (RateLimitPolicySettings Policy, PartitionedRateLimiter<HttpContext> Limiter)? GlobalObserver,
            Dictionary<string, (RateLimitPolicySettings Policy, PartitionedRateLimiter<HttpContext> Limiter)> PolicyObservers)
            BuildObservers(ParameterAccessor.Parts.RateLimit rl)
        {
            (RateLimitPolicySettings, PartitionedRateLimiter<HttpContext>)? globalObserver = null;
            var policyObservers = new Dictionary<string, (RateLimitPolicySettings Policy, PartitionedRateLimiter<HttpContext> Limiter)>();
            if (!Parameters.AllowRateLimit()) return (null, policyObservers);
            if (rl == null) return (null, policyObservers);
            if (rl.GlobalLimiter != null && rl.GlobalLimiter.EffectiveMode(rl.Mode) == "LogOnly")
            {
                globalObserver = (rl.GlobalLimiter, BodyRateLimitHelper.BuildGlobalLimiterForObservation(rl));
            }
            if (rl.Policies != null)
            {
                foreach (var name in BodyRateLimitHelper.PolicyNames)
                {
                    if (!rl.Policies.TryGetValue(name, out var p) || p == null) continue;
                    if (p.EffectiveMode(rl.Mode) != "LogOnly") continue;
                    var policyName = name;
                    var limiter = PartitionedRateLimiter.Create<HttpContext, string>(httpCtx =>
                        BodyRateLimitHelper.BuildPolicyFor(httpCtx, rl, policyName));
                    policyObservers[policyName] = (p, limiter);
                }
            }
            return (globalObserver, policyObservers);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if ((_globalObserver == null && _policyObservers.Count == 0) || !IsTargetedRequest(context))
            {
                await _next(context);
                return;
            }
            var applicable = new List<(string Name, RateLimitPolicySettings Policy, PartitionedRateLimiter<HttpContext> Limiter)>();
            if (_globalObserver.HasValue)
            {
                applicable.Add(("GlobalLimiter", _globalObserver.Value.Policy, _globalObserver.Value.Limiter));
            }
            var attrPolicy = context.GetEndpoint()?.Metadata.GetMetadata<EnableRateLimitingAttribute>()?.PolicyName;
            if (!string.IsNullOrEmpty(attrPolicy) && _policyObservers.TryGetValue(attrPolicy, out var entry))
            {
                applicable.Add((attrPolicy, entry.Policy, entry.Limiter));
            }
            if (applicable.Count == 0)
            {
                await _next(context);
                return;
            }
            var leases = new List<RateLimitLease>(applicable.Count);
            try
            {
                foreach (var (name, policy, limiter) in applicable)
                {
                    try
                    {
                        var lease = limiter.AttemptAcquire(context, permitCount: 1);
                        leases.Add(lease);
                        if (!lease.IsAcquired)
                        {
                            LogWouldHaveRejected(context, name, policy, lease);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "BodyRateLimitObserver: policy {0} acquire failed", name);
                    }
                }
                await _next(context);
            }
            finally
            {
                for (var i = leases.Count - 1; i >= 0; i--)
                {
                    try { leases[i].Dispose(); }
                    catch { }
                }
            }
        }

        private bool IsTargetedRequest(HttpContext context)
        {
            var rl = _rl;
            if (rl == null) return false;
            if (BodyRateLimitHelper.IsPathExcluded(context, rl.ExcludePaths)) return false;
            if (!BodyRateLimitHelper.IsPathApplied(context, rl.ApplyPaths)) return false;
            if (BodyRateLimitHelper.IsExcluded(context, rl.Exclusions)) return false;
            return true;
        }

        private static void LogWouldHaveRejected(
            HttpContext context,
            string policyName,
            RateLimitPolicySettings policy,
            RateLimitLease lease)
        {
            var retryAfter = lease.TryGetMetadata(MetadataName.RetryAfter, out var ra)
                ? Math.Max(1, (int)Math.Ceiling(ra.TotalSeconds))
                : 60;
            BodyRateLimitHelper.LogWouldHaveRejected(
                context: context,
                policyName: policyName,
                policy: policy,
                retryAfter: retryAfter);
        }
    }

    public static class BodyRateLimitObserverExtensions
    {
        public static IApplicationBuilder UseBodyRateLimitObserver(this IApplicationBuilder app)
            => app.UseMiddleware<BodyRateLimitObserver>();
    }
}
