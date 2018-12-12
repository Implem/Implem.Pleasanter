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
    [Serializable]
    public class BinaryModel : BaseModel
    {
        public long BinaryId = 0;
        public int TenantId = 0;
        public long ReferenceId = 0;
        public string Guid = string.Empty;
        public string BinaryType = string.Empty;
        public Title Title = new Title();
        public string Body = string.Empty;
        public byte[] Bin = null;
        public byte[] Thumbnail = null;
        public byte[] Icon = null;
        public byte[] Logo = null;
        public string FileName = string.Empty;
        public string Extension = string.Empty;
        public long Size = 0;
        public string ContentType = string.Empty;
        public BinarySettings BinarySettings = new BinarySettings();
        [NonSerialized] public long SavedBinaryId = 0;
        [NonSerialized] public int SavedTenantId = 0;
        [NonSerialized] public long SavedReferenceId = 0;
        [NonSerialized] public string SavedGuid = string.Empty;
        [NonSerialized] public string SavedBinaryType = string.Empty;
        [NonSerialized] public string SavedTitle = string.Empty;
        [NonSerialized] public string SavedBody = string.Empty;
        [NonSerialized] public byte[] SavedBin = null;
        [NonSerialized] public byte[] SavedThumbnail = null;
        [NonSerialized] public byte[] SavedIcon = null;
        [NonSerialized] public byte[] SavedLogo = null;
        [NonSerialized] public string SavedFileName = string.Empty;
        [NonSerialized] public string SavedExtension = string.Empty;
        [NonSerialized] public long SavedSize = 0;
        [NonSerialized] public string SavedContentType = string.Empty;
        [NonSerialized] public string SavedBinarySettings = "{}";

        public bool BinaryId_Updated(Context context, Column column = null)
        {
            return BinaryId != SavedBinaryId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != BinaryId);
        }

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool ReferenceId_Updated(Context context, Column column = null)
        {
            return ReferenceId != SavedReferenceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != ReferenceId);
        }

        public bool Guid_Updated(Context context, Column column = null)
        {
            return Guid != SavedGuid && Guid != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Guid);
        }

        public bool BinaryType_Updated(Context context, Column column = null)
        {
            return BinaryType != SavedBinaryType && BinaryType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != BinaryType);
        }

        public bool Title_Updated(Context context, Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Title.Value);
        }

        public bool Body_Updated(Context context, Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Body);
        }

        public bool FileName_Updated(Context context, Column column = null)
        {
            return FileName != SavedFileName && FileName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != FileName);
        }

        public bool Extension_Updated(Context context, Column column = null)
        {
            return Extension != SavedExtension && Extension != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Extension);
        }

        public bool Size_Updated(Context context, Column column = null)
        {
            return Size != SavedSize &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != Size);
        }

        public bool ContentType_Updated(Context context, Column column = null)
        {
            return ContentType != SavedContentType && ContentType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ContentType);
        }

        public bool BinarySettings_Updated(Context context, Column column = null)
        {
            return BinarySettings.ToJson() != SavedBinarySettings && BinarySettings.ToJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != BinarySettings.ToJson());
        }

        public bool Bin_Updated(Context context, Column column = null)
        {
            return Bin != SavedBin && Bin != null;
        }

        public bool Thumbnail_Updated(Context context, Column column = null)
        {
            return Thumbnail != SavedThumbnail && Thumbnail != null;
        }

        public bool Icon_Updated(Context context, Column column = null)
        {
            return Icon != SavedIcon && Icon != null;
        }

        public bool Logo_Updated(Context context, Column column = null)
        {
            return Logo != SavedLogo && Logo != null;
        }

        public BinarySettings Session_BinarySettings(Context context)
        {
            return context.SessionData.Get("BinarySettings") != null
                ? context.SessionData.Get("BinarySettings")?.ToString().Deserialize<BinarySettings>() ?? new BinarySettings()
                : BinarySettings;
        }

        public void Session_BinarySettings(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "BinarySettings",
                value: value,
                page: true);
        }

        public BinaryModel()
        {
        }

        public BinaryModel(
            Context context,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public BinaryModel(
            Context context,
            long binaryId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            BinaryId = binaryId;
            Get(context: context);
            if (clearSessions) ClearSessions(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public BinaryModel(Context context, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            if (dataRow != null) Set(context, dataRow, tableAlias);
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
            Session_BinarySettings(context: context, value: null);
        }

        public BinaryModel Get(
            Context context,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(context, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectBinaries(
                    tableType: tableType,
                    column: column ?? Rds.BinariesDefaultColumns(),
                    join: join ??  Rds.BinariesJoinDefault(),
                    where: where ?? Rds.BinariesWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public Error.Types Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            TenantId = context.TenantId;
            Size = Bin.Length;
            var statements = new List<SqlStatement>();
            CreateStatements(context, statements, tableType, param, otherInitValue);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            BinaryId = (response.Identity ?? BinaryId).ToLong();
            if (get) Get(context: context);
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertBinaries(
                    tableType: tableType,
                    setIdentity: true,
                    param: param ?? Rds.BinariesParamDefault(
                        context: context,
                        binaryModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue))
            });
            return statements;
        }

        public Error.Types Update(
            Context context,
            SiteSettings ss,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            if (setBySession) SetBySession(context: context);
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(
                context: context,
                ss: ss,
                statements: statements,
                timestamp: timestamp,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Count == 0) return Error.Types.UpdateConflicts;
            if (get) Get(context: context);
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.BinariesWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateBinaries(
                    where: where,
                    param: param ?? Rds.BinariesParamDefault(
                        context: context, binaryModel: this, otherInitValue: otherInitValue),
                    countRecord: true)
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.BinariesColumnCollection();
            var param = new Rds.BinariesParamCollection();
            column.BinaryId(function: Sqls.Functions.SingleColumn); param.BinaryId();
            column.TenantId(function: Sqls.Functions.SingleColumn); param.TenantId();
            column.ReferenceId(function: Sqls.Functions.SingleColumn); param.ReferenceId();
            column.Guid(function: Sqls.Functions.SingleColumn); param.Guid();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.BinaryType(function: Sqls.Functions.SingleColumn); param.BinaryType();
            column.Title(function: Sqls.Functions.SingleColumn); param.Title();
            column.Body(function: Sqls.Functions.SingleColumn); param.Body();
            column.Bin(function: Sqls.Functions.SingleColumn); param.Bin();
            column.Thumbnail(function: Sqls.Functions.SingleColumn); param.Thumbnail();
            column.Icon(function: Sqls.Functions.SingleColumn); param.Icon();
            column.Logo(function: Sqls.Functions.SingleColumn); param.Logo();
            column.FileName(function: Sqls.Functions.SingleColumn); param.FileName();
            column.Extension(function: Sqls.Functions.SingleColumn); param.Extension();
            column.Size(function: Sqls.Functions.SingleColumn); param.Size();
            column.ContentType(function: Sqls.Functions.SingleColumn); param.ContentType();
            column.BinarySettings(function: Sqls.Functions.SingleColumn); param.BinarySettings();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            return Rds.InsertBinaries(
                tableType: tableType,
                param: param,
                select: Rds.SelectBinaries(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types UpdateOrCreate(
            Context context,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertBinaries(
                    where: where ?? Rds.BinariesWhereDefault(this),
                    param: param ?? Rds.BinariesParamDefault(
                        context: context, binaryModel: this, setDefault: true))
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            BinaryId = (response.Identity ?? BinaryId).ToLong();
            Get(context: context);
            return Error.Types.None;
        }

        public Error.Types Delete(Context context)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.BinariesWhere().BinaryId(BinaryId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteBinaries(where: where)
            });
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(Context context, long binaryId)
        {
            BinaryId = binaryId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreBinaries(
                        where: Rds.BinariesWhere().BinaryId(BinaryId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteBinaries(
                    tableType: tableType,
                    param: Rds.BinariesParam().BinaryId(BinaryId)));
            return Error.Types.None;
        }

        public void SetByModel(BinaryModel binaryModel)
        {
            TenantId = binaryModel.TenantId;
            ReferenceId = binaryModel.ReferenceId;
            Guid = binaryModel.Guid;
            BinaryType = binaryModel.BinaryType;
            Title = binaryModel.Title;
            Body = binaryModel.Body;
            Bin = binaryModel.Bin;
            Thumbnail = binaryModel.Thumbnail;
            Icon = binaryModel.Icon;
            Logo = binaryModel.Logo;
            FileName = binaryModel.FileName;
            Extension = binaryModel.Extension;
            Size = binaryModel.Size;
            ContentType = binaryModel.ContentType;
            BinarySettings = binaryModel.BinarySettings;
            Comments = binaryModel.Comments;
            Creator = binaryModel.Creator;
            Updator = binaryModel.Updator;
            CreatedTime = binaryModel.CreatedTime;
            UpdatedTime = binaryModel.UpdatedTime;
            VerUp = binaryModel.VerUp;
            Comments = binaryModel.Comments;
        }

        private void SetBySession(Context context)
        {
            if (!context.Forms.Exists("Binaries_BinarySettings")) BinarySettings = Session_BinarySettings(context: context);
        }

        private void Set(Context context, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "BinaryId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                BinaryId = dataRow[column.ColumnName].ToLong();
                                SavedBinaryId = BinaryId;
                            }
                            break;
                        case "TenantId":
                            TenantId = dataRow[column.ColumnName].ToInt();
                            SavedTenantId = TenantId;
                            break;
                        case "ReferenceId":
                            ReferenceId = dataRow[column.ColumnName].ToLong();
                            SavedReferenceId = ReferenceId;
                            break;
                        case "Guid":
                            Guid = dataRow[column.ColumnName].ToString();
                            SavedGuid = Guid;
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "BinaryType":
                            BinaryType = dataRow[column.ColumnName].ToString();
                            SavedBinaryType = BinaryType;
                            break;
                        case "Title":
                            Title = new Title(dataRow, "BinaryId");
                            SavedTitle = Title.Value;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "Bin":
                            Bin = dataRow.Bytes("Bin");
                            SavedBin = Bin;
                            break;
                        case "Thumbnail":
                            Thumbnail = dataRow.Bytes("Bin");
                            SavedThumbnail = Thumbnail;
                            break;
                        case "Icon":
                            Icon = dataRow.Bytes("Bin");
                            SavedIcon = Icon;
                            break;
                        case "Logo":
                            Logo = dataRow.Bytes("Bin");
                            SavedLogo = Logo;
                            break;
                        case "FileName":
                            FileName = dataRow[column.ColumnName].ToString();
                            SavedFileName = FileName;
                            break;
                        case "Extension":
                            Extension = dataRow[column.ColumnName].ToString();
                            SavedExtension = Extension;
                            break;
                        case "Size":
                            Size = dataRow[column.ColumnName].ToLong();
                            SavedSize = Size;
                            break;
                        case "ContentType":
                            ContentType = dataRow[column.ColumnName].ToString();
                            SavedContentType = ContentType;
                            break;
                        case "BinarySettings":
                            BinarySettings = dataRow[column.ColumnName].ToString().Deserialize<BinarySettings>() ?? new BinarySettings();
                            SavedBinarySettings = BinarySettings.ToJson();
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return
                BinaryId_Updated(context: context) ||
                TenantId_Updated(context: context) ||
                ReferenceId_Updated(context: context) ||
                Guid_Updated(context: context) ||
                Ver_Updated(context: context) ||
                BinaryType_Updated(context: context) ||
                Title_Updated(context: context) ||
                Body_Updated(context: context) ||
                FileName_Updated(context: context) ||
                Extension_Updated(context: context) ||
                Size_Updated(context: context) ||
                ContentType_Updated(context: context) ||
                BinarySettings_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SiteImagePrefix(
            Context context, Libraries.Images.ImageData.SizeTypes sizeType)
        {
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        ReferenceId, Libraries.Images.ImageData.Types.SiteImage)
                            .UrlPrefix(sizeType);
                default:
                    return Rds.ExecuteScalar_datetime(
                        context: context,
                        statements: Rds.SelectBinaries(
                            column: Rds.BinariesColumn()
                                .UpdatedTime(function: Sqls.Functions.Max),
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("SiteImage")))
                                    .ToString("?yyyyMMddHHmmss");
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string TenantImagePrefix(
            Context context, Libraries.Images.ImageData.SizeTypes sizeType)
        {
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        ReferenceId, Libraries.Images.ImageData.Types.TenantImage)
                            .UrlPrefix(sizeType);
                default:
                    return Rds.ExecuteScalar_datetime(
                        context: context,
                        statements: Rds.SelectBinaries(
                            column: Rds.BinariesColumn()
                                .UpdatedTime(function: Sqls.Functions.Max),
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("TenantImage")))
                                    .ToString("?yyyyMMddHHmmss");
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public byte[] SiteImage(
            Context context,
            Libraries.Images.ImageData.SizeTypes sizeType,
            SqlColumnCollection column)
        {
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        ReferenceId, Libraries.Images.ImageData.Types.SiteImage)
                            .Read(sizeType);
                default:
                    return Rds.ExecuteScalar_bytes(
                        context: context,
                        statements: Rds.SelectBinaries(
                            column: column,
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("SiteImage")));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public byte[] TenantImage(
            Context context,
            Libraries.Images.ImageData.SizeTypes sizeType,
            SqlColumnCollection column)
        {
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        ReferenceId, Libraries.Images.ImageData.Types.TenantImage)
                            .Read(sizeType);
                default:
                    return Rds.ExecuteScalar_bytes(
                        context: context,
                        statements: Rds.SelectBinaries(
                            column: column,
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("TenantImage")));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types UpdateSiteImage(Context context, byte[] bin)
        {
            BinaryType = "SiteImage";
            var imageData = new Libraries.Images.ImageData(
                bin,
                ReferenceId,
                Libraries.Images.ImageData.Types.SiteImage);
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local": imageData.WriteToLocal(); break;
                default:
                    Bin = imageData.ReSizeBytes(Libraries.Images.ImageData.SizeTypes.Regular);
                    Thumbnail = imageData.ReSizeBytes(
                        Libraries.Images.ImageData.SizeTypes.Thumbnail);
                    Icon = imageData.ReSizeBytes(Libraries.Images.ImageData.SizeTypes.Icon);
                    Rds.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.UpdateOrInsertBinaries(
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("SiteImage"),
                            param: Rds.BinariesParamDefault(
                                context: context,
                                binaryModel: this,
                                setDefault: true)));
                    break;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types UpdateTenantImage(Context context, byte[] bin)
        {
            BinaryType = "TenantImage";
            var imageData = new Libraries.Images.ImageData(
                bin,
                ReferenceId,
                Libraries.Images.ImageData.Types.TenantImage);
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local": imageData.WriteToLocal(); break;
                default:
                    Logo = imageData.ReSizeBytes(Libraries.Images.ImageData.SizeTypes.Logo);
                    Rds.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.UpdateOrInsertBinaries(
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("TenantImage"),
                            param: Rds.BinariesParamDefault(
                                context: context,
                                binaryModel: this,
                                setDefault: true)));
                    break;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types DeleteSiteImage(Context context)
        {
            BinaryType = "SiteImage";
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    new Libraries.Images.ImageData(
                        ReferenceId,
                        Libraries.Images.ImageData.Types.SiteImage)
                            .DeleteLocalFiles();
                    break;
                default:
                    Rds.ExecuteNonQuery(
                        context: context,
                        statements: Rds.PhysicalDeleteBinaries(
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("SiteImage")));
                    break;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types DeleteTenantImage(Context context)
        {
            BinaryType = "TenantImage";
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    new Libraries.Images.ImageData(
                        ReferenceId,
                        Libraries.Images.ImageData.Types.TenantImage)
                            .DeleteLocalFiles();
                    break;
                default:
                    Rds.ExecuteNonQuery(
                        context: context,
                        statements: Rds.PhysicalDeleteBinaries(
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("TenantImage")));
                    break;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public BinaryModel(long referenceId)
        {
            ReferenceId = referenceId;
        }
    }
}
