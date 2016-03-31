using System.Web.Optimization;
namespace Implem.Pleasanter
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/Generals")
                .Include("~/Scripts/_Init.js")
                .Include("~/Scripts/BurnDowns.js")
                .Include("~/Scripts/Displays.js")
                .Include("~/Scripts/EventHandlers.js")
                .Include("~/Scripts/Markdowns.js")
                .Include("~/Scripts/Dialogs.js")
                .Include("~/Scripts/DropDowns.js")
                .Include("~/Scripts/Histories.js")
                .Include("~/Scripts/Gantts.js")
                .Include("~/Scripts/Grids.js")
                .Include("~/Scripts/Legends.js")
                .Include("~/Scripts/Navigations.js")
                .Include("~/Scripts/OutgoingMails.js")
                .Include("~/Scripts/Position.js")
                .Include("~/Scripts/Search.js")
                .Include("~/Scripts/Separates.js")
                .Include("~/Scripts/SiteMenu.js")
                .Include("~/Scripts/SiteSettings.js")
                .Include("~/Scripts/Statuses.js")
                .Include("~/Scripts/Tabs.js")
                .Include("~/Scripts/Times.js")
                .Include("~/Scripts/Focuses.js")
                .Include("~/Scripts/SiteInfo.js")
                .Include("~/Scripts/Transports.js")
                .Include("~/Scripts/JqueryUi.js")
                .Include("~/Scripts/_Show.js"));
            bundles.Add(new ScriptBundle("~/bundles/TenantValidator").Include("~/Scripts/Validations/Tenant.js"));
            bundles.Add(new ScriptBundle("~/bundles/DeptValidator").Include("~/Scripts/Validations/Dept.js"));
            bundles.Add(new ScriptBundle("~/bundles/UserValidator").Include("~/Scripts/Validations/User.js"));
            bundles.Add(new ScriptBundle("~/bundles/OutgoingMailValidator").Include("~/Scripts/Validations/OutgoingMail.js"));
            bundles.Add(new ScriptBundle("~/bundles/SiteValidator").Include("~/Scripts/Validations/Site.js"));
            bundles.Add(new ScriptBundle("~/bundles/IssueValidator").Include("~/Scripts/Validations/Issue.js"));
            bundles.Add(new ScriptBundle("~/bundles/ResultValidator").Include("~/Scripts/Validations/Result.js"));
            bundles.Add(new ScriptBundle("~/bundles/WikiValidator").Include("~/Scripts/Validations/Wiki.js"));
            bundles.Add(new StyleBundle("~/content/styles")
                .Include("~/Styles/Site.css"));
        }
    }
}
