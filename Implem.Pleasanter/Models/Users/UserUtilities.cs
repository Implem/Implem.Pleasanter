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
        public static string Index(SiteSettings ss)
        {
            var invalid = UserValidators.OnEntry(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Users",
                script: JavaScripts.ViewMode(viewMode),
                title: Displays.Users() + " - " + Displays.List(),
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("UserForm")
                                .Class("main-form")
                                .Action(Locations.Action("Users")),
                            action: () => hb
                                .ViewFilters(ss: ss, view: view)
                                .Aggregations(
                                    ss: ss,
                                    aggregations: gridData.Aggregations)
                                .Div(id: "ViewModeContainer", action: () => hb
                                    .Grid(
                                        ss: ss,
                                        gridData: gridData,
                                        view: view))
                                .MainCommands(
                                    ss: ss,
                                    siteId: ss.SiteId,
                                    verType: Versions.VerTypes.Latest)
                                .Div(css: "margin-bottom")
                                .Hidden(controlId: "TableName", value: "Users")
                                .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl())
                                .Hidden(
                                    controlId: "GridOffset",
                                    value: Parameters.General.GridPageSize.ToString()))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ImportSettingsDialog")
                            .Class("dialog")
                            .Title(Displays.Import()))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ExportSettingsDialog")
                            .Class("dialog")
                            .Title(Displays.ExportSettings()));
                }).ToString();
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            SiteSettings ss,
            GridData gridData,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            var invalid = UserValidators.OnEntry(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Users",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(Routes.Action()),
                userStyle: ss.ViewModeStyles(Routes.Action()),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("UsersForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(ss.SiteId)),
                        action: () => hb
                            .ViewSelector(ss: ss, view: view)
                            .ViewFilters(ss: ss, view: view)
                            .Aggregations(
                                ss: ss,
                                aggregations: gridData.Aggregations)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Users")
                            .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl()))
                    .MoveDialog(bulk: true)
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.Export())))
                    .ToString();
        }

        public static string IndexJson(SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            return new ResponseCollection()
                .ViewMode(
                    ss: ss,
                    view: view,
                    gridData: gridData,
                    invoke: "setGrid",
                    body: new HtmlBuilder()
                        .Grid(
                            ss: ss,
                            gridData: gridData,
                            view: view))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static GridData GetGridData(SiteSettings ss, View view, int offset = 0)
        {
            ss.SetColumnAccessControls();
            return new GridData(
                ss: ss,
                view: view,
                join: Rds.UsersJoinDefault(),
                where: Rds.UsersWhere().TenantId(Sessions.TenantId()),
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregations: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
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
            SiteSettings ss,
            ResponseCollection res = null,
            int offset = 0,
            bool clearCheck = false,
            string action = "GridRows",
            Message message = null)
        {
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(
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
                    .CopyDirectUrlToClipboard(ss: ss))
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: gridData.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
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
            SiteSettings ss,
            GridData gridData,
            View view,
            bool addHeader = true,
            bool clearCheck = false,
            string action = "GridRows")
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            var columns = ss.GetGridColumns(checkPermission: true);
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columns: columns, 
                            view: view,
                            checkAll: checkAll,
                            action: action))
                .TBody(action: () => gridData.TBody(
                    hb: hb,
                    ss: ss,
                    columns: columns,
                    checkAll: checkAll));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.UsersColumn();
            new List<string> { "UserId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks().Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.UsersColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.UsersColumn();
            new List<string> { "UserId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks().Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.UsersColumn(column));
            return sqlColumnCollection;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, SiteSettings ss, Column column, UserModel userModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    ss: ss,
                    gridDesign: column.GridDesign,
                    userModel: userModel);
            }
            else
            {
                var mine = userModel.Mine();
                switch (column.Name)
                {
                    case "UserId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.UserId)
                            : hb.Td(column: column, value: string.Empty);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Ver)
                            : hb.Td(column: column, value: string.Empty);
                    case "LoginId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.LoginId)
                            : hb.Td(column: column, value: string.Empty);
                    case "Name":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Name)
                            : hb.Td(column: column, value: string.Empty);
                    case "UserCode":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.UserCode)
                            : hb.Td(column: column, value: string.Empty);
                    case "Birthday":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Birthday)
                            : hb.Td(column: column, value: string.Empty);
                    case "Gender":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Gender)
                            : hb.Td(column: column, value: string.Empty);
                    case "Language":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Language)
                            : hb.Td(column: column, value: string.Empty);
                    case "TimeZoneInfo":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.TimeZoneInfo)
                            : hb.Td(column: column, value: string.Empty);
                    case "DeptCode":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DeptCode)
                            : hb.Td(column: column, value: string.Empty);
                    case "Dept":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Dept)
                            : hb.Td(column: column, value: string.Empty);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Body)
                            : hb.Td(column: column, value: string.Empty);
                    case "LastLoginTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.LastLoginTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "PasswordExpirationTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.PasswordExpirationTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "PasswordChangeTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.PasswordChangeTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumberOfLogins":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumberOfLogins)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumberOfDenial":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumberOfDenial)
                            : hb.Td(column: column, value: string.Empty);
                    case "TenantManager":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.TenantManager)
                            : hb.Td(column: column, value: string.Empty);
                    case "Disabled":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Disabled)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassA)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassB)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassC)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassD)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassE)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassF)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassG)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassH)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassI)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassK)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassL)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassM)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassN)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassO)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassP)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassR)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassS)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassT)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassU)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassV)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassW)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassX)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassY)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.ClassZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumA)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumB)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumC)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumD)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumE)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumF)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumG)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumH)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumI)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumK)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumL)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumM)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumN)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumO)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumP)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumR)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumS)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumT)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumU)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumV)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumW)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumX)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumY)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.NumZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateA)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateB)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateC)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateD)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateE)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateF)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateG)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateH)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateI)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateK)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateL)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateM)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateN)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateO)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateP)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateR)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateS)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateT)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateU)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateV)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateW)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateX)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateY)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DateZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionA)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionB)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionC)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionD)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionE)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionF)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionG)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionH)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionI)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionK)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionL)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionM)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionN)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionO)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionP)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionR)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionS)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionT)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionU)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionV)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionW)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionX)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionY)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.DescriptionZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckA)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckB)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckC)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckD)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckE)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckF)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckG)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckH)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckI)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckK)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckL)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckM)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckN)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckO)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckP)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckR)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckS)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckT)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckU)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckV)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckW)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckX)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckY)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CheckZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "LdapSearchRoot":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.LdapSearchRoot)
                            : hb.Td(column: column, value: string.Empty);
                    case "SynchronizedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.SynchronizedTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Comments)
                            : hb.Td(column: column, value: string.Empty);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Creator)
                            : hb.Td(column: column, value: string.Empty);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.Updator)
                            : hb.Td(column: column, value: string.Empty);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.CreatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: userModel.UpdatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, UserModel userModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "UserId": value = userModel.UserId.GridText(column: column); break;
                    case "Ver": value = userModel.Ver.GridText(column: column); break;
                    case "LoginId": value = userModel.LoginId.GridText(column: column); break;
                    case "Name": value = userModel.Name.GridText(column: column); break;
                    case "UserCode": value = userModel.UserCode.GridText(column: column); break;
                    case "Birthday": value = userModel.Birthday.GridText(column: column); break;
                    case "Gender": value = userModel.Gender.GridText(column: column); break;
                    case "Language": value = userModel.Language.GridText(column: column); break;
                    case "TimeZoneInfo": value = userModel.TimeZoneInfo.GridText(column: column); break;
                    case "DeptCode": value = userModel.DeptCode.GridText(column: column); break;
                    case "Dept": value = userModel.Dept.GridText(column: column); break;
                    case "Body": value = userModel.Body.GridText(column: column); break;
                    case "LastLoginTime": value = userModel.LastLoginTime.GridText(column: column); break;
                    case "PasswordExpirationTime": value = userModel.PasswordExpirationTime.GridText(column: column); break;
                    case "PasswordChangeTime": value = userModel.PasswordChangeTime.GridText(column: column); break;
                    case "NumberOfLogins": value = userModel.NumberOfLogins.GridText(column: column); break;
                    case "NumberOfDenial": value = userModel.NumberOfDenial.GridText(column: column); break;
                    case "TenantManager": value = userModel.TenantManager.GridText(column: column); break;
                    case "Disabled": value = userModel.Disabled.GridText(column: column); break;
                    case "ClassA": value = userModel.ClassA.GridText(column: column); break;
                    case "ClassB": value = userModel.ClassB.GridText(column: column); break;
                    case "ClassC": value = userModel.ClassC.GridText(column: column); break;
                    case "ClassD": value = userModel.ClassD.GridText(column: column); break;
                    case "ClassE": value = userModel.ClassE.GridText(column: column); break;
                    case "ClassF": value = userModel.ClassF.GridText(column: column); break;
                    case "ClassG": value = userModel.ClassG.GridText(column: column); break;
                    case "ClassH": value = userModel.ClassH.GridText(column: column); break;
                    case "ClassI": value = userModel.ClassI.GridText(column: column); break;
                    case "ClassJ": value = userModel.ClassJ.GridText(column: column); break;
                    case "ClassK": value = userModel.ClassK.GridText(column: column); break;
                    case "ClassL": value = userModel.ClassL.GridText(column: column); break;
                    case "ClassM": value = userModel.ClassM.GridText(column: column); break;
                    case "ClassN": value = userModel.ClassN.GridText(column: column); break;
                    case "ClassO": value = userModel.ClassO.GridText(column: column); break;
                    case "ClassP": value = userModel.ClassP.GridText(column: column); break;
                    case "ClassQ": value = userModel.ClassQ.GridText(column: column); break;
                    case "ClassR": value = userModel.ClassR.GridText(column: column); break;
                    case "ClassS": value = userModel.ClassS.GridText(column: column); break;
                    case "ClassT": value = userModel.ClassT.GridText(column: column); break;
                    case "ClassU": value = userModel.ClassU.GridText(column: column); break;
                    case "ClassV": value = userModel.ClassV.GridText(column: column); break;
                    case "ClassW": value = userModel.ClassW.GridText(column: column); break;
                    case "ClassX": value = userModel.ClassX.GridText(column: column); break;
                    case "ClassY": value = userModel.ClassY.GridText(column: column); break;
                    case "ClassZ": value = userModel.ClassZ.GridText(column: column); break;
                    case "NumA": value = userModel.NumA.GridText(column: column); break;
                    case "NumB": value = userModel.NumB.GridText(column: column); break;
                    case "NumC": value = userModel.NumC.GridText(column: column); break;
                    case "NumD": value = userModel.NumD.GridText(column: column); break;
                    case "NumE": value = userModel.NumE.GridText(column: column); break;
                    case "NumF": value = userModel.NumF.GridText(column: column); break;
                    case "NumG": value = userModel.NumG.GridText(column: column); break;
                    case "NumH": value = userModel.NumH.GridText(column: column); break;
                    case "NumI": value = userModel.NumI.GridText(column: column); break;
                    case "NumJ": value = userModel.NumJ.GridText(column: column); break;
                    case "NumK": value = userModel.NumK.GridText(column: column); break;
                    case "NumL": value = userModel.NumL.GridText(column: column); break;
                    case "NumM": value = userModel.NumM.GridText(column: column); break;
                    case "NumN": value = userModel.NumN.GridText(column: column); break;
                    case "NumO": value = userModel.NumO.GridText(column: column); break;
                    case "NumP": value = userModel.NumP.GridText(column: column); break;
                    case "NumQ": value = userModel.NumQ.GridText(column: column); break;
                    case "NumR": value = userModel.NumR.GridText(column: column); break;
                    case "NumS": value = userModel.NumS.GridText(column: column); break;
                    case "NumT": value = userModel.NumT.GridText(column: column); break;
                    case "NumU": value = userModel.NumU.GridText(column: column); break;
                    case "NumV": value = userModel.NumV.GridText(column: column); break;
                    case "NumW": value = userModel.NumW.GridText(column: column); break;
                    case "NumX": value = userModel.NumX.GridText(column: column); break;
                    case "NumY": value = userModel.NumY.GridText(column: column); break;
                    case "NumZ": value = userModel.NumZ.GridText(column: column); break;
                    case "DateA": value = userModel.DateA.GridText(column: column); break;
                    case "DateB": value = userModel.DateB.GridText(column: column); break;
                    case "DateC": value = userModel.DateC.GridText(column: column); break;
                    case "DateD": value = userModel.DateD.GridText(column: column); break;
                    case "DateE": value = userModel.DateE.GridText(column: column); break;
                    case "DateF": value = userModel.DateF.GridText(column: column); break;
                    case "DateG": value = userModel.DateG.GridText(column: column); break;
                    case "DateH": value = userModel.DateH.GridText(column: column); break;
                    case "DateI": value = userModel.DateI.GridText(column: column); break;
                    case "DateJ": value = userModel.DateJ.GridText(column: column); break;
                    case "DateK": value = userModel.DateK.GridText(column: column); break;
                    case "DateL": value = userModel.DateL.GridText(column: column); break;
                    case "DateM": value = userModel.DateM.GridText(column: column); break;
                    case "DateN": value = userModel.DateN.GridText(column: column); break;
                    case "DateO": value = userModel.DateO.GridText(column: column); break;
                    case "DateP": value = userModel.DateP.GridText(column: column); break;
                    case "DateQ": value = userModel.DateQ.GridText(column: column); break;
                    case "DateR": value = userModel.DateR.GridText(column: column); break;
                    case "DateS": value = userModel.DateS.GridText(column: column); break;
                    case "DateT": value = userModel.DateT.GridText(column: column); break;
                    case "DateU": value = userModel.DateU.GridText(column: column); break;
                    case "DateV": value = userModel.DateV.GridText(column: column); break;
                    case "DateW": value = userModel.DateW.GridText(column: column); break;
                    case "DateX": value = userModel.DateX.GridText(column: column); break;
                    case "DateY": value = userModel.DateY.GridText(column: column); break;
                    case "DateZ": value = userModel.DateZ.GridText(column: column); break;
                    case "DescriptionA": value = userModel.DescriptionA.GridText(column: column); break;
                    case "DescriptionB": value = userModel.DescriptionB.GridText(column: column); break;
                    case "DescriptionC": value = userModel.DescriptionC.GridText(column: column); break;
                    case "DescriptionD": value = userModel.DescriptionD.GridText(column: column); break;
                    case "DescriptionE": value = userModel.DescriptionE.GridText(column: column); break;
                    case "DescriptionF": value = userModel.DescriptionF.GridText(column: column); break;
                    case "DescriptionG": value = userModel.DescriptionG.GridText(column: column); break;
                    case "DescriptionH": value = userModel.DescriptionH.GridText(column: column); break;
                    case "DescriptionI": value = userModel.DescriptionI.GridText(column: column); break;
                    case "DescriptionJ": value = userModel.DescriptionJ.GridText(column: column); break;
                    case "DescriptionK": value = userModel.DescriptionK.GridText(column: column); break;
                    case "DescriptionL": value = userModel.DescriptionL.GridText(column: column); break;
                    case "DescriptionM": value = userModel.DescriptionM.GridText(column: column); break;
                    case "DescriptionN": value = userModel.DescriptionN.GridText(column: column); break;
                    case "DescriptionO": value = userModel.DescriptionO.GridText(column: column); break;
                    case "DescriptionP": value = userModel.DescriptionP.GridText(column: column); break;
                    case "DescriptionQ": value = userModel.DescriptionQ.GridText(column: column); break;
                    case "DescriptionR": value = userModel.DescriptionR.GridText(column: column); break;
                    case "DescriptionS": value = userModel.DescriptionS.GridText(column: column); break;
                    case "DescriptionT": value = userModel.DescriptionT.GridText(column: column); break;
                    case "DescriptionU": value = userModel.DescriptionU.GridText(column: column); break;
                    case "DescriptionV": value = userModel.DescriptionV.GridText(column: column); break;
                    case "DescriptionW": value = userModel.DescriptionW.GridText(column: column); break;
                    case "DescriptionX": value = userModel.DescriptionX.GridText(column: column); break;
                    case "DescriptionY": value = userModel.DescriptionY.GridText(column: column); break;
                    case "DescriptionZ": value = userModel.DescriptionZ.GridText(column: column); break;
                    case "CheckA": value = userModel.CheckA.GridText(column: column); break;
                    case "CheckB": value = userModel.CheckB.GridText(column: column); break;
                    case "CheckC": value = userModel.CheckC.GridText(column: column); break;
                    case "CheckD": value = userModel.CheckD.GridText(column: column); break;
                    case "CheckE": value = userModel.CheckE.GridText(column: column); break;
                    case "CheckF": value = userModel.CheckF.GridText(column: column); break;
                    case "CheckG": value = userModel.CheckG.GridText(column: column); break;
                    case "CheckH": value = userModel.CheckH.GridText(column: column); break;
                    case "CheckI": value = userModel.CheckI.GridText(column: column); break;
                    case "CheckJ": value = userModel.CheckJ.GridText(column: column); break;
                    case "CheckK": value = userModel.CheckK.GridText(column: column); break;
                    case "CheckL": value = userModel.CheckL.GridText(column: column); break;
                    case "CheckM": value = userModel.CheckM.GridText(column: column); break;
                    case "CheckN": value = userModel.CheckN.GridText(column: column); break;
                    case "CheckO": value = userModel.CheckO.GridText(column: column); break;
                    case "CheckP": value = userModel.CheckP.GridText(column: column); break;
                    case "CheckQ": value = userModel.CheckQ.GridText(column: column); break;
                    case "CheckR": value = userModel.CheckR.GridText(column: column); break;
                    case "CheckS": value = userModel.CheckS.GridText(column: column); break;
                    case "CheckT": value = userModel.CheckT.GridText(column: column); break;
                    case "CheckU": value = userModel.CheckU.GridText(column: column); break;
                    case "CheckV": value = userModel.CheckV.GridText(column: column); break;
                    case "CheckW": value = userModel.CheckW.GridText(column: column); break;
                    case "CheckX": value = userModel.CheckX.GridText(column: column); break;
                    case "CheckY": value = userModel.CheckY.GridText(column: column); break;
                    case "CheckZ": value = userModel.CheckZ.GridText(column: column); break;
                    case "LdapSearchRoot": value = userModel.LdapSearchRoot.GridText(column: column); break;
                    case "SynchronizedTime": value = userModel.SynchronizedTime.GridText(column: column); break;
                    case "Comments": value = userModel.Comments.GridText(column: column); break;
                    case "Creator": value = userModel.Creator.GridText(column: column); break;
                    case "Updator": value = userModel.Updator.GridText(column: column); break;
                    case "CreatedTime": value = userModel.CreatedTime.GridText(column: column); break;
                    case "UpdatedTime": value = userModel.UpdatedTime.GridText(column: column); break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorNew(SiteSettings ss)
        {
            if (Contract.UsersLimit())
            {
                return HtmlTemplates.Error(Error.Types.UsersLimit);
            }
            return Editor(ss, new UserModel(
                SiteSettingsUtilities.UsersSiteSettings(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(SiteSettings ss, int userId, bool clearSessions)
        {
            var userModel = new UserModel(
                SiteSettingsUtilities.UsersSiteSettings(),
                userId: userId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            userModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtilities.UsersSiteSettings(), userModel.UserId);
            return Editor(ss, userModel);
        }

        public static string Editor(SiteSettings ss, UserModel userModel)
        {
            var invalid = UserValidators.OnEditing(ss, userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(userModel.Mine());
            return hb.Template(
                ss: ss,
                verType: userModel.VerType,
                methodType: userModel.MethodType,
                referenceType: "Users",
                title: userModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Users() + " - " + Displays.New()
                    : userModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
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
            this HtmlBuilder hb, SiteSettings ss, UserModel userModel)
        {
            var commentsColumn = ss.GetColumn("Comments");
            var commentsColumnPermissionType = commentsColumn.ColumnPermissionType();
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("UserForm")
                        .Class("main-form confirm-reload")
                        .Action(userModel.UserId != 0
                            ? Locations.Action("Users", userModel.UserId)
                            : Locations.Action("Users")),
                    action: () => hb
                        .RecordHeader(
                            ss: ss,
                            baseModel: userModel,
                            tableName: "Users")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    comments: userModel.Comments,
                                    column: commentsColumn,
                                    verType: userModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(userModel: userModel)
                            .FieldSetGeneral(ss: ss, userModel: userModel)
                            .FieldSetMailAddresses(userModel: userModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: userModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                ss: ss,
                                siteId: 0,
                                verType: userModel.VerType,
                                referenceId: userModel.UserId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        userModel: userModel,
                                        ss: ss)))
                        .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl())
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
                .OutgoingMailsForm("Users", userModel.UserId, userModel.Ver)
                .CopyDialog("Users", userModel.UserId)
                .OutgoingMailDialog()
                .EditorExtensions(userModel: userModel, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, UserModel userModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General()))
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetMailAddresses",
                        text: Displays.MailAddresses(),
                        _using: userModel.MethodType != BaseModel.MethodTypes.New))
                .Li(
                    _using: userModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings ss,
            UserModel userModel)
        {
            var mine = userModel.Mine();
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    ss: ss, userModel: userModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            SiteSettings ss,
            UserModel userModel,
            bool preview = false)
        {
            ss.GetEditorColumns().ForEach(column =>
            {
                switch (column.Name)
                {
                    case "UserId":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.UserId.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.Ver.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "LoginId":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.LoginId.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Name":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.Name.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "UserCode":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.UserCode.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Password":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.Password.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "PasswordValidate":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.PasswordValidate.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "PasswordDummy":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.PasswordDummy.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "RememberMe":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.RememberMe.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Birthday":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.Birthday.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Gender":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.Gender.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Language":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.Language.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "TimeZone":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.TimeZone.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DeptId":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DeptId.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.Body.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "LastLoginTime":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.LastLoginTime.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "PasswordExpirationTime":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.PasswordExpirationTime.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "PasswordChangeTime":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.PasswordChangeTime.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumberOfLogins":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumberOfLogins.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumberOfDenial":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumberOfDenial.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "TenantManager":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.TenantManager.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Disabled":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.Disabled.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "OldPassword":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.OldPassword.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ChangedPassword":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ChangedPassword.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ChangedPasswordValidator":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ChangedPasswordValidator.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AfterResetPassword":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.AfterResetPassword.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AfterResetPasswordValidator":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.AfterResetPasswordValidator.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DemoMailAddress":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DemoMailAddress.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassA":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassB":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassC":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassD":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassE":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassF":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassG":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassH":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassI":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassJ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassK":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassL":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassM":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassN":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassO":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassP":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassQ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassR":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassS":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassT":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassU":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassV":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassW":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassX":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassY":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassZ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.ClassZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumA":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumB":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumC":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumD":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumE":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumF":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumG":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumH":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumI":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumJ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumK":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumL":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumM":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumN":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumO":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumP":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumQ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumR":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumS":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumT":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumU":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumV":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumW":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumX":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumY":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumZ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.NumZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateA":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateB":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateC":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateD":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateE":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateF":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateG":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateH":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateI":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateJ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateK":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateL":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateM":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateN":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateO":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateP":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateQ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateR":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateS":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateT":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateU":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateV":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateW":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateX":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateY":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateZ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DateZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionA":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionB":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionC":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionD":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionE":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionF":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionG":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionH":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionI":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionJ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionK":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionL":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionM":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionN":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionO":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionP":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionQ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionR":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionS":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionT":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionU":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionV":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionW":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionX":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionY":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionZ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.DescriptionZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckA":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckB":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckC":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckD":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckE":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckF":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckG":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckH":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckI":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckJ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckK":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckL":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckM":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckN":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckO":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckP":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckQ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckR":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckS":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckT":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckU":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckV":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckW":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckX":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckY":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckZ":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.CheckZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "LdapSearchRoot":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.LdapSearchRoot.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "SynchronizedTime":
                        hb.Field(
                            ss,
                            column,
                            userModel.MethodType,
                            userModel.SynchronizedTime.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                }
            });
            if (!preview) hb.VerUpCheckBox(userModel);
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb, SiteSettings ss, UserModel userModel)
        {
            if (userModel.VerType == Versions.VerTypes.Latest &&
                userModel.MethodType != BaseModel.MethodTypes.New &&
                Parameters.Authentication.Provider == null)
            {
                if (userModel.Self())
                {
                    hb.Button(
                        text: Displays.ChangePassword(),
                        controlCss: "button-icon",
                        onClick: "$p.openDialog($(this));",
                        icon: "ui-icon-person",
                        selector: "#ChangePasswordDialog");
                }
                if (Sessions.User().TenantManager)
                {
                    hb.Button(
                        text: Displays.ResetPassword(),
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
            this HtmlBuilder hb, UserModel userModel, SiteSettings ss)
        {
            return hb
                .ChangePasswordDialog(userId: userModel.UserId, ss: ss)
                .ResetPasswordDialog(userId: userModel.UserId, ss: ss);
        }

        public static string EditorJson(SiteSettings ss, int userId)
        {
            return EditorResponse(ss, new UserModel(ss, userId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            SiteSettings ss,
            UserModel userModel,
            Message message = null,
            string switchTargets = null)
        {
            userModel.MethodType = BaseModel.MethodTypes.Edit;
            return new UsersResponseCollection(userModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(ss, userModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<int> GetSwitchTargets(SiteSettings ss, int userId)
        {
            if (Permissions.CanManageTenant())
            {
                var view = Views.GetBySession(ss);
                var where = view.Where(ss: ss, where: Rds.UsersWhere().TenantId(Sessions.TenantId()));
                var switchTargets = Rds.ExecuteScalar_int(statements:
                    Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: where)) <= Parameters.General.SwitchTargetsLimit
                            ? Rds.ExecuteTable(statements: Rds.SelectUsers(
                                column: Rds.UsersColumn().UserId(),
                                where: where,
                                orderBy: view.OrderBy(ss, Rds.UsersOrderBy()
                                    .UpdatedTime(SqlOrderBy.Types.desc))))
                                    .AsEnumerable()
                                    .Select(o => o["UserId"].ToInt())
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

        public static string Create(SiteSettings ss)
        {
            if (Contract.UsersLimit())
            {
                return Error.Types.UsersLimit.MessageJson();
            }
            var userModel = new UserModel(ss, 0, setByForm: true);
            var invalid = UserValidators.OnCreating(ss, userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = userModel.Create(ss);
            switch (error)
            {
                case Error.Types.None:
                    Sessions.Set("Message", Messages.Created(userModel.Title.Value));
                    return new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            controller: Routes.Controller(),
                            id: ss.Columns.Any(o => o.Linking)
                                ? Forms.Long("LinkId")
                                : userModel.UserId))
                        .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static string Update(SiteSettings ss, int userId)
        {
            var userModel = new UserModel(ss, userId, setByForm: true);
            var invalid = UserValidators.OnUpdating(ss, userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                case Error.Types.PermissionNotSelfChange:
                    return Messages.ResponsePermissionNotSelfChange()
                        .Val("#Users_TenantManager", userModel.SavedTenantManager)
                        .ClearFormData("Users_TenantManager")
                        .ToJson();
                default: return invalid.MessageJson();
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = userModel.Update(ss);
            switch (error)
            {
                case Error.Types.None:
                    var res = new UsersResponseCollection(userModel);
                    return ResponseByUpdate(res, ss, userModel)
                        .PrependComment(
                            ss,
                            ss.GetColumn("Comments"),
                            userModel.Comments,
                            userModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        userModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            UsersResponseCollection res,
            SiteSettings ss,
            UserModel userModel)
        {
            return res
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", userModel.Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: userModel, tableName: "Users"))
                .SetMemory("formChanged", false)
                .Message(Messages.Updated(userModel.Title.Value))
                .Comment(
                    ss,
                    ss.GetColumn("Comments"),
                    userModel.Comments,
                    userModel.DeleteCommentId)
                .ClearFormData();
        }

        public static string Delete(SiteSettings ss, int userId)
        {
            var userModel = new UserModel(ss, userId);
            var invalid = UserValidators.OnDeleting(ss, userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = userModel.Delete(ss);
            switch (error)
            {
                case Error.Types.None:
                    Sessions.Set("Message", Messages.Deleted(userModel.Title.Value));
                    var res = new UsersResponseCollection(userModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.Index("Users"));
                    return res.ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static string Histories(
            SiteSettings ss, int userId, Message message = null)
        {
            var userModel = new UserModel(ss, userId);
            ss.SetColumnAccessControls(userModel.Mine());
            var columns = ss.GetHistoryColumns(checkPermission: true);
            if (!ss.CanRead())
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            var hb = new HtmlBuilder();
            hb
                .HistoryCommands()
                .Table(
                    attributes: new HtmlAttributes().Class("grid history"),
                    action: () => hb
                        .THead(action: () => hb
                            .GridHeader(
                                columns: columns,
                                sort: false,
                                checkRow: true))
                        .TBody(action: () => hb
                            .HistoriesTableBody(
                                ss: ss,
                                columns: columns,
                                userModel: userModel)));
            return new UsersResponseCollection(userModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
                .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb, SiteSettings ss, List<Column> columns, UserModel userModel)
        {
            new UserCollection(
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

        public static string History(SiteSettings ss, int userId)
        {
            var userModel = new UserModel(ss, userId);
            ss.SetColumnAccessControls(userModel.Mine());
            userModel.Get(
                ss, 
                where: Rds.UsersWhere()
                    .UserId(userModel.UserId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            userModel.VerType = Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(ss, userModel).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ChangePassword(int userId)
        {
            var userModel = new UserModel(
                SiteSettingsUtilities.UsersSiteSettings(), userId, setByForm: true);
            var invalid = UserValidators.OnPasswordChanging(userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = userModel.ChangePassword();
            return error.Has()
                ? error.MessageJson()
                : new UsersResponseCollection(userModel)
                    .OldPassword(string.Empty)
                    .ChangedPassword(string.Empty)
                    .ChangedPasswordValidator(string.Empty)
                    .Ver()
                    .Timestamp()
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        baseModel: userModel, tableName: "Users"))
                    .CloseDialog()
                    .ClearFormData()
                    .Message(Messages.ChangingPasswordComplete())
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ChangePasswordAtLogin()
        {
            var userModel = new UserModel(Forms.Data("Users_LoginId"));
            var invalid = UserValidators.OnPasswordChangingAtLogin(userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = userModel.ChangePasswordAtLogin();
            return error.Has()
                ? error.MessageJson()
                : userModel.Allow(Forms.Data("ReturnUrl"), atLogin: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ResetPassword(int userId)
        {
            var userModel = new UserModel(
                SiteSettingsUtilities.UsersSiteSettings(), userId, setByForm: true);
            var invalid = UserValidators.OnPasswordResetting();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = userModel.ResetPassword();
            return error.Has()
                ? error.MessageJson()
                : new UsersResponseCollection(userModel)
                    .PasswordExpirationTime()
                    .PasswordChangeTime()
                    .AfterResetPassword(string.Empty)
                    .AfterResetPasswordValidator(string.Empty)
                    .Ver()
                    .Timestamp()
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        baseModel: userModel, tableName: "Users"))
                    .CloseDialog()
                    .ClearFormData()
                    .Message(Messages.PasswordResetCompleted())
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string AddMailAddresses(SiteSettings ss, int userId)
        {
            var userModel = new UserModel(SiteSettingsUtilities.UsersSiteSettings(), userId);
            var mailAddress = Forms.Data("MailAddress").Trim();
            var selected = Forms.List("MailAddresses");
            var badMailAddress = string.Empty;
            var invalid = UserValidators.OnAddingMailAddress(
                ss, userModel, mailAddress, out badMailAddress);
            switch (invalid)
            {
                case Error.Types.None:
                    break;
                case Error.Types.BadMailAddress:
                    return invalid.MessageJson(badMailAddress);
                default:
                    return invalid.MessageJson();
            }
            var error = userModel.AddMailAddress(mailAddress, selected);
            return error.Has()
                ? error.MessageJson()
                : ResponseMailAddresses(userModel, selected);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteMailAddresses(SiteSettings ss, int userId)
        {
            var userModel = new UserModel(SiteSettingsUtilities.UsersSiteSettings(), userId);
            var invalid = UserValidators.OnUpdating(ss, userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var selected = Forms.List("MailAddresses");
            var error = userModel.DeleteMailAddresses(selected);
            return error.Has()
                ? error.MessageJson()
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
            this HtmlBuilder hb, long userId, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ChangePasswordDialog")
                    .Class("dialog")
                    .Title(Displays.ChangePassword()),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ChangePasswordForm")
                            .Action(Locations.Action("Users", userId))
                            .DataEnter("#ChangePassword"),
                        action: () => hb
                            .Field(
                                ss: ss,
                                column: ss.GetColumn("OldPassword"))
                            .Field(
                                ss: ss,
                                column: ss.GetColumn("ChangedPassword"))
                            .Field(
                                ss: ss,
                                column: ss.GetColumn("ChangedPasswordValidator"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ChangePassword",
                                    text: Displays.Change(),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ChangePassword",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ResetPasswordDialog(
            this HtmlBuilder hb, long userId, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ResetPasswordDialog")
                    .Class("dialog")
                    .Title(Displays.ResetPassword()),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ResetPasswordForm")
                            .Action(Locations.Action("Users", userId))
                            .DataEnter("#ResetPassword"),
                        action: () => hb
                            .Field(
                                ss: ss,
                                column: ss.GetColumn("AfterResetPassword"))
                            .Field(
                                ss: ss,
                                column: ss.GetColumn("AfterResetPasswordValidator"))
                            .P(css: "hidden", action: () => hb
                                .TextBox(
                                    textType: HtmlTypes.TextTypes.Password,
                                    controlCss: " dummy not-send"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ResetPassword",
                                    text: Displays.Reset(),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ResetPassword",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows()
        {
            return GridRows(
                SiteSettingsUtilities.UsersSiteSettings(),
                offset: DataViewGrid.Offset());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string HtmlLogin(string returnUrl, string message)
        {
            var hb = new HtmlBuilder();
            var ss = SiteSettingsUtilities.UsersSiteSettings();
            return hb.Template(
                ss: ss,
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
                            .Text(Displays.Portal())))
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("UserForm")
                            .Class("main-form")
                            .Action(Locations.Raw("users", "_action_?ReturnUrl=" + returnUrl))
                            .DataEnter("#Login"),
                        action: () => hb
                            .FieldSet(id: "LoginFieldSet", action: () => hb
                                .Div(action: () => hb
                                    .Field(
                                        ss: ss,
                                        column: ss.GetColumn("LoginId"),
                                        fieldCss: "field-wide",
                                        controlCss: " always-send focus")
                                    .Field(
                                        ss: ss,
                                        column: ss.GetColumn("Password"),
                                        fieldCss: "field-wide",
                                        controlCss: " always-send")
                                    .Div(id: "Tenants")
                                    .Field(
                                        ss: ss,
                                        column: ss.GetColumn("RememberMe")))
                                .Div(id: "LoginCommands", action: () => hb
                                    .Button(
                                        controlId: "Login",
                                        controlCss: "button-icon button-right-justified validate",
                                        text: Displays.Login(),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-unlocked",
                                        action: "Authenticate",
                                        method: "post",
                                        type: "submit"))))
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("DemoForm")
                            .Action(Locations.Get("demos", "_action_")),
                        _using: Parameters.Service.Demo,
                        action: () => hb
                            .Div(id: "Demo", action: () => hb
                                .FieldSet(
                                    css: " enclosed-thin",
                                    legendText: Displays.ViewDemoEnvironment(),
                                    action: () => hb
                                        .Div(id: "DemoFields", action: () => hb
                                            .Field(
                                                ss: ss,
                                                column: ss.GetColumn("DemoMailAddress"))
                                            .Button(
                                                text: Displays.Register(),
                                                controlCss: "button-icon validate",
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-mail-closed",
                                                action: "Register",
                                                method: "post")))))
                    .P(id: "Message", css: "message-form-bottom", action: () => hb.Raw(message))
                    .ChangePasswordAtLoginDialog(ss: ss)
                    .Hidden(controlId: "ReturnUrl", value: QueryStrings.Data("ReturnUrl")))
                    .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Expired()
        {
            return HtmlTemplates.Error(Error.Types.Expired);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ChangePasswordAtLoginDialog(
            this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ChangePasswordDialog")
                    .Class("dialog")
                    .Title(Displays.ChangePassword()),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ChangePasswordForm")
                            .Action(Locations.Action("Users"))
                            .DataEnter("#ChangePassword"),
                        action: () => hb
                            .Field(
                                ss: ss,
                                column: ss.GetColumn("ChangedPassword"))
                            .Field(
                                ss: ss,
                                column: ss.GetColumn("ChangedPasswordValidator"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ChangePassword",
                                    text: Displays.Change(),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.changePasswordAtLogin($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ChangePasswordAtLogin",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FieldSetMailAddresses(this HtmlBuilder hb, UserModel userModel)
        {
            if (userModel.MethodType == BaseModel.MethodTypes.New) return hb;
            var listItemCollection = Rds.ExecuteTable(statements:
                Rds.SelectMailAddresses(
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
            userModel.Session_MailAddresses(listItemCollection.Keys.ToList());
            return hb.FieldSet(id: "FieldSetMailAddresses", action: () => hb
                .FieldSelectable(
                    controlId: "MailAddresses",
                    fieldCss: "field-vertical w500",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h350",
                    labelText: Displays.MailAddresses(),
                    listItemCollection: listItemCollection,
                    commandOptionAction: () => hb
                        .Div(css: "command-left", action: () => hb
                            .TextBox(
                                controlId: "MailAddress",
                                controlCss: " w200")
                            .Button(
                                text: Displays.Add(),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "AddMailAddress",
                                method: "post")
                            .Button(
                                controlId: "DeleteMailAddresses",
                                controlCss: "button-icon",
                                text: Displays.Delete(),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-image",
                                action: "DeleteMailAddresses",
                                method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SyncByLdap()
        {
            Ldap.Sync();
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ApiEditor(SiteSettings ss)
        {
            var userModel = new UserModel(ss, Sessions.UserId());
            var invalid = UserValidators.OnApiEditing(userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var hb = new HtmlBuilder();
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Users",
                title: Displays.ApiSettings(),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("UserForm")
                            .Class("main-form")
                            .Action(Locations.Action("Users")),
                        action: () => hb
                            .ApiEditor(userModel)
                            .MainCommands(
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")))
                                .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ApiEditor(this HtmlBuilder hb, UserModel userModel)
        {
            return hb
                .Div(id: "EditorTabsContainer", css: "max", action: () => hb
                    .Ul(id: "EditorTabs", action: () => hb
                        .Li(action: () => hb
                            .A(
                                href: "#FieldSetGeneral",
                                text: Displays.General())))
                    .FieldSet(id: "FieldSetGeneral", action: () => hb
                        .FieldText(
                            controlId: "ApiKey",
                            fieldCss: "field-wide",
                            labelText: Displays.ApiKey(),
                            text: userModel.ApiKey))
                    .Div(
                        id: "ApiEditorCommands",
                        action: () => hb
                            .Button(
                                controlId: "CreateApiKey",
                                controlCss: "button-icon",
                                text: userModel.ApiKey.IsNullOrEmpty()
                                    ? Displays.Create()
                                    : Displays.ReCreate(),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "CreateApiKey",
                                method: "post")
                            .Button(
                                controlId: "DeleteApiKey",
                                controlCss: "button-icon",
                                text: Displays.Delete(),
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
        public static string CreateApiKey(SiteSettings ss)
        {
            var userModel = new UserModel(ss, Sessions.UserId());
            var invalid = UserValidators.OnApiCreating(userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var error = userModel.CreateApiKey(ss);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return new ResponseCollection()
                    .ReplaceAll("#EditorTabsContainer", new HtmlBuilder().ApiEditor(userModel))
                    .Message(Messages.ApiKeyCreated())
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteApiKey(SiteSettings ss)
        {
            var userModel = new UserModel(ss, Sessions.UserId());
            var invalid = UserValidators.OnApiDeleting(userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var error = userModel.DeleteApiKey(ss);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return new ResponseCollection()
                    .ReplaceAll("#EditorTabsContainer", new HtmlBuilder().ApiEditor(userModel))
                    .Message(Messages.ApiKeyCreated())
                    .ToJson();
            }
        }
    }
}
