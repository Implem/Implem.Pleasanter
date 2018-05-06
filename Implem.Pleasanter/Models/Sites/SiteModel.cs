using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
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
    [Serializable]
    public class SiteModel : BaseItemModel
    {
        public SiteSettings SiteSettings;
        public int TenantId = Sessions.TenantId();
        public string ReferenceType = "Sites";
        public long ParentId = 0;
        public long InheritPermission = 0;
        public SiteCollection Ancestors = null;
        public int SiteMenu = 0;
        public List<string> MonitorChangesColumns = null;
        public List<string> TitleColumns = null;
        public Export Export = null;

        public TitleBody TitleBody
        {
            get
            {
                return new TitleBody(SiteId, Title.Value, Title.DisplayValue, Body);
            }
        }

        [NonSerialized] public int SavedTenantId = Sessions.TenantId();
        [NonSerialized] public string SavedReferenceType = "Sites";
        [NonSerialized] public long SavedParentId = 0;
        [NonSerialized] public long SavedInheritPermission = 0;
        [NonSerialized] public string SavedSiteSettings = "[]";
        [NonSerialized] public SiteCollection SavedAncestors = null;
        [NonSerialized] public int SavedSiteMenu = 0;
        [NonSerialized] public List<string> SavedMonitorChangesColumns = null;
        [NonSerialized] public List<string> SavedTitleColumns = null;
        [NonSerialized] public Export SavedExport = null;

        public bool TenantId_Updated(Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != TenantId);
        }

        public bool ReferenceType_Updated(Column column = null)
        {
            return ReferenceType != SavedReferenceType && ReferenceType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ReferenceType);
        }

        public bool ParentId_Updated(Column column = null)
        {
            return ParentId != SavedParentId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != ParentId);
        }

        public bool InheritPermission_Updated(Column column = null)
        {
            return InheritPermission != SavedInheritPermission &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != InheritPermission);
        }

        public bool SiteSettings_Updated(Column column = null)
        {
            return SiteSettings.RecordingJson() != SavedSiteSettings && SiteSettings.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != SiteSettings.RecordingJson());
        }

        public SiteSettings Session_SiteSettings()
        {
            return this.PageSession("SiteSettings") != null
                ? this.PageSession("SiteSettings")?.ToString().Deserialize<SiteSettings>() ?? new SiteSettings(ReferenceType)
                : SiteSettings;
        }

        public void Session_SiteSettings(object value)
        {
            this.PageSession("SiteSettings", value);
        }

        public List<string> Session_MonitorChangesColumns()
        {
            return this.PageSession("MonitorChangesColumns") != null
                ? this.PageSession("MonitorChangesColumns") as List<string> ?? new List<string>()
                : MonitorChangesColumns;
        }

        public void Session_MonitorChangesColumns(object value)
        {
            this.PageSession("MonitorChangesColumns", value);
        }

        public List<string> Session_TitleColumns()
        {
            return this.PageSession("TitleColumns") != null
                ? this.PageSession("TitleColumns") as List<string> ?? new List<string>()
                : TitleColumns;
        }

        public void Session_TitleColumns(object value)
        {
            this.PageSession("TitleColumns", value);
        }

        public Export Session_Export()
        {
            return this.PageSession("Export") != null
                ? this.PageSession("Export") as Export ?? new Export()
                : Export;
        }

        public void Session_Export(object value)
        {
            this.PageSession("Export", value);
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
                case "SiteSettings": return SiteSettings.RecordingJson();
                case "Ancestors": return Ancestors.ToString();
                case "SiteMenu": return SiteMenu.ToString();
                case "MonitorChangesColumns": return MonitorChangesColumns.ToString();
                case "TitleColumns": return TitleColumns.ToString();
                case "Export": return Export.ToString();
                case "Comments": return Comments.ToJson();
                case "Creator": return Creator.Id.ToString();
                case "Updator": return Updator.Id.ToString();
                case "CreatedTime": return CreatedTime.Value.ToString();
                case "VerUp": return VerUp.ToString();
                case "Timestamp": return Timestamp;
                default: return null;
            }
        }

        public Dictionary<string, string> PropertyValues(IEnumerable<string> names)
        {
            var hash = new Dictionary<string, string>();
            names?.ForEach(name =>
            {
                switch (name)
                {
                    case "TenantId":
                        hash.Add("TenantId", TenantId.ToString());
                        break;
                    case "SiteId":
                        hash.Add("SiteId", SiteId.ToString());
                        break;
                    case "UpdatedTime":
                        hash.Add("UpdatedTime", UpdatedTime.Value.ToString());
                        break;
                    case "Ver":
                        hash.Add("Ver", Ver.ToString());
                        break;
                    case "Title":
                        hash.Add("Title", Title.Value);
                        break;
                    case "Body":
                        hash.Add("Body", Body);
                        break;
                    case "TitleBody":
                        hash.Add("TitleBody", TitleBody.ToString());
                        break;
                    case "ReferenceType":
                        hash.Add("ReferenceType", ReferenceType);
                        break;
                    case "ParentId":
                        hash.Add("ParentId", ParentId.ToString());
                        break;
                    case "InheritPermission":
                        hash.Add("InheritPermission", InheritPermission.ToString());
                        break;
                    case "SiteSettings":
                        hash.Add("SiteSettings", SiteSettings.RecordingJson());
                        break;
                    case "Ancestors":
                        hash.Add("Ancestors", Ancestors.ToString());
                        break;
                    case "SiteMenu":
                        hash.Add("SiteMenu", SiteMenu.ToString());
                        break;
                    case "MonitorChangesColumns":
                        hash.Add("MonitorChangesColumns", MonitorChangesColumns.ToString());
                        break;
                    case "TitleColumns":
                        hash.Add("TitleColumns", TitleColumns.ToString());
                        break;
                    case "Export":
                        hash.Add("Export", Export.ToString());
                        break;
                    case "Comments":
                        hash.Add("Comments", Comments.ToJson());
                        break;
                    case "Creator":
                        hash.Add("Creator", Creator.Id.ToString());
                        break;
                    case "Updator":
                        hash.Add("Updator", Updator.Id.ToString());
                        break;
                    case "CreatedTime":
                        hash.Add("CreatedTime", CreatedTime.Value.ToString());
                        break;
                    case "VerUp":
                        hash.Add("VerUp", VerUp.ToString());
                        break;
                    case "Timestamp":
                        hash.Add("Timestamp", Timestamp);
                        break;
                }
            });
            return hash;
        }

        public List<long> SwitchTargets;

        public SiteModel()
        {
        }

        public SiteModel(
            long parentId,
            long inheritPermission,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
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
            bool setByApi = false,
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

        public SiteModel(DataRow dataRow, string tableAlias = null)
        {
            OnConstructing();
            Set(dataRow, tableAlias);
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
            SiteInfo.SetSiteUserHash(Sessions.TenantId(), SiteId);
        }

        public void ClearSessions()
        {
            Session_SiteSettings(null);
            Session_MonitorChangesColumns(null);
            Session_TitleColumns(null);
            Session_Export(null);
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
                orderBy: orderBy,
                param: param,
                distinct: distinct,
                top: top)));
            SetSiteSettingsProperties();
            return this;
        }

        public string FullText(
            SiteSettings ss, bool backgroundTask = false, bool onCreating = false)
        {
            if (Parameters.Search.Provider != "FullText") return null;
            if (!Parameters.Search.CreateIndexes && !backgroundTask) return null;
            if (AccessStatus == Databases.AccessStatuses.NotFound) return null;
            if (ReferenceType == "Wikis") return null;
            var fullText = new List<string>();
            SiteInfo.TenantCaches[Sessions.TenantId()]
                .SiteMenu.Breadcrumb(SiteId).FullText(fullText);
            SiteId.FullText(fullText);
            ss.EditorColumns.ForEach(columnName =>
            {
                switch (columnName)
                {
                    case "Title":
                        Title.FullText(fullText);
                        break;
                    case "Body":
                        Body.FullText(fullText);
                        break;
                    case "Comments":
                        Comments.FullText(fullText);
                        break;
                }
            });
            Creator.FullText(fullText);
            Updator.FullText(fullText);
            CreatedTime.FullText(fullText);
            UpdatedTime.FullText(fullText);
            if (!onCreating)
            {
                FullTextExtensions.OutgoingMailsFullText(fullText, "Sites", SiteId);
            }
            return fullText
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Distinct()
                .Join(" ");
        }

        public Dictionary<string, int> SearchIndexHash(SiteSettings ss)
        {
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            else
            {
                var searchIndexHash = new Dictionary<string, int>();
                SiteInfo.TenantCaches[Sessions.TenantId()]
                    .SiteMenu.Breadcrumb(SiteId).SearchIndexes(searchIndexHash, 100);
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

        public Error.Types Update(
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            RdsUser rdsUser = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool get = true)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(statements, timestamp, param, otherInitValue, additionalStatements);
            if (permissionChanged)
            {
                statements.UpdatePermissions(ss, SiteId, permissions, site: true);
            }
            statements.Add(Rds.PhysicalDeleteReminderSchedules(
                where: Rds.ReminderSchedulesWhere()
                    .SiteId(SiteId)));
            SiteSettings.Reminders?.ForEach(reminder =>
                statements.Add(Rds.UpdateOrInsertReminderSchedules(
                    param: Rds.ReminderSchedulesParam()
                        .SiteId(SiteId)
                        .Id(reminder.Id)
                        .ScheduledTime(reminder.StartDateTime.Next(reminder.Type)),
                    where: Rds.ReminderSchedulesWhere()
                        .SiteId(SiteId)
                        .Id(reminder.Id))));
            var count = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            if (count == 0) return Error.Types.UpdateConflicts;
            if (get) Get();
            UpdateRelatedRecords();
            SiteInfo.Reflesh();
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.SitesWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateSites(
                    where: where,
                    param: param ?? Rds.SitesParamDefault(this, otherInitValue: otherInitValue),
                    countRecord: true),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.SitesUpdated)
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.SitesColumnCollection();
            var param = new Rds.SitesParamCollection();
            column.TenantId(function: Sqls.Functions.SingleColumn); param.TenantId();
            column.SiteId(function: Sqls.Functions.SingleColumn); param.SiteId();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.Title(function: Sqls.Functions.SingleColumn); param.Title();
            column.ReferenceType(function: Sqls.Functions.SingleColumn); param.ReferenceType();
            column.ParentId(function: Sqls.Functions.SingleColumn); param.ParentId();
            column.InheritPermission(function: Sqls.Functions.SingleColumn); param.InheritPermission();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            if (!Body.InitialValue())
            {
                column.Body(function: Sqls.Functions.SingleColumn);
                param.Body();
            }
            if (!SiteSettings.InitialValue())
            {
                column.SiteSettings(function: Sqls.Functions.SingleColumn);
                param.SiteSettings();
            }
            if (!Comments.InitialValue())
            {
                column.Comments(function: Sqls.Functions.SingleColumn);
                param.Comments();
            }
            return Rds.InsertSites(
                tableType: tableType,
                param: param,
                select: Rds.SelectSites(column: column, where: where),
                addUpdatorParam: false);
        }

        public void UpdateRelatedRecords(
            RdsUser rdsUser = null,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            var fullText = FullText(SiteSettings);
            var statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                where: Rds.ItemsWhere().ReferenceId(SiteId),
                param: Rds.ItemsParam()
                    .SiteId(SiteId)
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                addUpdatedTimeParam: addUpdatedTimeParam,
                addUpdatorParam: addUpdatorParam,
                _using: updateItems));
            statements.Add(Rds.PhysicalDeleteLinks(
                where: Rds.LinksWhere().SourceId(SiteId)));
            statements.Add(LinkUtilities.Insert(SiteSettings.Links
                .Select(o => o.SiteId)
                .Distinct()
                .ToDictionary(o => o, o => SiteId)));
            Rds.ExecuteNonQuery(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            Libraries.Search.Indexes.Create(SiteSettings, this);
        }

        public Error.Types UpdateOrCreate(
            RdsUser rdsUser = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Sites")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.UpdateOrInsertSites(
                    selectIdentity: true,
                    where: where ?? Rds.SitesWhereDefault(this),
                    param: param ?? Rds.SitesParamDefault(this, setDefault: true)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.SitesUpdated)
            };
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            SiteId = newId != 0 ? newId : SiteId;
            Get();
            Libraries.Search.Indexes.Create(SiteSettings, this);
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            var siteMenu = SiteInfo.TenantCaches[Sessions.TenantId()]
                .SiteMenu.Children(SiteId, withParent: true);
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteItems(
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
                        where: Rds.SitesWhere().SiteId(SiteId)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.SitesUpdated)
                });
            Libraries.Search.Indexes.Create(SiteSettings, this);
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteSites(
                    tableType: tableType,
                    param: Rds.SitesParam().TenantId(TenantId).SiteId(SiteId)));
            Libraries.Search.Indexes.Create(SiteSettings, this);
            return Error.Types.None;
        }

        public void SetByForm()
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
                    case "Comments": Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                controlId.Substring("Comment".Length).ToInt(),
                                Forms.Data(controlId));
                        }
                        break;
                }
            });
            SetSiteSettings();
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
        }

        public void SetByApi()
        {
            var data = Forms.String().Deserialize<SiteApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.Title != null) Title = new Title(SiteId, data.Title);
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.ReferenceType != null) ReferenceType = data.ReferenceType.ToString().ToString();
            if (data.InheritPermission != null) InheritPermission = data.InheritPermission.ToLong().ToLong();
            if (data.Comments != null) Comments.Prepend(data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            SetSiteSettings();
        }

        private bool Matched(SiteSettings ss, View view)
        {
            if (view.ColumnFilterHash != null)
            {
                foreach (var filter in view.ColumnFilterHash)
                {
                    var match = true;
                    var column = ss.GetColumn(filter.Key);
                    switch (filter.Key)
                    {
                        case "UpdatedTime": match = UpdatedTime.Value.Matched(column, filter.Value); break;
                        case "CreatedTime": match = CreatedTime.Value.Matched(column, filter.Value); break;
                    }
                    if (!match) return false;
                }
            }
            return true;
        }

        private void SetBySession()
        {
            if (!Forms.HasData("Sites_SiteSettings")) SiteSettings = Session_SiteSettings();
            if (!Forms.HasData("Sites_MonitorChangesColumns")) MonitorChangesColumns = Session_MonitorChangesColumns();
            if (!Forms.HasData("Sites_TitleColumns")) TitleColumns = Session_TitleColumns();
            if (!Forms.HasData("Sites_Export")) Export = Session_Export();
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

        private void Set(DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "TenantId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                TenantId = dataRow[column.ColumnName].ToInt();
                                SavedTenantId = TenantId;
                            }
                            break;
                        case "SiteId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                SiteId = dataRow[column.ColumnName].ToLong();
                                SavedSiteId = SiteId;
                            }
                            break;
                        case "UpdatedTime":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                UpdatedTime = new Time(dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                                SavedUpdatedTime = UpdatedTime.Value;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Title":
                            Title = new Title(dataRow, "SiteId");
                            SavedTitle = Title.Value;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "ReferenceType":
                            ReferenceType = dataRow[column.ColumnName].ToString();
                            SavedReferenceType = ReferenceType;
                            break;
                        case "ParentId":
                            ParentId = dataRow[column.ColumnName].ToLong();
                            SavedParentId = ParentId;
                            break;
                        case "InheritPermission":
                            InheritPermission = dataRow[column.ColumnName].ToLong();
                            SavedInheritPermission = InheritPermission;
                            break;
                        case "SiteSettings":
                            SiteSettings = GetSiteSettings(dataRow);
                            SavedSiteSettings = SiteSettings.RecordingJson();
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated()
        {
            return
                TenantId_Updated() ||
                Ver_Updated() ||
                Title_Updated() ||
                Body_Updated() ||
                ReferenceType_Updated() ||
                ParentId_Updated() ||
                InheritPermission_Updated() ||
                SiteSettings_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
        }

        public List<string> Mine()
        {
            var mine = new List<string>();
            var userId = Sessions.UserId();
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types Create(bool otherInitValue = false)
        {
            if (!otherInitValue) SiteSettings = new SiteSettings(ReferenceType);
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
                            .ReferenceId(raw: Def.Sql.Identity)
                            .DeptId(0)
                            .UserId(Sessions.UserId())
                            .PermissionType(Permissions.Manager()),
                        _using: InheritPermission == 0),
                    StatusUtilities.UpdateStatus(StatusUtilities.Types.SitesUpdated)
                });
            SiteId = newId != 0 ? newId : SiteId;
            Get();
            SiteSettings = SiteSettingsUtilities.Get(this, SiteId);
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
                    wikiModel.Create(SiteSettings);
                    break;
                default:
                    Libraries.Search.Indexes.Create(SiteSettings, this);
                    break;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SiteSettings GetSiteSettings(DataRow dataRow)
        {
            return dataRow.String("SiteSettings").Deserialize<SiteSettings>() ??
                new SiteSettings(ReferenceType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsPropertiesBySession()
        {
            SiteSettings = Session_SiteSettings();
            SiteSettings.InheritPermission = InheritPermission;
            SetSiteSettingsProperties();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsProperties()
        {
            if (SiteSettings == null)
            {
                SiteSettings = SiteSettingsUtilities.SitesSiteSettings(SiteId);
            }
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
            var invalidFormat = string.Empty;
            var invalid = SiteValidators.OnSetSiteSettings(
                SiteSettingsUtilities.Get(this, SiteId), out invalidFormat);
            switch (invalid)
            {
                case Error.Types.BadFormat:
                    return Messages.ResponseBadFormat(invalidFormat).ToJson();
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var res = new SitesResponseCollection(this);
            SetSiteSettingsPropertiesBySession();
            SetSiteSettings(res);
            Session_SiteSettings(SiteSettings.ToJson());
            return res
                .SetMemory("formChanged", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteSettings(ResponseCollection res)
        {
            var controlId = Forms.ControlId();
            SiteSettings.SetJoinedSsHash();
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
                case "GridJoin":
                    SetGridColumnsSelectable(res);
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
                case "FilterJoin":
                    SetFilterColumnsSelectable(res);
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
                case "ResetEditorColumn":
                    ResetEditorColumn(res);
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
                case "MoveUpSummaries":
                case "MoveDownSummaries":
                    SetSummariesOrder(res, controlId);
                    break;
                case "NewSummary":
                case "EditSummary":
                    OpenSummaryDialog(res, controlId);
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
                case "UpdateSummary":
                    UpdateSummary(res);
                    break;
                case "DeleteSummaries":
                    DeleteSummaries(res);
                    break;
                case "MoveUpFormulas":
                case "MoveDownFormulas":
                    SetFormulasOrder(res, controlId);
                    break;
                case "NewFormula":
                case "EditFormula":
                    OpenFormulaDialog(res, controlId);
                    break;
                case "AddFormula":
                    AddFormula(res);
                    break;
                case "UpdateFormula":
                    UpdateFormula(res);
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
                case "AddView":
                    AddView(res);
                    break;
                case "UpdateView":
                    UpdateView(res);
                    break;
                case "DeleteViews":
                    DeleteViews(res);
                    break;
                case "MoveUpNotifications":
                case "MoveDownNotifications":
                    SetNotificationsOrder(res, controlId);
                    break;
                case "NewNotification":
                case "EditNotification":
                    OpenNotificationDialog(res, controlId);
                    break;
                case "AddNotification":
                    AddNotification(res, controlId);
                    break;
                case "UpdateNotification":
                    UpdateNotification(res, controlId);
                    break;
                case "DeleteNotifications":
                    DeleteNotifications(res);
                    break;
                case "MoveUpMonitorChangesColumns":
                case "MoveDownMonitorChangesColumns":
                case "ToDisableMonitorChangesColumns":
                case "ToEnableMonitorChangesColumns":
                    SetMonitorChangesColumns(res, controlId);
                    break;
                case "NewReminder":
                case "EditReminder":
                    OpenReminderDialog(res, controlId);
                    break;
                case "AddReminder":
                    AddReminder(res, controlId);
                    break;
                case "UpdateReminder":
                    UpdateReminder(res, controlId);
                    break;
                case "DeleteReminders":
                    DeleteReminders(res);
                    break;
                case "MoveUpReminders":
                case "MoveDownReminders":
                    SetRemindersOrder(res, controlId);
                    break;
                case "TestReminders":
                    TestReminders(res);
                    break;
                case "MoveUpExports":
                case "MoveDownExports":
                    SetExportsOrder(res, controlId);
                    break;
                case "NewExport":
                case "EditExport":
                    OpenExportDialog(res, controlId);
                    break;
                case "AddExport":
                    AddExport(res, controlId);
                    break;
                case "UpdateExport":
                    UpdateExport(res, controlId);
                    break;
                case "DeleteExports":
                    DeleteExports(res);
                    break;
                case "ExportJoin":
                    SetExportColumnsSelectable(res);
                    break;
                case "SearchExportColumns":
                    SetExportColumnsSelectableBySearch(res);
                    break;
                case "MoveUpExportColumns":
                case "MoveDownExportColumns":
                case "ToDisableExportColumns":
                case "ToEnableExportColumns":
                    SetExportColumns(res, controlId);
                    break;
                case "OpenExportColumnsDialog":
                    OpenExportColumnsDialog(res, controlId);
                    break;
                case "UpdateExportColumn":
                    UpdateExportColumns(res, controlId);
                    break;
                case "MoveUpStyles":
                case "MoveDownStyles":
                    SetStylesOrder(res, controlId);
                    break;
                case "NewStyle":
                case "EditStyle":
                    OpenStyleDialog(res, controlId);
                    break;
                case "AddStyle":
                    AddStyle(res, controlId);
                    break;
                case "UpdateStyle":
                    UpdateStyle(res, controlId);
                    break;
                case "DeleteStyles":
                    DeleteStyles(res);
                    break;
                case "MoveUpScripts":
                case "MoveDownScripts":
                    SetScriptsOrder(res, controlId);
                    break;
                case "NewScript":
                case "EditScript":
                    OpenScriptDialog(res, controlId);
                    break;
                case "AddScript":
                    AddScript(res, controlId);
                    break;
                case "UpdateScript":
                    UpdateScript(res, controlId);
                    break;
                case "DeleteScripts":
                    DeleteScripts(res);
                    break;
                default:
                    Forms.All()
                        .Where(o => o.Key != controlId)
                        .ForEach(data =>
                            SiteSettings.Set(data.Key, data.Value));
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HtmlBuilder ReplaceSiteMenu(long sourceId, long destinationId)
        {
            var siteMenu = SiteInfo.TenantCaches[Sessions.TenantId()].SiteMenu;
            return new HtmlBuilder().SiteMenu(
                ss: SiteSettings,
                siteId: destinationId,
                referenceType: ReferenceType,
                title: siteMenu.Get(destinationId).Title,
                siteConditions: siteMenu.SiteConditions(SiteId));
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
                SiteSettings.GridSelectableOptions(enabled: false, join: Forms.Data("GridJoin")),
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
                var column = SiteSettings.GetColumn(selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest());
                }
                else if(column.Joined)
                {
                    res.Message(Messages.CanNotPerformed());
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
                        selectedValueTextCollection: columnName.ToSingleList()))
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
        private void SetGridColumnsSelectable(ResponseCollection res)
        {
            var listItemCollection = SiteSettings
                .GridSelectableOptions(enabled: false, join: Forms.Data("GridJoin"));
            if (!listItemCollection.Any())
            {
                res.Message(Messages.NotFound());
            }
            else
            {
                res.Html("#GridSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: listItemCollection));
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
                SiteSettings.FilterSelectableOptions(
                    enabled: false, join: Forms.Data("FilterJoin")),
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
                var column = SiteSettings.GetColumn(selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest());
                }
                else if (column.Joined)
                {
                    res.Message(Messages.CanNotPerformed());
                }
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
                        selectedValueTextCollection: columnName.ToSingleList()))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFilterColumnsSelectable(ResponseCollection res)
        {
            var listItemCollection = SiteSettings
                .FilterSelectableOptions(enabled: false, join: Forms.Data("FilterJoin"));
            if (!listItemCollection.Any())
            {
                res.Message(Messages.NotFound());
            }
            else
            {
                res.Html("#FilterSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: listItemCollection));
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
                    .SetFormData("AggregationDestination", selectedColumns?.ToJson());
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
                    .SetFormData("AggregationDestination", selectedColumns?.ToJson());
            }
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
                selectedColumns.Any(o => SiteSettings.EditorColumn(o).Required))
            {
                res.Message(Messages.CanNotDisabled(
                    SiteSettings.EditorColumn(selectedColumns.FirstOrDefault(o =>
                        SiteSettings.EditorColumn(o).Required)).LabelText));
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
                        selectedValueTextCollection: columnName.ToSingleList()))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ResetEditorColumn(ResponseCollection res)
        {
            var ss = new SiteSettings(ReferenceType);
            res.Html(
                "#EditorColumnDialog",
                SiteUtilities.EditorColumnDialog(
                    ss: SiteSettings,
                    column: ss.GetColumn(Forms.Data("EditorColumnName")),
                    titleColumns: ss.TitleColumns));
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
        private void SetSummariesOrder(ResponseCollection res, string controlId)
        {
            var selected = Forms.IntList("EditSummary");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                SiteSettings.Summaries.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditSummary", new HtmlBuilder()
                    .EditSummary(ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenSummaryDialog(ResponseCollection res, string controlId)
        {
            SiteSettings.SetLinkedSiteSettings();
            if (SiteSettings.Destinations?.Any() != true)
            {
                res.Message(Messages.NoLinks());
            }
            else
            {
                if (controlId == "NewSummary")
                {
                    OpenSummaryDialog(
                        res, new Summary(SiteSettings.Destinations.FirstOrDefault().SiteId));
                }
                else
                {
                    var summary = SiteSettings.Summaries?.Get(Forms.Int("SummaryId"));
                    if (summary == null)
                    {
                        OpenDialogError(res, Messages.SelectOne());
                    }
                    else
                    {
                        SiteSettingsUtilities.Get(this, SiteId);
                        OpenSummaryDialog(res, summary);
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenSummaryDialog(ResponseCollection res, Summary summary)
        {
            res.Html("#SummaryDialog", SiteUtilities.SummaryDialog(
                ss: SiteSettings,
                controlId: Forms.ControlId(),
                summary: summary));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummarySiteId(ResponseCollection res)
        {
            var siteId = Forms.Long("SummarySiteId");
            var destinationSiteModel = new SiteModel(siteId);
            res
                .ReplaceAll("#SummaryDestinationColumnField", new HtmlBuilder()
                    .SummaryDestinationColumn(destinationSiteModel.SiteSettings))
                .ReplaceAll("#SummaryLinkColumnField", new HtmlBuilder()
                    .SummaryLinkColumn(
                        ss: SiteSettings,
                        siteId: siteId));
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
            SiteSettings.SetLinkedSiteSettings();
            var siteId = Forms.Long("SummarySiteId");
            var destinationSs = SiteSettings.Destinations.Get(siteId);
            int? destinationCondition = Forms.Int("SummaryDestinationCondition");
            int? sourceCondition = Forms.Int("SummarySourceCondition");
            var error = SiteSettings.AddSummary(
                siteId,
                new SiteModel(Forms.Long("SummarySiteId")).ReferenceType,
                Forms.Data("SummaryDestinationColumn"),
                destinationSs?.Views?.Get(destinationCondition)?.Id,
                Forms.Bool("SummarySetZeroWhenOutOfCondition"),
                Forms.Data("SummaryLinkColumn"),
                Forms.Data("SummaryType"),
                Forms.Data("SummarySourceColumn"),
                SiteSettings.Views?.Get(sourceCondition)?.Id);
            if (error.Has())
            {
                res.Message(error.Message());
            }
            else
            {
                res
                    .ReplaceAll("#EditSummary", new HtmlBuilder()
                        .EditSummary(ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateSummary(ResponseCollection res)
        {
            SiteSettings.SetLinkedSiteSettings();
            var siteId = Forms.Long("SummarySiteId");
            var destinationSs = SiteSettings.Destinations.Get(siteId);
            int? destinationCondition = Forms.Int("SummaryDestinationCondition");
            int? sourceCondition = Forms.Int("SummarySourceCondition");
            var outOfCondition = Forms.Data("SummaryOutOfCondition").Trim();
            var error = SiteSettings.UpdateSummary(
                Forms.Int("SummaryId"),
                siteId,
                new SiteModel(Forms.Long("SummarySiteId")).ReferenceType,
                Forms.Data("SummaryDestinationColumn"),
                destinationSs?.Views?.Get(destinationCondition)?.Id,
                Forms.Bool("SummarySetZeroWhenOutOfCondition"),
                Forms.Data("SummaryLinkColumn"),
                Forms.Data("SummaryType"),
                Forms.Data("SummarySourceColumn"),
                SiteSettings.Views?.Get(sourceCondition)?.Id);
            if (error.Has())
            {
                res.Message(error.Message());
            }
            else
            {
                res
                    .Html("#EditSummary", new HtmlBuilder()
                        .EditSummary(ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteSummaries(ResponseCollection res)
        {
            var selected = Forms.IntList("EditSummary");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                SiteSettings.Summaries.Delete(selected);
                res.ReplaceAll("#EditSummary", new HtmlBuilder()
                    .EditSummary(ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFormulasOrder(ResponseCollection res, string controlId)
        {
            var selected = Forms.IntList("EditFormula");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                SiteSettings.Formulas.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditFormula", new HtmlBuilder()
                    .EditFormula(ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenFormulaDialog(ResponseCollection res, string controlId)
        {
            if (controlId == "NewFormula")
            {
                var formulaSet = new FormulaSet();
                OpenFormulaDialog(res, formulaSet);
            }
            else
            {
                var formulaSet = SiteSettings.Formulas?.Get(Forms.Int("FormulaId"));
                if (formulaSet == null)
                {
                    OpenDialogError(res, Messages.SelectOne());
                }
                else
                {
                    SiteSettingsUtilities.Get(this, SiteId);
                    OpenFormulaDialog(res, formulaSet);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenFormulaDialog(ResponseCollection res, FormulaSet formulaSet)
        {
            res.Html("#FormulaDialog", SiteUtilities.FormulaDialog(
                ss: SiteSettings, controlId: Forms.ControlId(), formulaSet: formulaSet));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddFormula(ResponseCollection res)
        {
            var outOfCondition = Forms.Data("FormulaOutOfCondition").Trim();
            var error = SiteSettings.AddFormula(
                Forms.Data("FormulaTarget"),
                Forms.Int("FormulaCondition"),
                Forms.Data("Formula"),
                outOfCondition != string.Empty
                    ? outOfCondition
                    : null);
            if (error.Has())
            {
                res.Message(error.Message());
            }
            else
            {
                res
                    .ReplaceAll("#EditFormula", new HtmlBuilder()
                        .EditFormula(ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateFormula(ResponseCollection res)
        {
            var id = Forms.Int("FormulaId");
            var outOfCondition = Forms.Data("FormulaOutOfCondition").Trim();
            var error = SiteSettings.UpdateFormula(
                id,
                Forms.Data("FormulaTarget"),
                Forms.Int("FormulaCondition"),
                Forms.Data("Formula"),
                outOfCondition != string.Empty
                    ? outOfCondition
                    : null);
            if (error.Has())
            {
                res.Message(error.Message());
            }
            else
            {
                res
                    .Html("#EditFormula", new HtmlBuilder()
                        .EditFormula(ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteFormulas(ResponseCollection res)
        {
            var selected = Forms.IntList("EditFormula");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                SiteSettings.Formulas.Delete(selected);
                res.ReplaceAll("#EditFormula", new HtmlBuilder()
                    .EditFormula(ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetViewsOrder(ResponseCollection res, string controlId)
        {
            var command = ColumnUtilities.ChangeCommand(controlId);
            var selectedColumns = Forms.IntList("Views");
            SiteSettings.SetViewsOrder(command, selectedColumns);
            res
                .Html(
                    "#Views",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.ViewSelectableOptions(),
                        selectedValueTextCollection: selectedColumns.Select(o => o.ToString())))
                .SetFormData("Views", selectedColumns.ToJson());
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
                var idList = Forms.IntList("Views");
                if (idList.Count() != 1)
                {
                    OpenDialogError(res, Messages.SelectOne());
                }
                else
                {
                    view = SiteSettings.Views?.Get(idList.First());
                    if (view == null)
                    {
                        OpenDialogError(res, Messages.SelectOne());
                    }
                    else
                    {
                        SiteSettingsUtilities.Get(this, SiteId);
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
        private void AddViewFilter(ResponseCollection res)
        {
            SiteSettingsUtilities.Get(this, SiteId);
            res
                .Append(
                    "#ViewFiltersTab .items",
                    new HtmlBuilder().ViewFilter(
                        ss: SiteSettings,
                        column: SiteSettings.GetColumn(Forms.Data("ViewFilterSelector"))))
                .Remove("#ViewFilterSelector option:selected");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddView(ResponseCollection res)
        {
            SiteSettings.AddView(new View(SiteSettings));
            res
                .ViewResponses(SiteSettings, new List<int>
                {
                    SiteSettings.ViewLatestId.ToInt()
                })
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateView(ResponseCollection res)
        {
            var selected = Forms.Int("ViewId");
            var view = SiteSettings.Views?.Get(selected);
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
            SiteSettings.Views?.RemoveAll(o =>
                Forms.IntList("Views").Contains(o.Id));
            res.ViewResponses(SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenNotificationDialog(ResponseCollection res, string controlId)
        {
            if (controlId == "NewNotification")
            {
                OpenNotificationDialog(res, new Notification(
                    Notification.Types.Mail,
                    SiteSettings.ColumnDefinitionHash.MonitorChangesDefinitions()
                        .Select(o => o.ColumnName)
                        .Where(o => SiteSettings.EditorColumns.Contains(o) || o == "Comments")
                        .ToList()));
            }
            else
            {
                var notification = SiteSettings.Notifications?.Get(Forms.Int("NotificationId"));
                if (notification == null)
                {
                    OpenDialogError(res, Messages.SelectOne());
                }
                else
                {
                    SiteSettingsUtilities.Get(this, SiteId);
                    OpenNotificationDialog(res, notification);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenNotificationDialog(ResponseCollection res, Notification notification)
        {
            if (!Contract.Notice())
            {
                res.Message(Messages.Restricted());
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
        private void AddNotification(ResponseCollection res, string controlId)
        {
            if (!Contract.Notice())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                SiteSettings.Notifications.Add(new Notification(
                    SiteSettings.Notifications.MaxOrDefault(o => o.Id) + 1,
                    (Notification.Types)Forms.Int("NotificationType"),
                    Forms.Data("NotificationPrefix"),
                    Forms.Data("NotificationAddress"),
                    Forms.Data("NotificationToken"),
                    Session_MonitorChangesColumns(),
                    Forms.Int("BeforeCondition"),
                    Forms.Int("AfterCondition"),
                    (Notification.Expressions)Forms.Int("Expression")));
                SetNotificationsResponseCollection(res);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateNotification(ResponseCollection res, string controlId)
        {
            if (!Contract.Notice())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var notification = SiteSettings.Notifications.Get(Forms.Int("NotificationId"));
                if (notification == null)
                {
                    res.Message(Messages.NotFound());
                }
                else
                {
                    notification.Update(
                        (Notification.Types)Forms.Int("NotificationType"),
                        Forms.Data("NotificationPrefix"),
                        Forms.Data("NotificationAddress"),
                        Forms.Data("NotificationToken"),
                        Session_MonitorChangesColumns(),
                        Forms.Int("BeforeCondition"),
                        Forms.Int("AfterCondition"),
                        (Notification.Expressions)Forms.Int("Expression"));
                    SetNotificationsResponseCollection(res);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetNotificationsOrder(ResponseCollection res, string controlId)
        {
            if (!Contract.Notice())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var selected = Forms.IntList("EditNotification");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets()).ToJson();
                }
                else
                {
                    SiteSettings.Notifications.MoveUpOrDown(
                        ColumnUtilities.ChangeCommand(controlId), selected);
                    res.Html("#EditNotification", new HtmlBuilder()
                        .EditNotification(ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetNotificationsResponseCollection(ResponseCollection res)
        {
            if (!Contract.Notice())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                res
                    .ReplaceAll("#EditNotification", new HtmlBuilder()
                        .EditNotification(ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetMonitorChangesColumns(ResponseCollection res, string controlId)
        {
            if (!Contract.Notice())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var notification = SiteSettings.Notifications.Get(Forms.Int("NotificationId"));
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
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteNotifications(ResponseCollection res)
        {
            if (!Contract.Notice())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var selected = Forms.IntList("EditNotification");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets()).ToJson();
                }
                else
                {
                    SiteSettings.Notifications.Delete(selected);
                    res.ReplaceAll("#EditNotification", new HtmlBuilder()
                        .EditNotification(ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenReminderDialog(ResponseCollection res, string controlId)
        {
            if (controlId == "NewReminder")
            {
                OpenReminderDialog(res, new Reminder() { Subject = Title.Value });
            }
            else
            {
                var reminder = SiteSettings.Reminders?.Get(Forms.Int("ReminderId"));
                if (reminder == null)
                {
                    OpenDialogError(res, Messages.SelectOne());
                }
                else
                {
                    SiteSettingsUtilities.Get(this, SiteId);
                    OpenReminderDialog(res, reminder);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenReminderDialog(ResponseCollection res, Reminder reminder)
        {
            if (!Contract.Remind())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                res.Html("#ReminderDialog", SiteUtilities.ReminderDialog(
                    ss: SiteSettings,
                    controlId: Forms.ControlId(),
                    reminder: reminder));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddReminder(ResponseCollection res, string controlId)
        {
            if (!Contract.Remind())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var invalidMailAddress = string.Empty;
                var invalid = SiteValidators.SetReminder(out invalidMailAddress);
                switch (invalid)
                {
                    case Error.Types.None:
                        SiteSettings.Reminders.Add(new Reminder(
                            id: SiteSettings.Reminders.MaxOrDefault(o => o.Id) + 1,
                            subject: Forms.Data("ReminderSubject"),
                            body: Forms.Data("ReminderBody"),
                            line: SiteSettings.LabelTextToColumnName(
                                Forms.Data("ReminderLine")),
                            from: Forms.Data("ReminderFrom"),
                            to: Forms.Data("ReminderTo"),
                            column: Forms.Data("ReminderColumn"),
                            startDateTime: Forms.DateTime("ReminderStartDateTime"),
                            type: (Times.RepeatTypes)Forms.Int("ReminderType"),
                            range: Forms.Int("ReminderRange"),
                            sendCompletedInPast: Forms.Bool("ReminderSendCompletedInPast"),
                            notSendIfNotApplicable: Forms.Bool("NotSendIfNotApplicable"),
                            condition: Forms.Int("ReminderCondition")));
                        SetRemindersResponseCollection(res);
                        break;
                    case Error.Types.BadMailAddress:
                    case Error.Types.ExternalMailAddress:
                        res.Message(invalid.Message(invalidMailAddress));
                        break;
                    default:
                        res.Message(invalid.Message());
                        break;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateReminder(ResponseCollection res, string controlId)
        {
            if (!Contract.Remind())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var reminder = SiteSettings.Reminders.Get(Forms.Int("ReminderId"));
                if (reminder == null)
                {
                    res.Message(Messages.NotFound());
                }
                else
                {
                    var invalidMailAddress = string.Empty;
                    var invalid = SiteValidators.SetReminder(out invalidMailAddress);
                    switch (invalid)
                    {
                        case Error.Types.None:
                            reminder.Update(
                                subject: Forms.Data("ReminderSubject"),
                                body: Forms.Data("ReminderBody"),
                                line: SiteSettings.LabelTextToColumnName(
                                    Forms.Data("ReminderLine")),
                                from: Forms.Data("ReminderFrom"),
                                to: Forms.Data("ReminderTo"),
                                column: Forms.Data("ReminderColumn"),
                                startDateTime: Forms.DateTime("ReminderStartDateTime"),
                                type: (Times.RepeatTypes)Forms.Int("ReminderType"),
                                range: Forms.Int("ReminderRange"),
                                sendCompletedInPast: Forms.Bool("ReminderSendCompletedInPast"),
                                notSendIfNotApplicable: Forms.Bool("NotSendIfNotApplicable"),
                                condition: Forms.Int("ReminderCondition"));
                            SetRemindersResponseCollection(res);
                            break;
                        case Error.Types.BadMailAddress:
                        case Error.Types.ExternalMailAddress:
                            res.Message(invalid.Message(invalidMailAddress));
                            break;
                        default:
                            res.Message(invalid.Message());
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetRemindersOrder(ResponseCollection res, string controlId)
        {
            if (!Contract.Remind())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var selected = Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets()).ToJson();
                }
                else
                {
                    SiteSettings.Reminders.MoveUpOrDown(
                        ColumnUtilities.ChangeCommand(controlId), selected);
                    res.Html("#EditReminder", new HtmlBuilder()
                        .EditReminder(ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetRemindersResponseCollection(ResponseCollection res)
        {
            if (!Contract.Remind())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                res
                    .ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteReminders(ResponseCollection res)
        {
            if (!Contract.Remind())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var selected = Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets()).ToJson();
                }
                else
                {
                    SiteSettings.Reminders.Delete(selected);
                    res.ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void TestReminders(ResponseCollection res)
        {
            if (!Contract.Remind())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var selected = Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets()).ToJson();
                }
                else
                {
                    SiteSettings.Remind(selected, test: true);
                    res.ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportDialog(ResponseCollection res, string controlId)
        {
            SiteSettings.SetExports();
            if (controlId == "NewExport")
            {
                OpenExportDialog(res, new Export(SiteSettings.DefaultExportColumns()));
            }
            else
            {
                var export = SiteSettings.Exports?.Get(Forms.Int("ExportId"));
                if (export == null)
                {
                    OpenDialogError(res, Messages.SelectOne());
                }
                else
                {
                    SiteSettingsUtilities.Get(this, SiteId);
                    OpenExportDialog(res, export);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportDialog(ResponseCollection res, Export export)
        {
            if (!Contract.Export())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                Session_Export(export);
                res.Html("#ExportDialog", SiteUtilities.ExportDialog(
                    ss: SiteSettings,
                    controlId: Forms.ControlId(),
                    export: export));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddExport(ResponseCollection res, string controlId)
        {
            if (!Contract.Export())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                SiteSettings.SetExports();
                var export = Session_Export();
                export.Id = SiteSettings.Exports.MaxOrDefault(o => o.Id) + 1;
                export.Name = Forms.Data("ExportName");
                export.Header = Forms.Bool("ExportHeader");
                export.Join = Forms.Data("ExportJoin").Deserialize<Join>();
                SiteSettings.Exports.Add(export);
                SetExportsResponseCollection(res);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateExport(ResponseCollection res, string controlId)
        {
            if (!Contract.Export())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var export = SiteSettings.Exports.Get(Forms.Int("ExportId"));
                if (export == null)
                {
                    res.Message(Messages.NotFound());
                }
                else
                {
                    SiteSettings.SetExports();
                    export.Update(
                        Forms.Data("ExportName"),
                        Forms.Bool("ExportHeader"),
                        Forms.Data("ExportJoin").Deserialize<Join>(),
                        Session_Export().Columns);
                    SetExportsResponseCollection(res);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportsOrder(ResponseCollection res, string controlId)
        {
            if (!Contract.Export())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var selected = Forms.IntList("EditExport");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets()).ToJson();
                }
                else
                {
                    SiteSettings.Exports.MoveUpOrDown(
                        ColumnUtilities.ChangeCommand(controlId), selected);
                    res.Html("#EditExport", new HtmlBuilder()
                        .EditExport(ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportsResponseCollection(ResponseCollection res)
        {
            if (!Contract.Export())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                res
                    .ReplaceAll("#EditExport", new HtmlBuilder()
                        .EditExport(ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportColumnsSelectable(ResponseCollection res)
        {
            SiteSettings.SetExports();
            var join = Forms.Data("ExportJoin").Deserialize<Join>();
            var searchText = Forms.Data("SearchExportColumns");
            var current = new List<ExportColumn>();
            var sources = new List<ExportColumn>();
            var allows = join?
                .Where(o => SiteSettings.JoinedSsHash?.ContainsKey(o.SiteId) == true)
                .ToList();
            allows?.Reverse();
            allows?.ForEach(link =>
            {
                var ss = SiteSettings.JoinedSsHash.Get(link.SiteId);
                current.AddRange(ss.DefaultExportColumns());
                sources.AddRange(ss.ExportColumns(searchText));
            });
            current.AddRange(SiteSettings.DefaultExportColumns());
            sources.AddRange(SiteSettings.ExportColumns(searchText));
            Session_Export(new Export(current));
            res
                .Html("#ExportColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: ExportUtilities
                        .CurrentColumnOptions(current)))
                .SetFormData("ExportColumns", "[]")
                .Html("#ExportSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: ExportUtilities
                        .SourceColumnOptions(sources)))
                .SetFormData("ExportSourceColumns", "[]");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportColumnsSelectableBySearch(ResponseCollection res)
        {
            SiteSettings.SetExports();
            var join = Forms.Data("ExportJoin").Deserialize<Join>();
            var searchText = Forms.Data("SearchExportColumns");
            var sources = new List<ExportColumn>();
            var allows = join?
                .Where(o => SiteSettings.JoinedSsHash?.ContainsKey(o.SiteId) == true)
                .ToList();
            allows?.Reverse();
            allows?.ForEach(link => sources.AddRange(
                SiteSettings.JoinedSsHash.Get(link.SiteId).ExportColumns(searchText)));
            sources.AddRange(SiteSettings.ExportColumns(searchText));
            res
                .Html("#ExportSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: ExportUtilities
                        .SourceColumnOptions(sources)))
                .SetFormData("ExportSourceColumns", "[]");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportColumns(ResponseCollection res, string controlId)
        {
            if (!Contract.Export())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                Export = Session_Export();
                if (Export == null && controlId == "EditExport")
                {
                    res.Message(Messages.NotFound());
                }
                else
                {
                    SiteSettings.SetExport(Export);
                    var command = ColumnUtilities.ChangeCommand(controlId);
                    var selectedColumns = Forms.List("ExportColumns");
                    var selectedSourceColumns = Forms.List("ExportSourceColumns");
                    switch (command)
                    {
                        case "ToDisable":
                            Export.Columns.RemoveAll(o =>
                                selectedColumns.Any(p => p == o.Id.ToString()));
                            selectedColumns = new List<string>();
                            break;
                        case "ToEnable":
                            var columns = new List<string>();
                            selectedSourceColumns
                                .Select(o => o.Deserialize<ExportColumn>())
                                .Where(o => o != null)
                                .ForEach(exportColumn =>
                                {
                                    exportColumn.Id = Export.NewColumnId();
                                    exportColumn.Init(SiteSettings.JoinedSsHash
                                        .Get(exportColumn.SiteId));
                                    Export.Columns.Add(exportColumn);
                                    columns.Add(exportColumn.Id.ToString());
                                });
                            selectedColumns = columns;
                            ExportSourceColumnResponse(res);
                            break;
                        default:
                            Export.Columns = ColumnUtilities.GetChanged(
                                Export.Columns.Select(o => o.Id.ToString()).ToList(),
                                command,
                                selectedColumns,
                                selectedSourceColumns)
                                    .Select(o => Export.Columns
                                        .FirstOrDefault(p => o == p.Id.ToString()))
                                    .ToList();
                            break;
                    }
                    res
                        .Html("#ExportColumns", new HtmlBuilder()
                            .SelectableItems(
                                listItemCollection: ExportUtilities
                                    .CurrentColumnOptions(Export.Columns),
                                selectedValueTextCollection: selectedColumns))
                        .SetFormData("ExportColumns", selectedColumns.ToJson());
                    Session_Export(Export);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ExportSourceColumnResponse(ResponseCollection res)
        {
            res
                .Html("#ExportSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: ExportUtilities
                        .SourceColumnOptions(
                            SiteSettings,
                            Forms.Data("ExportJoin").Deserialize<Join>(),
                            Forms.Data("SearchExportColumns"))))
                .SetFormData("ExportSourceColumns", "[]");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteExports(ResponseCollection res)
        {
            if (!Contract.Export())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                var selected = Forms.IntList("EditExport");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets()).ToJson();
                }
                else
                {
                    SiteSettings.Exports.Delete(selected);
                    res.ReplaceAll("#EditExport", new HtmlBuilder()
                        .EditExport(ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportColumnsDialog(ResponseCollection res, string controlId)
        {
            Export = Session_Export();
            SiteSettings.SetExport(Export);
            var selected = Forms.List("ExportColumns");
            if (selected.Count() != 1)
            {
                res.Message(Messages.SelectOne());
            }
            else
            {
                var column = Export.Columns.FirstOrDefault(o =>
                    o.Id.ToString() == selected[0]);
                if (column == null)
                {
                    res.Message(Messages.NotFound());
                }
                else
                {
                    OpenExportColumnsDialog(res, column);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportColumnsDialog(ResponseCollection res, ExportColumn column)
        {
            if (!Contract.Export())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                res.Html("#ExportColumnsDialog", SiteUtilities.ExportColumnsDialog(
                    ss: SiteSettings,
                    controlId: Forms.ControlId(),
                    exportColumn: column));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateExportColumns(ResponseCollection res, string controlId)
        {
            if (!Contract.Export())
            {
                res.Message(Messages.Restricted());
            }
            else
            {
                Export = Session_Export();
                SiteSettings.SetExport(Export);
                var column = Export.Columns.FirstOrDefault(o =>
                    o.Id == Forms.Int("ExportColumnId"));
                if (column == null)
                {
                    res.Message(Messages.NotFound());
                }
                else
                {
                    var selected = new List<string> { column.Id.ToString() };
                    column.Update(
                        Forms.Data("ExportColumnLabelText"),
                        (ExportColumn.Types)Forms.Int("ExportColumnType"),
                        Forms.Data("ExportFormat"));
                    res
                        .Html("#ExportColumns", new HtmlBuilder().SelectableItems(
                            listItemCollection: ExportUtilities
                                .CurrentColumnOptions(Export.Columns),
                            selectedValueTextCollection: selected))
                        .SetFormData("ExportColumns", selected.ToJson())
                        .CloseDialog("#ExportColumnsDialog");
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetStylesOrder(ResponseCollection res, string controlId)
        {
            var selected = Forms.IntList("EditStyle");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                SiteSettings.Styles.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditStyle", new HtmlBuilder()
                    .EditStyle(ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenStyleDialog(ResponseCollection res, string controlId)
        {
            if (controlId == "NewStyle")
            {
                var style = new Style() { All = true };
                OpenStyleDialog(res, style);
            }
            else
            {
                var style = SiteSettings.Styles?.Get(Forms.Int("StyleId"));
                if (style == null)
                {
                    OpenDialogError(res, Messages.SelectOne());
                }
                else
                {
                    SiteSettingsUtilities.Get(this, SiteId);
                    OpenStyleDialog(res, style);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenStyleDialog(ResponseCollection res, Style style)
        {
            res.Html("#StyleDialog", SiteUtilities.StyleDialog(
                ss: SiteSettings, controlId: Forms.ControlId(), style: style));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddStyle(ResponseCollection res, string controlId)
        {
            SiteSettings.Styles.Add(new Style(
                id: SiteSettings.Styles.MaxOrDefault(o => o.Id) + 1,
                title: Forms.Data("StyleTitle"),
                all: Forms.Bool("StyleAll"),
                _new: Forms.Bool("StyleNew"),
                edit: Forms.Bool("StyleEdit"),
                index: Forms.Bool("StyleIndex"),
                calendar: Forms.Bool("StyleCalendar"),
                crosstab: Forms.Bool("StyleCrosstab"),
                gantt: Forms.Bool("StyleGantt"),
                burnDown: Forms.Bool("StyleBurnDown"),
                timeSeries: Forms.Bool("StyleTimeSeries"),
                kamban: Forms.Bool("StyleKamban"),
                imageLib: Forms.Bool("StyleImageLib"),
                body: Forms.Data("StyleBody")));
            res
                .ReplaceAll("#EditStyle", new HtmlBuilder()
                    .EditStyle(ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateStyle(ResponseCollection res, string controlId)
        {
            SiteSettings.Styles?
                .FirstOrDefault(o => o.Id == Forms.Int("StyleId"))?
                .Update(
                    title: Forms.Data("StyleTitle"),
                    all: Forms.Bool("StyleAll"),
                    _new: Forms.Bool("StyleNew"),
                    edit: Forms.Bool("StyleEdit"),
                    index: Forms.Bool("StyleIndex"),
                    calendar: Forms.Bool("StyleCalendar"),
                    crosstab: Forms.Bool("StyleCrosstab"),
                    gantt: Forms.Bool("StyleGantt"),
                    burnDown: Forms.Bool("StyleBurnDown"),
                    timeSeries: Forms.Bool("StyleTimeSeries"),
                    kamban: Forms.Bool("StyleKamban"),
                    imageLib: Forms.Bool("StyleImageLib"),
                    body: Forms.Data("StyleBody"));
            res
                .Html("#EditStyle", new HtmlBuilder()
                    .EditStyle(ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteStyles(ResponseCollection res)
        {
            var selected = Forms.IntList("EditStyle");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                SiteSettings.Styles.Delete(selected);
                res.ReplaceAll("#EditStyle", new HtmlBuilder()
                    .EditStyle(ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetScriptsOrder(ResponseCollection res, string controlId)
        {
            var selected = Forms.IntList("EditScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                SiteSettings.Scripts.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditScript", new HtmlBuilder()
                    .EditScript(ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenScriptDialog(ResponseCollection res, string controlId)
        {
            if (controlId == "NewScript")
            {
                var script = new Script() { All = true };
                OpenScriptDialog(res, script);
            }
            else
            {
                var script = SiteSettings.Scripts?.Get(Forms.Int("ScriptId"));
                if (script == null)
                {
                    OpenDialogError(res, Messages.SelectOne());
                }
                else
                {
                    SiteSettingsUtilities.Get(this, SiteId);
                    OpenScriptDialog(res, script);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenScriptDialog(ResponseCollection res, Script script)
        {
            res.Html("#ScriptDialog", SiteUtilities.ScriptDialog(
                ss: SiteSettings, controlId: Forms.ControlId(), script: script));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddScript(ResponseCollection res, string controlId)
        {
            SiteSettings.Scripts.Add(new Script(
                id: SiteSettings.Scripts.MaxOrDefault(o => o.Id) + 1,
                title: Forms.Data("ScriptTitle"),
                all: Forms.Bool("ScriptAll"),
                _new: Forms.Bool("ScriptNew"),
                edit: Forms.Bool("ScriptEdit"),
                index: Forms.Bool("ScriptIndex"),
                calendar: Forms.Bool("ScriptCalendar"),
                crosstab: Forms.Bool("ScriptCrosstab"),
                gantt: Forms.Bool("ScriptGantt"),
                burnDown: Forms.Bool("ScriptBurnDown"),
                timeSeries: Forms.Bool("ScriptTimeSeries"),
                kamban: Forms.Bool("ScriptKamban"),
                imageLib: Forms.Bool("ScriptImageLib"),
                body: Forms.Data("ScriptBody")));
            res
                .ReplaceAll("#EditScript", new HtmlBuilder()
                    .EditScript(ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateScript(ResponseCollection res, string controlId)
        {
            SiteSettings.Scripts?
                .FirstOrDefault(o => o.Id == Forms.Int("ScriptId"))?
                .Update(
                    title: Forms.Data("ScriptTitle"),
                    all: Forms.Bool("ScriptAll"),
                    _new: Forms.Bool("ScriptNew"),
                    edit: Forms.Bool("ScriptEdit"),
                    index: Forms.Bool("ScriptIndex"),
                    calendar: Forms.Bool("ScriptCalendar"),
                    crosstab: Forms.Bool("ScriptCrosstab"),
                    gantt: Forms.Bool("ScriptGantt"),
                    burnDown: Forms.Bool("ScriptBurnDown"),
                    timeSeries: Forms.Bool("ScriptTimeSeries"),
                    kamban: Forms.Bool("ScriptKamban"),
                    imageLib: Forms.Bool("ScriptImageLib"),
                    body: Forms.Data("ScriptBody"));
            res
                .Html("#EditScript", new HtmlBuilder()
                    .EditScript(ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteScripts(ResponseCollection res)
        {
            var selected = Forms.IntList("EditScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                SiteSettings.Scripts.Delete(selected);
                res.ReplaceAll("#EditScript", new HtmlBuilder()
                    .EditScript(ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool NotFound()
        {
            return SiteId != 0 && AccessStatus != Databases.AccessStatuses.Selected;
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
            Dictionary<string, ControlData> selectableOptions,
            List<string> selectedColumns,
            Dictionary<string, ControlData> selectableSourceOptions,
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
                .SetFormData(typeName + "Columns", selectedColumns.ToJson())
                .Html("#" + typeName + "SourceColumns",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: selectableSourceOptions,
                        selectedValueTextCollection: selectedSourceColumns))
                .SetFormData(typeName + "SourceColumns", selectedSourceColumns.ToJson());
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

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDialogError(ResponseCollection res, Message message)
        {
            res
                .Message(message)
                .CloseDialog();
        }
    }
}
