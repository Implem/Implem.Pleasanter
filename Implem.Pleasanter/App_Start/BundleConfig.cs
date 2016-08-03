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
                "~/Scripts/_Ajax.js",
                "~/Scripts/_DefaultButton.js",
                "~/Scripts/_Form.js",
                "~/Scripts/_Time.js",
                "~/Scripts/BurnDown.js",
                "~/Scripts/Dialog.js",
                "~/Scripts/Display.js",
                "~/Scripts/DropDown.js",
                "~/Scripts/EventHandler.js",
                "~/Scripts/Export.js",
                "~/Scripts/Focuses.js",
                "~/Scripts/Gantt.js",
                "~/Scripts/Grid.js",
                "~/Scripts/History.js",
                "~/Scripts/Import.js",
                "~/Scripts/Item.js",
                "~/Scripts/JqueryUi.js",
                "~/Scripts/Kamban.js",
                "~/Scripts/KeyControl.js",
                "~/Scripts/Legend.js",
                "~/Scripts/Login.js",
                "~/Scripts/Markdown.js",
                "~/Scripts/Move.js",
                "~/Scripts/Navigation.js",
                "~/Scripts/OutgoingMail.js",
                "~/Scripts/Position.js",
                "~/Scripts/Search.js",
                "~/Scripts/SiteInfo.js",
                "~/Scripts/Separate.js",
                "~/Scripts/SiteMenu.js",
                "~/Scripts/SiteSettings.js",
                "~/Scripts/Status.js",
                "~/Scripts/TimeSeries.js",
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
