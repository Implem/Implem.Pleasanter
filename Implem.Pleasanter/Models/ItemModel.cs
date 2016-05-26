using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Styles;
using Implem.Pleasanter.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Models
{
    public class ItemModel : BaseModel
    {
        public long ReferenceId = 0;
        public string ReferenceType = string.Empty;
        public long SiteId = 0;
        public string Title = string.Empty;
        public string Subset = string.Empty;
        public bool UpdateTarget = true;
        public SiteModel Site = null;
        public long SavedReferenceId = 0;
        public string SavedReferenceType = string.Empty;
        public long SavedSiteId = 0;
        public string SavedTitle = string.Empty;
        public string SavedSubset = string.Empty;
        public bool SavedUpdateTarget = true;
        public SiteModel SavedSite = null;
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool ReferenceType_Updated { get { return ReferenceType != SavedReferenceType && ReferenceType != null; } }
        public bool SiteId_Updated { get { return SiteId != SavedSiteId; } }
        public bool Title_Updated { get { return Title != SavedTitle && Title != null; } }
        public bool Subset_Updated { get { return Subset != SavedSubset && Subset != null; } }
        public bool UpdateTarget_Updated { get { return UpdateTarget != SavedUpdateTarget; } }

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
                column: column ?? Rds.ItemsColumnDefault(),
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
                return SitesUtility.SiteTop(SiteSettingsUtility.SitesSiteSettings(0));
            }
            SetSite();
            Site.SiteSettings.SetLinks();
            switch (Site.ReferenceType)
            {
                case "Sites": return SitesUtility.SiteMenu(Site);
                case "Issues": return IssuesUtility.Index(Site.IssuesSiteSettings(), Site.PermissionType);
                case "Results": return ResultsUtility.Index(Site.ResultsSiteSettings(), Site.PermissionType);
                case "Wikis": return WikisUtility.Index(Site.WikisSiteSettings(), Site.PermissionType);
                default: return HtmlTemplates.NotFound().ToString();
            }
        }

        public string New()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Sites": return SitesUtility.EditorNew(
                    ReferenceId != 0
                        ? Site.PermissionType
                        : Permissions.Types.Manager,
                    ReferenceId);
                case "Issues": return IssuesUtility.EditorNew(Site, ReferenceId);
                case "Results": return ResultsUtility.EditorNew(Site, ReferenceId);
                case "Wikis": return WikisUtility.EditorNew(Site, ReferenceId);
                default: return HtmlTemplates.NotFound().ToString();
            }
        }

        public string NewByLink()
        {
            return new ResponseCollection()
                .Html("#MainContainer", New())
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
                case "Sites": return SitesUtility.Editor(ReferenceId, clearSessions: true);
                case "Issues": return IssuesUtility.Editor(Site, ReferenceId, clearSessions: true);
                case "Results": return ResultsUtility.Editor(Site, ReferenceId, clearSessions: true);
                case "Wikis": return WikisUtility.Editor(Site, ReferenceId, clearSessions: true);
                default: return HtmlTemplates.NotFound().ToString();
            }
        }

        public string Import()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssuesUtility.Import(siteModel: Site);
                case "Results": return ResultsUtility.Import(siteModel: Site);
                case "Wikis": return WikisUtility.Import(siteModel: Site);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public ResponseFile Export()
        {
            SetSite();
            Site.SiteSettings.SetLinks();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssuesUtility.Export(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    siteModel: Site);
                case "Results": return ResultsUtility.Export(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    siteModel: Site);
                case "Wikis": return WikisUtility.Export(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    siteModel: Site);
                default: return null;
            }
        }

        public string DataView()
        {
            SetSite();
            Site.SiteSettings.SetLinks();
            DataViewSelectors.Set(Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues": return IssuesUtility.DataView(
                    siteSettings: Site.IssuesSiteSettings(), permissionType: Site.PermissionType);
                case "Results": return ResultsUtility.DataView(
                    siteSettings: Site.ResultsSiteSettings(), permissionType: Site.PermissionType);
                case "Wikis": return WikisUtility.DataView(
                    siteSettings: Site.WikisSiteSettings(), permissionType: Site.PermissionType);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string GridRows()
        {
            SetSite();
            Site.SiteSettings.SetLinks();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssuesUtility.GridRows(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType,
                    offset: Offset());
                case "Results": return ResultsUtility.GridRows(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType,
                    offset: Offset());
                case "Wikis": return WikisUtility.GridRows(
                    siteSettings: Site.WikisSiteSettings(),
                    permissionType: Site.PermissionType,
                    offset: Offset());
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        private int Offset()
        {
            return
                Forms.Data("ControlId").StartsWith("DataViewFilters_") ||
                Forms.Keys().Any(o => o.StartsWith("GridSorters_"))
                    ? 0
                    : Forms.Int("GridOffset");
        }

        public string Create()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Sites": return new SiteModel(setByForm: true) { ParentId = Site.SiteId }
                    .Create(
                        permissionType: ReferenceId != 0
                            ? Site.PermissionType
                            : Permissions.Types.Manager,
                        parentId: ReferenceId,
                        inheritPermission: Site.InheritPermission);
                case "Issues": return new IssueModel(
                    siteSettings: Site.IssuesSiteSettings(),
                    permissionType: Site.PermissionType,
                    issueId: 0,
                    setByForm: true)
                        .Create();
                case "Results": return new ResultModel(
                    siteSettings: Site.ResultsSiteSettings(),
                    permissionType: Site.PermissionType,
                    resultId: 0,
                    setByForm: true)
                        .Create();
                case "Wikis": return new WikiModel(
                    siteSettings: Site.WikisSiteSettings(),
                    permissionType: Site.PermissionType,
                    wikiId: 0,
                    setByForm: true)
                        .Create();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Update()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return new SiteModel(ReferenceId, setByForm: true)
                    .Update();
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Update();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Update();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Update();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string DeleteComment()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return new SiteModel(ReferenceId, setByForm: true)
                    .Update();
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Update();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Update();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Update();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Copy()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return new SiteModel(ReferenceId, setByForm: true)
                    .Copy();
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Copy();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Copy();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Copy();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string MoveTargets()
        {
            SetSite();
            return new ResponseCollection().Html("#Dialog_MoveTargets", new HtmlBuilder()
                .OptionCollection(
                    optionCollection: MoveTargets(
                        new SiteCollection(where: Rds.SitesWhere()
                            .TenantId(Sessions.TenantId())
                            .ReferenceType(
                                value: new string[] { "Sites", Site.ReferenceType }, 
                                multiParamOperator: " or "))
                                    .Where(o => o.PermissionType.CanRead())), 
                    selectedValue: Site.SiteId.ToString())).ToJson();
        }

        private Dictionary<string, ControlData> MoveTargets(
            IEnumerable<SiteModel> siteCollection)
        {
            var moveTargets = new Dictionary<string, ControlData>();
            siteCollection
                .Where(o => o.ReferenceType == Site.ReferenceType)
                .Where(o => o.PermissionType.CanUpdate())
                .ForEach(siteModel =>
                {
                    var current = siteModel;
                    var titles = new List<string>() { current.Title.Value };
                    while(siteCollection.Any(o => o.SiteId == current.ParentId))
                    {
                        current = siteCollection.First(o => o.SiteId == current.ParentId);
                        titles.Insert(0, current.Title.Value);
                    }
                    moveTargets.Add(
                        siteModel.SiteId.ToString(),
                        new ControlData(titles.Join(" / ")));
                });
            return moveTargets;
        }

        public string Move()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Move();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Move();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Move();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string BulkMove()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssuesUtility.BulkMove(
                    Site.IssuesSiteSettings(), Site.PermissionType);
                case "Results": return ResultsUtility.BulkMove(
                    Site.ResultsSiteSettings(), Site.PermissionType);
                case "Wikis": return WikisUtility.BulkMove(
                    Site.WikisSiteSettings(), Site.PermissionType);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Delete()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return new SiteModel(ReferenceId, setByForm: true)
                    .Delete();
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Delete();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Delete();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId,
                    setByForm: true)
                        .Delete();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string BulkDelete()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssuesUtility.BulkDelete(
                    Site.PermissionType, Site.IssuesSiteSettings());
                case "Results": return ResultsUtility.BulkDelete(
                    Site.PermissionType, Site.ResultsSiteSettings());
                case "Wikis": return WikisUtility.BulkDelete(
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
                case "Sites": return new SiteModel().Restore(ReferenceId);
                case "Issues": return new IssueModel().Restore(ReferenceId);
                case "Results": return new ResultModel().Restore(ReferenceId);
                case "Wikis": return new WikiModel().Restore(ReferenceId);
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string EditSeparateSettings()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .EditSeparateSettings();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Separate()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Separate();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Histories()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return new SiteModel(
                    new SiteSettings("Sites"),
                    ReferenceId)
                        .Histories();
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Histories();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Histories();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Histories();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string History()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return Site.History();
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .History();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .History();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .History();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Previous()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return new SiteModel(ReferenceId)
                    .Previous();
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Previous();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Previous();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Previous();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Next()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return new SiteModel(ReferenceId)
                    .Next();
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Next();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Next();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Next();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string Reload()
        {
            SetSite();
            switch (ReferenceType)
            {
                case "Sites": return new SiteModel(ReferenceId)
                    .Reload();
                case "Issues": return new IssueModel(
                    Site.IssuesSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Reload();
                case "Results": return new ResultModel(
                    Site.ResultsSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Reload();
                case "Wikis": return new WikiModel(
                    Site.WikisSiteSettings(),
                    Site.PermissionType,
                    ReferenceId)
                        .Reload();
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string BurnDownRecordDetails()
        {
            SetSite();
            switch (Site.ReferenceType)
            {
                case "Issues": return IssuesUtility
                    .BurnDownRecordDetails(Site.IssuesSiteSettings());
                default: return Messages.ResponseNotFound().ToJson();
            }
        }

        public string SynchronizeSummary()
        {
            SetSite();
            return Site.SynchronizeSummary();
        }

        private void SetSite()
        {
            Site = GetSite();
        }

        public SiteModel GetSite()
        {
            return new SiteModel(ReferenceType == "Sites" ? ReferenceId : SiteId);
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
                    case "UpdateTarget": UpdateTarget = dataRow[name].ToBool(); SavedUpdateTarget = UpdateTarget; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
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

    public class ItemCollection : List<ItemModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public ItemCollection(
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

        public ItemCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private ItemCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new ItemModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public ItemCollection(
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
                Rds.SelectItems(
                    dataTableName: "Main",
                    column: column ?? Rds.ItemsColumnDefault(),
                    join: join ??  Rds.ItemsJoinDefault(),
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
                statements.AddRange(Rds.ItemsAggregations(aggregationCollection, where));
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
                statements: Rds.ItemsStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class ItemsUtility
    {
    }
}
