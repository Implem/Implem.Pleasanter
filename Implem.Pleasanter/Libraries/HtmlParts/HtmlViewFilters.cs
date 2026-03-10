using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlViewFilters
    {
        public static HtmlBuilder ViewFilters(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view)
        {
            var process = ss.GetProcess(
                context: context,
                id: context.Forms.Int("BulkProcessingItems"));
            return ss.ReferenceType != "Sites"
                && ss.UseFiltersArea == true
                && view?.FiltersDisplayType != View.DisplayTypes.AlwaysHidden
                    ? view?.GetFiltersReduced() != true
                        ? hb.Div(
                            id: "ViewFilters",
                            action: () => hb
                                .ViewFiltersLabelMenus(
                                    context: context,
                                    ss: ss)
                                .DisplayControl(
                                    context: context,
                                    view: view,
                                    id: "ReduceViewFilters",
                                    icon: "ui-icon-close")
                                .Reset(context: context)
                                .Incomplete(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    disabled: process?.View?.Incomplete == true)
                                .Own(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    disabled: process?.View?.Own == true)
                                .NearCompletionTime(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    disabled: process?.View?.NearCompletionTime == true)
                                .Delay(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    disabled: process?.View?.Delay == true)
                                .Limit(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    disabled: process?.View?.Overdue == true)
                                .Columns(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    process: process)
                                .Search(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    disabled: process?.View?.Search?.Any() == true)
                                .FilterButton(
                                    context: context,
                                    ss: ss)
                                .Hidden(
                                    controlId: "TriggerRelatingColumns_Filter",
                                    value: Jsons.ToJson(ss?.RelatingColumns),
                                    _using: ss?.UseRelatingColumnsOnFilter == true))
                        : hb.Div(
                            id: "ViewFilters",
                            css: "reduced",
                            action: () => hb
                                .DisplayControl(
                                    context: context,
                                    view: view,
                                    id: "ExpandViewFilters",
                                    icon: "ui-icon-folder-open")
                                .Hidden(
                                    controlId: "TriggerRelatingColumns_Filter",
                                    value: Jsons.ToJson(ss?.RelatingColumns),
                                    _using: ss?.UseRelatingColumnsOnFilter == true))
                    : hb.Div(
                        id: "ViewFilters",
                        css: "always-hidden");
        }

        private static HtmlBuilder DisplayControl(
            this HtmlBuilder hb,
            Context context,
            string id,
            string icon,
            View view)
        {
            if (view?.FiltersDisplayType == View.DisplayTypes.AlwaysDisplayed)
            {
                return hb;
            }
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id(id)
                    .Class("display-control")
                    .OnClick("$p.send($(this));")
                    .DataMethod("post"),
                action: () => hb
                    .Span(css: "ui-icon " + icon)
                    .Text(text: Displays.Filters(context: context) + ":"));
        }

        private static HtmlBuilder Reset(this HtmlBuilder hb, Context context)
        {
            return hb.Button(
                controlId: "ViewFilters_Reset",
                text: Displays.Reset(context: context),
                controlCss: "button-icon",
                icon: "ui-icon-close",
                method: "post");
        }

        public static HtmlBuilder Incomplete(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool disabled = false)
        {
            var labelIcon = LabelIcon(
                ss: ss,
                view: view,
                name: "ViewFilters_Incomplete");
            return ss.UseIncompleteFilter == true
                ? hb
                    .FieldCheckBox(
                        fieldId: "ViewFilters_Incomplete" + "Field",
                        controlId: "ViewFilters_Incomplete",
                        fieldCss: "field-auto-thin",
                        controlCss: ss.UseFilterButton != true
                            ? " auto-postback"
                            : string.Empty,
                        labelText: Displays.Incomplete(context: context),
                        labelIcon: labelIcon,
                        _checked: view.Incomplete == true,
                        disabled: disabled,
                        method: "post",
                        labelPositionIsRight: true,
                        _using: view.HasIncompleteColumns(context: context, ss: ss)
                            && Visible(ss, "Status"))
                : hb;
        }

        public static HtmlBuilder Own(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool disabled = false)
        {
            var labelIcon = LabelIcon(
                ss: ss,
                view: view,
                name: "ViewFilters_Own");
            return ss.UseOwnFilter == true
                ? hb
                    .FieldCheckBox(
                        fieldId: "ViewFilters_Own" + "Field",
                        controlId: "ViewFilters_Own",
                        fieldCss: "field-auto-thin",
                        controlCss: ss.UseFilterButton != true
                            ? " auto-postback"
                            : string.Empty,
                        labelText: Displays.Own(context: context),
                        labelIcon: labelIcon,
                        _checked: view.Own == true,
                        disabled: disabled,
                        method: "post",
                        labelPositionIsRight: true,
                        _using: view.HasOwnColumns(context: context, ss: ss)
                            && (Visible(ss, "Manager") || Visible(ss, "Owner")))
                : hb;
        }

        public static HtmlBuilder NearCompletionTime(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool disabled = false)
        {
            var labelIcon = LabelIcon(
                ss: ss,
                view: view,
                name: "ViewFilters_NearCompletionTime");
            return ss.UseNearCompletionTimeFilter == true
                ? hb
                    .FieldCheckBox(
                        fieldId: "ViewFilters_NearCompletionTime" + "Field",
                        controlId: "ViewFilters_NearCompletionTime",
                        fieldCss: "field-auto-thin",
                        controlCss: ss.UseFilterButton != true
                            ? " auto-postback"
                            : string.Empty,
                        labelText: Displays.NearCompletionTime(context: context),
                        labelIcon: labelIcon,
                        _checked: view.NearCompletionTime == true,
                        disabled: disabled,
                        method: "post",
                        labelPositionIsRight: true,
                        _using: view.HasNearCompletionTimeColumns(context: context, ss: ss)
                            && Visible(ss, "CompletionTime"))
                : hb;
        }

        public static HtmlBuilder Delay(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool disabled = false)
        {
            var labelIcon = LabelIcon(
                ss: ss,
                view: view,
                name: "ViewFilters_Delay");
            return ss.UseDelayFilter == true
                ? hb
                    .FieldCheckBox(
                        fieldId: "ViewFilters_Delay" + "Field",
                        controlId: "ViewFilters_Delay",
                        fieldCss: "field-auto-thin",
                        controlCss: ss.UseFilterButton != true
                            ? " auto-postback"
                            : string.Empty,
                        labelText: Displays.Delay(context: context),
                        labelIcon: labelIcon,
                        _checked: view.Delay == true,
                        disabled: disabled,
                        method: "post",
                        labelPositionIsRight: true,
                        _using: view.HasDelayColumns(context: context, ss: ss)
                            && Visible(ss, "ProgressRate"))
                : hb;
        }

        public static HtmlBuilder Limit(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool disabled = false)
        {
            var labelIcon = LabelIcon(
                ss: ss,
                view: view,
                name: "ViewFilters_Overdue");
            return ss.UseOverdueFilter == true
                ? hb
                    .FieldCheckBox(
                        fieldId: "ViewFilters_Overdue" + "Field",
                        controlId: "ViewFilters_Overdue",
                        fieldCss: "field-auto-thin",
                        controlCss: ss.UseFilterButton != true
                            ? " auto-postback"
                            : string.Empty,
                        labelText: Displays.Overdue(context: context),
                        labelIcon: labelIcon,
                        _checked: view.Overdue == true,
                        disabled: disabled,
                        method: "post",
                        labelPositionIsRight: true,
                        _using: view.HasOverdueColumns(context: context, ss: ss)
                            && Visible(ss, "CompletionTime"))
                : hb;
        }

        private static bool Visible(SiteSettings ss, string columnName)
        {
            return
                ss.GridColumns.Contains(columnName)
                || ss.GetEditorColumnNames().Contains(columnName);
        }

        internal static string GetDisplayDateFilterRange(
            Context context, string value, bool timepicker)
        {
            if (value.IsNullOrEmpty()) return value;
            switch (value)
            {
                case "[\"Today\"]":
                    return Displays.Today(context: context);
                case "[\"ThisMonth\"]":
                    return Displays.ThisMonth(context: context);
                case "[\"ThisYear\"]":
                    return Displays.ThisYear(context: context);
                default:
                    var dts = value.Trim('[', '"', ']').Split(new[] { ',' });
                    var s = (dts.Length > 0) ? (timepicker ? dts[0] : dts[0].Split(' ').FirstOrDefault()) : "";
                    var e = (dts.Length > 1) ? (timepicker ? dts[1] : dts[1].Split(' ').FirstOrDefault()) : "";
                    return s + " - " + e;
            }
        }

        internal static string GetNumericFilterRange(string value)
        {
            if (value.IsNullOrEmpty()) return value;
            var dts = value.Trim('[', '"', ']').Split(new[] { ',' });
            var s = (dts.Length > 0) ? dts[0].Split(' ').FirstOrDefault() : "";
            var e = (dts.Length > 1) ? dts[1].Split(' ').FirstOrDefault() : "";
            return s + " - " + e;
        }

        private static HtmlBuilder Columns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Process process = null)
        {
            ss.GetFilterColumns(
                context: context,
                view: view,
                checkPermission: true).ForEach(column =>
                    Column(
                        hb: hb,
                        context: context,
                        ss: ss,
                        view: view,
                        column: column,
                        process: process));
            return hb;
        }

        internal static HtmlBuilder ViewFiltersColumnOnGrid(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Column column)
        {
            Column(
                hb: hb,
                context: context,
                ss: ss,
                view: view,
                column: column,
                onGridHeader: true);
            return hb;
        }

        public static void Column(
            HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Column column,
            bool onGridHeader = false,
            Process process = null)
        {
            var idPrefix = onGridHeader ? "ViewFiltersOnGridHeader__" : "ViewFilters__";
            var action = onGridHeader ? "GridRows" : null;
            var controlOnly = onGridHeader;
            var disabled = process != null
                && ((column.ColumnName == "Status" && process.CurrentStatus != -1)
                    || process.View?.ColumnFilterHash?.ContainsKey(column.ColumnName) == true);
            string labelIcon = LabelIcon(
                ss: ss,
                view: view,
                name: column.ColumnName);
            var labelText = Strings.CoalesceEmpty(
                ColumnUtilities.GetMultilingualLabelText(
                    target: column.MultilingualLabelText,
                    context: context),
                column.GridLabelText);
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsBool:
                    hb.CheckBox(
                        context: context,
                        ss: ss,
                        column: column,
                        view: view,
                        idPrefix: idPrefix,
                        action: action,
                        labelIcon: labelIcon,
                        controlOnly: controlOnly,
                        disabled: disabled);
                    break;
                case Types.CsNumeric:
                    if (column.Id_Ver)
                    {
                        if (column.DateFilterSetMode == ColumnUtilities.DateFilterSetMode.Default)
                        {
                            hb.FieldTextBox(
                                fieldId: idPrefix + column.ColumnName + "Field",
                                controlId: idPrefix + column.ColumnName,
                                fieldCss: "field-auto-thin",
                                controlCss: ss.UseFilterButton != true
                                    ? " auto-postback"
                                    : string.Empty,
                                labelText: labelText,
                                labelTitle: ss.LabelTitle(column),
                                labelIcon: labelIcon,
                                controlOnly: controlOnly,
                                disabled: disabled,
                                method: "post");
                        }
                        else
                        {
                            SetNumericRangeDialog(
                                hb: hb,
                                context: context,
                                ss: ss,
                                view: view,
                                column: column,
                                idPrefix: idPrefix,
                                action: action,
                                labelIcon: labelIcon,
                                controlOnly: controlOnly,
                                disabled: disabled);
                        }
                    }
                    else if (column.DateFilterSetMode == ColumnUtilities.DateFilterSetMode.Default)
                    {
                        hb.DropDown(
                            context: context,
                            ss: ss,
                            column: column,
                            view: view,
                            optionCollection: column.HasChoices()
                                ? column.UseSearch == true
                                    ? []
                                    : column.EditChoices(
                                        context: context,
                                        addNotSet: true,
                                        own: true,
                                        limit: Parameters.General.DropDownSearchPageSize)
                                : column.NumFilterOptions(context: context),
                            idPrefix: idPrefix,
                            controlOnly: controlOnly,
                            action: action,
                            labelIcon: labelIcon,
                            useFilterButton: ss.UseFilterButton,
                            disabled: disabled);
                    }
                    else
                    {
                        SetNumericRangeDialog(
                            hb: hb,
                            context: context,
                            ss: ss,
                            view: view,
                            column: column,
                            idPrefix: idPrefix,
                            action: action,
                            labelIcon: labelIcon,
                            controlOnly: controlOnly,
                            disabled: disabled);
                    }
                    break;
                case Types.CsDateTime:
                    if (column.DateFilterSetMode == ColumnUtilities.DateFilterSetMode.Default)
                    {
                        hb.DropDown(
                            context: context,
                            ss: ss,
                            column: column,
                            view: view,
                            optionCollection: column.DateFilterOptions(context: context),
                            idPrefix: idPrefix,
                            controlOnly: controlOnly,
                            action: action,
                            labelIcon: labelIcon,
                            useFilterButton: ss.UseFilterButton,
                            disabled: disabled);
                    }
                    else
                    {
                        hb.FieldTextBox(
                            fieldId: idPrefix + column.ColumnName + "_DateRangeField",
                            controlId: idPrefix + column.ColumnName + "_DateRange",
                            fieldCss: "field-auto-thin",
                            controlCss: (column.UseSearch == true ? " search" : string.Empty),
                            labelText: labelText,
                            labelTitle: ss.LabelTitle(column),
                            labelIcon: labelIcon,
                            controlOnly: controlOnly,
                            action: "openSetDateRangeDialog",
                            text: GetDisplayDateFilterRange(
                                context: context,
                                value: view.ColumnFilter(column.ColumnName),
                                timepicker: column.DateTimepicker()),
                            disabled: disabled,
                            method: "post",
                            attributes: new Dictionary<string, string>
                            {
                                ["onfocus"] = $"$p.openSetDateRangeDialog($(this))"
                            });
                        if (!context.Forms.ContainsKey("ViewFilters_Negative"))
                        {
                            hb.Hidden(attributes: new HtmlAttributes()
                                .Id(idPrefix + column.ColumnName)
                                .Class(column.UseSearch == true ? " search" : string.Empty)
                                .DataMethod("post")
                                .DataAction(action)
                                .Value(view.ColumnFilter(column.ColumnName)));
                        }
                    }
                    break;
                case Types.CsString:
                    if (column.HasChoices())
                    {
                        var currentSs = column.SiteSettings;
                        if (view.ColumnFilterHash?.ContainsKey(column.ColumnName) == true &&
                            column.UseSearch == true &&
                            currentSs.Links
                                ?.Where(o => o.SiteId > 0)
                                .Any(o => o.ColumnName == column.Name) == true)
                        {
                            currentSs.SetChoiceHash(
                                context: context,
                                columnName: column?.Name,
                                selectedValues: view.ColumnFilter(column.ColumnName)
                                    .Deserialize<List<string>>());
                        }
                        hb.DropDown(
                            context: context,
                            ss: currentSs,
                            column: column,
                            view: view,
                            optionCollection: column.UseSearch == true
                                ? []
                                : column.EditChoices(
                                    context: context,
                                    addNotSet: true,
                                    own: true,
                                    limit: Parameters.General.DropDownSearchPageSize),
                            idPrefix: idPrefix,
                            controlOnly: controlOnly,
                            action: action,
                            labelIcon: labelIcon,
                            useFilterButton: ss.UseFilterButton,
                            disabled: disabled);
                    }
                    else
                    {
                        hb.FieldTextBox(
                            fieldId: idPrefix + column.ColumnName + "Field",
                            controlId: idPrefix + column.ColumnName,
                            fieldCss: "field-auto-thin",
                            controlCss: ss.UseFilterButton != true
                                ? " auto-postback"
                                : string.Empty,
                            labelText: labelText,
                            labelTitle: ss.LabelTitle(column),
                            labelIcon: labelIcon,
                            controlOnly: controlOnly,
                            action: action,
                            text: view.ColumnFilter(column.ColumnName),
                            disabled: disabled,
                            method: "post");
                    }
                    break;
                default:
                    break;
            }
        }

        private static void SetNumericRangeDialog(
            HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Column column,
            string idPrefix,
            string action,
            string labelIcon,
            bool controlOnly,
            bool disabled = false)
        {
            hb
                .FieldTextBox(
                    fieldId: idPrefix + column.ColumnName + "_NumericRangeField",
                    controlId: idPrefix + column.ColumnName + "_NumericRange",
                    fieldCss: "field-auto-thin",
                    controlCss: (column.UseSearch == true ? " search" : string.Empty),
                    labelText: Strings.CoalesceEmpty(
                        ColumnUtilities.GetMultilingualLabelText(
                            target: column.MultilingualLabelText,
                            context: context),
                        column.GridLabelText),
                    labelTitle: ss.LabelTitle(column),
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    action: "openSetNumericRangeDialog",
                    text: GetNumericFilterRange(view.ColumnFilter(column.ColumnName)),
                    disabled: disabled,
                    method: "post",
                    attributes: new Dictionary<string, string>
                    {
                        ["onfocus"] = $"$p.openSetNumericRangeDialog($(this))"
                    });
            if (!context.Forms.ContainsKey("ViewFilters_Negative"))
            {
                hb
                    .Hidden(attributes: new HtmlAttributes()
                    .Id(idPrefix + column.ColumnName)
                    .Class(column.UseSearch == true ? " search" : string.Empty)
                    .DataMethod("post")
                    .DataAction(action)
                    .Value(view.ColumnFilter(column.ColumnName)));
            }
        }

        private static HtmlBuilder CheckBox(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            View view,
            string idPrefix = "ViewFilters__",
            string action = null,
            string labelIcon = null,
            bool controlOnly = false,
            bool disabled = false)
        {
            var labelText = Strings.CoalesceEmpty(
                ColumnUtilities.GetMultilingualLabelText(
                    target: column.MultilingualLabelText,
                    context: context),
                column.GridLabelText);
            switch (column.CheckFilterControlType)
            {
                case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                    return hb.FieldDropDown(
                        context: context,
                        fieldId: idPrefix + column.ColumnName + "Field",
                        controlId: idPrefix + column.ColumnName,
                        fieldCss: "field-auto-thin",
                        controlCss: ss.UseFilterButton != true
                            ? " auto-postback"
                            : string.Empty,
                        labelText: labelText,
                        labelTitle: ss.LabelTitle(column),
                        labelIcon: labelIcon,
                        controlOnly: controlOnly,
                        action: action,
                        optionCollection: ColumnUtilities
                            .CheckFilterTypeOptions(context: context),
                        selectedValue: view.ColumnFilter(column.ColumnName),
                        addSelectedValue: false,
                        insertBlank: true,
                        disabled: disabled,
                        method: "post");
                default:
                    return hb.FieldCheckBox(
                        fieldId: idPrefix + column.ColumnName + "Field",
                        controlId: idPrefix + column.ColumnName,
                        fieldCss: "field-auto-thin",
                        controlCss: ss.UseFilterButton != true
                            ? " auto-postback"
                            : string.Empty,
                        labelText: labelText,
                        labelTitle: ss.LabelTitle(column),
                        labelIcon: labelIcon,
                        controlOnly: controlOnly,
                        action: action,
                        _checked: view.ColumnFilter(column.ColumnName).ToBool(),
                        disabled: disabled,
                        method: "post");
            }
        }

        private static HtmlBuilder DropDown(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            View view,
            Dictionary<string, ControlData> optionCollection,
            string idPrefix = "ViewFilters__",
            string action = null,
            string labelIcon = null,
            bool controlOnly = false,
            bool? useFilterButton = false,
            bool disabled = false)
        {
            var selectedValue = view.ColumnFilter(column.ColumnName);
            if (column.UseSearch == true
                 || column.ChoiceHash?.Count == Parameters.General.DropDownSearchPageSize)
            {
                selectedValue?.Deserialize<List<string>>()?.ForEach(value =>
                {
                    if (column.Type != Settings.Column.Types.Normal)
                    {
                        var id = value.ToInt();
                        if (id > 0
                            && !(column.Type == Settings.Column.Types.User
                                && id == SiteInfo.AnonymousId))
                        {
                            optionCollection.AddIfNotConainsKey(
                                value, new ControlData(SiteInfo.Name(
                                    context: context,
                                    id: id,
                                    type: column.Type)));
                        }
                    }
                    else
                    {
                        HtmlFields.EditChoices(
                            context: context,
                            ss: ss,
                            column: column,
                            value: value)
                                .ForEach(data =>
                                    optionCollection.AddIfNotConainsKey(data.Key, data.Value));
                    }
                });
            }
            return hb.FieldDropDown(
                context: context,
                fieldId: idPrefix + column.ColumnName + "Field",
                controlId: idPrefix + column.ColumnName,
                fieldCss: "field-auto-thin",
                controlCss: (useFilterButton != true
                    ? " auto-postback"
                    : string.Empty)
                        + (column.UseSearch == true
                            ? " search"
                            : string.Empty),
                labelText: Strings.CoalesceEmpty(
                    ColumnUtilities.GetMultilingualLabelText(
                        target: column.MultilingualLabelText,
                        context: context),
                    column.GridLabelText),
                labelTitle: ss.LabelTitle(column),
                labelIcon: labelIcon,
                controlOnly: controlOnly,
                action: action,
                optionCollection: optionCollection,
                selectedValue: selectedValue,
                multiple: true,
                addSelectedValue: false,
                disabled: disabled,
                method: "post",
                column: column);
        }

        public static HtmlBuilder Search(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool disabled = false)
        {
            var labelIcon = view.ColumnFilterNegatives?.Contains("ViewFilters_Search") == true
                ? "ui-icon-info"
                : string.Empty;
            return ss.UseSearchFilter == true
                ? hb
                    .FieldTextBox(
                        fieldId: "ViewFilters_Search" + "Field",
                        controlId: "ViewFilters_Search",
                        fieldCss: "field-auto-thin",
                        controlCss: ss.UseFilterButton != true
                            ? " auto-postback"
                            : string.Empty,
                        labelText: Displays.Search(context: context),
                        labelIcon: labelIcon,
                        text: view.Search,
                        disabled: disabled,
                        method: "post",
                        _using: context.Controller == "items"
                            || context.Controller == "publishes")
                : hb;
        }

        private static HtmlBuilder FilterButton(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            return ss.UseFilterButton == true
                ? hb
                    .Button(
                        controlId: "FilterButton",
                        controlCss: "button-icon",
                        text: Displays.Filters(context: context),
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-search",
                        method: "post")
                    .Hidden(
                        controlId: "UseFilterButton",
                        value: "1")
                : hb;
        }

        private static string LabelIcon(SiteSettings ss, View view, string name)
        {
            return view.UseNegativeFilters(
                ss: ss,
                name: name) == true
                    ? "ui-icon-info"
                    : string.Empty;
        }
    }
}