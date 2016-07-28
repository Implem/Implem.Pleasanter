using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using System.Web.Optimization;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlScripts
    {
        public static HtmlBuilder Scripts(
            this HtmlBuilder hb,
            string script,
            string userScript,
            string referenceType,
            bool byRest)
        {
            return !byRest
                ? hb
                    .Script(src: Navigations.Get("Scripts/Plugins/jquery-2.1.4.min.js"))
                    .Script(src: Navigations.Get("Scripts/Plugins/jquery-ui.min.js"))
                    .Script(src: Navigations.Get("Scripts/Plugins/jquery.validate.min.js"))
                    .Script(src: Navigations.Get("Scripts/Plugins/d3.min.js"))
                    .Script(src: ResolveBundleUrl("~/bundles/Generals"))
                    .Script(script: script, _using: !script.IsNullOrEmpty())
                    .Script(script: userScript, _using: !userScript.IsNullOrEmpty())
                    .ItemValidator(referenceType: referenceType)
                    .Script(src: Src("OutgoingMails"))
                    .Internationalization()
                : hb;
        }

        public static HtmlBuilder ItemValidator(this HtmlBuilder hb, string referenceType)
        {
            return hb.Script(id: "ItemValidator", src: Src(referenceType));
        }

        private static string Src(string referenceType)
        {
            switch (referenceType)
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

        private static string Validator(string referenceType)
        {
            switch (referenceType)
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
