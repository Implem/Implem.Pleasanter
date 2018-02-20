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
                "~/Scripts/_Data.js",
                "~/Scripts/_Dispatch.js",
                "~/Scripts/_Form.js",
                "~/Scripts/_Time.js",
                "~/Scripts/_View.js",
                "~/Scripts/_ControllEvents.js",
                "~/Scripts/AggregationEvents.js",
                "~/Scripts/AnchorEvents.js",
                "~/Scripts/Attachments.js",
                "~/Scripts/AttachmentsEvents.js",
                "~/Scripts/Basket.js",
                "~/Scripts/BasketEvents.js",
                "~/Scripts/BurnDown.js",
                "~/Scripts/BurnDownEvents.js",
                "~/Scripts/Calendar.js",
                "~/Scripts/CalendarEvents.js",
                "~/Scripts/Confirm.js",
                "~/Scripts/ConfirmEvents.js",
                "~/Scripts/Crosstab.js",
                "~/Scripts/Dialog.js",
                "~/Scripts/DialogEvents.js",
                "~/Scripts/Display.js",
                "~/Scripts/DropDownSearch.js",
                "~/Scripts/DropDownSearchEvents.js",
                "~/Scripts/Export.js",
                "~/Scripts/FilterEvents.js",
                "~/Scripts/Focus.js",
                "~/Scripts/FocusEvents.js",
                "~/Scripts/Gantt.js",
                "~/Scripts/GanttEvents.js",
                "~/Scripts/GridEvents.js",
                "~/Scripts/Group.js",
                "~/Scripts/HistoryEvents.js",
                "~/Scripts/ImageLib.js",
                "~/Scripts/ImageLibEvents.js",
                "~/Scripts/Import.js",
                "~/Scripts/Item.js",
                "~/Scripts/JqueryUi.js",
                "~/Scripts/Kamban.js",
                "~/Scripts/KambanEvents.js",
                "~/Scripts/KeyEvents.js",
                "~/Scripts/LegendEvents.js",
                "~/Scripts/Loading.js",
                "~/Scripts/MarkDown.js",
                "~/Scripts/MarkDownEvents.js",
                "~/Scripts/MenuEvents.js",
                "~/Scripts/Move.js",
                "~/Scripts/MultiSelect.js",
                "~/Scripts/Navigation.js",
                "~/Scripts/NavigationEvents.js",
                "~/Scripts/OutgoingMail.js",
                "~/Scripts/OutgoingMailEvents.js",
                "~/Scripts/Permissions.js",
                "~/Scripts/Position.js",
                "~/Scripts/PositionEvents.js",
                "~/Scripts/Scroll.js",
                "~/Scripts/ScrollEvents.js",
                "~/Scripts/SearchEvents.js",
                "~/Scripts/Security.js",
                "~/Scripts/Selectable.js",
                "~/Scripts/Separate.js",
                "~/Scripts/Site.js",
                "~/Scripts/SiteInfo.js",
                "~/Scripts/SiteMenu.js",
                "~/Scripts/SiteSettings.js",
                "~/Scripts/SiteSettingsEvents.js",
                "~/Scripts/StatusEvents.js",
                "~/Scripts/SubmitEvents.js",
                "~/Scripts/Template.js",
                "~/Scripts/TimeSeries.js",
                "~/Scripts/Validator.js",
                "~/Scripts/Video.js",
                "~/Scripts/ViewMode.js",
                "~/Scripts/_Show.js"
            };
        }

        private static void AddStyles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/content/styles")
                .Include("~/Styles/Site.css"));
        }
    }
}
