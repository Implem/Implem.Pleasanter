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
    public class MailAddressModel : BaseModel
    {
        public long OwnerId = 0;
        public string OwnerType = string.Empty;
        public long MailAddressId = 0;
        public string MailAddress = string.Empty;
        public Title Title { get { return new Title(MailAddressId, MailAddress); } }
        public long SavedOwnerId = 0;
        public string SavedOwnerType = string.Empty;
        public long SavedMailAddressId = 0;
        public string SavedMailAddress = string.Empty;
        public bool OwnerId_Updated { get { return OwnerId != SavedOwnerId; } }
        public bool OwnerType_Updated { get { return OwnerType != SavedOwnerType && OwnerType != null; } }
        public bool MailAddressId_Updated { get { return MailAddressId != SavedMailAddressId; } }
        public bool MailAddress_Updated { get { return MailAddress != SavedMailAddress && MailAddress != null; } }
        public List<long> SwitchTargets;

        public MailAddressModel()
        {
        }

        public MailAddressModel(
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

        public MailAddressModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            long mailAddressId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            MailAddressId = mailAddressId;
            PermissionType = permissionType;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public MailAddressModel(
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
        }

        public MailAddressModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectMailAddresses(
                tableType: tableType,
                column: column ?? Rds.MailAddressesDefaultColumns(),
                join: join ??  Rds.MailAddressesJoinDefault(),
                where: where ?? Rds.MailAddressesWhereDefault(this),
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
                    Rds.InsertMailAddresses(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.MailAddressesParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            MailAddressId = newId != 0 ? newId : MailAddressId;
            Get();
            OnCreated();
            return EditorJson(this, Messages.Created(Title.ToString()));
        }

        private void OnCreating()
        {
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
                    case "MailAddresses_OwnerId": if (!SiteSettings.GetColumn("OwnerId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_OwnerType": if (!SiteSettings.GetColumn("OwnerType").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_MailAddressId": if (!SiteSettings.GetColumn("MailAddressId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Ver": if (!SiteSettings.GetColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_MailAddress": if (!SiteSettings.GetColumn("MailAddress").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Title": if (!SiteSettings.GetColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Comments": if (!SiteSettings.GetColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Creator": if (!SiteSettings.GetColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Updator": if (!SiteSettings.GetColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_VerUp": if (!SiteSettings.GetColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                    Rds.UpdateMailAddresses(
                        verUp: VerUp,
                        where: Rds.MailAddressesWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: param ?? Rds.MailAddressesParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new MailAddressesResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        private void OnUpdated(ref MailAddressesResponseCollection responseCollection)
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
                    case "MailAddresses_OwnerId": if (!SiteSettings.GetColumn("OwnerId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_OwnerType": if (!SiteSettings.GetColumn("OwnerType").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_MailAddressId": if (!SiteSettings.GetColumn("MailAddressId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Ver": if (!SiteSettings.GetColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_MailAddress": if (!SiteSettings.GetColumn("MailAddress").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Title": if (!SiteSettings.GetColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Comments": if (!SiteSettings.GetColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Creator": if (!SiteSettings.GetColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Updator": if (!SiteSettings.GetColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_VerUp": if (!SiteSettings.GetColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(MailAddressesResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FormResponse(this)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "MailAddresses"))
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
                    Rds.UpdateOrInsertMailAddresses(
                        selectIdentity: true,
                        where: where ?? Rds.MailAddressesWhereDefault(this),
                        param: param ?? Rds.MailAddressesParamDefault(this, setDefault: true))
                });
            MailAddressId = newId != 0 ? newId : MailAddressId;
            Get();
            var responseCollection = new MailAddressesResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref MailAddressesResponseCollection responseCollection)
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
                    Rds.DeleteMailAddresses(
                        where: Rds.MailAddressesWhere().MailAddressId(MailAddressId))
                });
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
            var responseCollection = new MailAddressesResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.Index("MailAddresses"));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref MailAddressesResponseCollection responseCollection)
        {
        }

        public string Restore(long mailAddressId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            MailAddressId = mailAddressId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreMailAddresses(
                        where: Rds.MailAddressesWhere().MailAddressId(MailAddressId))
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
                statements: Rds.PhysicalDeleteMailAddresses(
                    tableType: tableType,
                    param: Rds.MailAddressesParam().MailAddressId(MailAddressId)));
            var responseCollection = new MailAddressesResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref MailAddressesResponseCollection responseCollection)
        {
        }

        public string Histories()
        {
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columnCollection: SiteSettings.HistoryColumnCollection(),
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new MailAddressCollection(
                            siteSettings: SiteSettings,
                            permissionType: PermissionType,
                            where: Rds.MailAddressesWhere().MailAddressId(MailAddressId),
                            orderBy: Rds.MailAddressesOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(mailAddressModel => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(mailAddressModel.Ver)
                                            .DataLatest(1, _using: mailAddressModel.Ver == Ver),
                                        action: () =>
                                            SiteSettings.HistoryColumnCollection()
                                                .ForEach(column => hb
                                                    .TdValue(column, mailAddressModel))))));
            return new MailAddressesResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.MailAddressesWhere()
                    .MailAddressId(MailAddressId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            SwitchTargets = MailAddressUtilities.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string EditorJson()
        {
            return EditorJson(this);
        }

        private string EditorJson(MailAddressModel mailAddressModel, Message message = null)
        {
            mailAddressModel.MethodType = MethodTypes.Edit;
            return new MailAddressesResponseCollection(this)
                .Invoke("clearDialogs")
                .ReplaceAll(
                    "#MainContainer",
                    mailAddressModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? MailAddressUtilities.Editor(mailAddressModel)
                        : MailAddressUtilities.Editor(this))
                .Invoke("setCurrentIndex")
                .Invoke("validateMailAddresses")
                .Message(message)
                .ClearFormData()
                .ToJson();
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "MailAddresses_OwnerId": OwnerId = Forms.Data(controlId).ToLong(); break;
                    case "MailAddresses_OwnerType": OwnerType = Forms.Data(controlId).ToString(); break;
                    case "MailAddresses_MailAddress": MailAddress = Forms.Data(controlId).ToString(); break;
                    case "MailAddresses_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                    default: break;
                }
            });
        }

        private void SetBySession()
        {
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
                    case "OwnerId": if (dataRow[name] != DBNull.Value) { OwnerId = dataRow[name].ToLong(); SavedOwnerId = OwnerId; } break;
                    case "OwnerType": if (dataRow[name] != DBNull.Value) { OwnerType = dataRow[name].ToString(); SavedOwnerType = OwnerType; } break;
                    case "MailAddressId": if (dataRow[name] != DBNull.Value) { MailAddressId = dataRow[name].ToLong(); SavedMailAddressId = MailAddressId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "MailAddress": MailAddress = dataRow[name].ToString(); SavedMailAddress = MailAddress; break;
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
            return new MailAddressesResponseCollection(this)
                .ReplaceAll(
                    "#MainContainer",
                    MailAddressUtilities.Editor(this))
                .Invoke("validateMailAddresses")
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
        public MailAddressModel(long userId)
        {
            Get(
                where: Rds.MailAddressesWhere()
                    .OwnerId(userId)
                    .OwnerType("Users"),
                top: 1);
        }
    }
}
