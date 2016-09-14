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
    public static class ExportSettingUtilities
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData();
            var exportSettingCollection = ExportSettingCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                referenceType: "ExportSettings",
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
                            .Id("ExportSettingsForm")
                            .Class("main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "ExportSettings",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: exportSettingCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    exportSettingCollection: exportSettingCollection,
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
                            .Hidden(controlId: "TableName", value: "ExportSettings")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                .MoveDialog("items", siteSettings.SiteId, bulk: true)
                .Div(attributes: new HtmlAttributes()
                    .Id("ExportSettingsDialog")
                    .Class("dialog")
                    .Title(Displays.ExportSettings())))
                .ToString();
        }

        private static ExportSettingCollection ExportSettingCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new ExportSettingCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "ExportSettings",
                    formData: formData,
                    where: Rds.ExportSettingsWhere()),
                orderBy: GridSorters.Get(
                    formData, Rds.ExportSettingsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            ExportSettingCollection exportSettingCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    exportSettingCollection: exportSettingCollection,
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
            ExportSettingCollection exportSettingCollection,
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
                            exportSettingCollection: exportSettingCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == exportSettingCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var exportSettingCollection = ExportSettingCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    exportSettingCollection: exportSettingCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: exportSettingCollection.Aggregations,
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
            var exportSettingCollection = ExportSettingCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    exportSettingCollection: exportSettingCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: exportSettingCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, exportSettingCollection.Count()))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            ExportSettingCollection exportSettingCollection,
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
                .TBody(action: () => exportSettingCollection
                    .ForEach(exportSettingModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(exportSettingModel.ExportSettingId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: exportSettingModel.ExportSettingId.ToString()));
                                siteSettings.GridColumnCollection()
                                    .ForEach(column => hb
                                        .TdValue(
                                            column: column,
                                            exportSettingModel: exportSettingModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var gridSqlColumn = Rds.ExportSettingsColumn()
                .ExportSettingId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(column =>
                Rds.ExportSettingsColumn(gridSqlColumn, column.ColumnName));
            return gridSqlColumn;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, ExportSettingModel exportSettingModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: exportSettingModel.Ver);
                case "Comments": return hb.Td(column: column, value: exportSettingModel.Comments);
                case "Creator": return hb.Td(column: column, value: exportSettingModel.Creator);
                case "Updator": return hb.Td(column: column, value: exportSettingModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: exportSettingModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: exportSettingModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(
                new ExportSettingModel(
                    SiteSettingsUtility.ExportSettingsSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New),
                byRest: false);
        }

        public static string Editor(long exportSettingId, bool clearSessions)
        {
            var exportSettingModel = new ExportSettingModel(
                    SiteSettingsUtility.ExportSettingsSiteSettings(),
                    Permissions.Admins(),
                exportSettingId: exportSettingId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            exportSettingModel.SwitchTargets = ExportSettingUtilities.GetSwitchTargets(
                SiteSettingsUtility.ExportSettingsSiteSettings());
            return Editor(exportSettingModel, byRest: false);
        }

        public static string Editor(ExportSettingModel exportSettingModel, bool byRest)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            exportSettingModel.SiteSettings.SetChoicesByPlaceholders();
            return hb.Template(
                siteId: 0,
                referenceType: "ExportSettings",
                title: exportSettingModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.ExportSettings() + " - " + Displays.New()
                    : exportSettingModel.Title.Value,
                permissionType: permissionType,
                verType: exportSettingModel.VerType,
                methodType: exportSettingModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    exportSettingModel.AccessStatus != Databases.AccessStatuses.NotFound,
                byRest: byRest,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager;
                    hb
                        .Editor(
                            exportSettingModel: exportSettingModel,
                            permissionType: permissionType,
                            siteSettings: exportSettingModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "ExportSettings")
                        .Hidden(controlId: "Id", value: exportSettingModel.ExportSettingId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            ExportSettingModel exportSettingModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("ExportSettingForm")
                        .Class("main-form")
                        .Action(exportSettingModel.ExportSettingId != 0
                            ? Navigations.Action("ExportSettings", exportSettingModel.ExportSettingId)
                            : Navigations.Action("ExportSettings")),
                    action: () => hb
                        .RecordHeader(
                            baseModel: exportSettingModel,
                            tableName: "ExportSettings")
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: exportSettingModel.Comments,
                                verType: exportSettingModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(exportSettingModel: exportSettingModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                exportSettingModel: exportSettingModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: exportSettingModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: exportSettingModel.VerType,
                                backUrl: Navigations.Index("ExportSettings"),
                                referenceType: "ExportSettings",
                                referenceId: exportSettingModel.ExportSettingId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        exportSettingModel: exportSettingModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: exportSettingModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "ExportSettings_Timestamp",
                            css: "must-transport",
                            value: exportSettingModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: exportSettingModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("ExportSettings", exportSettingModel.ExportSettingId, exportSettingModel.Ver)
                .CopyDialog("ExportSettings", exportSettingModel.ExportSettingId)
                .OutgoingMailDialog()
                .EditorExtensions(exportSettingModel: exportSettingModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, ExportSettingModel exportSettingModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: exportSettingModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ExportSettingModel exportSettingModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "ReferenceType": hb.Field(siteSettings, column, exportSettingModel.MethodType, exportSettingModel.ReferenceType.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ReferenceId": hb.Field(siteSettings, column, exportSettingModel.MethodType, exportSettingModel.ReferenceId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Title": hb.Field(siteSettings, column, exportSettingModel.MethodType, exportSettingModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "ExportSettingId": hb.Field(siteSettings, column, exportSettingModel.MethodType, exportSettingModel.ExportSettingId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, exportSettingModel.MethodType, exportSettingModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "AddHeader": hb.Field(siteSettings, column, exportSettingModel.MethodType, exportSettingModel.AddHeader.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(exportSettingModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            ExportSettingModel exportSettingModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            ExportSettingModel exportSettingModel,
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
                    statements: Rds.SelectExportSettings(
                        column: Rds.ExportSettingsColumn().ExportSettingId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "ExportSettings",
                            formData: formData,
                            where: Rds.ExportSettingsWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.ExportSettingsOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["ExportSettingId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, ExportSettingModel exportSettingModel)
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
        public static string Edit(string referenceType, long referenceId)
        {
            return Edit(new ResponseCollection(), referenceType, referenceId).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection Edit(
            this ResponseCollection responseCollection, string referenceType, long referenceId)
        {
            var exportSettingModel = ExportSetting(referenceType, referenceId);
            ExportSettingUtilities.SetSessions(exportSettingModel);
            var hb = new HtmlBuilder();
            return responseCollection
                .Html("#ExportSettingsDialog", hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ExportSettingsForm")
                            .Action(Navigations.ItemAction(referenceId, "ExportSettings")),
                        action: () => hb
                            .Columns(
                                exportSettingModel.ExportColumns,
                                exportSettingModel.GetSiteSettings())
                            .Settings(referenceType, referenceId)
                            .P(css: "message-dialog")
                            .Commands(referenceType, referenceId)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ExportSettingModel ExportSetting(string referenceType, long referenceId)
        {
            var exportSettingCollection = Collection(referenceType, referenceId);
            return exportSettingCollection.Count > 0
                ? exportSettingCollection.FirstOrDefault()
                : new ExportSettingModel(Permissions.Types.NotSet, referenceType, referenceId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ExportSettingCollection Collection(string referenceType, long referenceId)
        {
            return new ExportSettingCollection(
                SiteSettingsUtility.ExportSettingsSiteSettings(),
                Permissions.Types.NotSet,
                where: Rds.ExportSettingsWhere()
                    .ReferenceType(referenceType)
                    .ReferenceId(referenceId),
                orderBy: Rds.ExportSettingsOrderBy()
                    .Title());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Columns(
            this HtmlBuilder hb, ExportColumns exportSettings, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                css: "fieldset enclosed-auto w500 h400",
                legendText: Displays.SettingColumnList(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "ExportSettings_Columns",
                        fieldCss: "field-vertical w500",
                        controlContainerCss: "container-selectable",
                        controlCss: " h300",
                        listItemCollection: exportSettings.ExportColumnHash(siteSettings),
                        selectedValueCollection: new List<string>(),
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ColumnToUp",
                                    controlCss: "button-icon",
                                    text: Displays.MoveUp(),
                                    onClick: "$p.send($(this), 'ExportSettingsForm');",
                                    icon: "ui-icon-circle-triangle-n",
                                    action: "Set",
                                    method: "post")
                                .Button(
                                    controlId: "ColumnToDown",
                                    controlCss: "button-icon",
                                    text: Displays.MoveDown(),
                                    onClick: "$p.send($(this), 'ExportSettingsForm');",
                                    icon: "ui-icon-circle-triangle-s",
                                    action: "Set",
                                    method: "post")
                                .Button(
                                    controlId: "ColumnToVisible",
                                    controlCss: "button-icon",
                                    text: Displays.Output(),
                                    onClick: "$p.send($(this), 'ExportSettingsForm');",
                                    icon: "ui-icon-image",
                                    action: "Set",
                                    method: "put")
                                .Button(
                                    controlId: "ColumnToHide",
                                    controlCss: "button-icon",
                                    text: Displays.NotOutput(),
                                    onClick: "$p.send($(this), 'ExportSettingsForm');",
                                    icon: "ui-icon-close",
                                    action: "Set",
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Settings(this HtmlBuilder hb, string referenceType, long referenceId)
        {
            var exportSettingCollection = new ExportSettingCollection(
                SiteSettingsUtility.ExportSettingsSiteSettings(),
                Permissions.Types.NotSet,
                where: Rds.ExportSettingsWhere()
                    .ReferenceType(referenceType)
                    .ReferenceId(referenceId));
            var exportSettingModel = exportSettingCollection.FirstOrDefault();
            return hb.FieldSet(
                css: "fieldset enclosed-auto w400 h400",
                legendText: Displays.ExportSettings(),
                action: () => hb
                    .FieldDropDown(
                        controlId: "ExportSettings_ExportSettingId",
                        controlCss: " auto-postback",
                        labelText: Displays.ExportSettings_ExportSettingId(),
                        optionCollection: exportSettingCollection.ToDictionary(
                            o => o.ExportSettingId.ToString(),
                            o => new ControlData(o.Title.Value)),
                        action: "Change",
                        method: "put")
                    .FieldTextBox(
                        controlId: "ExportSettings_Title",
                        controlCss: " must-transport",
                        labelText: Displays.ExportSettings_Title(),
                        text: exportSettingModel != null
                            ? exportSettingModel.Title.Value
                            : string.Empty)
                    .FieldCheckBox(
                        controlId: "ExportSettings_AddHeader",
                        controlCss: " must-transport",
                        labelText: Displays.ExportSettings_AddHeader(),
                        _checked: exportSettingModel != null
                            ? exportSettingModel.AddHeader
                            : true,
                        labelPositionIsRight: true)
                    .Div(
                        css: "command-field",
                            action: () => hb
                            .Button(
                                controlId: "UpdateExportSettings",
                                controlCss: "button-icon",
                                text: Displays.Save(),
                                onClick: "$p.send($(this), 'ExportSettingsForm');",
                                icon: "ui-icon-disk",
                                action: "UpdateOrCreate",
                                method: "put")
                            .Button(
                                controlId: "DeleteExportSettings",
                                controlCss: "button-icon",
                                text: Displays.Delete(),
                                onClick: "$p.send($(this), 'ExportSettingsForm');",
                                icon: "ui-icon-trash",
                                action: "Delete",
                                method: "delete",
                                confirm: "Displays_ConfirmDelete")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Commands(
            this HtmlBuilder hb, string referenceType, long referenceId)
        {
            return hb.Div(
                css: "command-center",
                action: () => hb
                    .Button(
                        controlId: "Export",
                        controlCss: "button-icon",
                        text: Displays.Export(),
                        onClick: "$p.export($(this));",
                        icon: "ui-icon-arrowreturnthick-1-w",
                        action: "Set",
                        method: "put")
                    .Button(
                        controlId: "CancelExport",
                        controlCss: "button-icon",
                        text: Displays.Cancel(),
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Change()
        {
            var exportSettingModel = new ExportSettingModel(
                SiteSettingsUtility.ExportSettingsSiteSettings(),
                Permissions.Types.NotSet).Get(
                    where: Rds.ExportSettingsWhere()
                        .ExportSettingId(Forms.Long("ExportSettings_ExportSettingId")));
            SetSessions(exportSettingModel);
            exportSettingModel.Session_ExportColumns(Jsons.ToJson(exportSettingModel.ExportColumns));
            return new HtmlBuilder()
                .SelectableItems(listItemCollection: exportSettingModel.ExportColumnHash())
                .Html("#ExportSettings_Columns")
                .Val("#ExportSettings_Title", exportSettingModel.Title.Value)
                .Val("#ExportSettings_AddHeader", exportSettingModel.AddHeader)
                .ClearFormData()
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetSessions(ExportSettingModel exportSettingModel)
        {
            exportSettingModel.Session_Title(exportSettingModel.Title);
            exportSettingModel.Session_AddHeader(exportSettingModel.AddHeader);
            exportSettingModel.Session_ExportColumns(Jsons.ToJson(exportSettingModel.ExportColumns));
        }
    }
}
