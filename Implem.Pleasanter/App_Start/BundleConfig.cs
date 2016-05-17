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
                .Include("~/Scripts/KeyControls.js")
                .Include("~/Scripts/Legends.js")
                .Include("~/Scripts/Logins.js")
                .Include("~/Scripts/Navigations.js")
                .Include("~/Scripts/OutgoingMails.js")
                .Include("~/Scripts/Position.js")
                .Include("~/Scripts/Search.js")
                .Include("~/Scripts/Separates.js")
                .Include("~/Scripts/SiteMenu.js")
                .Include("~/Scripts/SiteSettings.js")
                .Include("~/Scripts/Statuses.js")
                .Include("~/Scripts/Times.js")
                .Include("~/Scripts/Focuses.js")
                .Include("~/Scripts/SiteInfo.js")
                .Include("~/Scripts/Transports.js")
                .Include("~/Scripts/JqueryUi.js")
                .Include("~/Scripts/_Show.js"));
            Validators(bundles);
            bundles.Add(new StyleBundle("~/content/styles")
                .Include("~/Styles/Site.css"));
        }

        private static void Validators(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/TenantValidator").Include("~/Scripts/Validators/Tenant.js"));
            bundles.Add(new ScriptBundle("~/bundles/DemoValidator").Include("~/Scripts/Validators/Demo.js"));
            bundles.Add(new ScriptBundle("~/bundles/DeptValidator").Include("~/Scripts/Validators/Dept.js"));
            bundles.Add(new ScriptBundle("~/bundles/UserValidator").Include("~/Scripts/Validators/User.js"));
            bundles.Add(new ScriptBundle("~/bundles/OutgoingMailValidator").Include("~/Scripts/Validators/OutgoingMail.js"));
            bundles.Add(new ScriptBundle("~/bundles/SiteValidator").Include("~/Scripts/Validators/Site.js"));
            bundles.Add(new ScriptBundle("~/bundles/IssueValidator").Include("~/Scripts/Validators/Issue.js"));
            bundles.Add(new ScriptBundle("~/bundles/ResultValidator").Include("~/Scripts/Validators/Result.js"));
            bundles.Add(new ScriptBundle("~/bundles/WikiValidator").Include("~/Scripts/Validators/Wiki.js"));
        }
    }
}
