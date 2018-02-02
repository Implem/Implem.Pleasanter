using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class BinaryValidators
    {
        public static Error.Types OnGetting(SiteSettings ss)
        {
            if (!ss.CanRead())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(SiteSettings ss)
        {
            if (!ss.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUploadingSiteImage(SiteSettings ss, byte[] file)
        {
            if (!ss.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            if (file == null)
            {
                return Error.Types.SelectFile;
            }
            try
            {
                System.Drawing.Image.FromStream(new System.IO.MemoryStream(file));
            }
            catch (System.Exception)
            {
                return Error.Types.IncorrectFileFormat;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnDeletingSiteImage(SiteSettings ss)
        {
            if (!ss.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUploadingImage(System.Web.HttpPostedFileBase[] files)
        {
            if (!Contract.Attachments())
            {
                return Error.Types.BadRequest;
            }
            var newTotalFileSize = files.Sum(x => x.ContentLength);
            if (OverTenantStorageSize(
                BinaryUtilities.UsedTenantStorageSize(),
                newTotalFileSize,
                Contract.TenantStorageSize()))
            {
                return Error.Types.OverTenantStorageSize;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUploading(
            Column column,
            Libraries.DataTypes.Attachments attachments,
            System.Web.HttpPostedFileBase[] files)
        {
            if (!Contract.Attachments())
            {
                return Error.Types.BadRequest;
            }
            if (OverLimitQuantity(attachments.Count(), files.Count(), column.LimitQuantity))
            {
                return Error.Types.OverLimitQuantity;
            }
            if (OverLimitSize(files, column.LimitSize))
            {
                return Error.Types.OverLimitSize;
            }
            var newTotalFileSize = files.Sum(x => x.ContentLength);
            if (OverTotalLimitSize(
                attachments.Select(x => x.Size.ToLong()).Sum(),
                newTotalFileSize,
                column.TotalLimitSize))
            {
                return Error.Types.OverTotalLimitSize;
            }
            if (OverTenantStorageSize(
                BinaryUtilities.UsedTenantStorageSize(),
                newTotalFileSize,
                Contract.TenantStorageSize()))
            {
                return Error.Types.OverTenantStorageSize;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverLimitQuantity(long fileCount, long newFileCount, int? limit)
        {
            if ((fileCount + newFileCount) > limit) return true;
            return false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverLimitSize(System.Web.HttpPostedFileBase[] files, int? limit)
        {
            foreach (var item in files)
            {
                if (item.ContentLength > (long)limit * 1024 * 1024) return true;
            }
            return false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverTotalLimitSize(
            long totalFileSize, long newTotalFileSize, int? limit)
        {
            if ((totalFileSize + newTotalFileSize) > (long)limit * 1024 * 1024) return true;
            return false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverTenantStorageSize(
            long totalFileSize, long newTotalFileSize, int? limit)
        {
            if (limit != null &&
                (totalFileSize + newTotalFileSize) > (long)limit * 1024 * 1024 * 1024) return true;
            return false;
        }
    }
}
