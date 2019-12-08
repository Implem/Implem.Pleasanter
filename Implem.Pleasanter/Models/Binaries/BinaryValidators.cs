using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class BinaryValidators
    {
        public static ErrorData OnGetting(Context context, SiteSettings ss)
        {
            if (!context.HasPermission(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnUpdating(Context context, SiteSettings ss)
        {
            if (!context.CanManageSite(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUploadingSiteImage(
            Context context, SiteSettings ss, byte[] bin)
        {
            if (!context.CanManageSite(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            if (bin == null)
            {
                return Error.Types.SelectFile;
            }
            try
            {
                System.Drawing.Image.FromStream(new System.IO.MemoryStream(bin));
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
        public static Error.Types OnUploadingTenantImage(
            Context context, SiteSettings ss, byte[] bin)
        {
            if (!Permissions.CanManageTenant(context)
                && context.UserSettings?.EnableManageTenant != true)
            {
                return Error.Types.HasNotPermission;
            }
            if (bin == null)
            {
                return Error.Types.SelectFile;
            }
            try
            {
                System.Drawing.Image.FromStream(new System.IO.MemoryStream(bin));
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
        public static Error.Types OnDeletingTenantImage(Context context, SiteSettings ss)
        {
            if (!Permissions.CanManageTenant(context)
                && context.UserSettings?.EnableManageTenant != true)
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUploadingImage(Context context)
        {
            if (!context.ContractSettings.Attachments())
            {
                return Error.Types.BadRequest;
            }
            var newTotalFileSize = context.PostedFiles.Sum(x => x.Size);
            if (OverTenantStorageSize(
                BinaryUtilities.UsedTenantStorageSize(context: context),
                newTotalFileSize,
                context.ContractSettings.StorageSize))
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
            Libraries.DataTypes.Attachments attachments)
        {
            if (!context.ContractSettings.Attachments())
            {
                return Error.Types.BadRequest;
            }
            if (OverLimitQuantity(
                attachments: attachments,
                limitQuantity: column.LimitQuantity))
            {
                return Error.Types.OverLimitQuantity;
            }
            if (OverLimitSize(
                attachments: attachments,
                limitSize: column.LimitSize))
            {
                return Error.Types.OverLimitSize;
            }
            if (OverTotalLimitSize(
                attachments: attachments,
                totalLimitSize: column.TotalLimitSize))
            {
                return Error.Types.OverTotalLimitSize;
            }
            if (OverTenantStorageSize(
                totalFileSize: BinaryUtilities.UsedTenantStorageSize(context: context),
                newTotalFileSize: attachments
                    .Select(o => o.Added == true
                        ? o.Size
                        : o.Deleted == true
                            ? o.Size * -1
                            : 0)
                    .Sum()
                    .ToDecimal(),
                storageSize: context.ContractSettings.StorageSize))
            {
                return Error.Types.OverTenantStorageSize;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUploading(
            Context context,
            SiteSettings ss,
            Dictionary<string, Libraries.DataTypes.Attachments> attachmentsHash)
        {
            if (!context.ContractSettings.Attachments())
            {
                return Error.Types.BadRequest;
            }
            foreach (var keyValue in attachmentsHash)
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: keyValue.Key);
                var attachments = keyValue.Value;
                if (OverLimitQuantity(
                    attachments: attachments,
                    limitQuantity: column.LimitQuantity))
                {
                    return Error.Types.OverLimitQuantity;
                }
                if (OverLimitSize(
                    attachments: attachments,
                    limitSize: column.LimitSize))
                {
                    return Error.Types.OverLimitSize;
                }
                if (OverTotalLimitSize(
                    attachments: attachments,
                    totalLimitSize: column.TotalLimitSize))
                {
                    return Error.Types.OverTotalLimitSize;
                }
                if (OverTenantStorageSize(
                    totalFileSize: BinaryUtilities.UsedTenantStorageSize(context: context),
                    newTotalFileSize: attachmentsHash
                        .Select(o =>
                            o.Value.Where(p => p.Added == true).Select(p => p.Size).Sum()
                                - o.Value.Where(p => p.Deleted == true).Select(p => p.Size).Sum())
                        .Sum()
                        .ToDecimal(),
                    storageSize: context.ContractSettings.StorageSize))
                {
                    return Error.Types.OverTenantStorageSize;
                }
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverLimitQuantity(
            Libraries.DataTypes.Attachments attachments, decimal? limitQuantity)
        {
            return attachments
                .Where(o => o.Deleted != true)
                .Count()
                    > limitQuantity;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverLimitSize(
            Libraries.DataTypes.Attachments attachments, decimal? limitSize)
        {
            return attachments.Any(o =>
                o.Added == true
                && o.Deleted != true
                && o.Size
                    > limitSize * 1024 * 1024);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverTotalLimitSize(
            Libraries.DataTypes.Attachments attachments, decimal? totalLimitSize)
        {
            return attachments
                .Where(o => o.Deleted != true)
                .Select(o => o.Size)
                .Sum()
                    > totalLimitSize * 1024 * 1024;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverTenantStorageSize(
            decimal totalFileSize, decimal newTotalFileSize, decimal? storageSize)
        {
            return storageSize != null && (totalFileSize + newTotalFileSize)
                > storageSize * 1024 * 1024 * 1024;
        }
    }
}
