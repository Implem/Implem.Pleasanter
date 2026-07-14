using Implem.ParameterAccessor.Parts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Implem.Pleasanter.MCP.Infrastructure
{
    public class McpRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PartitionedRateLimiter<HttpContext> _limiter;

        public McpRateLimitMiddleware(RequestDelegate next, McpRateLimitSettings rateLimit)
        {
            _next = next;
            _limiter = McpRateLimitHelper.CreateChainedLimiter(rateLimit);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_limiter == null)
            {
                await _next(context);
                return;
            }
            using var lease = await _limiter.AcquireAsync(context, permitCount: 1, context.RequestAborted);
            if (lease.IsAcquired)
            {
                await _next(context);
                return;
            }
            await McpRateLimitHelper.OnRejectedAsync(context, lease, context.RequestAborted);
        }
    }

    public static class McpRateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseMcpRateLimitMiddleware(this IApplicationBuilder app, McpRateLimitSettings rateLimit)
        {
            return app.UseMiddleware<McpRateLimitMiddleware>(rateLimit);
        }
    }
}
