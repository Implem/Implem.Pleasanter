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
        public string FileName = string.Empty;
        public string Extension = string.Empty;
        public int Size = 0;
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
        [NonSerialized] public string SavedFileName = string.Empty;
        [NonSerialized] public string SavedExtension = string.Empty;
        [NonSerialized] public int SavedSize = 0;
        [NonSerialized] public string SavedContentType = string.Empty;
        [NonSerialized] public string SavedBinarySettings = "[]";

        public bool BinaryId_Updated(Column column = null)
        {
            return BinaryId != SavedBinaryId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != BinaryId);
        }

        public bool TenantId_Updated(Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != TenantId);
        }

        public bool ReferenceId_Updated(Column column = null)
        {
            return ReferenceId != SavedReferenceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != ReferenceId);
        }

        public bool Guid_Updated(Column column = null)
        {
            return Guid != SavedGuid && Guid != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Guid);
        }

        public bool BinaryType_Updated(Column column = null)
        {
            return BinaryType != SavedBinaryType && BinaryType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != BinaryType);
        }

        public bool Title_Updated(Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Title.Value);
        }

        public bool Body_Updated(Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Body);
        }

        public bool FileName_Updated(Column column = null)
        {
            return FileName != SavedFileName && FileName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != FileName);
        }

        public bool Extension_Updated(Column column = null)
        {
            return Extension != SavedExtension && Extension != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Extension);
        }

        public bool Size_Updated(Column column = null)
        {
            return Size != SavedSize &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != Size);
        }

        public bool ContentType_Updated(Column column = null)
        {
            return ContentType != SavedContentType && ContentType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ContentType);
        }

        public bool BinarySettings_Updated(Column column = null)
        {
            return BinarySettings.ToJson() != SavedBinarySettings && BinarySettings.ToJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != BinarySettings.ToJson());
        }

        public bool Bin_Updated(Column column = null)
        {
            return Bin != SavedBin && Bin != null;
        }

        public bool Thumbnail_Updated(Column column = null)
        {
            return Thumbnail != SavedThumbnail && Thumbnail != null;
        }

        public bool Icon_Updated(Column column = null)
        {
            return Icon != SavedIcon && Icon != null;
        }

        public BinarySettings Session_BinarySettings()
        {
            return this.PageSession("BinarySettings") != null
                ? this.PageSession("BinarySettings")?.ToString().Deserialize<BinarySettings>() ?? new BinarySettings()
                : BinarySettings;
        }

        public void Session_BinarySettings(object value)
        {
            this.PageSession("BinarySettings", value);
        }

        public BinaryModel()
        {
        }

        public BinaryModel(
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public BinaryModel(
            long binaryId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            BinaryId = binaryId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public BinaryModel(DataRow dataRow, string tableAlias = null)
        {
            OnConstructing();
            Set(dataRow, tableAlias);
            OnConstructed();
        }

        private void OnConstructing()
        {
        }

        private void OnConstructed()
        {
        }

        public void ClearSessions()
        {
            Session_BinarySettings(null);
        }

        public BinaryModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectBinaries(
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
            RdsUser rdsUser = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            Size = Bin.Length;
            var statements = new List<SqlStatement>();
            CreateStatements(statements, tableType, param, otherInitValue);
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            BinaryId = newId != 0 ? newId : BinaryId;
            if (get) Get();
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertBinaries(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.BinariesParamDefault(
                        this, setDefault: true, otherInitValue: otherInitValue))
            });
            return statements;
        }

        public Error.Types Update(
            RdsUser rdsUser = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool get = true)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(statements, timestamp, param, otherInitValue, additionalStatements);
            var count = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            if (count == 0) return Error.Types.UpdateConflicts;
            if (get) Get();
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
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
                    param: param ?? Rds.BinariesParamDefault(this, otherInitValue: otherInitValue),
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
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            if (!Guid.InitialValue())
            {
                column.Guid(function: Sqls.Functions.SingleColumn);
                param.Guid();
            }
            if (!BinaryType.InitialValue())
            {
                column.BinaryType(function: Sqls.Functions.SingleColumn);
                param.BinaryType();
            }
            if (!Title.InitialValue())
            {
                column.Title(function: Sqls.Functions.SingleColumn);
                param.Title();
            }
            if (!Body.InitialValue())
            {
                column.Body(function: Sqls.Functions.SingleColumn);
                param.Body();
            }
            if (!Bin.InitialValue())
            {
                column.Bin(function: Sqls.Functions.SingleColumn);
                param.Bin();
            }
            if (!Thumbnail.InitialValue())
            {
                column.Thumbnail(function: Sqls.Functions.SingleColumn);
                param.Thumbnail();
            }
            if (!Icon.InitialValue())
            {
                column.Icon(function: Sqls.Functions.SingleColumn);
                param.Icon();
            }
            if (!FileName.InitialValue())
            {
                column.FileName(function: Sqls.Functions.SingleColumn);
                param.FileName();
            }
            if (!Extension.InitialValue())
            {
                column.Extension(function: Sqls.Functions.SingleColumn);
                param.Extension();
            }
            if (!Size.InitialValue())
            {
                column.Size(function: Sqls.Functions.SingleColumn);
                param.Size();
            }
            if (!ContentType.InitialValue())
            {
                column.ContentType(function: Sqls.Functions.SingleColumn);
                param.ContentType();
            }
            if (!BinarySettings.InitialValue())
            {
                column.BinarySettings(function: Sqls.Functions.SingleColumn);
                param.BinarySettings();
            }
            if (!Comments.InitialValue())
            {
                column.Comments(function: Sqls.Functions.SingleColumn);
                param.Comments();
            }
            return Rds.InsertBinaries(
                tableType: tableType,
                param: param,
                select: Rds.SelectBinaries(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types UpdateOrCreate(
            RdsUser rdsUser = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertBinaries(
                    selectIdentity: true,
                    where: where ?? Rds.BinariesWhereDefault(this),
                    param: param ?? Rds.BinariesParamDefault(this, setDefault: true))
            };
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            BinaryId = newId != 0 ? newId : BinaryId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            var statements = new List<SqlStatement>();
            var where = Rds.BinariesWhere().BinaryId(BinaryId);
            statements.AddRange(new List<SqlStatement>
            {
                CopyToStatement(where, Sqls.TableTypes.Deleted),
                Rds.PhysicalDeleteBinaries(where: where)
            });
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(long binaryId)
        {
            BinaryId = binaryId;
            Rds.ExecuteNonQuery(
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
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteBinaries(
                    tableType: tableType,
                    param: Rds.BinariesParam().BinaryId(BinaryId)));
            return Error.Types.None;
        }

        public void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Binaries_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                controlId.Substring("Comment".Length).ToInt(),
                                Forms.Data(controlId));
                        }
                        break;
                }
            });
            if (Routes.Action() == "deletecomment")
            {
                DeleteCommentId = Forms.ControlId().Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
            Forms.FileKeys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    default: break;
                }
            });
        }

        private void SetBySession()
        {
            if (!Forms.HasData("Binaries_BinarySettings")) BinarySettings = Session_BinarySettings();
        }

        private void Set(DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(DataRow dataRow, string tableAlias = null)
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
                        case "FileName":
                            FileName = dataRow[column.ColumnName].ToString();
                            SavedFileName = FileName;
                            break;
                        case "Extension":
                            Extension = dataRow[column.ColumnName].ToString();
                            SavedExtension = Extension;
                            break;
                        case "Size":
                            Size = dataRow[column.ColumnName].ToInt();
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
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated()
        {
            return
                BinaryId_Updated() ||
                TenantId_Updated() ||
                ReferenceId_Updated() ||
                Guid_Updated() ||
                Ver_Updated() ||
                BinaryType_Updated() ||
                Title_Updated() ||
                Body_Updated() ||
                FileName_Updated() ||
                Extension_Updated() ||
                Size_Updated() ||
                ContentType_Updated() ||
                BinarySettings_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SiteImagePrefix(Libraries.Images.ImageData.SizeTypes sizeType)
        {
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        ReferenceId, Libraries.Images.ImageData.Types.SiteImage)
                            .UrlPrefix(sizeType);
                default:
                    return Rds.ExecuteScalar_datetime(statements:
                        Rds.SelectBinaries(
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
        public byte[] SiteImage(
            Libraries.Images.ImageData.SizeTypes sizeType, SqlColumnCollection column)
        {
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        ReferenceId, Libraries.Images.ImageData.Types.SiteImage)
                            .Read(sizeType);
                default:
                    return Rds.ExecuteScalar_bytes(statements:
                        Rds.SelectBinaries(
                            column: column,
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("SiteImage")));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types UpdateSiteImage(byte[] data)
        {
            BinaryType = "SiteImage";
            var imageData = new Libraries.Images.ImageData(
                data,
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
                    Rds.ExecuteNonQuery(transactional: true, statements:
                        Rds.UpdateOrInsertBinaries(
                            selectIdentity: true,
                            where: Rds.BinariesWhere()
                                .ReferenceId(ReferenceId)
                                .BinaryType("SiteImage"),
                            param: Rds.BinariesParamDefault(this, setDefault: true)));
                    break;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types DeleteSiteImage()
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
                    Rds.ExecuteNonQuery(statements:
                        Rds.PhysicalDeleteBinaries(
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
        public BinaryModel(long referenceId)
        {
            ReferenceId = referenceId;
        }
    }
}
