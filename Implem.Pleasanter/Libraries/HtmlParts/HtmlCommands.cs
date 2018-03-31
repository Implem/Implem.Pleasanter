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
                            text: Displays.GoBack(),
                            controlCss: "button-icon",
                            accessKey: "q",
                            onClick: "$p.back();",
                            icon: "ui-icon-circle-arrow-w");
                    }
                    if (Routes.Action() == "new")
                    {
                        hb.Button(
                            text: Displays.Create(),
                            controlCss: "button-icon validate",
                            accessKey: "s",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "Create",
                            method: "post");
                    }
                    else if (ss.CanRead() && verType == Versions.VerTypes.Latest)
                    {
                        hb
                            .Button(
                                text: Displays.Update(),
                                controlCss: "button-icon validate",
                                accessKey: "s",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "Update",
                                method: "put",
                                _using: updateButton && ss.CanUpdate())
                            .Button(
                                text: Displays.Copy(),
                                controlCss: "button-icon open-dialog",
                                accessKey: "c",
                                onClick: "$p.openDialog($(this));",
                                icon: "ui-icon-copy",
                                selector: "#CopyDialog",
                                _using: copyButton && ss.CanCreate())
                            .Button(
                                text: Displays.Move(),
                                controlCss: "button-icon open-dialog",
                                accessKey: "o",
                                onClick: "$p.moveTargets($(this));",
                                icon: "ui-icon-transferthick-e-w",
                                selector: "#MoveDialog",
                                action: "MoveTargets",
                                method: "get",
                                _using: moveButton && ss.CanUpdate())
                            .Button(
                                controlId: "EditOutgoingMail",
                                text: Displays.Mail(),
                                controlCss: "button-icon",
                                onClick: "$p.openOutgoingMailDialog($(this));",
                                icon: "ui-icon-mail-closed",
                                action: "Edit",
                                method: "put",
                                accessKey: "m",
                                _using: mailButton && ss.CanSendMail())
                            .Button(
                                text: Displays.Delete(),
                                controlCss: "button-icon",
                                accessKey: "r",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-trash",
                                action: "Delete",
                                method: "delete",
                                confirm: "ConfirmDelete",
                                _using: deleteButton && ss.CanDelete() && !ss.IsSite())
                            .Button(
                                text: Displays.DeleteSite(),
                                controlCss: "button-icon",
                                accessKey: "r",
                                onClick: "$p.openDeleteSiteDialog($(this));",
                                icon: "ui-icon-trash",
                                _using: deleteButton && ss.CanDelete() && ss.IsSite());
                        if (Routes.Controller() == "items")
                        {
                            switch (Routes.Action())
                            {
                                case "index":
                                    hb
                                        .Button(
                                            text: Displays.BulkMove(),
                                            controlCss: "button-icon open-dialog",
                                            accessKey: "o",
                                            onClick: "$p.moveTargets($(this));",
                                            icon: "ui-icon-transferthick-e-w",
                                            selector: "#MoveDialog",
                                            action: "MoveTargets",
                                            method: "get",
                                            _using: ss.CanUpdate())
                                        .Button(
                                            text: Displays.BulkDelete(),
                                            controlCss: "button-icon",
                                            accessKey: "r",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-trash",
                                            action: "BulkDelete",
                                            method: "delete",
                                            confirm: "ConfirmDelete",
                                            _using: ss.CanDelete())
                                        .Button(
                                            controlId: "EditImportSettings",
                                            text: Displays.Import(),
                                            controlCss: "button-icon",
                                            accessKey: "w",
                                            onClick: "$p.openImportSettingsDialog($(this));",
                                            icon: "ui-icon-arrowreturnthick-1-e",
                                            selector: "#ImportSettingsDialog",
                                            _using: ss.CanImport())
                                        .Button(
                                            text: Displays.Export(),
                                            controlCss: "button-icon",
                                            accessKey: "x",
                                            onClick: "$p.openExportSelectorDialog($(this));",
                                            icon: "ui-icon-arrowreturnthick-1-w",
                                            action: "OpenExportSelectorDialog",
                                            method: "post",
                                            _using: ss.CanExport());
                                    break;
                                case "crosstab":
                                    hb.Button(
                                        text: Displays.Export(),
                                        controlCss: "button-icon",
                                        accessKey: "x",
                                        onClick: "$p.exportCrosstab();",
                                        icon: "ui-icon-arrowreturnthick-1-w",
                                        _using: ss.CanExport());
                                    break;
                            }
                        }
                    }
                    extensions?.Invoke();
                }));
        }
    }
}