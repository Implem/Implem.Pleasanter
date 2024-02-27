using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlCommands
    {
        public static HtmlBuilder MainCommands(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view = null,
            Versions.VerTypes verType = Versions.VerTypes.Latest,
            bool readOnly = false,
            bool backButton = true,
            bool updateButton = false,
            bool copyButton = false,
            bool moveButton = false,
            bool mailButton = false,
            bool deleteButton = false,
            ServerScriptModelRow serverScriptModelRow = null,
            Action extensions = null)
        {
            if (context.Controller == "items"
                && !ss.IsSite(context: context))
            {
                view = view ?? Views.GetBySession(
                    context: context,
                    ss: ss);
            }
            return hb.Div(id: "MainCommandsContainer", action: () => hb
                .Div(id: "MainCommands", action: () =>
                {
                    if (backButton)
                    {
                        hb.Button(
                            controlId: "GoBack",
                            text: Strings.CoalesceEmpty(
                                serverScriptModelRow?.Elements?.LabelText("GoBack"),
                                Displays.GoBack(context: context)),
                            controlCss: "button-icon button-neutral",
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
                                    controlCss: "button-icon validate button-positive",
                                    text: Displays.Invite(context: context),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-mail-closed",
                                    action: "Create",
                                    method: "post");
                                break;
                            default:
                                hb.Button(
                                    serverScriptModelRow: serverScriptModelRow,
                                    commandDisplayTypes: view?.CreateCommand,
                                    controlId: "CreateCommand",
                                    text: Displays.Create(context: context),
                                    controlCss: "button-icon validate button-positive",
                                    accessKey: "s",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "Create",
                                    method: "post");
                                break;
                        }
                        extensions?.Invoke();
                    }
                    else if (verType == Versions.VerTypes.Latest)
                    {
                        switch (context.Controller)
                        {
                            case "syslogs":
                                hb.Common(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    readOnly: readOnly,
                                    updateButton: updateButton,
                                    copyButton: copyButton,
                                    moveButton: moveButton,
                                    mailButton: mailButton,
                                    deleteButton: deleteButton,
                                    serverScriptModelRow: serverScriptModelRow);
                                switch (context.Action)
                                {
                                    case "index":
                                        hb
                                            .Button(
                                                controlId: "OpenExportSelectorDialogCommand",
                                                text: Displays.Export(context: context),
                                                controlCss: "button-icon button-positive",
                                                accessKey: "x",
                                                onClick: "$p.openExportSelectorDialog($(this));",
                                                icon: "ui-icon-arrowreturnthick-1-w",
                                                action: "OpenExportSelectorDialog",
                                                method: "post",
                                                _using: context.HasPrivilege);
                                        break;
                                }
                                break;
                            case "users":
                                hb.Common(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    readOnly: readOnly,
                                    updateButton: updateButton,
                                    copyButton: copyButton,
                                    moveButton: moveButton,
                                    mailButton: mailButton,
                                    deleteButton: deleteButton,
                                    serverScriptModelRow: serverScriptModelRow);
                                switch (context.Action)
                                {
                                    case "index":
                                        hb
                                            .Button(
                                                controlId: "BulkDeleteCommand",
                                                text: Displays.BulkDelete(context: context),
                                                controlCss: "button-icon button-negative",
                                                accessKey: "r",
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-trash",
                                                action: "BulkDelete",
                                                method: "delete",
                                                confirm: "ConfirmDelete",
                                                _using: context.CanDelete(ss: ss)
                                                    && !readOnly)
                                            .Button(
                                                controlId: "EditImportSettings",
                                                text: Displays.Import(context: context),
                                                controlCss: "button-icon button-positive",
                                                accessKey: "w",
                                                onClick: "$p.openImportSettingsDialog($(this));",
                                                icon: "ui-icon-arrowreturnthick-1-e",
                                                selector: "#ImportSettingsDialog",
                                                _using: context.CanImport(ss: ss)
                                                    && !readOnly)
                                            .Button(
                                                controlId: "OpenExportSelectorDialogCommand",
                                                text: Displays.Export(context: context),
                                                controlCss: "button-icon button-positive",
                                                accessKey: "x",
                                                onClick: "$p.openExportSelectorDialog($(this));",
                                                icon: "ui-icon-arrowreturnthick-1-w",
                                                action: "OpenExportSelectorDialog",
                                                method: "post",
                                                _using: context.CanExport(ss: ss));
                                        break;
                                }
                                break;
                            case "depts":
                            case "groups":
                                hb.Common(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    readOnly: readOnly,
                                    updateButton: updateButton,
                                    copyButton: copyButton,
                                    moveButton: moveButton,
                                    mailButton: mailButton,
                                    deleteButton: deleteButton,
                                    serverScriptModelRow: serverScriptModelRow);
                                switch (context.Action)
                                {
                                    case "index":
                                        hb
                                            .Button(
                                                controlId: "BulkDeleteCommand",
                                                text: Displays.BulkDelete(context: context),
                                                controlCss: "button-icon button-negative",
                                                accessKey: "r",
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-trash",
                                                action: "BulkDelete",
                                                method: "delete",
                                                confirm: "ConfirmDelete",
                                                _using: context.CanDelete(ss: ss)
                                                    && !readOnly)
                                            .Button(
                                                controlId: "EditImportSettings",
                                                text: Displays.Import(context: context),
                                                controlCss: "button-icon button-positive",
                                                accessKey: "w",
                                                onClick: "$p.openImportSettingsDialog($(this));",
                                                icon: "ui-icon-arrowreturnthick-1-e",
                                                selector: "#ImportSettingsDialog",
                                                _using: context.CanImport(ss: ss)
                                                    && !readOnly)
                                            .Button(
                                                controlId: "OpenExportSelectorDialogCommand",
                                                text: Displays.Export(context: context),
                                                controlCss: "button-icon button-positive",
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
                                            controlCss: "button-icon validate button-positive",
                                            text: Strings.CoalesceEmpty(
                                                serverScriptModelRow?.Elements?.LabelText("BulkDeleteCommand"),
                                                Displays.ApprovalRequest(context: context)),
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
                                                controlCss: "button-icon button-positive",
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-mail-closed",
                                                action: "Approval",
                                                method: "put",
                                                _using: Permissions.PrivilegedUsers(loginId: context.LoginId))
                                            .Button(
                                                controlId: "DeleteCommand",
                                                text: Displays.Delete(context: context),
                                                controlCss: "button-icon button-negative",
                                                accessKey: "r",
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-trash",
                                                action: "Delete",
                                                method: "delete",
                                                confirm: "ConfirmDelete",
                                                _using: deleteButton
                                                    && context.CanDelete(ss: ss)
                                                    && !ss.IsSite(context: context)
                                                    && !readOnly);
                                        break;
                                    case "index":
                                        hb.Button(
                                            controlId: "BulkDeleteCommand",
                                            text: Strings.CoalesceEmpty(
                                                serverScriptModelRow?.Elements?.LabelText("BulkDeleteCommand"),
                                                Displays.BulkDelete(context: context)),
                                            controlCss: "button-icon button-negative",
                                            accessKey: "r",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-trash",
                                            action: "BulkDelete",
                                            method: "delete",
                                            confirm: "ConfirmDelete",
                                            _using: context.CanDelete(ss: ss)
                                                && !readOnly);
                                        break;
                                }
                                break;
                            case "items":
                                if (ss.ReferenceType == "Dashboards"
                                    && context.Action == "index")
                                {
                                    hb
                                        .Button(
                                            controlId: "UpdateDashboardPartLayouts",
                                            text: Displays.SaveLayout(context: context),
                                            controlCss: "button-icon button-positive",
                                            accessKey: "s",
                                            icon: "ui-icon-disk",
                                            onClick: "$p.updateDashboardPartLayouts();",
                                            action: "Update",
                                            method: "put",
                                            _using: context.CanUpdate(ss: ss));
                                    break;
                                }
                                hb.Common(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    readOnly: readOnly,
                                    updateButton: updateButton,
                                    copyButton: copyButton,
                                    moveButton: moveButton,
                                    mailButton: mailButton,
                                    deleteButton: deleteButton,
                                    serverScriptModelRow: serverScriptModelRow);
                                if (context.Forms.Bool("EditOnGrid"))
                                {
                                    hb
                                        .Button(
                                            serverScriptModelRow: serverScriptModelRow,
                                            commandDisplayTypes: view?.UpdateCommand,
                                            controlId: "UpdateByGridCommand",
                                            text: Displays.Update(context: context),
                                            controlCss: "button-icon validate button-positive",
                                            accessKey: "s",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-disk",
                                            action: "UpdateByGrid",
                                            method: "post",
                                            _using: context.CanUpdate(ss: ss))
                                        .Button(
                                            serverScriptModelRow: serverScriptModelRow,
                                            commandDisplayTypes: view?.EditOnGridCommand,
                                            controlId: "EditOnGridCommand",
                                            text: Displays.ListMode(context: context),
                                            controlCss: "button-icon button-neutral",
                                            onClick: "$p.editOnGrid($(this),0);",
                                            icon: "ui-icon-arrowreturnthick-1-w",
                                            action: "Index",
                                            method: "post");
                                }
                                else if (ss.ReferenceType != "Sites")
                                {
                                    switch (context.Action)
                                    {
                                        case "index":
                                            var bulkProcessingItems = ss.BulkProcessingItems(
                                                context: context,
                                                ss: ss);
                                            var process = ss.GetProcess(
                                                context: context,
                                                id: context.Forms.Int("BulkProcessingItems"));
                                            if (process == null)
                                            {
                                                hb
                                                    .Button(
                                                        serverScriptModelRow: serverScriptModelRow,
                                                        commandDisplayTypes: view?.BulkMoveTargetsCommand,
                                                        controlId: "BulkMoveTargetsCommand",
                                                        text: Displays.BulkMove(context: context),
                                                        controlCss: "button-icon open-dialog button-positive",
                                                        accessKey: "o",
                                                        onClick: "$p.moveTargets($(this));",
                                                        icon: "ui-icon-transferthick-e-w",
                                                        selector: "#MoveDialog",
                                                        action: "MoveTargets",
                                                        method: "get",
                                                        _using: ss.MoveTargets?.Any() == true
                                                            && context.CanUpdate(ss: ss)
                                                            && !ss.GridColumnsHasSources(gridColumns: view?.GridColumns)
                                                            && ss.MoveTargetsOptions(sites: ss.NumberOfMoveTargetsTable(context: context))
                                                                .Any(o => ss.MoveTargets.Contains(o.Key.ToLong()))
                                                            && !readOnly)
                                                    .Button(
                                                        serverScriptModelRow: serverScriptModelRow,
                                                        commandDisplayTypes: view?.BulkDeleteCommand,
                                                        controlId: "BulkDeleteCommand",
                                                        text: Displays.BulkDelete(context: context),
                                                        controlCss: "button-icon button-negative",
                                                        accessKey: "r",
                                                        onClick: "$p.send($(this));",
                                                        icon: "ui-icon-trash",
                                                        action: "BulkDelete",
                                                        method: "delete",
                                                        confirm: "ConfirmDelete",
                                                        _using: context.CanDelete(ss: ss)
                                                            && !ss.GridColumnsHasSources(gridColumns: view?.GridColumns)
                                                            && !readOnly)
                                                    .Button(
                                                        serverScriptModelRow: serverScriptModelRow,
                                                        commandDisplayTypes: view?.EditImportSettings,
                                                        controlId: "EditImportSettings",
                                                        text: Displays.Import(context: context),
                                                        controlCss: "button-icon button-positive",
                                                        accessKey: "w",
                                                        onClick: "$p.openImportSettingsDialog($(this));",
                                                        icon: "ui-icon-arrowreturnthick-1-e",
                                                        selector: "#ImportSettingsDialog",
                                                        _using: context.CanImport(ss: ss)
                                                            && !readOnly)
                                                    .Button(
                                                        serverScriptModelRow: serverScriptModelRow,
                                                        commandDisplayTypes: view?.OpenExportSelectorDialogCommand,
                                                        controlId: "OpenExportSelectorDialogCommand",
                                                        text: Displays.Export(context: context),
                                                        controlCss: "button-icon button-positive",
                                                        accessKey: "x",
                                                        onClick: "$p.openExportSelectorDialog($(this));",
                                                        icon: "ui-icon-arrowreturnthick-1-w",
                                                        action: "OpenExportSelectorDialog",
                                                        method: "post",
                                                        _using: context.CanExport(ss: ss)
                                                            && ExportUtilities.HasExportableTemplates(
                                                                context: context,
                                                                ss: ss))
                                                    .Button(
                                                        serverScriptModelRow: serverScriptModelRow,
                                                        commandDisplayTypes: view?.OpenBulkUpdateSelectorDialogCommand,
                                                        controlId: "OpenBulkUpdateSelectorDialogCommand",
                                                        text: Displays.BulkUpdate(context: context),
                                                        controlCss: "button-icon button-positive",
                                                        accessKey: "s",
                                                        onClick: "$p.openBulkUpdateSelectorDialog($(this));",
                                                        icon: "ui-icon-disk",
                                                        action: "OpenBulkUpdateSelectorDialog",
                                                        method: "post",
                                                        _using: context.CanUpdate(ss: ss)
                                                            && ss.GetAllowBulkUpdateOptions(context: context)?.Any() == true
                                                            && !readOnly)
                                                    .Button(
                                                        serverScriptModelRow: serverScriptModelRow,
                                                        commandDisplayTypes: view?.EditOnGridCommand,
                                                        controlId: "EditOnGridCommand",
                                                        text: Displays.EditMode(context: context),
                                                        controlCss: "button-icon button-positive",
                                                        onClick: "$p.editOnGrid($(this),1);",
                                                        icon: "ui-icon-arrowreturnthick-1-w",
                                                        action: "Index",
                                                        method: "post",
                                                        _using: ss.GridEditorType == SiteSettings.GridEditorTypes.Grid
                                                            && context.CanUpdate(ss: ss)
                                                            && !ss.GridColumnsHasSources(gridColumns: view?.GridColumns)
                                                            && ss.IntegratedSites?.Any() != true
                                                            && !readOnly);
                                            }
                                            hb.DropDown(
                                                context: context,
                                                controlId: "BulkProcessingItems",
                                                controlCss: " auto-postback always-send w150",
                                                optionCollection: bulkProcessingItems,
                                                selectedValue: process?.Id.ToString(),
                                                method: "post",
                                                _using: bulkProcessingItems?.Any() == true);
                                            if (process != null)
                                            {
                                                hb
                                                    .Button(
                                                        controlId: "BulkProcessCommand",
                                                        text: Displays.Execute(context: context),
                                                        controlCss: "button-icon button-positive",
                                                        onClick: "$p.send($(this));",
                                                        icon: "ui-icon-disk",
                                                        action: "BulkProcess",
                                                        method: "post",
                                                        confirm: process.ConfirmationMessage)
                                                    .Button(
                                                        controlId: "BulkProcessCancelCommand",
                                                        text: Displays.ListMode(context: context),
                                                        controlCss: "button-icon button-neutral",
                                                        onClick: "$('#BulkProcessingItems').val('').change();",
                                                        icon: "ui-icon-arrowreturnthick-1-w");
                                            }
                                            break;
                                        case "crosstab":
                                            hb.Button(
                                                serverScriptModelRow: serverScriptModelRow,
                                                commandDisplayTypes: view?.ExportCrosstabCommand,
                                                controlId: "ExportCrosstabCommand",
                                                text: Displays.Export(context: context),
                                                controlCss: "button-icon button-positive",
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
                                    view: view,
                                    readOnly: readOnly,
                                    updateButton: updateButton,
                                    copyButton: copyButton,
                                    moveButton: moveButton,
                                    mailButton: mailButton,
                                    deleteButton: deleteButton,
                                    serverScriptModelRow: serverScriptModelRow);
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
            View view,
            bool readOnly,
            bool updateButton,
            bool copyButton,
            bool moveButton,
            bool mailButton,
            bool deleteButton,
            ServerScriptModelRow serverScriptModelRow)
        {
            return hb
                .Button(
                    serverScriptModelRow: serverScriptModelRow,
                    commandDisplayTypes: view?.UpdateCommand,
                    controlId: "UpdateCommand",
                    text: Displays.Update(context: context),
                    controlCss: "button-icon validate button-positive",
                    accessKey: "s",
                    onClick: "$p.send($(this));",
                    icon: "ui-icon-disk",
                    action: "Update",
                    method: "put",
                    _using: updateButton
                        && context.CanUpdate(ss: ss)
                        && !readOnly)
                .Button(
                    serverScriptModelRow: serverScriptModelRow,
                    commandDisplayTypes: view?.OpenCopyDialogCommand,
                    controlId: "OpenCopyDialogCommand",
                    text: Displays.Copy(context: context),
                    controlCss: "button-icon open-dialog button-positive",
                    accessKey: "c",
                    onClick: "$p.openDialog($(this));",
                    icon: "ui-icon-copy",
                    selector: "#CopyDialog",
                    _using: copyButton
                        && context.CanCreate(ss: ss)
                        && (ss.IsSite(context: context) || ss.AllowCopy == true))
                .Button(
                    serverScriptModelRow: serverScriptModelRow,
                    commandDisplayTypes: view?.ReferenceCopyCommand,
                    controlId: "ReferenceCopyCommand",
                    text: Displays.ReferenceCopy(context: context),
                    controlCss: "button-icon button-positive",
                    accessKey: "k",
                    onClick: $"location.href='{Locations.ItemNew(context: context, id: ss.SiteId)}?CopyFrom={context.Id}'",
                    icon: "ui-icon-copy",
                    _using: copyButton
                        && context.CanCreate(ss: ss)
                        && !ss.IsSite(context: context)
                        && ss.AllowReferenceCopy == true)
                .Button(
                    serverScriptModelRow: serverScriptModelRow,
                    commandDisplayTypes: view?.MoveTargetsCommand,
                    controlId: "MoveTargetsCommand",
                    text: Displays.Move(context: context),
                    controlCss: "button-icon open-dialog button-positive",
                    accessKey: "o",
                    onClick: "$p.moveTargets($(this));",
                    icon: "ui-icon-transferthick-e-w",
                    selector: "#MoveDialog",
                    action: "MoveTargets",
                    method: "get",
                    _using: moveButton
                        && ss.MoveTargets?.Any() == true
                        && context.CanUpdate(ss: ss)
                        && ss.MoveTargetsOptions(sites: ss.NumberOfMoveTargetsTable(context: context))
                            .Any(o => ss.MoveTargets.Contains(o.Key.ToLong()))
                        && !readOnly)
                .Button(
                    serverScriptModelRow:serverScriptModelRow,
                    commandDisplayTypes: view?.EditOutgoingMail,
                    controlId: "EditOutgoingMail",
                    text: Displays.Mail(context: context),
                    controlCss: "button-icon button-positive",
                    onClick: "$p.openOutgoingMailDialog($(this));",
                    icon: "ui-icon-mail-closed",
                    action: "Edit",
                    method: "put",
                    accessKey: "m",
                    _using: mailButton
                        && context.CanSendMail(ss: ss))
                .Button(
                    serverScriptModelRow: serverScriptModelRow,
                    commandDisplayTypes: view?.DeleteCommand,
                    controlId: "DeleteCommand",
                    text: Displays.Delete(context: context),
                    controlCss: "button-icon button-negative",
                    accessKey: "r",
                    onClick: "$p.send($(this));",
                    icon: "ui-icon-trash",
                    action: "Delete",
                    method: "delete",
                    confirm: "ConfirmDelete",
                    _using: deleteButton
                        && context.CanDelete(ss: ss)
                        && !ss.IsSite(context: context)
                        && !readOnly)
                .Button(
                    serverScriptModelRow: serverScriptModelRow,
                    commandDisplayTypes: view?.OpenDeleteSiteDialogCommand,
                    controlId: "OpenDeleteSiteDialogCommand",
                    text: Displays.DeleteSite(context: context),
                    controlCss: "button-icon button-negative",
                    accessKey: "r",
                    onClick: "$p.openDeleteSiteDialog($(this));",
                    icon: "ui-icon-trash",
                    _using: deleteButton
                        && context.CanDelete(ss: ss)
                        && ss.IsSite(context: context)
                        && !readOnly);
        }

        private static HtmlBuilder Button(
            this HtmlBuilder hb,
            ServerScriptModelRow serverScriptModelRow,
            View.CommandDisplayTypes? commandDisplayTypes,
            string controlId = null,
            string text = null,
            string controlCss = null,
            string title = null,
            string accessKey = null,
            string onClick = null,
            string href = null,
            string dataId = null,
            string icon = null,
            string selector = null,
            string action = null,
            string method = null,
            string confirm = null,
            string type = "button",
            bool _using = true)
        {
            var serverScriptElements = serverScriptModelRow?.Elements;
            return hb.Button(
                controlId: controlId,
                text: Strings.CoalesceEmpty(
                    serverScriptElements?.LabelText(controlId),
                    text),
                controlCss: controlCss,
                style: (serverScriptElements != null
                    ? serverScriptElements.Hidden(controlId) == true
                    : commandDisplayTypes == View.CommandDisplayTypes.Hidden)
                        ? "display:none;"
                        : string.Empty,
                title: title,
                accessKey: accessKey,
                onClick: onClick,
                href: href,
                dataId: dataId,
                icon: icon,
                selector: selector,
                action: action,
                method: method,
                confirm: confirm,
                type: type,
                disabled: serverScriptElements != null 
                    ? serverScriptElements.Disabled(controlId) == true
                    : commandDisplayTypes == View.CommandDisplayTypes.Disabled,
                _using: _using
                    && serverScriptModelRow?.Elements?.None(controlId) != true
                    && commandDisplayTypes != View.CommandDisplayTypes.None);
        }
    }
}