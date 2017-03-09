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
        public static string Set(string reference, long id)
        {
            var ss = SiteSettingsUtilities.GetByReference(reference, id);
            return new ExportSettingModel(ss.ReferenceType, id).Set(ss, id);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UpdateOrCreate(string reference, long id)
        {
            var ss = SiteSettingsUtilities.GetByReference(reference, id);
            var exportSettingModel = new ExportSettingModel(
                ss.ReferenceType, id, withTitle: true);
            var invalid = ExportSettingValidator.OnExporting(ss);
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
                        where: Rds.ExportSettingsWhere()
                            .ReferenceId(exportSettingModel.ReferenceId))
                                .Select(o => o.Title?.Value),
                    Displays.Setting()));
            }
            var error = exportSettingModel.UpdateOrCreate(
                where: Rds.ExportSettingsWhere()
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
                        optionCollection: Collection(ss.ReferenceType, id).ToDictionary(
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
        public static string Delete(string reference, long id)
        {
            var ss = SiteSettingsUtilities.GetByReference(reference, id);
            var exportSettingModel = new ExportSettingModel(ss.ReferenceType, id, withTitle: true);
            var invalid = ExportSettingValidator.OnExporting(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = exportSettingModel.Delete();
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorJson(ss, reference, id);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string EditorJson(string reference, long id)
        {
            return EditorJson(SiteSettingsUtilities.GetByReference(reference, id), reference, id);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string EditorJson(SiteSettings ss, string reference, long id)
        {
            var invalid = ExportSettingValidator.OnExporting(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            return EditorResponse(new ResponseCollection(), ss, reference, id).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection EditorResponse(
            this ResponseCollection res, SiteSettings ss, string reference, long id)
        {
            var exportSettingModel = ExportSetting(ss.ReferenceType, id);
            SetSessions(exportSettingModel);
            var hb = new HtmlBuilder();
            return res
                .Html("#ExportSettingsDialog", hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ExportSettingsForm")
                            .Action(Locations.Action(
                                reference, id, "ExportSettings")),
                        action: () => hb
                            .Columns(
                                exportSettingModel.ExportColumns,
                                exportSettingModel.GetSiteSettings())
                            .Settings(ss.ReferenceType, id)
                            .P(css: "message-dialog")
                            .Commands()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ExportSettingModel ExportSetting(string reference, long id)
        {
            var exportSettingCollection = Collection(reference, id);
            return exportSettingCollection.Count > 0
                ? exportSettingCollection.FirstOrDefault()
                : new ExportSettingModel(reference, id);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ExportSettingCollection Collection(string referenceType, long id)
        {
            return new ExportSettingCollection(
                where: Rds.ExportSettingsWhere()
                    .ReferenceType(referenceType)
                    .ReferenceId(id),
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
        private static HtmlBuilder Settings(this HtmlBuilder hb, string referenceType, long id)
        {
            var exportSettingCollection = new ExportSettingCollection(
                where: Rds.ExportSettingsWhere()
                    .ReferenceType(referenceType)
                    .ReferenceId(id));
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
        private static HtmlBuilder Commands(this HtmlBuilder hb)
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
        public static string Change(string reference, long id)
        {
            var exportSettingModel = new ExportSettingModel()
                .Get(where: Rds.ExportSettingsWhere()
                    .ExportSettingId(Forms.Long("ExportSettings_ExportSettingId")));
            var invalid = ExportSettingValidator.OnExporting(
                SiteSettingsUtilities.GetByReference(reference, id));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
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
