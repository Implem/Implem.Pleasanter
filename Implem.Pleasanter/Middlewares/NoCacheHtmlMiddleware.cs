using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Middlewares
{
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
