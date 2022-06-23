using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
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
            if (!IpAddresses.AllowedIpAddress(
                allowIpAddresses: Parameters.Security.AllowIpAddresses,
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
            // ASP.NETによる認証が行われてセッションがログイン状態となっているかチェック
            // SamlLoginが要求されている場合にはユーザが作成されていないためチェックをスキップ
            if (context.IsAuthenticated == true
                && context.Action != "samllogin")
            {
                // データベースからユーザが取得できていない場合（無効、存在しない場合等）のチェック
                if (!context.Authenticated)
                {
                    if (Authentications.Windows(context: context))
                    {
                        // 統合Windows認証の場合は何もしない
                        filterContext.Result = new EmptyResult();
                        return;
                    }
                    else
                    {
                        // 通常の認証の場合にはサインアウトさせる
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
