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
        public static string EditorNew()
        {
            return Editor(new BinaryModel(
                SiteSettingsUtility.BinariesSiteSettings(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long binaryId, bool clearSessions)
        {
            var binaryModel = new BinaryModel(
                SiteSettingsUtility.BinariesSiteSettings(),
                binaryId: binaryId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            binaryModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtility.BinariesSiteSettings());
            return Editor(binaryModel);
        }

        public static string Editor(BinaryModel binaryModel)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            return hb.Template(
                permissionType: permissionType,
                verType: binaryModel.VerType,
                methodType: binaryModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    binaryModel.AccessStatus != Databases.AccessStatuses.NotFound,
                referenceType: "Binaries",
                title: binaryModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Binaries() + " - " + Displays.New()
                    : binaryModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            binaryModel: binaryModel,
                            permissionType: permissionType,
                            siteSettings: binaryModel.SiteSettings)
                        .Hidden(controlId: "TableName", value: "Binaries")
                        .Hidden(controlId: "Id", value: binaryModel.BinaryId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            BinaryModel binaryModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("BinaryForm")
                        .Class("main-form")
                        .Action(binaryModel.BinaryId != 0
                            ? Navigations.Action("Binaries", binaryModel.BinaryId)
                            : Navigations.Action("Binaries")),
                    action: () => hb
                        .RecordHeader(
                            permissionType: permissionType,
                            baseModel: binaryModel,
                            tableName: "Binaries")
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: binaryModel.Comments,
                                verType: binaryModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(binaryModel: binaryModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                binaryModel: binaryModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: binaryModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: binaryModel.VerType,
                                referenceType: "Binaries",
                                referenceId: binaryModel.BinaryId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        binaryModel: binaryModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: binaryModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Binaries_Timestamp",
                            css: "must-transport",
                            value: binaryModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: binaryModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("Binaries", binaryModel.BinaryId, binaryModel.Ver)
                .CopyDialog("Binaries", binaryModel.BinaryId)
                .OutgoingMailDialog()
                .EditorExtensions(binaryModel: binaryModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, BinaryModel binaryModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: binaryModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            BinaryModel binaryModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.EditorColumnCollection().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "ReferenceId": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.ReferenceId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "BinaryId": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.BinaryId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Ver": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "BinaryType": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.BinaryType.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Title": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Body": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Body.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "FileName": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.FileName.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Extension": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Extension.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        case "Size": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Size.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                    }
                });
                hb.VerUpCheckBox(binaryModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            BinaryModel binaryModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            BinaryModel binaryModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static List<long> GetSwitchTargets(SiteSettings siteSettings)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData();
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectBinaries(
                        column: Rds.BinariesColumn().BinaryId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Binaries",
                            formData: formData,
                            where: Rds.BinariesWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.BinariesOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["BinaryId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

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
                Libraries.Images.ImageData.SizeTypes.Thumbnail,
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
