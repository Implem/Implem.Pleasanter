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
        public static Error.Types OnGetting(Context context, SiteSettings ss)
        {
            if (!context.HasPermission(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(Context context, SiteSettings ss)
        {
            if (!context.CanManageSite(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUploadingSiteImage(
            Context context, SiteSettings ss, byte[] file)
        {
            if (!context.CanManageSite(ss: ss))
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
        public static Error.Types OnDeletingSiteImage(Context context, SiteSettings ss)
        {
            if (!context.CanManageSite(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUploadingImage(
            Context context, System.Web.HttpPostedFileBase[] files)
        {
            if (!Contract.Attachments(context: context))
            {
                return Error.Types.BadRequest;
            }
            var newTotalFileSize = files.Sum(x => x.ContentLength.ToDecimal());
            if (OverTenantStorageSize(
                BinaryUtilities.UsedTenantStorageSize(context: context),
                newTotalFileSize,
                Contract.TenantStorageSize(context: context)))
            {
                return Error.Types.OverTenantStorageSize;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnDeletingImage(
            Context context, SiteSettings ss, BinaryModel binaryModel)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            if (binaryModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Error.Types.NotFound;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUploading(
            Context context,
            Column column,
            Libraries.DataTypes.Attachments attachments,
            System.Web.HttpPostedFileBase[] files)
        {
            if (!Contract.Attachments(context: context))
            {
                return Error.Types.BadRequest;
            }
            if (OverLimitQuantity(
                attachments.Count().ToDecimal(),
                files.Count().ToDecimal(),
                column.LimitQuantity))
            {
                return Error.Types.OverLimitQuantity;
            }
            if (OverLimitSize(files, column.LimitSize))
            {
                return Error.Types.OverLimitSize;
            }
            var newTotalFileSize = files.Sum(x => x.ContentLength.ToDecimal());
            if (OverTotalLimitSize(
                attachments.Select(x => x.Size.ToDecimal()).Sum(),
                newTotalFileSize,
                column.TotalLimitSize))
            {
                return Error.Types.OverTotalLimitSize;
            }
            if (OverTenantStorageSize(
                BinaryUtilities.UsedTenantStorageSize(context: context),
                newTotalFileSize,
                Contract.TenantStorageSize(context: context)))
            {
                return Error.Types.OverTenantStorageSize;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverLimitQuantity(
            decimal fileCount, decimal newFileCount, decimal? limit)
        {
            if ((fileCount + newFileCount) > limit) return true;
            return false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverLimitSize(System.Web.HttpPostedFileBase[] files, decimal? limit)
        {
            foreach (var item in files)
            {
                if (item.ContentLength > limit * 1024 * 1024) return true;
            }
            return false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverTotalLimitSize(
            decimal totalFileSize, decimal newTotalFileSize, decimal? limit)
        {
            if ((totalFileSize + newTotalFileSize) > limit * 1024 * 1024) return true;
            return false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverTenantStorageSize(
            decimal totalFileSize, decimal newTotalFileSize, decimal? limit)
        {
            if (limit != null &&
                (totalFileSize + newTotalFileSize) > limit * 1024 * 1024 * 1024) return true;
            return false;
        }
    }
}
