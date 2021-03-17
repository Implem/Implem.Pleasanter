﻿using Implem.Libraries.Utilities;
using System.Collections.Generic;
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
                "~/Scripts/_Api.js",
                "~/Scripts/_Data.js",
                "~/Scripts/_Dispatch.js",
                "~/Scripts/_Elements.js",
                "~/Scripts/_Event.js",
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
                "~/Scripts/BulkUpdate.js",
                "~/Scripts/BurnDown.js",
                "~/Scripts/BurnDownEvents.js",
                "~/Scripts/Calendar.js",
                "~/Scripts/CalendarEvents.js",
                "~/Scripts/Clipboard.js",
                "~/Scripts/ColumnAccessControl.js",
                "~/Scripts/Confirm.js",
                "~/Scripts/ConfirmEvents.js",
                "~/Scripts/Crosstab.js",
                "~/Scripts/Dialog.js",
                "~/Scripts/DialogEvents.js",
                "~/Scripts/Display.js",
                "~/Scripts/DropDownSearch.js",
                "~/Scripts/DropDownSearchEvents.js",
                "~/Scripts/Expand.js",
                "~/Scripts/Export.js",
                "~/Scripts/FilterEvents.js",
                "~/Scripts/Focus.js",
                "~/Scripts/FocusEvents.js",
                "~/Scripts/Gantt.js",
                "~/Scripts/GanttEvents.js",
                "~/Scripts/Grid.js",
                "~/Scripts/GridEvents.js",
                "~/Scripts/Group.js",
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
                "~/Scripts/LockEvents.js",
                "~/Scripts/Log.js",
                "~/Scripts/MarkDown.js",
                "~/Scripts/MarkDownEvents.js",
                "~/Scripts/MenuEvents.js",
                "~/Scripts/Message.js",
                "~/Scripts/MessageEvents.js",
                "~/Scripts/Move.js",
                "~/Scripts/MultiSelect.js",
                "~/Scripts/Navigation.js",
                "~/Scripts/NavigationEvents.js",
                "~/Scripts/OutgoingMail.js",
                "~/Scripts/OutgoingMailEvents.js",
                "~/Scripts/Permission.js",
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
                "~/Scripts/SitePackage.js",
                "~/Scripts/SiteSettings.js",
                "~/Scripts/SiteSettingsEvents.js",
                "~/Scripts/StartGuide.js",
                "~/Scripts/StatusEvents.js",
                "~/Scripts/SubmitEvents.js",
                "~/Scripts/Template.js",
                "~/Scripts/TimeSeries.js",
                "~/Scripts/Users.js",
                "~/Scripts/Validator.js",
                "~/Scripts/Video.js",
                "~/Scripts/ViewMode.js",
                "~/Scripts/Visibility.js",
                "~/Scripts/_Show.js",
                "~/Scripts/RelatingColumns.js",
                "~/Scripts/FieldSelectable.js",
                "~/Scripts/Tenants.js",
                "~/Scripts/SetDateRangeDialog.js",
                "~/Scripts/setNumericRangeDialog.js"
            };
        }

        private static void AddStyles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/content/styles")
                .Include("~/Styles/Site.css"));
        }
    }
}
