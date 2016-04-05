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
    public class SiteModel : BaseItemModel
    {
        public long Id { get { return SiteId; } }
        public override long UrlId { get { return SiteId; } }
        public int TenantId = Sessions.TenantId();
        public string ReferenceType = "Sites";
        public long ParentId = 0;
        public long InheritPermission = 0;
        public SiteCollection Ancestors = null;
        public PermissionCollection PermissionSourceCollection = null;
        public PermissionCollection PermissionDestinationCollection = null;
        public int SiteMenu = 0;
        public TitleBody TitleBody { get { return new TitleBody(SiteId, Title.Value, Title.DisplayValue, Body); } }
        public int SavedTenantId = Sessions.TenantId();
        public string SavedReferenceType = "Sites";
        public long SavedParentId = 0;
        public long SavedInheritPermission = 0;
        public long SavedPermissionType = 31;
        public string SavedSiteSettings = string.Empty;
        public SiteCollection SavedAncestors = null;
        public PermissionCollection SavedPermissionSourceCollection = null;
        public PermissionCollection SavedPermissionDestinationCollection = null;
        public int SavedSiteMenu = 0;
        public bool TenantId_Updated { get { return TenantId != SavedTenantId; } }
        public bool ReferenceType_Updated { get { return ReferenceType != SavedReferenceType && ReferenceType != null; } }
        public bool ParentId_Updated { get { return ParentId != SavedParentId; } }
        public bool InheritPermission_Updated { get { return InheritPermission != SavedInheritPermission; } }
        public bool SiteSettings_Updated { get { return SiteSettings.ToJson() != SavedSiteSettings && SiteSettings.ToJson() != null; } }

        public SiteSettings Session_SiteSettings()
        {
            return this.PageSession("SiteSettings") != null
                ? this.PageSession("SiteSettings")?.ToString().Deserialize<SiteSettings>() ?? new SiteSettings(ReferenceType)
                : SiteSettings;
        }

        public void  Session_SiteSettings(object value)
        {
            this.PageSession("SiteSettings", value);
        }

        public PermissionCollection Session_PermissionSourceCollection()
        {
            return this.PageSession("PermissionSourceCollection") != null
                ? this.PageSession("PermissionSourceCollection") as PermissionCollection ?? new PermissionCollection()
                : PermissionSourceCollection;
        }

        public void  Session_PermissionSourceCollection(object value)
        {
            this.PageSession("PermissionSourceCollection", value);
        }

        public PermissionCollection Session_PermissionDestinationCollection()
        {
            return this.PageSession("PermissionDestinationCollection") != null
                ? this.PageSession("PermissionDestinationCollection") as PermissionCollection ?? new PermissionCollection()
                : PermissionDestinationCollection;
        }

        public void  Session_PermissionDestinationCollection(object value)
        {
            this.PageSession("PermissionDestinationCollection", value);
        }

        public List<long> SwitchTargets;

        public SiteModel()
        {
        }

        public SiteModel(
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = this.SitesSiteSettings();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public SiteModel(
            long siteId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteId = siteId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public SiteModel(
            DataRow dataRow)
        {
            OnConstructing();
            Set(dataRow);
            OnConstructed();
        }

        private void OnConstructing()
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnConstructed()
        {
            SiteInfo.SetSiteDeptIdCollection(SiteId);
            SiteInfo.SetSiteUserIdCollection(SiteId);
        }

        public void ClearSessions()
        {
            Session_SiteSettings(null);
            Session_PermissionSourceCollection(null);
            Session_PermissionDestinationCollection(null);
        }

        public SiteModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectSites(
                tableType: tableType,
                column: column ?? Rds.SitesColumnDefault(),
                join: join ??  Rds.SitesJoinDefault(),
                where: where ?? Rds.SitesWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            SetSiteSettingsProperties();
            return this;
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
                    Rds.UpdateSites(
                        verUp: VerUp,
                        where: Rds.SitesWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.SitesParamDefault(this, paramAll: paramAll),
                        countRecord: true),
                    Rds.Conditions("@@rowcount = 1"),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(SiteId),
                        param: Rds.ItemsParam()
                            .SiteId(SiteId)
                            .Title(SitesUtility.TitleDisplayValue(SiteSettings, this))
                            .Subset(Jsons.ToJson(new SiteSubset(this, SiteSettings)))
                            .UpdateTarget(true)),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(SiteId)),
                    LinksUtility.Insert(SiteSettings.LinkColumnSiteIdHash
                        .Select(o => o.Value)
                        .Distinct()
                        .ToDictionary(o => o, o => SiteId)),
                    Rds.End()
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new SitesResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnUpdating(ref SqlParamCollection param)
        {
            if (ReferenceType_Updated)
            {
                SiteSettings = new SiteSettings(ReferenceType);
            }
        }

        private void OnUpdated(ref SitesResponseCollection responseCollection)
        {
        }

        private string ValidateBeforeUpdate()
        {
            if (!PermissionType.CanEditSite())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_TenantId": if (!SiteSettings.AllColumn("TenantId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_SiteId": if (!SiteSettings.AllColumn("SiteId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_Body": if (!SiteSettings.AllColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_TitleBody": if (!SiteSettings.AllColumn("TitleBody").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_ReferenceType": if (!SiteSettings.AllColumn("ReferenceType").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_ParentId": if (!SiteSettings.AllColumn("ParentId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_InheritPermission": if (!SiteSettings.AllColumn("InheritPermission").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_PermissionType": if (!SiteSettings.AllColumn("PermissionType").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_SiteSettings": if (!SiteSettings.AllColumn("SiteSettings").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_Ancestors": if (!SiteSettings.AllColumn("Ancestors").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_PermissionSourceCollection": if (!SiteSettings.AllColumn("PermissionSourceCollection").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_PermissionDestinationCollection": if (!SiteSettings.AllColumn("PermissionDestinationCollection").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_SiteMenu": if (!SiteSettings.AllColumn("SiteMenu").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Sites_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private ResponseCollection ResponseByUpdate(SitesResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.DisplayValue + " - " + Displays.EditSettings())
                .Html("#RecordInfo", Html.Builder().RecordInfo(baseModel: this, tableName: "Sites"))
                .Html("#RecordHistories", Html.Builder().RecordHistories(ver: Ver, verType: VerType))
                .Message(Messages.Updated(Title.ToString()))
                .RemoveComment(DeleteCommentId, _using: DeleteCommentId != 0)
                .ClearFormData();
        }

        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            if (!PermissionType.CanEditSite())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SetBySession();
            OnUpdatingOrCreating(ref where, ref param);
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam()
                            .ReferenceType("Sites")
                            .SiteId(SiteId)
                            .Title(Title.Value)),
                    Rds.UpdateOrInsertSites(
                        selectIdentity: true,
                        where: where ?? Rds.SitesWhereDefault(this),
                        param: param ?? Rds.SitesParamDefault(this, setDefault: true))
                });
            SiteId = newId != 0 ? newId : SiteId;
            Get();
            var responseCollection = new SitesResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref SitesResponseCollection responseCollection)
        {
        }

        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanEditSite())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere().ReferenceId(SiteId)),
                    Rds.DeleteSites(
                        where: Rds.SitesWhere().TenantId(TenantId).SiteId(SiteId))
                });
            var responseCollection = new SitesResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.ItemIndex(ParentId));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref SitesResponseCollection responseCollection)
        {
        }

        public string Restore(long siteId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SiteId = siteId;
            Rds.ExecuteNonQuery(
                connectionString: Def.Db.DbOwner,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(SiteId)),
                    Rds.RestoreSites(
                        where: Rds.SitesWhere().SiteId(SiteId))
                });
            return new ResponseCollection().ToJson();
        }

        public string PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            if (!PermissionType.CanEditSite())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnPhysicalDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteSites(
                    tableType: tableType,
                    param: Rds.SitesParam().TenantId(TenantId).SiteId(SiteId)));
            var responseCollection = new SitesResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref SitesResponseCollection responseCollection)
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
                    new SiteCollection(
                        where: Rds.SitesWhere().SiteId(SiteId),
                        orderBy: Rds.SitesOrderBy().UpdatedTime(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(siteModel => hb
                            .Tr(
                                attributes: Html.Attributes()
                                    .Class("grid-row not-link")
                                    .OnClick(Def.JavaScript.HistoryAndCloseDialog
                                        .Params(siteModel.Ver))
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-latest", 1, _using: siteModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryGridColumnCollection().ForEach(column =>
                                        hb.TdValue(column, siteModel))));
                });
            return new SitesResponseCollection(this).Html("#HistoriesForm", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.SitesWhere()
                    .SiteId(SiteId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerType(SiteId);
            SwitchTargets = SitesUtility.GetSwitchTargets(SiteSettings, SiteId);
            return Editor();
        }

        public string PreviousHistory()
        {
            Get(
                where: Rds.SitesWhere()
                    .SiteId(SiteId)
                    .Ver(Forms.Int("Ver"), _operator: "<"),
                orderBy: Rds.SitesOrderBy()
                    .Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = SitesUtility.GetSwitchTargets(SiteSettings, SiteId);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(SiteId, Versions.DirectioTypes.Previous);
                    return Editor();
                default:
                    return new SitesResponseCollection(this).ToJson();
            }
        }

        public string NextHistory()
        {
            Get(
                where: Rds.SitesWhere()
                    .SiteId(SiteId)
                    .Ver(Forms.Int("Ver"), _operator: ">"),
                orderBy: Rds.SitesOrderBy()
                    .Ver(SqlOrderBy.Types.asc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = SitesUtility.GetSwitchTargets(SiteSettings, SiteId);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(SiteId, Versions.DirectioTypes.Next);
                    return Editor();
                default:
                    return new SitesResponseCollection(this).ToJson();
            }
        }

        public string Previous()
        {
            var switchTargets = SitesUtility.GetSwitchTargets(SiteSettings, SiteId);
            var siteModel = new SiteModel(
                siteId: switchTargets.Previous(SiteId),
                switchTargets: switchTargets);
            return RecordResponse(siteModel);
        }

        public string Next()
        {
            var switchTargets = SitesUtility.GetSwitchTargets(SiteSettings, SiteId);
            var siteModel = new SiteModel(
                siteId: switchTargets.Next(SiteId),
                switchTargets: switchTargets);
            return RecordResponse(siteModel);
        }

        public string Reload()
        {
            SwitchTargets = SitesUtility.GetSwitchTargets(SiteSettings, SiteId);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            SiteModel siteModel, Message message = null, bool pushState = true)
        {
            siteModel.MethodType = BaseModel.MethodTypes.Edit;
            return new SitesResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    siteModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? SitesUtility.Editor(siteModel)
                        : SitesUtility.Editor(this))
                .Message(message)
                .PushState(
                    Navigations.ItemEdit(siteModel.SiteId),
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
                    case "Sites_Title": Title = new Title(SiteId, Forms.Data(controlId)); break;
                    case "Sites_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Sites_ReferenceType": ReferenceType = Forms.Data(controlId).ToString(); break;
                    case "Sites_InheritPermission": InheritPermission = Forms.Data(controlId).ToLong(); break;
                    case "Sites_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
            SetSiteSettings();
        }

        private void SetBySession()
        {
            if (!Forms.HasData("Sites_SiteSettings")) SiteSettings = Session_SiteSettings();
            if (!Forms.HasData("Sites_PermissionSourceCollection")) PermissionSourceCollection = Session_PermissionSourceCollection();
            if (!Forms.HasData("Sites_PermissionDestinationCollection")) PermissionDestinationCollection = Session_PermissionDestinationCollection();
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
                    case "TenantId": if (dataRow[name] != DBNull.Value) { TenantId = dataRow[name].ToInt(); SavedTenantId = TenantId; } break;
                    case "SiteId": if (dataRow[name] != DBNull.Value) { SiteId = dataRow[name].ToLong(); SavedSiteId = SiteId; } break;
                    case "UpdatedTime": if (dataRow[name] != DBNull.Value) { UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "Title": Title = new Title(dataRow, "SiteId"); SavedTitle = Title.Value; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "ReferenceType": ReferenceType = dataRow[name].ToString(); SavedReferenceType = ReferenceType; break;
                    case "ParentId": ParentId = dataRow[name].ToLong(); SavedParentId = ParentId; break;
                    case "InheritPermission": InheritPermission = dataRow[name].ToLong(); SavedInheritPermission = InheritPermission; break;
                    case "PermissionType": PermissionType = GetPermissionType(dataRow); SavedPermissionType = PermissionType.ToLong(); break;
                    case "SiteSettings": SiteSettings = GetSiteSettings(dataRow); SavedSiteSettings = SiteSettings.ToJson(); break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                }
            }
            if (SiteSettings != null)
            {
                Title.DisplayValue = SitesUtility.TitleDisplayValue(SiteSettings, this);
            }
        }

        private string Editor()
        {
            return new SitesResponseCollection(this)
                .Html("#MainContainer", SitesUtility.Editor(this))
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
        public SiteModel(
            SiteSettings siteSettings,
            long siteId)
        {
            OnConstructing();
            SiteId = siteId;
            Get();
            SiteSettings = siteSettings;
            OnConstructed();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Create(
            Permissions.Types permissionType,
            long parentId,
            long inheritPermission,
            bool paramAll = false)
        {
            SiteId = 0;
            ParentId = parentId;
            InheritPermission = inheritPermission;
            if (!paramAll) SiteSettings = new SiteSettings(ReferenceType);
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam().ReferenceType("Sites")),
                    Rds.InsertSites(
                        param: Rds.SitesParam()
                            .SiteId(raw: Def.Sql.Identity)
                            .TenantId(TenantId)
                            .Title(Title.Value.MaxLength(1024))
                            .Body(Body)
                            .ReferenceType(ReferenceType.MaxLength(32))
                            .ParentId(ParentId)
                            .InheritPermission(raw: inheritPermission == 0
                                ? Def.Sql.Identity
                                : inheritPermission.ToString())
                            .SiteSettings(SiteSettings.ToJson())
                            .Comments(Comments.ToJson())),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(raw: Def.Sql.Identity),
                        param: Rds.ItemsParam()
                            .SiteId(raw: Def.Sql.Identity)
                            .Subset(Jsons.ToJson(new SiteSubset(this, SiteSettings)))),
                    Rds.InsertPermissions(
                        param: Rds.PermissionsParam()
                            .ReferenceType("Sites")
                            .ReferenceId(raw: Def.Sql.Identity)
                            .DeptId(0)
                            .UserId(Sessions.UserId())
                            .PermissionType(Permissions.Types.Manager),
                        _using: InheritPermission == 0)
                });
            SiteId = newId != 0 ? newId : SiteId;
            Get();
            SiteSettings = SiteSettingsUtility.Get(SiteId, ReferenceType);
            switch (ReferenceType)
            {
                case "Wikis":
                    var wikiModel = new WikiModel(SiteSettings, PermissionType)
                    {
                        SiteId = SiteId,
                        Title = Title,
                        Body = Body,
                        Comments = Comments
                    };
                    wikiModel.Create();
                    return new ResponseCollection().Href(
                        Navigations.ItemEdit(wikiModel.WikiId)).ToJson();
                default:
                    return PermissionType.CanEditSite()
                        ? RecordResponse(this, Messages.Created(Title.ToString()))
                        : new ResponseCollection().Href(Navigations.ItemIndex(SiteId)).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Copy()
        {
            Title.Value += Displays.SuffixCopy();
            if (!Forms.Data("Dialog_ConfirmCopy_WithComments").ToBool())
            {
                Comments.Clear();
            }
            return Create(
                permissionType: PermissionType,
                parentId: ParentId,
                inheritPermission: InheritPermission,
                paramAll: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SiteSettings GetSiteSettings(DataRow dataRow)
        {
            return dataRow["SiteSettings"].ToString().Deserialize<SiteSettings>() ??
                new SiteSettings(ReferenceType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsProperties()
        {
            if (SiteSettings == null) SiteSettings = SiteSettingsUtility.SitesSiteSettings(SiteId);
            SiteSettings.SiteId = SiteId;
            SiteSettings.ParentId = ParentId;
            SiteSettings.Title = Title.Value;
            SiteSettings.AccessStatus = AccessStatus;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsPropertiesBySession()
        {
            SiteSettings = Session_SiteSettings();
            SetSiteSettingsProperties();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SetSiteSettings()
        {
            var responseCollection = new SitesResponseCollection(this);
            SetSiteSettingsPropertiesBySession();
            switch (Forms.Data("ControlId"))
            {
                case "OpenDialog_ColumnProperties":
                    OpenDialog_ColumnProperties(responseCollection);
                    break;
                default:
                    SetSiteSettings(responseCollection);
                    SetGridColumns(responseCollection);
                    SetEditorColumns(responseCollection);
                    SetColumnProperties(responseCollection);
                    SetTitleColumns(responseCollection);
                    SetAggregations(responseCollection);
                    SetSummaries(responseCollection);
                    break;
            }
            Session_SiteSettings(Jsons.ToJson(SiteSettings));
            return responseCollection.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private Permissions.Types GetPermissionType(DataRow dataRow)
        {
            return Permissions.Admins(
                (Permissions.Types)dataRow["PermissionType"].ToLong());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string MoveSiteMenu(long siteId)
        {
            if (SiteId != 0 && !PermissionType.CanEditSite())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            Rds.ExecuteNonQuery(statements: Rds.UpdateSites(
                where: Rds.SitesWhere()
                    .TenantId(Sessions.TenantId())
                    .SiteId(siteId),
                param: Rds.SitesParam().ParentId(Forms.Long("ParentId"))));
            return new ResponseCollection()
                .Remove(".nav-site[data-id=\"" + siteId + "\"]")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SortSiteMenu()
        {
            if (SiteId != 0 && !PermissionType.CanEditSite())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var ownerId = SiteId == 0
                ? Sessions.UserId()
                : 0;
            new OrderModel()
            {
                ReferenceId = SiteId,
                ReferenceType = "Sites",
                OwnerId = ownerId,
                Data = Forms.LongList("Data").Where(o => o != 0).ToList()
            }.UpdateOrCreate();
            return new ResponseCollection().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDialog_ColumnProperties(ResponseCollection responseCollection)
        {
            var selectedColumns = Forms.Data("EditorColumns").Split(';').
                Where(o => o != string.Empty);
            if (selectedColumns.Count() != 1)
            {
                responseCollection.Message(Messages.RequireColumn());
            }
            else
            {
                var column = SiteSettings.EditorColumn(selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    responseCollection.Message(Messages.InvalidRequest());
                }
                else
                {
                    responseCollection
                        .Html(
                            "#Dialog_ColumnProperties",
                            SitesUtility.ColumnProperties(SiteSettings, column))
                        .ClearFormData("ColumnProperty,", "startsWith");
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteSettings(ResponseCollection responseCollection)
        {
            Forms.All().Where(o => o.Key.StartsWith("SiteSettings,")).ForEach(data =>
                SiteSettings.Set(data.Key.Split_2nd(), data.Value));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetGridColumns(ResponseCollection responseCollection)
        {
            var selectedColumns = Forms.Data("GridColumns").Split(';').
                Where(o => o != string.Empty);
            if (selectedColumns.Count() != 0)
            {
                SiteSettings.SetGridColumns(
                    responseCollection,
                    Forms.Data("ControlId"),
                    selectedColumns);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetAggregations(ResponseCollection responseCollection)
        {
            if (!Forms.Data("AggregationDestination").IsNullOrEmpty() ||
                !Forms.Data("AggregationSource").IsNullOrEmpty())
            {
                SiteSettings.SetAggregations(
                    responseCollection,
                    Forms.Data("ControlId"),
                    Forms.Data("AggregationDestination").Split(';'),
                    Forms.Data("AggregationSource").Split(';'));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummaries(ResponseCollection responseCollection)
        {
            var controlId = Forms.Data("ControlId");
            switch (controlId)
            {
                case "SummarySiteId":
                    var destinationSiteId = Forms.Long("SummarySiteId");
                    var destinationSiteSettings = new SiteModel(destinationSiteId);
                    var siteDataRows = SiteSettings.SummarySiteDataRows();
                    responseCollection
                        .ReplaceAll("#Field_SummaryDestinationColumn", Html.Builder()
                            .SummaryDestinationColumn(
                                referenceType: destinationSiteSettings.ReferenceType,
                                siteId: destinationSiteSettings.SiteId,
                                siteDataRows: siteDataRows))
                        .ReplaceAll("#Field_SummaryLinkColumn", Html.Builder()
                            .SummaryLinkColumn(
                                siteId: destinationSiteId,
                                siteSettings: SiteSettings));
                    break;
                case "SummaryType":
                    responseCollection.ReplaceAll("#Field_SummarySourceColumn", Html.Builder()
                        .SummarySourceColumn(SiteSettings, Forms.Data("SummaryType")));
                    break;
                case "AddSummary":
                    SiteSettings.AddSummary(
                        responseCollection,
                        Forms.Long("SummarySiteId"),
                        new SiteModel(Forms.Long("SummarySiteId")).ReferenceType,
                        Forms.Data("SummaryDestinationColumn"),
                        Forms.Data("SummaryLinkColumn"),
                        Forms.Data("SummaryType"),
                        Forms.Data("SummarySourceColumn"));
                    responseCollection.ReplaceAll("#SummarySettings", Html.Builder()
                        .SummarySettings(sourceSiteSettings: SiteSettings));
                    break;
                default:
                    if (controlId.StartsWith("DeleteSummary,"))
                    {
                        var summary = SiteSettings.SummaryCollection.FirstOrDefault(
                            o => o.Id == controlId.Split(',')._2nd().ToLong());
                        SiteSettings.DeleteSummary(responseCollection, summary.Id);
                        responseCollection.ReplaceAll("#SummarySettings", Html.Builder()
                            .SummarySettings(sourceSiteSettings: SiteSettings));
                    }
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorColumns(ResponseCollection responseCollection)
        {
            var selectedColumns = Forms.Data("EditorColumns").Split(';').
                Where(o => o != string.Empty);
            if (selectedColumns.Count() != 0)
            {
                var controlId = Forms.Data("ControlId");
                if (controlId == "HideEditorColumns" &&
                    selectedColumns.Any(o => !SiteSettings.EditorColumn(o).Nullable))
                {
                    responseCollection.Message(Messages.CanNotHide(
                        SiteSettings.EditorColumn(selectedColumns.FirstOrDefault(o =>
                            !SiteSettings.EditorColumn(o).Nullable)).LabelText));
                }
                else
                {
                    SiteSettings.SetEditorColumns(responseCollection, controlId, selectedColumns);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetColumnProperties(ResponseCollection responseCollection)
        {
            var selectedColumns = Forms.Data("EditorColumns").Split(';').
                Where(o => o != string.Empty);
            if (selectedColumns.Count() == 1)
            {
                var column = SiteSettings.EditorColumn(selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    responseCollection.Message(Messages.InvalidRequest());
                }
                else
                {
                    Forms.All()
                        .Where(o => o.Key.StartsWith("ColumnProperty,"))
                        .ForEach(data =>
                            SiteSettings.SetColumnProperty(
                                column,
                                data.Key.Split_2nd(),
                                data.Value));
                    responseCollection.Html("#EditorColumns",
                        Html.Builder().SelectableItems(
                            listItemCollection: SiteSettings.EditorColumnHash(),
                            selectedValueTextCollection: selectedColumns));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SynchronizeSummary()
        {
            SetSiteSettingsPropertiesBySession();
            return Summaries.Synchronize(SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetTitleColumns(ResponseCollection responseCollection)
        {
            var selectedColumns = Forms.Data("TitleColumns").Split(';').
                Where(o => o != string.Empty);
            if (selectedColumns.Count() != 0)
            {
                SiteSettings.SetTitleColumns(
                    responseCollection, Forms.Data("ControlId"), selectedColumns);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SiteModel InheritSite()
        {
            return new SiteModel(InheritPermission);
        }
    }

    public class SiteSubset
    {
        public int TenantId;
        public string TenantId_LabelText;
        public long SiteId;
        public string SiteId_LabelText;
        public int Ver;
        public string Ver_LabelText;
        public Title Title;
        public string Title_LabelText;
        public string Body;
        public string Body_LabelText;
        public TitleBody TitleBody;
        public string TitleBody_LabelText;
        public string ReferenceType;
        public string ReferenceType_LabelText;
        public long ParentId;
        public string ParentId_LabelText;
        public long InheritPermission;
        public string InheritPermission_LabelText;
        public Permissions.Types PermissionType;
        public string PermissionType_LabelText;
        public SiteSettings SiteSettings;
        public string SiteSettings_LabelText;
        public SiteCollection Ancestors;
        public string Ancestors_LabelText;
        public PermissionCollection PermissionSourceCollection;
        public string PermissionSourceCollection_LabelText;
        public PermissionCollection PermissionDestinationCollection;
        public string PermissionDestinationCollection_LabelText;
        public int SiteMenu;
        public string SiteMenu_LabelText;
        public Comments Comments;
        public string Comments_LabelText;
        public User Creator;
        public string Creator_LabelText;
        public User Updator;
        public string Updator_LabelText;

        public SiteSubset()
        {
        }

        public SiteSubset(SiteModel siteModel, SiteSettings siteSettings)
        {
            TenantId = siteModel.TenantId;
            TenantId_LabelText = siteSettings.EditorColumn("TenantId")?.LabelText;
            SiteId = siteModel.SiteId;
            SiteId_LabelText = siteSettings.EditorColumn("SiteId")?.LabelText;
            Ver = siteModel.Ver;
            Ver_LabelText = siteSettings.EditorColumn("Ver")?.LabelText;
            Title = siteModel.Title;
            Title_LabelText = siteSettings.EditorColumn("Title")?.LabelText;
            Body = siteModel.Body;
            Body_LabelText = siteSettings.EditorColumn("Body")?.LabelText;
            TitleBody = siteModel.TitleBody;
            TitleBody_LabelText = siteSettings.EditorColumn("TitleBody")?.LabelText;
            ReferenceType = siteModel.ReferenceType;
            ReferenceType_LabelText = siteSettings.EditorColumn("ReferenceType")?.LabelText;
            ParentId = siteModel.ParentId;
            ParentId_LabelText = siteSettings.EditorColumn("ParentId")?.LabelText;
            InheritPermission = siteModel.InheritPermission;
            InheritPermission_LabelText = siteSettings.EditorColumn("InheritPermission")?.LabelText;
            PermissionType = siteModel.PermissionType;
            PermissionType_LabelText = siteSettings.EditorColumn("PermissionType")?.LabelText;
            SiteSettings = siteModel.SiteSettings;
            SiteSettings_LabelText = siteSettings.EditorColumn("SiteSettings")?.LabelText;
            Ancestors = siteModel.Ancestors;
            Ancestors_LabelText = siteSettings.EditorColumn("Ancestors")?.LabelText;
            PermissionSourceCollection = siteModel.PermissionSourceCollection;
            PermissionSourceCollection_LabelText = siteSettings.EditorColumn("PermissionSourceCollection")?.LabelText;
            PermissionDestinationCollection = siteModel.PermissionDestinationCollection;
            PermissionDestinationCollection_LabelText = siteSettings.EditorColumn("PermissionDestinationCollection")?.LabelText;
            SiteMenu = siteModel.SiteMenu;
            SiteMenu_LabelText = siteSettings.EditorColumn("SiteMenu")?.LabelText;
            Comments = siteModel.Comments;
            Comments_LabelText = siteSettings.EditorColumn("Comments")?.LabelText;
            Creator = siteModel.Creator;
            Creator_LabelText = siteSettings.EditorColumn("Creator")?.LabelText;
            Updator = siteModel.Updator;
            Updator_LabelText = siteSettings.EditorColumn("Updator")?.LabelText;
        }

        public Dictionary<string, int> SearchIndexCollection()
        {
            var searchIndexHash = new Dictionary<string, int>();
            Title.SearchIndexes(searchIndexHash, 4);
            Body.SearchIndexes(searchIndexHash, 200);
            Comments.SearchIndexes(searchIndexHash, 200);
            Creator.SearchIndexes(searchIndexHash, 100);
            Updator.SearchIndexes(searchIndexHash, 100);
            SearchIndexExtensions.OutgoingMailsSearchIndexes(searchIndexHash, "Sites", SiteId);
            return searchIndexHash;
        }
    }

    public class SiteCollection : List<SiteModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public SiteCollection(
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
            Set(Get(
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

        public SiteCollection(
            DataTable dataTable)
        {
            Set(dataTable);
        }

        private SiteCollection Set(
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new SiteModel(dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public SiteCollection(
            string commandText,
            SqlParamCollection param = null)
        {
            Set(Get(commandText, param));
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
                Rds.SelectSites(
                    dataTableName: "Main",
                    column: column ?? Rds.SitesColumnDefault(),
                    join: join ??  Rds.SitesJoinDefault(),
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
                statements.AddRange(Rds.SitesAggregations(aggregationCollection, where));
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
                statements: Rds.SitesStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class SitesUtility
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, SiteModel siteModel)
        {
            switch (column.ColumnName)
            {
                case "SiteId": return hb.Td(column: column, value: siteModel.SiteId);
                case "UpdatedTime": return hb.Td(column: column, value: siteModel.UpdatedTime);
                case "Ver": return hb.Td(column: column, value: siteModel.Ver);
                case "Title": return hb.Td(column: column, value: siteModel.Title);
                case "Body": return hb.Td(column: column, value: siteModel.Body);
                case "TitleBody": return hb.Td(column: column, value: siteModel.TitleBody);
                case "Comments": return hb.Td(column: column, value: siteModel.Comments);
                case "Creator": return hb.Td(column: column, value: siteModel.Creator);
                case "Updator": return hb.Td(column: column, value: siteModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: siteModel.CreatedTime);
                default: return hb;
            }
        }

        public static List<long> GetSwitchTargets(SiteSettings siteSettings, long siteId)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData(siteId);
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn().SiteId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Sites",
                            formData: formData,
                            where: Rds.SitesWhere().TenantId(Sessions.TenantId()).SiteId(siteId)),
                        orderBy: GridSorters.Get(
                            formData, Rds.SitesOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["SiteId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        private static HtmlBuilder ReferenceType(
            this HtmlBuilder hb, string selectedValue, BaseModel.MethodTypes methodType)
        {
            return methodType == BaseModel.MethodTypes.New
                ? hb.Select(
                    attributes: Html.Attributes()
                        .Id_Css("Sites_ReferenceType", "control-dropdown"),
                    action: () => hb
                        .OptionCollection(optionCollection: new Dictionary<string, string>
                        {
                            { "Sites", Displays.Sites() },
                            { "Issues", Displays.Issues() },
                            { "Results", Displays.Results() },
                            { "Wikis", Displays.Wikis() }
                        },
                        selectedValue: selectedValue))
                : hb.Span(css: "control-text", action: () => hb
                    .Text(text: Displays.Get(selectedValue)));
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, SiteModel siteModel)
        {
            var displayValue = siteSettings.ColumnCollection
                .Where(o => o.TitleVisible.ToBool())
                .OrderBy(o => siteSettings.TitleOrder.IndexOf(o.ColumnName))
                .Select(column => TitleDisplayValue(column, siteModel))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, SiteModel siteModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(siteModel.Title.Value).Text()
                    : siteModel.Title.Value;
                default: return string.Empty;
            }
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, DataRow dataRow)
        {
            var displayValue = siteSettings.ColumnCollection
                .Where(o => o.TitleVisible.ToBool())
                .OrderBy(o => siteSettings.TitleOrder.IndexOf(o.ColumnName))
                .Select(column => TitleDisplayValue(column, dataRow))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, DataRow dataRow)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(dataRow["Title"].ToString()).Text()
                    : dataRow["Title"].ToString();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(long siteId, bool clearSessions)
        {
            return Editor(new SiteModel(
                siteId, clearSessions, methodType: BaseModel.MethodTypes.Edit));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, SiteModel siteModel)
        {
            return hb.Ul(css: "tabmenu", action: () =>
            {
                hb.Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.Basic()));
                if (siteModel.MethodType != BaseModel.MethodTypes.New)
                {
                    hb.Li(() => hb
                        .A(
                            href: "#SiteImageSettingsEditor",
                            text: Displays.SiteImageSettingsEditor()));
                    if (siteModel.ReferenceType != "Sites")
                    {
                        hb
                            .Li(() => hb
                                .A(
                                    href: "#GridSettingsEditor",
                                    text: Displays.GridSettingsEditor()))
                            .Li(action: () => hb
                                .A(
                                    href: "#EditorSettingsEditor",
                                    text: Displays.EditorSettingsEditor()))
                            .Li(() => hb
                                .A(
                                    href: "#SummarySettingsEditor",
                                    text: Displays.SummarySettingsEditor()))
                            .Li(() => hb
                                .A(
                                    href: "#MailerSettingsEditor",
                                    text: Displays.MailerSettingsEditor()));
                    }
                }
                hb.Hidden(controlId: "TableName", value: "Sites");
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteTop(SiteSettings siteSettings)
        {
            var hb = Html.Builder();
            var permissionType = Permissions.Types.Manager;
            var verType = Versions.VerTypes.Latest;
            return hb.Template(
                siteId: 0,
                modelName: "Site",
                title: Displays.Top(),
                permissionType: permissionType,
                verType: verType,
                backUrl: string.Empty,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: true,
                action: () =>
                {
                    hb.Form(
                        attributes: Html.Attributes()
                            .Id_Css("SitesForm", "main-form")
                            .Action(Navigations.ItemAction(0)),
                        action: () => hb
                            .Nav(action: () => hb
                                .Ul(css: "nav-sites sortable", action: () =>
                                    Menu(0).ForEach(siteModelChild => hb
                                        .SiteMenu(
                                            siteSettings: siteSettings,
                                            permissionType: permissionType,
                                            siteId: siteModelChild.SiteId,
                                            referenceType: siteModelChild.ReferenceType,
                                            title: siteModelChild.Title.Value))))
                            .SiteMenuData());
                    hb.MainCommands(
                        siteId: 0,
                        permissionType: permissionType,
                        verType: verType,
                        backUrl: string.Empty);
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteMenu(SiteModel siteModel)
        {
            var hb = Html.Builder();
            var siteSettings = siteModel.SitesSiteSettings();
            return hb.Template(
                siteId: siteModel.SiteId,
                modelName: "Site",
                title: siteModel.Title.Value,
                permissionType: siteModel.PermissionType,
                verType: Versions.VerTypes.Latest,
                backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                methodType: BaseModel.MethodTypes.Index,
                allowAccess:
                    siteModel.PermissionType.CanRead() &&
                    siteModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    hb.Form(
                        attributes: Html.Attributes()
                            .Id_Css("SitesForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .Nav(css: "cf", action: () => hb
                                .Ul(css: "nav-sites", action: () => hb
                                    .ToUpper(siteModel)))
                            .Nav(css: "cf", action: () => hb
                                .Ul(css: "nav-sites sortable", action: () =>
                                    Menu(siteSettings.SiteId).ForEach(siteModelChild => hb
                                        .SiteMenu(
                                            siteSettings: siteSettings,
                                            permissionType: siteModel.PermissionType,
                                            siteId: siteModelChild.SiteId,
                                            referenceType: siteModelChild.ReferenceType,
                                            title: siteModelChild.Title.Value))))
                            .SiteMenuData());
                    if (siteSettings.SiteId != 0)
                    {
                        hb.MainCommands(
                            siteId: siteModel.SiteId,
                            permissionType: siteModel.PermissionType,
                            verType: Versions.VerTypes.Latest,
                            backUrl: Navigations.ItemIndex(siteSettings.ParentId));
                    }
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuData(this HtmlBuilder hb)
        {
            return hb
                .Hidden(attributes: Html.Attributes()
                    .Id("MoveSiteMenu")
                    .DataAction("MoveSiteMenu")
                    .DataMethod("post"))
                .Hidden(attributes: Html.Attributes()
                    .Id("SortSiteMenu")
                    .DataAction("SortSiteMenu")
                    .DataMethod("put"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ToUpper(this HtmlBuilder hb, SiteModel siteModel)
        {
            return siteModel.SiteId != 0
                ? hb.SiteMenu(
                    siteSettings: siteModel.SiteSettings,
                    permissionType: siteModel.PermissionType,
                    siteId: siteModel.ParentId,
                    referenceType: "Sites",
                    title: Displays.ToUpper(),
                    toUpper: true)
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenu(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            long siteId,
            string referenceType,
            string title,
            bool toUpper = false)
        {
            var binaryModel = new BinaryModel(permissionType, siteId);
            var hasImage = binaryModel.ExistsSiteImage(ImageData.SizeTypes.Thumbnail);
            return hb.Li(
                attributes: Html.Attributes()
                    .Class(CssClasses.Get("nav-site " + referenceType.ToLower() +
                        (hasImage
                            ? " has-image"
                            : string.Empty),
                         toUpper
                            ? " to-upper"
                            : string.Empty))
                    .DataId(siteId.ToString())
                    .DataType(referenceType),
                action: () => hb
                    .A(
                        attributes: Html.Attributes()
                            .Href(SiteHref(siteSettings, siteId, referenceType)),
                        action: () =>
                        {
                            if (toUpper)
                            {
                                if (hasImage)
                                {
                                    hb
                                        .Img(
                                            src: Navigations.Get(
                                                "Items",
                                                siteId.ToString(),
                                                "Binaries",
                                                "SiteImageIcon",
                                                binaryModel.SiteImagePrefix(
                                                    ImageData.SizeTypes.Thumbnail)),
                                            css: "site-image-icon")
                                        .Span(css: "title", action: () => hb
                                            .Text(title));
                                }
                                else
                                {
                                    hb.Icon(
                                        iconCss: "ui-icon-circle-arrow-n",
                                        cssText: "title",
                                        text: title);
                                }
                            }
                            else
                            {
                                if (hasImage)
                                {
                                    hb.Img(
                                        src: Navigations.Get(
                                            "Items",
                                            siteId.ToString(),
                                            "Binaries",
                                            "SiteImageThumbnail",
                                            binaryModel.SiteImagePrefix(
                                                ImageData.SizeTypes.Thumbnail)),
                                        css: "site-image-thumbnail");
                                }
                                hb.Span(css: "title", action: () => hb
                                    .Text(title));
                            }
                            if (referenceType == "Sites")
                            {
                                hb.Div(css: "heading");
                            }
                            else
                            {
                                switch (referenceType)
                                {
                                    case "Wikis": break;
                                    default:
                                        hb.Div(css: "stacking1").Div(css: "stacking2");
                                        break;
                                }
                            }
                        }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SiteHref(
            SiteSettings siteSettings, long siteId, string referenceType)
        {
            switch (referenceType)
            {
                case "Wikis": return Navigations.ItemEdit(
                    new WikiModel(siteSettings).Get(where: Rds.WikisWhere().SiteId(siteId))
                        .WikiId);
                default: return Navigations.ItemIndex(siteId);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<SiteModel> Menu(long parentId)
        {
            var siteDataRows = new SiteCollection(where: Rds.SitesWhere()
                .TenantId(Sessions.TenantId())
                .ParentId(parentId)
                .Sub(Rds.ExistsPermissions(where: Rds.PermissionsWhere()
                    .ReferenceType("Sites")
                    .Or(Rds.PermissionsWhere()
                        .UserId(_operator: "=@_U")
                        .DeptId(_operator: "=@_D"))))).Where(o => o.PermissionType.CanRead());
            var orderModel = new OrderModel(parentId, "Sites");
            siteDataRows.ForEach(siteModel =>
            {
                var index = orderModel.Data.IndexOf(siteModel.SiteId);
                siteModel.SiteMenu = (index != -1 ? index : int.MaxValue);
            });
            return siteDataRows.OrderBy(o => o.SiteMenu);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string EditorNew(Permissions.Types permissionType, long siteId)
        {
            return Editor(new SiteModel()
            {
                MethodType = BaseModel.MethodTypes.New,
                SiteId = siteId,
                PermissionType = permissionType
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(SiteModel siteModel)
        {
            var hb = Html.Builder();
            return hb.Template(
                siteId: siteModel.SiteId,
                modelName: "Site",
                title: siteModel.Title + " - " + Displays.EditSettings(),
                permissionType: siteModel.PermissionType,
                verType: siteModel.VerType,
                backUrl: Navigations.ItemIndex(siteModel.SiteId),
                methodType: siteModel.MethodType,
                allowAccess: AllowAccess(siteModel),
                action: () => hb
                    .Editor(siteModel: siteModel)
                    .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                    .Hidden(controlId: "ReferenceType", value: "Sites")).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool AllowAccess(SiteModel siteModel)
        {
            if (siteModel.AccessStatus == Databases.AccessStatuses.NotFound)
            {
                return false;
            }
            switch (siteModel.MethodType)
            {
                case BaseModel.MethodTypes.New:
                    return siteModel.PermissionType.CanCreate();
                default:
                    return siteModel.PermissionType.CanEditSite();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(this HtmlBuilder hb, SiteModel siteModel)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: Html.Attributes()
                        .Id_Css("SiteForm", "main-form")
                        .Action(Navigations.ItemAction(siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            id: siteModel.SiteId,
                            baseModel: siteModel,
                            tableName: "Sites",
                            switcher: false)
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: siteModel.Comments,
                                verType: siteModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(siteModel: siteModel)
                            .Fields(siteModel: siteModel)
                            .MainCommands(
                                siteId: siteModel.SiteId,
                                permissionType: siteModel.PermissionType,
                                verType: siteModel.VerType,
                                backUrl: Navigations.ItemIndex(siteModel.SiteId),
                                referenceType: "items",
                                referenceId: siteModel.SiteId,
                                updateButton: true,
                                copyButton: true,
                                mailButton: true,
                                deleteButton: true))
                        .Hidden(
                            controlId: "Sites_Timestamp",
                            css: "control-hidden must-transport",
                            value: siteModel.Timestamp))
                .OutgoingMailsForm("Sites", siteModel.SiteId, siteModel.Ver)
                .Dialog_Copy("items", siteModel.SiteId)
                .Dialog_Move("items", siteModel.SiteId)
                .Dialog_Histories(Navigations.ItemAction(siteModel.SiteId))
                .Dialog_OutgoingMail());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Fields(this HtmlBuilder hb, SiteModel siteModel)
        {
            hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                hb
                    .FieldText(
                        controlId: "Sites_SiteId",
                        labelText: Displays.Sites_SiteId(),
                        text: siteModel.SiteId.ToString())
                    .FieldText(
                        controlId: "Sites_Ver",
                        labelText: Displays.Sites_Ver(),
                        text: siteModel.Ver.ToString())
                    .FieldTextBox(
                        controlId: "Sites_Title",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_Title(),
                        text: siteModel.Title.Value.ToString())
                    .FieldMarkDown(
                        controlId: "Sites_Body",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_Body(),
                        text: siteModel.Body)
                    .Field(
                        controlId: "Sites_ReferenceType",
                        labelText: Displays.Sites_ReferenceType(),
                        controlAction: () => hb
                            .ReferenceType(
                                selectedValue: siteModel.ReferenceType,
                                methodType: siteModel.MethodType))
                    .VerUpCheckBox(siteModel);
                if (siteModel.PermissionType.CanEditPermission() &&
                    siteModel.MethodType != BaseModel.MethodTypes.New)
                {
                    hb.FieldAnchor(
                        controlContainerCss: "m-l30",
                        iconCss: "ui-icon-person a",
                        text: Displays.EditPermissions(),
                        href: Navigations.ItemEdit(siteModel.SiteId, "Permissions"));
                }
            });
            if (siteModel.MethodType != BaseModel.MethodTypes.New)
            {
                hb.SiteImageSettingsEditor(siteModel.SiteSettings);
                if (siteModel.ReferenceType != "Sites")
                {
                    hb
                        .GridSettingsEditor(siteModel.SiteSettings)
                        .EditorSettingsEditor(siteModel.SiteSettings)
                        .SummarySettingsEditor(siteModel.SiteSettings)
                        .MailerSettingsEditor(siteModel.SiteSettings);
                }
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteImageSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                id: "SiteImageSettingsEditor",
                action: () => hb
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.Icon(),
                        action: () => hb
                            .FieldTextBox(
                                textStyle: HtmlControls.TextStyles.File,
                                controlId: "SiteSettings,SiteImage",
                                fieldCss: "field-auto-thin",
                                controlCss: " w400",
                                labelText: Displays.File())
                            .Button(
                                controlId: "SetSiteImage",
                                controlCss: "button-save",
                                text: Displays.Setting(),
                                onClick: Def.JavaScript.SetSiteImage,
                                action: "binaries/updatesiteimage",
                                method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                id: "GridSettingsEditor",
                action: () => hb
                    .FieldSpinner(
                        controlId: "SiteSettings,NearDeadlineBeforeDays",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SettingNearDeadlineBeforeDays(),
                        value: siteSettings.NearDeadlineBeforeDays.ToDecimal(),
                        min: Parameters.NearDeadlineBeforeDaysMin,
                        max: Parameters.NearDeadlineBeforeDaysMax,
                        step: 1,
                        width: 25)
                    .FieldSpinner(
                        controlId: "SiteSettings,NearDeadlineAfterDays",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SettingNearDeadlineAfterDays(),
                        value: siteSettings.NearDeadlineAfterDays.ToDecimal(),
                        min: Parameters.NearDeadlineAfterDaysMin,
                        max: Parameters.NearDeadlineAfterDaysMax,
                        step: 1,
                        width: 25)
                    .FieldSpinner(
                        controlId: "SiteSettings,GridPageSize",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.SettingGridPageSize(),
                        value: siteSettings.GridPageSize.ToDecimal(),
                        min: Parameters.GridPageSizeMin,
                        max: Parameters.GridPageSizeMax,
                        step: 1,
                        width: 25)
                    .GridColumns(siteSettings)
                    .Aggregations(siteSettings)
                    .Dialog_AggregationDetails(siteSettings));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridColumns(this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.SettingColumns(),
                action: () => hb
                    .FieldSelectable(
                    controlId: "GridColumns",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlCss: " h350",
                    labelText: Displays.SettingColumnList(),
                    listItemCollection: siteSettings.GridColumnHash(),
                    selectedValueCollection: new List<string>(),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "MoveUpGridColumns",
                                controlCss: "button-up",
                                text: Displays.Up(),
                                onClick: Def.JavaScript.Submit,
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                controlId: "MoveDownGridColumns",
                                controlCss: "button-down",
                                text: Displays.Down(),
                                onClick: Def.JavaScript.Submit,
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                controlId: "ShowGridColumns",
                                controlCss: "button-visible",
                                text: Displays.Visible(),
                                onClick: Def.JavaScript.Submit,
                                action: "SetSiteSettings",
                                method: "put")
                            .Button(
                                controlId: "HideGridColumns",
                                controlCss: "button-hide",
                                text: Displays.Hide(),
                                onClick: Def.JavaScript.Submit,
                                action: "SetSiteSettings",
                                method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Aggregations(this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.SettingAggregations(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "AggregationDestination",
                        fieldCss: "field-vertical both",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.SettingGroupByList(),
                        listItemCollection: siteSettings.AggregationDestination(),
                        selectedValueCollection: new List<string>(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpAggregations",
                                    controlCss: "button-up",
                                    text: Displays.Up(),
                                    onClick: Def.JavaScript.Submit,
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveDownAggregations",
                                    controlCss: "button-down",
                                    text: Displays.Down(),
                                    onClick: Def.JavaScript.Submit,
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    text: Displays.AdvancedSetting(),
                                    controlCss: "button-setting open-dialog",
                                    onClick: Def.JavaScript.OpenDialog,
                                    selector: "#Dialog_AggregationDetails")
                                .Button(
                                    controlId: "DeleteAggregations",
                                    controlCss: "button-to-right",
                                    text: Displays.Delete(),
                                    onClick: Def.JavaScript.Submit,
                                    action: "SetSiteSettings",
                                    method: "put")))
                    .FieldSelectable(
                        controlId: "AggregationSource",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlCss: " h350",
                        labelText: Displays.SettingSelectionList(),
                        listItemCollection: siteSettings.AggregationSource(),
                        selectedValueCollection: new List<string>(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "AddAggregations",
                                    controlCss: "button-to-left",
                                    text: Displays.Add(),
                                    onClick: Def.JavaScript.Submit,
                                    action: "SetSiteSettings",
                                    method: "post"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder Dialog_AggregationDetails(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.Div(
                attributes: Html.Attributes()
                    .Id_Css("Dialog_AggregationDetails", "dialog")
                    .Title(Displays.AggregationDetails()),
                action: () => hb
                    .FieldDropDown(
                        controlId: "AggregationType",
                        labelText: Displays.SettingAggregationType(),
                        optionCollection: new Dictionary<string, string>
                        {
                            { "Count", Displays.Count() },
                            { "Total", Displays.Total() },
                            { "Average", Displays.Average() }
                        })
                    .FieldDropDown(
                        controlId: "AggregationTarget",
                        fieldCss: " hidden togglable",
                        labelText: Displays.SettingAggregationTarget(),
                        optionCollection: Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == siteSettings.ReferenceType)
                            .Where(o => o.Computable)
                            .Where(o => o.TypeName != "datetime")
                            .ToDictionary(
                                o => o.ColumnName,
                                o => siteSettings.AllColumn(o.ColumnName).LabelText))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "SetAggregationDetails",
                            text: Displays.Setting(),
                            controlCss: "button-setting",
                            onClick: Def.JavaScript.SetAggregationDetails,
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-cancel",
                            onClick: Def.JavaScript.CancelDialog)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                id: "EditorSettingsEditor",
                action: () =>
                    hb.FieldSet(
                        legendText: Displays.SettingColumns(),
                        css: " enclosed",
                        action: () => hb
                            .FieldSelectable(
                                controlId: "EditorColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlCss: " h350",
                                labelText: Displays.SettingColumnList(),
                                listItemCollection: siteSettings.EditorColumnHash(),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "MoveUpEditorColumns",
                                            text: Displays.Up(),
                                            controlCss: "button-up",
                                            onClick: Def.JavaScript.Submit,
                                            action: "SetSiteSettings",
                                            method: "post")
                                        .Button(
                                            controlId: "MoveDownEditorColumns",
                                            text: Displays.Down(),
                                            controlCss: "button-down",
                                            onClick: Def.JavaScript.Submit,
                                            action: "SetSiteSettings",
                                            method: "post")
                                        .Button(
                                            controlId: "ShowEditorColumns",
                                            text: Displays.Visible(),
                                            controlCss: "button-visible",
                                            onClick: Def.JavaScript.Submit,
                                            action: "SetSiteSettings",
                                            method: "put")
                                        .Button(
                                            controlId: "HideEditorColumns",
                                            text: Displays.Hide(),
                                            controlCss: "button-hide",
                                            onClick: Def.JavaScript.Submit,
                                            action: "SetSiteSettings",
                                            method: "put")
                                        .Button(
                                            controlId: "OpenDialog_ColumnProperties",
                                            text: Displays.AdvancedSetting(),
                                            controlCss: "button-setting",
                                            onClick: Def.JavaScript.OpenDialog_ColumnProperties,
                                            action: "SetSiteSettings",
                                            method: "put")))
                    .Div(attributes: Html.Attributes()
                        .Id_Css("Dialog_ColumnProperties", "dialog")
                        .Title(Displays.AdvancedSetting()))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ColumnProperties(SiteSettings siteSettings, Column column)
        {
            var hb = Html.Builder();
            hb.FieldSet(
                css: " enclosed",
                legendText: column.LabelTextDefault,
                action: () =>
                {
                    hb
                        .FieldTextBox(
                            controlId: "ColumnProperty,LabelText",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.SettingLabel(),
                            text: column.LabelText)
                        .FieldDropDown(
                            controlId: "ColumnProperty,FieldCss",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Style(),
                            optionCollection: new Dictionary<string, string>
                            {
                                { "field-normal", Displays.Normal() },
                                { "field-wide", Displays.Wide() },
                                { "field-auto", Displays.Auto() }
                            },
                            selectedValue: column.FieldCss);
                    if (column.Nullable)
                    {
                        hb.FieldCheckBox(
                            controlId: "ColumnProperty,EditorReadOnly",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.ReadOnly(),
                            _checked: column.EditorReadOnly.ToBool());
                    }
                    if (column.TypeName == "datetime")
                    {
                        hb
                            .FieldDropDown(
                                controlId: "ColumnProperty,GridDateTime",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.SettingGridDateTime(),
                                optionCollection: DateTimeOptions(),
                                selectedValue: column.GridDateTime)
                            .FieldDropDown(
                                controlId: "ColumnProperty,ControlDateTime",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.SettingControlDateTime(),
                                optionCollection: DateTimeOptions(),
                                selectedValue: column.ControlDateTime);
                    }
                    if (column.ColumnName == "Title")
                    {
                        hb
                            .FieldTextBox(
                                controlId: "SiteSettings,TitleSeparator",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.SettingTitleSeparator(),
                                text: siteSettings.TitleSeparator)
                            .FieldSelectable(
                                controlId: "TitleColumns",
                                fieldCss: "field-vertical both",
                                controlContainerCss: "container-selectable",
                                controlCss: " h350",
                                labelText: Displays.SettingTitleColumn(),
                                listItemCollection: siteSettings.TitleColumnHash(),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "MoveUpTitleColumns",
                                            text: Displays.Up(),
                                            controlCss: "button-up",
                                            onClick: Def.JavaScript.Submit,
                                            action: "SetSiteSettings",
                                            method: "post")
                                        .Button(
                                            controlId: "MoveDownTitleColumns",
                                            text: Displays.Down(),
                                            controlCss: "button-down",
                                            onClick: Def.JavaScript.Submit,
                                            action: "SetSiteSettings",
                                            method: "post")
                                        .Button(
                                            controlId: "ShowTitleColumns",
                                            text: Displays.Visible(),
                                            controlCss: "button-visible",
                                            onClick: Def.JavaScript.Submit,
                                            action: "SetSiteSettings",
                                            method: "put")
                                        .Button(
                                            controlId: "HideTitleColumns",
                                            text: Displays.Hide(),
                                            controlCss: "button-hide",
                                            onClick: Def.JavaScript.Submit,
                                            action: "SetSiteSettings",
                                            method: "put")
                                        .Button(
                                            controlId: "OpenDialog_ColumnProperties",
                                            text: Displays.AdvancedSetting(),
                                            controlCss: "button-setting",
                                            onClick: Def.JavaScript.OpenDialog_ColumnProperties,
                                            action: "SetSiteSettings",
                                            method: "put")));
                    }
                    switch (column.TypeName.CsTypeSummary())
                    {
                        case Types.CsNumeric:
                            hb
                                .FieldTextBox(
                                    controlId: "ColumnProperty,Unit",
                                    fieldCss: "field-auto-thin",
                                    controlCss: " w50",
                                    labelText: Displays.SettingUnit(),
                                    text: column.Unit)
                                .FieldSpinner(
                                    controlId: "ColumnProperty,DecimalPlaces",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.DecimalPlaces(),
                                    value: column.DecimalPlaces.ToDecimal(),
                                    min: 0,
                                    max: 3,
                                    step: 1);
                            if (!column.NotUpdate)
                            {
                                var hidden = column.ControlType != "Spinner"
                                    ? " hidden"
                                    : string.Empty;
                                hb
                                    .FieldDropDown(
                                        controlId: "ColumnProperty,ControlType",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.ControlType(),
                                        optionCollection: new Dictionary<string, string>
                                        {
                                            { "Normal", Displays.Normal() },
                                            { "Spinner", Displays.Spinner() }
                                        },
                                        selectedValue: column.ControlType)
                                    .FieldTextBox(
                                        fieldId: "Field_ColumnProperty,Min",
                                        controlId: "ColumnProperty,Min",
                                        fieldCss: "field-auto-thin both" + hidden,
                                        labelText: Displays.Min(),
                                        text: column.Format(column.Min.ToDecimal()))
                                    .FieldTextBox(
                                        fieldId: "Field_ColumnProperty,Max",
                                        controlId: "ColumnProperty,Max",
                                        fieldCss: "field-auto-thin" + hidden,
                                        labelText: Displays.Max(),
                                        text: column.Format(column.Max.ToDecimal()))
                                    .FieldTextBox(
                                        fieldId: "Field_ColumnProperty,Step",
                                        controlId: "ColumnProperty,Step",
                                        fieldCss: "field-auto-thin" + hidden,
                                        labelText: Displays.Step(),
                                        text: column.Format(column.Step.ToDecimal()));
                            }
                            break;
                    }
                    switch (column.ControlType)
                    {
                        case "ChoicesText":
                            hb.TextBox(
                                textStyle: HtmlControls.TextStyles.MultiLine,
                                controlId: "ColumnProperty,ChoicesText",
                                controlCss: " choices",
                                placeholder: Displays.SettingSelectionList(),
                                text: column.ChoicesText);
                            break;
                        case "Limit":
                            hb.FieldSpinner(
                                controlId: "ColumnProperty,DefaultInput",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.SettingLimitDefault(),
                                value: column.DefaultInput.ToDecimal(),
                                min: column.Min.ToInt(),
                                max: column.Max.ToInt(),
                                step: column.Step.ToInt(),
                                width: column.Width);
                            break;
                        default:
                            break;
                    }
                });
            return hb
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetColumnProperties",
                        text: Displays.Setting(),
                        controlCss: "button-setting",
                        onClick: Def.JavaScript.CloseDialogAndSubmit,
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        text: Displays.Cancel(),
                        controlCss: "button-cancel",
                        onClick: Def.JavaScript.CancelDialog));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> DateTimeOptions()
        {
            return Def.DisplayDefinitionCollection
                .Where(o => o.Type == "Date")
                .Where(o => o.Language == string.Empty)
                .ToDictionary(o => o.Id, o => Displays.Get(o.Id));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummarySettingsEditor(
            this HtmlBuilder hb,
            SiteSettings siteSettings)
        {
            var siteDataRows = siteSettings.SummarySiteDataRows();
            var summarySiteIdHash = SummarySiteIdHash(siteDataRows, siteSettings);
            var firstSiteId = summarySiteIdHash.Select(o => o.Key.ToLong()).FirstOrDefault();
            return siteDataRows.Count() > 0
                ? hb.FieldSet(
                    id: "SummarySettingsEditor",
                    action: () =>
                        hb.FieldSet(
                            legendText: Displays.SettingColumns(),
                            css: " enclosed",
                            action: () => hb
                                .FieldDropDown(
                                    controlId: "SummarySiteId",
                                    controlCss: " auto-postback",
                                    labelText: Displays.SummarySiteId(),
                                    optionCollection: summarySiteIdHash,
                                    action: "SetSiteSettings",
                                    method: "post")
                                .SummaryDestinationColumn(
                                    siteId: firstSiteId,
                                    referenceType: siteSettings.ReferenceType,
                                    siteDataRows: siteDataRows)
                                .SummaryLinkColumn(
                                    siteId: firstSiteId,
                                    siteSettings: siteSettings)
                                .FieldDropDown(
                                    controlId: "SummaryType",
                                    controlCss: " auto-postback",
                                    labelText: Displays.SummaryType(),
                                    optionCollection: SummaryTypeCollection(),
                                    action: "SetSiteSettings",
                                    method: "post")
                                .SummarySourceColumn(siteSettings)
                                .FieldContainer(actionOptions: () => hb
                                    .Div(css: "buttons", action: () => hb
                                        .Button(
                                            controlId: "AddSummary",
                                            text: Displays.Add(),
                                            controlCss: "button-create",
                                            onClick: Def.JavaScript.AddSummary,
                                            action: "SetSiteSettings",
                                            method: "put")))
                                .SummarySettings(sourceSiteSettings: siteSettings)))
                : hb.FieldSet(
                    id: "SummarySettingsEditor",
                    action: () => hb
                        .P(action: () => hb
                            .Text(text: Displays.NoLinks())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryDestinationColumn(
            this HtmlBuilder hb,
            long siteId,
            string referenceType,
            EnumerableRowCollection<DataRow> siteDataRows)
        {
            return hb.FieldDropDown(
                fieldId: "Field_SummaryDestinationColumn",
                controlId: "SummaryDestinationColumn",
                labelText: Displays.SummaryDestinationColumn(),
                optionCollection: SummaryDestinationColumnCollection(
                    siteDataRows, siteId, referenceType),
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> SummaryDestinationColumnCollection(
            EnumerableRowCollection<DataRow> siteDataRows,
            long siteId,
            string referenceType)
        {
            return siteDataRows
                .Where(o => o["SiteId"].ToLong() == siteId)
                .Select(o => (
                    o["SiteSettings"].ToString().Deserialize<SiteSettings>() ??
                    SiteSettingsUtility.Get(siteId, referenceType)).ColumnCollection)
                .FirstOrDefault()?
                .Where(o => o.Computable)
                .Where(o => !o.NotUpdate)
                .OrderBy(o => o.No)
                .ToDictionary(
                    o => o.ColumnName,
                    o => o.LabelText);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> SummarySiteIdHash(
            EnumerableRowCollection<DataRow> summarySiteCollection,
            SiteSettings siteSettings)
        {
            return summarySiteCollection
                .OrderBy(o =>
                    o["SiteId"].ToLong() != siteSettings.SiteId)
                .ToDictionary(
                    o => o["SiteId"].ToString(),
                    o => o["Title"].ToString());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> SummaryTypeCollection()
        {
            return new Dictionary<string, string>
            {
                { "Count", Displays.Count() },
                { "Total", Displays.Total() },
                { "Average", Displays.Average() },
                { "Max", Displays.Max() },
                { "Min", Displays.Min() }
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryLinkColumn(
            this HtmlBuilder hb,
            long siteId,
            SiteSettings siteSettings)
        {
            return hb.FieldDropDown(
                fieldId: "Field_SummaryLinkColumn",
                controlId: "SummaryLinkColumn",
                labelText: Displays.SummaryLinkColumn(),
                optionCollection: siteSettings.LinkColumnSiteIdHash
                    .Where(o => o.Value == siteId)
                    .ToDictionary(
                        o => o.Key.Split('_')._1st(),
                        o => siteSettings.AllColumn(o.Key.Split('_')._1st()).LabelText),
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummarySourceColumn(
            this HtmlBuilder hb, SiteSettings siteSettings, string type = "Count")
        {
            switch (type)
            {
                case "Count":
                    return hb.FieldContainer(
                        fieldId: "Field_SummarySourceColumn",
                        fieldCss: " hidden");
                default:
                    return hb.FieldDropDown(
                        fieldId: "Field_SummarySourceColumn",
                        controlId: "SummarySourceColumn",
                        labelText: Displays.SummarySourceColumn(),
                        optionCollection: siteSettings.ColumnCollection
                            .Where(o => o.Computable)
                            .ToDictionary(o => o.ColumnName, o => o.LabelText),
                        action: "SetSiteSettings",
                        method: "post");
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummarySettings(
            this HtmlBuilder hb, SiteSettings sourceSiteSettings)
        {
            return hb.Div(id: "SummarySettings", action: () =>
            {
                hb.Table(css: "grid", action: () =>
                {
                    hb.Tr(css: "ui-widget-header", action: () => hb
                        .Th(action: () => hb
                            .Text(Displays.SummarySiteId()))
                        .Th(action: () => hb
                            .Text(Displays.SummaryDestinationColumn()))
                        .Th(action: () => hb
                            .Text(Displays.SummaryLinkColumn()))
                        .Th(action: () => hb
                            .Text(Displays.SummaryType()))
                        .Th(action: () => hb
                            .Text(Displays.SummarySourceColumn()))
                        .Th(action: () => hb
                            .Text(Displays.Operations())));
                    if (sourceSiteSettings.SummaryCollection.Count > 0)
                    {
                        var dataRows = Rds.ExecuteTable(statements:
                            Rds.SelectSites(
                                column: Rds.SitesColumn()
                                    .SiteId()
                                    .ReferenceType()
                                    .Title()
                                    .SiteSettings(),
                                where: Rds.SitesWhere()
                                    .TenantId(Sessions.TenantId())
                                    .SiteId_In(sourceSiteSettings.SummaryCollection
                                        .Select(o => o.SiteId)))).AsEnumerable();
                        sourceSiteSettings.SummaryCollection.ForEach(summary =>
                        {
                            var dataRow = dataRows.FirstOrDefault(o =>
                                o["SiteId"].ToLong() == summary.SiteId);
                            var destinationSiteSettings = dataRow["SiteSettings"]
                                .ToString()
                                .Deserialize<SiteSettings>() ??
                                    SiteSettingsUtility.Get(
                                        dataRow["SiteId"].ToLong(),
                                        dataRow["ReferenceType"].ToString());
                            if (destinationSiteSettings != null)
                            {
                                hb.Tr(css: "grid-row not-link", action: () => hb
                                    .Td(action: () => hb
                                        .Text(dataRow["Title"].ToString()))
                                    .Td(action: () => hb
                                        .Text(destinationSiteSettings.AllColumn(
                                            summary.DestinationColumn)?.LabelText))
                                    .Td(action: () => hb
                                        .Text(sourceSiteSettings.AllColumn(
                                            summary.LinkColumn)?.LabelText))
                                    .Td(action: () => hb
                                        .Text(SummaryType(summary.Type)))
                                    .Td(action: () => hb
                                        .Text(sourceSiteSettings.AllColumn(
                                            summary.SourceColumn)?.LabelText))
                                    .Td(action: () => hb
                                        .Button(
                                            controlId: "SynchronizeSummary," + summary.Id,
                                            controlCss: "button-synchronize",
                                            text: Displays.Synchronize(),
                                            onClick: Def.JavaScript.Submit,
                                            action: "SynchronizeSummary",
                                            method: "put",
                                            confirm: Displays.ConfirmSynchronize())
                                        .Button(
                                            controlId: "DeleteSummary," + summary.Id,
                                            controlCss: "button-delete",
                                            text: Displays.Delete(),
                                            onClick: Def.JavaScript.Submit,
                                            action: "SetSiteSettings",
                                            method: "delete")));
                            }
                        });
                    }
                });
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SummaryType(string type)
        {
            switch (type)
            {
                case "Count": return Displays.Count();
                case "Total": return Displays.Total();
                case "Average": return Displays.Average();
                case "Max": return Displays.Max();
                case "Min": return Displays.Min();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MailerSettingsEditor(
            this HtmlBuilder hb, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                id: "MailerSettingsEditor",
                action: () => hb
                    .FieldTextBox(
                        textStyle: HtmlControls.TextStyles.MultiLine,
                        controlId: "SiteSettings,AddressBook",
                        fieldCss: "field-wide",
                        labelText: Displays.AddressBook(),
                        text: siteSettings.AddressBook.ToStr())
                    .FieldSet(
                        legendText: Displays.DefaultDestinations(),
                        css: " enclosed-thin",
                        action: () => hb
                            .FieldTextBox(
                                textStyle: HtmlControls.TextStyles.MultiLine,
                                controlId: "SiteSettings,MailToDefault",
                                fieldCss: "field-wide",
                                labelText: Displays.OutgoingMails_To(),
                                text: siteSettings.MailToDefault.ToStr())
                            .FieldTextBox(
                                textStyle: HtmlControls.TextStyles.MultiLine,
                                controlId: "SiteSettings,MailCcDefault",
                                fieldCss: "field-wide",
                                labelText: Displays.OutgoingMails_Cc(),
                                text: siteSettings.MailCcDefault.ToStr())
                            .FieldTextBox(
                                textStyle: HtmlControls.TextStyles.MultiLine,
                                controlId: "SiteSettings,MailBccDefault",
                                fieldCss: "field-wide",
                                labelText: Displays.OutgoingMails_Bcc(),
                                text: siteSettings.MailBccDefault.ToStr())));
        }
    }
}
