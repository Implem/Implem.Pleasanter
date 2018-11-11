using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class UserUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(Context context, SiteSettings ss)
        {
            var invalid = UserValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.Template(
                context: context,
                ss: ss,
                view: view,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Users",
                script: JavaScripts.ViewMode(viewMode),
                title: Displays.Users(context: context) + " - " + Displays.List(context: context),
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("UserForm")
                                .Class("main-form")
                                .Action(Locations.Action(
                                    context: context,
                                    controller: "Users")),
                            action: () => hb
                                .ViewFilters(
                                    context: context,
                                    ss: ss,
                                    view: view)
                                .Aggregations(
                                    context: context,
                                    ss: ss,
                                    aggregations: gridData.Aggregations)
                                .Div(id: "ViewModeContainer", action: () => hb
                                    .Grid(
                                        context: context,
                                        ss: ss,
                                        gridData: gridData,
                                        view: view))
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    siteId: ss.SiteId,
                                    verType: Versions.VerTypes.Latest)
                                .Div(css: "margin-bottom")
                                .Hidden(
                                    controlId: "TableName",
                                    value: "Users")
                                .Hidden(
                                    controlId: "BaseUrl",
                                    value: Locations.BaseUrl(context: context))
                                .Hidden(
                                    controlId: "GridOffset",
                                    value: Parameters.General.GridPageSize.ToString()))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ImportSettingsDialog")
                            .Class("dialog")
                            .Title(Displays.Import(context: context)))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ExportSettingsDialog")
                            .Class("dialog")
                            .Title(Displays.ExportSettings(context: context)));
                }).ToString();
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            var invalid = UserValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            return hb.Template(
                context: context,
                ss: ss,
                view: view,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Users",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(context: context),
                userStyle: ss.ViewModeStyles(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("UsersForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(
                                context: context,
                                id: ss.SiteId)),
                        action: () => hb
                            .ViewSelector(context: context, ss: ss, view: view)
                            .ViewFilters(context: context, ss: ss, view: view)
                            .Aggregations(
                                context: context,
                                ss: ss,
                                aggregations: gridData.Aggregations)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")
                            .Hidden(
                                controlId: "TableName",
                                value: "Users")
                            .Hidden(
                                controlId: "BaseUrl",
                                value: Locations.BaseUrl(context: context)))
                    .MoveDialog(context: context, bulk: true)
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.Export(context: context))))
                    .ToString();
        }

        public static string IndexJson(Context context, SiteSettings ss)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            return new ResponseCollection()
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    gridData: gridData,
                    invoke: "setGrid",
                    body: new HtmlBuilder()
                        .Grid(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static GridData GetGridData(
            Context context, SiteSettings ss, View view, int offset = 0)
        {
            ss.SetColumnAccessControls(context: context);
            return new GridData(
                context: context,
                ss: ss,
                view: view,
                join: Rds.UsersJoinDefault(),
                where: Rds.UsersWhere().TenantId(context.TenantId),
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregations: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            string action = "GridRows")
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class(ss.GridCss())
                        .DataValue("back", _using: ss?.IntegratedSites?.Any() == true)
                        .DataAction(action)
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view,
                            action: action))
                .Hidden(
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        gridData.DataRows.Count(),
                        gridData.Aggregations.TotalCount)
                            .ToString())
                .Button(
                    controlId: "ViewSorter",
                    controlCss: "hidden",
                    action: action,
                    method: "post")
                .Button(
                    controlId: "ViewSorters_Reset",
                    controlCss: "hidden",
                    action: action,
                    method: "post");
        }

        public static string GridRows(
            Context context,
            SiteSettings ss,
            ResponseCollection res = null,
            int offset = 0,
            bool clearCheck = false,
            string action = "GridRows",
            Message message = null)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(
                context: context,
                ss: ss,
                view: view,
                offset: offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .CloseDialog()
                .ReplaceAll("#CopyDirectUrlToClipboard", new HtmlBuilder()
                    .CopyDirectUrlToClipboard(
                        context: context,
                        view: view))
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    context: context,
                    ss: ss,
                    aggregations: gridData.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    context: context,
                    ss: ss,
                    gridData: gridData,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck,
                    action: action))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    gridData.DataRows.Count(),
                    gridData.Aggregations.TotalCount))
                .Paging("#Grid")
                .Message(message)
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            bool addHeader = true,
            bool clearCheck = false,
            string action = "GridRows")
        {
            var checkAll = clearCheck
                ? false
                : context.Forms.Bool("GridCheckAll");
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            context: context,
                            columns: columns, 
                            view: view,
                            checkAll: checkAll,
                            action: action))
                .TBody(action: () => gridData.TBody(
                    hb: hb,
                    context: context,
                    ss: ss,
                    columns: columns,
                    checkAll: checkAll));
        }

        private static SqlColumnCollection GridSqlColumnCollection(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.UsersColumn();
            new List<string> { "UserId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.UsersColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.UsersColumn();
            new List<string> { "UserId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.UsersColumn(column));
            return sqlColumnCollection;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            UserModel userModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
                    userModel: userModel);
            }
            else
            {
                var mine = userModel.Mine(context: context);
                switch (column.Name)
                {
                    case "UserId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.UserId)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Ver)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "LoginId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LoginId)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Name":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Name)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "UserCode":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.UserCode)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Birthday":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Birthday)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Gender":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Gender)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Language":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Language)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "TimeZoneInfo":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.TimeZoneInfo)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DeptCode":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DeptCode)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Dept":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Dept)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Body)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "LastLoginTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LastLoginTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "PasswordExpirationTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.PasswordExpirationTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "PasswordChangeTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.PasswordChangeTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumberOfLogins":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumberOfLogins)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumberOfDenial":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumberOfDenial)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "TenantManager":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.TenantManager)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Disabled":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Disabled)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.ClassZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DateZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DescriptionZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CheckZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "LdapSearchRoot":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LdapSearchRoot)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "SynchronizedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.SynchronizedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Comments)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Creator)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Updator)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.CreatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string gridDesign,
            UserModel userModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "UserId": value = userModel.UserId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = userModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "LoginId": value = userModel.LoginId.GridText(
                        context: context,
                        column: column); break;
                    case "Name": value = userModel.Name.GridText(
                        context: context,
                        column: column); break;
                    case "UserCode": value = userModel.UserCode.GridText(
                        context: context,
                        column: column); break;
                    case "Birthday": value = userModel.Birthday.GridText(
                        context: context,
                        column: column); break;
                    case "Gender": value = userModel.Gender.GridText(
                        context: context,
                        column: column); break;
                    case "Language": value = userModel.Language.GridText(
                        context: context,
                        column: column); break;
                    case "TimeZoneInfo": value = userModel.TimeZoneInfo.GridText(
                        context: context,
                        column: column); break;
                    case "DeptCode": value = userModel.DeptCode.GridText(
                        context: context,
                        column: column); break;
                    case "Dept": value = userModel.Dept.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = userModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "LastLoginTime": value = userModel.LastLoginTime.GridText(
                        context: context,
                        column: column); break;
                    case "PasswordExpirationTime": value = userModel.PasswordExpirationTime.GridText(
                        context: context,
                        column: column); break;
                    case "PasswordChangeTime": value = userModel.PasswordChangeTime.GridText(
                        context: context,
                        column: column); break;
                    case "NumberOfLogins": value = userModel.NumberOfLogins.GridText(
                        context: context,
                        column: column); break;
                    case "NumberOfDenial": value = userModel.NumberOfDenial.GridText(
                        context: context,
                        column: column); break;
                    case "TenantManager": value = userModel.TenantManager.GridText(
                        context: context,
                        column: column); break;
                    case "Disabled": value = userModel.Disabled.GridText(
                        context: context,
                        column: column); break;
                    case "ClassA": value = userModel.ClassA.GridText(
                        context: context,
                        column: column); break;
                    case "ClassB": value = userModel.ClassB.GridText(
                        context: context,
                        column: column); break;
                    case "ClassC": value = userModel.ClassC.GridText(
                        context: context,
                        column: column); break;
                    case "ClassD": value = userModel.ClassD.GridText(
                        context: context,
                        column: column); break;
                    case "ClassE": value = userModel.ClassE.GridText(
                        context: context,
                        column: column); break;
                    case "ClassF": value = userModel.ClassF.GridText(
                        context: context,
                        column: column); break;
                    case "ClassG": value = userModel.ClassG.GridText(
                        context: context,
                        column: column); break;
                    case "ClassH": value = userModel.ClassH.GridText(
                        context: context,
                        column: column); break;
                    case "ClassI": value = userModel.ClassI.GridText(
                        context: context,
                        column: column); break;
                    case "ClassJ": value = userModel.ClassJ.GridText(
                        context: context,
                        column: column); break;
                    case "ClassK": value = userModel.ClassK.GridText(
                        context: context,
                        column: column); break;
                    case "ClassL": value = userModel.ClassL.GridText(
                        context: context,
                        column: column); break;
                    case "ClassM": value = userModel.ClassM.GridText(
                        context: context,
                        column: column); break;
                    case "ClassN": value = userModel.ClassN.GridText(
                        context: context,
                        column: column); break;
                    case "ClassO": value = userModel.ClassO.GridText(
                        context: context,
                        column: column); break;
                    case "ClassP": value = userModel.ClassP.GridText(
                        context: context,
                        column: column); break;
                    case "ClassQ": value = userModel.ClassQ.GridText(
                        context: context,
                        column: column); break;
                    case "ClassR": value = userModel.ClassR.GridText(
                        context: context,
                        column: column); break;
                    case "ClassS": value = userModel.ClassS.GridText(
                        context: context,
                        column: column); break;
                    case "ClassT": value = userModel.ClassT.GridText(
                        context: context,
                        column: column); break;
                    case "ClassU": value = userModel.ClassU.GridText(
                        context: context,
                        column: column); break;
                    case "ClassV": value = userModel.ClassV.GridText(
                        context: context,
                        column: column); break;
                    case "ClassW": value = userModel.ClassW.GridText(
                        context: context,
                        column: column); break;
                    case "ClassX": value = userModel.ClassX.GridText(
                        context: context,
                        column: column); break;
                    case "ClassY": value = userModel.ClassY.GridText(
                        context: context,
                        column: column); break;
                    case "ClassZ": value = userModel.ClassZ.GridText(
                        context: context,
                        column: column); break;
                    case "NumA": value = userModel.NumA.GridText(
                        context: context,
                        column: column); break;
                    case "NumB": value = userModel.NumB.GridText(
                        context: context,
                        column: column); break;
                    case "NumC": value = userModel.NumC.GridText(
                        context: context,
                        column: column); break;
                    case "NumD": value = userModel.NumD.GridText(
                        context: context,
                        column: column); break;
                    case "NumE": value = userModel.NumE.GridText(
                        context: context,
                        column: column); break;
                    case "NumF": value = userModel.NumF.GridText(
                        context: context,
                        column: column); break;
                    case "NumG": value = userModel.NumG.GridText(
                        context: context,
                        column: column); break;
                    case "NumH": value = userModel.NumH.GridText(
                        context: context,
                        column: column); break;
                    case "NumI": value = userModel.NumI.GridText(
                        context: context,
                        column: column); break;
                    case "NumJ": value = userModel.NumJ.GridText(
                        context: context,
                        column: column); break;
                    case "NumK": value = userModel.NumK.GridText(
                        context: context,
                        column: column); break;
                    case "NumL": value = userModel.NumL.GridText(
                        context: context,
                        column: column); break;
                    case "NumM": value = userModel.NumM.GridText(
                        context: context,
                        column: column); break;
                    case "NumN": value = userModel.NumN.GridText(
                        context: context,
                        column: column); break;
                    case "NumO": value = userModel.NumO.GridText(
                        context: context,
                        column: column); break;
                    case "NumP": value = userModel.NumP.GridText(
                        context: context,
                        column: column); break;
                    case "NumQ": value = userModel.NumQ.GridText(
                        context: context,
                        column: column); break;
                    case "NumR": value = userModel.NumR.GridText(
                        context: context,
                        column: column); break;
                    case "NumS": value = userModel.NumS.GridText(
                        context: context,
                        column: column); break;
                    case "NumT": value = userModel.NumT.GridText(
                        context: context,
                        column: column); break;
                    case "NumU": value = userModel.NumU.GridText(
                        context: context,
                        column: column); break;
                    case "NumV": value = userModel.NumV.GridText(
                        context: context,
                        column: column); break;
                    case "NumW": value = userModel.NumW.GridText(
                        context: context,
                        column: column); break;
                    case "NumX": value = userModel.NumX.GridText(
                        context: context,
                        column: column); break;
                    case "NumY": value = userModel.NumY.GridText(
                        context: context,
                        column: column); break;
                    case "NumZ": value = userModel.NumZ.GridText(
                        context: context,
                        column: column); break;
                    case "DateA": value = userModel.DateA.GridText(
                        context: context,
                        column: column); break;
                    case "DateB": value = userModel.DateB.GridText(
                        context: context,
                        column: column); break;
                    case "DateC": value = userModel.DateC.GridText(
                        context: context,
                        column: column); break;
                    case "DateD": value = userModel.DateD.GridText(
                        context: context,
                        column: column); break;
                    case "DateE": value = userModel.DateE.GridText(
                        context: context,
                        column: column); break;
                    case "DateF": value = userModel.DateF.GridText(
                        context: context,
                        column: column); break;
                    case "DateG": value = userModel.DateG.GridText(
                        context: context,
                        column: column); break;
                    case "DateH": value = userModel.DateH.GridText(
                        context: context,
                        column: column); break;
                    case "DateI": value = userModel.DateI.GridText(
                        context: context,
                        column: column); break;
                    case "DateJ": value = userModel.DateJ.GridText(
                        context: context,
                        column: column); break;
                    case "DateK": value = userModel.DateK.GridText(
                        context: context,
                        column: column); break;
                    case "DateL": value = userModel.DateL.GridText(
                        context: context,
                        column: column); break;
                    case "DateM": value = userModel.DateM.GridText(
                        context: context,
                        column: column); break;
                    case "DateN": value = userModel.DateN.GridText(
                        context: context,
                        column: column); break;
                    case "DateO": value = userModel.DateO.GridText(
                        context: context,
                        column: column); break;
                    case "DateP": value = userModel.DateP.GridText(
                        context: context,
                        column: column); break;
                    case "DateQ": value = userModel.DateQ.GridText(
                        context: context,
                        column: column); break;
                    case "DateR": value = userModel.DateR.GridText(
                        context: context,
                        column: column); break;
                    case "DateS": value = userModel.DateS.GridText(
                        context: context,
                        column: column); break;
                    case "DateT": value = userModel.DateT.GridText(
                        context: context,
                        column: column); break;
                    case "DateU": value = userModel.DateU.GridText(
                        context: context,
                        column: column); break;
                    case "DateV": value = userModel.DateV.GridText(
                        context: context,
                        column: column); break;
                    case "DateW": value = userModel.DateW.GridText(
                        context: context,
                        column: column); break;
                    case "DateX": value = userModel.DateX.GridText(
                        context: context,
                        column: column); break;
                    case "DateY": value = userModel.DateY.GridText(
                        context: context,
                        column: column); break;
                    case "DateZ": value = userModel.DateZ.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionA": value = userModel.DescriptionA.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionB": value = userModel.DescriptionB.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionC": value = userModel.DescriptionC.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionD": value = userModel.DescriptionD.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionE": value = userModel.DescriptionE.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionF": value = userModel.DescriptionF.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionG": value = userModel.DescriptionG.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionH": value = userModel.DescriptionH.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionI": value = userModel.DescriptionI.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionJ": value = userModel.DescriptionJ.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionK": value = userModel.DescriptionK.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionL": value = userModel.DescriptionL.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionM": value = userModel.DescriptionM.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionN": value = userModel.DescriptionN.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionO": value = userModel.DescriptionO.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionP": value = userModel.DescriptionP.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionQ": value = userModel.DescriptionQ.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionR": value = userModel.DescriptionR.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionS": value = userModel.DescriptionS.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionT": value = userModel.DescriptionT.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionU": value = userModel.DescriptionU.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionV": value = userModel.DescriptionV.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionW": value = userModel.DescriptionW.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionX": value = userModel.DescriptionX.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionY": value = userModel.DescriptionY.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionZ": value = userModel.DescriptionZ.GridText(
                        context: context,
                        column: column); break;
                    case "CheckA": value = userModel.CheckA.GridText(
                        context: context,
                        column: column); break;
                    case "CheckB": value = userModel.CheckB.GridText(
                        context: context,
                        column: column); break;
                    case "CheckC": value = userModel.CheckC.GridText(
                        context: context,
                        column: column); break;
                    case "CheckD": value = userModel.CheckD.GridText(
                        context: context,
                        column: column); break;
                    case "CheckE": value = userModel.CheckE.GridText(
                        context: context,
                        column: column); break;
                    case "CheckF": value = userModel.CheckF.GridText(
                        context: context,
                        column: column); break;
                    case "CheckG": value = userModel.CheckG.GridText(
                        context: context,
                        column: column); break;
                    case "CheckH": value = userModel.CheckH.GridText(
                        context: context,
                        column: column); break;
                    case "CheckI": value = userModel.CheckI.GridText(
                        context: context,
                        column: column); break;
                    case "CheckJ": value = userModel.CheckJ.GridText(
                        context: context,
                        column: column); break;
                    case "CheckK": value = userModel.CheckK.GridText(
                        context: context,
                        column: column); break;
                    case "CheckL": value = userModel.CheckL.GridText(
                        context: context,
                        column: column); break;
                    case "CheckM": value = userModel.CheckM.GridText(
                        context: context,
                        column: column); break;
                    case "CheckN": value = userModel.CheckN.GridText(
                        context: context,
                        column: column); break;
                    case "CheckO": value = userModel.CheckO.GridText(
                        context: context,
                        column: column); break;
                    case "CheckP": value = userModel.CheckP.GridText(
                        context: context,
                        column: column); break;
                    case "CheckQ": value = userModel.CheckQ.GridText(
                        context: context,
                        column: column); break;
                    case "CheckR": value = userModel.CheckR.GridText(
                        context: context,
                        column: column); break;
                    case "CheckS": value = userModel.CheckS.GridText(
                        context: context,
                        column: column); break;
                    case "CheckT": value = userModel.CheckT.GridText(
                        context: context,
                        column: column); break;
                    case "CheckU": value = userModel.CheckU.GridText(
                        context: context,
                        column: column); break;
                    case "CheckV": value = userModel.CheckV.GridText(
                        context: context,
                        column: column); break;
                    case "CheckW": value = userModel.CheckW.GridText(
                        context: context,
                        column: column); break;
                    case "CheckX": value = userModel.CheckX.GridText(
                        context: context,
                        column: column); break;
                    case "CheckY": value = userModel.CheckY.GridText(
                        context: context,
                        column: column); break;
                    case "CheckZ": value = userModel.CheckZ.GridText(
                        context: context,
                        column: column); break;
                    case "LdapSearchRoot": value = userModel.LdapSearchRoot.GridText(
                        context: context,
                        column: column); break;
                    case "SynchronizedTime": value = userModel.SynchronizedTime.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = userModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = userModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = userModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = userModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = userModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorNew(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.UsersLimit(context: context))
            {
                return HtmlTemplates.Error(context, Error.Types.UsersLimit);
            }
            return Editor(context: context, ss: ss, userModel: new UserModel(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, int userId, bool clearSessions)
        {
            var userModel = new UserModel(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: userId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            userModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: userModel.UserId);
            return Editor(context: context, ss: ss, userModel: userModel);
        }

        public static string Editor(
            Context context, SiteSettings ss, UserModel userModel)
        {
            var invalid = UserValidators.OnEditing(
                context: context,
                ss: ss,
                userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(
                context: context,
                mine: userModel.Mine(context: context));
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                verType: userModel.VerType,
                methodType: userModel.MethodType,
                referenceType: "Users",
                title: userModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Users(context: context) + " - " + Displays.New(context: context)
                    : userModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            context: context,
                            ss: ss,
                            userModel: userModel)
                        .Hidden(controlId: "TableName", value: "Users")
                        .Hidden(controlId: "Id", value: userModel.UserId.ToString());
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, UserModel userModel)
        {
            var commentsColumn = ss.GetColumn(
                context: context,
                columnName: "Comments");
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("UserForm")
                        .Class("main-form confirm-reload")
                        .Action(userModel.UserId != 0
                            ? Locations.Action(
                                context: context,
                                controller: "Users",
                                id: userModel.UserId)
                            : Locations.Action(
                                context: context,
                                controller: "Users")),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: userModel,
                            tableName: "Users")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: userModel.Comments,
                                    column: commentsColumn,
                                    verType: userModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(context: context, userModel: userModel)
                            .FieldSetGeneral(
                                context: context,
                                ss: ss,
                                userModel: userModel)
                            .FieldSetMailAddresses(
                                context: context,
                                userModel: userModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: userModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: 0,
                                verType: userModel.VerType,
                                referenceId: userModel.UserId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        context: context,
                                        userModel: userModel,
                                        ss: ss)))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "MethodType",
                            value: userModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Users_Timestamp",
                            css: "always-send",
                            value: userModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: userModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Users",
                    referenceId: userModel.UserId,
                    referenceVer: userModel.Ver)
                .CopyDialog(
                    context: context,
                    referenceType: "Users",
                    id: userModel.UserId)
                .OutgoingMailDialog()
                .EditorExtensions(
                    context: context,
                    ss: ss,
                    userModel: userModel));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, UserModel userModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetMailAddresses",
                        text: Displays.MailAddresses(context: context),
                        _using: userModel.MethodType != BaseModel.MethodTypes.New))
                .Li(
                    _using: userModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context))));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            UserModel userModel)
        {
            var mine = userModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context, ss: ss, userModel: userModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            UserModel userModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
            {
                switch (column.Name)
                {
                    case "UserId":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.UserId
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.Ver
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "LoginId":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.LoginId
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Name":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.Name
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "UserCode":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.UserCode
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Password":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.Password
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "PasswordValidate":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.PasswordValidate
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "PasswordDummy":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.PasswordDummy
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "RememberMe":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.RememberMe
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Birthday":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.Birthday
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Gender":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.Gender
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Language":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.Language
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "TimeZone":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.TimeZone
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DeptId":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DeptId
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.Body
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "LastLoginTime":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.LastLoginTime
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "PasswordExpirationTime":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.PasswordExpirationTime
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "PasswordChangeTime":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.PasswordChangeTime
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumberOfLogins":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumberOfLogins
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumberOfDenial":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumberOfDenial
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "TenantManager":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.TenantManager
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Disabled":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.Disabled
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "OldPassword":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.OldPassword
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ChangedPassword":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ChangedPassword
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ChangedPasswordValidator":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ChangedPasswordValidator
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AfterResetPassword":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.AfterResetPassword
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AfterResetPasswordValidator":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.AfterResetPasswordValidator
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DemoMailAddress":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DemoMailAddress
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.ClassZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.NumZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DateZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.DescriptionZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.CheckZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "LdapSearchRoot":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.LdapSearchRoot
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "SynchronizedTime":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: userModel.MethodType,
                            value: userModel.SynchronizedTime
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                }
            });
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: userModel);
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            UserModel userModel)
        {
            if (userModel.VerType == Versions.VerTypes.Latest &&
                userModel.MethodType != BaseModel.MethodTypes.New &&
                Rds.ExecuteScalar_bool(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
                            .UserId(userModel.UserId)
                            .Password(_operator: "is not null"))))
            {
                if (userModel.Self(context: context))
                {
                    hb.Button(
                        text: Displays.ChangePassword(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openDialog($(this));",
                        icon: "ui-icon-person",
                        selector: "#ChangePasswordDialog");
                }
                if (context.User?.TenantManager == true)
                {
                    hb.Button(
                        text: Displays.ResetPassword(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openDialog($(this));",
                        icon: "ui-icon-person",
                        selector: "#ResetPasswordDialog");
                }
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            UserModel userModel)
        {
            return hb
                .ChangePasswordDialog(
                    context: context,
                    ss: ss,
                    userId: userModel.UserId)
                .ResetPasswordDialog(
                    context: context,
                    ss: ss,
                    userId: userModel.UserId);
        }

        public static string EditorJson(Context context, SiteSettings ss, int userId)
        {
            return EditorResponse(context, ss, new UserModel(
                context, ss, userId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            UserModel userModel,
            Message message = null,
            string switchTargets = null)
        {
            userModel.MethodType = BaseModel.MethodTypes.Edit;
            return new UsersResponseCollection(userModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, ss, userModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int userId)
        {
            if (Permissions.CanManageTenant(context: context))
            {
                var view = Views.GetBySession(context: context, ss: ss);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: Rds.UsersWhere().TenantId(context.TenantId));
                var switchTargets = Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: where)) <= Parameters.General.SwitchTargetsLimit
                            ? Rds.ExecuteTable(
                                context: context,
                                statements: Rds.SelectUsers(
                                    column: Rds.UsersColumn().UserId(),
                                    where: where,
                                    orderBy: view.OrderBy(
                                        context: context,
                                        ss: ss,
                                        orderBy: Rds.UsersOrderBy()
                                            .UpdatedTime(SqlOrderBy.Types.desc))))
                                            .AsEnumerable()
                                            .Select(dataRow => dataRow.Int("UserId"))
                                            .ToList()
                            : new List<int>();
                if (!switchTargets.Contains(userId))
                {
                    switchTargets.Add(userId);
                }
                return switchTargets;
            }
            else
            {
                return new List<int> { userId };
            }
        }

        public static string Create(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.UsersLimit(context: context))
            {
                return Error.Types.UsersLimit.MessageJson(context: context);
            }
            var userModel = new UserModel(context, ss, 0, setByForm: true);
            var invalid = UserValidators.OnCreating(
                context: context,
                ss: ss,
                userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = userModel.Create(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: userModel.Title.Value));
                    return new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : userModel.UserId))
                        .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Update(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(
                context: context, ss: ss, userId: userId, setByForm: true);
            var invalid = UserValidators.OnUpdating(
                context: context,
                ss: ss,
                userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                case Error.Types.PermissionNotSelfChange:
                    return Messages.ResponsePermissionNotSelfChange(context: context)
                        .Val("#Users_TenantManager", userModel.SavedTenantManager)
                        .ClearFormData("Users_TenantManager")
                        .ToJson();
                default: return invalid.MessageJson(context: context);
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var error = userModel.Update(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    var res = new UsersResponseCollection(userModel);
                    return ResponseByUpdate(res, context, ss, userModel)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: userModel.Comments,
                            verType: userModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: userModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            UsersResponseCollection res,
            Context context,
            SiteSettings ss,
            UserModel userModel)
        {
            if (context.Forms.Bool("IsDialogEditorForm"))
            {
                var view = Views.GetBySession(
                    context: context,
                    ss: ss);
                var gridData = new GridData(
                    context: context,
                    ss: ss,
                    view: view,
                    where: Rds.UsersWhere().UserId(userModel.UserId));
                var columns = ss.GetGridColumns(
                    context: context,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{userModel.UserId}\"]",
                        gridData.TBody(
                            hb: new HtmlBuilder(),
                            context: context,
                            ss: ss,
                            columns: columns,
                            checkAll: false))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: userModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", userModel.Title.Value)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: userModel,
                        tableName: "Users"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: userModel.Title.Value))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: userModel.Comments,
                        deleteCommentId: userModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        public static string Delete(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(context, ss, userId);
            var invalid = UserValidators.OnDeleting(
                context: context,
                ss: ss,
                userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = userModel.Delete(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: userModel.Title.Value));
                    var res = new UsersResponseCollection(userModel);
                    res
                        .SetMemory("formChanged", false)
                        .Href(Locations.Index(
                            context: context,
                            controller: "Users"));
                    return res.ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int userId, Message message = null)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: userId);
            ss.SetColumnAccessControls(
                context: context,
                mine: userModel.Mine(context: context));
            var columns = ss.GetHistoryColumns(context: context, checkPermission: true);
            if (!context.CanRead(ss: ss))
            {
                return Error.Types.HasNotPermission.MessageJson(context: context);
            }
            var hb = new HtmlBuilder();
            hb
                .HistoryCommands(context: context, ss: ss)
                .Table(
                    attributes: new HtmlAttributes().Class("grid history"),
                    action: () => hb
                        .THead(action: () => hb
                            .GridHeader(
                                context: context,
                                columns: columns,
                                sort: false,
                                checkRow: true))
                        .TBody(action: () => hb
                            .HistoriesTableBody(
                                context: context,
                                ss: ss,
                                columns: columns,
                                userModel: userModel)));
            return new UsersResponseCollection(userModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
                .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            UserModel userModel)
        {
            new UserCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.UsersWhere().UserId(userModel.UserId),
                orderBy: Rds.UsersOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(userModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(userModelHistory.Ver)
                                .DataLatest(1, _using:
                                    userModelHistory.Ver == userModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: userModelHistory.Ver.ToString(),
                                            _using: userModelHistory.Ver < userModel.Ver));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            userModel: userModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.UsersColumnCollection()
                .UserId()
                .Ver();
            columns.ForEach(column => sqlColumn.UsersColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: userId);
            ss.SetColumnAccessControls(
                context: context,
                mine: userModel.Mine(context: context));
            userModel.Get(
                context: context,
                ss: ss,
                where: Rds.UsersWhere()
                    .UserId(userModel.UserId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            userModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, userModel).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ChangePassword(Context context, int userId)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId,
                setByForm: true);
            var invalid = UserValidators.OnPasswordChanging(
                context: context,
                userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = userModel.ChangePassword(context: context);
            return error.Has()
                ? error.MessageJson(context: context)
                : new UsersResponseCollection(userModel)
                    .OldPassword(context: context, value: string.Empty)
                    .ChangedPassword(context: context, value: string.Empty)
                    .ChangedPasswordValidator(context: context, value: string.Empty)
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: userModel,
                        tableName: "Users"))
                    .CloseDialog()
                    .ClearFormData()
                    .Message(Messages.ChangingPasswordComplete(context: context))
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ChangePasswordAtLogin(Context context)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            var userModel = new UserModel(
                context: context,
                ss: ss,
                loginId: context.Forms.Data("Users_LoginId"),
                setByForm: true);
            var invalid = UserValidators.OnPasswordChangingAtLogin(
                context: context,
                userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = userModel.ChangePasswordAtLogin(context: context);
            return error.Has()
                ? error.MessageJson(context: context)
                : userModel.Allow(
                    context: context,
                    returnUrl: context.Forms.Data("ReturnUrl"),
                    atLogin: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ResetPassword(Context context, int userId)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId,
                setByForm: true);
            var invalid = UserValidators.OnPasswordResetting(context: context);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = userModel.ResetPassword(context: context);
            return error.Has()
                ? error.MessageJson(context: context)
                : new UsersResponseCollection(userModel)
                    .PasswordExpirationTime(context: context, ss: ss)
                    .PasswordChangeTime(context: context, ss: ss)
                    .AfterResetPassword(context: context, value: string.Empty)
                    .AfterResetPasswordValidator(context: context, value: string.Empty)
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: userModel,
                        tableName: "Users"))
                    .CloseDialog()
                    .ClearFormData()
                    .Message(Messages.PasswordResetCompleted(context: context))
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string AddMailAddresses(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId);
            var mailAddress = context.Forms.Data("MailAddress").Trim();
            var selected = context.Forms.List("MailAddresses");
            var badMailAddress = string.Empty;
            var invalid = UserValidators.OnAddingMailAddress(
                context: context,
                userModel: userModel,
                mailAddress: mailAddress,
                data: out badMailAddress);
            switch (invalid)
            {
                case Error.Types.None:
                    break;
                case Error.Types.BadMailAddress:
                    return invalid.MessageJson(
                        context: context,
                        data: badMailAddress);
                default:
                    return invalid.MessageJson(context: context);
            }
            var error = userModel.AddMailAddress(
                context: context,
                mailAddress: mailAddress,
                selected: selected);
            return error.Has()
                ? error.MessageJson(context: context)
                : ResponseMailAddresses(userModel, selected);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteMailAddresses(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId);
            var invalid = UserValidators.OnUpdating(
                context: context, ss: ss, userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var selected = context.Forms.List("MailAddresses");
            var error = userModel.DeleteMailAddresses(
                context: context,
                selected: selected);
            return error.Has()
                ? error.MessageJson(context: context)
                : ResponseMailAddresses(userModel, selected);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ResponseMailAddresses(
            UserModel userModel, IEnumerable<string> selected)
        {
            return new ResponseCollection()
                .Html(
                    "#MailAddresses",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: userModel.MailAddresses.ToDictionary(
                            o => o, o => new ControlData(o)),
                        selectedValueTextCollection: selected))
                .Val("#MailAddress", string.Empty)
                .SetMemory("formChanged", true)
                .Focus("#MailAddress")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ChangePasswordDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, long userId)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ChangePasswordDialog")
                    .Class("dialog")
                    .Title(Displays.ChangePassword(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ChangePasswordForm")
                            .Action(Locations.Action(
                                context: context,
                                controller: "Users",
                                id: userId))
                            .DataEnter("#ChangePassword"),
                        action: () => hb
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "OldPassword"))
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "ChangedPassword"))
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "ChangedPasswordValidator"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ChangePassword",
                                    text: Displays.Change(context: context),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ChangePassword",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ResetPasswordDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, long userId)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ResetPasswordDialog")
                    .Class("dialog")
                    .Title(Displays.ResetPassword(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ResetPasswordForm")
                            .Action(Locations.Action(
                                context: context,
                                controller: "Users",
                                id: userId))
                            .DataEnter("#ResetPassword"),
                        action: () => hb
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "AfterResetPassword"))
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "AfterResetPasswordValidator"))
                            .P(css: "hidden", action: () => hb
                                .TextBox(
                                    textType: HtmlTypes.TextTypes.Password,
                                    controlCss: " dummy not-send"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ResetPassword",
                                    text: Displays.Reset(context: context),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ResetPassword",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows(Context context)
        {
            return GridRows(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                offset: DataViewGrid.Offset(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string HtmlLogin(Context context, string returnUrl, string message)
        {
            var hb = new HtmlBuilder();
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                verType: Versions.VerTypes.Latest,
                useBreadcrumb: false,
                useTitle: false,
                useSearch: false,
                useNavigationMenu: false,
                methodType: BaseModel.MethodTypes.Edit,
                referenceType: "Users",
                title: string.Empty,
                action: () => hb
                    .Div(id: "PortalLink", action: () => hb
                        .A(href: Parameters.General.HtmlPortalUrl, action: () => hb
                            .Text(Displays.Portal(context: context))))
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("UserForm")
                            .Class("main-form")
                            .Action(Locations.Raw(
                                context: context,
                                parts: new string[]
                                {
                                    "users",
                                    "_action_?ReturnUrl=" + returnUrl
                                }))
                            .DataEnter("#Login"),
                        action: () => hb
                            .FieldSet(id: "LoginFieldSet", action: () => hb
                                .Div(action: () => hb
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: ss.GetColumn(
                                            context: context,
                                            columnName: "LoginId"),
                                        fieldCss: "field-wide",
                                        controlCss: " always-send focus")
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: ss.GetColumn(
                                            context: context,
                                            columnName: "Password"),
                                        fieldCss: "field-wide",
                                        controlCss: " always-send")
                                    .Div(id: "Tenants")
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: ss.GetColumn(
                                            context: context,
                                            columnName: "RememberMe")))
                                .Div(id: "LoginCommands", action: () => hb
                                    .Button(
                                        controlId: "Login",
                                        controlCss: "button-icon button-right-justified validate",
                                        text: Displays.Login(context: context),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-unlocked",
                                        action: "Authenticate",
                                        method: "post",
                                        type: "submit"))))
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("DemoForm")
                            .Action(Locations.Get(
                                context: context,
                                parts: new string[]
                                {
                                    "demos",
                                    "_action_"
                                })),
                        _using: Parameters.Service.Demo,
                        action: () => hb
                            .Div(id: "Demo", action: () => hb
                                .FieldSet(
                                    css: " enclosed-thin",
                                    legendText: Displays.ViewDemoEnvironment(context: context),
                                    action: () => hb
                                        .Div(id: "DemoFields", action: () => hb
                                            .Field(
                                                context: context,
                                                ss: ss,
                                                column: ss.GetColumn(
                                                    context: context,
                                                    columnName: "DemoMailAddress"))
                                            .Button(
                                                text: Displays.Register(context: context),
                                                controlCss: "button-icon validate",
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-mail-closed",
                                                action: "Register",
                                                method: "post")))))
                    .P(id: "Message", css: "message-form-bottom", action: () => hb.Raw(message))
                    .ChangePasswordAtLoginDialog(
                        context: context,
                        ss: ss)
                    .Hidden(
                        controlId: "ReturnUrl",
                        value: context.QueryStrings.Data("ReturnUrl")))
                    .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Expired(Context context)
        {
            return HtmlTemplates.Error(context, Error.Types.Expired);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ChangePasswordAtLoginDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ChangePasswordDialog")
                    .Class("dialog")
                    .Title(Displays.ChangePassword(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ChangePasswordForm")
                            .Action(Locations.Action(
                                context: context,
                                controller: "Users"))
                            .DataEnter("#ChangePassword"),
                        action: () => hb
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "ChangedPassword"))
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "ChangedPasswordValidator"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ChangePassword",
                                    text: Displays.Change(context: context),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.changePasswordAtLogin($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ChangePasswordAtLogin",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FieldSetMailAddresses(
            this HtmlBuilder hb, Context context, UserModel userModel)
        {
            if (userModel.MethodType == BaseModel.MethodTypes.New) return hb;
            var listItemCollection = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectMailAddresses(
                    column: Rds.MailAddressesColumn()
                        .MailAddressId()
                        .MailAddress(),
                    where: Rds.MailAddressesWhere()
                        .OwnerId(userModel.UserId)
                        .OwnerType("Users")))
                            .AsEnumerable()
                            .ToDictionary(
                                o => o["MailAddress"].ToString(),
                                o => new ControlData(o["MailAddress"].ToString()));
            userModel.Session_MailAddresses(
                context: context,
                value: listItemCollection.Keys.ToList().ToJson());
            return hb.FieldSet(id: "FieldSetMailAddresses", action: () => hb
                .FieldSelectable(
                    controlId: "MailAddresses",
                    fieldCss: "field-vertical w500",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h350",
                    labelText: Displays.MailAddresses(context: context),
                    listItemCollection: listItemCollection,
                    commandOptionAction: () => hb
                        .Div(css: "command-left", action: () => hb
                            .TextBox(
                                controlId: "MailAddress",
                                controlCss: " w200")
                            .Button(
                                text: Displays.Add(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "AddMailAddress",
                                method: "post")
                            .Button(
                                controlId: "DeleteMailAddresses",
                                controlCss: "button-icon",
                                text: Displays.Delete(context: context),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-image",
                                action: "DeleteMailAddresses",
                                method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SyncByLdap(Context context)
        {
            Ldap.Sync(context: context);
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ApiEditor(Context context, SiteSettings ss)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: context.UserId);
            var invalid = UserValidators.OnApiEditing(
                context: context,
                userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Users",
                title: Displays.ApiSettings(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("UserForm")
                            .Class("main-form")
                            .Action(Locations.Action(
                                context: context,
                                controller: "Users")),
                        action: () => hb
                            .ApiEditor(
                                context: context,
                                userModel: userModel)
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")))
                                .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ApiEditor(
            this HtmlBuilder hb, Context context, UserModel userModel)
        {
            return hb
                .Div(id: "EditorTabsContainer", css: "max", action: () => hb
                    .Ul(id: "EditorTabs", action: () => hb
                        .Li(action: () => hb
                            .A(
                                href: "#FieldSetGeneral",
                                text: Displays.General(context: context))))
                    .FieldSet(id: "FieldSetGeneral", action: () => hb
                        .FieldText(
                            controlId: "ApiKey",
                            fieldCss: "field-wide",
                            labelText: Displays.ApiKey(context: context),
                            text: userModel.ApiKey))
                    .Div(
                        id: "ApiEditorCommands",
                        action: () => hb
                            .Button(
                                controlId: "CreateApiKey",
                                controlCss: "button-icon",
                                text: userModel.ApiKey.IsNullOrEmpty()
                                    ? Displays.Create(context: context)
                                    : Displays.ReCreate(context: context),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "CreateApiKey",
                                method: "post")
                            .Button(
                                controlId: "DeleteApiKey",
                                controlCss: "button-icon",
                                text: Displays.Delete(context: context),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-trash",
                                action: "DeleteApiKey",
                                method: "post",
                                confirm: "ConfirmDelete",
                                _using: !userModel.ApiKey.IsNullOrEmpty())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string CreateApiKey(Context context, SiteSettings ss)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: context.UserId);
            var invalid = UserValidators.OnApiCreating(
                context: context,
                userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var error = userModel.CreateApiKey(context: context, ss: ss);
            if (error.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection()
                    .ReplaceAll(
                        "#EditorTabsContainer",
                        new HtmlBuilder().ApiEditor(
                            context: context,
                            userModel: userModel))
                    .Message(Messages.ApiKeyCreated(context: context))
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteApiKey(Context context, SiteSettings ss)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: context.UserId);
            var invalid = UserValidators.OnApiDeleting(
                context: context,
                userModel: userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var error = userModel.DeleteApiKey(context: context, ss: ss);
            if (error.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection()
                    .ReplaceAll(
                        "#EditorTabsContainer",
                        new HtmlBuilder().ApiEditor(
                            context: context,
                            userModel: userModel))
                    .Message(Messages.ApiKeyCreated(context: context))
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult GetByApi(Context context, SiteSettings ss)
        {
            var api = context.Forms.String().Deserialize<Api>();
            if (api == null)
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var view = api?.View ?? new View();
            var siteId = view.ColumnFilterHash
                .Where(f => f.Key == "SiteId")
                .Select(f => f.Value)
                .FirstOrDefault()?.ToLong();
            var siteModel = siteId.HasValue ? new SiteModel(context, siteId.Value) : null;
            if (siteModel != null)
            {
                if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
                {
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
                }
                var invalid = SiteValidators.OnReading(
                    context,
                    siteModel.SitesSiteSettings(context, siteId.Value),
                    siteModel);
                switch (invalid)
                {
                    case Error.Types.None: break;
                    default: return ApiResults.Error(
                        context: context,
                        type: invalid);
                }
            }
            var siteUsers = siteModel != null
                ? SiteInfo.SiteUsers(context, siteModel.InheritPermission)?
                .Where(o => !SiteInfo.User(context, o).Disabled).ToArray()
                : null;
            var pageSize = Parameters.Api.PageSize;
            var userCollection = new UserCollection(
                context: context,
                ss: ss,
                where: view.Where(context: context, ss: ss)
                .Users_TenantId(context.TenantId)
                .SqlWhereLike(
                    name: "SearchText",
                    searchText: view.ColumnFilterHash
                    .Where(f => f.Key == "SearchText")
                    .Select(f => f.Value)
                    .FirstOrDefault(),
                    clauseCollection: new List<string>()
                    {
                        Rds.Users_LoginId_WhereLike(),
                        Rds.Users_Name_WhereLike(),
                        Rds.Users_UserCode_WhereLike(),
                        Rds.Users_Body_WhereLike(),
                        Rds.Depts_DeptCode_WhereLike(),
                        Rds.Depts_DeptName_WhereLike(),
                        Rds.Depts_Body_WhereLike()
                    }),
                orderBy: view.OrderBy(context: context, ss: ss, pageSize: pageSize),
                offset: api.Offset,
                pageSize: pageSize,
                countRecord: true);
            var users = siteUsers == null
                ? userCollection
                : userCollection.Join(siteUsers, c => c.UserId, s => s, (c, s) => c);
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Offset = api.Offset,
                    PageSize = pageSize,
                    TotalCount = users.Count(),
                    Data = users.Select(o => o.GetByApi(
                        context: context,
                        ss: ss))
                }
            }.ToJson());
        }
    }
}
