using System.Web.Optimization;
namespace Implem.Pleasanter
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/Generals")
                .Include("~/Scripts/_Init.js")
                .Include("~/Scripts/_Time.js")
                .Include("~/Scripts/_DefaultButton.js")
                .Include("~/Scripts/BurnDown.js")
                .Include("~/Scripts/Displays.js")
                .Include("~/Scripts/EventHandlers.js")
                .Include("~/Scripts/Markdowns.js")
                .Include("~/Scripts/Dialogs.js")
                .Include("~/Scripts/DropDowns.js")
                .Include("~/Scripts/Histories.js")
                .Include("~/Scripts/Gantt.js")
                .Include("~/Scripts/Grids.js")
                .Include("~/Scripts/Kamban.js")
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
                .Include("~/Scripts/TimeSeries.js")
                .Include("~/Scripts/Focuses.js")
                .Include("~/Scripts/SiteInfo.js")
                .Include("~/Scripts/Transports.js")
                .Include("~/Scripts/JqueryUi.js")
                .Include("~/Scripts/_Show.js")
                .Include("~/Scripts/Validators/Custom.js"));
            Validators(bundles);
            bundles.Add(new StyleBundle("~/content/styles")
                .Include("~/Styles/Site.css"));
        }

        private static void Validators(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/TenantsValidator").Include("~/Scripts/Validators/Tenants.js"));
            bundles.Add(new ScriptBundle("~/bundles/DemosValidator").Include("~/Scripts/Validators/Demos.js"));
            bundles.Add(new ScriptBundle("~/bundles/DeptsValidator").Include("~/Scripts/Validators/Depts.js"));
            bundles.Add(new ScriptBundle("~/bundles/UsersValidator").Include("~/Scripts/Validators/Users.js"));
            bundles.Add(new ScriptBundle("~/bundles/OutgoingMailsValidator").Include("~/Scripts/Validators/OutgoingMails.js"));
            bundles.Add(new ScriptBundle("~/bundles/SitesValidator").Include("~/Scripts/Validators/Sites.js"));
            bundles.Add(new ScriptBundle("~/bundles/IssuesValidator").Include("~/Scripts/Validators/Issues.js"));
            bundles.Add(new ScriptBundle("~/bundles/ResultsValidator").Include("~/Scripts/Validators/Results.js"));
            bundles.Add(new ScriptBundle("~/bundles/WikisValidator").Include("~/Scripts/Validators/Wikis.js"));
        }
    }
}
