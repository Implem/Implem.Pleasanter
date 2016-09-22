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
    public static class BinaryUtilities
    {
        private static string DataViewTemplate(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            BinaryCollection binaryCollection,
            FormData formData,
            string dataViewName,
            Action dataViewBody)
        {
            return hb.Template(
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                siteId: siteSettings.SiteId,
                parentId: siteSettings.ParentId,
                referenceType: "Binaries",
                title: siteSettings.Title + " - " + Displays.List(),
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
                            .Id("BinariesForm")
                            .Class("main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: binaryCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => dataViewBody())
                            .MainCommands(
                                siteId: siteSettings.SiteId,
                                permissionType: permissionType,
                                verType: Versions.VerTypes.Latest,
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Binaries")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                .MoveDialog("items", siteSettings.SiteId, bulk: true)
                .Div(attributes: new HtmlAttributes()
                    .Id("ExportSettingsDialog")
                    .Class("dialog")
                    .Title(Displays.ExportSettings())))
                .ToString();
        }

        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData();
            var binaryCollection = BinaryCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.DataViewTemplate(
                siteSettings: siteSettings,
                permissionType: permissionType,
                binaryCollection: binaryCollection,
                formData: formData,
                dataViewName: dataViewName,
                dataViewBody: () => hb.Grid(
                   binaryCollection: binaryCollection,
                   siteSettings: siteSettings,
                   permissionType: permissionType,
                   formData: formData));
        }

        public static string IndexJson(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var binaryCollection = BinaryCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    binaryCollection: binaryCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: binaryCollection.Aggregations,
                    container: false))
                .WindowScrollTop().ToJson();
        }

        private static BinaryCollection BinaryCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new BinaryCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Binaries",
                    formData: formData,
                    where: Rds.BinariesWhere()),
                orderBy: GridSorters.Get(
                    formData, Rds.BinariesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            BinaryCollection binaryCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    binaryCollection: binaryCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            BinaryCollection binaryCollection,
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
                            binaryCollection: binaryCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == binaryCollection.Count()
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
            var binaryCollection = BinaryCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    binaryCollection: binaryCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: binaryCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, binaryCollection.Count()))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            BinaryCollection binaryCollection,
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
                .TBody(action: () => binaryCollection
                    .ForEach(binaryModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(binaryModel.BinaryId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: binaryModel.BinaryId.ToString()));
                                siteSettings.GridColumnCollection()
                                    .ForEach(column => hb
                                        .TdValue(
                                            column: column,
                                            binaryModel: binaryModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var gridSqlColumn = Rds.BinariesColumn()
                .BinaryId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(column =>
                Rds.BinariesColumn(gridSqlColumn, column.ColumnName));
            return gridSqlColumn;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, BinaryModel binaryModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: binaryModel.Ver);
                case "Comments": return hb.Td(column: column, value: binaryModel.Comments);
                case "Creator": return hb.Td(column: column, value: binaryModel.Creator);
                case "Updator": return hb.Td(column: column, value: binaryModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: binaryModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: binaryModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(
                new BinaryModel(
                    SiteSettingsUtility.BinariesSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long binaryId, bool clearSessions)
        {
            var binaryModel = new BinaryModel(
                    SiteSettingsUtility.BinariesSiteSettings(),
                    Permissions.Admins(),
                binaryId: binaryId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            binaryModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtility.BinariesSiteSettings());
            return Editor(binaryModel);
        }

        public static string Editor(BinaryModel binaryModel)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            return hb.Template(
                permissionType: permissionType,
                verType: binaryModel.VerType,
                methodType: binaryModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    binaryModel.AccessStatus != Databases.AccessStatuses.NotFound,
                referenceType: "Binaries",
                title: binaryModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Binaries() + " - " + Displays.New()
                    : binaryModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            binaryModel: binaryModel,
                            permissionType: permissionType,
                            siteSettings: binaryModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "Binaries")
                        .Hidden(controlId: "Id", value: binaryModel.BinaryId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            BinaryModel binaryModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("BinaryForm")
                        .Class("main-form")
                        .Action(binaryModel.BinaryId != 0
                            ? Navigations.Action("Binaries", binaryModel.BinaryId)
                            : Navigations.Action("Binaries")),
                    action: () => hb
                        .RecordHeader(
                            baseModel: binaryModel,
                            tableName: "Binaries")
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: binaryModel.Comments,
                                verType: binaryModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(binaryModel: binaryModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                binaryModel: binaryModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: binaryModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: binaryModel.VerType,
                                referenceType: "Binaries",
                                referenceId: binaryModel.BinaryId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        binaryModel: binaryModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: binaryModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Binaries_Timestamp",
                            css: "must-transport",
                            value: binaryModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: binaryModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("Binaries", binaryModel.BinaryId, binaryModel.Ver)
                .CopyDialog("Binaries", binaryModel.BinaryId)
                .OutgoingMailDialog()
                .EditorExtensions(binaryModel: binaryModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, BinaryModel binaryModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: binaryModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            BinaryModel binaryModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "ReferenceId": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.ReferenceId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "BinaryId": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.BinaryId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "BinaryType": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.BinaryType.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Title": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Body": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Body.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "FileName": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.FileName.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Extension": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Extension.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Size": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Size.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(binaryModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            BinaryModel binaryModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            BinaryModel binaryModel,
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
                    statements: Rds.SelectBinaries(
                        column: Rds.BinariesColumn().BinaryId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Binaries",
                            formData: formData,
                            where: Rds.BinariesWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.BinariesOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["BinaryId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, BinaryModel binaryModel)
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
