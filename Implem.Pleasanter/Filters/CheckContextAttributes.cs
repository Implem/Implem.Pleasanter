﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
namespace Implem.PleasanterFilters
{
    public class CheckContextAttributes : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            Performance.GeneratedTime = DateTime.Now;
            Performance.PreviousTime = DateTime.Now;
            var context = new Context(
                sessionStatus: false,
                sessionData: false,
                item: false,
                setPermissions: false);
            if (context.Controller != "errors" && Parameters.SyntaxErrors?.Any() == true)
            {
                filterContext.Result = new RedirectResult(
                    Locations.ParameterSyntaxError(context: context));
            }
            if ((filterContext.HttpContext.Request.Path == null
                || !filterContext.HttpContext.Request.Path.ToString().ToLower().StartsWith("/api/"))
                && !IpAddresses.AllowedIpAddress(
                    context: context,
                    allowIpAddresses: Parameters.Security.AllowIpAddresses,
                    ipRestrictionExcludeMembers: Parameters.Security.IpRestrictionExcludeMembers,
                    ipAddress: context.UserHostAddress))
            {
                filterContext.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "403 Forbidden"
                };
                return;
            }
            if (context.Authenticated
                && !context.ContractSettings.AllowedIpAddress(
                    context: context,
                    context.UserHostAddress))
            {
                Authentications.SignOut(context: context);
                filterContext.Result = new RedirectResult(
                    Locations.InvalidIpAddress(context: context));
                return;
            }
            if (context.Authenticated
                && context.ContractSettings.OverDeadline(context: context))
            {
                Authentications.SignOut(context: context);
                filterContext.Result = new RedirectResult(
                    Locations.Login(context: context) + "?expired=1");
                return;
            }
            if (context.Authenticated
                && Parameters.Security.TokenCheck
                && filterContext.HttpContext.Request.HasFormContentType
                && filterContext.HttpContext.Request.Form.Count > 0
                && filterContext.HttpContext.Request.Form["Token"] != context.Token())
            {
                filterContext.HttpContext.Response.StatusCode = 400;
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    filterContext.Result = new JsonResult(
                    new
                    {
                        Message = Displays.InvalidRequest(context: context)
                    });
                }
                else
                {
                    filterContext.Result = new ContentResult()
                    {
                        Content = Displays.InvalidRequest(context: context)
                    };
                }
                return;
            }
            if (context.IsAuthenticated == true
                && context.Action != "samllogin")
            {
                if (!context.Authenticated)
                {
                    if (Authentications.Windows(context: context))
                    {
                        filterContext.Result = new EmptyResult();
                        return;
                    }
                    else
                    {
                        Authentications.SignOut(context: context);
                        filterContext.Result = new RedirectResult(
                            Locations.Login(context: context));
                        return;
                    }
                }
            }
            if (!ValidatePostFiles(context, filterContext))
            {
                filterContext.Result = new ContentResult()
                {
                    StatusCode = 400,
                    Content = "400 Bad Request "
                };
                return;
            }
            SiteInfo.Refresh(context: context);
        }

        private bool ValidatePostFiles(Context context, AuthorizationFilterContext filterContext)
        {
            if (filterContext.HttpContext.Request.HasFormContentType == false) return true;
            var files = filterContext.HttpContext.Request.Form?.Files;
            if (files != null)
            {
                foreach (var f in files)
                {
                    if (Files.ValidateFileName(f.FileName) == false)
                    {
                        new SysLogModel(
                            context: context,
                            method: $"{nameof(CheckContextAttributes)}.{nameof(OnAuthorization)}",
                            message: $"Invalid File Name: '{f.FileName}'",
                            sysLogType: SysLogModel.SysLogTypes.Info);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
