using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlDataViewFilters
    {
        public static HtmlBuilder DataViewFilters(
            this HtmlBuilder hb, SiteSettings ss, DataView dataView)
        {
            return !Reduced(ss.SiteId)
                ? hb.Div(
                    id: "DataViewFilters",
                    action: () => hb
                        .DisplayControl(
                            id: "ReduceDataViewFilters",
                            icon: "ui-icon-close")
                        .Reset()
                        .Incomplete(ss: ss, dataView: dataView)
                        .Own(ss: ss, dataView: dataView)
                        .NearCompletionTime(ss: ss, dataView: dataView)
                        .Delay(ss: ss, dataView: dataView)
                        .Limit(ss: ss, dataView: dataView)
                        .Columns(ss: ss, dataView: dataView)
                        .Search(ss: ss, dataView: dataView))
                : hb.Div(
                    id: "DataViewFilters",
                    css: "reduced",
                    action: () => hb
                        .DisplayControl(
                            id: "ExpandDataViewFilters",
                            icon: "ui-icon-folder-open"));
        }

        private static bool Reduced(long? siteId)
        {
            var key = "ReduceDataViewFilters_" + (siteId != null
                ? Pages.Key()
                : siteId.ToString());
            if (Forms.ControlId() == "ReduceDataViewFilters")
            {
                HttpContext.Current.Session[key] = true;
            }
            else if (Forms.ControlId() == "ExpandDataViewFilters")
            {
                HttpContext.Current.Session.Remove(key);
            }
            return HttpContext.Current.Session[key].ToBool();
        }

        private static HtmlBuilder DisplayControl(this HtmlBuilder hb, string id, string icon)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id(id)
                    .Class("display-control")
                    .OnClick("$p.send($(this));")
                    .DataMethod("post"),
                action: () => hb
                    .Span(css: "ui-icon " + icon)
                    .Text(text: Displays.Filters() + ":"));
        }

        private static HtmlBuilder Reset(this HtmlBuilder hb)
        {
            return hb.Button(
                controlId: "DataViewFilters_Reset",
                text: Displays.Reset(),
                controlCss: "button-icon",
                icon: "ui-icon-close",
                method: "post");
        }

        private static HtmlBuilder Incomplete(
            this HtmlBuilder hb, SiteSettings ss, DataView dataView)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_Incomplete",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Incomplete(),
                _checked: dataView.Incomplete == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "Status"));
        }

        private static HtmlBuilder Own(
            this HtmlBuilder hb, SiteSettings ss, DataView dataView)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_Own",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Own(),
                _checked: dataView.Own == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "Owner"));
        }

        private static HtmlBuilder NearCompletionTime(
            this HtmlBuilder hb, SiteSettings ss, DataView dataView)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_NearCompletionTime",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.NearCompletionTime(),
                _checked: dataView.NearCompletionTime == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "CompletionTime"));
        }

        private static HtmlBuilder Delay(
            this HtmlBuilder hb, SiteSettings ss, DataView dataView)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_Delay",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Delay(),
                _checked: dataView.Delay == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "ProgressRate"));
        }

        private static HtmlBuilder Limit(
            this HtmlBuilder hb, SiteSettings ss, DataView dataView)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_Overdue",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Overdue(),
                _checked: dataView.Overdue == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "CompletionTime"));
        }

        private static bool Visible(SiteSettings ss, string columnName)
        {
            return
                ss.GridColumns.Contains(columnName) ||
                ss.EditorColumns.Contains(columnName);
        }

        private static HtmlBuilder Columns(
            this HtmlBuilder hb, SiteSettings ss, DataView dataView)
        {
            ss.FilterColumnCollection().ForEach(column =>
            {
                switch (column.TypeName.CsTypeSummary())
                {
                    case Types.CsBool:
                        hb.CheckBox(
                            column: column,
                            ss: ss,
                            dataView: dataView);
                        break;
                    case Types.CsDateTime:
                        var timePeriod = TimePeriod.Get(column.RecordedTime);
                        hb.DropDown(
                            ss: ss,
                            column: column,
                            dataView: dataView,
                            optionCollection: timePeriod);
                        break;
                    case Types.CsNumeric:
                    case Types.CsString:
                        if (column.HasChoices())
                        {
                            hb.DropDown(
                                ss: ss,
                                column: column,
                                dataView: dataView,
                                optionCollection: column.EditChoices(addNotSet: true));
                        }
                        break;
                    default:
                        break;
                }
            });
            return hb;
        }

        private static HtmlBuilder CheckBox(
            this HtmlBuilder hb, SiteSettings ss, Column column, DataView dataView)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_" + column.Id,
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Get(column.GridLabelText),
                _checked: dataView.ColumnFilter(column.ColumnName).ToBool(),
                method: "post",
                _using:
                    ss.GridColumns.Contains(column.ColumnName) ||
                    ss.EditorColumns.Contains(column.ColumnName));
        }

        private static HtmlBuilder DropDown(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column column,
            DataView dataView,
            Dictionary<string, ControlData> optionCollection)
        {
            return hb.FieldDropDown(
                controlId: "DataViewFilters_" + column.Id,
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Get(column.GridLabelText),
                optionCollection: optionCollection,
                selectedValue: dataView.ColumnFilter(column.ColumnName),
                multiple: true,
                addSelectedValue: false,
                method: "post",
                _using:
                    ss.GridColumns.Contains(column.ColumnName) ||
                    ss.EditorColumns.Contains(column.ColumnName) ||
                    column.RecordedTime);
        }

        private static HtmlBuilder Search(
            this HtmlBuilder hb, SiteSettings ss, DataView dataView)
        {
            return hb.FieldTextBox(
                controlId: "DataViewFilters_Search",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Search(),
                text: dataView.Search,
                method: "post",
                _using: Routes.Controller().ToLower() == "items");
        }
    }
}