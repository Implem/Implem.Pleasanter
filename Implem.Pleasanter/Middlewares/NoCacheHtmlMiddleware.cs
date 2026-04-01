using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Middlewares
{
    /// <summary>
    /// Middleware that adds Cache-Control headers to HTML responses to prevent browser caching.
    ///
    /// This is useful in containerized environments (Kubernetes, Docker Swarm, SPCS, etc.)
    /// where services may be suspended and resumed. Without this middleware, browsers may
    /// cache error pages (503, auth redirects) during downtime and continue displaying
    /// them after the service recovers.
    ///
    /// Configuration:
    ///   Service.json: "DisableHtmlCache": true
    ///   Or environment variable: PLEASANTER_DISABLE_HTML_CACHE=true
    ///
    /// When enabled, HTML responses (text/html) will include:
    ///   Cache-Control: no-cache, no-store, must-revalidate
    ///   Pragma: no-cache
    ///   Expires: 0
    ///
    /// Static assets (JS, CSS, images) are not affected.
    /// </summary>
    public class NoCacheHtmlMiddleware
    {
        private readonly RequestDelegate _next;

        public NoCacheHtmlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var contentType = context.Response.ContentType;
                if (!string.IsNullOrEmpty(contentType) &&
                    contentType.Contains("text/html", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                    context.Response.Headers["Pragma"] = "no-cache";
                    context.Response.Headers["Expires"] = "0";
                }
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

    public static class NoCacheHtmlMiddlewareExtensions
    {
        public static IApplicationBuilder UseNoCacheHtml(this IApplicationBuilder app)
        {
            return app.UseMiddleware<NoCacheHtmlMiddleware>();
        }
    }
}
