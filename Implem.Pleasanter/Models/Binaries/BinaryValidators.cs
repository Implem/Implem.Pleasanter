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
        public static Error.Types OnUploading(
            Column column,
            Libraries.DataTypes.Attachments attachments,
            System.Web.HttpPostedFileBase[] files)
        {
            if (!Contract.Attachments())
            {
                return Error.Types.BadRequest;
            }
            if (ValidFilesCount(attachments.Count(), files.Count(), column.LimitQuantity))
            {
                return Error.Types.OverLimitQuantity;
            }
            if (ValidFileSize(files, column.LimitSize))
            {
                return Error.Types.OverLimitSize;
            }
            if (ValidFilesTotalSize(
                attachments.Select(x => x.Size.ToInt()).Sum(),
                files.Select(x => x.ContentLength).Sum(), column.TotalLimitSize))
            {
                return Error.Types.OverTotalLimitSize;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool ValidFilesCount(long fileCount, long newFileCount, int? limitQuality)
        {
            if ((fileCount + newFileCount) > limitQuality) return true;
            return false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool ValidFileSize(System.Web.HttpPostedFileBase[] files, int? limitSize)
        {
            foreach (var item in files)
            {
                if (item.ContentLength > limitSize * 1024 * 1024) return true;
            }
            return false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool ValidFilesTotalSize(int totalFileSize, int newTotalFileSize, int? limitTotalSize)
        {
            if ((totalFileSize + newTotalFileSize) > limitTotalSize * 1024 * 1024) return true;
            return false;
        }
    }
}
