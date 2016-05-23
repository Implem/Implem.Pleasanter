using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Styles;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.ViewParts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
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
                column: column ?? Rds.BinariesColumnDefault(),
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
                    case "Binaries_ReferenceId": if (!SiteSettings.AllColumn("ReferenceId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinaryId": if (!SiteSettings.AllColumn("BinaryId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinaryType": if (!SiteSettings.AllColumn("BinaryType").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Body": if (!SiteSettings.AllColumn("Body").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Bin": if (!SiteSettings.AllColumn("Bin").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Thumbnail": if (!SiteSettings.AllColumn("Thumbnail").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Icon": if (!SiteSettings.AllColumn("Icon").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_FileName": if (!SiteSettings.AllColumn("FileName").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Extension": if (!SiteSettings.AllColumn("Extension").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Size": if (!SiteSettings.AllColumn("Size").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinarySettings": if (!SiteSettings.AllColumn("BinarySettings").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                    case "Binaries_ReferenceId": if (!SiteSettings.AllColumn("ReferenceId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinaryId": if (!SiteSettings.AllColumn("BinaryId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinaryType": if (!SiteSettings.AllColumn("BinaryType").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Body": if (!SiteSettings.AllColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Bin": if (!SiteSettings.AllColumn("Bin").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Thumbnail": if (!SiteSettings.AllColumn("Thumbnail").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Icon": if (!SiteSettings.AllColumn("Icon").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_FileName": if (!SiteSettings.AllColumn("FileName").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Extension": if (!SiteSettings.AllColumn("Extension").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Size": if (!SiteSettings.AllColumn("Size").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_BinarySettings": if (!SiteSettings.AllColumn("BinarySettings").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Binaries_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value + " - " + Displays.Edit())
                .Html("#RecordInfo", Html.Builder().RecordInfo(baseModel: this, tableName: "Binaries"))
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
            var hb = Html.Builder();
            hb.Table(
                attributes: Html.Attributes().Class("grid"),
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
                                attributes: Html.Attributes()
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
                : Versions.VerType(BinaryId);
            SwitchTargets = BinariesUtility.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = BinariesUtility.GetSwitchTargets(SiteSettings);
            var binaryModel = new BinaryModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                binaryId: switchTargets.Previous(BinaryId),
                switchTargets: switchTargets);
            return RecordResponse(binaryModel);
        }

        public string Next()
        {
            var switchTargets = BinariesUtility.GetSwitchTargets(SiteSettings);
            var binaryModel = new BinaryModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                binaryId: switchTargets.Next(BinaryId),
                switchTargets: switchTargets);
            return RecordResponse(binaryModel);
        }

        public string Reload()
        {
            SwitchTargets = BinariesUtility.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            BinaryModel binaryModel, Message message = null, bool pushState = true)
        {
            binaryModel.MethodType = BaseModel.MethodTypes.Edit;
            return new BinariesResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    binaryModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? BinariesUtility.Editor(binaryModel)
                        : BinariesUtility.Editor(this))
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
                }
            }
        }

        private string Editor()
        {
            return new BinariesResponseCollection(this)
                .Html("#MainContainer", BinariesUtility.Editor(this))
                .ToJson();
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool ExistsSiteImage(ImageData.SizeTypes sizeType)
        {
            if (!PermissionType.CanRead())
            {
                return false;
            }
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new ImageData(ReferenceId, ImageData.Types.SiteImage).Exists(sizeType);
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
        public string SiteImagePrefix(ImageData.SizeTypes sizeType)
        {
            if (!PermissionType.CanRead())
            {
                return string.Empty;
            }
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new ImageData(ReferenceId, ImageData.Types.SiteImage).UrlPrefix(sizeType);
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
            return SiteImage(ImageData.SizeTypes.Thumbnail, Rds.BinariesColumn().Thumbnail());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public byte[] SiteImageIcon()
        {
            return SiteImage(ImageData.SizeTypes.Icon, Rds.BinariesColumn().Icon());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private byte[] SiteImage(ImageData.SizeTypes sizeType, SqlColumnCollection column)
        {
            if (!PermissionType.CanRead())
            {
                return null;
            }
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return new ImageData(ReferenceId, ImageData.Types.SiteImage).Read(sizeType);
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
            var imageData = new ImageData(
                Forms.File(ImageData.Types.SiteImage.ToString()),
                ReferenceId,
                ImageData.Types.SiteImage);
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local": imageData.WriteToLocal(); break;
                default:
                    Bin = imageData.ReSizeBytes(ImageData.SizeTypes.Regular);
                    Thumbnail = imageData.ReSizeBytes(ImageData.SizeTypes.Thumbnail);
                    Icon = imageData.ReSizeBytes(ImageData.SizeTypes.Icon);
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

    public class BinaryCollection : List<BinaryModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public BinaryCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null,
            bool get = true)
        {
            if (get)
            {
                Set(siteSettings, permissionType, Get(
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord,
                    aggregationCollection: aggregationCollection));
            }
        }

        public BinaryCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private BinaryCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new BinaryModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public BinaryCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            string commandText,
            SqlParamCollection param = null)
        {
            Set(siteSettings, permissionType, Get(commandText, param));
        }

        private DataTable Get(
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool history = false,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null)
        {
            var statements = new List<SqlStatement>
            {
                Rds.SelectBinaries(
                    dataTableName: "Main",
                    column: column ?? Rds.BinariesColumnDefault(),
                    join: join ??  Rds.BinariesJoinDefault(),
                    where: where ?? null,
                    orderBy: orderBy ?? null,
                    param: param ?? null,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord)
            };
            if (aggregationCollection != null)
            {
                statements.AddRange(Rds.BinariesAggregations(aggregationCollection, where));
            }
            var dataSet = Rds.ExecuteDataSet(
                transactional: false,
                statements: statements.ToArray());
            Aggregations.Set(dataSet, aggregationCollection);
            return dataSet.Tables["Main"];
        }

        private DataTable Get(string commandText, SqlParamCollection param = null)
        {
            return Rds.ExecuteTable(
                transactional: false,
                statements: Rds.BinariesStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class BinariesUtility
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = Html.Builder();
            var formData = DataViewFilters.SessionFormData();
            var binaryCollection = BinaryCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                referenceId: "Binaries",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: IndexScript(
                    binaryCollection: binaryCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                userScript: siteSettings.GridScript,
                action: () => hb
                    .Form(
                        attributes: Html.Attributes()
                            .Id_Css("BinariesForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "Binaries",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: binaryCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    binaryCollection: binaryCollection,
                                    siteSettings: siteSettings,
                                    permissionType: permissionType,
                                    formData: formData,
                                    dataViewName: dataViewName))
                            .MainCommands(
                                siteId: siteSettings.SiteId,
                                permissionType: permissionType,
                                verType: Versions.VerTypes.Latest,
                                backUrl: Navigations.Index("Admins"),
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Binaries")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                    .Dialog_Move("items", siteSettings.SiteId, bulk: true)
                    .Div(attributes: Html.Attributes()
                        .Id_Css("Dialog_ExportSettings", "dialog")
                        .Title(Displays.ExportSettings()))).ToString();
        }

        private static BinaryCollection BinaryCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new BinaryCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Binaries",
                    formData: formData,
                    where: Rds.BinariesWhere()),
                orderBy: GridSorters.Get(
                    formData, Rds.BinariesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static string IndexScript(
            BinaryCollection binaryCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            return string.Empty;
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            BinaryCollection binaryCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    binaryCollection: binaryCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        public static string DataView(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            switch (DataViewSelectors.Get(siteSettings.SiteId))
            {
                default: return Grid(siteSettings: siteSettings, permissionType: permissionType);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            BinaryCollection binaryCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: Html.Attributes()
                        .Id_Css("Grid", "grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            binaryCollection: binaryCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == binaryCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var binaryCollection = BinaryCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", Html.Builder().Grid(
                    siteSettings: siteSettings,
                    binaryCollection: binaryCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: binaryCollection.Aggregations,
                    container: false))
                .WindowScrollTop().ToJson();
        }

        public static string GridRows(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ResponseCollection responseCollection = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var formData = DataViewFilters.SessionFormData();
            var binaryCollection = BinaryCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", Html.Builder().GridRows(
                    siteSettings: siteSettings,
                    binaryCollection: binaryCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: binaryCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, binaryCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            BinaryCollection binaryCollection,
            FormData formData,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            if (addHeader)
            {
                hb.GridHeader(
                    columnCollection: siteSettings.GridColumnCollection(), 
                    formData: formData,
                    checkAll: checkAll);
            }
            binaryCollection.ForEach(binaryModel => hb
                .Tr(
                    attributes: Html.Attributes()
                        .Class("grid-row")
                        .DataId(binaryModel.BinaryId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: binaryModel.BinaryId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    binaryModel: binaryModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.BinariesColumn()
                .BinaryId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "ReferenceId": select.ReferenceId(); break;
                    case "BinaryId": select.BinaryId(); break;
                    case "Ver": select.Ver(); break;
                    case "BinaryType": select.BinaryType(); break;
                    case "Title": select.Title(); break;
                    case "Body": select.Body(); break;
                    case "Bin": select.Bin(); break;
                    case "Thumbnail": select.Thumbnail(); break;
                    case "Icon": select.Icon(); break;
                    case "FileName": select.FileName(); break;
                    case "Extension": select.Extension(); break;
                    case "Size": select.Size(); break;
                    case "BinarySettings": select.BinarySettings(); break;
                    case "Comments": select.Comments(); break;
                    case "Creator": select.Creator(); break;
                    case "Updator": select.Updator(); break;
                    case "CreatedTime": select.CreatedTime(); break;
                    case "UpdatedTime": select.UpdatedTime(); break;
                }
            });
            return select;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, BinaryModel binaryModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: binaryModel.Ver);
                case "Comments": return hb.Td(column: column, value: binaryModel.Comments);
                case "Creator": return hb.Td(column: column, value: binaryModel.Creator);
                case "Updator": return hb.Td(column: column, value: binaryModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: binaryModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: binaryModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(new BinaryModel(
                SiteSettingsUtility.BinariesSiteSettings(),
                Permissions.Admins(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long binaryId, bool clearSessions)
        {
            var binaryModel = new BinaryModel(
                SiteSettingsUtility.BinariesSiteSettings(),
                Permissions.Admins(),
                binaryId: binaryId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            binaryModel.SwitchTargets = BinariesUtility.GetSwitchTargets(
                SiteSettingsUtility.BinariesSiteSettings());
            return Editor(binaryModel);
        }

        public static string Editor(BinaryModel binaryModel)
        {
            var hb = Html.Builder();
            var permissionType = Permissions.Admins();
            var siteSettings = SiteSettingsUtility.BinariesSiteSettings();
            return hb.Template(
                siteId: 0,
                referenceId: "Binaries",
                title: binaryModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Binaries() + " - " + Displays.New()
                    : binaryModel.Title.Value,
                permissionType: permissionType,
                verType: binaryModel.VerType,
                methodType: binaryModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    binaryModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager;
                    hb
                        .Editor(
                            binaryModel: binaryModel,
                            permissionType: permissionType,
                            siteSettings: siteSettings)
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
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: Html.Attributes()
                        .Id_Css("BinaryForm", "main-form")
                        .Action(binaryModel.BinaryId != 0
                            ? Navigations.Action("Binaries", binaryModel.BinaryId)
                            : Navigations.Action("Binaries")),
                    action: () => hb
                        .RecordHeader(
                            id: binaryModel.BinaryId,
                            baseModel: binaryModel,
                            tableName: "Binaries",
                            switchTargets: binaryModel.SwitchTargets?
                                .Select(o => o.ToLong()).ToList())
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: binaryModel.Comments,
                                verType: binaryModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(binaryModel: binaryModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                binaryModel: binaryModel)
                            .FieldSet(attributes: Html.Attributes()
                                .Id("FieldSetHistories")
                                .DataAction("Histories")
                                .DataMethod("get"))
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: binaryModel.VerType,
                                backUrl: Navigations.Index("Binaries"),
                                referenceType: "Binaries",
                                referenceId: binaryModel.BinaryId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        binaryModel: binaryModel,
                                        siteSettings: siteSettings)))
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
                            value: binaryModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("Binaries", binaryModel.BinaryId, binaryModel.Ver)
                .Dialog_Copy("Binaries", binaryModel.BinaryId)
                .Dialog_OutgoingMail()
                .EditorExtensions(binaryModel: binaryModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, BinaryModel binaryModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(action: () => hb
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
                siteSettings.ColumnCollection
                    .Where(o => o.EditorVisible.ToBool())
                    .OrderBy(o => siteSettings.EditorColumnsOrder.IndexOf(o.ColumnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "ReferenceId": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.ReferenceId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "BinaryId": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.BinaryId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Ver": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Ver.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "BinaryType": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.BinaryType.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Title": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Title.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Body": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Body.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "FileName": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.FileName.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Extension": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Extension.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Size": hb.Field(siteSettings, column, binaryModel.MethodType, binaryModel.Size.ToControl(column), column.ColumnPermissionType(permissionType)); break;
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
    }
}
