using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlCommands
    {
        public static HtmlBuilder MainCommands(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            Versions.VerTypes verType,
            long referenceId = 0,
            bool backButton = true,
            bool updateButton = false,
            bool copyButton = false,
            bool moveButton = false,
            bool mailButton = false,
            bool deleteButton = false,
            Action extensions = null)
        {
            return hb.Div(id: "MainCommandsContainer", action: () => hb
                .Div(id: "MainCommands", action: () =>
                {
                    if (backButton)
                    {
                        hb.Button(
                            controlId: "GoBack",
                            text: Displays.GoBack(context: context),
                            controlCss: "button-icon",
                            accessKey: "q",
                            onClick: "$p.back();",
                            icon: "ui-icon-circle-arrow-w");
                    }
                    if (context.Action == "new")
                    {
                        hb.Button(
                            text: Displays.Create(context: context),
                            controlCss: "button-icon validate",
                            accessKey: "s",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "Create",
                            method: "post");
                    }
                    else if (context.CanRead(ss: ss) && verType == Versions.VerTypes.Latest)
                    {
                        hb
                            .Button(
                                text: Displays.Update(context: context),
                                controlCss: "button-icon validate",
                                accessKey: "s",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "Update",
                                method: "put",
                                _using: updateButton && context.CanUpdate(ss: ss))
                            .Button(
                                text: Displays.Copy(context: context),
                                controlCss: "button-icon open-dialog",
                                accessKey: "c",
                                onClick: "$p.openDialog($(this));",
                                icon: "ui-icon-copy",
                                selector: "#CopyDialog",
                                _using: copyButton && context.CanCreate(ss: ss))
                            .Button(
                                text: Displays.Move(context: context),
                                controlCss: "button-icon open-dialog",
                                accessKey: "o",
                                onClick: "$p.moveTargets($(this));",
                                icon: "ui-icon-transferthick-e-w",
                                selector: "#MoveDialog",
                                action: "MoveTargets",
                                method: "get",
                                _using: moveButton && context.CanUpdate(ss: ss))
                            .Button(
                                controlId: "EditOutgoingMail",
                                text: Displays.Mail(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.openOutgoingMailDialog($(this));",
                                icon: "ui-icon-mail-closed",
                                action: "Edit",
                                method: "put",
                                accessKey: "m",
                                _using: mailButton && context.CanSendMail(ss: ss))
                            .Button(
                                text: Displays.Delete(context: context),
                                controlCss: "button-icon",
                                accessKey: "r",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-trash",
                                action: "Delete",
                                method: "delete",
                                confirm: "ConfirmDelete",
                                _using: deleteButton
                                    && context.CanDelete(ss: ss)
                                    && !ss.IsSite(context: context))
                            .Button(
                                text: Displays.DeleteSite(context: context),
                                controlCss: "button-icon",
                                accessKey: "r",
                                onClick: "$p.openDeleteSiteDialog($(this));",
                                icon: "ui-icon-trash",
                                _using: deleteButton
                                    && context.CanDelete(ss: ss)
                                    && ss.IsSite(context: context));
                        switch (context.Controller)
                        {
                            case "users":
                                switch (context.Action)
                                {
                                    case "index":
                                        hb
                                            .Button(
                                                controlId: "EditImportSettings",
                                                text: Displays.Import(context: context),
                                                controlCss: "button-icon",
                                                accessKey: "w",
                                                onClick: "$p.openImportSettingsDialog($(this));",
                                                icon: "ui-icon-arrowreturnthick-1-e",
                                                selector: "#ImportSettingsDialog",
                                                _using: context.CanImport(ss: ss))
                                            .Button(
                                                text: Displays.Export(context: context),
                                                controlCss: "button-icon",
                                                accessKey: "x",
                                                onClick: "$p.openExportSelectorDialog($(this));",
                                                icon: "ui-icon-arrowreturnthick-1-w",
                                                action: "OpenExportSelectorDialog",
                                                method: "post",
                                                _using: context.CanExport(ss: ss));
                                        break;
                                }
                                break;
                            case "items":
                                if (ss.ReferenceType != "Sites")
                                {
                                    switch (context.Action)
                                    {
                                        case "index":
                                            hb
                                                .Button(
                                                    text: Displays.BulkMove(context: context),
                                                    controlCss: "button-icon open-dialog",
                                                    accessKey: "o",
                                                    onClick: "$p.moveTargets($(this));",
                                                    icon: "ui-icon-transferthick-e-w",
                                                    selector: "#MoveDialog",
                                                    action: "MoveTargets",
                                                    method: "get",
                                                    _using: context.CanUpdate(ss: ss))
                                                .Button(
                                                    text: Displays.BulkDelete(context: context),
                                                    controlCss: "button-icon",
                                                    accessKey: "r",
                                                    onClick: "$p.send($(this));",
                                                    icon: "ui-icon-trash",
                                                    action: "BulkDelete",
                                                    method: "delete",
                                                    confirm: "ConfirmDelete",
                                                    _using: context.CanDelete(ss: ss))
                                                .Button(
                                                    controlId: "EditImportSettings",
                                                    text: Displays.Import(context: context),
                                                    controlCss: "button-icon",
                                                    accessKey: "w",
                                                    onClick: "$p.openImportSettingsDialog($(this));",
                                                    icon: "ui-icon-arrowreturnthick-1-e",
                                                    selector: "#ImportSettingsDialog",
                                                    _using: context.CanImport(ss: ss))
                                                .Button(
                                                    text: Displays.Export(context: context),
                                                    controlCss: "button-icon",
                                                    accessKey: "x",
                                                    onClick: "$p.openExportSelectorDialog($(this));",
                                                    icon: "ui-icon-arrowreturnthick-1-w",
                                                    action: "OpenExportSelectorDialog",
                                                    method: "post",
                                                    _using: context.CanExport(ss: ss));
                                            break;
                                        case "crosstab":
                                            hb.Button(
                                                text: Displays.Export(context: context),
                                                controlCss: "button-icon",
                                                accessKey: "x",
                                                onClick: "$p.exportCrosstab();",
                                                icon: "ui-icon-arrowreturnthick-1-w",
                                                _using: context.CanExport(ss: ss));
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    extensions?.Invoke();
                }));
        }
    }
}