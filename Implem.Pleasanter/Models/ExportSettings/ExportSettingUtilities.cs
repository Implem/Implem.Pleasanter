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
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UpdateOrCreate(
            SiteSettings ss, string referenceType, long referenceId)
        {
            var exportSettingModel = new ExportSettingModel(
                referenceType, referenceId, withTitle: true);
            var invalid = ExportSettingValidator.OnUpdatingOrCreating(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (exportSettingModel.ExportColumns.ReferenceType.IsNullOrEmpty())
            {
                exportSettingModel.ExportColumns.ReferenceType = exportSettingModel.ReferenceType;
            }
            if (Forms.Data("ExportSettings_Title") == string.Empty)
            {
                exportSettingModel.Title = new Title(0, Unique.New(
                    new ExportSettingCollection(
                        SiteSettingsUtilities.ExportSettingsSiteSettings(),
                        where: Rds.ExportSettingsWhere()
                            .ReferenceId(exportSettingModel.ReferenceId))
                                .Select(o => o.Title?.Value),
                    Displays.Setting()));
            }
            var error = exportSettingModel.UpdateOrCreate(where:
                Rds.ExportSettingsWhere()
                    .ReferenceId(exportSettingModel.ReferenceId)
                    .Title(exportSettingModel.Title.Value));
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return new ResponseCollection()
                    .Html(
                        "#ExportSettings_ExportSettingId",
                        new HtmlBuilder().OptionCollection(
                        optionCollection: Collection(referenceType, referenceId).ToDictionary(
                            o => o.ExportSettingId.ToString(),
                            o => new ControlData(o.Title.Value)),
                        selectedValue: exportSettingModel.ExportSettingId.ToString(),
                        addSelectedValue: false))
                    .Message(Messages.Updated(exportSettingModel.Title.ToString()))
                    .Val("#ExportSettings_Title", exportSettingModel.Title.Value)
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Delete(string referenceType, long referenceId)
        {
            var exportSettingModel = new ExportSettingModel(
                referenceType,
                referenceId,
                withTitle: true);
            var error = exportSettingModel.Delete();
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorJson(referenceType, referenceId);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string EditorJson(string referenceType, long referenceId)
        {
            return EditorResponse(new ResponseCollection(), referenceType, referenceId).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection EditorResponse(
            this ResponseCollection res, string referenceType, long referenceId)
        {
            var exportSettingModel = ExportSetting(referenceType, referenceId);
            SetSessions(exportSettingModel);
            var hb = new HtmlBuilder();
            return res
                .Html("#ExportSettingsDialog", hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ExportSettingsForm")
                            .Action(Locations.Action(
                                referenceType, referenceId, "ExportSettings")),
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
                : new ExportSettingModel(referenceType, referenceId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ExportSettingCollection Collection(string referenceType, long referenceId)
        {
            return new ExportSettingCollection(
                SiteSettingsUtilities.ExportSettingsSiteSettings(),
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
            this HtmlBuilder hb, ExportColumns exportSettings, SiteSettings ss)
        {
            return hb.FieldSet(
                css: "fieldset enclosed-auto w500 h400",
                legendText: Displays.ColumnList(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "ExportSettings_Columns",
                        fieldCss: "field-vertical w500",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h300",
                        listItemCollection: exportSettings.ExportColumnHash(ss),
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
                SiteSettingsUtilities.ExportSettingsSiteSettings(),
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
                        controlCss: " always-send",
                        labelText: Displays.ExportSettings_Title(),
                        text: exportSettingModel != null
                            ? exportSettingModel.Title.Value
                            : string.Empty)
                    .FieldCheckBox(
                        controlId: "ExportSettings_AddHeader",
                        controlCss: " always-send",
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
                SiteSettingsUtilities.ExportSettingsSiteSettings())
                    .Get(where: Rds.ExportSettingsWhere()
                        .ExportSettingId(Forms.Long("ExportSettings_ExportSettingId")));
            SetSessions(exportSettingModel);
            exportSettingModel.Session_ExportColumns(Jsons.ToJson(exportSettingModel.ExportColumns));
            return new ResponseCollection()
                .Html("#ExportSettings_Columns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: exportSettingModel.ExportColumnHash()))
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
