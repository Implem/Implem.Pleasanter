using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlPermissionsViewer
    {
        private class PermissionsViewerRow
        {
            public long SiteId;
            public string Title;
            public long InheritSiteId;
            public string InheritTitle;
            public bool Inherited;
            public string Source;
            public string SourceType;
            public int SourceId;
            public bool Self;
            public Permissions.Types PermissionType;
            public DateTime UpdatedTime;
        }

        private class PermissionsViewerSorter
        {
            public string Key;
            public string OrderType;
        }

        private class PermissionsViewerFilter
        {
            public string Key;
            public string Value;
        }

        private class PermissionsViewerColumn
        {
            public string Key;
            public string Text;
        }

        private const string SortersId = "PermissionsViewerSorters";
        private const string FiltersId = "PermissionsViewerFilterConditions";
        private const string OffsetId = "PermissionsViewerOffset";
        private const string GridId = "PermissionsViewerGrid";
        private const string MenuId = "PermissionsViewerHeaderMenu";
        private const string MenusId = MenuId + "s";
        private const string BodyId = "PermissionsViewerBody";
        private const string MoreId = "PermissionsViewerMore";
        private const string ExpandTargetId = "PermissionsViewerExpandTarget";
        private const string ExportDialogId = "PermissionsViewerExportDialog";
        private const string FilterExpandId = "PermissionsViewerFilterExpand";
        private const string FiltersAreaId = "PermissionsViewerFilters";
        private const string CountId = "PermissionsViewerCount";
        private const string EncodingId = "PermissionsViewerExportEncoding";
        private static int PageSize => Parameters.General.GridPageSize > 0
            ? Parameters.General.GridPageSize
            : 20;

        private static readonly Permissions.Types[] ColumnTypes = new[]
        {
            Permissions.Types.Read,
            Permissions.Types.Create,
            Permissions.Types.Update,
            Permissions.Types.Delete,
            Permissions.Types.SendMail,
            Permissions.Types.Export,
            Permissions.Types.Import,
            Permissions.Types.ManageSite,
            Permissions.Types.ManagePermission
        };

        public static bool Readable(Context context, string tableName)
        {
            switch (tableName)
            {
                case "Depts":
                    return Permissions.CanManageTenant(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "Groups":
                    return Permissions.CanEditGroup(context: context);
                default:
                    return true;
            }
        }

        private static bool Licensed()
        {
            return Parameters.General.PermissionsViewer == true
                && Parameters.AllowPermissionsViewer();
        }

        private static HtmlBuilder LicenseGuide(this HtmlBuilder hb, Context context)
        {
            return hb.Div(css: "tabs-panel-inner", action: () => hb
                .Div(
                    id: "PermissionsViewerLicenseGuide",
                    action: () => hb.Div(
                        css: "recommend-guide-inner",
                        action: () => hb.A(
                            href: Parameters.General.RecommendUrl1.Params(
                                Parameters.General.PleasanterSource,
                                "manual",
                                "permissions-viewer"),
                            target: "_blank",
                            action: () => hb
                                .Text(text: Displays.RecommendPermissionsViewer(context: context))))));
        }

        public static ResponseCollection PermissionsViewer(
            Context context,
            ResponseCollection res,
            string tableName,
            int id)
        {
            if (!Licensed())
            {
                return res.Html(
                    target: "#FieldSetPermissionsViewer",
                    value: new HtmlBuilder().LicenseGuide(context: context));
            }
            var aggregate = Aggregate(
                context: context,
                tableName: tableName,
                id: id);
            var filterExpand = FilterExpand(
                context: context,
                tableName: tableName,
                id: id);
            var encoding = Encoding(
                context: context,
                tableName: tableName,
                id: id);
            var privileged = Privileged(
                context: context,
                tableName: tableName,
                id: id);
            var sorters = Sorters(
                context: context,
                tableName: tableName,
                id: id,
                aggregate: aggregate);
            var filters = Filters(
                context: context,
                tableName: tableName,
                id: id);
            var offset = context.Forms.Int(OffsetId);
            var totalCount = 0;
            var rows = Get(
                context: context,
                tableName: tableName,
                id: id,
                filters: filters,
                sorters: sorters,
                aggregate: aggregate,
                offset: offset,
                pageSize: PageSize,
                totalCount: out totalCount);
            if (offset == 0)
            {
                return context.Forms.ContainsKey(FiltersId)
                    ? res.Html(
                        target: $"#{GridId}",
                        value: new HtmlBuilder().Grid(
                            context: context,
                            tableName: tableName,
                            sorters: sorters,
                            rows: rows,
                            totalCount: totalCount,
                            aggregate: aggregate))
                    : res.Html(
                        target: "#FieldSetPermissionsViewer",
                        value: new HtmlBuilder().PermissionsViewer(
                            context: context,
                            tableName: tableName,
                            sorters: sorters,
                            filters: filters,
                            rows: rows,
                            totalCount: totalCount,
                            aggregate: aggregate,
                            filterExpand: filterExpand,
                            encoding: encoding,
                            privileged: privileged,
                            hasRows: totalCount > 0 || HasRows(
                                context: context,
                                tableName: tableName,
                                id: id)));
            }
            return res
                    .Append(
                        target: $"#{BodyId}",
                        value: new HtmlBuilder().BodyRows(
                            context: context,
                            tableName: tableName,
                            rows: rows,
                            aggregate: aggregate))
                    .Html(
                        target: $"#{MoreId}",
                        value: new HtmlBuilder().MoreButton(
                            context: context,
                            offset: offset + PageSize,
                            count: totalCount));
        }

        public static ResponseFile PermissionsViewerCsv(
            Context context,
            string tableName,
            int id)
        {
            if (!Licensed())
            {
                return null;
            }
            var aggregate = Aggregate(
                context: context,
                tableName: tableName,
                id: id);
            var sorters = Sorters(
                context: context,
                tableName: tableName,
                id: id,
                aggregate: aggregate);
            var filters = Filters(
                context: context,
                tableName: tableName,
                id: id);
            var totalCount = 0;
            var rows = Get(
                context: context,
                tableName: tableName,
                id: id,
                filters: filters,
                sorters: sorters,
                aggregate: aggregate,
                offset: 0,
                pageSize: 0,
                totalCount: out totalCount);
            return new ResponseFile(
                fileContent: Csv(
                    context: context,
                    rows: rows,
                    aggregate: aggregate),
                fileDownloadName: ExportUtilities.FileName(
                    context: context,
                    title: Displays.Get(
                        context: context,
                        id: tableName),
                    name: id.ToString()),
                encoding: Encoding(
                    context: context,
                    tableName: tableName,
                    id: id));
        }

        private static string Csv(
            Context context,
            List<PermissionsViewerRow> rows,
            bool aggregate)
        {
            var csv = new System.Text.StringBuilder();
            var columns = Columns(
                context: context,
                aggregate: aggregate);
            csv.Append(
                columns.Select(column => CsvValue(value: column.Text)).Join(","),
                "\n");
            rows.ForEach(row => csv.Append(
                CsvRow(
                    context: context,
                    row: row,
                    aggregate: aggregate).Join(","),
                "\n"));
            return csv.ToString();
        }

        private static List<string> CsvRow(
            Context context,
            PermissionsViewerRow row,
            bool aggregate)
        {
            var values = new List<string>
            {
                CsvValue(value: row.SiteId.ToString()),
                CsvValue(value: row.Title),
                CsvValue(value: row.Inherited
                    ? row.InheritTitle
                    : string.Empty)
            };
            if (!aggregate)
            {
                values.Add(CsvValue(value: row.Source));
            }
            ColumnTypes.ForEach(o => values.Add(CsvValue(value: row.PermissionType
                .HasFlag(o)
                .ToExport(
                    context: context,
                    column: null))));
            values.Add(CsvValue(value: row.UpdatedTime.ToLocal(
                context: context,
                format: Displays.Get(
                    context: context,
                    id: "YmdahmFormat"))));
            return values;
        }

        private static string CsvValue(string value)
        {
            return Models.CsvUtilities.EncloseDoubleQuotes(
                value: value,
                encloseDoubleQuotes: true);
        }

        private static HtmlBuilder PermissionsViewer(
            this HtmlBuilder hb,
            Context context,
            string tableName,
            List<PermissionsViewerSorter> sorters,
            List<PermissionsViewerFilter> filters,
            List<PermissionsViewerRow> rows,
            int totalCount,
            bool aggregate,
            bool filterExpand,
            string encoding,
            bool privileged,
            bool hasRows)
        {
            if (!hasRows)
            {
                return hb.Div(css: "tabs-panel-inner", action: () => hb
                    .PrivilegedAlert(
                        context: context,
                        privileged: privileged)
                    .Div(
                        css: "message-dialog",
                        action: () => hb
                            .Text(text: Displays.NotFoundPermissionsViewer(context: context))));
            }
            return hb.Div(css: "tabs-panel-inner", action: () => hb
                .PrivilegedAlert(
                    context: context,
                    privileged: privileged)
                .Hidden(
                    controlId: SortersId,
                    css: "always-send",
                    value: sorters.ToJson())
                .Hidden(
                    controlId: FiltersId,
                    css: "always-send",
                    value: filters.ToJson())
                .Hidden(
                    controlId: OffsetId,
                    css: "always-send",
                    value: "0")
                .Hidden(
                    controlId: FilterExpandId,
                    css: "always-send",
                    value: filterExpand
                        ? "1"
                        : "0")
                .Commands(
                    context: context,
                    aggregate: aggregate)
                .FilterSection(
                    context: context,
                    filterExpand: filterExpand)
                .ViewFilters(
                    context: context,
                    tableName: tableName,
                    filters: filters,
                    filterExpand: filterExpand)
                .Div(
                    id: GridId,
                    action: () => hb.Grid(
                        context: context,
                        tableName: tableName,
                        sorters: sorters,
                        rows: rows,
                        totalCount: totalCount,
                        aggregate: aggregate))
                .SorterMenus(context: context)
                .ExportDialog(
                    context: context,
                    encoding: encoding));
        }

        private static HtmlBuilder PrivilegedAlert(
            this HtmlBuilder hb,
            Context context,
            bool privileged)
        {
            return hb.Div(
                id: "PermissionsViewerAlert",
                _using: privileged,
                action: () => hb
                    .Div(action: () => hb
                        .Span(
                            css: "body alert-error",
                            action: () => hb
                                .Text(text: Displays.PrivilegedUserNotice(context: context)))));
        }

        private static HtmlBuilder Commands(
            this HtmlBuilder hb,
            Context context,
            bool aggregate)
        {
            return hb.Div(
                id: "PermissionsViewerCommands",
                action: () =>
                {
                    hb.FieldCheckBox(
                        fieldId: ExpandTargetId + "Field",
                        controlId: ExpandTargetId,
                        fieldCss: "field-auto-thin",
                        controlCss: " permissions-viewer-expand-target not-set-form-changed",
                        labelText: Displays.ExpandTarget(context: context),
                        _checked: !aggregate,
                        alwaysSend: true);
                    hb.Button(
                        controlId: "OpenPermissionsViewerExportDialog",
                        controlCss: "button-icon",
                        text: Displays.Export(context: context),
                        onClick: "$p.openDialog($(this), '#FieldSetPermissionsViewer');",
                        icon: "ui-icon-arrowthickstop-1-s",
                        selector: "#" + ExportDialogId);
                });
        }

        private static HtmlBuilder ExportDialog(
            this HtmlBuilder hb,
            Context context,
            string encoding)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id(ExportDialogId)
                    .Class("dialog")
                    .Title(Displays.Export(context: context)),
                action: () => hb
                    .FieldDropDown(
                        context: context,
                        fieldId: EncodingId + "Field",
                        controlId: EncodingId,
                        controlCss: " always-send not-set-form-changed",
                        labelText: Displays.CharacterCode(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            { "Shift-JIS", "Shift-JIS" },
                            { "UTF-8", "UTF-8" }
                        },
                        selectedValue: encoding,
                        addSelectedValue: false)
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "ExportPermissionsViewer",
                            controlCss: "button-icon button-positive",
                            text: Displays.Export(context: context),
                            onClick: "$p.exportPermissionsViewer();",
                            icon: "ui-icon-arrowreturnthick-1-w")
                        .Button(
                            controlCss: "button-icon button-neutral",
                            text: Displays.Cancel(context: context),
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        private static HtmlBuilder FilterSection(
            this HtmlBuilder hb,
            Context context,
            bool filterExpand)
        {
            return hb.Div(action: () => hb
                .Label(
                    css: "field-section expand",
                    attributes: new HtmlAttributes()
                        .For(FiltersAreaId),
                    action: () => hb
                        .Span(css: filterExpand
                            ? "ui-icon ui-icon-triangle-1-s"
                            : "ui-icon ui-icon-triangle-1-e")
                        .Text(text: Displays.Filters(context: context))));
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            Context context,
            string tableName,
            List<PermissionsViewerSorter> sorters,
            List<PermissionsViewerRow> rows,
            int totalCount,
            bool aggregate)
        {
            return hb
                .Div(
                    id: CountId,
                    css: "aggregations",
                    action: () => hb
                        .Span(css: "label", action: () => hb
                            .Text(text: Displays.Quantity(context: context)))
                        .Span(css: "data", action: () => hb
                            .Text(text: totalCount.ToString())))
                .GridTable(
                    context: context,
                    css: "not-link",
                    containerCss: "app-auto-height",
                    action: () => hb
                        .THead(action: () => hb
                            .Tr(
                                css: "ui-widget-header",
                                action: () => Columns(
                                    context: context,
                                    aggregate: aggregate)
                                        .ForEach(column => hb.HeaderCell(
                                            sorters: sorters,
                                            column: column))))
                        .TBody(
                            id: BodyId,
                            action: () => hb.BodyRows(
                                context: context,
                                tableName: tableName,
                                rows: rows,
                                aggregate: aggregate)))
                .Div(
                    id: MoreId,
                    css: "command-center",
                    action: () => hb.MoreButton(
                        context: context,
                        offset: PageSize,
                        count: totalCount));
        }

        private static HtmlBuilder BodyRows(
            this HtmlBuilder hb,
            Context context,
            string tableName,
            IEnumerable<PermissionsViewerRow> rows,
            bool aggregate)
        {
            rows.ForEach(row => hb
                .Tr(
                    css: "grid-row",
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .Text(text: row.SiteId.ToString()));
                        hb.Td(
                            attributes: new HtmlAttributes()
                                .Title(Breadcrumb(
                                    context: context,
                                    siteId: row.SiteId)),
                            action: () => hb
                                .A(
                                    href: SiteAccessControlUrl(
                                        context: context,
                                        siteId: row.SiteId),
                                    target: "_blank",
                                    text: row.Title));
                        hb.InheritCell(
                            context: context,
                            row: row);
                        if (!aggregate)
                        {
                            hb.SourceCell(
                                context: context,
                                row: row);
                        }
                        ColumnTypes.ForEach(o => hb
                            .Td(css: "center-align", action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: row.PermissionType.HasFlag(o))));
                        hb.Td(action: () => hb
                            .Text(text: row.UpdatedTime.ToLocal(
                                context: context,
                                format: Displays.Get(
                                    context: context,
                                    id: "YmdahmFormat"))));
                    }));
            return hb;
        }

        private static string SiteAccessControlUrl(Context context, long siteId)
        {
            return Locations.ItemEdit(
                context: context,
                id: siteId)
                    + "?TabName=FieldSetSiteAccessControl";
        }

        private static HtmlBuilder SourceCell(
            this HtmlBuilder hb,
            Context context,
            PermissionsViewerRow row)
        {
            if (row.Self || row.SourceType == "User")
            {
                return hb.Td(action: () => hb
                    .Text(text: row.Source));
            }
            return hb.Td(
                action: () => hb
                    .A(
                        href: Locations.Edit(
                            context: context,
                            controller: row.SourceType == "Dept"
                                ? "Depts"
                                : "Groups",
                            id: row.SourceId),
                        target: "_blank",
                        text: row.Source));
        }

        private static HtmlBuilder InheritCell(
            this HtmlBuilder hb,
            Context context,
            PermissionsViewerRow row)
        {
            if (!row.Inherited)
            {
                return hb.Td();
            }
            return hb.Td(
                attributes: new HtmlAttributes()
                    .Title(Breadcrumb(
                        context: context,
                        siteId: row.InheritSiteId)),
                action: () => hb
                    .A(
                        href: SiteAccessControlUrl(
                            context: context,
                            siteId: row.InheritSiteId),
                        target: "_blank",
                        text: row.InheritTitle));
        }

        private static string Breadcrumb(Context context, long siteId)
        {
            var sites = SiteInfo.TenantCaches.Get(context.TenantId)?.SiteMenu
                ?.Breadcrumb(
                    context: context,
                    siteId: siteId)
                        .Select(o => $"{o.Title}({o.SiteId})")
                            ?? new string[] { siteId.ToString() };
            return new string[] { Displays.Top(context: context) }
                .Concat(sites)
                .Join(" > ");
        }

        private static HtmlBuilder MoreButton(
            this HtmlBuilder hb,
            Context context,
            int offset,
            int count)
        {
            return hb.Button(
                controlId: "LoadNextPermissionsViewer",
                controlCss: "button-icon",
                text: Displays.LoadNextRows(
                    context: context,
                    data: PageSize.ToString()),
                onClick: "$p.loadNextPermissionsViewer();",
                icon: "ui-icon-arrowthick-1-s",
                _using: offset < count);
        }

        private static HtmlBuilder HeaderCell(
            this HtmlBuilder hb,
            List<PermissionsViewerSorter> sorters,
            PermissionsViewerColumn column)
        {
            var sorter = sorters.FirstOrDefault(o => o.Key == column.Key);
            return hb.Th(
                css: "sortable",
                attributes: new HtmlAttributes()
                    .DataName(column.Key)
                    .DataCellSticky(column.Key == "SiteId" || column.Key == "Title"),
                action: () => hb
                    .Div(
                        attributes: new HtmlAttributes()
                            .DataId(SortersId + "__" + column.Key)
                            .Add(
                                name: "data-order-type",
                                value: OrderBy(sorter: sorter)),
                        action: () => hb
                            .Span(action: () => hb
                                .Text(text: column.Text))
                            .SortIcon(orderType: sorter?.OrderType)));
        }

        private static List<PermissionsViewerColumn> Columns(Context context, bool aggregate)
        {
            var columns = new List<PermissionsViewerColumn>
            {
                new PermissionsViewerColumn
                {
                    Key = "SiteId",
                    Text = Displays.SiteId(context: context)
                },
                new PermissionsViewerColumn
                {
                    Key = "Title",
                    Text = Displays.Site(context: context)
                },
                new PermissionsViewerColumn
                {
                    Key = "Inherit",
                    Text = Displays.InheritSource(context: context)
                }
            };
            if (!aggregate)
            {
                columns.Add(new PermissionsViewerColumn
                {
                    Key = "Source",
                    Text = Displays.Target(context: context)
                });
            }
            ColumnTypes.ForEach(o => columns.Add(new PermissionsViewerColumn
            {
                Key = o.ToString(),
                Text = Displays.Get(
                    context: context,
                    id: o.ToString())
            }));
            columns.Add(new PermissionsViewerColumn
            {
                Key = "UpdatedTime",
                Text = Displays.PermissionSetTime(context: context)
            });
            return columns;
        }

        private static Dictionary<string, string> SourceTypes(Context context, string tableName)
        {
            switch (tableName)
            {
                case "Depts":
                    return new Dictionary<string, string>
                    {
                        { "Dept", Displays.Dept(context: context) },
                        { "Group", Displays.Group(context: context) }
                    };
                case "Groups":
                    return new Dictionary<string, string>
                    {
                        { "Group", Displays.Group(context: context) }
                    };
                default:
                    return new Dictionary<string, string>
                    {
                        { "User", Displays.User(context: context) },
                        { "Group", Displays.Group(context: context) },
                        { "Dept", Displays.Dept(context: context) }
                    };
            }
        }

        private static string OrderBy(PermissionsViewerSorter sorter)
        {
            switch (sorter?.OrderType)
            {
                case "asc": return "desc";
                case "desc": return "release";
                default: return "asc";
            }
        }

        private static HtmlBuilder ViewFilters(
            this HtmlBuilder hb,
            Context context,
            string tableName,
            List<PermissionsViewerFilter> filters,
            bool filterExpand)
        {
            return hb.Div(
                id: FiltersAreaId,
                attributes: new HtmlAttributes().Add(
                    name: "style",
                    value: "display: none;",
                    _using: !filterExpand),
                action: () =>
                {
                    hb.Button(
                        controlId: "ResetPermissionsViewerFilters",
                        controlCss: "button-icon",
                        text: Displays.Reset(context: context),
                        icon: "ui-icon-close");
                    hb.FieldTextBox(
                        context: context,
                        fieldId: FiltersAreaId + "__SiteId" + "Field",
                        controlId: FiltersAreaId + "__SiteId",
                        fieldCss: "field-auto-thin",
                        controlCss: " permissions-viewer-filter not-set-form-changed",
                        labelText: Displays.SiteId(context: context),
                        text: FilterValue(
                            filters: filters,
                            key: "SiteId"));
                    hb.FieldTextBox(
                        context: context,
                        fieldId: FiltersAreaId + "__Title" + "Field",
                        controlId: FiltersAreaId + "__Title",
                        fieldCss: "field-auto-thin",
                        controlCss: " permissions-viewer-filter not-set-form-changed",
                        labelText: Displays.Site(context: context),
                        text: FilterValue(
                            filters: filters,
                            key: "Title"));
                    var sourceTypes = SourceTypes(
                        context: context,
                        tableName: tableName);
                    hb.FieldDropDown(
                        context: context,
                        fieldId: FiltersAreaId + "__Source" + "Field",
                        controlId: FiltersAreaId + "__Source",
                        fieldCss: "field-auto-thin",
                        controlCss: " permissions-viewer-filter not-set-form-changed",
                        labelText: Displays.Target(context: context),
                        optionCollection: sourceTypes,
                        selectedValue: FilterValue(
                            filters: filters,
                            key: "Source"),
                        multiple: true,
                        addSelectedValue: false,
                        _using: sourceTypes.Count > 1);
                    hb.FieldTextBox(
                        context: context,
                        fieldId: FiltersAreaId + "__Inherit" + "Field",
                        controlId: FiltersAreaId + "__Inherit",
                        fieldCss: "field-auto-thin",
                        controlCss: " permissions-viewer-filter not-set-form-changed",
                        labelText: Displays.InheritSource(context: context),
                        text: FilterValue(
                            filters: filters,
                            key: "Inherit"));
                    ColumnTypes.ForEach(o => hb
                        .OnOffFilter(
                            context: context,
                            filters: filters,
                            key: o.ToString(),
                            labelText: Displays.Get(
                                context: context,
                                id: o.ToString())));
                });
        }

        private static HtmlBuilder OnOffFilter(
            this HtmlBuilder hb,
            Context context,
            List<PermissionsViewerFilter> filters,
            string key,
            string labelText)
        {
            return hb.FieldDropDown(
                context: context,
                fieldId: FiltersAreaId + "__" + key + "Field",
                controlId: FiltersAreaId + "__" + key,
                fieldCss: "field-auto-thin",
                controlCss: " permissions-viewer-filter not-set-form-changed",
                labelText: labelText,
                optionCollection: new Dictionary<string, string>
                {
                    { "1", Displays.On(context: context) },
                    { "0", Displays.Off(context: context) }
                },
                selectedValue: FilterValue(
                    filters: filters,
                    key: key),
                addSelectedValue: false,
                insertBlank: true);
        }

        private static string FilterValue(List<PermissionsViewerFilter> filters, string key)
        {
            return filters.FirstOrDefault(o => o.Key == key)?.Value ?? string.Empty;
        }

        private static HtmlBuilder SorterMenus(
            this HtmlBuilder hb,
            Context context)
        {
            return hb.Div(
                id: MenusId,
                action: () => Columns(
                    context: context,
                    aggregate: false)
                        .ForEach(column => hb.SorterMenu(
                            context: context,
                            key: column.Key)));
        }

        private static HtmlBuilder SorterMenu(
            this HtmlBuilder hb,
            Context context,
            string key)
        {
            return hb.Ul(
                id: MenuId + "__" + key,
                attributes: new HtmlAttributes()
                    .Class("menu menu-sort")
                    .Add(name: "style", value: "display: none; position: absolute;"),
                action: () => hb.SorterMenuItems(context: context));
        }

        private static HtmlBuilder SortIcon(this HtmlBuilder hb, string orderType)
        {
            switch (orderType)
            {
                case "asc":
                    return hb.Icon(iconCss: "ui-icon-triangle-1-n");
                case "desc":
                    return hb.Icon(iconCss: "ui-icon-triangle-1-s");
                default:
                    return hb.Span(
                        css: "ui-icon ui-icon-triangle-1-n",
                        attributes: new HtmlAttributes()
                            .Add(name: "style", value: "visibility: hidden;"));
            }
        }

        private static List<PermissionsViewerSorter> Sorters(
            Context context, string tableName, int id, bool aggregate)
        {
            var sorters = Conditions<PermissionsViewerSorter>(
                context: context,
                controlId: SortersId,
                tableName: tableName,
                id: id);
            if (aggregate)
            {
                sorters.RemoveAll(o => o.Key == "Source");
            }
            return sorters;
        }

        private static bool Privileged(Context context, string tableName, int id)
        {
            return tableName == "Users"
                && Permissions.PrivilegedUsers(loginId: SiteInfo.User(
                    context: context,
                    userId: id).LoginId);
        }

        private static string Encoding(Context context, string tableName, int id)
        {
            var value = Value(
                context: context,
                controlId: EncodingId,
                tableName: tableName,
                id: id);
            return value == "UTF-8"
                ? value
                : "Shift-JIS";
        }

        private static bool Aggregate(Context context, string tableName, int id)
        {
            return !Flag(
                context: context,
                controlId: ExpandTargetId,
                tableName: tableName,
                id: id,
                defaultValue: false);
        }

        private static bool FilterExpand(Context context, string tableName, int id)
        {
            return Flag(
                context: context,
                controlId: FilterExpandId,
                tableName: tableName,
                id: id,
                defaultValue: true);
        }

        private static List<PermissionsViewerFilter> Filters(
            Context context, string tableName, int id)
        {
            return Conditions<PermissionsViewerFilter>(
                context: context,
                controlId: FiltersId,
                tableName: tableName,
                id: id);
        }

        private static List<T> Conditions<T>(
            Context context, string controlId, string tableName, int id)
        {
            var key = SessionKey(
                context: context,
                controlId: controlId,
                tableName: tableName,
                id: id);
            var json = context.Forms.Data(controlId);
            if (context.Forms.ContainsKey(controlId))
            {
                SessionUtilities.Set(
                    context: context,
                    key: key,
                    value: json);
            }
            else
            {
                json = context.SessionData.Get(key);
            }
            return json.Deserialize<List<T>>() ?? new List<T>();
        }

        private static bool Flag(
            Context context, string controlId, string tableName, int id, bool defaultValue)
        {
            var key = SessionKey(
                context: context,
                controlId: controlId,
                tableName: tableName,
                id: id);
            if (context.Forms.ContainsKey(controlId))
            {
                var value = context.Forms.Bool(controlId);
                SessionUtilities.Set(
                    context: context,
                    key: key,
                    value: value
                        ? "1"
                        : "0");
                return value;
            }
            var saved = context.SessionData.Get(key);
            return saved.IsNullOrEmpty()
                ? defaultValue
                : saved == "1";
        }

        private static string Value(
            Context context, string controlId, string tableName, int id)
        {
            var key = SessionKey(
                context: context,
                controlId: controlId,
                tableName: tableName,
                id: id);
            if (context.Forms.ContainsKey(controlId))
            {
                var value = context.Forms.Data(controlId);
                SessionUtilities.Set(
                    context: context,
                    key: key,
                    value: value);
                return value;
            }
            return context.SessionData.Get(key);
        }

        private static string SessionKey(
            Context context, string controlId, string tableName, int id)
        {
            return $"{controlId}_{context.TenantId}_{tableName}_{id}";
        }

        private static List<PermissionsViewerRow> Get(
            Context context,
            string tableName,
            int id,
            List<PermissionsViewerFilter> filters,
            List<PermissionsViewerSorter> sorters,
            bool aggregate,
            int offset,
            int pageSize,
            out int totalCount)
        {
            totalCount = 0;
            var where = Where(
                context: context,
                tableName: tableName,
                id: id,
                filters: filters);
            if (where == null)
            {
                return new List<PermissionsViewerRow>();
            }
            var dataSet = Repository.ExecuteDataSet(
                context: context,
                transactional: false,
                statements: new SqlStatement[]
                {
                    Rds.SelectPermissions(
                        dataTableName: "Main",
                        column: MainColumn(aggregate: aggregate),
                        join: SitesJoin(),
                        where: where,
                        groupBy: GroupBy(aggregate: aggregate),
                        orderBy: OrderBy(
                            tableName: tableName,
                            id: id,
                            sorters: sorters,
                            aggregate: aggregate),
                        offset: offset,
                        pageSize: pageSize),
                    CountStatement(
                        where: where,
                        aggregate: aggregate)
                });
            totalCount = Rds.Count(dataSet);
            return Rows(
                context: context,
                tableName: tableName,
                id: id,
                aggregate: aggregate,
                dataRows: dataSet.Tables["Main"].AsEnumerable());
        }

        private static SqlColumnCollection MainColumn(bool aggregate)
        {
            if (!aggregate)
            {
                return Rds.PermissionsColumn()
                    .ReferenceId()
                    .DeptId()
                    .GroupId()
                    .UserId()
                    .PermissionType()
                    .UpdatedTime()
                    .Add(
                        tableName: "Sites",
                        columnBracket: "\"SiteId\"",
                        _as: "SiteId");
            }
            var column = new SqlColumnCollection()
                .Add(
                    tableName: "Sites",
                    columnBracket: "\"SiteId\"",
                    _as: "SiteId")
                .Add(
                    tableName: "Permissions",
                    columnBracket: "\"ReferenceId\"",
                    _as: "ReferenceId")
                .Add(
                    columnBracket: "max(\"Permissions\".\"UpdatedTime\")",
                    _as: "UpdatedTime");
            ColumnTypes.ForEach(o => column.Add(
                columnBracket: $"max({PermissionTypeRaw(key: o.ToString())})",
                _as: o.ToString()));
            return column;
        }

        private static SqlGroupByCollection GroupBy(bool aggregate)
        {
            return aggregate
                ? new SqlGroupByCollection()
                    .Add(columnBracket: "\"SiteId\"", tableName: "Sites")
                    .Add(columnBracket: "\"Title\"", tableName: "Sites")
                    .Add(columnBracket: "\"ReferenceId\"", tableName: "Permissions")
                    .Add(columnBracket: "\"Title\"", tableName: "InheritSites")
                : null;
        }

        private static SqlStatement CountStatement(SqlWhereCollection where, bool aggregate)
        {
            return aggregate
                ? Rds.SelectPermissions(
                    dataTableName: "Count",
                    column: new SqlColumnCollection().Add(
                        columnBracket: "count(distinct \"Sites\".\"SiteId\")"),
                    join: SitesJoin(),
                    where: where)
                : Rds.SelectCount(
                    tableName: "Permissions",
                    join: SitesJoin(),
                    where: where);
        }

        private static bool HasRows(Context context, string tableName, int id)
        {
            var where = Where(
                context: context,
                tableName: tableName,
                id: id,
                filters: new List<PermissionsViewerFilter>());
            return where != null
                && Rds.Count(Repository.ExecuteDataSet(
                    context: context,
                    transactional: false,
                    statements: new SqlStatement[]
                    {
                        Rds.SelectCount(
                            tableName: "Permissions",
                            join: SitesJoin(),
                            where: where)
                    })) > 0;
        }

        private static SqlOrderByCollection OrderBy(
            string tableName, int id, List<PermissionsViewerSorter> sorters, bool aggregate)
        {
            var orderBy = new SqlOrderByCollection();
            if (!sorters.Any())
            {
                Add(
                    orderBy: orderBy,
                    tableName: tableName,
                    id: id,
                    key: "SiteId",
                    orderType: "desc",
                    aggregate: aggregate);
                if (!aggregate)
                {
                    Add(
                        orderBy: orderBy,
                        tableName: tableName,
                        id: id,
                        key: "Source",
                        orderType: "asc",
                        aggregate: aggregate);
                }
                AddUniqueOrder(
                    orderBy: orderBy,
                    aggregate: aggregate);
                return orderBy;
            }
            sorters.ForEach(sorter => Add(
                orderBy: orderBy,
                tableName: tableName,
                id: id,
                key: sorter.Key,
                orderType: sorter.OrderType,
                aggregate: aggregate));
            AddUniqueOrder(
                orderBy: orderBy,
                aggregate: aggregate);
            return orderBy;
        }

        private static void AddUniqueOrder(SqlOrderByCollection orderBy, bool aggregate)
        {
            var raws = new List<string> { "\"Sites\".\"SiteId\"" };
            if (!aggregate)
            {
                raws.Add("\"Permissions\".\"DeptId\"");
                raws.Add("\"Permissions\".\"GroupId\"");
                raws.Add("\"Permissions\".\"UserId\"");
            }
            raws
                .Where(raw => !orderBy.Any(o => o.Raw?.StartsWith(raw) == true))
                .ForEach(raw => orderBy.Add(raw: raw + " asc"));
        }

        private static void Add(
            SqlOrderByCollection orderBy,
            string tableName,
            int id,
            string key,
            string orderType,
            bool aggregate)
        {
            var direction = orderType == "desc"
                ? " desc"
                : " asc";
            SortRaws(
                tableName: tableName,
                id: id,
                key: key,
                aggregate: aggregate)
                    .ForEach(raw => orderBy.Add(raw: raw + direction));
        }

        private static List<string> SortRaws(
            string tableName, int id, string key, bool aggregate)
        {
            switch (key)
            {
                case "SiteId":
                    return new List<string> { "\"Sites\".\"SiteId\"" };
                case "Title":
                    return new List<string> { "\"Sites\".\"Title\"" };
                case "Source":
                    return new List<string>
                    {
                        SourceTypeOrderRaw(),
                        SourceNameOrderRaw(
                            tableName: tableName,
                            id: id)
                    };
                case "Inherit":
                    return new List<string> { InheritTitleRaw() };
                case "UpdatedTime":
                    return new List<string>
                    {
                        aggregate
                            ? "max(\"Permissions\".\"UpdatedTime\")"
                            : "\"Permissions\".\"UpdatedTime\""
                    };
                default:
                    var raw = PermissionTypeRaw(key: key);
                    return new List<string>
                    {
                        aggregate
                            ? $"max({raw})"
                            : raw
                    };
            }
        }

        private static string SourceTypeOrderRaw()
        {
            return "case when \"Permissions\".\"UserId\"<>0 then 1"
                + " when \"Permissions\".\"GroupId\"<>0 then 2 else 3 end";
        }

        private static string SourceNameOrderRaw(string tableName, int id)
        {
            var name = "coalesce(\"SourceGroups\".\"GroupName\","
                + "\"SourceDepts\".\"DeptName\",'')";
            switch (tableName)
            {
                case "Depts":
                    return $"case when \"Permissions\".\"DeptId\"={id}"
                        + $" then '' else {name} end";
                case "Groups":
                    return $"case when \"Permissions\".\"GroupId\"={id}"
                        + $" then '' else {name} end";
                default:
                    return name;
            }
        }

        private static string InheritTitleRaw()
        {
            return "case when \"Sites\".\"SiteId\"<>\"Permissions\".\"ReferenceId\""
                + " then \"InheritSites\".\"Title\" else '' end";
        }

        private static string PermissionTypeRaw(string key)
        {
            var type = Permissions.Get(key).ToLong();
            return $"case when \"Permissions\".\"PermissionType\" & {type} = {type}"
                + " then 1 else 0 end";
        }

        private static SqlWhereCollection Where(
            Context context,
            string tableName,
            int id,
            List<PermissionsViewerFilter> filters)
        {
            var where = TargetWhere(
                context: context,
                tableName: tableName,
                id: id);
            if (where == null)
            {
                return null;
            }
            var ret = Rds.PermissionsWhere();
            ret.AddRange(Rds.SitesWhere().TenantId(context.TenantId));
            ret.AddRange(where);
            ret.AddRange(FilterWhere(filters: filters));
            return ret;
        }

        private static SqlWhereCollection TargetWhere(Context context, string tableName, int id)
        {
            switch (tableName)
            {
                case "Depts":
                    return Rds.PermissionsWhere()
                        .Or(or: Rds.PermissionsWhere()
                            .DeptId(id)
                            .GroupId_In(sub: Rds.SelectGroupMembers(
                                distinct: true,
                                column: Rds.GroupMembersColumn().GroupId(),
                                where: TenantGroupMembersWhere(context: context)
                                    .DeptId(id))));
                case "Groups":
                    return Rds.PermissionsWhere().GroupId_In(GroupIds(
                        context: context,
                        id: id));
                case "Users":
                    var userModel = SiteInfo.User(context: context, userId: id);
                    return Rds.PermissionsWhere()
                        .Or(or: Rds.PermissionsWhere()
                            .UserId(id)
                            .DeptId(userModel.DeptId, _using: userModel.DeptId != 0)
                            .GroupId_In(sub: Rds.SelectGroupMembers(
                                distinct: true,
                                column: Rds.GroupMembersColumn().GroupId(),
                                where: TenantGroupMembersWhere(context: context)
                                    .Or(or: Rds.GroupMembersWhere()
                                        .UserId(id)
                                        .DeptId(
                                            userModel.DeptId,
                                            _using: userModel.DeptId != 0)))));
                default:
                    return null;
            }
        }

        private static Rds.GroupMembersWhereCollection TenantGroupMembersWhere(Context context)
        {
            return Rds.GroupMembersWhere()
                .GroupId_In(sub: Rds.SelectGroups(
                    column: Rds.GroupsColumn().GroupId(),
                    where: Rds.GroupsWhere().TenantId(context.TenantId)));
        }

        private static List<int> GroupIds(Context context, int id)
        {
            return GroupChildUtilities.GetParentIds(
                context: context,
                startId: id)
                    .Concat(GroupChildUtilities.GetChildrenIds(
                        context: context,
                        startIds: new int[] { id }))
                    .Concat(new int[] { id })
                    .Distinct()
                    .ToList();
        }

        private static SqlJoinCollection SitesJoin()
        {
            return new SqlJoinCollection(
                new SqlJoin(
                    tableBracket: "\"Sites\"",
                    joinType: SqlJoin.JoinTypes.Inner,
                    joinExpression: "\"Sites\".\"InheritPermission\""
                        + "=\"Permissions\".\"ReferenceId\""),
                new SqlJoin(
                    tableBracket: "\"Sites\"",
                    joinType: SqlJoin.JoinTypes.LeftOuter,
                    joinExpression: "\"InheritSites\".\"SiteId\""
                        + "=\"Permissions\".\"ReferenceId\"",
                    _as: "InheritSites"),
                new SqlJoin(
                    tableBracket: "\"Depts\"",
                    joinType: SqlJoin.JoinTypes.LeftOuter,
                    joinExpression: "\"SourceDepts\".\"DeptId\""
                        + "=\"Permissions\".\"DeptId\"",
                    _as: "SourceDepts"),
                new SqlJoin(
                    tableBracket: "\"Groups\"",
                    joinType: SqlJoin.JoinTypes.LeftOuter,
                    joinExpression: "\"SourceGroups\".\"GroupId\""
                        + "=\"Permissions\".\"GroupId\"",
                    _as: "SourceGroups"));
        }

        private static SqlWhereCollection FilterWhere(List<PermissionsViewerFilter> filters)
        {
            var where = Rds.PermissionsWhere();
            filters
                .Where(filter => !filter.Value.IsNullOrEmpty())
                .ForEach(filter =>
                {
                    switch (filter.Key)
                    {
                        case "SiteId":
                            where.Add(
                                tableName: "Sites",
                                columnBrackets: new string[] { "\"SiteId\"" },
                                name: "FilterSiteId",
                                value: filter.Value.Trim().ToLong());
                            break;
                        case "Title":
                            where.Add(
                                tableName: "Sites",
                                columnBrackets: new string[] { "\"Title\"" },
                                name: "FilterTitle",
                                value: $"%{filter.Value}%",
                                _operator: " like ");
                            break;
                        case "Inherit":
                            where.Add(raw: InheritedRaw());
                            where.Add(
                                tableName: "InheritSites",
                                columnBrackets: new string[] { "\"Title\"" },
                                name: "FilterInherit",
                                value: $"%{filter.Value}%",
                                _operator: " like ");
                            break;
                        case "Source":
                            var values = filter.Value.Deserialize<List<string>>()
                                ?? new List<string> { filter.Value };
                            where.Add(raw: SourceTypeRaw(values: values));
                            break;
                        default:
                            where.Add(raw: PermissionTypeRaw(key: filter.Key)
                                + (filter.Value == "1"
                                    ? "=1"
                                    : "=0"));
                            break;
                    }
                });
            return where;
        }

        private static string InheritedRaw()
        {
            return "\"Sites\".\"SiteId\"<>\"Permissions\".\"ReferenceId\"";
        }

        private static string SourceTypeRaw(List<string> values)
        {
            var conditions = values
                .Select(value => value switch
                {
                    "User" => "\"Permissions\".\"UserId\"<>0",
                    "Group" => "\"Permissions\".\"GroupId\"<>0",
                    "Dept" => "\"Permissions\".\"DeptId\"<>0",
                    _ => "1=0"
                })
                .ToList();
            return conditions.Any()
                ? "(" + conditions.Join(" or ") + ")"
                : "1=0";
        }

        private static List<PermissionsViewerRow> Rows(
            Context context,
            string tableName,
            int id,
            bool aggregate,
            EnumerableRowCollection<DataRow> dataRows)
        {
            var siteMenu = SiteInfo.TenantCaches.Get(context.TenantId)?.SiteMenu;
            if (aggregate)
            {
                return dataRows
                    .Select(dataRow =>
                    {
                        var siteId = dataRow.Long("SiteId");
                        var inheritSiteId = dataRow.Long("ReferenceId");
                        return new PermissionsViewerRow
                        {
                            SiteId = siteId,
                            Title = siteMenu?.Get(siteId)?.Title ?? siteId.ToString(),
                            InheritSiteId = inheritSiteId,
                            InheritTitle = siteMenu?.Get(inheritSiteId)?.Title
                                ?? inheritSiteId.ToString(),
                            Inherited = siteId != inheritSiteId,
                            PermissionType = ColumnTypes
                                .Where(o => dataRow.Int(o.ToString()) == 1)
                                .Aggregate(
                                    (Permissions.Types)0,
                                    (types, o) => types | o),
                            UpdatedTime = dataRow.DateTime("UpdatedTime")
                        };
                    })
                        .Where(o => siteMenu?.ContainsKey(o.SiteId) == true)
                        .ToList();
            }
            return dataRows
                .Select(dataRow =>
                {
                    var sourceType = SourceType(dataRow: dataRow);
                    var sourceId = SourceId(dataRow: dataRow);
                    var self = tableName == "Users"
                        ? sourceType == "User"
                        : sourceId == id;
                    var siteId = dataRow.Long("SiteId");
                    var inheritSiteId = dataRow.Long("ReferenceId");
                    return new PermissionsViewerRow
                    {
                        SiteId = siteId,
                        Title = siteMenu?.Get(siteId)?.Title ?? siteId.ToString(),
                        InheritSiteId = inheritSiteId,
                        InheritTitle = siteMenu?.Get(inheritSiteId)?.Title
                            ?? inheritSiteId.ToString(),
                        Inherited = siteId != inheritSiteId,
                        Source = Source(
                            context: context,
                            dataRow: dataRow,
                            sourceType: sourceType,
                            self: self),
                        SourceType = sourceType,
                        SourceId = sourceId,
                        Self = self,
                        PermissionType = (Permissions.Types)dataRow.Long("PermissionType"),
                        UpdatedTime = dataRow.DateTime("UpdatedTime")
                    };
                })
                    .Where(o => siteMenu?.ContainsKey(o.SiteId) == true)
                    .ToList();
        }

        private static string Source(
            Context context, DataRow dataRow, string sourceType, bool self)
        {
            switch (sourceType)
            {
                case "Dept":
                    if (self)
                    {
                        return Displays.Dept(context: context);
                    }
                    var deptName = SiteInfo.Dept(
                        tenantId: context.TenantId,
                        deptId: dataRow.Int("DeptId")).Name;
                    return $"{Displays.Dept(context: context)} - {deptName}";
                case "Group":
                    if (self)
                    {
                        return Displays.Group(context: context);
                    }
                    var groupName = SiteInfo.Group(
                        tenantId: context.TenantId,
                        groupId: dataRow.Int("GroupId")).Name;
                    return $"{Displays.Group(context: context)} - {groupName}";
                default:
                    return Displays.User(context: context);
            }
        }

        private static int SourceId(DataRow dataRow)
        {
            var deptId = dataRow.Int("DeptId");
            return deptId != 0
                ? deptId
                : dataRow.Int("GroupId");
        }

        private static string SourceType(DataRow dataRow)
        {
            if (dataRow.Int("DeptId") != 0)
            {
                return "Dept";
            }
            return dataRow.Int("GroupId") != 0
                ? "Group"
                : "User";
        }
    }
}
