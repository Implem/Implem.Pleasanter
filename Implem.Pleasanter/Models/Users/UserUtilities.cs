using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
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
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData();
            var userCollection = UserCollection(
                siteSettings,
                Permissions.Admins(),
                formData);
            return hb.Template(
                siteId: 0,
                referenceType: "Users",
                title: Displays.Users() + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: Sessions.User().TenantAdmin,
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("UserForm")
                                .Action(Navigations.Action("Users")),
                            action: () => hb
                                .DataViewFilters(siteSettings)
                                .Aggregations(
                                    siteSettings: siteSettings,
                                    aggregations: userCollection.Aggregations)
                                .Div(id: "DataViewContainer", action: () => hb
                                    .Grid(
                                        userCollection: userCollection,
                                        permissionType: permissionType,
                                        siteSettings: siteSettings,
                                        formData: formData))
                                .MainCommands(
                                    siteId: siteSettings.SiteId,
                                    permissionType: permissionType,
                                    verType: Versions.VerTypes.Latest,
                                    backUrl: Navigations.Index("Admins"))
                                .Div(css: "margin-bottom")
                                .Hidden(controlId: "TableName", value: "Users")
                                .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
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

        private static UserCollection UserCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new UserCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Users",
                    formData: formData,
                    where: Rds.UsersWhere().TenantId(Sessions.TenantId())),
                orderBy: GridSorters.Get(
                    formData, Rds.UsersOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            UserCollection userCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    userCollection: userCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        public static string DataView(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            switch (DataViewSelectors.Get(siteSettings.SiteId))
            {
                default: return Grid(siteSettings: siteSettings, permissionType: permissionType);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            UserCollection userCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class("grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            userCollection: userCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == userCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var userCollection = UserCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    userCollection: userCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: userCollection.Aggregations,
                    container: false))
                .WindowScrollTop().ToJson();
        }

        public static string GridRows(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ResponseCollection responseCollection = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var formData = DataViewFilters.SessionFormData();
            var userCollection = UserCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    userCollection: userCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: userCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, userCollection.Count()))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            UserCollection userCollection,
            FormData formData,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columnCollection: siteSettings.GridColumnCollection(), 
                            formData: formData,
                            checkAll: checkAll))
                .TBody(action: () => userCollection
                    .ForEach(userModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(userModel.UserId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: userModel.UserId.ToString()));
                                siteSettings.GridColumnCollection()
                                    .ForEach(column => hb
                                        .TdValue(
                                            column: column,
                                            userModel: userModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var gridSqlColumn = Rds.UsersColumn()
                .UserId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(column =>
                Rds.UsersColumn(gridSqlColumn, column.ColumnName));
            return gridSqlColumn;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, UserModel userModel)
        {
            switch (column.ColumnName)
            {
                case "UserId": return hb.Td(column: column, value: userModel.UserId);
                case "Ver": return hb.Td(column: column, value: userModel.Ver);
                case "LoginId": return hb.Td(column: column, value: userModel.LoginId);
                case "Disabled": return hb.Td(column: column, value: userModel.Disabled);
                case "LastName": return hb.Td(column: column, value: userModel.LastName);
                case "FirstName": return hb.Td(column: column, value: userModel.FirstName);
                case "Birthday": return hb.Td(column: column, value: userModel.Birthday);
                case "Sex": return hb.Td(column: column, value: userModel.Sex);
                case "Language": return hb.Td(column: column, value: userModel.Language);
                case "TimeZoneInfo": return hb.Td(column: column, value: userModel.TimeZoneInfo);
                case "Dept": return hb.Td(column: column, value: userModel.Dept);
                case "LastLoginTime": return hb.Td(column: column, value: userModel.LastLoginTime);
                case "PasswordExpirationTime": return hb.Td(column: column, value: userModel.PasswordExpirationTime);
                case "PasswordChangeTime": return hb.Td(column: column, value: userModel.PasswordChangeTime);
                case "NumberOfLogins": return hb.Td(column: column, value: userModel.NumberOfLogins);
                case "NumberOfDenial": return hb.Td(column: column, value: userModel.NumberOfDenial);
                case "Comments": return hb.Td(column: column, value: userModel.Comments);
                case "Creator": return hb.Td(column: column, value: userModel.Creator);
                case "Updator": return hb.Td(column: column, value: userModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: userModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: userModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(
                new UserModel(
                    SiteSettingsUtility.UsersSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New),
                byRest: false);
        }

        public static string Editor(int userId, bool clearSessions)
        {
            var userModel = new UserModel(
                    SiteSettingsUtility.UsersSiteSettings(),
                    Permissions.Admins(),
                userId: userId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            userModel.SwitchTargets = UserUtilities.GetSwitchTargets(
                SiteSettingsUtility.UsersSiteSettings());
            return Editor(userModel, byRest: false);
        }

        public static string Editor(UserModel userModel, bool byRest)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            userModel.SiteSettings.SetChoicesByPlaceholders();
            return hb.Template(
                siteId: 0,
                referenceType: "Users",
                title: userModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Users() + " - " + Displays.New()
                    : userModel.Title.Value,
                permissionType: permissionType,
                verType: userModel.VerType,
                methodType: userModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() || userModel.Self() &&
                    userModel.AccessStatus != Databases.AccessStatuses.NotFound,
                byRest: byRest,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager | Permissions.Types.EditProfile;
                    hb
                        .Editor(
                            userModel: userModel,
                            permissionType: permissionType,
                            siteSettings: userModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "Users")
                        .Hidden(controlId: "Id", value: userModel.UserId.ToString());
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            UserModel userModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("UserForm")
                        .Class("main-form")
                        .Action(userModel.UserId != 0
                            ? Navigations.Action("Users", userModel.UserId)
                            : Navigations.Action("Users")),
                    action: () => hb
                        .RecordHeader(
                            id: userModel.UserId,
                            baseModel: userModel,
                            tableName: "Users",
                            switchTargets: userModel.SwitchTargets?
                                .Select(o => o.ToLong()).ToList())
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: userModel.Comments,
                                verType: userModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(userModel: userModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                userModel: userModel)
                            .FieldSetMailAddresses(userModel: userModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: userModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: userModel.VerType,
                                backUrl: Permissions.Admins().CanEditTenant() ? Navigations.Index("Users") : Navigations.Top(),
                                referenceType: "Users",
                                referenceId: userModel.UserId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        userModel: userModel,
                                        siteSettings: siteSettings)))
                        .Hidden(
                            controlId: "MethodType",
                            value: userModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Users_Timestamp",
                            css: "must-transport",
                            value: userModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: userModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("Users", userModel.UserId, userModel.Ver)
                .CopyDialog("Users", userModel.UserId)
                .OutgoingMailDialog()
                .EditorExtensions(userModel: userModel, siteSettings: siteSettings));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, UserModel userModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.Basic()))
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
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            UserModel userModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "UserId": hb.Field(siteSettings, column, userModel.MethodType, userModel.UserId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, userModel.MethodType, userModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "LoginId": hb.Field(siteSettings, column, userModel.MethodType, userModel.LoginId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Disabled": hb.Field(siteSettings, column, userModel.MethodType, userModel.Disabled.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Password": hb.Field(siteSettings, column, userModel.MethodType, userModel.Password.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "PasswordValidate": hb.Field(siteSettings, column, userModel.MethodType, userModel.PasswordValidate.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "PasswordDummy": hb.Field(siteSettings, column, userModel.MethodType, userModel.PasswordDummy.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "RememberMe": hb.Field(siteSettings, column, userModel.MethodType, userModel.RememberMe.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "LastName": hb.Field(siteSettings, column, userModel.MethodType, userModel.LastName.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "FirstName": hb.Field(siteSettings, column, userModel.MethodType, userModel.FirstName.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Birthday": hb.Field(siteSettings, column, userModel.MethodType, userModel.Birthday?.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Sex": hb.Field(siteSettings, column, userModel.MethodType, userModel.Sex.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Language": hb.Field(siteSettings, column, userModel.MethodType, userModel.Language.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "TimeZone": hb.Field(siteSettings, column, userModel.MethodType, userModel.TimeZone.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DeptId": hb.Field(siteSettings, column, userModel.MethodType, userModel.DeptId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "FirstAndLastNameOrder": hb.Field(siteSettings, column, userModel.MethodType, userModel.FirstAndLastNameOrder.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "LastLoginTime": hb.Field(siteSettings, column, userModel.MethodType, userModel.LastLoginTime?.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "PasswordExpirationTime": hb.Field(siteSettings, column, userModel.MethodType, userModel.PasswordExpirationTime?.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "PasswordChangeTime": hb.Field(siteSettings, column, userModel.MethodType, userModel.PasswordChangeTime?.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumberOfLogins": hb.Field(siteSettings, column, userModel.MethodType, userModel.NumberOfLogins.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "NumberOfDenial": hb.Field(siteSettings, column, userModel.MethodType, userModel.NumberOfDenial.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "TenantAdmin": hb.Field(siteSettings, column, userModel.MethodType, userModel.TenantAdmin.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "OldPassword": hb.Field(siteSettings, column, userModel.MethodType, userModel.OldPassword.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ChangedPassword": hb.Field(siteSettings, column, userModel.MethodType, userModel.ChangedPassword.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ChangedPasswordValidator": hb.Field(siteSettings, column, userModel.MethodType, userModel.ChangedPasswordValidator.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "AfterResetPassword": hb.Field(siteSettings, column, userModel.MethodType, userModel.AfterResetPassword.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "AfterResetPasswordValidator": hb.Field(siteSettings, column, userModel.MethodType, userModel.AfterResetPasswordValidator.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DemoMailAddress": hb.Field(siteSettings, column, userModel.MethodType, userModel.DemoMailAddress.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(userModel);
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            UserModel userModel,
            SiteSettings siteSettings)
        {
            if (userModel.VerType == Versions.VerTypes.Latest &&
                userModel.MethodType != BaseModel.MethodTypes.New)
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
                if (Sessions.User().TenantAdmin)
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
            this HtmlBuilder hb,
            UserModel userModel,
            SiteSettings siteSettings)
        {
            return hb
                .ChangePasswordDialog(userId: userModel.UserId, siteSettings: siteSettings)
                .ResetPasswordDialog(userId: userModel.UserId, siteSettings: siteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static List<int> GetSwitchTargets(SiteSettings siteSettings)
        {
            if (Permissions.Admins().CanEditTenant())
            {
                var switchTargets = Forms.Data("SwitchTargets").Split(',')
                    .Select(o => o.ToInt())
                    .Where(o => o != 0)
                    .ToList();
                if (switchTargets.Count() == 0)
                {
                    var formData = DataViewFilters.SessionFormData();
                    switchTargets = Rds.ExecuteTable(
                        transactional: false,
                        statements: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: DataViewFilters.Get(
                                siteSettings: siteSettings,
                                tableName: "Users",
                                formData: formData,
                                where: Rds.UsersWhere().TenantId(Sessions.TenantId())),
                            orderBy: GridSorters.Get(
                                formData, Rds.UsersOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                    .AsEnumerable()
                                    .Select(o => o["UserId"].ToInt())
                                    .ToList();
                }
                return switchTargets;
            }
            else
            {
                return new List<int>();
            }
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, UserModel userModel)
        {
            Forms.All().Keys.ForEach(key =>
            {
                switch (key)
                {
                    default: break;
                }
            });
            return responseCollection;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ChangePasswordDialog(
            this HtmlBuilder hb, long userId, SiteSettings siteSettings)
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
                            .Action(Navigations.Action("Users", userId)),
                        action: () => hb
                            .Field(
                                siteSettings: siteSettings,
                                column: siteSettings.GetColumn("OldPassword"))
                            .Field(
                                siteSettings: siteSettings,
                                column: siteSettings.GetColumn("ChangedPassword"))
                            .Field(
                                siteSettings: siteSettings,
                                column: siteSettings.GetColumn("ChangedPasswordValidator"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Change(),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.send($(this), 'ChangePasswordForm');",
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
            this HtmlBuilder hb, long userId, SiteSettings siteSettings)
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
                            .Action(Navigations.Action("Users", userId)),
                        action: () => hb
                            .Field(
                                siteSettings: siteSettings,
                                column: siteSettings.GetColumn("AfterResetPassword"))
                            .Field(
                                siteSettings: siteSettings,
                                column: siteSettings.GetColumn("AfterResetPasswordValidator"))
                            .P(css: "hidden", action: () => hb
                                .TextBox(
                                    textType: HtmlTypes.TextTypes.Password,
                                    controlCss: " dummy not-transport"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Reset(),
                                    controlCss: "button-icon validate",
                                    onClick: "$p.send($(this), 'ResetPasswordForm');",
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
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                offset:
                    Forms.Data("ControlId").StartsWith("DataViewFilters_") ||
                    Forms.Data("ControlId").StartsWith("GridSorters_")
                        ? 0
                        : Forms.Int("GridOffset"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string HtmlLogin(string returnUrl)
        {
            var hb = new HtmlBuilder();
            var siteSettings = SiteSettingsUtility.UsersSiteSettings();
            return hb.Template(
                siteId: 0,
                referenceType: "Users",
                title: string.Empty,
                permissionType: Permissions.Admins(),
                verType: Versions.VerTypes.Latest,
                useBreadcrumb: false,
                useTitle: false,
                useSearch: false,
                useNavigationButtons: false,
                methodType: BaseModel.MethodTypes.Edit,
                allowAccess: true,
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("UserForm")
                            .Class("main-form")
                            .Action(Navigations.Get("users", "_action_?ReturnUrl="
                                + Url.Encode(returnUrl))),
                        cancelDefaultButton: false,
                        action: () => hb
                            .FieldSet(css: "login", action: () => hb
                                .Div(action: () => hb
                                    .Field(
                                        siteSettings: siteSettings,
                                        column: siteSettings.GetColumn("LoginId"),
                                        controlCss: " must-transport focus")
                                    .Field(
                                        siteSettings: siteSettings,
                                        column: siteSettings.GetColumn("Password"),
                                        fieldCss: "field-wide",
                                        controlCss: " must-transport")
                                    .Field(
                                        siteSettings: siteSettings,
                                        column: siteSettings.GetColumn("RememberMe")))
                                .Div(css: "login-commands cf", action: () => hb
                                    .Button(
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
                            .Action(Navigations.Get("demos", "_action_")),
                        cancelDefaultButton: false,
                        _using: Parameters.Service.Demo,
                        action: () => hb
                            .Div(css: "demo", action: () => hb
                                .FieldSet(
                                    legendText: Displays.ViewDemoEnvironment(),
                                    css: " enclosed-thin",
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Field(
                                                siteSettings: siteSettings,
                                                column: siteSettings.GetColumn("DemoMailAddress"))
                                            .Button(
                                                text: Displays.Register(),
                                                controlCss: "button-icon validate",
                                                onClick: "$p.send($(this), 'DemoForm');",
                                                icon: "ui-icon-mail-closed",
                                                action: "Register",
                                                method: "post")))))
                    .P(id: "Message", css: "message-form-bottom")
                    .ChangePasswordAtLoginDialog(siteSettings: siteSettings)
                    .Hidden(controlId: "ReturnUrl", value: QueryStrings.Data("ReturnUrl")))
                    .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ChangePasswordAtLoginDialog(
            this HtmlBuilder hb, SiteSettings siteSettings)
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
                            .Action(Navigations.Action("Users")),
                        action: () => hb
                            .Field(
                                siteSettings: siteSettings,
                                column: siteSettings.GetColumn("ChangedPassword"))
                            .Field(
                                siteSettings: siteSettings,
                                column: siteSettings.GetColumn("ChangedPasswordValidator"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
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
                                o => o["MailAddress"].ToString());
            userModel.Session_MailAddresses(listItemCollection.Values.ToList<string>());
            return hb.FieldSet(id: "FieldSetMailAddresses", action: () => hb
                .FieldSelectable(
                    controlId: "MailAddresses",
                    fieldCss: "field-vertical w500",
                    controlContainerCss: "container-selectable",
                    controlCss: " h350",
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
    }
}
