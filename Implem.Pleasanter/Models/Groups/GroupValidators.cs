using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class GroupValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnEntry(SiteSettings ss)
        {
            return
                Sessions.UserSettings().DisableGroupAdmin != true ||
                Permissions.CanManageTenant()
                    ? Error.Types.None
                    : Error.Types.HasNotPermission;
        }

        public static Error.Types OnReading(SiteSettings ss)
        {
            return ss.CanRead()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(SiteSettings ss, GroupModel groupModel)
        {
            switch (groupModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        ss.CanRead()&&
                        groupModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? Error.Types.None
                            : Error.Types.NotFound;        
                case BaseModel.MethodTypes.New:
                    return ss.CanCreate()
                        ? Error.Types.None
                        : Error.Types.HasNotPermission;
                default:
                    return Error.Types.NotFound;
            }
        }

        public static Error.Types OnCreating(SiteSettings ss, GroupModel groupModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(groupModel.Mine());
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "TenantId":
                        if (groupModel.TenantId_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "GroupName":
                        if (groupModel.GroupName_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (groupModel.Body_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate) return Error.Types.HasNotPermission;
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
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "TenantId":
                        if (groupModel.TenantId_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "GroupName":
                        if (groupModel.GroupName_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Body":
                        if (groupModel.Body_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate) return Error.Types.HasNotPermission;
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, GroupModel groupModel)
        {
            return ss.CanDelete()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnRestoring()
        {
            return Permissions.CanManageTenant()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnExporting(SiteSettings ss)
        {
            return ss.CanExport()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }
    }
}
