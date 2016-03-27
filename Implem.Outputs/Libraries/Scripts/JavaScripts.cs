using System.Web.Optimization;
namespace Implem.Pleasanter.Libraries.Scripts
{
    public static class JavaScripts
    {
        public static string Validator(string modelName)
        {
            switch (modelName)
            {
                case "Tenant": return ResolveBundleUrl("~/bundles/TenantValidator");
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
    }
}
