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
    public class ItemModel : BaseModel
    {
        public long ReferenceId = 0;
        public string ReferenceType = string.Empty;
        public long SiteId = 0;
        public string Title = string.Empty;
        public string Subset = string.Empty;
        public bool MaintenanceTarget = true;
        public SiteModel Site = null;
        public long SavedReferenceId = 0;
        public string SavedReferenceType = string.Empty;
        public long SavedSiteId = 0;
        public string SavedTitle = string.Empty;
        public string SavedSubset = string.Empty;
        public bool SavedMaintenanceTarget = true;
        public SiteModel SavedSite = null;
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool ReferenceType_Updated { get { return ReferenceType != SavedReferenceType && ReferenceType != null; } }
        public bool SiteId_Updated { get { return SiteId != SavedSiteId; } }
        public bool Title_Updated { get { return Title != SavedTitle && Title != null; } }
        public bool Subset_Updated { get { return Subset != SavedSubset && Subset != null; } }
        public bool MaintenanceTarget_Updated { get { return MaintenanceTarget != SavedMaintenanceTarget; } }

        public ItemModel(
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

        public ItemModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectItems(
                tableType: tableType,
                column: column ?? Rds.ItemsDefaultColumns(),
                join: join ??  Rds.ItemsJoinDefault(),
                where: where ?? Rds.ItemsWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public string Index()
        {
            if (ReferenceId == 0)
            {
                return SiteUtilities.SiteTop(
                    siteSettings: SiteSettingsUtility.SitesSiteSettings(0));
            }
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Sites": return SiteUtilities.SiteMenu(Site);
                case "Issues": return IssueUtilities.Index(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                case "Results": return ResultUtilities.Index(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType);
                case "Wikis": return WikiUtilities.Index(
                    siteSettings: Site.WikisSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string IndexJson()
        {
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.IndexJson(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                case "Results": return ResultUtilities.IndexJson(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string Gantt()
        {
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Gantt(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string GanttJson()
        {
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.GanttJson(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string BurnDown()
        {
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.BurnDown(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string BurnDownJson()
        {
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.BurnDownJson(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string BurnDownRecordDetailsJson()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities
                    .BurnDownRecordDetails(Site.IssuesSiteSettings());
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string TimeSeries()
        {
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.TimeSeries(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                case "Results": return ResultUtilities.TimeSeries(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string TimeSeriesJson()
        {
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.TimeSeriesJson(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                case "Results": return ResultUtilities.TimeSeriesJson(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string Kamban()
        {
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Kamban(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                case "Results": return ResultUtilities.Kamban(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string KambanJson()
        {
            SetSite();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.KambanJson(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                case "Results": return ResultUtilities.KambanJson(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string New()
        {
            SetSite(siteOnly: true);
            switch (Site.ReferenceType)
            {
                case "Sites": return SiteUtilities.EditorNew(
                    ReferenceId != 0
                        ? Site.PermissionType
                        : Permissions.Types.Manager,
                    ReferenceId);
                case "Issues": return IssueUtilities.EditorNew(Site);
                case "Results": return ResultUtilities.EditorNew(Site);
                case "Wikis": return WikiUtilities.EditorNew(Site);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string NewJson()
        {
            return new ResponseCollection()
                .ReplaceAll("#MainContainer", New())
                .ReplaceAll("#ItemValidator", new HtmlBuilder().ItemValidator(Site.ReferenceType))
                .Invoke("validate" + Site.ReferenceType)
                .WindowScrollTop()
                .FocusMainForm()
                .ClearFormData()
                .PushState("Edit", Navigations.Get("Items", ReferenceId.ToString(), "New"))
                .ToJson();
        }

        public string Editor()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Editor(ReferenceId, clearSessions: true);
                case "Issues": return IssueUtilities.Editor(Site, ReferenceId, clearSessions: true);
                case "Results": return ResultUtilities.Editor(Site, ReferenceId, clearSessions: true);
                case "Wikis": return WikiUtilities.Editor(Site, ReferenceId, clearSessions: true);
                default: return new HtmlBuilder().NotFoundTemplate().ToString();
            }
        }

        public string Import()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Import(siteModel: Site);
                case "Results": return ResultUtilities.Import(siteModel: Site);
                case "Wikis": return WikiUtilities.Import(siteModel: Site);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public ResponseFile Export()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Export(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    siteModel: Site);
                case "Results": return ResultUtilities.Export(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    siteModel: Site);
                case "Wikis": return WikiUtilities.Export(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    siteModel: Site);
                default: return null;
            }
        }

        public string GridRows()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.GridRows(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType,
                    offset: DataViewGrid.Offset());
                case "Results": return ResultUtilities.GridRows(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType,
                    offset: DataViewGrid.Offset());
                case "Wikis": return WikiUtilities.GridRows(
                    siteSettings: Site.WikisSiteSettings(),
                    permissionType: Site.PermissionType,
                    offset: DataViewGrid.Offset());
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Create()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Sites": return SiteUtilities.Create(
                    permissionType: Site.SiteId != 0
                        ? Site.PermissionType
                        : Permissions.Types.Manager,
                    parentId: Site.SiteId,
                    inheritPermission: Site.InheritPermission);
                case "Issues": return IssueUtilities.Create(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType);
                case "Results": return ResultUtilities.Create(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType);
                case "Wikis": return WikiUtilities.Create(
                    siteSettings: Site.WikisSiteSettings(),
                    permissionType: Site.PermissionType);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Update()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities
                    .Update(Site.SitesSiteSettings(), Site.PermissionType, ReferenceId);
                case "Issues": return IssueUtilities
                    .Update(Site.IssuesSiteSettings(), Site.PermissionType, ReferenceId);
                case "Results": return ResultUtilities
                    .Update(Site.ResultsSiteSettings(), Site.PermissionType, ReferenceId);
                case "Wikis": return WikiUtilities
                    .Update(Site.WikisSiteSettings(), Site.PermissionType, ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string DeleteComment()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities
                    .Update(Site.SitesSiteSettings(), Site.PermissionType, ReferenceId);
                case "Issues": return IssueUtilities
                    .Update(Site.IssuesSiteSettings(), Site.PermissionType, ReferenceId);
                case "Results": return ResultUtilities
                    .Update(Site.ResultsSiteSettings(), Site.PermissionType, ReferenceId);
                case "Wikis": return WikiUtilities
                    .Update(Site.WikisSiteSettings(), Site.PermissionType, ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Copy()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Copy(Site);
                case "Issues": return IssueUtilities.Copy(
                    siteSettings: Site.SiteSettings,
                    permissionType: Site.PermissionType,
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Copy(
                    siteSettings: Site.SiteSettings,
                    permissionType: Site.PermissionType,
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.Copy(
                    siteSettings: Site.SiteSettings,
                    permissionType: Site.PermissionType,
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string MoveTargets()
        {
            SetSite();
            return new ResponseCollection().Html("#MoveTargets", new HtmlBuilder()
                .OptionCollection(
                    optionCollection: MoveTargets(
                        Rds.ExecuteTable(statements: new SqlStatement(
                            commandText: Def.Sql.MoveTarget,
                            param: Rds.SitesParam()
                                .TenantId(Sessions.TenantId())
                                .ReferenceType(Site.ReferenceType)
                                .Permissions_PermissionType(
                                    Permissions.Types.Update.ToInt().ToString())))
                                        .AsEnumerable()), 
                    selectedValue: Site.SiteId.ToString())).ToJson();
        }

        private Dictionary<string, ControlData> MoveTargets(IEnumerable<DataRow> siteCollection)
        {
            var moveTargets = new Dictionary<string, ControlData>();
            siteCollection
                .Where(o => o["ReferenceType"].ToString() == Site.ReferenceType)
                .ForEach(dataRow =>
                {
                    var current = dataRow;
                    var titles = new List<string>() { current["Title"].ToString() };
                    while(siteCollection.Any(o =>
                        o["SiteId"].ToLong() == current["ParentId"].ToLong()))
                        {
                            current = siteCollection.First(o =>
                                o["SiteId"].ToLong() == current["ParentId"].ToLong());
                            titles.Insert(0, current["Title"].ToString());
                        }
                    moveTargets.Add(
                        dataRow["SiteId"].ToString(),
                        new ControlData(titles.Join(" / ")));
                });
            return moveTargets;
        }

        public string Move()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Issues": return IssueUtilities.Move(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType,
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Move(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType,
                    resultId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string BulkMove()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.BulkMove(
                    Site.IssuesSiteSettings(), Site.PermissionType);
                case "Results": return ResultUtilities.BulkMove(
                    Site.ResultsSiteSettings(), Site.PermissionType);
                case "Wikis": return WikiUtilities.BulkMove(
                    Site.WikisSiteSettings(), Site.PermissionType);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Delete()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Delete(siteId: ReferenceId);
                case "Issues": return IssueUtilities.Delete(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType,
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Delete(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType,
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.Delete(
                    siteSettings: Site.WikisSiteSettings(),
                    permissionType: Site.PermissionType,
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string BulkDelete()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.BulkDelete(
                    Site.PermissionType, Site.IssuesSiteSettings());
                case "Results": return ResultUtilities.BulkDelete(
                    Site.PermissionType, Site.ResultsSiteSettings());
                case "Wikis": return WikiUtilities.BulkDelete(
                    Site.PermissionType, Site.WikisSiteSettings());
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Restore(long referenceId)
        {
            ReferenceId = referenceId;
            Get(Sqls.TableTypes.Deleted, where: Rds.ItemsWhere().ReferenceId(ReferenceId));
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Restore(siteId: ReferenceId);
                case "Issues": return IssueUtilities.Restore(issueId: ReferenceId);
                case "Results": return ResultUtilities.Restore(resultId: ReferenceId);
                case "Wikis": return WikiUtilities.Restore(wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string EditSeparateSettings()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.EditSeparateSettings(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType,
                    issueId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Separate()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.Separate(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType,
                    issueId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Histories()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.Histories(siteModel: Site);
                case "Issues": return IssueUtilities.Histories(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType,
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.Histories(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType,
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.Histories(
                    siteSettings: Site.WikisSiteSettings(),
                    permissionType: Site.PermissionType,
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string History()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.History(siteModel: Site);
                case "Issues": return IssueUtilities.History(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType,
                    issueId: ReferenceId);
                case "Results": return ResultUtilities.History(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType,
                    resultId: ReferenceId);
                case "Wikis": return WikiUtilities.History(
                    siteSettings: Site.WikisSiteSettings(),
                    permissionType: Site.PermissionType,
                    wikiId: ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string EditorJson()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return SiteUtilities.EditorJson(ReferenceId);
                case "Issues": return IssueUtilities.EditorJson(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId);
                case "Results": return ResultUtilities.EditorJson(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId);
                case "Wikis": return WikiUtilities.EditorJson(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string UpdateByKamban()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssueUtilities.UpdateByKamban(Site);
                case "Results": return ResultUtilities.UpdateByKamban(Site);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string SynchronizeSummary()
        {
            SetSite();
            return Site.SynchronizeSummary();
        }

        public string SynchronizeFormulas()
        {
            SetSite();
            return Site.SynchronizeFormulas();
        }

        private void SetSite(bool siteOnly = false)
        {
            Site = GetSite(siteOnly);
        }

        public SiteModel GetSite(bool siteOnly = false)
        {
            return siteOnly
                ? new SiteModel(ReferenceId)
                : new SiteModel(ReferenceType == "Sites" ? ReferenceId : SiteId);
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
                    case "ReferenceId": if (dataRow[name] != DBNull.Value) { ReferenceId = dataRow[name].ToLong(); SavedReferenceId = ReferenceId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "ReferenceType": ReferenceType = dataRow[name].ToString(); SavedReferenceType = ReferenceType; break;
                    case "SiteId": SiteId = dataRow[name].ToLong(); SavedSiteId = SiteId; break;
                    case "Title": Title = dataRow[name].ToString(); SavedTitle = Title; break;
                    case "Subset": Subset = dataRow[name].ToString(); SavedSubset = Subset; break;
                    case "MaintenanceTarget": MaintenanceTarget = dataRow[name].ToBool(); SavedMaintenanceTarget = MaintenanceTarget; break;
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
        public ItemModel()
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ItemModel(long referenceId)
        {
            OnConstructing();
            ReferenceId = referenceId;
            Get();
            OnConstructed();
        }
    }
}
