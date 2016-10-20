using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class WikiValidators
    {
        public static Error.Types OnCreating(
            SiteSettings ss, Permissions.Types permissionType, WikiModel wikiModel)
        {
            if (!permissionType.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Wikis_Title":
                        if (!ss.GetColumn("Title").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Wikis_Body":
                        if (!ss.GetColumn("Body").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Wikis_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            SiteSettings ss, Permissions.Types permissionType, WikiModel wikiModel)
        {
            if (!permissionType.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Wikis_Title":
                        if (!ss.GetColumn("Title").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Wikis_Body":
                        if (!ss.GetColumn("Body").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Wikis_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            SiteSettings ss, Permissions.Types permissionType, WikiModel wikiModel)
        {
            if (!permissionType.CanDelete())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnRestoring()
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
