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
    public class SiteModel : BaseItemModel
    {
        public long Id { get { return SiteId; } }
        public override long UrlId { get { return SiteId; } }
        public int TenantId = Sessions.TenantId();
        public string ReferenceType = "Sites";
        public long ParentId = 0;
        public long InheritPermission = 0;
        public Permissions.Types PermissionType = (Permissions.Types)31;
        public SiteCollection Ancestors = null;
        public PermissionCollection PermissionSourceCollection = null;
        public PermissionCollection PermissionDestinationCollection = null;
        public int SiteMenu = 0;
        public List<string> MonitorChangesColumns = null;
        public List<string> TitleColumns = null;
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
        public List<string> SavedTitleColumns = null;
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

        public List<string> Session_TitleColumns()
        {
            return this.PageSession("TitleColumns") != null
                ? this.PageSession("TitleColumns") as List<string> ?? new List<string>()
                : TitleColumns;
        }

        public void  Session_TitleColumns(object value)
        {
            this.PageSession("TitleColumns", value);
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
                case "TitleColumns": return TitleColumns.ToString();
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
            Session_TitleColumns(null);
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

        public Dictionary<string, int> SearchIndexHash()
        {
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            else
            {
                var searchIndexHash = new Dictionary<string, int>();
                SiteInfo.SiteMenu.Breadcrumb(SiteId).SearchIndexes(searchIndexHash, 100);
                SiteId.SearchIndexes(searchIndexHash, 1);
                UpdatedTime.SearchIndexes(searchIndexHash, 200);
                Title.SearchIndexes(searchIndexHash, 4);
                Body.SearchIndexes(searchIndexHash, 200);
                Comments.SearchIndexes(searchIndexHash, 200);
                Creator.SearchIndexes(searchIndexHash, 100);
                Updator.SearchIndexes(searchIndexHash, 100);
                CreatedTime.SearchIndexes(searchIndexHash, 200);
                SearchIndexExtensions.OutgoingMailsSearchIndexes(
                    searchIndexHash, "Sites", SiteId);
                return searchIndexHash;
            }
        }

        public Error.Types Update(bool paramAll = false)
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
                        param: Rds.SitesParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return Error.Types.UpdateConflicts;
            Get();
            UpdateRelatedRecords();
            SiteInfo.SiteMenu.Set(SiteId);
            return Error.Types.None;
        }

        public void UpdateRelatedRecords(
            bool addUpdatedTimeParam = true, bool addUpdatorParam = true)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(SiteId),
                        param: Rds.ItemsParam()
                            .SiteId(SiteId)
                            .Title(SiteUtilities.TitleDisplayValue(SiteSettings, this))
                            .MaintenanceTarget(true),
                        addUpdatedTimeParam: addUpdatedTimeParam,
                        addUpdatorParam: addUpdatorParam),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(SiteId)),
                    LinkUtilities.Insert(SiteSettings.LinkCollection
                        .Select(o => o.SiteId)
                        .Distinct()
                        .ToDictionary(o => o, o => SiteId))
                });
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
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
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
            return Error.Types.None;
        }

        public Error.Types Restore(long siteId)
        {
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
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteSites(
                    tableType: tableType,
                    param: Rds.SitesParam().TenantId(TenantId).SiteId(SiteId)));
            return Error.Types.None;
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
            if (!Forms.HasData("Sites_TitleColumns")) TitleColumns = Session_TitleColumns();
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
                        param: Rds.ItemsParam().SiteId(raw: Def.Sql.Identity)),
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
                    var wikiModel = new WikiModel(SiteSettings)
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
            var res = new SitesResponseCollection(this);
            SetSiteSettingsPropertiesBySession();
            SetSiteSettings(res);
            Session_SiteSettings(SiteSettings.ToJson());
            return res.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteSettings(ResponseCollection res)
        {
            var controlId = Forms.ControlId();
            switch (controlId)
            {
                case "MoveUpGridColumns":
                case "MoveDownGridColumns":
                case "ToDisableGridColumns":
                case "ToEnableGridColumns":
                    SetGridColumns(res, controlId);
                    break;
                case "OpenGridColumnDialog":
                    OpenGridColumnDialog(res);
                    break;
                case "SetGridColumn":
                    SetGridColumn(res);
                    break;
                case "MoveUpFilterColumns":
                case "MoveDownFilterColumns":
                case "ToDisableFilterColumns":
                case "ToEnableFilterColumns":
                    SetFilterColumns(res, controlId);
                    break;
                case "OpenFilterColumnDialog":
                    OpenFilterColumnDialog(res);
                    break;
                case "SetFilterColumn":
                    SetFilterColumn(res);
                    break;
                case "AddAggregations":
                case "DeleteAggregations":
                case "MoveUpAggregations":
                case "MoveDownAggregations":
                    SetAggregations(res, controlId);
                    break;
                case "SetAggregationDetails":
                    SetAggregationDetails(res);
                    break;
                case "SummarySiteId":
                    SetSummarySiteId(res);
                    break;
                case "SummaryType":
                    SetSummaryType(res);
                    break;
                case "AddSummary":
                    AddSummary(res);
                    break;
                case "DeleteSummary":
                    DeleteSummary(res);
                    break;
                case "MoveUpEditorColumns":
                case "MoveDownEditorColumns":
                case "ToDisableEditorColumns":
                case "ToEnableEditorColumns":
                    SetEditorColumns(res, controlId);
                    break;
                case "OpenEditorColumnDialog":
                    OpenEditorColumnDialog(res);
                    break;
                case "SetEditorColumn":
                    SetEditorColumn(res);
                    break;
                case "MoveUpTitleColumns":
                case "MoveDownTitleColumns":
                case "ToDisableTitleColumns":
                case "ToEnableTitleColumns":
                    SetTitleColumns(res, controlId);
                    break;
                case "MoveUpLinkColumns":
                case "MoveDownLinkColumns":
                case "ToDisableLinkColumns":
                case "ToEnableLinkColumns":
                    SetLinkColumns(res, controlId);
                    break;
                case "MoveUpHistoryColumns":
                case "MoveDownHistoryColumns":
                case "ToDisableHistoryColumns":
                case "ToEnableHistoryColumns":
                    SetHistoryColumns(res, controlId);
                    break;
                case "MoveUpFormulas":
                case "MoveDownFormulas":
                    SetFormulas(res, controlId);
                    break;
                case "AddFormula":
                    AddFormula(res);
                    break;
                case "DeleteFormulas":
                    DeleteFormulas(res);
                    break;
                case "MoveUpViews":
                case "MoveDownViews":
                    SetViewsOrder(res, controlId);
                    break;
                case "NewView":
                case "EditView":
                    OpenViewDialog(res, controlId);
                    break;
                case "AddViewFilter":
                    AddViewFilter(res);
                    break;
                case "CreateView":
                    CreateView(res);
                    break;
                case "UpdateView":
                    UpdateView(res);
                    break;
                case "DeleteViews":
                    DeleteViews(res);
                    break;
                case "NewNotification":
                case "EditNotification":
                    OpenNotificationDialog(res, controlId);
                    break;
                case "CreateNotification":
                    CreateNotification(res, controlId);
                    break;
                case "UpdateNotification":
                    UpdateNotification(res, controlId);
                    break;
                case "DeleteNotification":
                    DeleteNotification(res, controlId);
                    break;
                case "MoveUpMonitorChangesColumns":
                case "MoveDownMonitorChangesColumns":
                case "ToDisableMonitorChangesColumns":
                case "ToEnableMonitorChangesColumns":
                    SetMonitorChangesColumns(res, controlId);
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
                    case "Format":
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
        public HtmlBuilder ReplaceSiteMenu(long sourceId, long destinationId)
        {
            return new HtmlBuilder().SiteMenu(
                ss: SiteSettings,
                pt: Permissions.Types.Manager,
                siteId: destinationId,
                referenceType: ReferenceType,
                title: SiteInfo.SiteMenu.Get(destinationId).Title,
                siteConditions: SiteInfo.SiteMenu.SiteConditions(SiteId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetGridColumns(ResponseCollection res, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("GridColumns");
            var selectedSourceColumns = Forms.List("GridSourceColumns");
            SiteSettings.SetGridColumns(command, selectedColumns, selectedSourceColumns);
            SetResponseAfterChangeColumns(
                res,
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
        private void OpenGridColumnDialog(ResponseCollection res)
        {
            var selectedColumns = Forms.List("GridColumns");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne());
            }
            else
            {
                var column = SiteSettings.GridColumn(selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest());
                }
                else
                {
                    res.Html(
                        "#GridColumnDialog",
                        SiteUtilities.GridColumnDialog(ss: SiteSettings, column: column));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetGridColumn(ResponseCollection res)
        {
            var columnName = Forms.Data("GridColumnName");
            var column = SiteSettings.GridColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest());
            }
            else
            {
                Forms.All().ForEach(data => SiteSettings.SetColumnProperty(
                    column, data.Key, GridColumnValue(data.Key, data.Value)));
                res
                    .Html("#GridColumns", new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.GridSelectableOptions(),
                        selectedValueTextCollection: new List<string> { columnName }))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string GridColumnValue(string name, string value)
        {
            switch (name)
            {
                case "GridDesign":
                    return Forms.Bool("UseGridDesign")
                        ? value
                        : null;
                default:
                    return value;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFilterColumns(ResponseCollection res, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("FilterColumns");
            var selectedSourceColumns = Forms.List("FilterSourceColumns");
            SiteSettings.SetFilterColumns(command, selectedColumns, selectedSourceColumns);
            SetResponseAfterChangeColumns(
                res,
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
        private void OpenFilterColumnDialog(ResponseCollection res)
        {
            var selectedColumns = Forms.List("FilterColumns");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne());
            }
            else
            {
                var column = SiteSettings.FilterColumn(selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest());
                }
                else
                {
                    res.Html(
                        "#FilterColumnDialog",
                        SiteUtilities.FilterColumnDialog(ss: SiteSettings, column: column));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFilterColumn(ResponseCollection res)
        {
            var columnName = Forms.Data("FilterColumnName");
            var column = SiteSettings.FilterColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest());
            }
            else
            {
                Forms.All().ForEach(data => SiteSettings.SetColumnProperty(
                    column, data.Key, data.Value));
                res
                    .Html("#FilterColumns", new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.FilterSelectableOptions(),
                        selectedValueTextCollection: new List<string> { columnName }))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetAggregations(ResponseCollection res, string controlId)
        {
            var selectedColumns = Forms.List("AggregationDestination");
            var selectedSourceColumns = Forms.List("AggregationSource");
            if (selectedColumns.Any() || selectedSourceColumns.Any())
            {
                SiteSettings.SetAggregations(
                    controlId,
                    selectedColumns,
                    selectedSourceColumns);
                res
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
        private void SetAggregationDetails(ResponseCollection res)
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
                res
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
        private void SetSummarySiteId(ResponseCollection res)
        {
            var destinationSiteId = Forms.Long("SummarySiteId");
            var destinationSiteSettings = new SiteModel(destinationSiteId);
            var siteDataRows = SiteSettings.SummarySiteDataRows();
            res
                .ReplaceAll("#SummaryDestinationColumnField", new HtmlBuilder()
                    .SummaryDestinationColumn(
                        referenceType: destinationSiteSettings.ReferenceType,
                        siteId: destinationSiteSettings.SiteId,
                        siteDataRows: siteDataRows))
                .ReplaceAll("#SummaryLinkColumnField", new HtmlBuilder()
                    .SummaryLinkColumn(
                        ss: SiteSettings,
                        siteId: destinationSiteId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummaryType(ResponseCollection res)
        {
            res.ReplaceAll("#SummarySourceColumnField", new HtmlBuilder()
                .SummarySourceColumn(SiteSettings, Forms.Data("SummaryType")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddSummary(ResponseCollection res)
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
                res.Message(error.Message());
            }
            else
            {
                res.ReplaceAll("#SummarySettings", new HtmlBuilder()
                    .SummarySettings(sourceSiteSettings: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteSummary(ResponseCollection res)
        {
            var summary = SiteSettings.SummaryCollection.FirstOrDefault(
                o => o.Id == Forms.Long("DeleteSummaryId"));
            SiteSettings.DeleteSummary(summary.Id);
            res.ReplaceAll("#SummarySettings", new HtmlBuilder()
                .SummarySettings(sourceSiteSettings: SiteSettings));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorColumns(ResponseCollection res, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("EditorColumns");
            var selectedSourceColumns = Forms.List("EditorSourceColumns");
            if (controlId == "ToDisableEditorColumns" &&
                selectedColumns.Any(o => !SiteSettings.EditorColumn(o).Nullable))
            {
                res.Message(Messages.CanNotDisabled(
                    SiteSettings.EditorColumn(selectedColumns.FirstOrDefault(o =>
                        !SiteSettings.EditorColumn(o).Nullable)).LabelText));
            }
            else
            {
                SiteSettings.SetEditorColumns(command, selectedColumns, selectedSourceColumns);
                SetResponseAfterChangeColumns(
                    res,
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
        private void OpenEditorColumnDialog(ResponseCollection res)
        {
            var selectedColumns = Forms.List("EditorColumns");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne());
            }
            else
            {
                var column = SiteSettings.EditorColumn(selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest());
                }
                else
                {
                    var titleColumns = SiteSettings.TitleColumns;
                    if (column.ColumnName == "Title")
                    {
                        Session_TitleColumns(titleColumns);
                    }
                    res.Html(
                        "#EditorColumnDialog",
                        SiteUtilities.EditorColumnDialog(
                            ss: SiteSettings,
                            column: column,
                            titleColumns: titleColumns));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorColumn(ResponseCollection res)
        {
            var columnName = Forms.Data("EditorColumnName");
            var column = SiteSettings.EditorColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest());
            }
            else
            {
                if (column.ColumnName == "Title")
                {
                    SiteSettings.TitleColumns = Session_TitleColumns();
                }
                Forms.All().ForEach(data =>
                    SiteSettings.SetColumnProperty(column, data.Key, data.Value));
                res
                    .Html("#EditorColumns", new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.EditorSelectableOptions(),
                        selectedValueTextCollection: new List<string> { columnName }))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetTitleColumns(ResponseCollection res, string controlId)
        {
            TitleColumns = Session_TitleColumns();
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("TitleColumns");
            var selectedSourceColumns = Forms.List("TitleSourceColumns");
            TitleColumns = ColumnUtilities.GetChanged(
                TitleColumns,
                ColumnUtilities.ChangeCommand(controlId),
                selectedColumns,
                selectedSourceColumns);
            SetResponseAfterChangeColumns(
                res,
                command,
                "Title",
                SiteSettings.TitleSelectableOptions(TitleColumns),
                selectedColumns,
                SiteSettings.TitleSelectableOptions(TitleColumns, enabled: false),
                selectedSourceColumns);
            Session_TitleColumns(TitleColumns);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetLinkColumns(ResponseCollection res, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("LinkColumns");
            var selectedSourceColumns = Forms.List("LinkSourceColumns");
            SiteSettings.SetLinkColumns(command, selectedColumns, selectedSourceColumns);
            SetResponseAfterChangeColumns(
                res,
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
        private void SetHistoryColumns(ResponseCollection res, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.List("HistoryColumns");
            var selectedSourceColumns = Forms.List("HistorySourceColumns");
            SiteSettings.SetHistoryColumns(command, selectedColumns, selectedSourceColumns);
            SetResponseAfterChangeColumns(
                res,
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
        private void SetFormulas(ResponseCollection res, string controlId)
        {
            var selectedColumns = Forms.List("Formulas");
            SiteSettings.SetFormulas(controlId, selectedColumns);
            res.Html("#Formulas", new HtmlBuilder()
                .SelectableItems(
                    listItemCollection: SiteSettings.FormulaItemCollection(),
                    selectedValueTextCollection: selectedColumns));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddFormula(ResponseCollection res)
        {
            var error = SiteSettings.AddFormula(Forms.Data("Formula"));
            if (error.Has())
            {
                res.Message(error.Message());
            }
            else
            {
                res
                    .Html("#Formulas", new HtmlBuilder()
                        .SelectableItems(listItemCollection: SiteSettings.FormulaItemCollection()))
                    .Val("#Formula", string.Empty);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteFormulas(ResponseCollection res)
        {
            SiteSettings.DeleteFormulas(Forms.Data("Formulas").Split(';'));
            res
                .Html("#Formulas", new HtmlBuilder()
                    .SelectableItems(listItemCollection: SiteSettings.FormulaItemCollection()))
                .ClearFormData("Formulas");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetViewsOrder(ResponseCollection res, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.IntList("Views", ';');
            SiteSettings.SetViewsOrder(command, selectedColumns);
            res
                .Html(
                    "#Views",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.ViewSelectableOptions(),
                        selectedValueTextCollection: selectedColumns.Select(o => o.ToString())))
                .SetFormData("Views", selectedColumns.Join(";"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenViewDialog(ResponseCollection res, string controlId)
        {
            View view;
            if (controlId == "NewView")
            {
                view = new View(SiteSettings);
                OpenViewDialog(res, view);
            }
            else
            {
                var idList = Forms.IntList("Views", ';');
                if (idList.Count() != 1)
                {
                    OpenViewError(res, Messages.SelectOne());
                }
                else
                {
                    view = SiteSettings.Views.FirstOrDefault(o => o.Id == idList.First());
                    if (view == null)
                    {
                        OpenViewError(res, Messages.SelectOne());
                    }
                    else
                    {
                        SiteSettingsUtility.Get(this);
                        OpenViewDialog(res, view);
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenViewDialog(ResponseCollection res, View view)
        {
            res.Html("#ViewDialog", SiteUtilities.ViewDialog(
                ss: SiteSettings,
                controlId: Forms.ControlId(),
                view: view));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenViewError(ResponseCollection res, Message message)
        {
            res
                .Message(message)
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddViewFilter(ResponseCollection res)
        {
            SiteSettingsUtility.Get(this);
            res
                .Append(
                    "#ViewFiltersTab .items",
                    new HtmlBuilder().ViewFilter(
                        SiteSettings.GetColumn(Forms.Data("ViewFilterSelector"))))
                .Remove("#ViewFilterSelector option:selected");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CreateView(ResponseCollection res)
        {
            SiteSettings.AddView(new View(SiteSettings));
            res
                .ViewResponses(SiteSettings, new List<int>
                {
                    SiteSettings.ViewLatestId
                })
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateView(ResponseCollection res)
        {
            var selected = Forms.Int("ViewId");
            var view = SiteSettings.Views.FirstOrDefault(o => o.Id == selected);
            if (view == null)
            {
                res.Message(Messages.NotFound());
            }
            else
            {
                view.SetByForm(SiteSettings);
                res
                    .ViewResponses(SiteSettings, new List<int> { selected })
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteViews(ResponseCollection res)
        {
            SiteSettings.Views.RemoveAll(o => Forms.IntList("Views", ';').Contains(o.Id));
            res.ViewResponses(SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenNotificationDialog(ResponseCollection res, string controlId)
        {
            var notification = controlId == "EditNotification"
                ? GetNotification(Forms.Int("NotificationId"))
                : new Notification(
                    Notification.Types.Mail,
                    string.Empty,
                    string.Empty,
                    SiteSettings.EditorColumns
                        .Concat(new List<string> { "Comments" }).ToList());
            if (notification == null)
            {
                res.Message(Messages.NotFound());
            }
            else
            {
                Session_MonitorChangesColumns(notification.MonitorChangesColumns);
                res.Html("#NotificationDialog", SiteUtilities.NotificationDialog(
                    ss: SiteSettings,
                    controlId: Forms.ControlId(),
                    notification: notification));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CreateNotification(ResponseCollection res, string controlId)
        {
            SiteSettings.Notifications.Add(new Notification(
                (Notification.Types)Forms.Int("NotificationType"),
                Forms.Data("NotificationPrefix"),
                Forms.Data("NotificationAddress"),
                Session_MonitorChangesColumns()));
            SetNotificationsResponseCollection(res);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateNotification(ResponseCollection res, string controlId)
        {
            var notification = GetNotification(Forms.Int("NotificationId"));
            if (notification == null)
            {
                res.Message(Messages.NotFound());
            }
            else
            {
                notification.Update(
                    Forms.Data("NotificationPrefix"),
                    Forms.Data("NotificationAddress"),
                    Session_MonitorChangesColumns());
                SetNotificationsResponseCollection(res);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteNotification(ResponseCollection res, string controlId)
        {
            var notification = GetNotification(Forms.Int("NotificationId"));
            if (notification == null)
            {
                res.Message(Messages.NotFound());
            }
            else
            {
                SiteSettings.Notifications.Remove(notification);
                SetNotificationsResponseCollection(res);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private Notification GetNotification(int id)
        {
            return SiteSettings.Notifications.Count > id
                ? SiteSettings.Notifications[id]
                : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetNotificationsResponseCollection(ResponseCollection res)
        {
            res
                .ReplaceAll(
                    "#NotificationSettings",
                    new HtmlBuilder().NotificationSettings(ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetMonitorChangesColumns(
            ResponseCollection res, string controlId)
        {
            var notification = GetNotification(Forms.Int("NotificationId"));
            MonitorChangesColumns = Session_MonitorChangesColumns();
            if (notification == null && controlId == "EditNotification")
            {
                res.Message(Messages.NotFound());
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
                    res,
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
            SiteSettings.SetChoiceHash();
            FormulaUtilities.Synchronize(this);
            return Messages.ResponseSynchronizationCompleted().ToJson();
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
            ResponseCollection res,
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
            res
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
