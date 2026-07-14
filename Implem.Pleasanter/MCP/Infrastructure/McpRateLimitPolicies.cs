using Implem.ParameterAccessor.Parts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Implem.Pleasanter.MCP.Infrastructure
{
    public static class McpRateLimitHelper
    {
        private const int DefaultRetryAfterSeconds = 60;

        public static async ValueTask OnRejectedAsync(HttpContext httpContext, RateLimitLease lease, CancellationToken cancellationToken)
        {
            var httpResponse = httpContext.Response;
            if (httpResponse.HasStarted)
            {
                return;
            }
            httpResponse.StatusCode = StatusCodes.Status429TooManyRequests;
            httpResponse.ContentType = "application/json";

            var retryAfter = lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue)
                ? Math.Max(1, (int)Math.Ceiling(retryAfterValue.TotalSeconds))
                : DefaultRetryAfterSeconds;

            httpResponse.Headers.RetryAfter = retryAfter.ToString();

            var response = new
            {
                jsonrpc = "2.0",
                error = new
                {
                    code = -32000,
                    message = $"Rate limit exceeded. Retry after {retryAfter} seconds."
                },
                id = (string)null
            };

            await httpResponse.WriteAsJsonAsync(response, cancellationToken);
        }

        private static bool TryGetMcpApiKey(HttpContext context, out string apiKey)
        {
            apiKey = null;

            if (!context.Request.Path.StartsWithSegments(McpConstants.BasePath))
            {
                return false;
            }

            apiKey = McpApiKeyHelper.TryGetMcpApiKey(context.Request);
            return !string.IsNullOrEmpty(apiKey);
        }

        public static List<PartitionedRateLimiter<HttpContext>> CreateRateLimiters(McpRateLimitSettings rateLimit)
        {
            var limiters = new List<PartitionedRateLimiter<HttpContext>>();

            if (rateLimit == null)
            {
                return limiters;
            }

            if (rateLimit.FixedWindow?.Enabled == true)
            {
                limiters.Add(CreateFixedWindowLimiter(rateLimit.FixedWindow));
            }

            if (rateLimit.SlidingWindow?.Enabled == true)
            {
                limiters.Add(CreateSlidingWindowLimiter(rateLimit.SlidingWindow));
            }

            if (rateLimit.TokenBucket?.Enabled == true)
            {
                limiters.Add(CreateTokenBucketLimiter(rateLimit.TokenBucket));
            }

            if (rateLimit.Concurrency?.Enabled == true)
            {
                limiters.Add(CreateConcurrencyLimiter(rateLimit.Concurrency));
            }

            return limiters;
        }

        public static PartitionedRateLimiter<HttpContext> CreateChainedLimiter(McpRateLimitSettings rateLimit)
        {
            var limiters = CreateRateLimiters(rateLimit);
            return limiters.Count switch
            {
                0 => null,
                1 => limiters[0],
                _ => PartitionedRateLimiter.CreateChained(limiters.ToArray())
            };
        }

        private static PartitionedRateLimiter<HttpContext> CreateFixedWindowLimiter(McpFixedWindowSettings settings)
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                if (!TryGetMcpApiKey(context, out var apiKey))
                {
                    return RateLimitPartition.GetNoLimiter(string.Empty);
                }

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: apiKey,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                        QueueLimit = 0
                    });
            });
        }

        private static PartitionedRateLimiter<HttpContext> CreateSlidingWindowLimiter(McpSlidingWindowSettings settings)
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                if (!TryGetMcpApiKey(context, out var apiKey))
                {
                    return RateLimitPartition.GetNoLimiter(string.Empty);
                }

                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: apiKey,
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                        SegmentsPerWindow = settings.SegmentsPerWindow > 0 ? settings.SegmentsPerWindow : 4,
                        QueueLimit = 0
                    });
            });
        }

        private static PartitionedRateLimiter<HttpContext> CreateTokenBucketLimiter(McpTokenBucketSettings settings)
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                if (!TryGetMcpApiKey(context, out var apiKey))
                {
                    return RateLimitPartition.GetNoLimiter(string.Empty);
                }

                return RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: apiKey,
                    factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = settings.TokenLimit,
                        TokensPerPeriod = settings.TokensPerPeriod,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(settings.ReplenishmentPeriodSeconds),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
        }

        private static PartitionedRateLimiter<HttpContext> CreateConcurrencyLimiter(McpConcurrencySettings settings)
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                if (!TryGetMcpApiKey(context, out var apiKey))
                {
                    return RateLimitPartition.GetNoLimiter(string.Empty);
                }

                return RateLimitPartition.GetConcurrencyLimiter(
                    partitionKey: apiKey,
                    factory: _ => new ConcurrencyLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        QueueLimit = 0
                    });
            });
        }
    }
}
