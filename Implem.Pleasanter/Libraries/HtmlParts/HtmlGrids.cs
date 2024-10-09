using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlGrids
    {
    public static HtmlBuilder TableSet(
        this HtmlBuilder hb,
        string id = null,
        string css = null,
        HtmlAttributes attributes = null,
        bool _using = true,
        Action action = null)
            {
                return _using
                    ? hb.Div(
                        id: !id.IsNullOrEmpty()
                            ? id + "Wrap"
                            : string.Empty,
                        css: "table-wrap",
                        action: () => hb.Table(
                            id: id,
                            css: css,
                            attributes: attributes,
                            action: action))
                    : hb;
            }

        public static HtmlBuilder GridHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<Column> columns,
            View view = null,
            bool sort = true,
            bool editRow = false,
            bool checkRow = true,
            bool checkAll = false,
            string action = "GridRows",
            ServerScriptModelRow serverScriptModelRow = null,
            string suffix = "")
        {
            return hb.Tr(
                css: "ui-widget-header",
                action: () =>
                {
                    if (editRow)
                    {
                        hb.Th(
                            action: () =>
                            {
                                if (editRow && context.CanCreate(ss: ss))
                                {
                                    hb.Button(
                                        title: Displays.New(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.newOnGrid($(this));",
                                        icon: "ui-icon-plus",
                                        action: "NewOnGrid",
                                        method: "post");
                                }
                            });
                    }
                    else if (checkRow)
                    {
                        hb.Th(action: () => hb
                            .CheckBox(
                                controlId: "GridCheckAll",
                                _checked: checkAll));
                    }
                    columns.ForEach(column =>
                    {
                        var gridLabelText = column.GridLabelText;
                        var serverScriptLabelText = serverScriptModelRow?.Columns
                            ?.Get(column.ColumnName)
                            ?.LabelText;
                        if (gridLabelText == column.LabelText
                            && serverScriptLabelText != null
                            && serverScriptLabelText != column.LabelText)
                        {
                            gridLabelText = serverScriptLabelText;
                        }
                        if (sort)
                        {
                            hb.Th(
                                css: suffix.IsNullOrEmpty()
                                    ? column.CellCss(css: "sortable")
                                    : null,
                                attributes: new HtmlAttributes()
                                    .DataName(column.ColumnName),
                                action: () => hb
                                    .Div(
                                        attributes: new HtmlAttributes()
                                            .DataId("ViewSorters__" + column.ColumnName)
                                            .Add("data-order-type", OrderBy(
                                                view, column.ColumnName))
                                            .DataAction(action)
                                            .DataMethod("post"),
                                        action: () => hb
                                            .Span(action: () => hb
                                                .Text(text: gridLabelText))
                                            .SortIcon(
                                                view: view,
                                                key: column.ColumnName)));
                        }
                        else
                        {
                            hb.Th(
                                css: column.CellCss(),
                                action: () => hb
                                    .Text(text: gridLabelText));
                        }
                    });
                });
        }

        public static HtmlBuilder GridHeaderMenus(
           this HtmlBuilder hb,
           Context context,
           SiteSettings ss,
           View view,
           IEnumerable<Column> columns,
           string suffix = "")
        {
            return hb.Div(id: "GridHeaderMenus", action: () =>
                columns.ForEach(column => hb
                    .Ul(
                        id: "GridHeaderMenu__" + column.ColumnName,
                        attributes: new HtmlAttributes()
                            .Class("menu menu-sort")
                            .Add("data-target", $"[data-id='ViewSorters__{column.ColumnName}']")
                            .Add("style", "display: none; position: absolute;"),
                        action: () => hb
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("grid-header-filter"),
                                action: () => hb.ViewFiltersColumnOnGrid(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    column: column),
                                _using: ss.UseGridHeaderFilters == true && ss.IsDefaultFilterColumn(column))
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("sort")
                                    .Add("data-order-type", "asc"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-triangle-1-n"))
                                        .Text(text: Displays.OrderAsc(context)))
                                )
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("sort")
                                    .Add("data-order-type", "desc"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-triangle-1-s"))
                                        .Text(text: Displays.OrderDesc(context)))
                                )
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("sort")
                                    .Add("data-order-type", "release"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-close"))
                                        .Text(text: Displays.OrderRelease(context)))
                                )
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("reset"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-power"))
                                        .Text(text: Displays.ResetOrder(context)))))),
                        _using: suffix.IsNullOrEmpty());
        }

        public static HtmlBuilder ViewFiltersLabelMenus(
           this HtmlBuilder hb,
           Context context,
           SiteSettings ss)
        {
            return ss.UseNegativeFilters == true
                ? hb.Div(id: "ViewFiltersLabelMenus", action: () =>
                    hb.Ul(
                        id: "ViewFilters__",
                        attributes: new HtmlAttributes()
                            .Class("menu menu-negative")
                            .DataMethod("post")
                            .Add("style", "display: none; position: absolute;"),
                        action: () => hb
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("negative"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-notice"))
                                        .Text(text: Displays.Negative(context)))
                                )
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("positive"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-power"))
                                        .Text(text: Displays.Positive(context))))))
                : hb;
        }

        private static string OrderBy(View view, string key)
        {
            switch (view?.ColumnSorter(key))
            {
                case SqlOrderBy.Types.asc: return "desc";
                case SqlOrderBy.Types.desc: return string.Empty;
                default: return "asc";
            }
        }

        public static HtmlBuilder SortIcon(this HtmlBuilder hb, View view, string key)
        {
            switch (view?.ColumnSorter(key))
            {
                case SqlOrderBy.Types.asc: return hb.Icon(iconCss: "ui-icon-triangle-1-n");
                case SqlOrderBy.Types.desc: return hb.Icon(iconCss: "ui-icon-triangle-1-s");
                default: return hb;
            }
        }

        public static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            List<Column> columns,
            EnumerableRowCollection<DataRow> dataRows,
            FormDataSet formDataSet = null,
            bool editRow = false,
            bool checkRow = true)
        {
            dataRows.ForEach(dataRow =>
                hb.Tr(
                    context: context,
                    ss: ss,
                    view: view,
                    columns: columns,
                    dataRow: dataRow,
                    recordSelector: new RecordSelector(context),
                    editRow: editRow,
                    checkRow: checkRow,
                    idColumn: Rds.IdColumn(ss.ReferenceType),
                    formDataSet: formDataSet));
            return hb;
        }

        public static HtmlBuilder Tr(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            List<Column> columns,
            DataRow dataRow,
            bool editRow,
            bool checkRow,
            string idColumn,
            RecordSelector recordSelector = null,
            FormDataSet formDataSet = null)
        {
            var dataId = dataRow.Long(idColumn);
            var dataVersion = dataRow.Int("Ver");
            var isHistory = dataRow.Bool("IsHistory");
            ServerScriptModelRow serverScriptModelRow = null;
            var depts = new Dictionary<string, DeptModel>();
            var groups = new Dictionary<string, GroupModel>();
            var registrations = new Dictionary<string, RegistrationModel>();
            var sites = new Dictionary<string, SiteModel>();
            var sysLogs = new Dictionary<string, SysLogModel>();
            var users = new Dictionary<string, UserModel>();
            var dashboards = new Dictionary<string, DashboardModel>();
            var issues = new Dictionary<string, IssueModel>();
            var results = new Dictionary<string, ResultModel>();
            switch (ss.ReferenceType)
            {
                case "Issues":
                    var issueModel = new IssueModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRow,
                        formData: editRow
                            ? formDataSet?.FirstOrDefault(o =>
                                o.Id == dataRow.Long("IssueId"))?.Data
                            : null);
                    ss.ClearColumnAccessControlCaches(baseModel: issueModel);
                    serverScriptModelRow = issueModel?.SetByBeforeOpeningRowServerScript(
                        context: context,
                        ss: ss,
                        view: view);
                    issues.Add("Issues", issueModel);
                    break;
                case "Results":
                    var resultModel = new ResultModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRow,
                        formData: editRow
                            ? formDataSet?.FirstOrDefault(o =>
                                o.Id == dataRow.Long("ResultId"))?.Data
                            : null);
                    ss.ClearColumnAccessControlCaches(baseModel: resultModel);
                    serverScriptModelRow = resultModel?.SetByBeforeOpeningRowServerScript(
                        context: context,
                        ss: ss,
                        view: view);
                    results.Add("Results", resultModel);
                    break;
            };
            var extendedRowCss = serverScriptModelRow?.ExtendedRowCss;
            extendedRowCss = extendedRowCss.IsNullOrEmpty() ? string.Empty : " " + extendedRowCss;
            return hb.Tr(
                attributes: new HtmlAttributes()
                    .Class("grid-row" + extendedRowCss)
                    .DataId(dataId.ToString())
                    .DataVer(dataVersion)
                    .DataLatest(1, _using: !isHistory)
                    .Add(name: "data-history", value: "1", _using: isHistory)
                    .Add(name: "data-locked", value: "1", _using: dataRow.Bool("Locked"))
                    .Add(name: "data-extension", value: serverScriptModelRow?.ExtendedRowData),
                action: () =>
                {
                    if (editRow)
                    {
                        hb.Td(action: () => hb
                            .Button(
                                title: Displays.Reload(context: context),
                                controlCss: "button-icon",
                                onClick: $"$p.getData($(this)).Id={dataId};$p.send($(this));",
                                icon: "ui-icon-refresh",
                                action: "ReloadRow",
                                method: "post",
                                _using: !isHistory)
                            .Button(
                                title: Displays.Copy(context: context),
                                controlCss: "button-icon",
                                onClick: $"$p.getData($(this)).OriginalId={dataId};$p.send($(this));",
                                icon: "ui-icon-copy",
                                action: "CopyRow",
                                method: "post",
                                _using: !isHistory && context.CanCreate(ss: ss))
                            .Hidden(
                                controlId: $"{ss.ReferenceType}_Timestamp_{ss.SiteId}_{dataId}",
                                css: "timestamp",
                                value: dataRow
                                    .Field<DateTime>("UpdatedTime")
                                    .ToString("yyyy/M/d H:m:s.fff")));
                    }
                    else if (checkRow)
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: recordSelector.Checked(dataId),
                                dataId: dataId.ToString(),
                                _using: !isHistory));
                    }
                    columns.ForEach(column =>
                    {
                        var key = column.TableName();
                        var serverScriptModelColumn = serverScriptModelRow
                            ?.Columns
                            ?.Get(column?.ColumnName);
                        switch (column.SiteSettings?.ReferenceType)
                        {
                            case "Depts":
                                var deptModel = depts.Get(key);
                                if (deptModel == null)
                                {
                                    deptModel = new DeptModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        tableAlias: column.TableAlias);
                                    depts.Add(key, deptModel);
                                    ss.ClearColumnAccessControlCaches(baseModel: deptModel);
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    deptModel: deptModel);
                                break;
                            case "Groups":
                                var groupModel = groups.Get(key);
                                if (groupModel == null)
                                {
                                    groupModel = new GroupModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        tableAlias: column.TableAlias);
                                    groups.Add(key, groupModel);
                                    ss.ClearColumnAccessControlCaches(baseModel: groupModel);
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    groupModel: groupModel);
                                break;
                            case "Registrations":
                                var registrationModel = registrations.Get(key);
                                if (registrationModel == null)
                                {
                                    registrationModel = new RegistrationModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        tableAlias: column.TableAlias);
                                    registrations.Add(key, registrationModel);
                                    ss.ClearColumnAccessControlCaches(baseModel: registrationModel);
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    registrationModel: registrationModel);
                                break;
                            case "Sites":
                                var siteModel = sites.Get(key);
                                if (siteModel == null)
                                {
                                    siteModel = new SiteModel(
                                        context: context,
                                        dataRow: dataRow,
                                        formData: editRow
                                            ? formDataSet?.FirstOrDefault(o =>
                                                o.Id == dataRow.Long("SiteId"))?.Data
                                            : null,
                                        tableAlias: column.TableAlias);
                                    sites.Add(key, siteModel);
                                    ss.ClearColumnAccessControlCaches(baseModel: siteModel);
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    siteModel: siteModel);
                                break;
                            case "SysLogs":
                                var sysLogModel = sysLogs.Get(key);
                                if (sysLogModel == null)
                                {
                                    sysLogModel = new SysLogModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        tableAlias: column.TableAlias);
                                    sysLogs.Add(key, sysLogModel);
                                    ss.ClearColumnAccessControlCaches(baseModel: sysLogModel);
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    sysLogModel: sysLogModel);
                                break;
                            case "Users":
                                var userModel = users.Get(key);
                                if (userModel == null)
                                {
                                    userModel = new UserModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        tableAlias: column.TableAlias);
                                    users.Add(key, userModel);
                                    ss.ClearColumnAccessControlCaches(baseModel: userModel);
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    userModel: userModel);
                                break;
                            case "Dashboards":
                                var dashboardModel = dashboards.Get(key);
                                if (dashboardModel == null)
                                {
                                    dashboardModel = new DashboardModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        formData: editRow
                                            ? formDataSet?.FirstOrDefault(o =>
                                                o.Id == dataRow.Long("DashboardId"))?.Data
                                            : null,
                                        tableAlias: column.TableAlias);
                                    dashboards.Add(key, dashboardModel);
                                    ss.ClearColumnAccessControlCaches(baseModel: dashboardModel);
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    dashboardModel: dashboardModel);
                                break;
                            case "Issues":
                                var issueModel = issues.Get(key);
                                if (issueModel == null)
                                {
                                    issueModel = new IssueModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        formData: editRow
                                            ? formDataSet?.FirstOrDefault(o =>
                                                o.Id == dataRow.Long("IssueId"))?.Data
                                            : null,
                                        tableAlias: column.TableAlias);
                                    issues.Add(key, issueModel);
                                    ss.ClearColumnAccessControlCaches(baseModel: issueModel);
                                }
                                if (!issueModel.Locked
                                    && !issueModel.ReadOnly
                                    && !isHistory
                                    && EditColumn(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: issueModel.Mine(context: context))
                                    && column.CanEdit(
                                        context: context,
                                        ss: ss,
                                        baseModel: issueModel))
                                {
                                    hb.Td(
                                        css: column.TextAlign switch
                                            {
                                                SiteSettings.TextAlignTypes.Right => " right-align",
                                                SiteSettings.TextAlignTypes.Center => " center-align",
                                                _ => string.Empty
                                            },
                                        action: () => hb.Field(
                                            context: context,
                                            column: column,
                                            issueModel: issueModel,
                                            ss: column.SiteSettings,
                                            controlOnly: true,
                                            idSuffix: issueModel.IdSuffix()));
                                }
                                else if (column.ColumnName.Contains("~")
                                    && !Permissions.CanRead(
                                        context: context,
                                        siteId: issueModel.SiteId,
                                        id: issueModel.IssueId))
                                {
                                    hb.Td(
                                        context: context,
                                        column: column,
                                        value: string.Empty,
                                        tabIndex: null,
                                        serverScriptModelColumn: serverScriptModelColumn);
                                }
                                else
                                {
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        issueModel: issueModel,
                                        serverScriptModelColumn: serverScriptModelColumn);
                                }
                                break;
                            case "Results":
                                var resultModel = results.Get(key);
                                if (resultModel == null)
                                {
                                    resultModel = new ResultModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        formData: editRow
                                            ? formDataSet?.FirstOrDefault(o =>
                                                o.Id == dataRow.Long("ResultId"))?.Data
                                            : null,
                                        tableAlias: column.TableAlias);
                                    results.Add(key, resultModel);
                                    ss.ClearColumnAccessControlCaches(baseModel: resultModel);
                                }
                                if (!resultModel.Locked
                                    && !resultModel.ReadOnly
                                    && !isHistory
                                    && EditColumn(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: resultModel.Mine(context: context))
                                    && column.CanEdit(
                                        context: context,
                                        ss: ss,
                                        baseModel: resultModel))
                                {
                                    hb.Td(
                                        css: column.TextAlign switch
                                            {
                                                SiteSettings.TextAlignTypes.Right => " right-align",
                                                SiteSettings.TextAlignTypes.Center => " center-align",
                                                _ => string.Empty
                                            },
                                        action: () => hb.Field(
                                            context: context,
                                            column: column,
                                            resultModel: resultModel,
                                            ss: column.SiteSettings,
                                            controlOnly: true,
                                            idSuffix: resultModel.IdSuffix()));
                                }
                                else if (column.ColumnName.Contains("~")
                                    && !Permissions.CanRead(
                                        context: context,
                                        siteId: resultModel.SiteId,
                                        id: resultModel.ResultId))
                                {
                                    hb.Td(
                                        context: context,
                                        column: column,
                                        value: string.Empty,
                                        tabIndex: null,
                                        serverScriptModelColumn: serverScriptModelColumn);
                                }
                                else
                                {
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        resultModel: resultModel,
                                        serverScriptModelColumn: serverScriptModelColumn);
                                }
                                break;
                        }
                    });
                });
        }

        private static bool EditColumn(
            Context context,
            SiteSettings ss,
            Column column,
            List<string> mine)
        {
            switch (column.ColumnName)
            {
                case "TitleBody":
                case "Creator":
                case "Updator":
                case "CreatedTime":
                case "UpdatedTime":
                case "Comments":
                case "SiteId":
                case "SiteTitle":
                    return false;
            }
            if (!column.CanUpdate(
                context: context,
                ss: ss,
                mine: mine))
            {
                return false;
            }
            if (column.GetEditorReadOnly())
            {
                return false;
            }
            if (column.Joined)
            {
                return false;
            }
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return false;
            }
            return context.Forms.Bool("EditOnGrid");
        }
    }
}
