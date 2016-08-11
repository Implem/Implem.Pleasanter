using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
            Permissions.Types permissionType,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            PermissionType = permissionType;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public BinaryModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            long binaryId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            BinaryId = binaryId;
            PermissionType = permissionType;
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

        public string Create(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            var error = ValidateBeforeCreate();
            if (error != null) return error;
            OnCreating();
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
            OnCreated();
            return RecordResponse(this, Messages.Created(Title.ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnCreating()
        {
            Size = Bin.Length;
        }

        private void OnCreated()
        {
        }

        private string ValidateBeforeCreate()
        {
            if (!PermissionType.CanCreate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Binaries_ReferenceId": if (!SiteSettings.GetColumn("ReferenceId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinaryId": if (!SiteSettings.GetColumn("BinaryId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Ver": if (!SiteSettings.GetColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinaryType": if (!SiteSettings.GetColumn("BinaryType").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Title": if (!SiteSettings.GetColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Body": if (!SiteSettings.GetColumn("Body").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Bin": if (!SiteSettings.GetColumn("Bin").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Thumbnail": if (!SiteSettings.GetColumn("Thumbnail").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Icon": if (!SiteSettings.GetColumn("Icon").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_FileName": if (!SiteSettings.GetColumn("FileName").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Extension": if (!SiteSettings.GetColumn("Extension").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Size": if (!SiteSettings.GetColumn("Size").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinarySettings": if (!SiteSettings.GetColumn("BinarySettings").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Comments": if (!SiteSettings.GetColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Creator": if (!SiteSettings.GetColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Updator": if (!SiteSettings.GetColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_VerUp": if (!SiteSettings.GetColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        public string Update(SqlParamCollection param = null, bool paramAll = false)
        {
            var error = ValidateBeforeUpdate();
            if (error != null) return error;
            SetBySession();
            OnUpdating(ref param);
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateBinaries(
                        verUp: VerUp,
                        where: Rds.BinariesWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.BinariesParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new BinariesResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        private void OnUpdated(ref BinariesResponseCollection responseCollection)
        {
        }

        private string ValidateBeforeUpdate()
        {
            if (!PermissionType.CanUpdate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Binaries_ReferenceId": if (!SiteSettings.GetColumn("ReferenceId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinaryId": if (!SiteSettings.GetColumn("BinaryId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Ver": if (!SiteSettings.GetColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinaryType": if (!SiteSettings.GetColumn("BinaryType").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Title": if (!SiteSettings.GetColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Body": if (!SiteSettings.GetColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Bin": if (!SiteSettings.GetColumn("Bin").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Thumbnail": if (!SiteSettings.GetColumn("Thumbnail").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Icon": if (!SiteSettings.GetColumn("Icon").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_FileName": if (!SiteSettings.GetColumn("FileName").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Extension": if (!SiteSettings.GetColumn("Extension").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Size": if (!SiteSettings.GetColumn("Size").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinarySettings": if (!SiteSettings.GetColumn("BinarySettings").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Comments": if (!SiteSettings.GetColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Creator": if (!SiteSettings.GetColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Updator": if (!SiteSettings.GetColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_VerUp": if (!SiteSettings.GetColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(BinariesResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FormResponse(this)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "Binaries"))
                .Message(Messages.Updated(Title.ToString()))
                .RemoveComment(DeleteCommentId, _using: DeleteCommentId != 0)
                .ClearFormData();
        }

        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            if (!PermissionType.CanUpdate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SetBySession();
            OnUpdatingOrCreating(ref where, ref param);
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
            var responseCollection = new BinariesResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref BinariesResponseCollection responseCollection)
        {
        }

        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteBinaries(
                        where: Rds.BinariesWhere().BinaryId(BinaryId))
                });
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
            var responseCollection = new BinariesResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.Index("Binaries"));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref BinariesResponseCollection responseCollection)
        {
        }

        public string Restore(long binaryId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            BinaryId = binaryId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreBinaries(
                        where: Rds.BinariesWhere().BinaryId(BinaryId))
                });
            return new ResponseCollection().ToJson();
        }

        public string PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            if (!PermissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnPhysicalDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteBinaries(
                    tableType: tableType,
                    param: Rds.BinariesParam().BinaryId(BinaryId)));
            var responseCollection = new BinariesResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref BinariesResponseCollection responseCollection)
        {
        }

        public string Histories()
        {
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () =>
                {
                    hb.GridHeader(
                        columnCollection: SiteSettings.HistoryColumnCollection(),
                        sort: false,
                        checkRow: false);
                    new BinaryCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.BinariesWhere().BinaryId(BinaryId),
                        orderBy: Rds.BinariesOrderBy().Ver(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(binaryModel => hb
                            .Tr(
                                attributes: new HtmlAttributes()
                                    .Class("grid-row history not-link")
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-ver", binaryModel.Ver)
                                    .Add("data-latest", 1, _using: binaryModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryColumnCollection().ForEach(column =>
                                        hb.TdValue(column, binaryModel))));
                });
            return new BinariesResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.BinariesWhere()
                    .BinaryId(BinaryId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            SwitchTargets = BinaryUtilities.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = BinaryUtilities.GetSwitchTargets(SiteSettings);
            var binaryModel = new BinaryModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                binaryId: switchTargets.Previous(BinaryId),
                switchTargets: switchTargets);
            return RecordResponse(binaryModel);
        }

        public string Next()
        {
            var switchTargets = BinaryUtilities.GetSwitchTargets(SiteSettings);
            var binaryModel = new BinaryModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                binaryId: switchTargets.Next(BinaryId),
                switchTargets: switchTargets);
            return RecordResponse(binaryModel);
        }

        public string Reload()
        {
            SwitchTargets = BinaryUtilities.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            BinaryModel binaryModel, Message message = null, bool pushState = true)
        {
            binaryModel.MethodType = BaseModel.MethodTypes.Edit;
            return new BinariesResponseCollection(this)
                .Invoke("clearDialogs")
                .ReplaceAll(
                    "#MainContainer",
                    binaryModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? BinaryUtilities.Editor(binaryModel, byRest: true)
                        : BinaryUtilities.Editor(this, byRest: true))
                .Invoke("validateBinaries")
                .Message(message)
                .PushState(
                    "Edit",
                    Navigations.Edit("Binaries", binaryModel.BinaryId),
                    _using: pushState)
                .ClearFormData()
                .ToJson();
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
                DeleteCommentId = Forms.Data("ControlId").Split(',')._2nd().ToInt();
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

        private string Editor()
        {
            return new BinariesResponseCollection(this)
                .ReplaceAll(
                    "#MainContainer",
                    BinaryUtilities.Editor(this, byRest: true))
                .Invoke("validateBinaries")
                .ToJson();
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName()).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool ExistsSiteImage(Libraries.Images.ImageData.SizeTypes sizeType)
        {
            if (!PermissionType.CanRead())
            {
                return false;
            }
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new Libraries.Images.ImageData(
                        ReferenceId, Libraries.Images.ImageData.Types.SiteImage)
                            .Exists(sizeType);
                default:
                    return Rds.ExecuteScalar_int(statements:
                        Rds.SelectBinaries(
                            column: Rds.BinariesColumn().BinariesCount(),
                            where: Rds.BinariesWhere().ReferenceId(ReferenceId))) == 1;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SiteImagePrefix(Libraries.Images.ImageData.SizeTypes sizeType)
        {
            if (!PermissionType.CanRead())
            {
                return string.Empty;
            }
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
        public byte[] SiteImageThumbnail()
        {
            return SiteImage(
                Libraries.Images.ImageData.SizeTypes.Thumbnail, Rds.BinariesColumn()
                    .Thumbnail());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public byte[] SiteImageIcon()
        {
            return SiteImage(
                Libraries.Images.ImageData.SizeTypes.Icon, Rds.BinariesColumn()
                    .Icon());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private byte[] SiteImage(
            Libraries.Images.ImageData.SizeTypes sizeType, SqlColumnCollection column)
        {
            if (!PermissionType.CanRead())
            {
                return null;
            }
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
        public string UpdateSiteImage()
        {
            if (!PermissionType.CanEditSite())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            BinaryType = "SiteImage";
            var imageData = new Libraries.Images.ImageData(
                Forms.File(Libraries.Images.ImageData.Types.SiteImage.ToString()),
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
            return Messages.ResponseFileUpdateCompleted().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public BinaryModel(SiteModel siteModel)
        {
            SiteSettings = siteModel.SiteSettings;
            PermissionType = siteModel.PermissionType;
            ReferenceId = siteModel.SiteId;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public BinaryModel(Permissions.Types permissionType, long referenceId)
        {
            PermissionType = permissionType;
            ReferenceId = referenceId;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public BinaryModel(
            long imageId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            BinaryId = imageId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public byte[] Show()
        {
            return Bin;
        }
    }
}
