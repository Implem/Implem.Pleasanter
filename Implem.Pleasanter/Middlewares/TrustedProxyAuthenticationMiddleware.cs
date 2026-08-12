using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Middlewares
{
    public class TrustedProxyAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<TrustedProxyAuthenticationMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            var header = Parameters.Authentication.TrustedProxyParameters?.Header
                ?? string.Empty;
            if (string.IsNullOrEmpty(header))
            {
                if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Trusted proxy authentication skipped: header name is not configured.");
                await next(httpContext);
                return;
            }
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Trusted proxy authentication skipped: request is already authenticated.");
                await next(httpContext);
                return;
            }
            if (!IsTrustedProxy(httpContext, out var trustReason))
            {
                if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Trusted proxy authentication skipped: {Reason}", trustReason);
                await next(httpContext);
                return;
            }
            if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Trusted proxy source accepted: {Reason}", trustReason);
            var proxyUser = httpContext.Request.Headers[header].FirstOrDefault();
            if (string.IsNullOrEmpty(proxyUser))
            {
                if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Trusted proxy authentication skipped: configured header is missing or empty.");
                await next(httpContext);
                return;
            }
            var context = new Context(
                    request: false,
                    sessionStatus: false,
                    sessionData: false,
                    user: false,
                    item: false);
            try
            {
                var userModel = new UserModel().Get(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context),
                    where: Rds.UsersWhere()
                        .LoginId(
                            value: context.Sqls.EscapeValue(proxyUser),
                            _operator: context.Sqls.LikeWithEscape)
                        .Disabled(false)
                        .Lockout(false));
                if (userModel.AccessStatus == Databases.AccessStatuses.Selected)
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userModel.LoginId)
                    };
                    var identity = new ClaimsIdentity(claims, "TrustedProxy");
                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    await httpContext.SignInAsync(
                        scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                        principal: principal,
                        properties: properties);
                    _ = userModel.AllowAfterUrl(
                        context: context,
                        returnUrl: "",
                        createPersistentCookie: true);
                    httpContext.User = principal;
                    if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Trusted proxy authentication succeeded.");
                }
                else if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Trusted proxy authentication skipped: no active matching user found.");
            }
            catch (Exception ex)
            {
                _ = new SysLogModel(context, ex);
                if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Trusted proxy authentication failed with exception type: {ExceptionType}", ex.GetType().Name);
            }
            await next(httpContext);
        }

        internal static bool IsTrustedProxy(HttpContext httpContext, out string reason)
        {
            var knownNetworks = Parameters.Security.ForwardedHeaders?.KnownNetworks ?? [];
            var knownProxies = Parameters.Security.ForwardedHeaders?.KnownProxies ?? [];
            var hasNetworks = knownNetworks.Count > 0;
            var hasProxies = knownProxies.Count > 0;
            if (!hasNetworks && !hasProxies)
            {
                reason = "KnownNetworks/KnownProxies are empty.";
                return false;
            }
            var remoteIp = ResolveProxyIp(httpContext);
            if (remoteIp == null)
            {
                reason = "Proxy source IP could not be resolved.";
                return false;
            }
            if (hasProxies)
            {
                foreach (var proxyStr in knownProxies)
                {
                    if (IPAddress.TryParse(proxyStr?.Trim(), out var proxy))
                    {
                        var compareProxy = NormalizeIp(proxy);
                        if (remoteIp.Equals(compareProxy))
                        {
                            reason = "Matched KnownProxies.";
                            return true;
                        }
                    }
                }
            }
            if (hasNetworks)
            {
                foreach (var networkStr in knownNetworks)
                {
                    if (IPNetwork.TryParse(networkStr?.Trim(), out var network) && network.Contains(remoteIp))
                    {
                        reason = "Matched KnownNetworks.";
                        return true;
                    }
                }
            }
            reason = "Source IP did not match KnownProxies/KnownNetworks.";
            return false;
        }

        private static IPAddress ResolveProxyIp(HttpContext httpContext)
        {
            return httpContext.Items.TryGetValue(
                TrustedProxyAuthenticationMiddlewareExtensions.ProxySourceIpKey,
                out var value)
                    ? NormalizeIp(value as IPAddress)
                    : null;
        }

        private static IPAddress NormalizeIp(IPAddress ip)
        {
            if (ip == null)
            {
                return null;
            }
            return ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip;
        }

    }

    public static class TrustedProxyAuthenticationMiddlewareExtensions
    {
        internal static readonly object ProxySourceIpKey = new();

        public static IApplicationBuilder UseTrustedProxySourceIpCapture(
            this IApplicationBuilder app)
        {
            if (Enabled())
            {
                return app.Use(async (httpContext, next) =>
                {
                    httpContext.Items[ProxySourceIpKey] = httpContext.Connection.RemoteIpAddress;
                    await next(httpContext);
                });
            }
            return app;
        }

        public static IApplicationBuilder UseTrustedProxyAuthentication(
            this IApplicationBuilder app)
        {
            if (Enabled())
            {
                return app.UseMiddleware<TrustedProxyAuthenticationMiddleware>();
            }
            return app;
        }

        private static bool Enabled()
        {
            return Parameters.Authentication.TrustedProxyParameters?.Enabled ?? false;
        }
    }
}
