using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class DeptValidators
    {
        public static Error.Types OnCreating(
            SiteSettings ss, Permissions.Types pt, DeptModel deptModel)
        {
            if (!pt.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Depts_DeptCode":
                        if (!ss.GetColumn("DeptCode").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_DeptName":
                        if (!ss.GetColumn("DeptName").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_Body":
                        if (!ss.GetColumn("Body").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            SiteSettings ss, Permissions.Types pt, DeptModel deptModel)
        {
            if (!pt.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Depts_DeptCode":
                        if (!ss.GetColumn("DeptCode").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_DeptName":
                        if (!ss.GetColumn("DeptName").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_Body":
                        if (!ss.GetColumn("Body").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Depts_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            SiteSettings ss, Permissions.Types pt, DeptModel deptModel)
        {
            if (!pt.CanDelete())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnRestoring()
        {
            if (!Permissions.Admins().CanManageTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
