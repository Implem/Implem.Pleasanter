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
            string modelName,
            bool allowAccess)
        {
            return hb
                .Script(src: Navigations.Get("Scripts/Plugins/jquery-2.1.4.min.js"))
                .Script(src: Navigations.Get("Scripts/Plugins/jquery-ui.min.js"))
                .Script(src: Navigations.Get("Scripts/Plugins/jquery.validate.min.js"))
                .Script(src: Navigations.Get("Scripts/Plugins/d3.min.js"))
                .Script(script: script, _using: script != string.Empty)
                .Script(script: userScript, _using: userScript != string.Empty)
                .Validator(methodType: methodType, modelName: modelName, allowAccess: allowAccess)
                .Internationalization();
        }

        private static HtmlBuilder Validator(
            this HtmlBuilder hb,
            BaseModel.MethodTypes methodType,
            string modelName,
            bool allowAccess)
        {
            return Editor(methodType) && allowAccess
                ? hb
                    .Script(src: Validator(modelName))
                    .Script(src: Validator("OutgoingMail"))
                : hb;
        }

        private static bool Editor(BaseModel.MethodTypes methodType)
        {
            return
                methodType == BaseModel.MethodTypes.Edit ||
                methodType == BaseModel.MethodTypes.New;
        }

        private static string Validator(string modelName)
        {
            switch (modelName)
            {
                case "Tenant": return ResolveBundleUrl("~/bundles/TenantValidator");
                case "Demo": return ResolveBundleUrl("~/bundles/DemoValidator");
                case "Dept": return ResolveBundleUrl("~/bundles/DeptValidator");
                case "User": return ResolveBundleUrl("~/bundles/UserValidator");
                case "OutgoingMail": return ResolveBundleUrl("~/bundles/OutgoingMailValidator");
                case "Site": return ResolveBundleUrl("~/bundles/SiteValidator");
                case "Issue": return ResolveBundleUrl("~/bundles/IssueValidator");
                case "Result": return ResolveBundleUrl("~/bundles/ResultValidator");
                case "Wiki": return ResolveBundleUrl("~/bundles/WikiValidator");
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
    }
}
