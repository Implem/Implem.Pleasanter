using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class WikiValidators
    {
        public static Error.Types OnCreating(SiteSettings ss, WikiModel wikiModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(wikiModel.Mine());
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Wikis_Title":
                        if (!ss.GetColumn("Title").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Wikis_Body":
                        if (!ss.GetColumn("Body").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(SiteSettings ss, WikiModel wikiModel)
        {
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(wikiModel.Mine());
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Wikis_Title":
                        if (wikiModel.Title_Updated &&
                            !ss.GetColumn("Title").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Wikis_Body":
                        if (wikiModel.Body_Updated &&
                            !ss.GetColumn("Body").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "SearchPermissionElements":
                    case "OpenPermissionsDialog":
                    case "AddPermissions":
                    case "DeletePermissions":
                    case "InheritPermission":
                        if (!ss.CanManagePermission())
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, WikiModel wikiModel)
        {
            if (!ss.CanDelete())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnRestoring()
        {
            if (!Permissions.CanManageTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnExporting(SiteSettings ss)
        {
            if (!ss.CanExport())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
