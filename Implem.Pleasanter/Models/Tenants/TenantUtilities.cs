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
    public static class TenantUtilities
    {
        public static string EditorNew()
        {
            return Editor(
                new TenantModel(
                    SiteSettingsUtility.TenantsSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(int tenantId, bool clearSessions)
        {
            var tenantModel = new TenantModel(
                    SiteSettingsUtility.TenantsSiteSettings(),
                    Permissions.Admins(),
                tenantId: tenantId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            tenantModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtility.TenantsSiteSettings());
            return Editor(tenantModel);
        }

        public static string Editor(TenantModel tenantModel)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            return hb.Template(
                permissionType: permissionType,
                verType: tenantModel.VerType,
                methodType: tenantModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    tenantModel.AccessStatus != Databases.AccessStatuses.NotFound,
                referenceType: "Tenants",
                title: tenantModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Tenants() + " - " + Displays.New()
                    : tenantModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            tenantModel: tenantModel,
                            permissionType: permissionType,
                            siteSettings: tenantModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "Tenants")
                        .Hidden(controlId: "Id", value: tenantModel.TenantId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            TenantModel tenantModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("TenantForm")
                        .Class("main-form")
                        .Action(tenantModel.TenantId != 0
                            ? Navigations.Action("Tenants", tenantModel.TenantId)
                            : Navigations.Action("Tenants")),
                    action: () => hb
                        .RecordHeader(
                            baseModel: tenantModel,
                            tableName: "Tenants")
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: tenantModel.Comments,
                                verType: tenantModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(tenantModel: tenantModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                tenantModel: tenantModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: tenantModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: tenantModel.VerType,
                                referenceType: "Tenants",
                                referenceId: tenantModel.TenantId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        tenantModel: tenantModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: tenantModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Tenants_Timestamp",
                            css: "must-transport",
                            value: tenantModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: tenantModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("Tenants", tenantModel.TenantId, tenantModel.Ver)
                .CopyDialog("Tenants", tenantModel.TenantId)
                .OutgoingMailDialog()
                .EditorExtensions(tenantModel: tenantModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, TenantModel tenantModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: tenantModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            TenantModel tenantModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "Ver": hb.Field(siteSettings, column, tenantModel.MethodType, tenantModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Title": hb.Field(siteSettings, column, tenantModel.MethodType, tenantModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Body": hb.Field(siteSettings, column, tenantModel.MethodType, tenantModel.Body.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(tenantModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            TenantModel tenantModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            TenantModel tenantModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static List<int> GetSwitchTargets(SiteSettings siteSettings)
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
                    statements: Rds.SelectTenants(
                        column: Rds.TenantsColumn().TenantId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Tenants",
                            formData: formData,
                            where: Rds.TenantsWhere().TenantId(Sessions.TenantId())),
                        orderBy: GridSorters.Get(
                            formData, Rds.TenantsOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["TenantId"].ToInt())
                                .ToList();    
            }
            return switchTargets;
        }
    }
}
