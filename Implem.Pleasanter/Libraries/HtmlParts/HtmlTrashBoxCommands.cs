using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTrashBoxCommands
    {
        public static HtmlBuilder TrashBoxCommands(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(
                css: "command-left",
                action: () => hb
                    .Button(
                        text: Displays.Restore(context: context),
                        controlCss: "button-icon button-positive",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-arrowreturnthick-1-n",
                        action: "Restore",
                        method: "post",
                        confirm: "ConfirmRestore",
                        _using: Parameters.Deleted.Restore)
                    .Button(
                        text: Displays.DeleteFromTrashBox(context: context),
                        controlCss: "button-icon button-negative",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-closethick",
                        action: "PhysicalDelete",
                        method: "delete",
                        confirm: "ConfirmPhysicalDelete",
                        _using: Parameters.Deleted.PhysicalDelete),
                _using: (Parameters.Deleted.Restore || Parameters.Deleted.PhysicalDelete)
                    && Enabled(context: context, ss: ss));
        }

        private static bool Enabled(Context context, SiteSettings ss)
        {
            switch (context.Controller)
            {
                case "items":
                    return context.CanManageSite(ss: ss);
                case "users":
                    return Permissions.CanManageUser(context: context);
                case "groups":
                    return Permissions.CanEditGroup(context: context);
                case "depts":
                    return Permissions.CanManageTenant(context: context)
                        || context.UserSettings.EnableManageTenant == true;
                default:
                    return false;
            }
        }
    }
}