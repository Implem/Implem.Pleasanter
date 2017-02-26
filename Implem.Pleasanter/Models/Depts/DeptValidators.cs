using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class DeptValidators
    {
        public static Error.Types OnCreating(SiteSettings ss, DeptModel deptModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Depts_DeptCode":
                        if (!ss.GetColumn("DeptCode").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_DeptName":
                        if (!ss.GetColumn("DeptName").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_Body":
                        if (!ss.GetColumn("Body").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(SiteSettings ss, DeptModel deptModel)
        {
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Depts_DeptCode":
                        if (!ss.GetColumn("DeptCode").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_DeptName":
                        if (!ss.GetColumn("DeptName").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_Body":
                        if (!ss.GetColumn("Body").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, DeptModel deptModel)
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
