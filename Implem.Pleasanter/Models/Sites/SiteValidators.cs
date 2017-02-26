using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class SiteValidators
    {
        public static Error.Types OnCreating(SiteSettings ss, SiteModel siteModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (!ss.GetColumn("Title").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Body":
                        if (!ss.GetColumn("Body").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!ss.GetColumn("ReferenceType").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!ss.GetColumn("InheritPermission").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(SiteSettings ss, SiteModel siteModel)
        {
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (!ss.GetColumn("Title").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Body":
                        if (!ss.GetColumn("Body").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!ss.GetColumn("ReferenceType").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!ss.GetColumn("InheritPermission").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, SiteModel siteModel)
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnMoving(
            long currentId,
            long destinationId,
            SiteSettings current,
            SiteSettings source,
            SiteSettings destination)
        {
            if (currentId != 0 && !current.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            if (!source.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            if (destinationId != 0 && !destination.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnSorting(SiteSettings ss)
        {
            if (ss.SiteId != 0 && !ss.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
