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
        public int TenantId = 0;
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

        [NonSerialized] public int SavedTenantId = 0;
        [NonSerialized] public string SavedReferenceType = "Sites";
        [NonSerialized] public long SavedParentId = 0;
        [NonSerialized] public long SavedInheritPermission = 0;
        [NonSerialized] public string SavedSiteSettings = string.Empty;
        [NonSerialized] public SiteCollection SavedAncestors = null;
        [NonSerialized] public int SavedSiteMenu = 0;
        [NonSerialized] public List<string> SavedMonitorChangesColumns = null;
        [NonSerialized] public List<string> SavedTitleColumns = null;
        [NonSerialized] public Export SavedExport = null;

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool ReferenceType_Updated(Context context, Column column = null)
        {
            return ReferenceType != SavedReferenceType && ReferenceType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ReferenceType);
        }

        public bool ParentId_Updated(Context context, Column column = null)
        {
            return ParentId != SavedParentId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != ParentId);
        }

        public bool InheritPermission_Updated(Context context, Column column = null)
        {
            return InheritPermission != SavedInheritPermission &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != InheritPermission);
        }

        public bool SiteSettings_Updated(Context context, Column column = null)
        {
            return SiteSettings.RecordingJson(context: context) != SavedSiteSettings && SiteSettings.RecordingJson(context: context) != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != SiteSettings.RecordingJson(context: context));
        }

        public SiteSettings Session_SiteSettings(Context context)
        {
            return this.PageSession(context: context, name: "SiteSettings") != null
                ? this.PageSession(context: context, name: "SiteSettings")?.ToString().Deserialize<SiteSettings>() ?? new SiteSettings(context: context, referenceType: ReferenceType)
                : SiteSettings;
        }

        public void Session_SiteSettings(Context context, object value)
        {
            this.PageSession(
                context: context,
                name: "SiteSettings",
                value: value);
        }

        public List<string> Session_MonitorChangesColumns(Context context)
        {
            return this.PageSession(context: context, name: "MonitorChangesColumns") != null
                ? this.PageSession(context: context, name: "MonitorChangesColumns") as List<string> ?? new List<string>()
                : MonitorChangesColumns;
        }

        public void Session_MonitorChangesColumns(Context context, object value)
        {
            this.PageSession(
                context: context,
                name: "MonitorChangesColumns",
                value: value);
        }

        public List<string> Session_TitleColumns(Context context)
        {
            return this.PageSession(context: context, name: "TitleColumns") != null
                ? this.PageSession(context: context, name: "TitleColumns") as List<string> ?? new List<string>()
                : TitleColumns;
        }

        public void Session_TitleColumns(Context context, object value)
        {
            this.PageSession(
                context: context,
                name: "TitleColumns",
                value: value);
        }

        public Export Session_Export(Context context)
        {
            return this.PageSession(context: context, name: "Export") != null
                ? this.PageSession(context: context, name: "Export") as Export ?? new Export()
                : Export;
        }

        public void Session_Export(Context context, object value)
        {
            this.PageSession(
                context: context,
                name: "Export",
                value: value);
        }

        public string PropertyValue(Context context, string name)
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
                case "SiteSettings": return SiteSettings.RecordingJson(context: context);
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

        public Dictionary<string, string> PropertyValues(Context context, IEnumerable<string> names)
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
                        hash.Add("SiteSettings", SiteSettings.RecordingJson(context: context));
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
            Context context,
            long parentId,
            long inheritPermission,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            ParentId = parentId;
            InheritPermission = inheritPermission;
            if (setByForm) SetByForm(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public SiteModel(
            Context context,
            long siteId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            SiteId = siteId;
            Get(context: context);
            if (clearSessions) ClearSessions(context: context);
            if (setByForm) SetByForm(context: context);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public SiteModel(Context context, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            if (dataRow != null) Set(context, dataRow, tableAlias);
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnConstructed(Context context)
        {
            SiteInfo.SetSiteUserHash(context: context, siteId: SiteId);
        }

        public void ClearSessions(Context context)
        {
            Session_SiteSettings(context: context, value: null);
            Session_MonitorChangesColumns(context: context, value: null);
            Session_TitleColumns(context: context, value: null);
            Session_Export(context: context, value: null);
        }

        public SiteModel Get(
            Context context,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(context, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    tableType: tableType,
                    column: column ?? Rds.SitesDefaultColumns(),
                    join: join ??  Rds.SitesJoinDefault(),
                    where: where ?? Rds.SitesWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            SetSiteSettingsProperties(context: context);
            return this;
        }

        public string FullText(
            Context context,
            SiteSettings ss,
            bool backgroundTask = false,
            bool onCreating = false)
        {
            if (Parameters.Search.Provider != "FullText") return null;
            if (!Parameters.Search.CreateIndexes && !backgroundTask) return null;
            if (AccessStatus == Databases.AccessStatuses.NotFound) return null;
            if (ReferenceType == "Wikis") return null;
            var fullText = new List<string>();
            SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu.Breadcrumb(context: context, siteId: SiteId)
                .FullText(context, fullText);
            SiteId.FullText(context, fullText);
            ss.EditorColumns.ForEach(columnName =>
            {
                switch (columnName)
                {
                    case "Title":
                        Title.FullText(context, fullText);
                        break;
                    case "Body":
                        Body.FullText(context, fullText);
                        break;
                    case "Comments":
                        Comments.FullText(context, fullText);
                        break;
                }
            });
            Creator.FullText(context, fullText);
            Updator.FullText(context, fullText);
            CreatedTime.FullText(context, fullText);
            UpdatedTime.FullText(context, fullText);
            if (!onCreating)
            {
                FullTextExtensions.OutgoingMailsFullText(
                    context: context,
                    fullText: fullText,
                    referenceType: "Sites",
                    referenceId: SiteId);
            }
            return fullText
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Distinct()
                .Join(" ");
        }

        public Dictionary<string, int> SearchIndexHash(Context context, SiteSettings ss)
        {
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            else
            {
                var searchIndexHash = new Dictionary<string, int>();
                SiteInfo.TenantCaches.Get(context.TenantId)?
                    .SiteMenu
                    .Breadcrumb(context: context, siteId: SiteId)
                    .SearchIndexes(context, searchIndexHash, 100);
                SiteId.SearchIndexes(context, searchIndexHash, 1);
                UpdatedTime.SearchIndexes(context, searchIndexHash, 200);
                Title.SearchIndexes(context, searchIndexHash, 4);
                Body.SearchIndexes(context, searchIndexHash, 200);
                Comments.SearchIndexes(context, searchIndexHash, 200);
                Creator.SearchIndexes(context, searchIndexHash, 100);
                Updator.SearchIndexes(context, searchIndexHash, 100);
                CreatedTime.SearchIndexes(context, searchIndexHash, 200);
                SearchIndexExtensions.OutgoingMailsSearchIndexes(
                    context: context,
                    searchIndexHash: searchIndexHash,
                    referenceType: "Sites",
                    referenceId: SiteId);
                return searchIndexHash;
            }
        }

        public Error.Types Update(
            Context context,
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            if (setBySession) SetBySession(context: context);
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(
                context: context,
                ss: ss,
                statements: statements,
                timestamp: timestamp,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements);
            if (permissionChanged)
            {
                statements.UpdatePermissions(context, ss, SiteId, permissions, site: true);
            }
            statements.Add(Rds.PhysicalDeleteReminderSchedules(
                where: Rds.ReminderSchedulesWhere()
                    .SiteId(SiteId)));
            SiteSettings.Reminders?.ForEach(reminder =>
                statements.Add(Rds.UpdateOrInsertReminderSchedules(
                    param: Rds.ReminderSchedulesParam()
                        .SiteId(SiteId)
                        .Id(reminder.Id)
                        .ScheduledTime(reminder.StartDateTime.Next(
                            context: context,
                            type: reminder.Type)),
                    where: Rds.ReminderSchedulesWhere()
                        .SiteId(SiteId)
                        .Id(reminder.Id))));
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Count == 0) return Error.Types.UpdateConflicts;
            if (get) Get(context: context);
            UpdateRelatedRecords(context: context);
            SiteInfo.Reflesh(context: context);
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
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
                    param: param ?? Rds.SitesParamDefault(
                        context: context, siteModel: this, otherInitValue: otherInitValue),
                    countRecord: true),
                StatusUtilities.UpdateStatus(
                    tenantId: TenantId,
                    type: StatusUtilities.Types.SitesUpdated),
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
            column.Body(function: Sqls.Functions.SingleColumn); param.Body();
            column.ReferenceType(function: Sqls.Functions.SingleColumn); param.ReferenceType();
            column.ParentId(function: Sqls.Functions.SingleColumn); param.ParentId();
            column.InheritPermission(function: Sqls.Functions.SingleColumn); param.InheritPermission();
            column.SiteSettings(function: Sqls.Functions.SingleColumn); param.SiteSettings();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            return Rds.InsertSites(
                tableType: tableType,
                param: param,
                select: Rds.SelectSites(column: column, where: where),
                addUpdatorParam: false);
        }

        public void UpdateRelatedRecords(
            Context context,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            var fullText = FullText(context, SiteSettings);
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
                context: context,
                transactional: true,
                statements: statements.ToArray());
            Libraries.Search.Indexes.Create(context, SiteSettings, this);
        }

        public Error.Types UpdateOrCreate(
            Context context,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    setIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Sites")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.UpdateOrInsertSites(
                    where: where ?? Rds.SitesWhereDefault(this),
                    param: param ?? Rds.SitesParamDefault(
                        context: context, siteModel: this, setDefault: true)),
                StatusUtilities.UpdateStatus(
                    tenantId: TenantId,
                    type: StatusUtilities.Types.SitesUpdated),
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            SiteId = (response.Identity ?? SiteId).ToLong();
            Get(context: context);
            Libraries.Search.Indexes.Create(context, SiteSettings, this);
            return Error.Types.None;
        }

        public Error.Types Delete(Context context, SiteSettings ss)
        {
            var siteMenu = SiteInfo.TenantCaches.Get(TenantId)?
                .SiteMenu
                .Children(
                    context: context,
                    siteId: ss.SiteId,
                    withParent: true);
            Rds.ExecuteNonQuery(
                context: context,
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

        public Error.Types Restore(Context context, long siteId)
        {
            SiteId = siteId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(SiteId)),
                    Rds.RestoreSites(
                        where: Rds.SitesWhere().SiteId(SiteId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: TenantId,
                        type: StatusUtilities.Types.SitesUpdated),
                });
            Libraries.Search.Indexes.Create(context, SiteSettings, this);
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteSites(
                    tableType: tableType,
                    param: Rds.SitesParam().TenantId(TenantId).SiteId(SiteId)));
            Libraries.Search.Indexes.Create(context, SiteSettings, this);
            return Error.Types.None;
        }

        public void SetByForm(Context context)
        {
            var ss = new SiteSettings();
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Sites_Title": Title = new Title(SiteId, Forms.Data(controlId)); break;
                    case "Sites_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Sites_ReferenceType": ReferenceType = Forms.Data(controlId).ToString(); break;
                    case "Sites_InheritPermission": InheritPermission = Forms.Data(controlId).ToLong(); break;
                    case "Sites_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(context: context, ss: ss, body: Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: controlId.Substring("Comment".Length).ToInt(),
                                body: Forms.Data(controlId));
                        }
                        break;
                }
            });
            SetSiteSettings(context: context);
            if (context.Action == "deletecomment")
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

        public void SetByModel(SiteModel siteModel)
        {
            TenantId = siteModel.TenantId;
            UpdatedTime = siteModel.UpdatedTime;
            Title = siteModel.Title;
            Body = siteModel.Body;
            ReferenceType = siteModel.ReferenceType;
            ParentId = siteModel.ParentId;
            InheritPermission = siteModel.InheritPermission;
            SiteSettings = siteModel.SiteSettings;
            Ancestors = siteModel.Ancestors;
            SiteMenu = siteModel.SiteMenu;
            MonitorChangesColumns = siteModel.MonitorChangesColumns;
            TitleColumns = siteModel.TitleColumns;
            Export = siteModel.Export;
            Comments = siteModel.Comments;
            Creator = siteModel.Creator;
            Updator = siteModel.Updator;
            CreatedTime = siteModel.CreatedTime;
            VerUp = siteModel.VerUp;
            Comments = siteModel.Comments;
        }

        public void SetByApi(Context context, SiteSettings ss)
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
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            SetSiteSettings(context: context);
        }

        private bool Matched(Context context, SiteSettings ss, View view)
        {
            if (view.ColumnFilterHash != null)
            {
                foreach (var filter in view.ColumnFilterHash)
                {
                    var match = true;
                    var column = ss.GetColumn(context: context, columnName: filter.Key);
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

        private void SetBySession(Context context)
        {
            if (!Forms.HasData("Sites_SiteSettings")) SiteSettings = Session_SiteSettings(context: context);
            if (!Forms.HasData("Sites_MonitorChangesColumns")) MonitorChangesColumns = Session_MonitorChangesColumns(context: context);
            if (!Forms.HasData("Sites_TitleColumns")) TitleColumns = Session_TitleColumns(context: context);
            if (!Forms.HasData("Sites_Export")) Export = Session_Export(context: context);
        }

        private void Set(Context context, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, DataRow dataRow, string tableAlias = null)
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
                                UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
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
                            SiteSettings = GetSiteSettings(context: context, dataRow: dataRow);
                            SavedSiteSettings = SiteSettings.RecordingJson(context: context);
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return
                TenantId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                Title_Updated(context: context) ||
                Body_Updated(context: context) ||
                ReferenceType_Updated(context: context) ||
                ParentId_Updated(context: context) ||
                InheritPermission_Updated(context: context) ||
                SiteSettings_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }

        public List<string> Mine(Context context)
        {
            var mine = new List<string>();
            var userId = context.UserId;
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types Create(Context context, bool otherInitValue = false)
        {
            if (!otherInitValue)
            {
                SiteSettings = new SiteSettings(context: context, referenceType: ReferenceType);
            }
            TenantId = context.TenantId;
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        setIdentity: true,
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
                            .SiteSettings(SiteSettings.RecordingJson(context: context))
                            .Comments(Comments.ToJson())),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(raw: Def.Sql.Identity),
                        param: Rds.ItemsParam().SiteId(raw: Def.Sql.Identity)),
                    Rds.InsertPermissions(
                        param: Rds.PermissionsParam()
                            .ReferenceId(raw: Def.Sql.Identity)
                            .DeptId(0)
                            .UserId(context.UserId)
                            .PermissionType(Permissions.Manager()),
                        _using: InheritPermission == 0),
                    StatusUtilities.UpdateStatus(
                        tenantId: TenantId,
                        type: StatusUtilities.Types.SitesUpdated),
                });
            SiteId = response.Identity ?? SiteId;
            Get(context: context);
            SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: this, referenceId: SiteId);
            switch (ReferenceType)
            {
                case "Wikis":
                    var wikiModel = new WikiModel(context: context, ss: SiteSettings)
                    {
                        SiteId = SiteId,
                        Title = Title,
                        Body = Body,
                        Comments = Comments
                    };
                    wikiModel.Create(context: context, ss: SiteSettings);
                    break;
                default:
                    Libraries.Search.Indexes.Create(
                        context: context,
                        ss: SiteSettings,
                        siteModel: this);
                    break;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SiteSettings GetSiteSettings(Context context, DataRow dataRow)
        {
            return dataRow.String("SiteSettings").Deserialize<SiteSettings>() ??
                new SiteSettings(context: context, referenceType: ReferenceType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsPropertiesBySession(Context context)
        {
            SiteSettings = Session_SiteSettings(context: context);
            SiteSettings.InheritPermission = InheritPermission;
            SetSiteSettingsProperties(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsProperties(Context context)
        {
            if (SiteSettings == null)
            {
                SiteSettings = SiteSettingsUtilities.SitesSiteSettings(
                    context: context, siteId: SiteId);
            }
            SiteSettings.SiteId = SiteId;
            SiteSettings.ParentId = ParentId;
            SiteSettings.Title = Title.Value;
            SiteSettings.AccessStatus = AccessStatus;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SetSiteSettings(Context context)
        {
            var invalidFormat = string.Empty;
            var invalid = SiteValidators.OnSetSiteSettings(
                context: context,
                ss: SiteSettingsUtilities.Get(
                    context: context, siteModel: this, referenceId: SiteId),
                data: out invalidFormat);
            switch (invalid)
            {
                case Error.Types.BadFormat:
                    return Messages.ResponseBadFormat(
                        context: context,
                        data: invalidFormat).ToJson();
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var res = new SitesResponseCollection(this);
            SetSiteSettingsPropertiesBySession(context: context);
            SetSiteSettings(context: context, res: res);
            Session_SiteSettings(
                context: context,
                value: SiteSettings.ToJson());
            return res
                .SetMemory("formChanged", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteSettings(Context context, ResponseCollection res)
        {
            var controlId = Forms.ControlId();
            SiteSettings.SetJoinedSsHash(context: context);
            switch (controlId)
            {
                case "OpenGridColumnDialog":
                    OpenGridColumnDialog(
                        context: context,
                        res: res);
                    break;
                case "SetGridColumn":
                    SetGridColumn(
                        context: context,
                        res: res);
                    break;
                case "GridJoin":
                    SetGridColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "OpenFilterColumnDialog":
                    OpenFilterColumnDialog(
                        context: context,
                        res: res);
                    break;
                case "SetFilterColumn":
                    SetFilterColumn(
                        context: context,
                        res: res);
                    break;
                case "FilterJoin":
                    SetFilterColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "AddAggregations":
                case "DeleteAggregations":
                    SetAggregations(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "SetAggregationDetails":
                    SetAggregationDetails(
                        context: context,
                        res: res);
                    break;
                case "OpenEditorColumnDialog":
                    OpenEditorColumnDialog(
                        context: context,
                        res: res);
                    break;
                case "SetEditorColumn":
                    SetEditorColumn(
                        context: context,
                        res: res);
                    break;
                case "ResetEditorColumn":
                    ResetEditorColumn(
                        context: context,
                        res: res);
                    break;
                case "MoveUpSummaries":
                case "MoveDownSummaries":
                    SetSummariesOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewSummary":
                case "EditSummary":
                    OpenSummaryDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "SummarySiteId":
                    SetSummarySiteId(
                        context: context,
                        res: res);
                    break;
                case "SummaryType":
                    SetSummaryType(
                        context: context,
                        res: res);
                    break;
                case "AddSummary":
                    AddSummary(
                        context: context,
                        res: res);
                    break;
                case "UpdateSummary":
                    UpdateSummary(
                        context: context,
                        res: res);
                    break;
                case "DeleteSummaries":
                    DeleteSummaries(
                        context: context,
                        res: res);
                    break;
                case "MoveUpFormulas":
                case "MoveDownFormulas":
                    SetFormulasOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewFormula":
                case "EditFormula":
                    OpenFormulaDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddFormula":
                    AddFormula(
                        context: context,
                        res: res);
                    break;
                case "UpdateFormula":
                    UpdateFormula(
                        context: context,
                        res: res);
                    break;
                case "DeleteFormulas":
                    DeleteFormulas(
                        context: context,
                        res: res);
                    break;
                case "NewView":
                case "EditView":
                    OpenViewDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddViewFilter":
                    AddViewFilter(
                        context: context,
                        res: res);
                    break;
                case "AddView":
                    AddView(
                        context: context,
                        res: res);
                    break;
                case "UpdateView":
                    UpdateView(
                        context: context,
                        res: res);
                    break;
                case "DeleteViews":
                    DeleteViews(res: res);
                    break;
                case "ViewGridJoin":
                    SetViewGridColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "MoveUpNotifications":
                case "MoveDownNotifications":
                    SetNotificationsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewNotification":
                case "EditNotification":
                    OpenNotificationDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddNotification":
                    AddNotification(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateNotification":
                    UpdateNotification(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteNotifications":
                    DeleteNotifications(
                        context: context,
                        res: res);
                    break;
                case "NewReminder":
                case "EditReminder":
                    OpenReminderDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddReminder":
                    AddReminder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateReminder":
                    UpdateReminder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteReminders":
                    DeleteReminders(
                        context: context,
                        res: res);
                    break;
                case "MoveUpReminders":
                case "MoveDownReminders":
                    SetRemindersOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "TestReminders":
                    TestReminders(
                        context: context,
                        res: res);
                    break;
                case "MoveUpExports":
                case "MoveDownExports":
                    SetExportsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewExport":
                case "EditExport":
                    OpenExportDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddExport":
                    AddExport(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateExport":
                    UpdateExport(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteExports":
                    DeleteExports(
                        context: context,
                        res: res);
                    break;
                case "ExportJoin":
                    SetExportColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "SearchExportColumns":
                    SetExportColumnsSelectableBySearch(
                        context: context,
                        res: res);
                    break;
                case "OpenExportColumnsDialog":
                    OpenExportColumnsDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateExportColumn":
                    UpdateExportColumns(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "MoveUpStyles":
                case "MoveDownStyles":
                    SetStylesOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewStyle":
                case "EditStyle":
                    OpenStyleDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddStyle":
                    AddStyle(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateStyle":
                    UpdateStyle(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteStyles":
                    DeleteStyles(
                        context: context,
                        res: res);
                    break;
                case "MoveUpScripts":
                case "MoveDownScripts":
                    SetScriptsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewScript":
                case "EditScript":
                    OpenScriptDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddScript":
                    AddScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateScript":
                    UpdateScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteScripts":
                    DeleteScripts(
                        context: context,
                        res: res);
                    break;
                case "MoveUpRelatingColumns":
                case "MoveDownRelatingColumns":
                    SetRelatingColumnsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewRelatingColumn":
                case "EditRelatingColumns":
                    OpenRelatingColumnDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddRelatingColumn":
                    AddRelatingColumn(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateRelatingColumn":
                    UpdateRelatingColumn(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteRelatingColumns":
                    DeleteRelatingColumns(
                        context: context,
                        res: res);
                    break;
                default:
                    Forms.All()
                        .Where(o => o.Key != controlId)
                        .ForEach(data =>
                            SiteSettings.Set(
                                propertyName: data.Key,
                                value: data.Value));
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HtmlBuilder ReplaceSiteMenu(Context context, long sourceId, long destinationId)
        {
            var siteMenu = SiteInfo.TenantCaches.Get(TenantId).SiteMenu;
            return new HtmlBuilder().SiteMenu(
                context: context,
                ss: SiteSettings,
                siteId: destinationId,
                referenceType: ReferenceType,
                title: siteMenu.Get(destinationId).Title,
                siteConditions: siteMenu.SiteConditions(context: context, ss: SiteSettings));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenGridColumnDialog(Context context, ResponseCollection res)
        {
            var selectedColumns = Forms.List("GridColumns");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest(context: context));
                }
                else if(column.Joined)
                {
                    res.Message(Messages.CanNotPerformed(context: context));
                }
                else
                {
                    SiteSettings.GridColumns = Forms.List("GridColumnsAll");
                    res.Html(
                        "#GridColumnDialog",
                        SiteUtilities.GridColumnDialog(
                            context: context,
                            ss: SiteSettings,
                            column: column));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetGridColumn(Context context, ResponseCollection res)
        {
            var columnName = Forms.Data("GridColumnName");
            var column = SiteSettings.GridColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest(context: context));
            }
            else
            {
                Forms.All().ForEach(data => SiteSettings.SetColumnProperty(
                    context: context,
                    column: column,
                    propertyName: data.Key,
                    value: GridColumnValue(data.Key, data.Value)));
                res
                    .Html("#GridColumns", new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.GridSelectableOptions(context: context),
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
        private void SetGridColumnsSelectable(Context context, ResponseCollection res)
        {
            SiteSettings.GridColumns = Forms.List("GridColumnsAll");
            var listItemCollection = SiteSettings.GridSelectableOptions(
                context: context, enabled: false, join: Forms.Data("GridJoin"));
            if (!listItemCollection.Any())
            {
                res.Message(Messages.NotFound(context: context));
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
        private void OpenFilterColumnDialog(Context context, ResponseCollection res)
        {
            var selectedColumns = Forms.List("FilterColumns");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest(context: context));
                }
                else if (column.Joined)
                {
                    res.Message(Messages.CanNotPerformed(context: context));
                }
                {
                    SiteSettings.FilterColumns = Forms.List("FilterColumnsAll");
                    res.Html(
                        "#FilterColumnDialog",
                        SiteUtilities.FilterColumnDialog(
                            context: context,
                            ss: SiteSettings,
                            column: column));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFilterColumn(Context context, ResponseCollection res)
        {
            var columnName = Forms.Data("FilterColumnName");
            var column = SiteSettings.FilterColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest(context: context));
            }
            else
            {
                Forms.All().ForEach(data => SiteSettings.SetColumnProperty(
                    context: context,
                    column: column,
                    propertyName: data.Key,
                    value: data.Value));
                res
                    .Html("#FilterColumns", new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.FilterSelectableOptions(context: context),
                        selectedValueTextCollection: columnName.ToSingleList()))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFilterColumnsSelectable(Context context, ResponseCollection res)
        {
            SiteSettings.FilterColumns = Forms.List("FilterColumnsAll");
            var listItemCollection = SiteSettings.FilterSelectableOptions(
                context: context, enabled: false, join: Forms.Data("FilterJoin"));
            if (!listItemCollection.Any())
            {
                res.Message(Messages.NotFound(context: context));
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
        private void SetAggregations(Context context, ResponseCollection res, string controlId)
        {
            var selectedColumns = Forms.List("AggregationDestination");
            var selectedSourceColumns = Forms.List("AggregationSource");
            if (selectedColumns.Any() || selectedSourceColumns.Any())
            {
                int id = 1;
                List<Libraries.Settings.Aggregation> aggregations = new List<Libraries.Settings.Aggregation>();
                List<string> refactSelectedColumns = new List<string>();
                Forms.List("AggregationDestinationAll").ForEach(a =>
                {
                    var aggrigation = SiteSettings.Aggregations.Where(o => o.Id == a.ToInt()).FirstOrDefault();
                    if (selectedColumns.Contains(aggrigation.Id.ToString()))
                    {
                        refactSelectedColumns.Add(id.ToString());
                    }
                    aggregations.Add(new Libraries.Settings.Aggregation()
                    {
                        Id = id++,
                        GroupBy = aggrigation.GroupBy,
                        Type = aggrigation.Type,
                        Target = aggrigation.Target,
                        Data = aggrigation.Data
                    });
                });
                SiteSettings.Aggregations = aggregations;
                selectedColumns = refactSelectedColumns;
                SiteSettings.SetAggregations(
                    controlId,
                    selectedColumns,
                    selectedSourceColumns);
                res
                    .Html("#AggregationDestination", new HtmlBuilder()
                        .SelectableItems(
                            listItemCollection: SiteSettings
                                .AggregationDestination(context: context),
                            selectedValueTextCollection: selectedColumns))
                    .SetFormData("AggregationDestination", selectedColumns?.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetAggregationDetails(Context context, ResponseCollection res)
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
                int id = 1;
                List<Aggregation> aggregations = new List<Aggregation>();
                List<string> refactSelectedColumns = new List<string>();
                Forms.List("AggregationDestinationAll").ForEach(a =>
                {
                    var aggrigation = SiteSettings.Aggregations.Where(o => o.Id == a.ToInt()).FirstOrDefault();
                    if (selectedColumns.Contains(aggrigation.Id.ToString()))
                    {
                        refactSelectedColumns.Add(id.ToString());
                    }
                    aggregations.Add(new Aggregation()
                    {
                        Id = id++,
                        GroupBy = aggrigation.GroupBy,
                        Type = aggrigation.Type,
                        Target = aggrigation.Target,
                        Data = aggrigation.Data
                    });
                });
                SiteSettings.Aggregations = aggregations;
                selectedColumns = refactSelectedColumns;
                SiteSettings.SetAggregationDetails(
                    type,
                    target,
                    selectedColumns,
                    selectedSourceColumns);
                res
                    .Html("#AggregationDestination", new HtmlBuilder()
                        .SelectableItems(
                            listItemCollection: SiteSettings
                                .AggregationDestination(context: context),
                            selectedValueTextCollection: selectedColumns))
                    .SetFormData("AggregationDestination", selectedColumns?.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenEditorColumnDialog(Context context, ResponseCollection res)
        {
            var selectedColumns = Forms.List("EditorColumns");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var column = SiteSettings.EditorColumn(selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest(context: context));
                }
                else
                {
                    var titleColumns = SiteSettings.TitleColumns;
                    if (column.ColumnName == "Title")
                    {
                        Session_TitleColumns(
                            context: context,
                            value: titleColumns);
                    }
                    SiteSettings.EditorColumns = Forms.List("EditorColumnsAll");
                    res.Html(
                        "#EditorColumnDialog",
                        SiteUtilities.EditorColumnDialog(
                            context: context,
                            ss: SiteSettings,
                            column: column,
                            titleColumns: titleColumns));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorColumn(Context context, ResponseCollection res)
        {
            var columnName = Forms.Data("EditorColumnName");
            var column = SiteSettings.EditorColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest(context: context));
            }
            else
            {
                if (column.ColumnName == "Title")
                {
                    SiteSettings.TitleColumns = Forms.List("TitleColumnsAll");
                }
                Forms.All().ForEach(data =>
                    SiteSettings.SetColumnProperty(
                        context: context,
                        column: column,
                        propertyName: data.Key,
                        value: data.Value));
                res
                    .Html("#EditorColumns", new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.EditorSelectableOptions(context: context),
                        selectedValueTextCollection: columnName.ToSingleList()))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ResetEditorColumn(Context context, ResponseCollection res)
        {
            var ss = new SiteSettings(context: context, referenceType: ReferenceType);
            res.Html(
                "#EditorColumnDialog",
                SiteUtilities.EditorColumnDialog(
                    context: context,
                    ss: SiteSettings,
                    column: ss.GetColumn(
                        context: context,
                        columnName: Forms.Data("EditorColumnName")),
                    titleColumns: ss.TitleColumns));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummariesOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = Forms.IntList("EditSummary");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Summaries.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditSummary", new HtmlBuilder()
                    .EditSummary(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenSummaryDialog(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.SetLinkedSiteSettings(context: context);
            if (SiteSettings.Destinations?.Any() != true)
            {
                res.Message(Messages.NoLinks(context: context));
            }
            else
            {
                if (controlId == "NewSummary")
                {
                    OpenSummaryDialog(
                        context: context,
                        res: res,
                        summary: new Summary(SiteSettings.Destinations.FirstOrDefault().SiteId));
                }
                else
                {
                    var summary = SiteSettings.Summaries?.Get(Forms.Int("SummaryId"));
                    if (summary == null)
                    {
                        OpenDialogError(res, Messages.SelectOne(context: context));
                    }
                    else
                    {
                        SiteSettingsUtilities.Get(
                            context: context, siteModel: this, referenceId: SiteId);
                        OpenSummaryDialog(
                            context: context,
                            res: res,
                            summary: summary);
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenSummaryDialog(Context context, ResponseCollection res, Summary summary)
        {
            res.Html("#SummaryDialog", SiteUtilities.SummaryDialog(
                context: context,
                ss: SiteSettings,
                controlId: Forms.ControlId(),
                summary: summary));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummarySiteId(Context context, ResponseCollection res)
        {
            var siteId = Forms.Long("SummarySiteId");
            var destinationSiteModel = new SiteModel(context: context, siteId: siteId);
            res
                .ReplaceAll("#SummaryDestinationColumnField", new HtmlBuilder()
                    .SummaryDestinationColumn(
                        context: context,
                        destinationSs: destinationSiteModel.SiteSettings))
                .ReplaceAll("#SummaryLinkColumnField", new HtmlBuilder()
                    .SummaryLinkColumn(
                        context: context,
                        ss: SiteSettings,
                        siteId: siteId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummaryType(Context context, ResponseCollection res)
        {
            res.ReplaceAll("#SummarySourceColumnField", new HtmlBuilder()
                .SummarySourceColumn(
                    context: context,
                    ss: SiteSettings,
                    type: Forms.Data("SummaryType")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddSummary(Context context, ResponseCollection res)
        {
            SiteSettings.SetLinkedSiteSettings(context: context);
            var siteId = Forms.Long("SummarySiteId");
            var destinationSs = SiteSettings.Destinations.Get(siteId);
            int? destinationCondition = Forms.Int("SummaryDestinationCondition");
            int? sourceCondition = Forms.Int("SummarySourceCondition");
            var error = SiteSettings.AddSummary(
                siteId,
                new SiteModel(context: context, siteId: Forms.Long("SummarySiteId")).ReferenceType,
                Forms.Data("SummaryDestinationColumn"),
                destinationSs?.Views?.Get(destinationCondition)?.Id,
                Forms.Bool("SummarySetZeroWhenOutOfCondition"),
                Forms.Data("SummaryLinkColumn"),
                Forms.Data("SummaryType"),
                Forms.Data("SummarySourceColumn"),
                SiteSettings.Views?.Get(sourceCondition)?.Id);
            if (error.Has())
            {
                res.Message(error.Message(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditSummary", new HtmlBuilder()
                        .EditSummary(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateSummary(Context context, ResponseCollection res)
        {
            SiteSettings.SetLinkedSiteSettings(context: context);
            var siteId = Forms.Long("SummarySiteId");
            var destinationSs = SiteSettings.Destinations.Get(siteId);
            int? destinationCondition = Forms.Int("SummaryDestinationCondition");
            int? sourceCondition = Forms.Int("SummarySourceCondition");
            var outOfCondition = Forms.Data("SummaryOutOfCondition").Trim();
            var error = SiteSettings.UpdateSummary(
                Forms.Int("SummaryId"),
                siteId,
                new SiteModel(context: context, siteId: Forms.Long("SummarySiteId")).ReferenceType,
                Forms.Data("SummaryDestinationColumn"),
                destinationSs?.Views?.Get(destinationCondition)?.Id,
                Forms.Bool("SummarySetZeroWhenOutOfCondition"),
                Forms.Data("SummaryLinkColumn"),
                Forms.Data("SummaryType"),
                Forms.Data("SummarySourceColumn"),
                SiteSettings.Views?.Get(sourceCondition)?.Id);
            if (error.Has())
            {
                res.Message(error.Message(context: context));
            }
            else
            {
                res
                    .Html("#EditSummary", new HtmlBuilder()
                        .EditSummary(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteSummaries(Context context, ResponseCollection res)
        {
            var selected = Forms.IntList("EditSummary");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Summaries.Delete(selected);
                res.ReplaceAll("#EditSummary", new HtmlBuilder()
                    .EditSummary(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFormulasOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = Forms.IntList("EditFormula");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Formulas.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditFormula", new HtmlBuilder()
                    .EditFormula(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenFormulaDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewFormula")
            {
                var formulaSet = new FormulaSet();
                OpenFormulaDialog(
                    context: context,
                    res: res,
                    formulaSet: formulaSet);
            }
            else
            {
                var formulaSet = SiteSettings.Formulas?.Get(Forms.Int("FormulaId"));
                if (formulaSet == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenFormulaDialog(
                        context: context,
                        res: res,
                        formulaSet: formulaSet);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenFormulaDialog(
            Context context, ResponseCollection res, FormulaSet formulaSet)
        {
            res.Html("#FormulaDialog", SiteUtilities.FormulaDialog(
                context: context,
                ss: SiteSettings,
                controlId: Forms.ControlId(),
                formulaSet: formulaSet));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddFormula(Context context, ResponseCollection res)
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
                res.Message(error.Message(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditFormula", new HtmlBuilder()
                        .EditFormula(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateFormula(Context context, ResponseCollection res)
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
                res.Message(error.Message(context: context));
            }
            else
            {
                res
                    .Html("#EditFormula", new HtmlBuilder()
                        .EditFormula(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteFormulas(Context context, ResponseCollection res)
        {
            var selected = Forms.IntList("EditFormula");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Formulas.Delete(selected);
                res.ReplaceAll("#EditFormula", new HtmlBuilder()
                    .EditFormula(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenViewDialog(Context context, ResponseCollection res, string controlId)
        {
            View view;
            if (controlId == "NewView")
            {
                view = new View(context: context, ss: SiteSettings);
                OpenViewDialog(context: context, res: res, view: view);
            }
            else
            {
                var idList = Forms.IntList("Views");
                if (idList.Count() != 1)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    view = SiteSettings.Views?.Get(idList.First());
                    if (view == null)
                    {
                        OpenDialogError(res, Messages.SelectOne(context: context));
                    }
                    else
                    {
                        SiteSettings.Views = SiteSettings.Views.Join(
                            Forms.List("ViewsAll").Select((val, key) => new { Key = key, Val = val }), v => v.Id, l => l.Val.ToInt(),
                                (v, l) => new { Views = v, OrderNo = l.Key })
                                .OrderBy(v => v.OrderNo)
                                .Select(v => v.Views)
                                .ToList();
                        SiteSettingsUtilities.Get(
                            context: context, siteModel: this, referenceId: SiteId);
                        OpenViewDialog(context: context, res: res, view: view);
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenViewDialog(Context context, ResponseCollection res, View view)
        {
            res.Html("#ViewDialog", SiteUtilities.ViewDialog(
                context: context,
                ss: SiteSettings,
                controlId: Forms.ControlId(),
                view: view));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddViewFilter(Context context, ResponseCollection res)
        {
            SiteSettingsUtilities.Get(context: context, siteModel: this, referenceId: SiteId);
            res
                .Append(
                    "#ViewFiltersTab .items",
                    new HtmlBuilder().ViewFilter(
                        context: context,
                        ss: SiteSettings,
                        column: SiteSettings.GetColumn(
                            context: context,
                            columnName: Forms.Data("ViewFilterSelector"))))
                .Remove("#ViewFilterSelector option:selected");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddView(Context context, ResponseCollection res)
        {
            SiteSettings.AddView(new View(context: context, ss: SiteSettings));
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
        private void UpdateView(Context context, ResponseCollection res)
        {
            var selected = Forms.Int("ViewId");
            var view = SiteSettings.Views?.Get(selected);
            if (view == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                view.SetByForm(context: context, ss: SiteSettings);
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
        private void SetViewGridColumnsSelectable(Context context, ResponseCollection res)
        {
            var gridColumns = Forms.List("ViewGridColumnsAll");
            var listItemCollection = SiteSettings.ViewGridSelectableOptions(
                context: context,
                gridColumns: gridColumns,
                enabled: false,
                join: Forms.Data("ViewGridJoin"));
            if (!listItemCollection.Any())
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                res.Html("#ViewGridSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: listItemCollection));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenNotificationDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewNotification")
            {
                OpenNotificationDialog(
                    context: context,
                    res: res,
                    notification: new Notification(
                        type: Notification.Types.Mail,
                        monitorChangesColumns: SiteSettings
                            .ColumnDefinitionHash
                            .MonitorChangesDefinitions()
                            .Select(o => o.ColumnName)
                            .Where(o => SiteSettings.EditorColumns.Contains(o)
                                || o == "Comments")
                            .ToList()));
            }
            else
            {
                var notification = SiteSettings.Notifications?.Get(Forms.Int("NotificationId"));
                if (notification == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenNotificationDialog(
                        context: context,
                        res: res,
                        notification: notification);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenNotificationDialog(
            Context context, ResponseCollection res, Notification notification)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                Session_MonitorChangesColumns(
                    context: context,
                    value: notification.MonitorChangesColumns);
                res.Html("#NotificationDialog", SiteUtilities.NotificationDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: Forms.ControlId(),
                    notification: notification));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddNotification(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                SiteSettings.Notifications.Add(new Notification(
                    SiteSettings.Notifications.MaxOrDefault(o => o.Id) + 1,
                    (Notification.Types)Forms.Int("NotificationType"),
                    Forms.Data("NotificationPrefix"),
                    Forms.Data("NotificationAddress"),
                    Forms.Data("NotificationToken"),
                    Forms.Bool("NotificationIsGroup"),
                    Forms.List("MonitorChangesColumnsAll"),
                    Forms.Int("BeforeCondition"),
                    Forms.Int("AfterCondition"),
                    (Notification.Expressions)Forms.Int("Expression")));
                SetNotificationsResponseCollection(context: context, res: res);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateNotification(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var notification = SiteSettings.Notifications.Get(Forms.Int("NotificationId"));
                if (notification == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    notification.Update(
                        (Notification.Types)Forms.Int("NotificationType"),
                        Forms.Data("NotificationPrefix"),
                        Forms.Data("NotificationAddress"),
                        Forms.Data("NotificationToken"),
                        Forms.Bool("NotificationIsGroup"),
                        Forms.List("MonitorChangesColumnsAll"),
                        Forms.Int("BeforeCondition"),
                        Forms.Int("AfterCondition"),
                        (Notification.Expressions)Forms.Int("Expression"));
                    SetNotificationsResponseCollection(context: context, res: res);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetNotificationsOrder(
            Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = Forms.IntList("EditNotification");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Notifications.MoveUpOrDown(
                        ColumnUtilities.ChangeCommand(controlId), selected);
                    res.Html("#EditNotification", new HtmlBuilder()
                        .EditNotification(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetNotificationsResponseCollection(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditNotification", new HtmlBuilder()
                        .EditNotification(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteNotifications(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = Forms.IntList("EditNotification");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Notifications.Delete(selected);
                    res.ReplaceAll("#EditNotification", new HtmlBuilder()
                        .EditNotification(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenReminderDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewReminder")
            {
                OpenReminderDialog(
                    context: context,
                    res: res,
                    reminder: new Reminder(context: context) { Subject = Title.Value });
            }
            else
            {
                var reminder = SiteSettings.Reminders?.Get(Forms.Int("ReminderId"));
                if (reminder == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenReminderDialog(
                        context: context,
                        res: res,
                        reminder: reminder);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenReminderDialog(
            Context context, ResponseCollection res, Reminder reminder)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res.Html("#ReminderDialog", SiteUtilities.ReminderDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: Forms.ControlId(),
                    reminder: reminder));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddReminder(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
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
                        SetRemindersResponseCollection(context: context, res: res);
                        break;
                    case Error.Types.BadMailAddress:
                    case Error.Types.ExternalMailAddress:
                        res.Message(invalid.Message(
                            context: context,
                            data: invalidMailAddress));
                        break;
                    default:
                        res.Message(invalid.Message(context: context));
                        break;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateReminder(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var reminder = SiteSettings.Reminders.Get(Forms.Int("ReminderId"));
                if (reminder == null)
                {
                    res.Message(Messages.NotFound(context: context));
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
                            SetRemindersResponseCollection(context: context, res: res);
                            break;
                        case Error.Types.BadMailAddress:
                        case Error.Types.ExternalMailAddress:
                            res.Message(invalid.Message(
                                context: context,
                                data: invalidMailAddress));
                            break;
                        default:
                            res.Message(invalid.Message(context: context));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetRemindersOrder(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Reminders.MoveUpOrDown(
                        ColumnUtilities.ChangeCommand(controlId), selected);
                    res.Html("#EditReminder", new HtmlBuilder()
                        .EditReminder(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetRemindersResponseCollection(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteReminders(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Reminders.Delete(selected);
                    res.ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void TestReminders(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Remind(context: context, idList: selected, test: true);
                    res.ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportDialog(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.SetExports(context: context);
            if (controlId == "NewExport")
            {
                OpenExportDialog(context: context, res: res, export: new Export(SiteSettings
                    .DefaultExportColumns(context: context)));
            }
            else
            {
                var export = SiteSettings.Exports?.Get(Forms.Int("ExportId"));
                if (export == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenExportDialog(context: context, res: res, export: export);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportDialog(Context context, ResponseCollection res, Export export)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                Session_Export(
                    context: context,
                    value: export);
                res.Html("#ExportDialog", SiteUtilities.ExportDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: Forms.ControlId(),
                    export: export));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddExport(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                SiteSettings.SetExports(context: context);
                Export = Session_Export(context: context);
                if (Export == null && controlId == "EditExport")
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    SiteSettings.SetExport(context: context, export: Export);
                    int id = 1;
                    var columns = new List<ExportColumn>();
                    Forms.List("ExportColumnsAll").ForEach(o =>
                    {
                        var exp = o.Deserialize<ExportColumn>()
                            ?? Session_Export(context: context)
                                .Columns
                                .Where(c => c.Id.ToString() == o)
                                .FirstOrDefault();
                        columns.Add(new ExportColumn()
                        {
                            SiteId = exp.SiteId,
                            Id = id++,
                            ColumnName = exp.ColumnName,
                            LabelText = exp.LabelText,
                            Type = exp.Type,
                            Format = exp.Format,
                            SiteTitle = exp.SiteTitle,
                            Column = exp.Column
                        });
                    });
                    Export.Columns = columns;
                    Export.Id = SiteSettings.Exports.MaxOrDefault(o => o.Id) + 1;
                    Export.Name = Forms.Data("ExportName");
                    Export.Header = Forms.Bool("ExportHeader");
                    Export.Join = Forms.Data("ExportJoin").Deserialize<Join>();
                    SiteSettings.Exports.Add(Export);
                    SetExportsResponseCollection(context: context, res: res);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateExport(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var export = SiteSettings.Exports.Get(Forms.Int("ExportId"));
                if (export == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    SiteSettings.SetExports(context: context);
                    int id = 1;
                    var columns = new List<ExportColumn>();
                    Forms.List("ExportColumnsAll").ForEach(o =>
                    {
                        var exp = o.Deserialize<ExportColumn>()
                            ?? Session_Export(context: context)
                                .Columns
                                .Where(c => c.Id.ToString() == o)
                                .FirstOrDefault();
                        columns.Add(new ExportColumn()
                        {
                            SiteId = exp.SiteId,
                            Id = id++,
                            ColumnName = exp.ColumnName,
                            LabelText = exp.LabelText,
                            Type = exp.Type,
                            Format = exp.Format,
                            SiteTitle = exp.SiteTitle,
                            Column = exp.Column
                        });
                    });
                    export.Update(
                        Forms.Data("ExportName"),
                        Forms.Bool("ExportHeader"),
                        Forms.Data("ExportJoin").Deserialize<Join>(),
                        columns);
                    SetExportsResponseCollection(context: context, res: res);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportsOrder(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = Forms.IntList("EditExport");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Exports.MoveUpOrDown(
                        ColumnUtilities.ChangeCommand(controlId), selected);
                    res.Html("#EditExport", new HtmlBuilder()
                        .EditExport(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportsResponseCollection(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditExport", new HtmlBuilder()
                        .EditExport(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportColumnsSelectable(Context context, ResponseCollection res)
        {
            SiteSettings.SetExports(context: context);
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
                current.AddRange(ss.DefaultExportColumns(context: context));
                sources.AddRange(ss.ExportColumns(
                    context: context, searchText: searchText));
            });
            current.AddRange(SiteSettings.DefaultExportColumns(context: context));
            sources.AddRange(SiteSettings.ExportColumns(
                context: context, searchText: searchText));
            Session_Export(
                context: context,
                value: new Export(current));
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
        private void SetExportColumnsSelectableBySearch(Context context, ResponseCollection res)
        {
            SiteSettings.SetExports(context: context);
            var join = Forms.Data("ExportJoin").Deserialize<Join>();
            var searchText = Forms.Data("SearchExportColumns");
            var sources = new List<ExportColumn>();
            var allows = join?
                .Where(o => SiteSettings.JoinedSsHash?.ContainsKey(o.SiteId) == true)
                .ToList();
            allows?.Reverse();
            allows?.ForEach(link => sources.AddRange(SiteSettings
                .JoinedSsHash
                .Get(link.SiteId)
                .ExportColumns(context: context, searchText: searchText)));
            sources.AddRange(SiteSettings.ExportColumns(
                context: context, searchText: searchText));
            res
                .Html("#ExportSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: ExportUtilities
                        .SourceColumnOptions(sources)))
                .SetFormData("ExportSourceColumns", "[]");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteExports(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = Forms.IntList("EditExport");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Exports.Delete(selected);
                    res.ReplaceAll("#EditExport", new HtmlBuilder()
                        .EditExport(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportColumnsDialog(
            Context context, ResponseCollection res, string controlId)
        {
            Export = Session_Export(context: context);
            SiteSettings.SetExport(context: context, export: Export);
            var selected = Forms.List("ExportColumns");
            if (selected.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                SiteSettings.SetExports(context: context);
                int id = 1;
                var columns = new List<ExportColumn>();
                var selectedNewId = "";
                Forms.List("ExportColumnsAll").ForEach(o =>
                {
                    var exp = o.Deserialize<ExportColumn>() ?? Export.Columns.Where(c => c.Id.ToString() == o).FirstOrDefault();
                    if (exp.Id.ToString() == selected[0]) selectedNewId = id.ToString();
                    columns.Add(new ExportColumn()
                    {
                        SiteId = exp.SiteId,
                        Id = id++,
                        ColumnName = exp.ColumnName,
                        LabelText = exp.LabelText,
                        Type = exp.Type,
                        Format = exp.Format,
                        SiteTitle = exp.SiteTitle,
                        Column = exp.Column
                    });
                });
                Export.Columns = columns;
                SiteSettings.SetExport(context: context, export: Export);
                var column = Export.Columns.FirstOrDefault(o =>
                    o.Id.ToString() == selectedNewId);
                if (column == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    OpenExportColumnsDialog(
                        context: context,
                        res: res,
                        column: column);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportColumnsDialog(
            Context context, ResponseCollection res, ExportColumn column)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res.Html("#ExportColumnsDialog", SiteUtilities.ExportColumnsDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: Forms.ControlId(),
                    exportColumn: column));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateExportColumns(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                Export = Session_Export(context: context);
                SiteSettings.SetExport(context: context, export: Export);
                var column = Export.Columns.FirstOrDefault(o =>
                    o.Id == Forms.Int("ExportColumnId"));
                if (column == null)
                {
                    res.Message(Messages.NotFound(context: context));
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
        private void SetStylesOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = Forms.IntList("EditStyle");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Styles.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditStyle", new HtmlBuilder()
                    .EditStyle(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenStyleDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewStyle")
            {
                var style = new Style() { All = true };
                OpenStyleDialog(
                    context: context,
                    res: res,
                    style: style);
            }
            else
            {
                var style = SiteSettings.Styles?.Get(Forms.Int("StyleId"));
                if (style == null)
                {
                    OpenDialogError(
                        res: res,
                        message: Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenStyleDialog(
                        context: context,
                        res: res,
                        style: style);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenStyleDialog(Context context, ResponseCollection res, Style style)
        {
            res.Html("#StyleDialog", SiteUtilities.StyleDialog(
                context: context,
                ss: SiteSettings,
                controlId: Forms.ControlId(),
                style: style));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddStyle(Context context, ResponseCollection res, string controlId)
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
                    .EditStyle(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateStyle(Context context, ResponseCollection res, string controlId)
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
                    .EditStyle(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteStyles(Context context, ResponseCollection res)
        {
            var selected = Forms.IntList("EditStyle");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Styles.Delete(selected);
                res.ReplaceAll("#EditStyle", new HtmlBuilder()
                    .EditStyle(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetScriptsOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = Forms.IntList("EditScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Scripts.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditScript", new HtmlBuilder()
                    .EditScript(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenScriptDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewScript")
            {
                var script = new Script() { All = true };
                OpenScriptDialog(
                    context: context,
                    res: res,
                    script: script);
            }
            else
            {
                var script = SiteSettings.Scripts?.Get(Forms.Int("ScriptId"));
                if (script == null)
                {
                    OpenDialogError(
                        res: res,
                        message: Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenScriptDialog(
                        context: context,
                        res: res,
                        script: script);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenScriptDialog(Context context, ResponseCollection res, Script script)
        {
            res.Html("#ScriptDialog", SiteUtilities.ScriptDialog(
                context: context,
                ss: SiteSettings,
                controlId: Forms.ControlId(),
                script: script));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddScript(Context context, ResponseCollection res, string controlId)
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
                    .EditScript(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateScript(Context context, ResponseCollection res, string controlId)
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
                    .EditScript(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteScripts(Context context, ResponseCollection res)
        {
            var selected = Forms.IntList("EditScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Scripts.Delete(selected);
                res.ReplaceAll("#EditScript", new HtmlBuilder()
                    .EditScript(
                        context: context,
                        ss: SiteSettings));
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
        public SiteModel InheritSite(Context context)
        {
            return new SiteModel(context: context, siteId: InheritPermission);
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

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetRelatingColumnsOrder(
            Context context, ResponseCollection res, string controlId)
        {
            var selected = Forms.IntList("EditRelatingColumns");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.RelatingColumns.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditRelatingColumns", new HtmlBuilder()
                    .EditRelatingColumns(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenRelatingColumnDialog(
            Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewRelatingColumn")
            {
                var relatingColumn = new RelatingColumn() { };
                OpenRelatingColumnDialog(
                    context: context,
                    res: res,
                    relatingColumn: relatingColumn);
            }
            else
            {
                var RelatingColumn = SiteSettings.RelatingColumns?.Get(Forms.Int("RelatingColumnId"));
                if (RelatingColumn == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenRelatingColumnDialog(
                        context: context,
                        res: res,
                        relatingColumn: RelatingColumn);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenRelatingColumnDialog(
            Context context, ResponseCollection res, RelatingColumn relatingColumn)
        {
            res.Html("#RelatingColumnDialog", SiteUtilities.RelatingColumnDialog(
                context: context,
                ss: SiteSettings,
                controlId: Forms.ControlId(),
                relatingColumn: relatingColumn));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddRelatingColumn(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.RelatingColumns.Add(new RelatingColumn(
                id: SiteSettings.RelatingColumns.MaxOrDefault(o => o.Id) + 1,
                title: Forms.Data("RelatingColumnTitle"),
                columns: Forms.List("RelatingColumnColumnsAll")));
            res
                .ReplaceAll("#EditRelatingColumns", new HtmlBuilder()
                    .EditRelatingColumns(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateRelatingColumn(
            Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.RelatingColumns?
                .FirstOrDefault(o => o.Id == Forms.Int("RelatingColumnId"))?
                .Update(
                    title: Forms.Data("RelatingColumnTitle"),
                    columns: Forms.List("RelatingColumnColumnsAll"));
            res
                .Html("#EditRelatingColumns", new HtmlBuilder()
                    .EditRelatingColumns(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteRelatingColumns(Context context, ResponseCollection res)
        {
            var selected = Forms.IntList("EditRelatingColumns");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.RelatingColumns.Delete(selected);
                res.ReplaceAll("#EditRelatingColumns", new HtmlBuilder()
                    .EditRelatingColumns(
                        context: context,
                        ss: SiteSettings));
            }
        }
    }
}
