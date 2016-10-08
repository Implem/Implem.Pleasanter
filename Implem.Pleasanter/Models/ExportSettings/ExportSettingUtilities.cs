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
    public static class ExportSettingUtilities
    {
        public static string EditorNew()
        {
            return Editor(
                new ExportSettingModel(
                    SiteSettingsUtility.ExportSettingsSiteSettings(),
                    Permissions.Admins(),
                    methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long exportSettingId, bool clearSessions)
        {
            var exportSettingModel = new ExportSettingModel(
                    SiteSettingsUtility.ExportSettingsSiteSettings(),
                    Permissions.Admins(),
                exportSettingId: exportSettingId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            exportSettingModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtility.ExportSettingsSiteSettings());
            return Editor(exportSettingModel);
        }

        public static string Editor(ExportSettingModel exportSettingModel)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            return hb.Template(
                permissionType: permissionType,
                verType: exportSettingModel.VerType,
                methodType: exportSettingModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    exportSettingModel.AccessStatus != Databases.AccessStatuses.NotFound,
                referenceType: "ExportSettings",
                title: exportSettingModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.ExportSettings() + " - " + Displays.New()
                    : exportSettingModel.Title.Value,
                action: () =>
                {
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
            return hb.Div(id: "Editor", action: () => hb
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
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: exportSettingModel.Comments,
                                verType: exportSettingModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(exportSettingModel: exportSettingModel)
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

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, ExportSettingModel exportSettingModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
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
                        controlWrapperCss: " h300",
                        listItemCollection: exportSettings.ExportColumnHash(siteSettings),
                        selectedValueCollection: new List<string>(),
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ColumnToUp",
                                    controlCss: "button-icon",
                                    text: Displays.MoveUp(),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-circle-triangle-n",
                                    action: "Set",
                                    method: "post")
                                .Button(
                                    controlId: "ColumnToDown",
                                    controlCss: "button-icon",
                                    text: Displays.MoveDown(),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-circle-triangle-s",
                                    action: "Set",
                                    method: "post")
                                .Button(
                                    controlId: "ColumnToVisible",
                                    controlCss: "button-icon",
                                    text: Displays.Output(),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-image",
                                    action: "Set",
                                    method: "put")
                                .Button(
                                    controlId: "ColumnToHide",
                                    controlCss: "button-icon",
                                    text: Displays.NotOutput(),
                                    onClick: "$p.send($(this));",
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
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "UpdateOrCreate",
                                method: "put")
                            .Button(
                                controlId: "DeleteExportSettings",
                                controlCss: "button-icon",
                                text: Displays.Delete(),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-trash",
                                action: "Delete",
                                method: "delete",
                                confirm: "ConfirmDelete")));
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
