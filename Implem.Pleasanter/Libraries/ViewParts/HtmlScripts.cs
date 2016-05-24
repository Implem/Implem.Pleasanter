using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Models;
using System.Web.Optimization;
namespace Implem.Pleasanter.Libraries.ViewParts
{
    public static class HtmlScripts
    {
        public static HtmlBuilder Scripts(
            this HtmlBuilder hb,
            BaseModel.MethodTypes methodType,
            string script,
            string userScript,
            string referenceId,
            bool allowAccess)
        {
            return hb
                .Script(src: Navigations.Get("Scripts/Plugins/jquery-2.1.4.min.js"))
                .Script(src: Navigations.Get("Scripts/Plugins/jquery-ui.min.js"))
                .Script(src: Navigations.Get("Scripts/Plugins/jquery.validate.min.js"))
                .Script(src: Navigations.Get("Scripts/Plugins/d3.min.js"))
                .Script(script: script, _using: !script.IsNullOrEmpty())
                .Script(script: userScript, _using: !userScript.IsNullOrEmpty())
                .Validator(methodType: methodType, referenceId: referenceId, allowAccess: allowAccess)
                .Internationalization();
        }

        private static HtmlBuilder Validator(
            this HtmlBuilder hb,
            BaseModel.MethodTypes methodType,
            string referenceId,
            bool allowAccess)
        {
            return Editor(methodType) && allowAccess
                ? hb
                    .Script(src: Validator(referenceId))
                    .Script(src: Validator("OutgoingMails"))
                : hb;
        }

        private static bool Editor(BaseModel.MethodTypes methodType)
        {
            return
                methodType == BaseModel.MethodTypes.Edit ||
                methodType == BaseModel.MethodTypes.New;
        }

        private static string ResolveBundleUrl(string url)
        {
            return BundleTable.Bundles.ResolveBundleUrl(url);
        }

        private static HtmlBuilder Internationalization(this HtmlBuilder hb)
        {
            switch (Sessions.Language())
            {
                case "ja": return hb
                    .Script(src: Navigations.Get(
                        "Scripts/Plugins/jquery-ui/i18n/datepicker-ja.js"));
                default: return hb;
            }
        }

        private static string Validator(string referenceId)
        {
            switch (referenceId)
            {
                case "Tenants": return ResolveBundleUrl("~/bundles/TenantsValidator");
                case "Demos": return ResolveBundleUrl("~/bundles/DemosValidator");
                case "Depts": return ResolveBundleUrl("~/bundles/DeptsValidator");
                case "Users": return ResolveBundleUrl("~/bundles/UsersValidator");
                case "OutgoingMails": return ResolveBundleUrl("~/bundles/OutgoingMailsValidator");
                case "Sites": return ResolveBundleUrl("~/bundles/SitesValidator");
                case "Issues": return ResolveBundleUrl("~/bundles/IssuesValidator");
                case "Results": return ResolveBundleUrl("~/bundles/ResultsValidator");
                case "Wikis": return ResolveBundleUrl("~/bundles/WikisValidator");
                default: return string.Empty;
            }
        }
    }
}
