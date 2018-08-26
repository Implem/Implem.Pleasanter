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
    public static class HtmlViewFilters
    {
        public static HtmlBuilder ViewFilters(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view)
        {
            return ss.ReferenceType != "Sites"
                ? !Reduced(ss.SiteId)
                    ? hb.Div(
                        id: "ViewFilters",
                        action: () => hb
                            .DisplayControl(
                                id: "ReduceViewFilters",
                                icon: "ui-icon-close")
                            .Reset()
                            .Incomplete(ss: ss, view: view)
                            .Own(ss: ss, view: view)
                            .NearCompletionTime(ss: ss, view: view)
                            .Delay(ss: ss, view: view)
                            .Limit(ss: ss, view: view)
                            .Columns(context: context, ss: ss, view: view)
                            .Search(view: view))
                    : hb.Div(
                        id: "ViewFilters",
                        css: "reduced",
                        action: () => hb
                            .DisplayControl(
                                id: "ExpandViewFilters",
                                icon: "ui-icon-folder-open"))
                : hb;
        }

        private static bool Reduced(long? siteId)
        {
            var key = "ReduceViewFilters_" + (siteId != null
                ? Pages.Key()
                : siteId.ToString());
            if (Forms.ControlId() == "ReduceViewFilters")
            {
                HttpContext.Current.Session[key] = true;
            }
            else if (Forms.ControlId() == "ExpandViewFilters")
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
                controlId: "ViewFilters_Reset",
                text: Displays.Reset(),
                controlCss: "button-icon",
                icon: "ui-icon-close",
                method: "post");
        }

        private static HtmlBuilder Incomplete(this HtmlBuilder hb, SiteSettings ss, View view)
        {
            return hb.FieldCheckBox(
                controlId: "ViewFilters_Incomplete",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Incomplete(),
                _checked: view.Incomplete == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "Status"));
        }

        private static HtmlBuilder Own(this HtmlBuilder hb, SiteSettings ss, View view)
        {
            return hb.FieldCheckBox(
                controlId: "ViewFilters_Own",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Own(),
                _checked: view.Own == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "Manager") || Visible(ss, "Owner"));
        }

        private static HtmlBuilder NearCompletionTime(
            this HtmlBuilder hb, SiteSettings ss, View view)
        {
            return hb.FieldCheckBox(
                controlId: "ViewFilters_NearCompletionTime",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.NearCompletionTime(),
                _checked: view.NearCompletionTime == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "CompletionTime"));
        }

        private static HtmlBuilder Delay(
            this HtmlBuilder hb, SiteSettings ss, View view)
        {
            return hb.FieldCheckBox(
                controlId: "ViewFilters_Delay",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Delay(),
                _checked: view.Delay == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "ProgressRate"));
        }

        private static HtmlBuilder Limit(this HtmlBuilder hb, SiteSettings ss, View view)
        {
            return hb.FieldCheckBox(
                controlId: "ViewFilters_Overdue",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Overdue(),
                _checked: view.Overdue == true,
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(ss, "CompletionTime"));
        }

        private static bool Visible(Column column)
        {
            return Visible(column.SiteSettings, column.Name);
        }

        private static bool Visible(SiteSettings ss, string columnName)
        {
            return
                ss.GridColumns.Contains(columnName) ||
                ss.EditorColumns.Contains(columnName);
        }

        private static HtmlBuilder Columns(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view)
        {
            ss.GetFilterColumns(context: context, checkPermission: true).ForEach(column =>
            {
                switch (column.TypeName.CsTypeSummary())
                {
                    case Types.CsBool:
                        hb.CheckBox(
                            context: context,
                            ss: ss,
                            column: column,
                            view: view);
                        break;
                    case Types.CsNumeric:
                        hb.DropDown(
                            context: context,
                            ss: ss,
                            column: column,
                            view: view,
                            optionCollection: column.HasChoices()
                                ? column.EditChoices(addNotSet: true)
                                : column.NumFilterOptions());
                        break;
                    case Types.CsDateTime:
                        hb.DropDown(
                            context: context,
                            ss: ss,
                            column: column,
                            view: view,
                            optionCollection: column.DateFilterOptions());
                        break;
                    case Types.CsString:
                        if (column.HasChoices())
                        {
                            var currentSs = column.SiteSettings;
                            if (view.ColumnFilterHash?.ContainsKey(column.ColumnName) == true &&
                                column.UseSearch == true &&
                                currentSs.Links?.Any(o =>
                                    o.ColumnName == column.ColumnName) == true)
                            {
                                currentSs.SetChoiceHash(
                                    context: context,
                                    columnName: column?.ColumnName,
                                    selectedValues: view.ColumnFilter(column.ColumnName)
                                        .Deserialize<List<string>>());
                            }
                            hb.DropDown(
                                context: context,
                                ss: ss,
                                column: column,
                                view: view,
                                optionCollection: column.EditChoices(addNotSet: true));
                        }
                        else if (Visible(column))
                        {
                            hb.FieldTextBox(
                                controlId: "ViewFilters__" + column.ColumnName,
                                fieldCss: "field-auto-thin",
                                controlCss: " auto-postback",
                                labelText: Displays.Get(column.GridLabelText),
                                labelTitle: ss.LabelTitle(column),
                                text: view.ColumnFilter(column.ColumnName),
                                method: "post");
                        }
                        break;
                    default:
                        break;
                }
            });
            return hb;
        }

        private static HtmlBuilder CheckBox(
            this HtmlBuilder hb, Context context, SiteSettings ss, Column column, View view)
        {
            var currentSs = column.SiteSettings;
            if (currentSs.GridColumns.Contains(column.ColumnName) ||
                currentSs.EditorColumns.Contains(column.ColumnName))
            {
                switch (column.CheckFilterControlType)
                {
                    case ColumnUtilities.CheckFilterControlTypes.OnOnly:
                        return hb.FieldCheckBox(
                            controlId: "ViewFilters__" + column.ColumnName,
                            fieldCss: "field-auto-thin",
                            controlCss: " auto-postback",
                            labelText: Displays.Get(column.GridLabelText),
                            labelTitle: ss.LabelTitle(column),
                            _checked: view.ColumnFilter(column.ColumnName).ToBool(),
                            method: "post");
                    case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                        return hb.FieldDropDown(
                            context: context,
                            controlId: "ViewFilters__" + column.ColumnName,
                            fieldCss: "field-auto-thin",
                            controlCss: " auto-postback",
                            labelText: Displays.Get(column.GridLabelText),
                            labelTitle: ss.LabelTitle(column),
                            optionCollection: ColumnUtilities.CheckFilterTypeOptions(),
                            selectedValue: view.ColumnFilter(column.ColumnName),
                            addSelectedValue: false,
                            insertBlank: true,
                            method: "post");
                }
            }
            return hb;
        }

        private static HtmlBuilder DropDown(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            View view,
            Dictionary<string, ControlData> optionCollection)
        {
            return hb.FieldDropDown(
                context: context,
                controlId: "ViewFilters__" + column.ColumnName,
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback" + (column.UseSearch == true
                    ? " search"
                    : string.Empty),
                labelText: Displays.Get(column.GridLabelText),
                labelTitle: ss.LabelTitle(column),
                optionCollection: optionCollection,
                selectedValue: view.ColumnFilter(column.ColumnName),
                multiple: true,
                addSelectedValue: false,
                method: "post",
                _using: Visible(column) || column.RecordedTime);
        }

        private static HtmlBuilder Search(this HtmlBuilder hb, View view)
        {
            return hb.FieldTextBox(
                controlId: "ViewFilters_Search",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Search(),
                text: view.Search,
                method: "post",
                _using: Routes.Controller() == "items");
        }
    }
}