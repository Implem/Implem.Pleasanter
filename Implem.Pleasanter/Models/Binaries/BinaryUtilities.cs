using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class BinaryUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool ExistsSiteImage(
            Context context,
            SiteSettings ss,
            long referenceId,
            Libraries.Images.ImageData.SizeTypes sizeType)
        {
            var invalid = BinaryValidators.OnGetting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return false;
            }
            switch (Parameters.BinaryStorage.GetSiteImageProvider())
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        referenceId,
                        Libraries.Images.ImageData.Types.SiteImage)
                            .Exists(sizeType)
                                || ExistsSiteImage(
                                    context: context,
                                    referenceId: referenceId);
                default:
                    return ExistsSiteImage(
                        context: context,
                        referenceId: referenceId);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool ExistsSiteImage(Context context, long referenceId)
        {
            return Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectBinaries(
                    column: Rds.BinariesColumn().BinariesCount(),
                    where: Rds.BinariesWhere()
                        .ReferenceId(referenceId)
                        .BinaryType("SiteImage"))) == 1;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool ExistsTenantImage(
            Context context,
            SiteSettings ss,
            long referenceId,
            Libraries.Images.ImageData.SizeTypes sizeType)
        {
            var invalid = BinaryValidators.OnGetting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return false;
            }
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        referenceId, Libraries.Images.ImageData.Types.TenantImage)
                            .Exists(sizeType);
                default:
                    return Repository.ExecuteScalar_int(
                        context: context,
                        statements: Rds.SelectBinaries(
                            column: Rds.BinariesColumn().BinariesCount(),
                            where: Rds.BinariesWhere()
                                .ReferenceId(referenceId)
                                .BinaryType("TenantImage"))) == 1;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteImagePrefix(
            Context context,
            SiteSettings ss,
            long referenceId,
            Libraries.Images.ImageData.SizeTypes sizeType)
        {
            var invalid = BinaryValidators.OnGetting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return string.Empty;
            }
            return new BinaryModel(referenceId).SiteImagePrefix(
                context: context,
                sizeType: sizeType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string TenantImagePrefix(
            Context context,
            SiteSettings ss,
            long referenceId,
            Libraries.Images.ImageData.SizeTypes sizeType)
        {
            var invalid = BinaryValidators.OnGetting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return string.Empty;
            }
            return new BinaryModel(referenceId).TenantImagePrefix(
                context: context,
                sizeType: sizeType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static (byte[] bytes, string contentType) SiteImageThumbnail(Context context, SiteModel siteModel)
        {
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: siteModel.SiteId);
            var invalid = BinaryValidators.OnGetting(
                context: context,
                ss: siteModel.SiteSettings);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return (null, null);
            }
            var binaryModel = new BinaryModel(
                context: context,
                referenceId: siteModel.SiteId,
                binaryType: "SiteImage");
            return (
                binaryModel.SiteImage(
                    context: context,
                    sizeType: Libraries.Images.ImageData.SizeTypes.Thumbnail,
                    column: Rds.BinariesColumn().Thumbnail()),
                binaryModel.ContentType.IsNullOrEmpty()
                    ? "image/bmp"
                    : binaryModel.ContentType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static (byte[] bytes, string contentType) SiteImageIcon(Context context, SiteModel siteModel)
        {
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: siteModel.SiteId);
            var invalid = BinaryValidators.OnGetting(
                context: context,
                ss: siteModel.SiteSettings);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return (null, null);
            }
            var binaryModel = new BinaryModel(
                context: context,
                referenceId: siteModel.SiteId,
                binaryType: "SiteImage");
            return (
                binaryModel.SiteImage(
                    context: context,
                    sizeType: Libraries.Images.ImageData.SizeTypes.Icon,
                    column: Rds.BinariesColumn().Icon()),
                binaryModel.ContentType.IsNullOrEmpty()
                    ? "image/bmp"
                    : binaryModel.ContentType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static (byte[] bytes, string contentType) TenantImageLogo(Context context, TenantModel tenantModel)
        {
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context);
            var invalid = BinaryValidators.OnGetting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return (null, null);
            }
            var binaryModel = new BinaryModel(
                context: context,
                referenceId: tenantModel.TenantId,
                binaryType: "TenantImage");
            return (
                binaryModel.TenantImage(
                    context: context,
                    sizeType: Libraries.Images.ImageData.SizeTypes.Logo,
                    column: Rds.BinariesColumn().Bin()),
                binaryModel.ContentType.IsNullOrEmpty()
                    ? "image/bmp"
                    : binaryModel.ContentType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UpdateSiteImage(Context context, SiteModel siteModel)
        {
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: siteModel.SiteId);
            var bin = context.PostedFiles.FirstOrDefault()?.Byte();
            var invalid = BinaryValidators.OnUploadingSiteImage(
                context: context,
                ss: siteModel.SiteSettings,
                bin: bin);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = new BinaryModel(siteModel.SiteId).UpdateSiteImage(
                context: context,
                ss: siteModel.SiteSettings,
                bin: bin);
            if (error.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection(context: context)
                    .Html(
                        "#TenantImageLogoContainer",
                        new HtmlBuilder().SiteImageIcon(
                            context: context,
                            ss: siteModel.SiteSettings,
                            siteId: siteModel.TenantId))
                    .Html(
                        "#TenantImageSettingsEditor",
                        new HtmlBuilder().SiteImageSettingsEditor(
                            context: context,
                            ss: siteModel.SiteSettings))
                    .Message(Messages.FileUpdateCompleted(context: context))
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UpdateTenantImage(Context context, TenantModel tenantModel)
        {
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context);
            var bin = context.PostedFiles.FirstOrDefault()?.Byte();
            var invalid = BinaryValidators.OnUploadingTenantImage(
                context: context,
                ss: ss,
                bin: bin);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = new BinaryModel(tenantModel.TenantId).UpdateTenantImage(
                context: context,
                ss: ss,
                bin: bin);
            if (error.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection(context: context)
                   .ReplaceAll(
                       "#Logo",
                       new HtmlBuilder().HeaderLogo(
                           context: context,
                           ss: ss))
                   .ReplaceAll(
                       "#TenantImageSettingsEditor",
                       new HtmlBuilder().TenantImageSettingsEditor(
                           context: context, tenantModel: tenantModel))
                   .Message(Messages.FileUpdateCompleted(context: context))
                   .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteSiteImage(Context context, SiteModel siteModel)
        {
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: siteModel.SiteId);
            var invalid = BinaryValidators.OnDeletingSiteImage(
                context: context,
                ss: siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = new BinaryModel(siteModel.SiteId)
                .DeleteSiteImage(context: context);
            if (error.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection(context: context)
                    .Html(
                        "#SiteImageIconContainer",
                        new HtmlBuilder().SiteImageIcon(
                            context: context,
                            ss: siteModel.SiteSettings,
                            siteId: siteModel.SiteId))
                    .Html(
                        "#SiteImageSettingsEditor",
                        new HtmlBuilder().SiteImageSettingsEditor(
                            context: context,
                            ss: siteModel.SiteSettings))
                    .Message(Messages.FileDeleteCompleted(context: context))
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteTenantImage(Context context, TenantModel tenantModel)
        {
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context);
            var invalid = BinaryValidators.OnDeletingTenantImage(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = new BinaryModel(tenantModel.TenantId)
                .DeleteTenantImage(context: context);
            if (error.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection(context: context)
                   .ReplaceAll(
                       "#Logo",
                       new HtmlBuilder().HeaderLogo(
                           context: context,
                           ss: ss))
                   .ReplaceAll(
                       "#TenantImageSettingsEditor",
                       new HtmlBuilder().TenantImageSettingsEditor(
                           context: context, tenantModel: tenantModel))
                   .Message(Messages.FileDeleteCompleted(context: context))
                   .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UploadImage(
            Context context,
            long id)
        {
            var columnName = context.Forms.Data("ControlId");
            if (columnName.Contains("_"))
            {
                columnName = columnName.Substring(columnName.IndexOf("_") + 1);
            }
            if (columnName.StartsWith("Comment"))
            {
                columnName = "Comments";
            }
            var file = context.PostedFiles.FirstOrDefault();
            var invalid = BinaryValidators.OnUploadingImage(
                context: context,
                file: file);
            switch (invalid)
            {
                case Error.Types.OverTenantStorageSize:
                    return Messages.ResponseOverTenantStorageSize(
                        context: context,
                        data: context.ContractSettings.StorageSize.ToString()).ToJson();
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            UploadImage(
                context: context,
                id: id,
                columnName: columnName,
                file: file);
            return new ResponseCollection(context: context)
                .InsertText(
                    "#" + context.Forms.ControlId(),
                    "![image]({0})".Params(Locations.ShowFile(
                        context: context,
                        guid: file.Guid)))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types UploadImage(
            Context context,
            SiteSettings ss,
            long id,
            Dictionary<string, PostedFile> postedFileHash)
        {
            var invalid = Error.Types.None;
            foreach (var file in postedFileHash)
            {
                invalid = BinaryValidators.OnUploadingImage(
                    context: context,
                    ss: ss,
                    columnName: file.Key,
                    file: file.Value);
                switch (invalid)
                {
                    case Error.Types.None:
                        invalid = UploadImage(
                            context: context,
                            id: id,
                            columnName: file.Key,
                            file: file.Value);
                        break;
                    default:
                        // エラーの場合はスキップする
                        break;
                }
            }
            return invalid;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types UploadImage(
            Context context,
            long id,
            string columnName,
            PostedFile file)
        {
            var bin = file.Byte();
            var ss = new ItemModel(
                context: context,
                referenceId: id)
                    .GetSite(
                        context: context,
                        initSiteSettings: true)
                            .SiteSettings;
            var thumbnailLimitSize = ss.GetColumn(
                context: context,
                columnName: columnName)?.ThumbnailLimitSize
                    ?? Parameters.BinaryStorage.ThumbnailLimitSize;
            var imageData = new Libraries.Images.ImageData(
                bin,
                ss.ReferenceId,
                Libraries.Images.ImageData.Types.SiteImage);
            if (Parameters.BinaryStorage.ImageLimitSize?.ToInt() > 0)
            {
                bin = imageData.ReSizeBytes(Parameters.BinaryStorage.ImageLimitSize);
            }
            var thumbnail = thumbnailLimitSize > 0
                ? imageData.ReSizeBytes(thumbnailLimitSize)
                : null;
            if (Parameters.BinaryStorage.IsLocal())
            {
                bin.Write(System.IO.Path.Combine(
                    Directories.BinaryStorage(),
                    "Images",
                    file.Guid));
                if (thumbnailLimitSize > 0)
                {
                    thumbnail.Write(System.IO.Path.Combine(
                        Directories.BinaryStorage(),
                        "Images",
                        file.Guid + "_thumbnail"));
                }
            }
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.InsertBinaries(
                    param: Rds.BinariesParam()
                        .TenantId(context.TenantId)
                        .ReferenceId(id)
                        .Guid(file.Guid)
                        .BinaryType(FileContentResults.RefererIsTenantManagement(context: context)
                            ? "TenantManagementImages"
                            : "Images")
                        .Title(file.FileName)
                        .Bin(bin, _using: !Parameters.BinaryStorage.IsLocal())
                        .Thumbnail(thumbnail, _using: thumbnail != null)
                        .FileName(file.FileName)
                        .Extension(file.Extension)
                        .Size(file.Size)
                        .ContentType(file.ContentType)));
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteImage(Context context, string guid)
        {
            var binaryModel = new BinaryModel()
                .Get(
                    context: context,
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .Guid(guid.ToUpper()));
            var ss = new ItemModel(
                context: context,
                referenceId: binaryModel.ReferenceId)
                    .GetSite(
                        context: context,
                        initSiteSettings: true)
                            .SiteSettings;
            var invalid = BinaryValidators.OnDeletingImage(
                context: context,
                ss: ss,
                binaryModel: binaryModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            binaryModel.Delete(context: context);
            var path = System.IO.Path.Combine(
                Directories.BinaryStorage(),
                "Images",
                binaryModel.Guid);
            if (System.IO.File.Exists(path))
            {
                Files.DeleteFile(path);
            }
            return new ResponseCollection(context: context)
                .Message(Messages.DeletedImage(context: context))
                .Remove($"#ImageLib .item[data-id=\"{guid}\"]")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseFile Donwload(Context context, string guid)
        {
            if (!context.ContractSettings.Attachments())
            {
                return null;
            }
            return FileContentResults.Download(context: context, guid: guid.ToUpper());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance ApiDonwload(Context context, string guid)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            if (!context.ContractSettings.Attachments())
            {
                return null;
            }
            return FileContentResults.DownloadByApi(context: context, guid: guid.ToUpper());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseFile DownloadTemp(Context context, string guid)
        {
            if (!context.ContractSettings.Attachments())
            {
                return null;
            }
            if (!BinaryUtilities.ValidateDownloadTemp(context: context, guid: guid))
            {
                return null;
            }
            return FileContentResults.DownloadTemp(context: context, guid: guid.ToUpper());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteTemp(Context context)
        {
            if (!context.ContractSettings.Attachments())
            {
                return null;
            }
            var guid = context.Forms.Data("Guid");
            RemoveTempFileSession(context: context, guid: guid);
            Libraries.DataSources.File.DeleteTemp(context: context, guid: guid);
            return "[]";
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string GetTempFileSessionKey(string guid)
        {
            return $"TempFile_{guid.ToUpper()}";
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool ValidateDownloadTemp(Context context, string guid)
        {
            // 同一セッション内でしか未確定中の添付ファイルは参照させない
            return SessionUtilities.Get(context: context)
                .Any(kv => kv.Key == GetTempFileSessionKey(guid));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SaveTempFileSession(Context context, string guid)
        {
            SessionUtilities.Set(
                context: context,
                key: GetTempFileSessionKey(guid),
                value: string.Empty);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void RemoveTempFileSession(Context context, string guid)
        {
            SessionUtilities.Remove(
                context: context,
                key: GetTempFileSessionKey(guid),
                page: false);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static decimal UsedTenantStorageSize(Context context)
        {
            return Repository.ExecuteScalar_decimal(
                context: context,
                statements: Rds.SelectBinaries(
                    column: Rds.BinariesColumn().Size(function: Sqls.Functions.Sum),
                    where: Rds.BinariesWhere().TenantId(context.TenantId)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlStatement UpdateReferenceId(
            Context context, SiteSettings ss, long referenceId, string values)
        {
            var guids = values?.RegexValues("[0-9a-z]{32}").ToList().ConvertAll(x => x.ToUpper());
            return guids?.Any() == true
                ? Rds.UpdateBinaries(
                    param: Rds.BinariesParam().ReferenceId(referenceId),
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId(ss.SiteId)
                        .Guid(guids, multiParamOperator: " or ")
                        .Creator(context.UserId))
                : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BinaryStorageProvider(Column column = null)
        {
            if (Parameters.BinaryStorage.UseStorageSelect)
            {
                return string.IsNullOrEmpty(column?.BinaryStorageProvider)
                    ? Parameters.BinaryStorage.DefaultBinaryStorageProvider
                    : column?.BinaryStorageProvider;
            }
            else
            {
                return Parameters.BinaryStorage.IsLocal()
                    ? "LocalFolder"
                    : "DataBase";
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BinaryStorageProvider(Column column, long size)
        {
            decimal s = size;
            return BinaryStorageProvider(column, s);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BinaryStorageProvider(Column column, decimal size)
        {
            var binaryStorageProvider = BinaryStorageProvider(column);
            switch (binaryStorageProvider)
            {
                case "AutoDataBaseOrLocalFolder":
                    return size > column?.LimitSize * 1024M * 1024M
                        ? "LocalFolder"
                        : "DataBase";
                default:
                    return binaryStorageProvider;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UploadFile(
            Context context,
            long id,
            System.Net.Http.Headers.ContentRangeHeaderValue contentRange)
        {
            var itemModel = new ItemModel(context, id);
            itemModel.SetSite(
                context: context,
                initSiteSettings: true);
            var ss = itemModel.Site.SiteSettings;
            var column = ss.GetColumn(context, TrimIdSuffix(context.Forms.Get("ColumnName")));
            var attachments = context.Forms.Get("AttachmentsData").Deserialize<Attachments>();
            var fileHash = context.Forms.Get("FileHash");
            contentRange = contentRange ?? context?.PostedFiles?.FirstOrDefault()?.ContentRange;
            {
                var invalid = HasPermission(context, ss, itemModel);
                switch (invalid.Type)
                {
                    case Error.Types.None: break;
                    default: return invalid.MessageJson(context);
                }
            }
            {
                var invalid = BinaryValidators.OnUploading(context, column, attachments, context.PostedFiles, new[] { contentRange });
                switch (invalid)
                {
                    case Error.Types.OverLimitQuantity:
                        return Messages.ResponseOverLimitQuantity(
                            context: context,
                            data: column.LimitQuantity.ToString()).ToJson();
                    case Error.Types.OverLimitSize:
                        return Messages.ResponseOverLimitSize(
                            context: context,
                            data: column.LimitSize.ToString()).ToJson();
                    case Error.Types.OverTotalLimitSize:
                        return Messages.ResponseOverTotalLimitSize(
                            context: context,
                            data: column.TotalLimitSize.ToString()).ToJson();
                    case Error.Types.OverLocalFolderLimitSize:
                        return Messages.ResponseOverLimitSize(
                            context: context,
                            data: column.LocalFolderLimitSize.ToString()).ToJson();
                    case Error.Types.OverLocalFolderTotalLimitSize:
                        return Messages.ResponseOverTotalLimitSize(
                            context: context,
                            data: column.LocalFolderTotalLimitSize.ToString()).ToJson();
                    case Error.Types.OverTenantStorageSize:
                        return Messages.ResponseOverTenantStorageSize(
                            context: context,
                            data: context.ContractSettings.StorageSize.ToString()).ToJson();
                    case Error.Types.None: break;
                    default: return invalid.MessageJson(context);
                }
            }
            var controlId = context.Forms.Get("ControlId");
            var fileUuid = context.Forms.Get("uuid")?.Split(',');
            var fileUuids = context.Forms.Get("Uuids")?.Split(',');
            var fileNames = context.Forms.Get("fileNames")?.Deserialize<string[]>();
            var fileSizes = context.Forms.Get("fileSizes")?.Deserialize<string[]>();
            var fileTypes = context.Forms.Get("fileTypes")?.Deserialize<string[]>();
            // 一覧編集画面からアップロードが行われた場合、ControlIdにSuffixが付与される
            // Suffixが付与されている場合にはcontrolOnlyをtrueにしてLabelTextが出力されないようにする
            var controlOnly = !context.Forms.ControlId().RegexFirst("_\\d+_-?\\d+$").IsNullOrEmpty();
            if (Parameters.BinaryStorage.TemporaryBinaryStorageProvider == "Rds")
            {
                var resultFileNames = new List<dynamic>();
                // Binariesテーブルに直接アップロードしてきたバイナリデータを登録する
                for (int filesIndex = 0; filesIndex < context.PostedFiles.Count; ++filesIndex)
                {
                    using (var memory = new System.IO.MemoryStream())
                    {
                        var file = context.PostedFiles[filesIndex];
                        file.InputStream.CopyTo(memory);
                        Repository.ExecuteNonQuery(
                            context: context,
                            statements: new SqlStatement(
                                commandText: context.Sqls.UpsertBinary,
                                param: new SqlParamCollection{
                                    { "ReferenceId", context.Id },
                                    { "Guid", fileUuid[filesIndex] },
                                    { "BinaryType", "Temporary" },
                                    { "Title",  file.FileName },
                                    { "Bin", memory.ToArray() },
                                }));
                        resultFileNames.Add(new { name = file.FileName });
                    }
                    SaveTempFileSession(
                        context: context,
                        guid: fileUuid[filesIndex]);
                    var tempBinaryHash = Repository.ExecuteScalar_bytes(
                            context: context,
                            statements: new SqlStatement(
                                commandText: context.Sqls.GetBinaryHash,
                                param: new SqlParamCollection{
                                    { "Algorithm", "md5" },
                                    { "Guid", fileUuid[filesIndex] }
                                }));
                    var invalid = ValidateFileHash(
                        tempBinaryHash: tempBinaryHash,
                        contentRange: contentRange,
                        hash: fileHash);
                    if (invalid != Error.Types.None) return invalid.MessageJson(context);
                }
                return CreateResult(
                    resultFileNames: resultFileNames.ToArray(),
                    responseJson: CreateResponseJson(
                        context: context,
                        fileUuids: fileUuids,
                        fileNames: fileNames,
                        fileSizes: fileSizes,
                        fileTypes: fileTypes,
                        ss: ss,
                        column: column,
                        controlId: controlId,
                        attachments: attachments,
                        controlOnly: controlOnly));
            }
            else
            {
                var resultFileNames = new List<KeyValuePair<PostedFile, System.IO.FileInfo>>();
                for (int filesIndex = 0; filesIndex < context.PostedFiles.Count; ++filesIndex)
                {
                    var file = context.PostedFiles[filesIndex];
                    var saveFile = GetTempFileInfo(fileUuid[filesIndex], file.FileName);
                    Save(file, saveFile);
                    resultFileNames.Add(
                        new KeyValuePair<PostedFile, System.IO.FileInfo>(
                            file,
                            saveFile));
                    SaveTempFileSession(
                        context: context,
                        guid: fileUuid[filesIndex]);
                }
                var invalid = ValidateFileHash(
                    fileInfo: resultFileNames[0].Value,
                    contentRange: contentRange,
                    hash: fileHash);
                if (invalid != Error.Types.None) return invalid.MessageJson(context);
                return CreateResult(
                    resultFileNames: resultFileNames
                        .Select(file => new { name = file.Value.Name })
                        .ToArray(),
                    responseJson: CreateResponseJson(
                        context: context,
                        fileUuids: fileUuids,
                        fileNames: fileNames,
                        fileSizes: fileSizes,
                        fileTypes: fileTypes,
                        ss: ss,
                        column: column,
                        controlId: controlId,
                        attachments: attachments,
                        controlOnly: controlOnly));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.IO.FileInfo GetTempFileInfo(string fileUuid, string fileName)
        {
            var tempDirectoryInfo = new System.IO.DirectoryInfo(DefinitionAccessor.Directories.Temp());
            if (!tempDirectoryInfo.Exists)
                tempDirectoryInfo.Create();
            var saveFileInfo = new System.IO.FileInfo(System.IO.Path.Combine(tempDirectoryInfo.FullName, fileUuid, fileName));
            var saveDirectoryInfo = saveFileInfo.Directory;
            if (!saveDirectoryInfo.Exists)
                saveDirectoryInfo.Create();
            if (!saveFileInfo.Exists)
                using (var fileStream = saveFileInfo.Create())
                    fileStream.Flush();
            return saveFileInfo;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Save(PostedFile file, System.IO.FileInfo saveFile)
        {
            System.IO.FileStream saveFileStream = null;
            var en = Enumerable.Range(0, 100).ToArray();
            foreach (var index in en)
            {
                try
                {
                    saveFileStream = saveFile.Open(System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read);
                    if (saveFileStream != null) break;
                }
                catch (System.IO.IOException)
                {
                    if (index >= en.Last()) throw;
                }
            }
            using (saveFileStream)
            {
                file.InputStream.CopyTo(saveFileStream);
                saveFileStream.Flush();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string CreateResult(
            dynamic[] resultFileNames,
            string responseJson)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(
                new
                {
                    files = resultFileNames,
                    ResponseJson = responseJson
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string CreateResponseJson(
            Context context,
            IEnumerable<string> fileUuids,
            IEnumerable<string> fileNames,
            IEnumerable<string> fileSizes,
            IEnumerable<string> fileTypes,
            SiteSettings ss,
            Column column,
            string controlId,
            List<Attachment> attachments,
            bool controlOnly)
        {
            Enumerable.Range(0, new[]
            {
                fileUuids.Count(),
                fileNames.Count(),
                fileSizes.Count(),
                fileTypes.Count()
            }.Min()).ForEach(index =>
            {
                var fileName = fileNames.Skip(index).First();
                if (column.OverwriteSameFileName == true)
                {
                    OverwriteSameFileName(attachments, fileName);
                }
                attachments.Add(new Attachment()
                {
                    Guid = fileUuids.Skip(index).First(),
                    Name = fileName,
                    Size = fileSizes.Skip(index).First().ToLong(),
                    Extention = System.IO.Path.GetExtension(fileNames.Skip(index).First()),
                    ContentType = fileTypes.Skip(index).First(),
                    Added = true,
                    Deleted = false
                });
            });
            var hb = new HtmlBuilder();
            return new ResponseCollection(context: context)
                .ReplaceAll($"#{controlId}Field", new HtmlBuilder()
                    .Field(
                        context: context,
                        ss: ss,
                        column: column,
                        value: attachments.ToJson(),
                        columnPermissionType: Permissions.ColumnPermissionType(
                            context: context,
                            ss: ss,
                            column: column,
                            null),
                        controlOnly: controlOnly,
                        idSuffix: System.Text.RegularExpressions.Regex.Match(controlId, "_\\d+_-?\\d+").Value))
                .SetData("#" + controlId)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types ValidateFileHash(
            System.IO.FileInfo fileInfo,
            System.Net.Http.Headers.ContentRangeHeaderValue contentRange,
            string hash)
        {
            if (string.IsNullOrEmpty(hash)) return Error.Types.None;
            if (contentRange.Length > (contentRange.To + 1)) return Error.Types.None;
            byte[] hashValue;
            using (var fileStream = fileInfo.Open(System.IO.FileMode.Open))
            {
                fileStream.Position = 0;
                hashValue = System.Security.Cryptography.MD5.Create().ComputeHash(fileStream);
                fileStream.Close();
            }
            return CompareFileHash(
                hash: hash,
                bytes: hashValue);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types ValidateFileHash(
            byte[] tempBinaryHash,
            System.Net.Http.Headers.ContentRangeHeaderValue contentRange,
            string hash)
        {
            if (string.IsNullOrEmpty(hash)) return Error.Types.None;
            if (contentRange.Length > (contentRange.To + 1)) return Error.Types.None;
            return CompareFileHash(
                hash: hash,
                bytes: tempBinaryHash);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types CompareFileHash(string hash, byte[] bytes)
        {
            var fileHash = string.Join(string.Empty, bytes.Select(h => h.ToString("x2")));
            return hash == fileHash ? Error.Types.None : Error.Types.InvalidRequest;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string TrimIdSuffix(string element)
        {
            var regex = new System.Text.RegularExpressions.Regex("_\\d+_-?\\d+$");
            return regex.Match(element).Value.IsNullOrEmpty()
                ? element
                : element.Replace(regex.Match(element).Value, string.Empty);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void OverwriteSameFileName(List<Attachment> attachments, String fileName)
        {
            attachments.ForEach(savedAttachment =>
            {
                if (savedAttachment.Name == fileName)
                {
                    savedAttachment.Deleted = true;
                    savedAttachment.Overwritten = true;
                }
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ErrorData HasPermission(Context context, SiteSettings ss, ItemModel itemModel)
        {
            if (ss.SiteId == ss.ReferenceId && itemModel.ReferenceType == "Sites")
                return context.CanCreate(ss)
                    ? new ErrorData(Error.Types.None)
                    : new ErrorData(Error.Types.HasNotPermission);
            switch (ss.ReferenceType)
            {
                case "Issues":
                    return IssueValidators.OnUpdating(
                        context: context,
                        ss: ss,
                        issueModel: new IssueModel(
                            context: context,
                            ss: ss,
                            issueId: context.Id));
                case "Results":
                    return ResultValidators.OnUpdating(
                        context: context,
                        ss: ss,
                        new ResultModel(
                            context: context,
                            ss: ss,
                            resultId: context.Id));
                default: return new ErrorData(Error.Types.HasNotPermission);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void DeleteFromLocal(
            Context context,
            EnumerableRowCollection<DataRow> dataRows,
            SiteSettings ss = null,
            Column column = null,
            long referenceId = 0,
            bool verUp = false)
        {
            dataRows.ForEach(binary =>
            {
                var binaryType = binary.String("BinaryType");
                if (binaryType == "Attachments")
                {
                    new Attachment() { Guid = binary.String("Guid") }.DeleteFromLocal(
                        context: context,
                        ss: ss,
                        column: column,
                        referenceId: referenceId,
                        verUp: verUp);
                }
                else if (binaryType == "Images")
                {
                    var path = System.IO.Path.Combine(
                        Directories.BinaryStorage(),
                        "Images",
                        binary.String("Guid"));
                    if (System.IO.File.Exists(path))
                    {
                        Files.DeleteFile(path);
                    }
                }
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Net.Http.Headers.ContentRangeHeaderValue GetContentRange(string contentRangeHeader)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(contentRangeHeader ?? string.Empty, "\\d+");
            return matches.Count > 0
                ? new System.Net.Http.Headers.ContentRangeHeaderValue(
                    long.Parse(matches[0].Value),
                    long.Parse(matches[1].Value),
                    long.Parse(matches[2].Value))
                : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance CreateAttachment(
            Context context,
            Attachment attachment)
        {
            var invalid = BinaryValidators.OnUploading(
                context: context,
                attachments: new Attachments() { attachment });
            if (invalid != Error.Types.None)
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: invalid));
            }
            return attachment.Create(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void UpdateImageReferenceId(
            Context context,
            long siteId,
            long referenceId)
        {
            Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectBinaries(
                column: Rds.BinariesColumn().Guid(),
                where: Rds.BinariesWhere()
                    .ReferenceId(referenceId)
                    .BinaryType("Images")))
                    .AsEnumerable()
                    .ForEach(dataRow =>
                    {
                        var guid = dataRow.String("Guid");
                        var id = Rds.ExecuteScalar_long(
                            context: context,
                            statements: Rds.SelectItems(
                            column: Rds.ItemsColumn().ReferenceId(),
                            where: Rds.ItemsWhere()
                                .SiteId(siteId)
                                .FullText($"%/{guid}/%", _operator: context.Sqls.Like),
                            top: 1));
                        if (id != 0)
                        {
                            // 削除対象となっている ReferenceId が見付かった場合には、引き続き該当の Images への参照を維持している ReferenceId へ変更
                            Rds.ExecuteNonQuery(
                                context: context,
                                statements: Rds.UpdateBinaries(
                                where: Rds.BinariesWhere()
                                    .TenantId(context.TenantId)
                                    .ReferenceId(referenceId)
                                    .BinaryType("Images")
                                    .Guid(guid),
                                param: Rds.BinariesParam().ReferenceId(id)));
                        }
                    });
        }
    }
}
