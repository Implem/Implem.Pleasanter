using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class BinaryUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool ExistsSiteImage(
            SiteSettings ss,
            long referenceId,
            Libraries.Images.ImageData.SizeTypes sizeType)
        {
            var invalid = BinaryValidators.OnGetting(ss);
            switch (invalid)
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
                    return Rds.ExecuteScalar_int(statements:
                        Rds.SelectBinaries(
                            column: Rds.BinariesColumn().BinariesCount(),
                            where: Rds.BinariesWhere()
                                .ReferenceId(referenceId)
                                .BinaryType("SiteImage"))) == 1;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteImagePrefix(
            SiteSettings ss,
            long referenceId,
            Libraries.Images.ImageData.SizeTypes sizeType)
        {
            var invalid = BinaryValidators.OnGetting(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return string.Empty;
            }
            return new BinaryModel(referenceId).SiteImagePrefix(sizeType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static byte[] SiteImageThumbnail(SiteModel siteModel)
        {
            siteModel.SiteSettings = SiteSettingsUtilities.Get(siteModel, siteModel.SiteId);
            var invalid = BinaryValidators.OnGetting(siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            return new BinaryModel(siteModel.SiteId).SiteImage(
                Libraries.Images.ImageData.SizeTypes.Thumbnail,
                Rds.BinariesColumn().Thumbnail());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static byte[] SiteImageIcon(SiteModel siteModel)
        {
            siteModel.SiteSettings = SiteSettingsUtilities.Get(siteModel, siteModel.SiteId);
            var invalid = BinaryValidators.OnGetting(siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            return new BinaryModel(siteModel.SiteId).SiteImage(
                Libraries.Images.ImageData.SizeTypes.Icon,
                Rds.BinariesColumn().Icon());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UpdateSiteImage(SiteModel siteModel)
        {
            siteModel.SiteSettings = SiteSettingsUtilities.Get(siteModel, siteModel.SiteId);
            var file = Forms.File(Libraries.Images.ImageData.Types.SiteImage.ToString());
            var invalid = BinaryValidators.OnUploadingSiteImage(siteModel.SiteSettings, file);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = new BinaryModel(siteModel.SiteId).UpdateSiteImage(file);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return new ResponseCollection()
                    .Html(
                        "#SiteImageIconContainer",
                        new HtmlBuilder().SiteImageIcon(
                            ss: siteModel.SiteSettings,
                            siteId: siteModel.SiteId))
                    .Html(
                        "#SiteImageSettingsEditor",
                        new HtmlBuilder().SiteImageSettingsEditor(
                            ss: siteModel.SiteSettings))
                    .Message(Messages.FileUpdateCompleted())
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteSiteImage(SiteModel siteModel)
        {
            siteModel.SiteSettings = SiteSettingsUtilities.Get(siteModel, siteModel.SiteId);
            var invalid = BinaryValidators.OnDeletingSiteImage(siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = new BinaryModel(siteModel.SiteId).DeleteSiteImage();
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return new ResponseCollection()
                    .Html(
                        "#SiteImageIconContainer",
                        new HtmlBuilder().SiteImageIcon(
                            ss: siteModel.SiteSettings,
                            siteId: siteModel.SiteId))
                    .Html(
                        "#SiteImageSettingsEditor",
                        new HtmlBuilder().SiteImageSettingsEditor(
                            ss: siteModel.SiteSettings))
                    .Message(Messages.FileDeleteCompleted())
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UploadImage(System.Web.HttpPostedFileBase[] files, long id)
        {
            var controlId = Forms.ControlId();
            var ss = new ItemModel(id).GetSite(initSiteSettings: true).SiteSettings;
            var invalid = BinaryValidators.OnUploadingImage(files);
            switch (invalid)
            {
                case Error.Types.OverTenantStorageSize:
                    return Messages.ResponseOverTenantStorageSize(
                        Contract.TenantStorageSize().ToString()).ToJson();
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var guid = Strings.NewGuid();
            var file = files[0];
            var size = file.ContentLength;
            var bin = file.Byte();
            Rds.ExecuteNonQuery(statements:
                Rds.InsertBinaries(
                    param: Rds.BinariesParam()
                        .TenantId(Sessions.TenantId())
                        .ReferenceId(id)
                        .Guid(guid)
                        .BinaryType("Images")
                        .Title(file.FileName)
                        .Bin(bin, _using: !Parameters.BinaryStorage.IsLocal())
                        .FileName(file.FileName)
                        .Extension(file.Extension())
                        .Size(size)
                        .ContentType(file.ContentType)));
            if (Parameters.BinaryStorage.IsLocal())
            {
                bin.Write(System.IO.Path.Combine(Directories.BinaryStorage(), "Images", guid));
            }
            var hb = new HtmlBuilder();
            return new ResponseCollection()
                .InsertText("#" + Forms.ControlId(), $"![image]({Locations.ShowFile(guid)})")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteImage(string guid)
        {
            var binaryModel = new BinaryModel()
                .Get(where: Rds.BinariesWhere()
                    .TenantId(Sessions.TenantId())
                    .Guid(guid));
            var ss = new ItemModel(binaryModel.ReferenceId)
                .GetSite(initSiteSettings: true).SiteSettings;
            var invalid = BinaryValidators.OnDeletingImage(ss, binaryModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            binaryModel.Delete();
            return new ResponseCollection()
                .Message(Messages.DeletedImage())
                .Remove($"#ImageLib .item[data-id=\"{guid}\"]")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string MultiUpload(System.Web.HttpPostedFileBase[] files, long id)
        {
            var controlId = Forms.ControlId();
            var ss = new ItemModel(id).GetSite(initSiteSettings: true).SiteSettings;
            var column = ss.GetColumn(Forms.Data("ColumnName"));
            var attachments = Forms.Data("AttachmentsData").Deserialize<Attachments>();
            var invalid = BinaryValidators.OnUploading(column, attachments, files);
            switch (invalid)
            {
                case Error.Types.OverLimitQuantity:
                    return Messages.ResponseOverLimitQuantity(
                        column.LimitQuantity.ToString()).ToJson();
                case Error.Types.OverLimitSize:
                    return Messages.ResponseOverLimitSize(
                        column.LimitSize.ToString()).ToJson();
                case Error.Types.OverTotalLimitSize:
                    return Messages.ResponseOverTotalLimitSize(
                        column.TotalLimitSize.ToString()).ToJson();
                case Error.Types.OverTenantStorageSize:
                    return Messages.ResponseOverTenantStorageSize(
                        Contract.TenantStorageSize().ToString()).ToJson();
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            files.ForEach(file => attachments.Add(new Attachment()
            {
                Guid = file.WriteToTemp(),
                Name = file.FileName.Split('\\').Last(),
                Size = file.ContentLength,
                Extention = file.Extension(),
                ContentType = file.ContentType,
                Added = true,
                Deleted = false
            }));
            var hb = new HtmlBuilder();
            return new ResponseCollection()
                .ReplaceAll($"#{controlId}Field", new HtmlBuilder()
                    .Field(
                        ss: ss,
                        column: column,
                        value: attachments.ToJson(),
                        columnPermissionType: column.ColumnPermissionType()))
                .SetData("#" + controlId)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.FileContentResult Donwload(string guid)
        {
            if (!Contract.Attachments())
            {
                return null;
            }
            return FileContentResults.Download(guid);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.FileContentResult DownloadTemp(string guid)
        {
            if (!Contract.Attachments())
            {
                return null;
            }
            return FileContentResults.DownloadTemp(guid);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteTemp()
        {
            if (!Contract.Attachments())
            {
                return null;
            }
            File.DeleteTemp(Forms.Data("Guid"));
            return "[]";
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static long UsedTenantStorageSize()
        {
            return Rds.ExecuteScalar_long(statements: Rds.SelectBinaries(
                column: Rds.BinariesColumn().Size(function: Sqls.Functions.Sum),
                where: Rds.BinariesWhere().TenantId(Sessions.TenantId())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlStatement UpdateReferenceId(
            SiteSettings ss, long referenceId, string values)
        {
            var guids = values?.RegexValues("[0-9a-z]{32}").ToList();
            return guids?.Any() == true
                ? Rds.UpdateBinaries(
                    param: Rds.BinariesParam().ReferenceId(referenceId),
                    where: Rds.BinariesWhere()
                        .TenantId(Sessions.TenantId())
                        .ReferenceId(ss.SiteId)
                        .Guid(guids, multiParamOperator: " or ")
                        .Creator(Sessions.UserId()))
                : null;
        }
    }
}
