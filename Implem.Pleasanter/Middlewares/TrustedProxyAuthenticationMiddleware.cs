using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Middlewares
{
    /// <summary>
    /// Middleware for trusted reverse proxy authentication.
    /// Reads a configurable HTTP header set by a trusted proxy and automatically
    /// signs in the matching Pleasanter user via cookie authentication.
    ///
    /// The header value is matched against the LoginId column in the Users table.
    /// If a matching, enabled user is found, a cookie authentication ticket is issued.
    /// If no match is found, the request continues to the normal login flow.
    ///
    /// Configuration (environment variables):
    ///   TRUSTED_PROXY_AUTH_ENABLED  - Set to any non-empty value to enable this middleware
    ///   TRUSTED_PROXY_AUTH_HEADER   - HTTP header name containing the authenticated username
    ///
    /// Supported reverse proxies (examples):
    ///   Snowflake SPCS:          Sf-Context-Current-User
    ///   nginx auth_request:      X-Forwarded-User
    ///   Entra ID App Proxy:      X-MS-CLIENT-PRINCIPAL-NAME
    ///   Cloudflare Access:       Cf-Access-Authenticated-User-Email
    /// </summary>
    public class TrustedProxyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _headerName;

        public TrustedProxyAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
            _headerName = Environment.GetEnvironmentVariable("TRUSTED_PROXY_AUTH_HEADER");
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (_headerName.IsNullOrEmpty())
            {
                await _next(httpContext);
                return;
            }

            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                await _next(httpContext);
                return;
            }

            var proxyUser = httpContext.Request.Headers[_headerName].FirstOrDefault();
            if (string.IsNullOrEmpty(proxyUser))
            {
                await _next(httpContext);
                return;
            }

            try
            {
                var context = new Context(
                    request: false,
                    sessionStatus: false,
                    sessionData: false,
                    user: false,
                    item: false);

                var userModel = new UserModel().Get(
                    context: context,
                    ss: null,
                    where: Rds.UsersWhere()
                        .LoginId(proxyUser)
                        .Disabled(false)
                        .Lockout(false));

                if (userModel.AccessStatus == Databases.AccessStatuses.Selected)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userModel.LoginId)
                    };
                    var identity = new ClaimsIdentity(claims, "TrustedProxy");
                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        properties);

                    httpContext.User = principal;
                }
            }
            catch (Exception)
            {
                // If DB lookup fails, fall through to normal login
            }

            await _next(httpContext);
        }
    }

    public static class TrustedProxyAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTrustedProxyAuthentication(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<TrustedProxyAuthenticationMiddleware>();
        }
    }
}
