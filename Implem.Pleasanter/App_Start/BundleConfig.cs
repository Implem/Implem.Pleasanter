using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;
namespace Implem.Pleasanter
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            AddGenerals(bundles);
            AddValidators(bundles);
            AddStyles(bundles);
        }

        private static void AddGenerals(BundleCollection bundles)
        {
            var generals = new ScriptBundle("~/bundles/Generals");
            Generals().ForEach(path => generals.Include(path));
            bundles.Add(generals);
        }

        public static IEnumerable<string> Generals()
        {
            return new string[] {
                "~/Scripts/_Init.js",
                "~/Scripts/_Time.js",
                "~/Scripts/_DefaultButton.js",
                "~/Scripts/BurnDown.js",
                "~/Scripts/Displays.js",
                "~/Scripts/EventHandlers.js",
                "~/Scripts/Markdowns.js",
                "~/Scripts/Dialogs.js",
                "~/Scripts/DropDowns.js",
                "~/Scripts/Histories.js",
                "~/Scripts/Gantt.js",
                "~/Scripts/Grids.js",
                "~/Scripts/Kamban.js",
                "~/Scripts/KeyControls.js",
                "~/Scripts/Legends.js",
                "~/Scripts/Logins.js",
                "~/Scripts/Navigations.js",
                "~/Scripts/OutgoingMails.js",
                "~/Scripts/Position.js",
                "~/Scripts/Search.js",
                "~/Scripts/Separates.js",
                "~/Scripts/SiteMenu.js",
                "~/Scripts/SiteSettings.js",
                "~/Scripts/Statuses.js",
                "~/Scripts/TimeSeries.js",
                "~/Scripts/Focuses.js",
                "~/Scripts/SiteInfo.js",
                "~/Scripts/Transports.js",
                "~/Scripts/JqueryUi.js",
                "~/Scripts/_Show.js",
                "~/Scripts/Validators/Custom.js"
            };
        }

        private static void AddValidators(BundleCollection bundles)
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

        private static void AddStyles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/content/styles")
                .Include("~/Styles/Site.css"));
        }
    }
}
