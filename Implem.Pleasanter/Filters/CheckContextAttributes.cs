﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using System.Linq;
using System.Web.Mvc;
namespace Implem.Pleasanter.Filters
{
    public class CheckContextAttributes : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = new Context(
                sessionStatus: false,
                sessionData: false,
                item: false);
            if (context.Controller != "errors" && Parameters.SyntaxErrors?.Any() == true)
            {
                filterContext.Result = new RedirectResult(
                    Locations.ParameterSyntaxError(context: context));
            }
            if (context.Authenticated
                && !context.ContractSettings.AllowedIpAddress(context.UserHostAddress))
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
            if (Parameters.Security.TokenCheck
                && filterContext.HttpContext.Request.Form.Count > 0
                && filterContext.HttpContext.Request.Form["Token"] != context.Token())
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            Message = Displays.InvalidRequest(context: context)
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
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
            if (!context.LoginId.IsNullOrEmpty())
            {
                if (!context.Authenticated)
                {
                    if (Authentications.Windows())
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
            SiteInfo.Reflesh(context: context);
        }
    }
}