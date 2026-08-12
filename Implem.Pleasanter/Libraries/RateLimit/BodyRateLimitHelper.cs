using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.RateLimit
{
    public static class BodyRateLimitHelper
    {
        private static readonly Logger Logger = LogManager.GetLogger("ratelimitlogs");

        private static readonly BodyRateLimitLogThrottle Throttle = new BodyRateLimitLogThrottle();

        private const int DefaultRetryAfterSeconds = 60;

        private const string RateLimitExceededDisplayId = "RateLimitExceeded";

        public static readonly string[] PolicyNames = new[]
        {
            "General",
            "List",
            "Admin",
            "Heavy",
            "ApiHeavy",
            "Api",
            "AnonymousIp",
            "PublicForm"
        };

        public static string ResolveClientIp(HttpContext ctx)
        {
            return ctx?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
        }

        public static (string Kind, string Key) ResolvePartitionKey(HttpContext ctx, string partition)
        {
            partition ??= "Auto";
            var rl = Parameters.RateLimit;
            var order = rl?.KeyResolver?.Order ?? new[] { "User", "Ip" };
            switch (partition)
            {
                case "User":
                    return TryGetUserKey(ctx, out var u) ? ("user", u) : (null, null);
                case "Ip":
                    return ("ip", ResolveClientIp(ctx));
                case "ApiKey":
                    var apiKey = ExtractApiKey(ctx);
                    return !string.IsNullOrEmpty(apiKey)
                        ? ("apikey", apiKey)
                        : ("ip", ResolveClientIp(ctx));
                case "Auto":
                default:
                    foreach (var step in order)
                    {
                        if (step == "User" && TryGetUserKey(ctx, out var u2))
                            return ("user", u2);
                        if (step == "Ip")
                            return ("ip", ResolveClientIp(ctx));
                    }
                    return ("ip", ResolveClientIp(ctx));
            }
        }

        private static bool TryGetUserKey(HttpContext ctx, out string key)
        {
            key = null;
            var loginId = ctx?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(loginId)) return false;
            key = loginId;
            return true;
        }

        private static string ExtractApiKey(HttpContext httpContext)
        {
            try
            {
                var auth = httpContext?.Request?.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(auth)
                    && auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = auth.Substring("Bearer ".Length).Trim();
                    if (!string.IsNullOrEmpty(token)) return token;
                }

                if (httpContext?.Request == null) return null;
                var request = httpContext.Request;

                const long maxApiKeyParseBytes = 10L * 1024 * 1024;
                if (request.ContentLength == null || request.ContentLength > maxApiKeyParseBytes)
                {
                    return null;
                }

                if (request.HasFormContentType)
                {
                    var parameters = request.Form["parameters"].ToString();
                    var api = Implem.Libraries.Utilities.Jsons.Deserialize<Implem.Pleasanter.Libraries.Requests.Api>(parameters);
                    if (!string.IsNullOrEmpty(api?.ApiKey)) return api.ApiKey;
                }
                else if (request.Body != null
                         && (request.ContentType?.Contains("json", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    request.EnableBuffering();
                    if (request.Body.CanSeek) request.Body.Position = 0;
                    using var reader = new System.IO.StreamReader(
                        stream: request.Body,
                        encoding: System.Text.Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: false,
                        bufferSize: 1024,
                        leaveOpen: true);
                    var body = reader.ReadToEnd();
                    if (request.Body.CanSeek) request.Body.Position = 0;
                    var api = Implem.Libraries.Utilities.Jsons.Deserialize<Implem.Pleasanter.Libraries.Requests.Api>(body);
                    if (!string.IsNullOrEmpty(api?.ApiKey)) return api.ApiKey;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "ExtractApiKey failed; falling back to NoLimiter");
            }
            return null;
        }

        public static bool IsExcluded(HttpContext ctx, RateLimitExclusions exclusions)
        {
            if (exclusions?.LoginIds == null || exclusions.LoginIds.Length == 0) return false;
            var user = ctx?.User;
            if (user?.Identity?.IsAuthenticated != true) return false;
            var loginId = user.Identity.Name;
            if (string.IsNullOrEmpty(loginId)) return false;
            return Array.Exists(
                exclusions.LoginIds,
                id => string.Equals(id, loginId, StringComparison.OrdinalIgnoreCase));
        }

        public static RateLimitPartition<string> BuildPolicy(HttpContext ctx, string policyName)
        {
            if (!Parameters.AllowRateLimit())
                return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            var rl = Parameters.RateLimit;
            if (rl == null) return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            if (rl.Policies == null || !rl.Policies.TryGetValue(policyName, out var policy) || policy == null)
                return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            if (policy.EffectiveMode(rl.Mode) != "On")
                return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            return BuildPartitionForPolicy(ctx, rl, policy);
        }

        internal static RateLimitPartition<string> BuildPolicyFor(
            HttpContext ctx, ParameterAccessor.Parts.RateLimit rl, string policyName)
        {
            if (rl?.Policies == null) return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            if (!rl.Policies.TryGetValue(policyName, out var policy) || policy == null)
                return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            return BuildPartitionForPolicy(ctx, rl, policy);
        }

        private static RateLimitPartition<string> BuildPartitionForPolicy(
            HttpContext ctx, ParameterAccessor.Parts.RateLimit rl, RateLimitPolicySettings policy)
        {
            if (IsPathExcluded(ctx, rl.ExcludePaths))
                return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            if (!IsPathApplied(ctx, rl.ApplyPaths))
                return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            if (IsExcluded(ctx, rl.Exclusions))
                return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            var (kind, key) = ResolvePartitionKey(ctx, policy.Partition);
            if (kind == null || string.IsNullOrEmpty(key))
                return RateLimitPartition.GetNoLimiter<string>(string.Empty);
            return CreatePartition($"{kind}:{key}", policy);
        }

        public static PartitionedRateLimiter<HttpContext> BuildGlobalLimiter(ParameterAccessor.Parts.RateLimit rl)
            => BuildGlobalLimiterCore(rl, requireOnMode: true);

        internal static PartitionedRateLimiter<HttpContext> BuildGlobalLimiterForObservation(ParameterAccessor.Parts.RateLimit rl)
            => BuildGlobalLimiterCore(rl, requireOnMode: false);

        private static PartitionedRateLimiter<HttpContext> BuildGlobalLimiterCore(
            ParameterAccessor.Parts.RateLimit rl, bool requireOnMode)
        {
            if (rl?.GlobalLimiter == null)
            {
                return PartitionedRateLimiter.Create<HttpContext, string>(
                    _ => RateLimitPartition.GetNoLimiter<string>(string.Empty));
            }
            var policy = rl.GlobalLimiter;
            var excludePaths = rl.ExcludePaths ?? Array.Empty<string>();
            var applyPaths = rl.ApplyPaths;
            var exclusions = rl.Exclusions;
            return PartitionedRateLimiter.Create<HttpContext, string>(httpCtx =>
            {
                if (requireOnMode && policy.EffectiveMode(rl.Mode) != "On")
                    return RateLimitPartition.GetNoLimiter<string>(string.Empty);
                if (IsPathExcluded(httpCtx, excludePaths))
                    return RateLimitPartition.GetNoLimiter<string>(string.Empty);
                if (!IsPathApplied(httpCtx, applyPaths))
                    return RateLimitPartition.GetNoLimiter<string>(string.Empty);
                if (IsExcluded(httpCtx, exclusions))
                    return RateLimitPartition.GetNoLimiter<string>(string.Empty);
                var (kind, key) = ResolvePartitionKey(httpCtx, policy.Partition);
                if (kind == null || string.IsNullOrEmpty(key))
                    return RateLimitPartition.GetNoLimiter<string>(string.Empty);
                return CreatePartition($"{kind}:{key}", policy);
            });
        }

        public static bool IsPathExcluded(HttpContext ctx, IEnumerable<string> excludePaths)
        {
            if (ctx == null || excludePaths == null) return false;
            foreach (var p in excludePaths)
            {
                if (string.IsNullOrEmpty(p)) continue;
                if (p[0] != '/') continue;
                if (ctx.Request.Path.StartsWithSegments(new PathString(p))) return true;
            }
            return false;
        }

        public static bool IsPathApplied(HttpContext ctx, IEnumerable<string> applyPaths)
        {
            if (ctx == null) return false;
            var paths = applyPaths as ICollection<string>;
            if (paths == null || paths.Count == 0) return true;
            foreach (var p in paths)
            {
                if (string.IsNullOrEmpty(p)) continue;
                if (p[0] != '/') continue;
                if (p == "/" || ctx.Request.Path.StartsWithSegments(new PathString(p))) return true;
            }
            return false;
        }

        private static RateLimitPartition<string> CreatePartition(string partitionKey, RateLimitPolicySettings policy)
        {
            switch (policy.Algorithm)
            {
                case "TokenBucket":
                    {
                        var tokenLimit = policy.TokenLimit;
                        var tokensPerPeriod = policy.TokensPerPeriod;
                        var replenishmentPeriod = TimeSpan.FromSeconds(Math.Max(1, policy.ReplenishmentPeriodSeconds));
                        var queueLimit = policy.QueueLimit;
                        return RateLimitPartition.GetTokenBucketLimiter(
                            partitionKey: partitionKey,
                            factory: _ => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = tokenLimit,
                                TokensPerPeriod = tokensPerPeriod,
                                ReplenishmentPeriod = replenishmentPeriod,
                                QueueLimit = queueLimit,
                                AutoReplenishment = true
                            });
                    }
                case "FixedWindow":
                    {
                        var permitLimit = policy.PermitLimit;
                        var window = TimeSpan.FromSeconds(Math.Max(1, policy.WindowSeconds));
                        var queueLimit = policy.QueueLimit;
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: partitionKey,
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = permitLimit,
                                Window = window,
                                QueueLimit = queueLimit
                            });
                    }
                case "SlidingWindow":
                    {
                        var permitLimit = policy.PermitLimit;
                        var window = TimeSpan.FromSeconds(Math.Max(1, policy.WindowSeconds));
                        var segments = Math.Max(1, policy.SegmentsPerWindow);
                        var queueLimit = policy.QueueLimit;
                        return RateLimitPartition.GetSlidingWindowLimiter(
                            partitionKey: partitionKey,
                            factory: _ => new SlidingWindowRateLimiterOptions
                            {
                                PermitLimit = permitLimit,
                                Window = window,
                                SegmentsPerWindow = segments,
                                QueueLimit = queueLimit
                            });
                    }
                case "Concurrency":
                    {
                        var permitLimit = Math.Max(1, policy.PermitLimit);
                        var queueLimit = policy.QueueLimit;
                        return RateLimitPartition.GetConcurrencyLimiter(
                            partitionKey: partitionKey,
                            factory: _ => new ConcurrencyLimiterOptions
                            {
                                PermitLimit = permitLimit,
                                QueueLimit = queueLimit
                            });
                    }
                default:
                    return RateLimitPartition.GetNoLimiter<string>(partitionKey);
            }
        }

        public static async ValueTask OnRejectedAsync(OnRejectedContext context, CancellationToken cancellationToken)
        {
            var httpContext = context.HttpContext;
            var response = httpContext.Response;
            if (response.HasStarted)
            {
                return;
            }
            response.StatusCode = StatusCodes.Status429TooManyRequests;
            var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var ra)
                ? Math.Max(1, (int)Math.Ceiling(ra.TotalSeconds))
                : DefaultRetryAfterSeconds;
            var rl = Parameters.RateLimit;
            var includeRetryAfter = rl?.RejectedResponse?.IncludeRetryAfter ?? true;
            if (includeRetryAfter)
            {
                response.Headers.RetryAfter = retryAfter.ToString();
            }
            LogRejected(httpContext, rl, retryAfter);
            var message = Implem.Pleasanter.Libraries.Responses.Displays.Get(
                id: RateLimitExceededDisplayId,
                language: ResolveLanguage(httpContext));
            var request = httpContext.Request;
            var attrPolicy = httpContext.GetEndpoint()?.Metadata
                .GetMetadata<EnableRateLimitingAttribute>()?.PolicyName;
            var isApiEndpoint = request.Path.StartsWithSegments(new PathString("/api"))
                || attrPolicy == "Api"
                || attrPolicy == "ApiHeavy";
            if (isApiEndpoint)
            {
                await WriteApiErrorJsonAsync(response, message, cancellationToken);
                return;
            }
            if (string.Equals(
                request.Headers["X-Requested-With"].ToString(),
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase))
            {
                await WriteResponseCollectionJsonAsync(response, message, cancellationToken);
                return;
            }
            var accept = request.Headers["Accept"].ToString();
            if (!string.IsNullOrEmpty(accept)
                && accept.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                await WriteApiErrorJsonAsync(response, message, cancellationToken);
                return;
            }
            await WriteHtmlAsync(
                response,
                message,
                includeRetryAfter ? retryAfter : (int?)null,
                cancellationToken);
        }

        private static async Task WriteApiErrorJsonAsync(
            HttpResponse response, string message, CancellationToken cancellationToken)
        {
            response.ContentType = "application/json;charset=utf-8";
            var body = new ApiResponse(
                id: 0,
                statusCode: StatusCodes.Status429TooManyRequests,
                message: message).ToJson();
            await response.WriteAsync(body, cancellationToken);
        }

        private static async Task WriteResponseCollectionJsonAsync(
            HttpResponse response, string message, CancellationToken cancellationToken)
        {
            response.ContentType = "application/json;charset=utf-8";
            var rc = new ResponseCollection();
            rc.Message(
                message: new Message(id: null, text: message, css: "alert-error"),
                target: "#Message");
            await response.WriteAsync(rc.ToJson(), cancellationToken);
        }

        private static async Task WriteHtmlAsync(
            HttpResponse response, string message, int? retryAfter, CancellationToken cancellationToken)
        {
            response.ContentType = "text/html; charset=utf-8";
            var encoded = System.Net.WebUtility.HtmlEncode(message);
            var html =
                "<!DOCTYPE html>"
                + "<html><head><meta charset=\"utf-8\">"
                + "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">"
                + "<title>429 Too Many Requests</title></head>"
                + "<body><h1>429 Too Many Requests</h1>"
                + $"<p>{encoded}</p>"
                + (retryAfter.HasValue ? $"<p>Retry-After: {retryAfter.Value} seconds.</p>" : string.Empty)
                + "</body></html>";
            await response.WriteAsync(html, cancellationToken);
        }

        public static string ResolveLanguage(HttpContext ctx)
        {
            var defaultLanguage = Parameters.Service?.DefaultLanguage;
            if (ctx == null)
            {
                return defaultLanguage;
            }
            var query = ctx.Request.Query["Language"].ToString();
            if (!string.IsNullOrEmpty(query))
            {
                return query;
            }
            var acceptLanguage = ctx.Request.Headers["Accept-Language"].ToString();
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                var lang = acceptLanguage
                    .Split(',')[0]
                    .Split(';')[0]
                    .Split('-')[0]
                    .Trim()
                    .ToLowerInvariant();
                switch (lang)
                {
                    case "en":
                    case "zh":
                    case "ja":
                    case "de":
                    case "ko":
                    case "es":
                        return lang;
                    case "vi":
                        return "vn";
                }
            }
            return defaultLanguage;
        }

        private static void LogRejected(HttpContext context, ParameterAccessor.Parts.RateLimit rl, int retryAfter)
        {
            var attrPolicy = context.GetEndpoint()?.Metadata
                .GetMetadata<EnableRateLimitingAttribute>()?.PolicyName;
            string policyName;
            bool globalLimiterApplicable;
            RateLimitPolicySettings policy;
            if (string.IsNullOrEmpty(attrPolicy))
            {
                policyName = "GlobalLimiter";
                globalLimiterApplicable = false;
                policy = rl?.GlobalLimiter;
            }
            else
            {
                policyName = attrPolicy;
                globalLimiterApplicable = true;
                policy = rl?.Policies != null && rl.Policies.TryGetValue(attrPolicy, out var p) ? p : null;
            }
            var (kind, key) = ResolvePartitionForLog(context, policy);
            BodyRateLimitMetrics.AddRejected(policyName, kind);
            if (rl?.RejectedResponse?.LogRejected != true) return;
            var ev = BuildLogEvent(
                context: context,
                partitionKind: kind,
                partitionKey: key,
                eventType: "RateLimitRejected",
                mode: "On",
                wouldHaveRejected: false,
                policyName: policyName,
                policy: policy,
                globalLimiterApplicable: globalLimiterApplicable,
                retryAfter: retryAfter);
            Throttle.Process(ev, LogStructured);
        }

        internal static void LogWouldHaveRejected(
            HttpContext context, string policyName, RateLimitPolicySettings policy, int retryAfter)
        {
            var (kind, key) = ResolvePartitionForLog(context, policy);
            BodyRateLimitMetrics.AddWouldReject(policyName, kind);
            var ev = BuildLogEvent(
                context: context,
                partitionKind: kind,
                partitionKey: key,
                eventType: "RateLimitWouldHaveRejected",
                mode: "LogOnly",
                wouldHaveRejected: true,
                policyName: policyName,
                policy: policy,
                globalLimiterApplicable: false,
                retryAfter: retryAfter);
            Throttle.Process(ev, LogStructured);
        }

        private static RateLimitLogEvent BuildLogEvent(
            HttpContext context,
            string partitionKind,
            string partitionKey,
            string eventType,
            string mode,
            bool wouldHaveRejected,
            string policyName,
            RateLimitPolicySettings policy,
            bool globalLimiterApplicable,
            int retryAfter)
            => new RateLimitLogEvent
            {
                EventType = eventType,
                Mode = mode,
                WouldHaveRejected = wouldHaveRejected,
                PolicyName = policyName,
                Algorithm = policy?.Algorithm,
                Partition = policy?.Partition,
                PartitionKind = partitionKind,
                PartitionKey = $"{partitionKind}:{MaskPartitionKey(partitionKind, partitionKey)}",
                RequestPath = context.Request.Path.Value,
                Method = context.Request.Method,
                LoginId = context.User?.Identity?.Name,
                RetryAfterSeconds = retryAfter,
                GlobalLimiterApplicable = globalLimiterApplicable,
                Context = context,
                Policy = policy,
            };

        internal static void LogStructured(RateLimitLogEvent ev)
        {
            var b = Logger.ForLogEvent(LogLevel.Warn)
                .Message($"{ev.EventType} {ev.PartitionKey} {ev.RequestPath}")
                .Property("EventType", ev.EventType);
            switch (ev.Shape)
            {
                case RateLimitLogShape.Detail:
                    b.Property("Mode", ev.Mode)
                     .Property("PolicyName", ev.PolicyName)
                     .Property("Algorithm", ev.Algorithm ?? "unknown")
                     .Property("Partition", ev.Partition ?? "unknown")
                     .Property("PartitionKey", ev.PartitionKey)
                     .Property("RequestPath", ev.RequestPath)
                     .Property("Method", ev.Method)
                     .Property("WouldHaveRejected", ev.WouldHaveRejected)
                     .Property("GlobalLimiterApplicable", ev.GlobalLimiterApplicable)
                     .Property("RetryAfterSeconds", ev.RetryAfterSeconds);
                    var (userId, tenantId) = ResolveIdentifiers(ev.Context, ev.Policy);
                    if (!string.IsNullOrEmpty(ev.LoginId)) b.Property("LoginId", ev.LoginId);
                    if (userId != null) b.Property("UserId", userId.Value);
                    if (tenantId != null) b.Property("TenantId", tenantId.Value);
                    break;

                case RateLimitLogShape.PerKeySuppressed:
                    b.Property("Mode", ev.Mode)
                     .Property("PolicyName", ev.PolicyName)
                     .Property("PartitionKey", ev.PartitionKey)
                     .Property("SuppressedCount", ev.SuppressedCount.Value)
                     .Property("WindowSeconds", ev.WindowSeconds.Value);
                    break;

                case RateLimitLogShape.GlobalSuppressed:
                    b.Property("SuppressedKeys", ev.SuppressedKeys.Value)
                     .Property("SuppressedCount", ev.SuppressedCount.Value)
                     .Property("WindowSeconds", ev.WindowSeconds.Value)
                     .Property("Note", ev.Note);
                    break;
            }
            b.Log();
        }

        internal static (int? UserId, int? TenantId) ResolveIdentifiers(
            HttpContext ctx, RateLimitPolicySettings policy)
        {
            var user = ctx?.User;
            if (user?.Identity?.IsAuthenticated != true) return (null, null);
            var loginId = user.Identity.Name;
            if (string.IsNullOrEmpty(loginId)) return (null, null);
            var resolved = TryResolveUserByLoginId(loginId);
            return resolved != null
                ? (resolved.Id, resolved.TenantId)
                : ((int?)null, (int?)null);
        }

        private static DataTypes.User TryResolveUserByLoginId(string loginId)
        {
            if (string.IsNullOrEmpty(loginId)) return null;
            foreach (var tenantCache in Server.SiteInfo.TenantCaches.Values)
            {
                var matched = tenantCache?.UserHash?
                    .FirstOrDefault(kv => string.Equals(
                        kv.Value?.LoginId, loginId, StringComparison.OrdinalIgnoreCase));
                if (matched.HasValue && matched.Value.Value != null)
                    return matched.Value.Value;
            }
            return null;
        }

        private static string MaskPartitionKey(string kind, string key)
        {
            if (kind != "apikey" || string.IsNullOrEmpty(key)) return key;
            var hash = System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(key));
            return "sha256:" + Convert.ToHexString(hash, 0, 4).ToLowerInvariant();
        }

        internal static (string Kind, string Key) ResolvePartitionForLog(
            HttpContext context, RateLimitPolicySettings policy)
        {
            var (kind, key) = ResolvePartitionKey(context, policy?.Partition ?? "Auto");
            if (kind != null && !string.IsNullOrEmpty(key))
            {
                return (kind, key);
            }
            return policy?.Partition == "ApiKey"
                ? ("anon", string.Empty)
                : ("ip", ResolveClientIp(context));
        }
    }
}
