using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlDataViewFilters
    {
        public static ResponseCollection DataViewFilters(
            this ResponseCollection responseCollection, SiteSettings siteSettings)
        {
            return
                Forms.ControlId() == "ReduceDataViewFilters" ||
                Forms.ControlId() == "ExpandDataViewFilters"
                    ? responseCollection.ReplaceAll(
                        "#DataViewFilters", new HtmlBuilder().DataViewFilters(siteSettings))
                    : responseCollection;
        }

        public static HtmlBuilder DataViewFilters(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            var formData = Requests.DataViewFilters.SessionFormData(siteSettings.SiteId);
            return !Reduced(siteSettings.SiteId)
                ? hb.Div(
                    id: "DataViewFilters",
                    action: () => hb
                        .DisplayControl(
                            id: "ReduceDataViewFilters",
                            icon: "ui-icon-close")
                        .Reset()
                        .Incomplete(siteSettings: siteSettings, formData: formData)
                        .Own(siteSettings: siteSettings, formData: formData)
                        .NearCompletionTime(siteSettings: siteSettings, formData: formData)
                        .Delay(siteSettings: siteSettings, formData: formData)
                        .Limit(siteSettings: siteSettings, formData: formData)
                        .Columns(siteSettings: siteSettings, formData: formData)
                        .Search(siteSettings: siteSettings, formData: formData))
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
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_Incomplete",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Incomplete(),
                _checked: formData.Checked("DataViewFilters_Incomplete"),
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(siteSettings, "Status"));
        }

        private static HtmlBuilder Own(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_Own",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Own(),
                _checked: formData.Checked("DataViewFilters_Own"),
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(siteSettings, "Owner"));
        }

        private static HtmlBuilder NearCompletionTime(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_NearCompletionTime",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.NearCompletionTime(),
                _checked: formData.Checked("DataViewFilters_NearCompletionTime"),
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(siteSettings, "CompletionTime"));
        }

        private static HtmlBuilder Delay(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_Delay",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Delay(),
                _checked: formData.Checked("DataViewFilters_Delay"),
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(siteSettings, "ProgressRate"));
        }

        private static HtmlBuilder Limit(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_Overdue",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Overdue(),
                _checked: formData.Checked("DataViewFilters_Overdue"),
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(siteSettings, "CompletionTime"));
        }

        private static bool Visible(SiteSettings siteSettings, string columnName)
        {
            return
                siteSettings.GridColumnsOrder.Contains(columnName) ||
                siteSettings.EditorColumnsOrder.Contains(columnName);
        }

        private static HtmlBuilder Columns(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            siteSettings.FilterColumnCollection().ForEach(column =>
            {
                switch (column.TypeName.CsTypeSummary())
                {
                    case Types.CsBool:
                        hb.CheckBox(
                            column: column,
                            siteSettings: siteSettings,
                            formData: formData);
                        break;
                    case Types.CsDateTime:
                        var timePeriod = TimePeriod(column.RecordedTime);
                        hb.DropDown(
                            siteSettings: siteSettings,
                            column: column,
                            formData: formData,
                            optionCollection: timePeriod);
                        break;
                    case Types.CsNumeric:
                    case Types.CsString:
                        if (column.HasChoices())
                        {
                            hb.DropDown(
                            siteSettings: siteSettings,
                                column: column,
                                formData: formData,
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
            this HtmlBuilder hb, SiteSettings siteSettings, Column column, FormData formData)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_" + column.Id,
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Get(column.LabelText),
                _checked: formData.Get("DataViewFilters_" + column.Id).ToBool(),
                method: "post",
                _using:
                    siteSettings.GridColumnsOrder.Contains(column.ColumnName) ||
                    siteSettings.EditorColumnsOrder.Contains(column.ColumnName));
        }

        private static HtmlBuilder DropDown(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Column column,
            FormData formData,
            Dictionary<string, ControlData> optionCollection)
        {
            return hb.FieldDropDown(
                controlId: "DataViewFilters_" + column.Id,
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Get(column.LabelText),
                optionCollection: optionCollection,
                selectedValue: formData.Get("DataViewFilters_" + column.Id),
                multiple: true,
                addSelectedValue: false,
                method: "post",
                _using:
                    siteSettings.GridColumnsOrder.Contains(column.ColumnName) ||
                    siteSettings.EditorColumnsOrder.Contains(column.ColumnName) ||
                    column.RecordedTime);
        }

        private static HtmlBuilder Search(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return hb.FieldTextBox(
                controlId: "DataViewFilters_Search",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Search(),
                text: formData.Get("DataViewFilters_Search"),
                method: "post",
                _using: Routes.Controller().ToLower() == "items");
        }

        private static Dictionary<string, ControlData> TimePeriod(bool recordedTime)
        {
            var hash = new Dictionary<string, ControlData>();
            var min = Min();
            var max = Max();
            for (var m = min; m <= max; m += 12)
            {
                SetFy(hash, DateTime.Now.AddMonths(m), recordedTime);
            }
            for (var m = min; m <= max; m += 6)
            {
                SetHalf(hash, DateTime.Now.AddMonths(m), recordedTime);
            }
            for (var m = min; m <= max; m += 3)
            {
                SetQuarter(hash, DateTime.Now.AddMonths(m), recordedTime);
            }
            for (var m = min; m <= max; m++)
            {
                SetMonth(hash, DateTime.Now.AddMonths(m), recordedTime);
            }
            return hash;
        }

        private static int Min()
        {
            return (DateTime.Now.AddYears(Parameters.General.FilterMinSpan).FyFrom() -
                DateTime.Now).Months();
        }

        private static int Max()
        {
            return (DateTime.Now.AddYears(Parameters.General.FilterMaxSpan + 1).FyFrom() -
                DateTime.Now).Months();
        }

        private static void SetMonth(
            Dictionary<string, ControlData> hash, DateTime current, bool recordedTime)
        {
            var timePeriod = new TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Month, current);
            if (!recordedTime || timePeriod.From <= DateTime.Now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData(current.ToString("y", Sessions.CultureInfo()) +
                        InRange(timePeriod)));
            }
        }

        private static void SetQuarter(
            Dictionary<string, ControlData> hash, DateTime current, bool recordedTime)
        {
            var timePeriod = new TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Quarter, current);
            if (!recordedTime || timePeriod.From <= DateTime.Now)
            {
                hash.Add(
                   Period(timePeriod),
                    new ControlData(Displays.Quarter(
                        current.Fy().ToString(), current.Quarter().ToString()) +
                            InRange(timePeriod)));
            }
        }

        private static void SetHalf(
            Dictionary<string, ControlData> hash, DateTime current, bool recordedTime)
        {
            var timePeriod = new TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Half, current);
            if (!recordedTime || timePeriod.From <= DateTime.Now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData((current.Half() == 1
                        ? Displays.Half1(current.Fy().ToString())
                        : Displays.Half2(current.Fy().ToString())) +
                            InRange(timePeriod)));
            }
        }

        private static void SetFy(
            Dictionary<string, ControlData> hash, DateTime current, bool recordedTime)
        {
            var timePeriod = new TimePeriod(Implem.Libraries.Classes.TimePeriod.Types.Fy, current);
            if (!recordedTime || timePeriod.From <= DateTime.Now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData(Displays.Fy(current.Fy().ToString()) + InRange(timePeriod)));
            }
        }

        private static string Period(TimePeriod timePeriod)
        {
            return
                timePeriod.From.ToString() + "," +
                timePeriod.To.ToString("yyyy/M/d H:m:s.fff");
        }

        private static string InRange(TimePeriod timePeriod)
        {
            return timePeriod.InRange(DateTime.Now)
                ? " *"
                : string.Empty;
        }
    }
}