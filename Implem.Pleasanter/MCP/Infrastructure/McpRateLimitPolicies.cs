using Implem.ParameterAccessor.Parts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Implem.Pleasanter.MCP.Infrastructure
{
    public static class McpRateLimitHelper
    {
        public static Func<OnRejectedContext, CancellationToken, ValueTask> CreateOnRejectedHandler(
            int defaultRetryAfterSeconds)
        {
            return async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue)
                    ? retryAfterValue.TotalSeconds
                    : defaultRetryAfterSeconds;

                context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter).ToString();

                var response = new
                {
                    jsonrpc = "2.0",
                    error = new
                    {
                        code = -32000,
                        message = $"Rate limit exceeded. Retry after {retryAfter:F0} seconds."
                    },
                    id = (string)null
                };

                await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            };
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
                System.Diagnostics.Debug.WriteLine(
                    $"[MCP RateLimit] FixedWindow有効: PermitLimit={rateLimit.FixedWindow.PermitLimit}, WindowSeconds={rateLimit.FixedWindow.WindowSeconds}");

                limiters.Add(CreateFixedWindowLimiter(rateLimit.FixedWindow));
            }

            if (rateLimit.SlidingWindow?.Enabled == true)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[MCP RateLimit] SlidingWindow有効: PermitLimit={rateLimit.SlidingWindow.PermitLimit}, WindowSeconds={rateLimit.SlidingWindow.WindowSeconds}");

                limiters.Add(CreateSlidingWindowLimiter(rateLimit.SlidingWindow));
            }

            if (rateLimit.TokenBucket?.Enabled == true)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[MCP RateLimit] TokenBucket有効: TokenLimit={rateLimit.TokenBucket.TokenLimit}, TokensPerPeriod={rateLimit.TokenBucket.TokensPerPeriod}");

                limiters.Add(CreateTokenBucketLimiter(rateLimit.TokenBucket));
            }

            if (rateLimit.Concurrency?.Enabled == true)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[MCP RateLimit] Concurrency有効: PermitLimit={rateLimit.Concurrency.PermitLimit}");

                limiters.Add(CreateConcurrencyLimiter(rateLimit.Concurrency));
            }

            System.Diagnostics.Debug.WriteLine($"[MCP RateLimit] 有効なリミッター数: {limiters.Count}");

            return limiters;
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
