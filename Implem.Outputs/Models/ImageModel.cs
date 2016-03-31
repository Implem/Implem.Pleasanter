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
using Implem.Pleasanter.Libraries.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Models
{
    public class ImageModel : BaseModel
    {
        public long ImageId = 0;
        public Title Title = new Title();
        public string Body = string.Empty;
        public byte[] Bin = null;
        public string FileName = string.Empty;
        public string Extension = string.Empty;
        public int Size = 0;
        public ImageSettings ImageSettings = new ImageSettings();
        public long SavedImageId = 0;
        public string SavedTitle = string.Empty;
        public string SavedBody = string.Empty;
        public byte[] SavedBin = null;
        public string SavedFileName = string.Empty;
        public string SavedExtension = string.Empty;
        public int SavedSize = 0;
        public string SavedImageSettings = string.Empty;
        public bool ImageId_Updated { get { return ImageId != SavedImageId; } }
        public bool Title_Updated { get { return Title.Value != SavedTitle && Title.Value != null; } }
        public bool Body_Updated { get { return Body != SavedBody && Body != null; } }
        public bool Bin_Updated { get { return Bin != SavedBin && Bin != null; } }
        public bool FileName_Updated { get { return FileName != SavedFileName && FileName != null; } }
        public bool Extension_Updated { get { return Extension != SavedExtension && Extension != null; } }
        public bool Size_Updated { get { return Size != SavedSize; } }
        public bool ImageSettings_Updated { get { return ImageSettings.ToJson() != SavedImageSettings && ImageSettings.ToJson() != null; } }

        public ImageSettings Session_ImageSettings()
        {
            return this.PageSession("ImageSettings") != null
                ? this.PageSession("ImageSettings")?.ToString().Deserialize<ImageSettings>() ?? new ImageSettings()
                : ImageSettings;
        }

        public void  Session_ImageSettings(object value)
        {
            this.PageSession("ImageSettings", value);
        }

        public List<long> SwitchTargets;

        public ImageModel()
        {
        }

        public ImageModel(
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

        public ImageModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            long imageId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            ImageId = imageId;
            PermissionType = permissionType;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public ImageModel(
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
            Session_ImageSettings(null);
        }

        public ImageModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectImages(
                tableType: tableType,
                column: column ?? Rds.ImagesColumnDefault(),
                join: join ??  Rds.ImagesJoinDefault(),
                where: where ?? Rds.ImagesWhereDefault(this),
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
                    Rds.InsertImages(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.ImagesParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            ImageId = newId != 0 ? newId : ImageId;
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
                    case "Images_ImageId": if (!SiteSettings.AllColumn("ImageId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Body": if (!SiteSettings.AllColumn("Body").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Bin": if (!SiteSettings.AllColumn("Bin").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_FileName": if (!SiteSettings.AllColumn("FileName").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Extension": if (!SiteSettings.AllColumn("Extension").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Size": if (!SiteSettings.AllColumn("Size").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_ImageSettings": if (!SiteSettings.AllColumn("ImageSettings").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                    Rds.UpdateImages(
                        verUp: VerUp,
                        where: Rds.ImagesWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.ImagesParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new ImagesResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        private void OnUpdated(ref ImagesResponseCollection responseCollection)
        {
        }

        public string DeleteComment()
        {
            var error = ValidateBeforeUpdate();
            if (error != null) return error;
            var commentId = Forms.Data("ControlId").Split(',')._2nd();
            Comments.RemoveAll(o => o.CommentId.ToString() == commentId);
            Update();
            return ResponseByUpdate(new ImagesResponseCollection(this))
                .RemoveComment(commentId)
                .ToJson();
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
                    case "Images_ImageId": if (!SiteSettings.AllColumn("ImageId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Body": if (!SiteSettings.AllColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Bin": if (!SiteSettings.AllColumn("Bin").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_FileName": if (!SiteSettings.AllColumn("FileName").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Extension": if (!SiteSettings.AllColumn("Extension").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Size": if (!SiteSettings.AllColumn("Size").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_ImageSettings": if (!SiteSettings.AllColumn("ImageSettings").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Images_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(ImagesResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.DisplayValue + " - " + Displays.Edit())
                .Html("#RecordInfo", Html.Builder().RecordInfo(baseModel: this, tableName: "Images"))
                .Html("#RecordHistories", Html.Builder().RecordHistories(ver: Ver, verType: VerType))
                .Message(Messages.Updated(Title.ToString()))
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
                    Rds.UpdateOrInsertImages(
                        selectIdentity: true,
                        where: where ?? Rds.ImagesWhereDefault(this),
                        param: param ?? Rds.ImagesParamDefault(this, setDefault: true))
                });
            ImageId = newId != 0 ? newId : ImageId;
            Get();
            var responseCollection = new ImagesResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref ImagesResponseCollection responseCollection)
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
                    Rds.DeleteImages(
                        where: Rds.ImagesWhere().ImageId(ImageId))
                });
            var responseCollection = new ImagesResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.Index("Images"));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref ImagesResponseCollection responseCollection)
        {
        }

        public string Restore(long imageId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            ImageId = imageId;
            Rds.ExecuteNonQuery(
                connectionString: Def.Db.DbOwner,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreImages(
                        where: Rds.ImagesWhere().ImageId(ImageId))
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
                statements: Rds.PhysicalDeleteImages(
                    tableType: tableType,
                    param: Rds.ImagesParam().ImageId(ImageId)));
            var responseCollection = new ImagesResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref ImagesResponseCollection responseCollection)
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
                        columnCollection: SiteSettings.HistoryGridColumnCollection(),
                        sort: false,
                        checkRow: false);
                    new ImageCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.ImagesWhere().ImageId(ImageId),
                        orderBy: Rds.ImagesOrderBy().UpdatedTime(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(imageModel => hb
                            .Tr(
                                attributes: Html.Attributes()
                                    .Class("grid-row not-link")
                                    .OnClick(Def.JavaScript.HistoryAndCloseDialog
                                        .Params(imageModel.Ver))
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-latest", 1, _using: imageModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryGridColumnCollection().ForEach(column =>
                                        hb.TdValue(column, imageModel))));
                });
            return new ImagesResponseCollection(this).Html("#HistoriesForm", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.ImagesWhere()
                    .ImageId(ImageId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerType(ImageId);
            SwitchTargets = ImagesUtility.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string PreviousHistory()
        {
            Get(
                where: Rds.ImagesWhere()
                    .ImageId(ImageId)
                    .Ver(Forms.Int("Ver"), _operator: "<"),
                orderBy: Rds.ImagesOrderBy()
                    .Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = ImagesUtility.GetSwitchTargets(SiteSettings);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(ImageId, Versions.DirectioTypes.Previous);
                    return Editor();
                default:
                    return new ImagesResponseCollection(this).ToJson();
            }
        }

        public string NextHistory()
        {
            Get(
                where: Rds.ImagesWhere()
                    .ImageId(ImageId)
                    .Ver(Forms.Int("Ver"), _operator: ">"),
                orderBy: Rds.ImagesOrderBy()
                    .Ver(SqlOrderBy.Types.asc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = ImagesUtility.GetSwitchTargets(SiteSettings);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(ImageId, Versions.DirectioTypes.Next);
                    return Editor();
                default:
                    return new ImagesResponseCollection(this).ToJson();
            }
        }

        public string Previous()
        {
            var switchTargets = ImagesUtility.GetSwitchTargets(SiteSettings);
            var imageModel = new ImageModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                imageId: switchTargets.Previous(ImageId),
                switchTargets: switchTargets);
            return RecordResponse(imageModel);
        }

        public string Next()
        {
            var switchTargets = ImagesUtility.GetSwitchTargets(SiteSettings);
            var imageModel = new ImageModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                imageId: switchTargets.Next(ImageId),
                switchTargets: switchTargets);
            return RecordResponse(imageModel);
        }

        public string Reload()
        {
            SwitchTargets = ImagesUtility.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            ImageModel imageModel, Message message = null, bool pushState = true)
        {
            imageModel.MethodType = BaseModel.MethodTypes.Edit;
            return new ImagesResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    imageModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? ImagesUtility.Editor(imageModel)
                        : ImagesUtility.Editor(this))
                .Message(message)
                .PushState(
                    Navigations.Get("Images", ImageId.ToString(), "Reload"),
                    Navigations.Edit("Images", imageModel.ImageId),
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
                    case "Images_Title": Title = new Title(ImageId, Forms.Data(controlId)); break;
                    case "Images_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Images_FileName": FileName = Forms.Data(controlId).ToString(); break;
                    case "Images_Extension": Extension = Forms.Data(controlId).ToString(); break;
                    case "Images_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments = Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default: break;
                }
            });
            Forms.FileKeys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Images_Bin": Bin = Forms.File(controlId); break;
                    default: break;
                }
            });
        }

        private void SetBySession()
        {
            if (!Forms.HasData("Images_ImageSettings")) ImageSettings = Session_ImageSettings();
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
                    case "ImageId": if (dataRow[name] != DBNull.Value) { ImageId = dataRow[name].ToLong(); SavedImageId = ImageId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "Title": Title = new Title(dataRow, "ImageId"); SavedTitle = Title.Value; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "Bin": Bin = dataRow.Bytes("Bin"); SavedBin = Bin; break;
                    case "FileName": FileName = dataRow[name].ToString(); SavedFileName = FileName; break;
                    case "Extension": Extension = dataRow[name].ToString(); SavedExtension = Extension; break;
                    case "Size": Size = dataRow[name].ToInt(); SavedSize = Size; break;
                    case "ImageSettings": ImageSettings = dataRow.String("ImageSettings").Deserialize<ImageSettings>() ?? new ImageSettings(); SavedImageSettings = ImageSettings.ToJson(); break;
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
            return new ImagesResponseCollection(this)
                .Html("#MainContainer", ImagesUtility.Editor(this))
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
        public ImageModel(
            long imageId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            ImageId = imageId;
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

    public class ImageCollection : List<ImageModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public ImageCollection(
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
            IEnumerable<Aggregation> aggregationCollection = null)
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

        public ImageCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private ImageCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new ImageModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public ImageCollection(
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
                Rds.SelectImages(
                    dataTableName: "Main",
                    column: column ?? Rds.ImagesColumnDefault(),
                    join: join ??  Rds.ImagesJoinDefault(),
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
                statements.AddRange(Rds.ImagesAggregations(aggregationCollection, where));
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
                statements: Rds.ImagesStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class ImagesUtility
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = Html.Builder();
            var formData = DataViewFilters.SessionFormData();
            var imageCollection = ImageCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                modelName: "Image",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: IndexScript(
                    imageCollection: imageCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                action: () => hb
                    .Form(
                        attributes: Html.Attributes()
                            .Id_Css("ImagesForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "Images",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: imageCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    imageCollection: imageCollection,
                                    siteSettings: siteSettings,
                                    permissionType: permissionType,
                                    formData: formData,
                                    dataViewName: dataViewName))
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Images")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                    .Dialog_Move("items", siteSettings.SiteId, bulk: true)
                    .Div(attributes: Html.Attributes()
                        .Id_Css("Dialog_ExportSettings", "dialog")
                        .Title(Displays.ExportSettings()))).ToString();
        }

        private static ImageCollection ImageCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new ImageCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Images",
                    formData: formData,
                    where: Rds.ImagesWhere()),
                orderBy: GridSorters.Get(
                    formData, Rds.ImagesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static string IndexScript(
            ImageCollection imageCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            return string.Empty;
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            ImageCollection imageCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    imageCollection: imageCollection,
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
            ImageCollection imageCollection,
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
                            imageCollection: imageCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize.ToString())
                .MainCommands(
                    siteId: siteSettings.SiteId,
                    permissionType: permissionType,
                    verType: Versions.VerTypes.Latest,
                    backUrl: Navigations.Index("Admins"),
                    bulkMoveButton: true,
                    bulkDeleteButton: true,
                    importButton: true,
                    exportButton: true);
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var imageCollection = ImageCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", imageCollection.Count > 0
                    ? Html.Builder().Grid(
                        siteSettings: siteSettings,
                        imageCollection: imageCollection,
                        permissionType: permissionType,
                        formData: formData)
                    : Html.Builder())
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: imageCollection.Aggregations,
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
            var imageCollection = ImageCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", Html.Builder().GridRows(
                    siteSettings: siteSettings,
                    imageCollection: imageCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: imageCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, imageCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            ImageCollection imageCollection,
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
            imageCollection.ForEach(imageModel => hb
                .Tr(
                    attributes: Html.Attributes()
                        .Class("grid-row")
                        .DataId(imageModel.ImageId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: imageModel.ImageId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    imageModel: imageModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.ImagesColumn()
                .ImageId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "ImageId": select.ImageId(); break;
                    case "Ver": select.Ver(); break;
                    case "Title": select.Title(); break;
                    case "Body": select.Body(); break;
                    case "Bin": select.Bin(); break;
                    case "FileName": select.FileName(); break;
                    case "Extension": select.Extension(); break;
                    case "Size": select.Size(); break;
                    case "ImageSettings": select.ImageSettings(); break;
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
            this HtmlBuilder hb, Column column, ImageModel imageModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: imageModel.Ver);
                case "Comments": return hb.Td(column: column, value: imageModel.Comments);
                case "Creator": return hb.Td(column: column, value: imageModel.Creator);
                case "Updator": return hb.Td(column: column, value: imageModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: imageModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: imageModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(new ImageModel(
                    SiteSettingsUtility.ImagesSiteSettings(),
                    Permissions.Admins(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long imageId, bool clearSessions)
        {
            var imageModel = new ImageModel(
                    SiteSettingsUtility.ImagesSiteSettings(),
                    Permissions.Admins(),
                imageId: imageId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            imageModel.SwitchTargets = ImagesUtility.GetSwitchTargets(
                SiteSettingsUtility.ImagesSiteSettings());
            return Editor(imageModel);
        }

        public static string Editor(ImageModel imageModel)
        {
            var hb = Html.Builder();
            var permissionType = Permissions.Admins();
            var siteSettings = SiteSettingsUtility.ImagesSiteSettings();
            return hb.Template(
                siteId: 0,
                modelName: "Image",
                title: imageModel.MethodType != BaseModel.MethodTypes.New
                    ? imageModel.Title.DisplayValue + " - " + Displays.Edit()
                    : Displays.Images() + " - " + Displays.New(),
                permissionType: permissionType,
                verType: imageModel.VerType,
                backUrl: Navigations.ItemIndex(0),
                methodType: imageModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    imageModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager;
                    hb
                        .Editor(
                            imageModel: imageModel,
                            permissionType: permissionType,
                            siteSettings: siteSettings)
                        .Hidden(controlId: "TableName", value: "Images")
                        .Hidden(controlId: "Id", value: imageModel.ImageId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            ImageModel imageModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: Html.Attributes()
                        .Id_Css("ImageForm", "main-form")
                        .Action(imageModel.ImageId != 0
                            ? Navigations.Action("Images", imageModel.ImageId)
                            : Navigations.Action("Images")),
                    action: () => hb
                        .RecordHeader(
                            id: imageModel.ImageId,
                            baseModel: imageModel,
                            tableName: "Images",
                            switchTargets: imageModel.SwitchTargets?
                                .Select(o => o.ToLong()).ToList())
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: imageModel.Comments,
                                verType: imageModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(imageModel: imageModel)
                            .Fields(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                imageModel: imageModel)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: imageModel.VerType,
                                backUrl: Navigations.Index("Images"),
                                referenceType: "Images",
                                referenceId: imageModel.ImageId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        imageModel: imageModel,
                                        siteSettings: siteSettings)))
                        .Hidden(
                            controlId: "Images_Timestamp",
                            css: "must-transport",
                            value: imageModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: imageModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("Images", imageModel.ImageId, imageModel.Ver)
                .Dialog_Copy("Images", imageModel.ImageId)
                .Dialog_Histories(Navigations.Action("Images", imageModel.ImageId))
                .Dialog_OutgoingMail()
                .EditorExtensions(imageModel: imageModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, ImageModel imageModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic())));
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ImageModel imageModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.ColumnCollection
                    .Where(o => o.EditorVisible.ToBool())
                    .OrderBy(o => siteSettings.EditorOrder.IndexOf(o.ColumnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "ImageId": hb.Field(siteSettings, column, imageModel.ImageId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Ver": hb.Field(siteSettings, column, imageModel.Ver.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Title": hb.Field(siteSettings, column, imageModel.Title.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Body": hb.Field(siteSettings, column, imageModel.Body.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "FileName": hb.Field(siteSettings, column, imageModel.FileName.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Extension": hb.Field(siteSettings, column, imageModel.Extension.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                        }
                    });
                hb.VerUpCheckBox(imageModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            ImageModel imageModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            ImageModel imageModel,
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
                    statements: Rds.SelectImages(
                        column: Rds.ImagesColumn().ImageId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Images",
                            formData: formData,
                            where: Rds.ImagesWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.ImagesOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["ImageId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static byte[] NavSiteThumbnail(long referenceId)
        {
            return Images.Get(referenceId, Images.Types.NavSite, Images.SizeTypes.Thumbnail);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static byte[] NavSiteIcon(long referenceId)
        {
            return Images.Get(referenceId, Images.Types.NavSite, Images.SizeTypes.Icon);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Update(long referenceId)
        {
            foreach (var key in Forms.FileKeys())
            {
                switch (key)
                {
                    case "NavSiteImage": return UpdateNavSiteIcon(referenceId, Forms.File(key));
                }
            }
            return new ResponseCollection().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UpdateNavSiteIcon(long referenceId, byte[] data)
        {
            Images.Write(data, referenceId, Images.Types.NavSite);
            return Messages.ResponseFileUpdateCompleted().ToJson();
        }
    }
}
