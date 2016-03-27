using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Views
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
                        .NearDeadline(siteSettings: siteSettings, formData: formData)
                        .Delay(siteSettings: siteSettings, formData: formData)
                        .Limit(siteSettings: siteSettings, formData: formData)
                        .Choices(siteSettings: siteSettings, formData: formData)));
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
            return Def.ExistsTable(siteSettings.ReferenceType, o => o.TypeCs == "CompletionTime")
                ? hb.FieldCheckBox(
                    controlId: "DataViewFilters_Incomplete",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Incomplete(),
                    _checked: formData.Checked("DataViewFilters_Incomplete"),
                    action: "DataView",
                    method: "post",
                    labelPositionIsRight: true)
                : hb;
        }

        private static HtmlBuilder Own(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return Def.ExistsTable(siteSettings.ReferenceType, o => o.Own)
                ? hb.FieldCheckBox(
                    controlId: "DataViewFilters_Own",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Own(),
                    _checked: formData.Checked("DataViewFilters_Own"),
                    action: "DataView",
                    method: "post",
                    labelPositionIsRight: true)
                : hb;
        }

        private static HtmlBuilder NearDeadline(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return Def.ExistsTable(siteSettings.ReferenceType, o => o.TypeCs == "CompletionTime")
                ? hb.FieldCheckBox(
                    controlId: "DataViewFilters_NearDeadline",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.NearDeadline(),
                    _checked: formData.Checked("DataViewFilters_NearDeadline"),
                    action: "DataView",
                    method: "post",
                    labelPositionIsRight: true)
                : hb;
        }

        private static HtmlBuilder Delay(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return Def.ExistsTable(siteSettings.ReferenceType, o => o.TypeCs == "ProgressRate")
                ? hb.FieldCheckBox(
                    controlId: "DataViewFilters_Delay",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Delay(),
                    _checked: formData.Checked("DataViewFilters_Delay"),
                    action: "DataView",
                    method: "post",
                    labelPositionIsRight: true)
                : hb;
        }

        private static HtmlBuilder Limit(
            this HtmlBuilder hb, SiteSettings siteSettings, FormData formData)
        {
            return Def.ExistsTable(siteSettings.ReferenceType, o => o.TypeCs == "CompletionTime")
                ? hb.FieldCheckBox(
                    controlId: "DataViewFilters_Overdue",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Overdue(),
                    _checked: formData.Checked("DataViewFilters_Overdue"),
                    action: "DataView",
                    method: "post",
                    labelPositionIsRight: true)
                : hb;
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
                            addBlank: true),
                        selectedValue: formData.Get("DataViewFilters_" + column.Id),
                        addSelectedValue: false,
                        action: "DataView",
                        method: "post");
                });
            return hb;
        }
    }
}