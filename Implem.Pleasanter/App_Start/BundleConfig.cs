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
                "~/Scripts/AggregationEvents.js",
                "~/Scripts/AnchorEvents.js",
                "~/Scripts/BurnDown.js",
                "~/Scripts/BurnDownEvents.js",
                "~/Scripts/ControllEvents.js",
                "~/Scripts/Dialog.js",
                "~/Scripts/DialogEvents.js",
                "~/Scripts/Display.js",
                "~/Scripts/DropDownEvents.js",
                "~/Scripts/Export.js",
                "~/Scripts/FilterEvents.js",
                "~/Scripts/Focus.js",
                "~/Scripts/FocusEvents.js",
                "~/Scripts/Gantt.js",
                "~/Scripts/GanttEvents.js",
                "~/Scripts/GridEvents.js",
                "~/Scripts/HistoryEvents.js",
                "~/Scripts/Import.js",
                "~/Scripts/Item.js",
                "~/Scripts/JqueryUi.js",
                "~/Scripts/Kamban.js",
                "~/Scripts/KambanEvents.js",
                "~/Scripts/KeyEvents.js",
                "~/Scripts/LegendEvents.js",
                "~/Scripts/MarkDown.js",
                "~/Scripts/MarkDownEvents.js",
                "~/Scripts/Move.js",
                "~/Scripts/MultiSelect.js",
                "~/Scripts/Navigation.js",
                "~/Scripts/NavigationEvents.js",
                "~/Scripts/OutgoingMail.js",
                "~/Scripts/OutgoingMailEvents.js",
                "~/Scripts/Position.js",
                "~/Scripts/PositionEvents.js",
                "~/Scripts/ScrollEvents.js",
                "~/Scripts/SearchEvents.js",
                "~/Scripts/Security.js",
                "~/Scripts/Selectable.js",
                "~/Scripts/Separate.js",
                "~/Scripts/SiteInfo.js",
                "~/Scripts/SiteMenuEvents.js",
                "~/Scripts/SiteSettings.js",
                "~/Scripts/SiteSettingsEvents.js",
                "~/Scripts/StatusEvents.js",
                "~/Scripts/SubmitEvents.js",
                "~/Scripts/TimeSeries.js",
                "~/Scripts/_Show.js",
                "~/Scripts/Validators/_Shared.js",
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
