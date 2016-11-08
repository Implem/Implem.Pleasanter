using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class SiteValidators
    {
        public static Error.Types OnCreating(
            SiteSettings ss, Permissions.Types pt, SiteModel siteModel)
        {
            if (!pt.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (!ss.GetColumn("Title").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Body":
                        if (!ss.GetColumn("Body").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!ss.GetColumn("ReferenceType").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!ss.GetColumn("InheritPermission").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Timestamp":
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
            SiteSettings ss, Permissions.Types pt, SiteModel siteModel)
        {
            if (!pt.CanEditSite())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (!ss.GetColumn("Title").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Body":
                        if (!ss.GetColumn("Body").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!ss.GetColumn("ReferenceType").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!ss.GetColumn("InheritPermission").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Timestamp":
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
            SiteSettings ss, Permissions.Types pt, SiteModel siteModel)
        {
            if (!pt.CanEditSite())
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnMoving(
            long currentId,
            long destinationId,
            Permissions.Types current,
            Permissions.Types source,
            Permissions.Types destination)
        {
            if (currentId != 0 && !current.CanEditSite())
            {
                return Error.Types.HasNotPermission;
            }
            if (!source.CanEditSite())
            {
                return Error.Types.HasNotPermission;
            }
            if (destinationId != 0 && !destination.CanEditSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
