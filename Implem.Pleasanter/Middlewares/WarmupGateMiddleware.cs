using System;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.BackgroundServices;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace Implem.Pleasanter.Middlewares
{
    public class WarmupGateMiddleware(RequestDelegate next)
    {
        private const string IisAppInitUserAgent =
            "IIS Application Initialization";

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (ApplicationWarmupHostedService.CurrentStatus == WarmupStatus.Completed)
            {
                await next(httpContext);
                return;
            }
            if (IsIisAppInitRequest(httpContext: httpContext))
            {
                await WaitAndRespond(httpContext: httpContext);
                return;
            }
            var path = httpContext.Request.Path.Value ?? string.Empty;
            var isHealthCheckPath = Parameters.Security.HealthCheck.Enabled
                && path.StartsWith("/healthz", StringComparison.OrdinalIgnoreCase);
            if (isHealthCheckPath ||
                path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/errors", StringComparison.OrdinalIgnoreCase))
            {
                await next(httpContext);
                return;
            }
            var api = IsApiRequest(httpContext: httpContext);
            var context = new Context(
                    sessionStatus: false,
                    sessionData: false,
                    user: false,
                    item: false,
                    setPermissions: false,
                    api: api);
            httpContext.Response.Headers.RetryAfter = "30";
            if (context.Ajax ||
                context.Api ||
                (path.StartsWith("/healthz", StringComparison.OrdinalIgnoreCase)
                    && !Parameters.Security.HealthCheck.Enabled))
            {
                httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(
                    text: Error.Types.ServiceUnavailable.MessageJson(context: context));
                return;
            }
            var basePath = httpContext.Request.PathBase.HasValue
                ? $"/{httpContext.Request.PathBase.Value.Trim('/')}"
                : string.Empty;
            httpContext.Response.Redirect($"{basePath}/errors/warmup");
            return;
        }

        private static bool IsApiRequest(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(
                other: new PathString("/api"),
                comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            var policyName = httpContext.GetEndpoint()
                ?.Metadata
                .GetMetadata<EnableRateLimitingAttribute>()
                ?.PolicyName;
            return policyName == "Api" || policyName == "ApiHeavy";
        }

        private static bool IsIisAppInitRequest(HttpContext httpContext)
        {
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            return userAgent.StartsWith(
                value: IisAppInitUserAgent,
                comparisonType: StringComparison.OrdinalIgnoreCase);
        }

        private static async Task WaitAndRespond(HttpContext httpContext)
        {
            try
            {
                await ApplicationWarmupHostedService.WaitForCompletionAsync(
                    cancellationToken: httpContext.RequestAborted);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            httpContext.Response.StatusCode =
                ApplicationWarmupHostedService.CurrentStatus == WarmupStatus.Completed
                    ? StatusCodes.Status200OK
                    : StatusCodes.Status503ServiceUnavailable;
            await httpContext.Response.WriteAsync(
                text: ApplicationWarmupHostedService.CurrentStatus.ToString());
        }
    }

    public static class WarmupGateMiddlewareExtensions
    {
        public static IApplicationBuilder UseWarmupGateMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WarmupGateMiddleware>();
        }
    }
}
