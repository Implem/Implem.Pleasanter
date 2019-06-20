using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Implem.Pleasanter.NetCore.Filters
{
    public class CheckContextAttributes : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var context = new ContextImplement(
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
            if (!context.LoginId.IsNullOrEmpty())
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
            SiteInfo.Reflesh(context: context);
        }
    }
}
