using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class DeptValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnEntry(Context context, SiteSettings ss)
        {
            return Permissions.CanManageTenant(context: context)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnReading(Context context, SiteSettings ss)
        {
            return context.CanRead(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(
            Context context, SiteSettings ss, DeptModel deptModel)
        {
            switch (deptModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)&&
                        deptModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? Error.Types.None
                            : Error.Types.NotFound;        
                case BaseModel.MethodTypes.New:
                    return context.CanCreate(ss: ss)
                        ? Error.Types.None
                        : Error.Types.HasNotPermission;
                default:
                    return Error.Types.NotFound;
            }
        }

        public static Error.Types OnCreating(
            Context context, SiteSettings ss, DeptModel deptModel)
        {
            if (!context.CanCreate(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: deptModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "DeptCode":
                        if (deptModel.DeptCode_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DeptName":
                        if (deptModel.DeptName_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (deptModel.Body_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            Context context, SiteSettings ss, DeptModel deptModel)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: deptModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "DeptCode":
                        if (deptModel.DeptCode_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DeptName":
                        if (deptModel.DeptName_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (deptModel.Body_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            Context context, SiteSettings ss, DeptModel deptModel)
        {
            return context.CanDelete(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnRestoring(Context context)
        {
            return Permissions.CanManageTenant(context: context)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnExporting(Context context, SiteSettings ss)
        {
            return context.CanExport(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }
    }
}
