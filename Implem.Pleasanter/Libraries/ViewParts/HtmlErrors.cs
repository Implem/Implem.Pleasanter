using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Utilities;
using System.Web;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.ViewParts
{
    public static class HtmlErrors
    {
        public static string Get()
        {
            var error = HttpContext.Current.Session["error"] as ExceptionContext;
            var hb = Html.Builder();
            return hb.Template(
                siteId: 0,
                modelName: string.Empty,
                title: Displays.Error(),
                permissionType: Permissions.Types.NotSet,
                verType: Versions.VerTypes.Latest,
                methodType: Models.BaseModel.MethodTypes.NotSet,
                allowAccess: true,
                action: () => hb
                    .Section(css: "error-page", action: () => hb
                        .H(number: 1, css: "error-page-title", action: () => hb
                            .Text(Displays.ExceptionTitle()))
                        .P(css: "error-page-body", action: () => hb
                            .Text(Displays.ExceptionBody()))
                        .P(css: "error-page-message", action: () => hb
                            .Text(error.Exception.Message))
                        .Details(error))).ToString();
        }

        private static HtmlBuilder Details(this HtmlBuilder hb, ExceptionContext error)
        {
            if (HttpContext.Current.Request.UserHostAddress == "::1" && Sessions.Developer())
            {
                hb
                    .P(css: "error-page-action", action: () => hb
                        .Em(action: () => hb
                            .Text(error.RouteData.Values["controller"].ToString()))
                        .Em(action: () => hb
                            .Text(error.RouteData.Values["action"].ToString()))
                        .Em(action: () => hb
                            .Text(error.Exception.TargetSite.ToString()))
                        .Em(action: () => hb
                            .Text(error.Exception.HResult.ToString())))
                    .P(css: "error-page-stacktrace", action: () => hb
                        .Text(error.Exception.StackTrace));
            }
            return hb;
        }
    }
}