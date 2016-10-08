using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
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
    public static class MailAddressUtilities
    {
        public static string EditorNew()
        {
            return Editor(
                new MailAddressModel(
                    SiteSettingsUtility.MailAddressesSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long mailAddressId, bool clearSessions)
        {
            var mailAddressModel = new MailAddressModel(
                    SiteSettingsUtility.MailAddressesSiteSettings(),
                    Permissions.Admins(),
                mailAddressId: mailAddressId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            mailAddressModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtility.MailAddressesSiteSettings());
            return Editor(mailAddressModel);
        }

        public static string Editor(MailAddressModel mailAddressModel)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            return hb.Template(
                permissionType: permissionType,
                verType: mailAddressModel.VerType,
                methodType: mailAddressModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    mailAddressModel.AccessStatus != Databases.AccessStatuses.NotFound,
                referenceType: "MailAddresses",
                title: mailAddressModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.MailAddresses() + " - " + Displays.New()
                    : mailAddressModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            mailAddressModel: mailAddressModel,
                            permissionType: permissionType,
                            siteSettings: mailAddressModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "MailAddresses")
                        .Hidden(controlId: "Id", value: mailAddressModel.MailAddressId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            MailAddressModel mailAddressModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MailAddressForm")
                        .Class("main-form")
                        .Action(mailAddressModel.MailAddressId != 0
                            ? Navigations.Action("MailAddresses", mailAddressModel.MailAddressId)
                            : Navigations.Action("MailAddresses")),
                    action: () => hb
                        .RecordHeader(
                            baseModel: mailAddressModel,
                            tableName: "MailAddresses")
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: mailAddressModel.Comments,
                                verType: mailAddressModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(mailAddressModel: mailAddressModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                mailAddressModel: mailAddressModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: mailAddressModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: mailAddressModel.VerType,
                                referenceType: "MailAddresses",
                                referenceId: mailAddressModel.MailAddressId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        mailAddressModel: mailAddressModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: mailAddressModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "MailAddresses_Timestamp",
                            css: "must-transport",
                            value: mailAddressModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: mailAddressModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("MailAddresses", mailAddressModel.MailAddressId, mailAddressModel.Ver)
                .CopyDialog("MailAddresses", mailAddressModel.MailAddressId)
                .OutgoingMailDialog()
                .EditorExtensions(mailAddressModel: mailAddressModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, MailAddressModel mailAddressModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: mailAddressModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            MailAddressModel mailAddressModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "OwnerId": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.OwnerId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "OwnerType": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.OwnerType.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "MailAddressId": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.MailAddressId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "MailAddress": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.MailAddress.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Title": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(mailAddressModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            MailAddressModel mailAddressModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            MailAddressModel mailAddressModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static List<long> GetSwitchTargets(SiteSettings siteSettings)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData();
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectMailAddresses(
                        column: Rds.MailAddressesColumn().MailAddressId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "MailAddresses",
                            formData: formData,
                            where: Rds.MailAddressesWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.MailAddressesOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["MailAddressId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, MailAddressModel mailAddressModel)
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
    }
}
