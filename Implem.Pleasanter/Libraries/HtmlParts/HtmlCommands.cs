using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlCommands
    {
        public static HtmlBuilder MainCommands(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Versions.VerTypes verType,
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
                        switch (context.Controller)
                        {
                            case "registrations":
                                hb.Button(
                                    controlId: "RegistrationId",
                                    controlCss: "button-icon validate",
                                    text: Displays.Invite(context: context),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-mail-closed",
                                    action: "Create",
                                    method: "post");
                                break;
                            default:
                                hb.Button(
                                    text: Displays.Create(context: context),
                                    controlCss: "button-icon validate",
                                    accessKey: "s",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "Create",
                                    method: "post");
                                break;
                        }
                    }
                    else if (verType == Versions.VerTypes.Latest)
                    {
                        switch (context.Controller)
                        {
                            case "users":
                                hb.Common(
                                    context: context,
                                    ss: ss,
                                    updateButton: updateButton,
                                    copyButton: copyButton,
                                    moveButton: moveButton,
                                    mailButton: mailButton,
                                    deleteButton: deleteButton);
                                switch (context.Action)
                                {
                                    case "index":
                                        hb
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
                                }
                                break;
                            case "registrations":
                                switch (context.Action)
                                {
                                    case "login":
                                        hb.Button(
                                            controlId: "RegistrationId",
                                            controlCss: "button-icon validate",
                                            text: Displays.ApprovalRequest(context: context),
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-mail-closed",
                                            action: "ApprovalRequest",
                                            method: "post");
                                        break;
                                    case "edit":
                                        hb
                                            .Button(
                                                controlId: "RegistrationApproval",
                                                text: Displays.Approval(context: context),
                                                controlCss: "button-icon",
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-mail-closed",
                                                action: "Approval",
                                                method: "put",
                                                _using: Permissions.PrivilegedUsers(loginId: context.LoginId))
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
                                                    && !ss.IsSite(context: context));
                                        break;
                                    case "index":
                                        hb.Button(
                                           text: Displays.BulkDelete(context: context),
                                           controlCss: "button-icon",
                                           accessKey: "r",
                                           onClick: "$p.send($(this));",
                                           icon: "ui-icon-trash",
                                           action: "BulkDelete",
                                           method: "delete",
                                           confirm: "ConfirmDelete",
                                           _using: context.CanDelete(ss: ss));
                                        break;
                                }
                                break;
                            case "items":
                                hb.Common(
                                    context: context,
                                    ss: ss,
                                    updateButton: updateButton,
                                    copyButton: copyButton,
                                    moveButton: moveButton,
                                    mailButton: mailButton,
                                    deleteButton: deleteButton);
                                if (context.Forms.Bool("EditOnGrid"))
                                {
                                    hb
                                        .Button(
                                            text: Displays.Update(context: context),
                                            controlCss: "button-icon validate",
                                            accessKey: "s",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-disk",
                                            action: "UpdateByGrid",
                                            method: "post",
                                            _using: context.CanRead(ss: ss))
                                        .Button(
                                            text: Displays.ListMode(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.editOnGrid($(this),0);",
                                            icon: "ui-icon-arrowreturnthick-1-w",
                                            action: "Index",
                                            method: "post",
                                            _using: context.CanRead(ss: ss));
                                }
                                else if (ss.ReferenceType != "Sites")
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
                                                    _using: ss.MoveTargets?.Any() == true
                                                        && context.CanUpdate(ss: ss)
                                                        && !ss.GridColumnsHasSources())
                                                .Button(
                                                    text: Displays.BulkDelete(context: context),
                                                    controlCss: "button-icon",
                                                    accessKey: "r",
                                                    onClick: "$p.send($(this));",
                                                    icon: "ui-icon-trash",
                                                    action: "BulkDelete",
                                                    method: "delete",
                                                    confirm: "ConfirmDelete",
                                                    _using: context.CanDelete(ss: ss)
                                                        && !ss.GridColumnsHasSources())
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
                                                    _using: context.CanExport(ss: ss))
                                                .Button(
                                                    text: Displays.BulkUpdate(context: context),
                                                    controlCss: "button-icon",
                                                    accessKey: "s",
                                                    onClick: "$p.openBulkUpdateSelectorDialog($(this));",
                                                    icon: "ui-icon-disk",
                                                    action: "OpenBulkUpdateSelectorDialog",
                                                    method: "post",
                                                    _using: context.CanUpdate(ss: ss)
                                                        && ss.GetAllowBulkUpdateColumns(context, ss).Any())
                                                .Button(
                                                    text: Displays.EditMode(context: context),
                                                    controlCss: "button-icon",
                                                    onClick: "$p.editOnGrid($(this),1);",
                                                    icon: "ui-icon-arrowreturnthick-1-w",
                                                    action: "Index",
                                                    method: "post",
                                                    _using: ss.GridEditorType == SiteSettings.GridEditorTypes.Grid
                                                        && context.CanUpdate(ss: ss)
                                                        && !ss.GridColumnsHasSources()
                                                        && ss.IntegratedSites?.Any() != true);
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
                            default:
                                hb.Common(
                                    context: context,
                                    ss: ss,
                                    updateButton: updateButton,
                                    copyButton: copyButton,
                                    moveButton: moveButton,
                                    mailButton: mailButton,
                                    deleteButton: deleteButton);
                                break;
                        }
                        extensions?.Invoke();
                    }
                }));
        }

        private static HtmlBuilder Common(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            bool updateButton = false,
            bool copyButton = false,
            bool moveButton = false,
            bool mailButton = false,
            bool deleteButton = false)
        {
            return hb
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
                    _using: moveButton
                        && ss.MoveTargets?.Any() == true
                        && context.CanUpdate(ss: ss))
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
        }
    }
}