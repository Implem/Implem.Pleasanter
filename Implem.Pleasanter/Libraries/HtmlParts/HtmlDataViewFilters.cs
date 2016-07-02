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
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlDataViewFilters
    {
        public static HtmlBuilder DataViewFilters(
            this HtmlBuilder hb, SiteSettings siteSettings, long? siteId = null)
        {
            var formData = Requests.DataViewFilters.SessionFormData(siteId);
            return hb.FieldSet(
                css: " enclosed-thin dataview-filters",
                legendText: Displays.Filters(),
                action: () => hb
                    .Div(action: () => hb
                        .Reset()
                        .Incomplete(siteSettings: siteSettings, formData: formData)
                        .Own(siteSettings: siteSettings, formData: formData)
                        .NearCompletionTime(siteSettings: siteSettings, formData: formData)
                        .Delay(siteSettings: siteSettings, formData: formData)
                        .Limit(siteSettings: siteSettings, formData: formData)
                        .Columns(siteSettings: siteSettings, formData: formData)
                        .Search(siteSettings: siteSettings, formData: formData)));
        }

        private static HtmlBuilder Reset(this HtmlBuilder hb)
        {
            return hb.Button(
                controlId: "DataViewFilters_Reset",
                text: Displays.Reset(),
                controlCss: "button-reset",
                action: "DataView",
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
                action: "DataView",
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
                action: "DataView",
                method: "post",
                labelPositionIsRight: true);
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
                action: "DataView",
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
                action: "DataView",
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
                action: "DataView",
                method: "post",
                labelPositionIsRight: true,
                _using: Visible(siteSettings, "CompletionTime"));
        }

        private static bool Visible(SiteSettings siteSettings, string columnName)
        {
            var column = siteSettings.AllColumn(columnName);
            return column != null &&
                (column.GridVisible.ToBool() || column.EditorVisible.ToBool());
        }

        private static HtmlBuilder Columns(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            siteSettings.FilterColumnCollection().ForEach(column =>
            {
                switch (column.TypeName.CsTypeSummary())
                {
                    case Types.CsBool:
                        hb.CheckBox(column: column, formData: formData);
                        break;
                    case Types.CsDateTime:
                        var timePeriod = TimePeriod(column.RecordedTime);
                        hb.DropDown(
                            column: column,
                            formData: formData,
                            optionCollection: timePeriod);
                        break;
                    case Types.CsNumeric:
                    case Types.CsString:
                        if (column.HasChoices())
                        {
                            hb.DropDown(
                                column: column,
                                formData: formData,
                                optionCollection: column.EditChoices(
                                    siteId: siteSettings.InheritPermission,
                                    addBlank: true,
                                    addNotSet: true));
                        }
                        break;
                    default:
                        break;
                }
            });
            return hb;
        }

        private static HtmlBuilder CheckBox(
            this HtmlBuilder hb, Column column, FormData formData)
        {
            return hb.FieldCheckBox(
                controlId: "DataViewFilters_" + column.Id,
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.Get(column.LabelText),
                _checked: formData.Get("DataViewFilters_" + column.Id).ToBool(),
                action: "DataView",
                method: "post",
                _using: column.GridVisible.ToBool() || column.EditorVisible.ToBool());
        }

        private static HtmlBuilder DropDown(
            this HtmlBuilder hb,
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
                addSelectedValue: false,
                action: "DataView",
                method: "post",
                _using:
                    column.GridVisible.ToBool() ||
                    column.EditorVisible.ToBool() ||
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
                action: "DataView",
                method: "post");
        }

        private static Dictionary<string, ControlData> TimePeriod(bool recordedTime)
        {
            var hash = new Dictionary<string, ControlData>
            {
                { string.Empty, new ControlData(string.Empty) }
            };
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