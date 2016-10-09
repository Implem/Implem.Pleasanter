using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
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
    public class BinaryModel : BaseModel
    {
        public long ReferenceId = 0;
        public long BinaryId = 0;
        public string BinaryType = string.Empty;
        public Title Title = new Title();
        public string Body = string.Empty;
        public byte[] Bin = null;
        public byte[] Thumbnail = null;
        public byte[] Icon = null;
        public string FileName = string.Empty;
        public string Extension = string.Empty;
        public int Size = 0;
        public BinarySettings BinarySettings = new BinarySettings();
        public long SavedReferenceId = 0;
        public long SavedBinaryId = 0;
        public string SavedBinaryType = string.Empty;
        public string SavedTitle = string.Empty;
        public string SavedBody = string.Empty;
        public byte[] SavedBin = null;
        public byte[] SavedThumbnail = null;
        public byte[] SavedIcon = null;
        public string SavedFileName = string.Empty;
        public string SavedExtension = string.Empty;
        public int SavedSize = 0;
        public string SavedBinarySettings = string.Empty;
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool BinaryId_Updated { get { return BinaryId != SavedBinaryId; } }
        public bool BinaryType_Updated { get { return BinaryType != SavedBinaryType && BinaryType != null; } }
        public bool Title_Updated { get { return Title.Value != SavedTitle && Title.Value != null; } }
        public bool Body_Updated { get { return Body != SavedBody && Body != null; } }
        public bool Bin_Updated { get { return Bin != SavedBin && Bin != null; } }
        public bool Thumbnail_Updated { get { return Thumbnail != SavedThumbnail && Thumbnail != null; } }
        public bool Icon_Updated { get { return Icon != SavedIcon && Icon != null; } }
        public bool FileName_Updated { get { return FileName != SavedFileName && FileName != null; } }
        public bool Extension_Updated { get { return Extension != SavedExtension && Extension != null; } }
        public bool Size_Updated { get { return Size != SavedSize; } }
        public bool BinarySettings_Updated { get { return BinarySettings.ToJson() != SavedBinarySettings && BinarySettings.ToJson() != null; } }

        public BinarySettings Session_BinarySettings()
        {
            return this.PageSession("BinarySettings") != null
                ? this.PageSession("BinarySettings")?.ToString().Deserialize<BinarySettings>() ?? new BinarySettings()
                : BinarySettings;
        }

        public void  Session_BinarySettings(object value)
        {
            this.PageSession("BinarySettings", value);
        }

        public List<long> SwitchTargets;

        public BinaryModel()
        {
        }

        public BinaryModel(
            SiteSettings siteSettings, 
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public BinaryModel(
            SiteSettings siteSettings, 
            long binaryId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            BinaryId = binaryId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public BinaryModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            Set(dataRow);
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
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public Error.Types Create(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            Size = Bin.Length;
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertBinaries(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.BinariesParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            BinaryId = newId != 0 ? newId : BinaryId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Update(SqlParamCollection param = null, bool paramAll = false)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateBinaries(
                        verUp: VerUp,
                        where: Rds.BinariesWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: param ?? Rds.BinariesParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return Error.Types.UpdateConflicts;
            Get();
            return Error.Types.None;
        }

        public Error.Types UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertBinaries(
                        selectIdentity: true,
                        where: where ?? Rds.BinariesWhereDefault(this),
                        param: param ?? Rds.BinariesParamDefault(this, setDefault: true))
                });
            BinaryId = newId != 0 ? newId : BinaryId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteBinaries(
                        where: Rds.BinariesWhere().BinaryId(BinaryId))
                });
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

        public Error.Types PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteBinaries(
                    tableType: tableType,
                    param: Rds.BinariesParam().BinaryId(BinaryId)));
            return Error.Types.None;
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Binaries_ReferenceId": ReferenceId = Forms.Data(controlId).ToLong(); break;
                    case "Binaries_BinaryType": BinaryType = Forms.Data(controlId).ToString(); break;
                    case "Binaries_Title": Title = new Title(BinaryId, Forms.Data(controlId)); break;
                    case "Binaries_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Binaries_FileName": FileName = Forms.Data(controlId).ToString(); break;
                    case "Binaries_Extension": Extension = Forms.Data(controlId).ToString(); break;
                    case "Binaries_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments = Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default: break;
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
                    case "Binaries_Bin": Bin = Forms.File(controlId); break;
                    case "Binaries_Thumbnail": Thumbnail = Forms.File(controlId); break;
                    case "Binaries_Icon": Icon = Forms.File(controlId); break;
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

        private void Set(DataRow dataRow)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var name = dataColumn.ColumnName;
                switch(name)
                {
                    case "ReferenceId": if (dataRow[name] != DBNull.Value) { ReferenceId = dataRow[name].ToLong(); SavedReferenceId = ReferenceId; } break;
                    case "BinaryId": if (dataRow[name] != DBNull.Value) { BinaryId = dataRow[name].ToLong(); SavedBinaryId = BinaryId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "BinaryType": BinaryType = dataRow[name].ToString(); SavedBinaryType = BinaryType; break;
                    case "Title": Title = new Title(dataRow, "BinaryId"); SavedTitle = Title.Value; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "Bin": Bin = dataRow.Bytes("Bin"); SavedBin = Bin; break;
                    case "Thumbnail": Thumbnail = dataRow.Bytes("Bin"); SavedThumbnail = Thumbnail; break;
                    case "Icon": Icon = dataRow.Bytes("Bin"); SavedIcon = Icon; break;
                    case "FileName": FileName = dataRow[name].ToString(); SavedFileName = FileName; break;
                    case "Extension": Extension = dataRow[name].ToString(); SavedExtension = Extension; break;
                    case "Size": Size = dataRow[name].ToInt(); SavedSize = Size; break;
                    case "BinarySettings": BinarySettings = dataRow.String("BinarySettings").Deserialize<BinarySettings>() ?? new BinarySettings(); SavedBinarySettings = BinarySettings.ToJson(); break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
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
                            column: Rds.BinariesColumn().UpdatedTime(),
                            where: Rds.BinariesWhere().ReferenceId(ReferenceId)))
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
                            where: Rds.BinariesWhere().ReferenceId(ReferenceId)));
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
                            where: Rds.BinariesWhere().ReferenceId(ReferenceId),
                            param: Rds.BinariesParamDefault(this, setDefault: true)));
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
