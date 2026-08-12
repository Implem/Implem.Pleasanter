using System;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Implem.Pleasanter.Libraries.BackgroundServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IHealthChecksBuilder AddDatabaseHealthCheck(
            this IHealthChecksBuilder services,
            bool enableDatabaseCheck,
            string dbms,
            string conStr,
            string healthQuery)
        {
            if (!enableDatabaseCheck)
            {
                return services;
            }
            return dbms switch
            {
                "SQLServer" => services.AddSqlServer(
                    connectionString: conStr,
                    healthQuery: healthQuery),
                "PostgreSQL" => services.AddNpgSql(
                    connectionString: conStr,
                    healthQuery: healthQuery),
                "MySQL" => services.AddMySql(
                    connectionString: conStr,
                    healthQuery: healthQuery),
                _ => services,
            };
        }

        public static IHealthChecksBuilder AddWarmupHealthCheck(this IHealthChecksBuilder builder)
        {
            return builder.AddCheck("warmup", () =>
            {
                var status = ApplicationWarmupHostedService.CurrentStatus;
                return status switch
                {
                    WarmupStatus.NotStarted => HealthCheckResult.Degraded("Warmup not started"),
                    WarmupStatus.InProgress => HealthCheckResult.Degraded("Warmup in progress"),
                    WarmupStatus.Completed => HealthCheckResult.Healthy("Warmup completed"),
                    WarmupStatus.Failed => HealthCheckResult.Unhealthy("Warmup failed"),
                    WarmupStatus.Canceled => HealthCheckResult.Unhealthy("Warmup canceled"),
                    WarmupStatus.TimedOut => HealthCheckResult.Unhealthy("Warmup timed out"),
                    _ => HealthCheckResult.Unhealthy($"Warmup status: {status}")
                };
            });
        }

        public static void MapDefaultHealthChecks(
            this IEndpointRouteBuilder endpoints,
            bool enableDetailedResponse,
            string[] requireHosts)
        {
            endpoints
                .MapHealthChecks(
                    pattern: "/healthz",
                    options: CreateHealthCheckOptions(
                        predicate: static _ => true,
                        responseWriter: enableDetailedResponse
                            ? UIResponseWriter.WriteHealthCheckUIResponse
                            : null))
                .RequireHost(requireHosts ?? []);
        }

        private static HealthCheckOptions CreateHealthCheckOptions(
            Func<HealthCheckRegistration, bool> predicate,
            Func<HttpContext, HealthReport, Task> responseWriter)
        {
            var options = new HealthCheckOptions()
            {
                Predicate = predicate,
            };
            options.ResultStatusCodes[HealthStatus.Healthy] = StatusCodes.Status200OK;
            options.ResultStatusCodes[HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable;
            options.ResultStatusCodes[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable;
            if (responseWriter != null)
            {
                options.ResponseWriter = responseWriter;
            }
            return options;
        }
    }
}
