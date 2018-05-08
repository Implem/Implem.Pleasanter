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

        private static GridData GetGridData(SiteSettings ss, View view, int offset = 0)
        {
            ss.SetColumnAccessControls();
            return new GridData(
                ss: ss,
                view: view,
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
            View view)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class("grid")
                        .DataValue("back", _using: ss?.IntegratedSites?.Any() == true)
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            ss: ss,
                            gridData: gridData,
                            view: view))
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
                    action: "GridRows",
                    method: "post")
                .Button(
                    controlId: "ViewSorters_Reset",
                    controlCss: "hidden",
                    action: "GridRows",
                    method: "post");
        }

        public static string GridRows(
            SiteSettings ss,
            ResponseCollection res = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view, offset: offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .CloseDialog()
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: gridData.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    ss: ss,
                    gridData: gridData,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
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
            bool clearCheck = false)
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
                            checkAll: checkAll))
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
                    case "Dept": value = userModel.Dept.GridText(column: column); break;
                    case "Body": value = userModel.Body.GridText(column: column); break;
                    case "LastLoginTime": value = userModel.LastLoginTime.GridText(column: column); break;
                    case "PasswordExpirationTime": value = userModel.PasswordExpirationTime.GridText(column: column); break;
                    case "PasswordChangeTime": value = userModel.PasswordChangeTime.GridText(column: column); break;
                    case "NumberOfLogins": value = userModel.NumberOfLogins.GridText(column: column); break;
                    case "NumberOfDenial": value = userModel.NumberOfDenial.GridText(column: column); break;
                    case "TenantManager": value = userModel.TenantManager.GridText(column: column); break;
                    case "Disabled": value = userModel.Disabled.GridText(column: column); break;
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
            if (error.Has())
            {
                return error.MessageJson();
            }
            Sessions.Set("Message", Messages.Created(userModel.Title.Value));
            return new ResponseCollection()
                .SetMemory("formChanged", false)
                .Href(Locations.Edit(
                    controller: Routes.Controller(),
                    id: ss.Columns.Any(o => o.Linking)
                        ? Forms.Long("LinkId")
                        : userModel.UserId))
                .ToJson();
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
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(userModel.Updator.Name).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var res = new UsersResponseCollection(userModel);
                return ResponseByUpdate(res, ss, userModel)
                    .PrependComment(
                        ss,
                        ss.GetColumn("Comments"),
                        userModel.Comments,
                        userModel.VerType)
                    .ToJson();
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
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(userModel.Title.Value));
                var res = new UsersResponseCollection(userModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.Index("Users"));
                return res.ToJson();
            }
        }

        public static string Restore(SiteSettings ss, int userId)
        {
            var userModel = new UserModel();
            var invalid = UserValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = userModel.Restore(ss, userId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                var res = new UsersResponseCollection(userModel);
                return res.ToJson();
            }
        }

        public static string Histories(SiteSettings ss, int userId)
        {
            var userModel = new UserModel(ss, userId);
            ss.SetColumnAccessControls(userModel.Mine());
            var columns = ss.GetHistoryColumns(checkPermission: true);
            if (!ss.CanRead())
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columns: columns,
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new UserCollection(
                            ss: ss,
                            column: HistoryColumn(columns),
                            where: Rds.UsersWhere().UserId(userModel.UserId),
                            orderBy: Rds.UsersOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(userModelHistory => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(userModelHistory.Ver)
                                            .DataLatest(1, _using:
                                                userModelHistory.Ver == userModel.Ver),
                                        action: () => columns
                                            .ForEach(column => hb
                                                .TdValue(
                                                    ss: ss,
                                                    column: column,
                                                    userModel: userModelHistory))))));
            return new UsersResponseCollection(userModel)
                .Html("#FieldSetHistories", hb).ToJson();
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
                            .Action(Locations.Get("users", "_action_?ReturnUrl="
                                + Url.Encode(returnUrl)))
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
