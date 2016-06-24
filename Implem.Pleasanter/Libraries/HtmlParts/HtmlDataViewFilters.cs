using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
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
                        .Checks(siteSettings: siteSettings, formData: formData)
                        .Choices(siteSettings: siteSettings, formData: formData)
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

        private static HtmlBuilder Checks(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            siteSettings.ColumnCollection
                .Where(o => o.TypeName == "bit")
                .Where(o => o.EditorVisible.ToBool())
                .ForEach(column =>
                {
                    hb.FieldCheckBox(
                        controlId: "DataViewFilters_" + column.Id,
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.Get(column.LabelText),
                        _checked: formData.Get("DataViewFilters_" + column.Id).ToBool(),
                        action: "DataView",
                        method: "post",
                        _using: column.GridVisible.ToBool() || column.EditorVisible.ToBool());
                });
            return hb;
        }

        private static HtmlBuilder Choices(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            siteSettings.ColumnCollection
                .Where(o => o.HasChoices())
                .ForEach(column =>
                {
                    hb.FieldDropDown(
                        controlId: "DataViewFilters_" + column.Id,
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.Get(column.LabelText),
                        optionCollection: column.EditChoices(
                            siteId: siteSettings.InheritPermission,
                            addBlank: true,
                            addNotSet: true),
                        selectedValue: formData.Get("DataViewFilters_" + column.Id),
                        addSelectedValue: false,
                        action: "DataView",
                        method: "post",
                        _using: column.GridVisible.ToBool() || column.EditorVisible.ToBool());
                });
            return hb;
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
    }
}