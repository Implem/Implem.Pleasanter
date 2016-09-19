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
    public static class DeptUtilities
    {
        private static string DataViewTemplate(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            DeptCollection deptCollection,
            FormData formData,
            string dataViewName,
            Action dataViewBody)
        {
            return hb.Template(
                siteId: siteSettings.SiteId,
                referenceType: "Depts",
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
                            .Id("DeptsForm")
                            .Class("main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: deptCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => dataViewBody())
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
                            .Hidden(controlId: "TableName", value: "Depts")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                .MoveDialog("items", siteSettings.SiteId, bulk: true)
                .Div(attributes: new HtmlAttributes()
                    .Id("ExportSettingsDialog")
                    .Class("dialog")
                    .Title(Displays.ExportSettings())))
                .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData();
            var deptCollection = DeptCollection(siteSettings, permissionType, formData);
            return hb.Template(
                siteId: 0,
                referenceType: "Depts",
                title: Displays.Depts() + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: Sessions.User().TenantAdmin,
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("DeptForm")
                                .Class("main-form")
                                .Action(Navigations.Action("Depts")),
                            action: () => hb
                                .Aggregations(
                                    siteSettings: siteSettings,
                                    aggregations: deptCollection.Aggregations)
                                .Div(id: "DataViewContainer", action: () => hb
                                    .Grid(
                                        deptCollection: deptCollection,
                                        permissionType: permissionType,
                                        siteSettings: siteSettings,
                                        formData: formData))
                                .MainCommands(
                                    siteId: siteSettings.SiteId,
                                    permissionType: permissionType,
                                    verType: Versions.VerTypes.Latest,
                                    backUrl: Navigations.Index("Admins"))
                                .Div(css: "margin-bottom")
                                .Hidden(controlId: "TableName", value: "Depts")
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

        public static string IndexJson(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var deptCollection = DeptCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    deptCollection: deptCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: deptCollection.Aggregations,
                    container: false))
                .WindowScrollTop().ToJson();
        }

        private static DeptCollection DeptCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new DeptCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Depts",
                    formData: formData,
                    where: Rds.DeptsWhere().TenantId(Sessions.TenantId())),
                orderBy: GridSorters.Get(
                    formData, Rds.DeptsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            DeptCollection deptCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    deptCollection: deptCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            DeptCollection deptCollection,
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
                            deptCollection: deptCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == deptCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
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
            var deptCollection = DeptCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    deptCollection: deptCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: deptCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, deptCollection.Count()))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            DeptCollection deptCollection,
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
                .TBody(action: () => deptCollection
                    .ForEach(deptModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(deptModel.DeptId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: deptModel.DeptId.ToString()));
                                siteSettings.GridColumnCollection()
                                    .ForEach(column => hb
                                        .TdValue(
                                            column: column,
                                            deptModel: deptModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var gridSqlColumn = Rds.DeptsColumn()
                .DeptId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(column =>
                Rds.DeptsColumn(gridSqlColumn, column.ColumnName));
            return gridSqlColumn;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, DeptModel deptModel)
        {
            switch (column.ColumnName)
            {
                case "DeptId": return hb.Td(column: column, value: deptModel.DeptId);
                case "Ver": return hb.Td(column: column, value: deptModel.Ver);
                case "DeptCode": return hb.Td(column: column, value: deptModel.DeptCode);
                case "Dept": return hb.Td(column: column, value: deptModel.Dept);
                case "Body": return hb.Td(column: column, value: deptModel.Body);
                case "Comments": return hb.Td(column: column, value: deptModel.Comments);
                case "Creator": return hb.Td(column: column, value: deptModel.Creator);
                case "Updator": return hb.Td(column: column, value: deptModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: deptModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: deptModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(
                new DeptModel(
                    SiteSettingsUtility.DeptsSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(int deptId, bool clearSessions)
        {
            var deptModel = new DeptModel(
                    SiteSettingsUtility.DeptsSiteSettings(),
                    Permissions.Admins(),
                deptId: deptId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            deptModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtility.DeptsSiteSettings());
            return Editor(deptModel);
        }

        public static string Editor(DeptModel deptModel)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            return hb.Template(
                siteId: 0,
                referenceType: "Depts",
                title: deptModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Depts() + " - " + Displays.New()
                    : deptModel.Title.Value,
                permissionType: permissionType,
                verType: deptModel.VerType,
                methodType: deptModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    deptModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    hb
                        .Editor(
                            deptModel: deptModel,
                            permissionType: permissionType,
                            siteSettings: deptModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "Depts")
                        .Hidden(controlId: "Id", value: deptModel.DeptId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            DeptModel deptModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("DeptForm")
                        .Class("main-form")
                        .Action(deptModel.DeptId != 0
                            ? Navigations.Action("Depts", deptModel.DeptId)
                            : Navigations.Action("Depts")),
                    action: () => hb
                        .RecordHeader(
                            baseModel: deptModel,
                            tableName: "Depts")
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: deptModel.Comments,
                                verType: deptModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(deptModel: deptModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                deptModel: deptModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: deptModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: deptModel.VerType,
                                backUrl: Navigations.Index("Depts"),
                                referenceType: "Depts",
                                referenceId: deptModel.DeptId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        deptModel: deptModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: deptModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Depts_Timestamp",
                            css: "must-transport",
                            value: deptModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: deptModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("Depts", deptModel.DeptId, deptModel.Ver)
                .CopyDialog("Depts", deptModel.DeptId)
                .OutgoingMailDialog()
                .EditorExtensions(deptModel: deptModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, DeptModel deptModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: deptModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            DeptModel deptModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "TenantId": hb.Field(siteSettings, column, deptModel.MethodType, deptModel.TenantId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DeptId": hb.Field(siteSettings, column, deptModel.MethodType, deptModel.DeptId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, deptModel.MethodType, deptModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DeptCode": hb.Field(siteSettings, column, deptModel.MethodType, deptModel.DeptCode.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "DeptName": hb.Field(siteSettings, column, deptModel.MethodType, deptModel.DeptName.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Body": hb.Field(siteSettings, column, deptModel.MethodType, deptModel.Body.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(deptModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            DeptModel deptModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            DeptModel deptModel,
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
                    statements: Rds.SelectDepts(
                        column: Rds.DeptsColumn().DeptId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Depts",
                            formData: formData,
                            where: Rds.DeptsWhere().TenantId(Sessions.TenantId())),
                        orderBy: GridSorters.Get(
                            formData, Rds.DeptsOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["DeptId"].ToInt())
                                .ToList();    
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, DeptModel deptModel)
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
        public static string GridRows()
        {
            return GridRows(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Admins(),
                offset: DataViewGrid.Offset());
        }
    }
}
