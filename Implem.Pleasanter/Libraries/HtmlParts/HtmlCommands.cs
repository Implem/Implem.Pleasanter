using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Items;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlCommands
    {
        public static HtmlBuilder MainCommands(
            this HtmlBuilder hb,
            long siteId,
            Permissions.Types permissionType,
            Versions.VerTypes verType,
            string backUrl,
            string referenceType = "",
            long referenceId = 0,
            bool updateButton = false,
            bool copyButton = false,
            bool moveButton = false,
            bool bulkMoveButton = false,
            bool mailButton = false,
            bool deleteButton = false,
            bool bulkDeleteButton = false,
            bool importButton = false,
            bool exportButton = false,
            Action extensions = null)
        {
            return hb.Div(css: "command-main-container", action: () => hb
                .Div(css: "command-main", action: () =>
                {
                    hb.Button(
                        controlId: "GoBack",
                        text: Displays.GoBack(),
                        controlCss: "button-goback",
                        accessKey: "q",
                        href: backUrl,
                        _using: backUrl != string.Empty);
                    var routesAction = Routes.Action();
                    if (permissionType.CanRead() && 
                        verType == Versions.VerTypes.Latest)
                    {
                        if (routesAction == "new" || routesAction == "newbylink")
                        {
                            hb.Button(
                                text: Displays.Create(),
                                controlCss: "button-save validate",
                                accessKey: "s",
                                onClick: Def.JavaScript.Create,
                                action: "Create",
                                method: "post");
                        }
                        else
                        {
                            hb
                                .Button(
                                    text: Displays.Update(),
                                    controlCss: "button-save validate",
                                    accessKey: "s",
                                    onClick: Def.JavaScript.Update,
                                    action: "Update",
                                    method: "put",
                                    _using: updateButton && permissionType.CanUpdate())
                                .Button(
                                    text: Displays.Copy(),
                                    controlCss: "button-copy open-dialog",
                                    accessKey: "c",
                                    onClick: Def.JavaScript.OpenDialog,
                                    selector: "#Dialog_ConfirmCopy",
                                    _using: copyButton && permissionType.CanCreate())
                                .Button(
                                    text: Displays.Move(),
                                    controlCss: "button-move open-dialog",
                                    accessKey: "o",
                                    onClick: Def.JavaScript.MoveTargets,
                                    action: "MoveTargets",
                                    method: "get",
                                    _using: moveButton && permissionType.CanUpdate())
                                .Button(
                                    text: Displays.BulkMove(),
                                    controlCss: "button-move open-dialog",
                                    accessKey: "o",
                                    onClick: Def.JavaScript.MoveTargets,
                                    action: "MoveTargets",
                                    method: "get",
                                    _using: bulkMoveButton && permissionType.CanUpdate())
                                .Button(
                                    controlId: "EditOutgoingMail",
                                    text: Displays.Mail(),
                                    controlCss: "button-send-mail",
                                    onClick: Def.JavaScript.EditOutgoingMail,
                                    action: Navigations.Edit(
                                        referenceType,
                                        referenceId,
                                        "OutgoingMails"),
                                    method: "put",
                                    accessKey: "m",
                                    _using: mailButton && permissionType.CanUpdate())
                                .Button(
                                    text: Displays.Delete(),
                                    controlCss: "button-delete",
                                    accessKey: "r",
                                    onClick: Def.JavaScript.Delete,
                                    action: "Delete",
                                    method: "delete",
                                    confirm: "Displays_ConfirmDelete",
                                    _using: deleteButton && permissionType.CanDelete())
                                .Button(
                                    text: Displays.BulkDelete(),
                                    controlCss: "button-delete",
                                    accessKey: "r",
                                    onClick: Def.JavaScript.Delete,
                                    action: "BulkDelete",
                                    method: "delete",
                                    confirm: "Displays_ConfirmDelete",
                                    _using: bulkDeleteButton && permissionType.CanDelete())
                                .Button(
                                    controlId: "EditImportSettings",
                                    text: Displays.Import(),
                                    controlCss: "button-import",
                                    accessKey: "w",
                                    onClick: Def.JavaScript.EditImportSettings,
                                    selector: "#Dialog_ImportSettings",
                                    _using: importButton && permissionType.CanImport())
                                .Button(
                                    controlId: "EditExportSettings",
                                    text: Displays.Export(),
                                    controlCss: "button-export",
                                    accessKey: "x",
                                    onClick: Def.JavaScript.EditExportSettings,
                                    action: Navigations.ItemAction(
                                        siteId, "ExportSettings", "Edit"),
                                    method: "put",
                                    _using: exportButton && permissionType.CanExport());
                        }
                    }
                    if (extensions != null) extensions();
                }));
        }
    }
}