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
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        referenceId, Libraries.Images.ImageData.Types.SiteImage)
                            .Exists(sizeType);
                default:
                    return Repository.ExecuteScalar_int(
                        context: context,
                        statements: Rds.SelectBinaries(
                            column: Rds.BinariesColumn().BinariesCount(),
                            where: Rds.BinariesWhere()
                                .ReferenceId(referenceId)
                                .BinaryType("SiteImage"))) == 1;
            }
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
            return  (
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
                context: context, bin: bin);
            if (error.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection()
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
                context: context, bin: bin);
            if (error.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection()
                   .ReplaceAll(
                       "#Logo",
                       new HtmlBuilder().HeaderLogo(
                           context: context))
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
                return new ResponseCollection()
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
                return new ResponseCollection()
                   .ReplaceAll(
                       "#Logo",
                       new HtmlBuilder().HeaderLogo(
                           context: context))
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
        public static string UploadImage(Context context, long id)
        {
            var invalid = BinaryValidators.OnUploadingImage(context: context);
            switch (invalid)
            {
                case Error.Types.OverTenantStorageSize:
                    return Messages.ResponseOverTenantStorageSize(
                        context: context,
                        data: context.ContractSettings.StorageSize.ToString()).ToJson();
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var file = context.PostedFiles.FirstOrDefault();
            var bin = file.Byte();
            var columnName = context.Forms.Data("ControlId");
            if (columnName.Contains("_"))
            {
                columnName = columnName.Substring(columnName.IndexOf("_") + 1);
            }
            if (columnName.StartsWith("Comment"))
            {
                columnName = "Comments";
            }
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
                        .BinaryType("Images")
                        .Title(file.FileName)
                        .Bin(bin, _using: !Parameters.BinaryStorage.IsLocal())
                        .Thumbnail(thumbnail, _using: thumbnail != null)
                        .FileName(file.FileName)
                        .Extension(file.Extension)
                        .Size(file.Size)
                        .ContentType(file.ContentType)));
            return new ResponseCollection()
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
            return new ResponseCollection()
                .Message(Messages.DeletedImage(context: context))
                .Remove($"#ImageLib .item[data-id=\"{guid}\"]")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string MultiUpload(Context context, long id)
        {
            var controlId = context.Forms.ControlId();
            var ss = new ItemModel(
                context: context,
                referenceId: id).GetSite(
                    context: context,
                    initSiteSettings: true)
                        .SiteSettings;
            var column = ss.GetColumn(
                context: context,
                columnName: context.Forms.Data("ColumnName"));
            var attachments = context.Forms.Data("AttachmentsData").Deserialize<Attachments>();
            var invalid = BinaryValidators.OnUploading(
                context: context,
                column: column,
                attachments: attachments);
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
                case Error.Types.OverTenantStorageSize:
                    return Messages.ResponseOverTenantStorageSize(
                        context: context,
                        data: context.ContractSettings.StorageSize.ToString()).ToJson();
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            context.PostedFiles.ForEach(file => attachments.Add(new Attachment()
            {
                Guid = file.Guid,
                Name = file.FileName.Split('\\').Last(),
                Size = file.Size,
                Extention = file.Extension,
                ContentType = file.ContentType,
                Added = true,
                Deleted = false
            }));
            var hb = new HtmlBuilder();
            var fieldId = controlId + "Field";
            return new ResponseCollection()
                .ReplaceAll("#" + fieldId, new HtmlBuilder()
                    .FieldAttachments(
                        context: context,
                        fieldId: fieldId,
                        controlId: controlId,
                        columnName: column.ColumnName,
                        fieldCss: column.FieldCss
                            + (column.TextAlign == SiteSettings.TextAlignTypes.Right
                                ? " right-align"
                                : string.Empty),
                        fieldDescription: column.Description,
                        labelText: column.LabelText,
                        value: attachments.ToJson(),
                        readOnly: column.ColumnPermissionType(context: context)
                            != Permissions.ColumnPermissionTypes.Update))
                .SetData("#" + controlId)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.FileContentResult Donwload(Context context, string guid)
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
        public static System.Web.Mvc.ContentResult ApiDonwload(Context context, string guid)
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
        public static System.Web.Mvc.FileContentResult DownloadTemp(Context context, string guid)
        {
            if (!context.ContractSettings.Attachments())
            {
                return null;
            }
            return FileContentResults.DownloadTemp(guid.ToUpper());
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
            Libraries.DataSources.File.DeleteTemp(context.Forms.Data("Guid"));
            return "[]";
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
            var guids = values?.RegexValues("[0-9a-z]{32}").ToList();
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
    }
}
