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
            Permissions.Types permissionType,
            long referenceId,
            Libraries.Images.ImageData.SizeTypes sizeType)
        {
            var invalid = BinaryValidators.OnGetting(permissionType);
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
            Permissions.Types permissionType,
            long referenceId,
            Libraries.Images.ImageData.SizeTypes sizeType)
        {
            var invalid = BinaryValidators.OnGetting(permissionType);
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
            var invalid = BinaryValidators.OnGetting(siteModel.PermissionType);
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
            var invalid = BinaryValidators.OnGetting(siteModel.PermissionType);
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
            var invalid = BinaryValidators.OnUpdating(siteModel.PermissionType);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
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
    }
}
