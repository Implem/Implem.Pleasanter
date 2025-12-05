using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
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
                SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(new System.IO.MemoryStream(bin));
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
                SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(new System.IO.MemoryStream(bin));
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
        public static Error.Types OnUploadingImage(
            Context context,
            SiteSettings ss = null,
            string columnName = null,
            PostedFile file = null)
        {
            if (!context.ContractSettings.Attachments())
            {
                return Error.Types.BadRequest;
            }
            // セキュリティ対策として画像以外のContentTypeを許可しない
            if (!file.ContentType.StartsWith("image/"))
            {
                return Error.Types.InvalidRequest;
            }
            var newTotalFileSize = context.PostedFiles.Sum(x => x.Size);
            if (OverTenantStorageSize(
                BinaryUtilities.UsedTenantStorageSize(context: context),
                newTotalFileSize,
                context.ContractSettings.StorageSize))
            {
                return Error.Types.OverTenantStorageSize;
            }
            if (ss?.GetColumn(
                context: context,
                columnName: columnName)?.AllowImage == false)
            {
                return Error.Types.BadRequest;
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
            Attachments attachments)
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
            var errorType = OverLimitSize(
                    attachments: attachments,
                    column: column);
            if (errorType != Error.Types.None)
            {
                return errorType;
            }
            switch (BinaryUtilities.BinaryStorageProvider(column))
            {
                case "LocalFolder":
                    if (OverTotalLimitSize(
                        attachments: attachments,
                        totalLimitSize: column.LocalFolderTotalLimitSize))
                    {
                        return Error.Types.OverLocalFolderTotalLimitSize;
                    }
                    break;
                case "AutoDataBaseOrLocalFolder":
                    if (OverTotalLimitSize(
                        attachments: attachments,
                        totalLimitSize: column.TotalLimitSize))
                    {
                        return Error.Types.OverTotalLimitSize;
                    }
                    break;
                default:
                    if (OverTotalLimitSize(
                        attachments: attachments,
                        totalLimitSize: column.TotalLimitSize))
                    {
                        return Error.Types.OverTotalLimitSize;
                    }
                    break;
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
            Dictionary<string, Attachments> attachmentsHash)
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
                var errorType = OverLimitSize(
                    attachments: attachments,
                    column: column);
                if (errorType != Error.Types.None)
                {
                    return errorType;
                }
                switch (BinaryUtilities.BinaryStorageProvider(column))
                {
                    case "LocalFolder":
                        if (OverTotalLimitSize(
                            attachments: attachments,
                            totalLimitSize: column.LocalFolderTotalLimitSize))
                        {
                            return Error.Types.OverLocalFolderTotalLimitSize;
                        }
                        break;
                    case "AutoDataBaseOrLocalFolder":
                        if(OverTotalLimitSize(
                            attachments: attachments,
                            totalLimitSize: column.TotalLimitSize))
                        {
                            return Error.Types.OverTotalLimitSize;
                        }
                        break;
                    default:
                        if (OverTotalLimitSize(
                            attachments: attachments,
                            totalLimitSize: column.TotalLimitSize))
                        {
                            return Error.Types.OverTotalLimitSize;
                        }
                        break;
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
        public static Error.Types OnUploading(
            Context context,
            Attachments attachments)
        {
            if (!context.ContractSettings.Attachments())
            {
                return Error.Types.BadRequest;
            }
            if (OverLimitQuantity(
                attachments: attachments,
                limitQuantity: Parameters.BinaryStorage.LimitQuantity))
            {
                return Error.Types.OverLimitQuantity;
            }
            switch (BinaryUtilities.BinaryStorageProvider())
            {
                case "LocalFolder":
                    if (OverTotalLimitSize(
                        attachments: attachments,
                        totalLimitSize: Parameters.BinaryStorage.MaxSize))
                    {
                        return Error.Types.OverLocalFolderTotalLimitSize;
                    }
                    break;
                case "AutoDataBaseOrLocalFolder":
                    if (OverTotalLimitSize(
                        attachments: attachments,
                        totalLimitSize: Parameters.BinaryStorage.MaxSize))
                    {
                        return Error.Types.OverTotalLimitSize;
                    }
                    break;
                default:
                    if (OverTotalLimitSize(
                        attachments: attachments,
                        totalLimitSize: Parameters.BinaryStorage.MaxSize))
                    {
                        return Error.Types.OverTotalLimitSize;
                    }
                    break;
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
            Column column,
            Attachments attachments,
            IList<PostedFile> files,
            IEnumerable<System.Net.Http.Headers.ContentRangeHeaderValue> contentRanges)
        {
            if (!context.ContractSettings.Attachments())
            {
                return Error.Types.BadRequest;
            }
            if (files.Count != contentRanges.Count())
            {
                return Error.Types.BadRequest;
            }
            if (OverLimitQuantity(
                attachments: attachments,
                limitQuantity: column.LimitQuantity,
                newFileCount: files.Count()))
            {
                return Error.Types.OverLimitQuantity;
            }
            var totalLength = default(long);
            foreach (var length in contentRanges.Select(r => r.Length ?? default(long)))
            {
                if (BinaryUtilities.BinaryStorageProvider(column, length) == "LocalFolder")
                {
                    if (OverLimitSize(
                        length: length,
                        limitSize: column.LocalFolderLimitSize))
                    {
                        return Error.Types.OverLocalFolderLimitSize;
                    }
                }
                else
                {
                    if (OverLimitSize(
                        length: length,
                        limitSize: column.LimitSize))
                    {
                        return Error.Types.OverLimitSize;
                    }
                }
                totalLength += length;
            }
            switch (BinaryUtilities.BinaryStorageProvider(column))
            {
                case "LocalFolder":
                    if (OverTotalLimitSize(
                        attachments: attachments,
                        totalLimitSize: column.LocalFolderTotalLimitSize,
                        newFileTotalSize: totalLength))
                    {
                        return Error.Types.OverLocalFolderTotalLimitSize;
                    }
                    break;
                case "AutoDataBaseOrLocalFolder":
                    if (OverTotalLimitSize(
                        attachments: attachments,
                        totalLimitSize: column.TotalLimitSize,
                        newFileTotalSize: totalLength))
                    {
                        return Error.Types.OverTotalLimitSize;
                    }
                    break;
                default:
                    if (OverTotalLimitSize(
                        attachments: attachments,
                        totalLimitSize: column.TotalLimitSize,
                        newFileTotalSize: totalLength))
                    {
                        return Error.Types.OverTotalLimitSize;
                    }
                    break;
            }
            if (OverTenantStorageSize(
                BinaryUtilities.UsedTenantStorageSize(context),
                totalLength,
                context.ContractSettings.StorageSize))
            {
                return Error.Types.OverTenantStorageSize;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverLimitQuantity(
            Attachments attachments, decimal? limitQuantity)
        {
            return attachments
                .Where(o => o.Deleted != true)
                .Count()
                    > limitQuantity;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverLimitQuantity(
            Attachments attachments, decimal? limitQuantity, decimal newFileCount = 0)
        {
            return attachments
                .Where(o => o.Deleted != true)
                .Count() + newFileCount
                    > limitQuantity;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Error.Types OverLimitSize(
            Attachments attachments, Column column)
        {
            foreach (var attachment in attachments
                .Where(o => o.Added == true && o.Deleted != true))
            {
                if (attachment.IsStoreLocalFolder(column))
                {
                    if (attachment.Size > column.LocalFolderLimitSize * 1024 * 1024)
                    {
                        return Error.Types.OverLocalFolderLimitSize;
                    }
                }
                else
                {
                    if (attachment.Size > column.LimitSize * 1024 * 1024)
                    {
                        return Error.Types.OverLimitSize;
                    }
                }
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverLimitSize(
            long length, decimal? limitSize)
        {
            return length > limitSize * 1024 * 1024;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverTotalLimitSize(
            Attachments attachments,
            decimal? totalLimitSize)
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
        private static bool OverTotalLimitSize(
            Attachments attachments,
            System.Func<Attachment,bool> storageSelector,
            decimal? totalLimitSize)
        {
            return attachments
                .Where(o => o.Deleted != true)
                .Where(storageSelector)
                .Select(o => o.Size)
                .Sum()
                    > totalLimitSize * 1024 * 1024;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool OverTotalLimitSize(
            Attachments attachments,
            decimal? totalLimitSize,
            decimal newFileTotalSize = 0)
        {
            return attachments
                .Where(o => o.Deleted != true)
                .Select(o => o.Size)
                .Sum() + newFileTotalSize
                    > totalLimitSize * 1024 * 1024;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Error.Types OverTotalLimitSize(
            Attachments attachments, Column column, long rdsLength, long localLength)
        {
            long limitSize = (long)(column.LimitSize ?? 0);
            if (attachments.Where(o => o.Deleted != true)
                .Select(o => o.Size)
                .Sum(i => i <= (limitSize * 1024 * 1024) ? i : 0) + rdsLength > column.LimitSize * 1024 * 1024)
            {
                return Error.Types.OverTotalLimitSize;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Error.Types OverTotalLimitSize(
            Attachments attachments, Column column)
        {
            long limitSize = (long)(column.LimitSize ?? 0);
            if (attachments.Where(o => o.Deleted != true)
                .Select(o => o.Size)
                .Sum(i => i <= (limitSize * 1024 * 1024) ? i : 0) > column.TotalLimitSize * 1024 * 1024)
            {
                return Error.Types.OverTotalLimitSize;
            }
            return Error.Types.None;
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

        /// <summary>
        /// Fixed:
        /// ファイル名を検証します（パストラバーサル対策）
        /// </summary>
        public static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            if (fileName.Contains('\0') ||
                fileName.Contains("..") ||
                fileName.Contains("/") ||
                fileName.Contains("\\") ||
                fileName.Contains(":") ||
                fileName.Length > 255)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Fixed:
        /// ファイル拡張子を検証します（危険な拡張子の拒否）
        /// </summary>
        public static bool IsAllowedExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            var extension = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                return true;
            }
            if (Parameters.Form.AttachmentExcludedExtensions.Contains(extension))
            {
                return false;
            }
            // 二重拡張子のチェック(filename.exe.txt のようなもののチェック)
            var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
            if (!string.IsNullOrEmpty(fileNameWithoutExtension))
            {
                var secondExtension = System.IO.Path.GetExtension(fileNameWithoutExtension);
                if (!string.IsNullOrEmpty(secondExtension) &&
                    Parameters.Form.AttachmentExcludedExtensions.Contains(secondExtension))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Fixed:
        /// Form機能用のアップロードパラメータを検証します
        /// </summary>
        public static Error.Types OnValidatingFormUpload(
            Context context,
            string[] uuids,
            string[] fileUuid,
            string[] fileNames)
        {
            // Form機能以外は検証スキップ
            if (!context.IsForm)
            {
                return Error.Types.None;
            }
            // UUID配列の検証
            if (fileUuid != null)
            {
                foreach (var uuid in fileUuid)
                {
                    if (!Validators.IsValidGuid(uuid))
                    {
                        return Error.Types.InvalidRequest;
                    }
                }
            }
            // Uuids配列の検証
            if (uuids != null)
            {
                foreach (var uuid in uuids)
                {
                    if (!Validators.IsValidGuid(uuid))
                    {
                        return Error.Types.InvalidRequest;
                    }
                }
            }
            // ファイル名の検証
            if (fileNames != null)
            {
                foreach (var fileName in fileNames)
                {
                    if (!IsValidFileName(fileName) || !IsAllowedExtension(fileName))
                    {
                        return Error.Types.InvalidRequest;
                    }
                }
            }
            // POSTされたファイルの検証
            if (context.PostedFiles != null)
            {
                foreach (var file in context.PostedFiles)
                {
                    if (!IsValidFileName(file.FileName) || !IsAllowedExtension(file.FileName))
                    {
                        return Error.Types.InvalidRequest;
                    }
                }
            }
            return Error.Types.None;
        }
    }
}
