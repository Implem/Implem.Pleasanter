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
        public List<string> MonitorChangesColumns = null;
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
        public List<string> SavedMonitorChangesColumns = null;
        public bool TenantId_Updated { get { return TenantId != SavedTenantId; } }
        public bool ReferenceType_Updated { get { return ReferenceType != SavedReferenceType && ReferenceType != null; } }
        public bool ParentId_Updated { get { return ParentId != SavedParentId; } }
        public bool InheritPermission_Updated { get { return InheritPermission != SavedInheritPermission; } }
        public bool SiteSettings_Updated { get { return SiteSettings.RecordingJson() != SavedSiteSettings && SiteSettings.RecordingJson() != null; } }

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

        public List<string> Session_MonitorChangesColumns()
        {
            return this.PageSession("MonitorChangesColumns") != null
                ? this.PageSession("MonitorChangesColumns") as List<string> ?? new List<string>()
                : MonitorChangesColumns;
        }

        public void  Session_MonitorChangesColumns(object value)
        {
            this.PageSession("MonitorChangesColumns", value);
        }

        public string PropertyValue(string name)
        {
            switch (name)
            {
                case "TenantId": return TenantId.ToString();
                case "SiteId": return SiteId.ToString();
                case "UpdatedTime": return UpdatedTime.Value.ToString();
                case "Ver": return Ver.ToString();
                case "Title": return Title.Value;
                case "Body": return Body;
                case "TitleBody": return TitleBody.ToString();
                case "ReferenceType": return ReferenceType;
                case "ParentId": return ParentId.ToString();
                case "InheritPermission": return InheritPermission.ToString();
                case "PermissionType": return PermissionType.ToLong().ToString();
                case "SiteSettings": return SiteSettings.RecordingJson();
                case "Ancestors": return Ancestors.ToString();
                case "PermissionSourceCollection": return PermissionSourceCollection.ToString();
                case "PermissionDestinationCollection": return PermissionDestinationCollection.ToString();
                case "SiteMenu": return SiteMenu.ToString();
                case "MonitorChangesColumns": return MonitorChangesColumns.ToString();
                case "Comments": return Comments.ToJson();
                case "Creator": return Creator.Id.ToString();
                case "Updator": return Updator.Id.ToString();
                case "CreatedTime": return CreatedTime.Value.ToString();
                case "VerUp": return VerUp.ToString();
                case "Timestamp": return Timestamp;
                default: return null;
            }
        }

        public List<long> SwitchTargets;

        public SiteModel()
        {
        }

        public SiteModel(
            long parentId,
            long inheritPermission,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = this.SitesSiteSettings();
            ParentId = parentId;
            InheritPermission = inheritPermission;
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
            SiteInfo.SetSiteUserIdCollection(SiteId);
        }

        public void ClearSessions()
        {
            Session_SiteSettings(null);
            Session_PermissionSourceCollection(null);
            Session_PermissionDestinationCollection(null);
            Session_MonitorChangesColumns(null);
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
                column: column ?? Rds.SitesDefaultColumns(),
                join: join ??  Rds.SitesJoinDefault(),
                where: where ?? Rds.SitesWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            SetSiteSettingsProperties();
            return this;
        }

        public Error.Types Update(SqlParamCollection param = null, bool paramAll = false)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateSites(
                        verUp: VerUp,
                        where: Rds.SitesWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: param ?? Rds.SitesParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return Error.Types.UpdateConflicts;
            Get();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                    where: Rds.ItemsWhere().ReferenceId(SiteId),
                    param: Rds.ItemsParam()
                        .SiteId(SiteId)
                        .Title(SiteUtilities.TitleDisplayValue(SiteSettings, this))
                        .Subset(Jsons.ToJson(new SiteSubset(this, SiteSettings)))
                        .MaintenanceTarget(true)),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(SiteId)),
                    LinkUtilities.Insert(SiteSettings.LinkColumnSiteIdHash
                        .Select(o => o.Value)
                        .Distinct()
                        .ToDictionary(o => o, o => SiteId))
                });
            SiteInfo.SiteMenu.Set(SiteId);
            return Error.Types.None;
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
            var siteMenu = SiteInfo.SiteMenu.Children(SiteId, withParent: true);
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere().SiteId_In(siteMenu.Select(o => o.SiteId))),
                    Rds.DeleteIssues(
                        where: Rds.IssuesWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Issues")
                            .Select(o => o.SiteId))),
                    Rds.DeleteResults(
                        where: Rds.ResultsWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Results")
                            .Select(o => o.SiteId))),
                    Rds.DeleteWikis(
                        where: Rds.WikisWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Wikis")
                            .Select(o => o.SiteId))),
                    Rds.DeleteSites(
                        where: Rds.SitesWhere()
                            .TenantId(TenantId)
                            .SiteId_In(siteMenu.Select(o => o.SiteId)))
                });
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
            var responseCollection = new SitesResponseCollection(this);
            SiteInfo.SiteMenu.RemoveAll((key, value) => key == SiteId);
            if (redirect)
            {
                responseCollection.Href(Navigations.ItemIndex(ParentId));
            }
            return responseCollection.ToJson();
        }

        public string Restore(long siteId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SiteId = siteId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
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
                        new SiteCollection(
                            where: Rds.SitesWhere().SiteId(SiteId),
                            orderBy: Rds.SitesOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(siteModel => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(siteModel.Ver)
                                            .DataLatest(1, _using: siteModel.Ver == Ver),
                                        action: () =>
                                            SiteSettings.HistoryColumnCollection()
                                                .ForEach(column => hb
                                                    .TdValue(column, siteModel))))));
            return new SitesResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
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
                : Versions.VerTypes.History;
            SwitchTargets = SiteUtilities.GetSwitchTargets(SiteSettings, SiteId);
            return Editor();
        }

        public string EditorJson()
        {
            return EditorJson(this);
        }

        private string EditorJson(SiteModel siteModel, Message message = null)
        {
            siteModel.MethodType = MethodTypes.Edit;
            return new SitesResponseCollection(this)
                .Invoke("clearDialogs")
                .ReplaceAll(
                    "#MainContainer",
                    siteModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? SiteUtilities.Editor(siteModel)
                        : SiteUtilities.Editor(this))
                .Invoke("setCurrentIndex")
                .Invoke("validateSites")
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
            SetSiteSettings();
        }

        private void SetBySession()
        {
            if (!Forms.HasData("Sites_SiteSettings")) SiteSettings = Session_SiteSettings();
            if (!Forms.HasData("Sites_PermissionSourceCollection")) PermissionSourceCollection = Session_PermissionSourceCollection();
            if (!Forms.HasData("Sites_PermissionDestinationCollection")) PermissionDestinationCollection = Session_PermissionDestinationCollection();
            if (!Forms.HasData("Sites_MonitorChangesColumns")) MonitorChangesColumns = Session_MonitorChangesColumns();
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
                    case "SiteSettings": SiteSettings = GetSiteSettings(dataRow); SavedSiteSettings = SiteSettings.RecordingJson(); break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
            if (SiteSettings != null)
            {
                Title.DisplayValue = SiteUtilities.TitleDisplayValue(SiteSettings, this);
            }
        }

        private string Editor()
        {
            return new SitesResponseCollection(this)
                .ReplaceAll(
                    "#MainContainer",
                    SiteUtilities.Editor(this))
                .Invoke("validateSites")
                .ToJson();
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
        public Error.Types Create(bool paramAll = false)
        {
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
                            .InheritPermission(raw: InheritPermission == 0
                                ? Def.Sql.Identity
                                : InheritPermission.ToString())
                            .SiteSettings(SiteSettings.RecordingJson())
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
                    break;
            }
            return Error.Types.None;
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
        public void SetSiteSettingsPropertiesBySession()
        {
            SiteSettings = Session_SiteSettings();
            SetSiteSettingsProperties();
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
        public string SetSiteSettings()
        {
            var error = ValidateBeforeSetSiteSettings();
            if (error != null) return error;
            var responseCollection = new SitesResponseCollection(this);
            SetSiteSettingsPropertiesBySession();
            SetSiteSettings(responseCollection);
            Session_SiteSettings(SiteSettings.ToJson());
            return responseCollection.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteSettings(ResponseCollection responseCollection)
        {
            var controlId = Forms.ControlId();
            switch (controlId)
            {
                case "MoveUpGridColumns":
                case "MoveDownGridColumns":
                case "ToDisableGridColumns":
                case "ToEnableGridColumns":
                    SetGridColumns(responseCollection, controlId);
                    break;
                case "MoveUpFilterColumns":
                case "MoveDownFilterColumns":
                case "ToDisableFilterColumns":
                case "ToEnableFilterColumns":
                    SetFilterColumns(responseCollection, controlId);
                    break;
                case "AddAggregations":
                case "DeleteAggregations":
                case "MoveUpAggregations":
                case "MoveDownAggregations":
                    SetAggregations(responseCollection, controlId);
                    break;
                case "SetAggregationDetails":
                    SetAggregationDetails(responseCollection);
                    break;
                case "SummarySiteId":
                    SetSummarySiteId(responseCollection);
                    break;
                case "SummaryType":
                    SetSummaryType(responseCollection);
                    break;
                case "AddSummary":
                    AddSummary(responseCollection);
                    break;
                case "DeleteSummary":
                    DeleteSummary(responseCollection);
                    break;
                case "MoveUpEditorColumns":
                case "MoveDownEditorColumns":
                case "ToDisableEditorColumns":
                case "ToEnableEditorColumns":
                    SetEditorColumns(responseCollection, controlId);
                    break;
                case "SetColumnProperties":
                    SetColumnProperties(responseCollection);
                    break;
                case "MoveUpTitleColumns":
                case "MoveDownTitleColumns":
                case "ToDisableTitleColumns":
                case "ToEnableTitleColumns":
                    SetTitleColumns(responseCollection, controlId);
                    break;
                case "MoveUpLinkColumns":
                case "MoveDownLinkColumns":
                case "ToDisableLinkColumns":
                case "ToEnableLinkColumns":
                    SetLinkColumns(responseCollection, controlId);
                    break;
                case "MoveUpHistoryColumns":
                case "MoveDownHistoryColumns":
                case "ToDisableHistoryColumns":
                case "ToEnableHistoryColumns":
                    SetHistoryColumns(responseCollection, controlId);
                    break;
                case "MoveUpFormulas":
                case "MoveDownFormulas":
                    SetFormulas(responseCollection, controlId);
                    break;
                case "AddFormula":
                    AddFormula(responseCollection);
                    break;
                case "DeleteFormulas":
                    DeleteFormulas(responseCollection);
                    break;
                case "OpenColumnPropertiesDialog":
                    OpenColumnPropertiesDialog(responseCollection);
                    break;
                case "NewNotification":
                case "EditNotification":
                    OpenNotificationDialog(responseCollection, controlId);
                    break;
                case "CreateNotification":
                    CreateNotification(responseCollection, controlId);
                    break;
                case "UpdateNotification":
                    UpdateNotification(responseCollection, controlId);
                    break;
                case "DeleteNotification":
                    DeleteNotification(responseCollection, controlId);
                    break;
                case "MoveUpMonitorChangesColumns":
                case "MoveDownMonitorChangesColumns":
                case "ToDisableMonitorChangesColumns":
                case "ToEnableMonitorChangesColumns":
                    SetMonitorChangesColumns(responseCollection, controlId);
                    break;
                default:
                    Forms.All()
                        .Where(o => o.Key.StartsWith("SiteSettings,"))
                        .ForEach(data =>
                            SiteSettings.Set(data.Key.Split_2nd(), data.Value));
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string ValidateBeforeSetSiteSettings()
        {
            foreach(var data in Forms.All())
            {
                switch (data.Key)
                {
                    case "ColumnProperty,Format":
                        try
                        {
                            0.ToString(data.Value, Sessions.CultureInfo());
                        }
                        catch (Exception)
                        {
                            return Messages.ResponseBadFormat(data.Value).ToJson();
                        }
                        break;
                }
            }
            return null;
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
        public string MoveSiteMenu(long sourceId)
        {
            var destinationId = Forms.Long("DestinationId");
            var toParent = SiteId != 0 && SiteInfo.SiteMenu.Get(SiteId).ParentId == destinationId;
            if (SiteId != 0 && !PermissionType.CanEditSite())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            Rds.ExecuteNonQuery(statements: Rds.UpdateSites(
                where: Rds.SitesWhere()
                    .TenantId(Sessions.TenantId())
                    .SiteId(sourceId),
                param: Rds.SitesParam().ParentId(destinationId)));
            SiteInfo.SiteMenu.Set(sourceId);
            var responseCollection = new ResponseCollection()
                .Remove(".nav-site[data-id=\"" + sourceId + "\"]");
            return toParent
                ? responseCollection.ToJson()
                : responseCollection
                    .ReplaceAll(
                        "[data-id=\"" + destinationId + "\"]",
                        ReplaceSiteMenu(sourceId, destinationId))
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private HtmlBuilder ReplaceSiteMenu(long sourceId, long destinationId)
        {
            return new HtmlBuilder().SiteMenu(
                siteSettings: SiteSettings,
                permissionType: Permissions.Types.Manager,
                siteId: destinationId,
                referenceType: ReferenceType,
                title: SiteInfo.SiteMenu.Get(destinationId).Title,
                siteConditions: SiteInfo.SiteMenu.SiteConditions(SiteId));
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
        private void OpenColumnPropertiesDialog(ResponseCollection responseCollection)
        {
            var selectedColumns = Forms.List("EditorColumns");
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
                    responseCollection.Html(
                        "#ColumnPropertiesDialog",
                        SiteUtilities.ColumnProperties(SiteSettings, column));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenNotificationDialog(ResponseCollection responseCollection, string controlId)
        {
            var notification = controlId == "EditNotification"
                ? SiteSettings.Notifications.FirstOrDefault(
                    o => o.Type == (Notification.Types)Forms.Int("NotificationType"))
                : new Notification(
                    Notification.Types.Mail,
                    string.Empty,
                    ColumnUtilities.MonitorChangesDefinitions(ReferenceType, enableOnly: true)
                        .Select(o => o.ColumnName)
                        .ToList());
            Session_MonitorChangesColumns(notification.MonitorChangesColumns);
            responseCollection
                .Html("#NotificationDialog", SiteUtilities.NotificationDialog(
                    siteSettings: SiteSettings,
                    controlId: Forms.ControlId(),
                    notification: notification))
                .Invoke("validateSites");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetGridColumns(ResponseCollection responseCollection, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("GridColumns");
            var selectedSourceColumns = Forms.List("GridSourceColumns");
            SiteSettings.SetGridColumns(command, selectedColumns, selectedSourceColumns);
            SetResponseAfterChangeColumns(
                responseCollection,
                command,
                "Grid",
                SiteSettings.GridSelectableOptions(),
                selectedColumns,
                SiteSettings.GridSelectableOptions(enabled: false),
                selectedSourceColumns);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFilterColumns(ResponseCollection responseCollection, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("FilterColumns");
            var selectedSourceColumns = Forms.List("FilterSourceColumns");
            SiteSettings.SetFilterColumns(command, selectedColumns, selectedSourceColumns);
            SetResponseAfterChangeColumns(
                responseCollection,
                command,
                "Filter",
                SiteSettings.FilterSelectableOptions(),
                selectedColumns,
                SiteSettings.FilterSelectableOptions(enabled: false),
                selectedSourceColumns);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CreateNotification(ResponseCollection responseCollection, string controlId)
        {
            var type = (Notification.Types)Forms.Int("NotificationType");
            var notification = GetNotification(type);
            if (notification != null)
            {
                responseCollection.Message(Messages.AlreadyAdded());
            }
            else
            {
                SiteSettings.Notifications.Add(new Notification(
                    type, Forms.Data("NotificationAddress"),
                    Session_MonitorChangesColumns()));
                SetNotificationsResponseCollection(responseCollection);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateNotification(ResponseCollection responseCollection, string controlId)
        {
            var notification = GetNotification((Notification.Types)Forms.Int("NotificationType"));
            if (notification == null)
            {
                responseCollection.Message(Messages.NotFound());
            }
            else
            {
                notification.Update(
                    Forms.Data("NotificationAddress"),
                    Session_MonitorChangesColumns());
                SetNotificationsResponseCollection(responseCollection);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteNotification(ResponseCollection responseCollection, string controlId)
        {
            var notification = GetNotification((Notification.Types)Forms.Int("NotificationType"));
            if (notification == null)
            {
                responseCollection.Message(Messages.NotFound());
            }
            else
            {
                SiteSettings.Notifications.Remove(notification);
                SetNotificationsResponseCollection(responseCollection);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private Notification GetNotification(Notification.Types type)
        {
            return SiteSettings.Notifications.FirstOrDefault(o => o.Type == type);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetNotificationsResponseCollection(ResponseCollection responseCollection)
        {
            responseCollection
                .ReplaceAll(
                    "#NotificationSettings",
                    new HtmlBuilder().NotificationSettings(siteSettings: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetMonitorChangesColumns(
            ResponseCollection responseCollection, string controlId)
        {
            var notification = GetNotification((Notification.Types)Forms.Int("NotificationType"));
            MonitorChangesColumns = Session_MonitorChangesColumns();
            if (notification == null && controlId == "EditNotification")
            {
                responseCollection.Message(Messages.NotFound());
            }
            else
            {
                var command = ColumnUtilities.ChangeCommand(controlId);
                var selectedColumns = Forms.List("MonitorChangesColumns");
                var selectedSourceColumns = Forms.List("MonitorChangesSourceColumns");
                MonitorChangesColumns = ColumnUtilities.GetChanged(
                    MonitorChangesColumns,
                    ColumnUtilities.ChangeCommand(controlId),
                    selectedColumns,
                    selectedSourceColumns);
                SetResponseAfterChangeColumns(
                    responseCollection,
                    command,
                    "MonitorChanges",
                    SiteSettings.MonitorChangesSelectableOptions(
                        MonitorChangesColumns),
                    selectedColumns,
                    SiteSettings.MonitorChangesSelectableOptions(
                        MonitorChangesColumns, enabled: false),
                    selectedSourceColumns);
                Session_MonitorChangesColumns(MonitorChangesColumns);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetAggregations(ResponseCollection responseCollection, string controlId)
        {
            var selectedColumns = Forms.List("AggregationDestination");
            var selectedSourceColumns = Forms.List("AggregationSource");
            if (selectedColumns.Any() || selectedSourceColumns.Any())
            {
                SiteSettings.SetAggregations(
                    controlId,
                    selectedColumns,
                    selectedSourceColumns);
                responseCollection
                    .Html("#AggregationDestination", new HtmlBuilder()
                        .SelectableItems(
                            listItemCollection: SiteSettings.AggregationDestination(),
                            selectedValueTextCollection: selectedColumns))
                    .SetFormData("AggregationDestination", selectedColumns?.Join(";"));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetAggregationDetails(ResponseCollection responseCollection)
        {
            Aggregation.Types type;
            Enum.TryParse(Forms.Data("AggregationType"), out type);
            var target = type != Aggregation.Types.Count
                ? Forms.Data("AggregationTarget")
                : string.Empty;
            var selectedColumns = Forms.List("AggregationDestination");
            var selectedSourceColumns = Forms.List("AggregationSource");
            if (selectedColumns.Any() || selectedSourceColumns.Any())
            {
                SiteSettings.SetAggregationDetails(
                    type,
                    target,
                    selectedColumns,
                    selectedSourceColumns);
                responseCollection
                    .Html("#AggregationDestination", new HtmlBuilder()
                        .SelectableItems(
                            listItemCollection: SiteSettings.AggregationDestination(),
                            selectedValueTextCollection: selectedColumns))
                    .SetFormData("AggregationDestination", selectedColumns?.Join(";"));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummarySiteId(ResponseCollection responseCollection)
        {
            var destinationSiteId = Forms.Long("SummarySiteId");
            var destinationSiteSettings = new SiteModel(destinationSiteId);
            var siteDataRows = SiteSettings.SummarySiteDataRows();
            responseCollection
                .ReplaceAll("#SummaryDestinationColumnField", new HtmlBuilder()
                    .SummaryDestinationColumn(
                        referenceType: destinationSiteSettings.ReferenceType,
                        siteId: destinationSiteSettings.SiteId,
                        siteDataRows: siteDataRows))
                .ReplaceAll("#SummaryLinkColumnField", new HtmlBuilder()
                    .SummaryLinkColumn(
                        siteId: destinationSiteId,
                        siteSettings: SiteSettings));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummaryType(ResponseCollection responseCollection)
        {
            responseCollection.ReplaceAll("#SummarySourceColumnField", new HtmlBuilder()
                .SummarySourceColumn(SiteSettings, Forms.Data("SummaryType")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddSummary(ResponseCollection responseCollection)
        {
            var error = SiteSettings.AddSummary(
                Forms.Long("SummarySiteId"),
                new SiteModel(Forms.Long("SummarySiteId")).ReferenceType,
                Forms.Data("SummaryDestinationColumn"),
                Forms.Data("SummaryLinkColumn"),
                Forms.Data("SummaryType"),
                Forms.Data("SummarySourceColumn"));
            if (error.Has())
            {
                responseCollection.Message(error.Message());
            }
            else
            {
                responseCollection.ReplaceAll("#SummarySettings", new HtmlBuilder()
                    .SummarySettings(sourceSiteSettings: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteSummary(ResponseCollection responseCollection)
        {
            var summary = SiteSettings.SummaryCollection.FirstOrDefault(
                o => o.Id == Forms.Long("DeleteSummaryId"));
            SiteSettings.DeleteSummary(summary.Id);
            responseCollection.ReplaceAll("#SummarySettings", new HtmlBuilder()
                .SummarySettings(sourceSiteSettings: SiteSettings));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorColumns(ResponseCollection responseCollection, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("EditorColumns");
            var selectedSourceColumns = Forms.List("EditorSourceColumns");
            if (controlId == "ToDisableEditorColumns" &&
                selectedColumns.Any(o => !SiteSettings.EditorColumn(o).Nullable))
            {
                responseCollection.Message(Messages.CanNotDisabled(
                    SiteSettings.EditorColumn(selectedColumns.FirstOrDefault(o =>
                        !SiteSettings.EditorColumn(o).Nullable)).LabelText));
            }
            else
            {
                SiteSettings.SetEditorColumns(command, selectedColumns, selectedSourceColumns);
                SetResponseAfterChangeColumns(
                    responseCollection,
                    command,
                    "Editor",
                    SiteSettings.EditorSelectableOptions(),
                    selectedColumns,
                    SiteSettings.EditorSelectableOptions(enabled: false),
                    selectedSourceColumns);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetColumnProperties(ResponseCollection responseCollection)
        {
            var selectedColumns = Forms.List("EditorColumns");
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
                        new HtmlBuilder().SelectableItems(
                            listItemCollection: SiteSettings.EditorSelectableOptions(),
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
            return Summaries.Synchronize(SiteSettings, SiteId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SynchronizeFormulas()
        {
            SetSiteSettingsPropertiesBySession();
            SiteSettings.SetChoicesByLinks();
            FormulaUtilities.Synchronize(this);
            return Messages.ResponseSynchronizationCompleted().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetTitleColumns(ResponseCollection responseCollection, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("TitleColumns");
            var selectedSourceColumns = Forms.List("TitleSourceColumns");
            SiteSettings.SetTitleColumns(command, selectedColumns, selectedSourceColumns);
            SetResponseAfterChangeColumns(
                responseCollection,
                command,
                "Title",
                SiteSettings.TitleSelectableOptions(),
                selectedColumns,
                SiteSettings.TitleSelectableOptions(enabled: false),
                selectedSourceColumns);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetLinkColumns(ResponseCollection responseCollection, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("LinkColumns");
            var selectedSourceColumns = Forms.List("LinkSourceColumns");
            SiteSettings.SetLinkColumns(command, selectedColumns, selectedSourceColumns);
            SetResponseAfterChangeColumns(
                responseCollection,
                command,
                "Link",
                SiteSettings.LinkSelectableOptions(),
                selectedColumns,
                SiteSettings.LinkSelectableOptions(enabled: false),
                selectedSourceColumns);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetHistoryColumns(ResponseCollection responseCollection, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("HistoryColumns");
            var selectedSourceColumns = Forms.List("HistorySourceColumns");
            SiteSettings.SetHistoryColumns(command, selectedColumns, selectedSourceColumns);
            SetResponseAfterChangeColumns(
                responseCollection,
                command,
                "History",
                SiteSettings.HistorySelectableOptions(),
                selectedColumns,
                SiteSettings.HistorySelectableOptions(enabled: false),
                selectedSourceColumns);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFormulas(ResponseCollection responseCollection, string controlId)
        {
            var selectedColumns = Forms.List("Formulas");
            SiteSettings.SetFormulas(controlId, selectedColumns);
            responseCollection.Html("#Formulas", new HtmlBuilder()
                .SelectableItems(
                    listItemCollection: SiteSettings.FormulaItemCollection(),
                    selectedValueTextCollection: selectedColumns));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddFormula(ResponseCollection responseCollection)
        {
            var error = SiteSettings.AddFormula(Forms.Data("Formula"));
            if (error.Has())
            {
                responseCollection.Message(error.Message());
            }
            else
            {
                responseCollection
                    .Html("#Formulas", new HtmlBuilder()
                        .SelectableItems(listItemCollection: SiteSettings.FormulaItemCollection()))
                    .Val("#Formula", string.Empty);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteFormulas(ResponseCollection responseCollection)
        {
            SiteSettings.DeleteFormulas(Forms.Data("Formulas").Split(';'));
            responseCollection
                .Html("#Formulas", new HtmlBuilder()
                    .SelectableItems(listItemCollection: SiteSettings.FormulaItemCollection()))
                .ClearFormData("Formulas");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SiteModel InheritSite()
        {
            return new SiteModel(InheritPermission);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SetResponseAfterChangeColumns(
            ResponseCollection responseCollection,
            string command,
            string typeName,
            Dictionary<string, string> selectableOptions,
            List<string> selectedColumns,
            Dictionary<string, string> selectableSourceOptions,
            List<string> selectedSourceColumns)
        {
            switch (command)
            {
                case "ToDisable":
                    Move(selectedColumns, selectedSourceColumns);
                    break;
                case "ToEnable":
                    Move(selectedSourceColumns, selectedColumns);
                    break;
            }
            responseCollection
                .Html("#" + typeName + "Columns",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: selectableOptions,
                        selectedValueTextCollection: selectedColumns))
                .SetFormData(typeName + "Columns", selectedColumns.Join(";"))
                .Html("#" + typeName + "SourceColumns",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: selectableSourceOptions,
                        selectedValueTextCollection: selectedSourceColumns))
                .SetFormData(typeName + "SourceColumns", selectedSourceColumns.Join(";"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Move(List<string> sources, List<string> destinations)
        {
            destinations.Clear();
            destinations.AddRange(sources);
            sources.Clear();
        }
    }
}
