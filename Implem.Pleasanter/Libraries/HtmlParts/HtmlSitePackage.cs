using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.SitePackages;
using System.Collections.Generic;
using static Implem.ParameterAccessor.Parts.Types;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlSitePackage
    {
        public static HtmlBuilder ImportSitePackageDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("SitePackageForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.File,
                        controlId: "Import",
                        fieldCss: "field-wide",
                        labelText: Displays.SitePackage(context: context))
                    .FieldCheckBox(
                        controlId: "IncludeData",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.IncludeData(context: context),
                        _checked: Parameters.SitePackage.IncludeDataOnImport == OptionTypes.On,
                        _using: context.ContractSettings.Import != false
                            && Parameters.SitePackage.IncludeDataOnImport != OptionTypes.Disabled)
                    .FieldCheckBox(
                        controlId: "IncludeSitePermission",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.IncludeSitePermission(context: context),
                        _checked: Parameters.SitePackage.IncludeSitePermissionOnImport == OptionTypes.On,
                        _using: Parameters.SitePackage.IncludeSitePermissionOnImport != OptionTypes.Disabled)
                    .FieldCheckBox(
                        controlId: "IncludeRecordPermission",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.IncludeRecordPermission(context: context),
                        _checked: Parameters.SitePackage.IncludeRecordPermissionOnImport == OptionTypes.On,
                        _using: Parameters.SitePackage.IncludeRecordPermissionOnImport != OptionTypes.Disabled)
                    .FieldCheckBox(
                        controlId: "IncludeColumnPermission",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.IncludeColumnPermission(context: context),
                        _checked: Parameters.SitePackage.IncludeColumnPermissionOnImport == OptionTypes.On,
                        _using: Parameters.SitePackage.IncludeColumnPermissionOnImport != OptionTypes.Disabled)
                    .FieldCheckBox(
                        controlId: "IncludeNotifications",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.IncludeNotifications(context: context),
                        _checked: Parameters.SitePackage.IncludeNotificationsOnImport == OptionTypes.On,
                        _using: Parameters.SitePackage.IncludeNotificationsOnImport != OptionTypes.Disabled)
                    .FieldCheckBox(
                        controlId: "IncludeReminders",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.IncludeReminders(context: context),
                        _checked: Parameters.SitePackage.IncludeRemindersOnImport == OptionTypes.On,
                        _using: Parameters.SitePackage.IncludeRemindersOnImport != OptionTypes.Disabled)
                    .P(css: "message-dialog")
                    .Div(
                        css: "command-center",
                        action: () => hb
                            .Button(
                                text: Displays.Import(context: context),
                                controlCss: "button-icon button-positive",
                                onClick: "$p.importSitePackage($(this));",
                                icon: "ui-icon-arrowreturnthick-1-e",
                                action: "ImportSitePackage",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon button-neutral",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel")));
        }

        public static HtmlBuilder ExportSitePackageDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, bool recursive = false)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("SitePackageForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldSet(
                        css: " enclosed-thin",
                        legendText: Displays.ExportSettings(context: context),
                        action: () => hb
                            .FieldSelectable(
                                controlId: "SitePackagesSelectable",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h350",
                                controlCss: " always-send send-all",
                                labelText: Displays.CurrentSettings(context: context),
                                listItemCollection: Utilities.SitePackageSelectableOptions(
                                    context: context,
                                    siteId: ss.SiteId,
                                    recursive: recursive),
                                selectedValueCollection: new List<string>(),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-right", action: () => hb
                                        .Button(
                                            controlId: "IncludeData",
                                            controlCss: "button-icon post",
                                            text: Displays.IncludeData(context: context),
                                            onClick: "$p.setIncludeExportData($(this));",
                                            icon: "ui-icon-check",
                                            _using: context.ContractSettings.Export != false
                                                && Parameters.SitePackage.IncludeDataOnExport != OptionTypes.Disabled)
                                        .Button(
                                            controlId: "ExcludeData",
                                            controlCss: "button-icon post",
                                            text: Displays.ExcludeData(context: context),
                                            onClick: "$p.setIncludeExportData($(this));",
                                            icon: "ui-icon-cancel",
                                            _using: context.ContractSettings.Export != false
                                                && Parameters.SitePackage.IncludeDataOnExport != OptionTypes.Disabled)
                                        .Button(
                                            controlId: "ToDisableExportSites",
                                            controlCss: "button-icon",
                                            text: Displays.ToDisable(context: context),
                                            onClick: "$p.siteSelected($(this),$('#SitePackagesSource'));",
                                            icon: "ui-icon-circle-triangle-e")))
                            .FieldSelectable(
                                controlId: "SitePackagesSource",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h350",
                                labelText: Displays.OptionList(context: context),
                                listItemCollection: new Dictionary<string, ControlData>(),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-left", action: () => hb
                                        .Button(
                                            controlId: "ToEnableExportSites",
                                            text: Displays.ToEnable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.siteSelected($(this),$('#SitePackagesSelectable'));",
                                            icon: "ui-icon-circle-triangle-w"))))
                        .FieldCheckBox(
                            controlId: "UseIndentOption",
                            fieldCss: "field-wide",
                            controlCss: " always-send",
                            labelText: Displays.UseIndentOption(context: context),
                            _checked: Parameters.SitePackage.UseIndentOptionOnExport == OptionTypes.On,
                            _using: Parameters.SitePackage.UseIndentOptionOnExport != OptionTypes.Disabled)
                        .FieldCheckBox(
                            controlId: "IncludeSitePermission",
                            fieldCss: "field-wide",
                            controlCss: " always-send",
                            labelText: Displays.IncludeSitePermission(context: context),
                            _checked: Parameters.SitePackage.IncludeSitePermissionOnExport == OptionTypes.On,
                            _using: Parameters.SitePackage.IncludeSitePermissionOnExport != OptionTypes.Disabled)
                        .FieldCheckBox(
                            controlId: "IncludeRecordPermission",
                            fieldCss: "field-wide",
                            controlCss: " always-send",
                            labelText: Displays.IncludeRecordPermission(context: context),
                            _checked: Parameters.SitePackage.IncludeRecordPermissionOnExport == OptionTypes.On,
                            _using: Parameters.SitePackage.IncludeRecordPermissionOnExport != OptionTypes.Disabled)
                        .FieldCheckBox(
                            controlId: "IncludeColumnPermission",
                            fieldCss: "field-wide",
                            controlCss: " always-send",
                            labelText: Displays.IncludeColumnPermission(context: context),
                            _checked: Parameters.SitePackage.IncludeColumnPermissionOnExport == OptionTypes.On,
                            _using: Parameters.SitePackage.IncludeColumnPermissionOnExport != OptionTypes.Disabled)
                        .FieldCheckBox(
                            controlId: "IncludeNotifications",
                            fieldCss: "field-wide",
                            controlCss: " always-send",
                            labelText: Displays.IncludeNotifications(context: context),
                            _checked: Parameters.SitePackage.IncludeNotificationsOnExport == OptionTypes.On,
                            _using: Parameters.SitePackage.IncludeNotificationsOnExport != OptionTypes.Disabled)
                        .FieldCheckBox(
                            controlId: "IncludeReminders",
                            fieldCss: "field-wide",
                            controlCss: " always-send",
                            labelText: Displays.IncludeReminders(context: context),
                            _checked: Parameters.SitePackage.IncludeRemindersOnExport == OptionTypes.On,
                            _using: Parameters.SitePackage.IncludeRemindersOnExport != OptionTypes.Disabled)
                        .P(css: "message-dialog")
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                text: Displays.Export(context: context),
                                controlCss: "button-icon button-positive",
                                onClick: "$p.exportSitePackage();",
                                icon: "ui-icon-arrowreturnthick-1-w",
                                action: "ExportSitePackage",
                                method: "get")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon button-neutral",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel")));
        }
    }
}