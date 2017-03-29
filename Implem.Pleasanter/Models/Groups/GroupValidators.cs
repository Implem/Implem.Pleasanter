using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class GroupValidators
    {
        public static Error.Types OnCreating(SiteSettings ss, GroupModel groupModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(groupModel.Mine());
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Groups_TenantId":
                        if (!ss.GetColumn("TenantId").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Groups_GroupName":
                        if (!ss.GetColumn("GroupName").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Groups_Body":
                        if (!ss.GetColumn("Body").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(SiteSettings ss, GroupModel groupModel)
        {
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(groupModel.Mine());
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Groups_TenantId":
                        if (!ss.GetColumn("TenantId").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Groups_GroupName":
                        if (!ss.GetColumn("GroupName").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Groups_Body":
                        if (!ss.GetColumn("Body").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, GroupModel groupModel)
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
