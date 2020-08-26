using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlGrids
    {
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
            string action = "GridRows")
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
                        if (sort)
                        {
                            hb.Th(
                                css: column.CellCss(css: "sortable"),
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
                                                .Text(text: Displays.Get(
                                                    context: context,
                                                    id: column.GridLabelText)))
                                            .SortIcon(
                                                view: view,
                                                key: column.ColumnName)));
                        }
                        else
                        {
                            hb.Th(
                                css: column.CellCss(),
                                action: () => hb
                                    .Text(text: Displays.Get(
                                        context: context,
                                        id: column.GridLabelText)));
                        }
                    });
                });
        }

        public static HtmlBuilder GridHeaderMenus(
           this HtmlBuilder hb,
           Context context,
           SiteSettings ss,
           View view,
           IEnumerable<Column> columns)
        {
            return hb.Div(id: "GridHeaderMenus", action: () =>
                columns.ForEach((column) => hb
                    .Ul(
                        id: "GridHeaderMenu__" + column.ColumnName,
                        attributes: new HtmlAttributes()
                            .Class("menu-sort")
                            .Add("data-target", $"[data-id='ViewSorters__{column.ColumnName}']")
                            .Add("style", "display: none; position: absolute;"),
                        action: () => hb
                            .Li(
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
                                    .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-triangle-1-n"))
                                    .Span(action: () => hb.Text(Displays.OrderAsc(context)))
                                )
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("sort")
                                    .Add("data-order-type", "desc"),
                                action: () => hb
                                    .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-triangle-1-s"))
                                    .Span(action: () => hb.Text(Displays.OrderDesc(context)))
                                )
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("sort")
                                    .Add("data-order-type", "release"),
                                action: () => hb
                                    .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-close"))
                                    .Span(action: () => hb.Text(Displays.OrderRelease(context)))
                                )
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("reset"),
                                action: () => hb
                                    .Span(attributes: new HtmlAttributes().Class("ui-icon ui-icon-power"))
                                    .Span(action: () => hb.Text(Displays.ResetOrder(context)))))));
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
            IEnumerable<Column> columns,
            EnumerableRowCollection<DataRow> dataRows,
            FormDataSet formDataSet = null,
            RecordSelector recordSelector = null,
            bool editRow = false,
            bool checkRow = true)
        {
            dataRows.ForEach(dataRow =>
                hb.Tr(
                    context: context,
                    ss: ss,
                    dataRow: dataRow,
                    columns: columns,
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
            DataRow dataRow,
            IEnumerable<Column> columns,
            bool editRow,
            bool checkRow,
            string idColumn,
            RecordSelector recordSelector = null,
            FormDataSet formDataSet = null)
        {
            var dataId = dataRow.Long(idColumn);
            var dataVersion = dataRow.Int("Ver");
            var isHistory = dataRow.Bool("IsHistory");
            var EditColumns = !isHistory
                ? columns.ToDictionary(
                    column => column.ColumnName,
                    column => EditColumn(
                        context: context,
                        column: column))
                : new Dictionary<string, bool>();
            return hb.Tr(
                attributes: new HtmlAttributes()
                    .Class("grid-row")
                    .DataId(dataId.ToString())
                    .DataVer(dataVersion)
                    .DataLatest(1, _using: !isHistory)
                    .Add(name: "data-history", value: "1", _using: isHistory)
                    .Add(name: "data-locked", value: "1", _using: dataRow.Bool("Locked")),
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
                    var depts = new Dictionary<string, DeptModel>();
                    var groups = new Dictionary<string, GroupModel>();
                    var registrations = new Dictionary<string, RegistrationModel>();
                    var users = new Dictionary<string, UserModel>();
                    var sites = new Dictionary<string, SiteModel>();
                    var issues = new Dictionary<string, IssueModel>();
                    var results = new Dictionary<string, ResultModel>();
                    columns.ForEach(column =>
                    {
                        var key = column.TableName();
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
                                    ss.SetColumnAccessControls(
                                        context: context,
                                        mine: deptModel.Mine(context: context));
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
                                    ss.SetColumnAccessControls(
                                        context: context,
                                        mine: groupModel.Mine(context: context));
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
                                    ss.SetColumnAccessControls(
                                        context: context,
                                        mine: registrationModel.Mine(context: context));
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    registrationModel: registrationModel);
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
                                    ss.SetColumnAccessControls(
                                        context: context,
                                        mine: userModel.Mine(context: context));
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    userModel: userModel);
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
                                    ss.SetColumnAccessControls(
                                        context: context,
                                        mine: siteModel.Mine(context: context));
                                }
                                hb.TdValue(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    siteModel: siteModel);
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
                                    ss.SetColumnAccessControls(
                                        context: context,
                                        mine: issueModel.Mine(context: context));
                                }
                                if (!issueModel.Locked && EditColumns.Get(column.ColumnName))
                                {
                                    hb.Td(
                                        css: column.TextAlign == SiteSettings.TextAlignTypes.Right
                                            ? " right-align"
                                            : string.Empty,
                                        action: () => hb.Field(
                                            context: context,
                                            column: column,
                                            issueModel: issueModel,
                                            ss: column.SiteSettings,
                                            controlOnly: true,
                                            idSuffix: issueModel.IdSuffix()));
                                }
                                else
                                {
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        issueModel: issueModel);
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
                                    ss.SetColumnAccessControls(
                                        context: context,
                                        mine: resultModel.Mine(context: context));
                                }
                                if (!resultModel.Locked && EditColumns.Get(column.ColumnName))
                                {
                                    hb.Td(
                                        css: column.TextAlign == SiteSettings.TextAlignTypes.Right
                                            ? " right-align"
                                            : string.Empty,
                                        action: () => hb.Field(
                                            context: context,
                                            column: column,
                                            resultModel: resultModel,
                                            ss: column.SiteSettings,
                                            controlOnly: true,
                                            idSuffix: resultModel.IdSuffix()));
                                }
                                else
                                {
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        resultModel: resultModel);
                                }
                                break;
                        }
                    });
                });
        }

        private static bool EditColumn(Context context, Column column)
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
            if (!column.CanUpdate)
            {
                return false;
            }
            if (column.EditorReadOnly == true)
            {
                return false;
            }
            if (column.Joined)
            {
                return false;
            }
            if (column.TypeCs == "Attachments")
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
