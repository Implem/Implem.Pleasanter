using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Utilities;
using System;
namespace Implem.Pleasanter.Libraries.Views
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
                    if (backUrl != string.Empty)
                    {
                        hb.Button(
                            controlId: "GoBack",
                            text: Displays.GoBack(),
                            controlCss: "button-goback",
                            accessKey: "q",
                            href: backUrl);
                    }
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
                            if (updateButton && permissionType.CanUpdate())
                            {
                                hb.Button(
                                    text: Displays.Update(),
                                    controlCss: "button-save validate",
                                    accessKey: "s",
                                    onClick: Def.JavaScript.Update,
                                    action: "Update",
                                    method: "put");
                            }
                            if (copyButton && permissionType.CanCreate())
                            {
                                hb.Button(
                                    text: Displays.Copy(),
                                    controlCss: "button-copy open-dialog",
                                    accessKey: "c",
                                    onClick: Def.JavaScript.OpenDialog,
                                    selector: "#Dialog_ConfirmCopy");
                            }
                            if (moveButton && permissionType.CanUpdate())
                            {
                                hb.Button(
                                    text: Displays.Move(),
                                    controlCss: "button-move open-dialog",
                                    accessKey: "o",
                                    onClick: Def.JavaScript.MoveTargets,
                                    action: "MoveTargets",
                                    method: "get");
                            }
                            if (bulkMoveButton && permissionType.CanUpdate())
                            {
                                hb.Button(
                                    text: Displays.BulkMove(),
                                    controlCss: "button-move open-dialog",
                                    accessKey: "o",
                                    onClick: Def.JavaScript.MoveTargets,
                                    action: "MoveTargets",
                                    method: "get");
                            }
                            if (mailButton && permissionType.CanUpdate())
                            {
                                hb.Button(
                                    controlId: "EditOutgoingMail",
                                    text: Displays.Mail(),
                                    controlCss: "button-send-mail",
                                    onClick: Def.JavaScript.EditOutgoingMail,
                                    action: Navigations.Edit(
                                        referenceType,
                                        referenceId,
                                        "OutgoingMails"),
                                    method: "put",
                                    accessKey: "m");
                            }
                            if (deleteButton && permissionType.CanDelete())
                            {
                                hb.Button(
                                    text: Displays.Delete(),
                                    controlCss: "button-delete",
                                    accessKey: "r",
                                    onClick: Def.JavaScript.Delete,
                                    action: "Delete",
                                    method: "delete",
                                    confirm: "Displays_ConfirmDelete");
                            }
                            if (bulkDeleteButton && permissionType.CanDelete())
                            {
                                hb.Button(
                                    text: Displays.BulkDelete(),
                                    controlCss: "button-delete",
                                    accessKey: "r",
                                    onClick: Def.JavaScript.Delete,
                                    action: "BulkDelete",
                                    method: "delete",
                                    confirm: "Displays_ConfirmDelete");
                            }
                            if (importButton && permissionType.CanImport())
                            {
                                hb.Button(
                                    controlId: "EditImportSettings",
                                    text: Displays.Import(),
                                    controlCss: "button-import",
                                    accessKey: "w",
                                    onClick: Def.JavaScript.EditImportSettings,
                                    selector: "#Dialog_ImportSettings");
                            }
                            if (exportButton && permissionType.CanExport())
                            {
                                hb.Button(
                                    controlId: "EditExportSettings",
                                    text: Displays.Export(),
                                    controlCss: "button-export",
                                    accessKey: "x",
                                    onClick: Def.JavaScript.EditExportSettings,
                                    action: Navigations.ItemAction(
                                        siteId, "ExportSettings", "Edit"),
                                    method: "put");
                            }
                        }
                    }
                    if (extensions != null) extensions();
                }));
        }
    }
}