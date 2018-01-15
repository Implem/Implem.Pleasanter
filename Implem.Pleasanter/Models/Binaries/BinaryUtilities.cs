using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
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
                            where: Rds.BinariesWhere().ReferenceId(referenceId))) == 1;
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
            var invalid = BinaryValidators.OnUpdating(siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = new BinaryModel(siteModel.SiteId).UpdateSiteImage(
                Forms.File(Libraries.Images.ImageData.Types.SiteImage.ToString()));
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return Messages.ResponseFileUpdateCompleted().ToJson();
            }
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
                    return Messages.OverLimitSize(
                        column.LimitSize.ToString()).ToJson();
                case Error.Types.OverTotalLimitSize:
                    return Messages.OverTotalLimitSize(
                        column.TotalLimitSize.ToString()).ToJson();
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            files.ForEach(file => attachments.Add(new Attachment()
            {
                Guid = file.WriteToTemp(),
                Name = file.FileName,
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
    }
}