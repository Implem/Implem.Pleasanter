using System;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Scim;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Implem.Pleasanter.Filters
{
    public class CheckScimContextAttributes : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            Performance.GeneratedTime = DateTime.Now;
            Performance.PreviousTime = DateTime.Now;
            if (!ScimFeatureUtilities.Enabled())
            {
                filterContext.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                filterContext.Result = ScimResponse.Error(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: "SCIM is disabled");
                return;
            }
            var context = new Context(
                sessionStatus: false,
                sessionData: false,
                item: false,
                setPermissions: false,
                api: true);
            if (!IpAddresses.AllowedIpAddress(
                context: context,
                allowIpAddresses: Parameters.Security.AllowIpAddresses,
                ipRestrictionExcludeMembers: null,
                ipAddress: context.UserHostAddress))
            {
                filterContext.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                filterContext.Result = ScimResponse.Error(
                    statusCode: StatusCodes.Status403Forbidden,
                    detail: "Forbidden");
                return;
            }
            if (!ScimTokenValidator.TryValidate(
                context,
                filterContext.HttpContext.Request.Headers.Authorization.ToString(),
                out var validation))
            {
                filterContext.HttpContext.Response.Headers.WWWAuthenticate = "Bearer realm=\"pleasanter\"";
                filterContext.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                filterContext.Result = ScimResponse.Error(
                    statusCode: StatusCodes.Status401Unauthorized,
                    detail: "Unauthorized");
                _ = new SysLogModel(
                    context: context,
                    method: $"{nameof(CheckScimContextAttributes)}.{nameof(OnAuthorization)}",
                    message: $"SCIM token validation failed. {validation.ToJson()}",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
                return;
            }
            context.TenantId = validation.TenantId;
            context.UserId = validation.UserId;
            if (!SiteInfo.TenantCaches.ContainsKey(validation.TenantId))
            {
                SiteInfo.TenantCaches.TryAdd(
                    key: validation.TenantId,
                    value: new TenantCache(context));
                SiteInfo.Refresh(context);
            }
            filterContext.HttpContext.Items[ScimRequestContext.ItemKey] = new ScimRequestContext
            {
                Context = context,
                TokenPrefix = validation.TokenPrefix
            };
        }
    }
}
