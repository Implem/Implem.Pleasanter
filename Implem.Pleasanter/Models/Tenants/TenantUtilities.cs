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
    public static class TenantUtilities
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData();
            var tenantCollection = TenantCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                referenceType: "Tenants",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: Libraries.Scripts.JavaScripts.DataView(
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                userScript: siteSettings.GridScript,
                userStyle: siteSettings.GridStyle,
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id_Css("TenantsForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "Tenants",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: tenantCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    tenantCollection: tenantCollection,
                                    siteSettings: siteSettings,
                                    permissionType: permissionType,
                                    formData: formData,
                                    dataViewName: dataViewName))
                            .MainCommands(
                                siteId: siteSettings.SiteId,
                                permissionType: permissionType,
                                verType: Versions.VerTypes.Latest,
                                backUrl: Navigations.Index("Admins"),
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Tenants")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                .MoveDialog("items", siteSettings.SiteId, bulk: true)
                .Div(attributes: new HtmlAttributes()
                    .Id_Css("ExportSettingsDialog", "dialog")
                    .Title(Displays.ExportSettings())))
                .ToString();
        }

        private static TenantCollection TenantCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new TenantCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Tenants",
                    formData: formData,
                    where: Rds.TenantsWhere().TenantId(Sessions.TenantId())),
                orderBy: GridSorters.Get(
                    formData, Rds.TenantsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            TenantCollection tenantCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    tenantCollection: tenantCollection,
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
            TenantCollection tenantCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id_Css("Grid", "grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            tenantCollection: tenantCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == tenantCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var tenantCollection = TenantCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    tenantCollection: tenantCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: tenantCollection.Aggregations,
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
            var tenantCollection = TenantCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    tenantCollection: tenantCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: tenantCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, tenantCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            TenantCollection tenantCollection,
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
                .TBody(action: () => tenantCollection
                    .ForEach(tenantModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(tenantModel.TenantId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: tenantModel.TenantId.ToString()));
                                siteSettings.GridColumnCollection()
                                    .ForEach(column => hb
                                        .TdValue(
                                            column: column,
                                            tenantModel: tenantModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var gridSqlColumn = Rds.TenantsColumn()
                .TenantId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(column =>
                Rds.TenantsColumn(gridSqlColumn, column.ColumnName));
            return gridSqlColumn;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, TenantModel tenantModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: tenantModel.Ver);
                case "TenantName": return hb.Td(column: column, value: tenantModel.TenantName);
                case "Title": return hb.Td(column: column, value: tenantModel.Title);
                case "Body": return hb.Td(column: column, value: tenantModel.Body);
                case "Comments": return hb.Td(column: column, value: tenantModel.Comments);
                case "Creator": return hb.Td(column: column, value: tenantModel.Creator);
                case "Updator": return hb.Td(column: column, value: tenantModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: tenantModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: tenantModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(
                new TenantModel(
                    SiteSettingsUtility.TenantsSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New),
                byRest: false);
        }

        public static string Editor(int tenantId, bool clearSessions)
        {
            var tenantModel = new TenantModel(
                    SiteSettingsUtility.TenantsSiteSettings(),
                    Permissions.Admins(),
                tenantId: tenantId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            tenantModel.SwitchTargets = TenantUtilities.GetSwitchTargets(
                SiteSettingsUtility.TenantsSiteSettings());
            return Editor(tenantModel, byRest: false);
        }

        public static string Editor(TenantModel tenantModel, bool byRest)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            tenantModel.SiteSettings.SetChoicesByPlaceholders();
            return hb.Template(
                siteId: 0,
                referenceType: "Tenants",
                title: tenantModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Tenants() + " - " + Displays.New()
                    : tenantModel.Title.Value,
                permissionType: permissionType,
                verType: tenantModel.VerType,
                methodType: tenantModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    tenantModel.AccessStatus != Databases.AccessStatuses.NotFound,
                byRest: byRest,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager;
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
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id_Css("TenantForm", "main-form")
                        .Action(tenantModel.TenantId != 0
                            ? Navigations.Action("Tenants", tenantModel.TenantId)
                            : Navigations.Action("Tenants")),
                    action: () => hb
                        .RecordHeader(
                            id: tenantModel.TenantId,
                            baseModel: tenantModel,
                            tableName: "Tenants",
                            switchTargets: tenantModel.SwitchTargets?
                                .Select(o => o.ToLong()).ToList())
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: tenantModel.Comments,
                                verType: tenantModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(tenantModel: tenantModel)
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
                                backUrl: Navigations.Index("Tenants"),
                                referenceType: "Tenants",
                                referenceId: tenantModel.TenantId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        tenantModel: tenantModel,
                                        siteSettings: siteSettings)))
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
                            value: tenantModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("Tenants", tenantModel.TenantId, tenantModel.Ver)
                .CopyDialog("Tenants", tenantModel.TenantId)
                .OutgoingMailDialog()
                .EditorExtensions(tenantModel: tenantModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, TenantModel tenantModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
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

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, TenantModel tenantModel)
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
