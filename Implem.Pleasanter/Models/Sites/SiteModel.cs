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
using System.Data.Common;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class SiteModel : BaseItemModel
    {
        public SiteSettings SiteSettings;
        public int TenantId = 0;
        public string GridGuide = string.Empty;
        public string EditorGuide = string.Empty;
        public string ReferenceType = "Sites";
        public long ParentId = 0;
        public long InheritPermission = 0;
        public bool Publish = false;
        public Time LockedTime = new Time();
        public User LockedUser = new User();
        public SiteCollection Ancestors = null;
        public int SiteMenu = 0;
        public List<string> MonitorChangesColumns = null;
        public List<string> TitleColumns = null;
        public Export Export = null;
        public DateTime ApiCountDate = 0.ToDateTime();
        public int ApiCount = 0;

        public TitleBody TitleBody
        {
            get
            {
                return new TitleBody(SiteId, Ver, VerType == Versions.VerTypes.History, Title.Value, Title.DisplayValue, Body);
            }
        }

        public int SavedTenantId = 0;
        public string SavedGridGuide = string.Empty;
        public string SavedEditorGuide = string.Empty;
        public string SavedReferenceType = "Sites";
        public long SavedParentId = 0;
        public long SavedInheritPermission = 0;
        public string SavedSiteSettings = string.Empty;
        public bool SavedPublish = false;
        public DateTime SavedLockedTime = 0.ToDateTime();
        public int SavedLockedUser = 0;
        public SiteCollection SavedAncestors = null;
        public int SavedSiteMenu = 0;
        public List<string> SavedMonitorChangesColumns = null;
        public List<string> SavedTitleColumns = null;
        public Export SavedExport = null;
        public DateTime SavedApiCountDate = 0.ToDateTime();
        public int SavedApiCount = 0;

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool GridGuide_Updated(Context context, Column column = null)
        {
            return GridGuide != SavedGridGuide && GridGuide != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != GridGuide);
        }

        public bool EditorGuide_Updated(Context context, Column column = null)
        {
            return EditorGuide != SavedEditorGuide && EditorGuide != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != EditorGuide);
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

        public bool Publish_Updated(Context context, Column column = null)
        {
            return Publish != SavedPublish &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != Publish);
        }

        public bool LockedUser_Updated(Context context, Column column = null)
        {
            return LockedUser.Id != SavedLockedUser &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != LockedUser.Id);
        }

        public bool ApiCount_Updated(Context context, Column column = null)
        {
            return ApiCount != SavedApiCount &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != ApiCount);
        }

        public bool LockedTime_Updated(Context context, Column column = null)
        {
            return LockedTime.Value != SavedLockedTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != LockedTime.Value.Date);
        }

        public bool ApiCountDate_Updated(Context context, Column column = null)
        {
            return ApiCountDate != SavedApiCountDate &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != ApiCountDate.Date);
        }

        public SiteSettings Session_SiteSettings(Context context)
        {
            return context.SessionData.Get("SiteSettings") != null
                ? context.SessionData.Get("SiteSettings")?.ToString().DeserializeSiteSettings(context: context) ?? new SiteSettings(context: context, referenceType: ReferenceType)
                : SiteSettings;
        }

        public void Session_SiteSettings(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "SiteSettings",
                value: value,
                page: true);
        }

        public List<string> Session_MonitorChangesColumns(Context context)
        {
            return context.SessionData.Get("MonitorChangesColumns") != null
                ? context.SessionData.Get("MonitorChangesColumns").Deserialize<List<string>>() ?? new List<string>()
                : MonitorChangesColumns;
        }

        public void Session_MonitorChangesColumns(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "MonitorChangesColumns",
                value: value,
                page: true);
        }

        public List<string> Session_TitleColumns(Context context)
        {
            return context.SessionData.Get("TitleColumns") != null
                ? context.SessionData.Get("TitleColumns").Deserialize<List<string>>() ?? new List<string>()
                : TitleColumns;
        }

        public void Session_TitleColumns(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "TitleColumns",
                value: value,
                page: true);
        }

        public Export Session_Export(Context context)
        {
            return context.SessionData.Get("Export") != null
                ? context.SessionData.Get("Export").Deserialize<Export>() ?? new Export()
                : Export;
        }

        public void Session_Export(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "Export",
                value: value,
                page: true);
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
                case "GridGuide": return GridGuide;
                case "EditorGuide": return EditorGuide;
                case "ReferenceType": return ReferenceType;
                case "ParentId": return ParentId.ToString();
                case "InheritPermission": return InheritPermission.ToString();
                case "SiteSettings": return SiteSettings.RecordingJson(context: context);
                case "Publish": return Publish.ToString();
                case "LockedTime": return LockedTime.Value.ToString();
                case "LockedUser": return LockedUser.Id.ToString();
                case "Ancestors": return Ancestors.ToString();
                case "SiteMenu": return SiteMenu.ToString();
                case "MonitorChangesColumns": return MonitorChangesColumns.ToString();
                case "TitleColumns": return TitleColumns.ToString();
                case "Export": return Export.ToString();
                case "ApiCountDate": return ApiCountDate.ToString();
                case "ApiCount": return ApiCount.ToString();
                case "Comments": return Comments.ToJson();
                case "Creator": return Creator.Id.ToString();
                case "Updator": return Updator.Id.ToString();
                case "CreatedTime": return CreatedTime.Value.ToString();
                case "VerUp": return VerUp.ToString();
                case "Timestamp": return Timestamp;
                default: return Value(
                    context: context,
                    columnName: name);
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
                    case "GridGuide":
                        hash.Add("GridGuide", GridGuide);
                        break;
                    case "EditorGuide":
                        hash.Add("EditorGuide", EditorGuide);
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
                    case "Publish":
                        hash.Add("Publish", Publish.ToString());
                        break;
                    case "LockedTime":
                        hash.Add("LockedTime", LockedTime.Value.ToString());
                        break;
                    case "LockedUser":
                        hash.Add("LockedUser", LockedUser.Id.ToString());
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
                    case "ApiCountDate":
                        hash.Add("ApiCountDate", ApiCountDate.ToString());
                        break;
                    case "ApiCount":
                        hash.Add("ApiCount", ApiCount.ToString());
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
                    default:
                        hash.Add(name, Value(
                            context: context,
                            columnName: name));
                        break;
                }
            });
            return hash;
        }

        public bool PropertyUpdated(Context context, string name)
        {
            switch (name)
            {
                case "TenantId": return TenantId_Updated(context: context);
                case "Ver": return Ver_Updated(context: context);
                case "Title": return Title_Updated(context: context);
                case "Body": return Body_Updated(context: context);
                case "GridGuide": return GridGuide_Updated(context: context);
                case "EditorGuide": return EditorGuide_Updated(context: context);
                case "ReferenceType": return ReferenceType_Updated(context: context);
                case "ParentId": return ParentId_Updated(context: context);
                case "InheritPermission": return InheritPermission_Updated(context: context);
                case "SiteSettings": return SiteSettings_Updated(context: context);
                case "Publish": return Publish_Updated(context: context);
                case "LockedTime": return LockedTime_Updated(context: context);
                case "LockedUser": return LockedUser_Updated(context: context);
                case "ApiCountDate": return ApiCountDate_Updated(context: context);
                case "ApiCount": return ApiCount_Updated(context: context);
                case "Comments": return Comments_Updated(context: context);
                case "Creator": return Creator_Updated(context: context);
                case "Updator": return Updator_Updated(context: context);
                default: 
                    switch (Def.ExtendedColumnTypes.Get(name))
                    {
                        case "Class": return Class_Updated(name);
                        case "Num": return Num_Updated(name);
                        case "Date": return Date_Updated(name);
                        case "Description": return Description_Updated(name);
                        case "Check": return Check_Updated(name);
                        case "Attachments": return Attachments_Updated(name);
                    }
                    break;
            }
            return false;
        }

        public List<long> SwitchTargets;

        public SiteModel()
        {
        }

        public SiteModel(
            Context context,
            long parentId,
            long inheritPermission,
            Dictionary<string, string> formData = null,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            ParentId = parentId;
            InheritPermission = inheritPermission;
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    formData: formData);
            }
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public SiteModel(
            Context context,
            long siteId,
            Dictionary<string, string> formData = null,
            bool setByApi = false,
            bool clearSessions = false,
            List<long> switchTargets = null,
            Dictionary<long, DataSet> linkedSsDataSetHash = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            SiteId = siteId;
            Get(context: context, linkedSsDataSetHash: linkedSsDataSetHash);
            if (clearSessions) ClearSessions(context: context);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    formData: formData);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public SiteModel(
            Context context,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    formData: formData);
            }
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
            Dictionary<long, DataSet> linkedSsDataSetHash = null,
            bool distinct = false,
            int top = 0)
        {
            Set(context, Repository.ExecuteTable(
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
            SetSiteSettingsProperties(context: context, linkedSsDataSetHash: linkedSsDataSetHash);
            return this;
        }

        public string FullText(
            Context context,
            SiteSettings ss,
            bool backgroundTask = false,
            bool onCreating = false)
        {
            if (!Parameters.Search.CreateIndexes && !backgroundTask) return null;
            if (AccessStatus == Databases.AccessStatuses.NotFound) return null;
            if (ReferenceType == "Wikis") return null;
            var fullText = new List<string>();
            SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu.Breadcrumb(context: context, siteId: SiteId)
                .FullText(context, fullText);
            SiteId.FullText(context, fullText);
            ss.GetEditorColumnNames(
                context: context,
                columnOnly: true).ForEach(columnName =>
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
                        default:
                            FullText(
                                context: context,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: columnName),
                                fullText: fullText);
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

        public ErrorData Update(
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
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                permissions: permissions,
                permissionChanged: permissionChanged,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements));
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
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: SiteId);
            }
            if (get)
            {
                Get(context: context);
            }
            UpdateRelatedRecords(
                context: context,
                get: get,
                addUpdatedTimeParam: true,
                addUpdatorParam: true,
                updateItems: true);
            SiteInfo.Reflesh(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.SitesWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp))
            {
                statements.Add(Rds.SitesCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            if (permissionChanged)
            {
                statements.UpdatePermissions(context, ss, SiteId, permissions, site: true);
            }
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            return new List<SqlStatement>
            {
                Rds.UpdateSites(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.SitesParamDefault(
                        context: context,
                        siteModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(SiteId)) {
                    IfConflicted = true,
                    Id = SiteId
                },
                StatusUtilities.UpdateStatus(
                    tenantId: TenantId,
                    type: StatusUtilities.Types.SitesUpdated),
            };
        }

        private List<SqlStatement> UpdateAttachmentsStatements(Context context)
        {
            var statements = new List<SqlStatement>();
            ColumnNames()
                .Where(columnName => columnName.StartsWith("Attachments"))
                .Where(columnName => Attachments_Updated(columnName: columnName))
                .ForEach(columnName =>
                    Attachments(columnName: columnName).Write(
                        context: context,
                        statements: statements,
                        referenceId: SiteId));
            return statements;
        }

        public void UpdateRelatedRecords(
            Context context,
            bool get = false,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: UpdateRelatedRecordsStatements(
                    context: context,
                    addUpdatedTimeParam: addUpdatedTimeParam,
                    addUpdatorParam: addUpdatorParam,
                    updateItems: updateItems)
                        .ToArray());
        }

        public List<SqlStatement> UpdateRelatedRecordsStatements(
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
            return statements;
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    selectIdentity: true,
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
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            SiteId = (response.Id ?? SiteId).ToLong();
            Get(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss)
        {
            var siteMenu = SiteInfo.TenantCaches.Get(TenantId)?
                .SiteMenu
                .Children(
                    context: context,
                    siteId: ss.SiteId,
                    withParent: true);
            var outside = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteId()
                        .Title(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .InheritPermission_In(siteMenu.Select(o => o.SiteId))))
                            .AsEnumerable()
                            .FirstOrDefault(o => !siteMenu.Any(p => p.SiteId == o.Long("SiteId")));
            if (outside != null)
            {
                return new ErrorData(
                    type: Error.Types.CannotDeletePermissionInherited,
                    data: $"{outside.Long("SiteId")} {outside.String("Title")}");
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        factory: context,
                        where: Rds.ItemsWhere().SiteId_In(siteMenu.Select(o => o.SiteId))),
                    Rds.DeleteIssues(
                        factory: context,
                        where: Rds.IssuesWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Issues")
                            .Select(o => o.SiteId))),
                    Rds.DeleteResults(
                        factory: context,
                        where: Rds.ResultsWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Results")
                            .Select(o => o.SiteId))),
                    Rds.DeleteWikis(
                        factory: context,
                        where: Rds.WikisWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Wikis")
                            .Select(o => o.SiteId))),
                    Rds.DeleteSites(
                        factory: context,
                        where: Rds.SitesWhere()
                            .TenantId(TenantId)
                            .SiteId_In(siteMenu.Select(o => o.SiteId)))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, long siteId)
        {
            SiteId = siteId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        factory: context,
                        where: Rds.ItemsWhere().ReferenceId(SiteId)),
                    Rds.RestoreSites(
                        factory: context,
                        where: Rds.SitesWhere().SiteId(SiteId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: TenantId,
                        type: StatusUtilities.Types.SitesUpdated),
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteSites(
                    tableType: tableType,
                    param: Rds.SitesParam().TenantId(TenantId).SiteId(SiteId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByForm(
            Context context,
            Dictionary<string, string> formData)
        {
            var ss = new SiteSettings();
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Sites_Title": Title = new Title(SiteId, value); break;
                    case "Sites_Body": Body = value.ToString(); break;
                    case "Sites_GridGuide": GridGuide = value.ToString(); break;
                    case "Sites_EditorGuide": EditorGuide = value.ToString(); break;
                    case "Sites_ReferenceType": ReferenceType = value.ToString(); break;
                    case "Sites_InheritPermission": InheritPermission = value.ToLong(); break;
                    case "Sites_Publish": Publish = value.ToBool(); break;
                    case "Sites_ApiCountDate": ApiCountDate = value.ToDateTime().ToUniversal(context: context); break;
                    case "Sites_ApiCount": ApiCount = value.ToInt(); break;
                    case "Sites_Timestamp": Timestamp = value.ToString(); break;
                    case "Comments": Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: value); break;
                    case "VerUp": VerUp = value.ToBool(); break;
                    default:
                        if (key.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: key.Substring("Comment".Length).ToInt(),
                                body: value);
                        }
                        else
                        {
                            var column = ss.GetColumn(
                                context: context,
                                columnName: key.Split_2nd('_'));
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.ColumnName,
                                        value: column.Round(value.ToDecimal(
                                            cultureInfo: context.CultureInfo())));
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.ColumnName,
                                        value: value.ToDateTime().ToUniversal(context: context));
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.ColumnName,
                                        value: value.ToBool());
                                    break;
                                case "Attachments":
                                    Attachments(
                                        columnName: column.ColumnName,
                                        value: value.Deserialize<Attachments>());
                                    break;
                            }
                        }
                        break;
                }
            });
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Ver = context.QueryStrings.Int("ver");
            }
            SetSiteSettings(context: context);
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = formData.Get("ControlId")?
                    .Split(',')
                    ._2nd()
                    .ToInt() ?? 0;
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        public void SetByModel(SiteModel siteModel)
        {
            TenantId = siteModel.TenantId;
            UpdatedTime = siteModel.UpdatedTime;
            Title = siteModel.Title;
            Body = siteModel.Body;
            GridGuide = siteModel.GridGuide;
            EditorGuide = siteModel.EditorGuide;
            ReferenceType = siteModel.ReferenceType;
            ParentId = siteModel.ParentId;
            InheritPermission = siteModel.InheritPermission;
            SiteSettings = siteModel.SiteSettings;
            Publish = siteModel.Publish;
            LockedTime = siteModel.LockedTime;
            LockedUser = siteModel.LockedUser;
            Ancestors = siteModel.Ancestors;
            SiteMenu = siteModel.SiteMenu;
            MonitorChangesColumns = siteModel.MonitorChangesColumns;
            TitleColumns = siteModel.TitleColumns;
            Export = siteModel.Export;
            ApiCountDate = siteModel.ApiCountDate;
            ApiCount = siteModel.ApiCount;
            Comments = siteModel.Comments;
            Creator = siteModel.Creator;
            Updator = siteModel.Updator;
            CreatedTime = siteModel.CreatedTime;
            VerUp = siteModel.VerUp;
            Comments = siteModel.Comments;
            ClassHash = siteModel.ClassHash;
            NumHash = siteModel.NumHash;
            DateHash = siteModel.DateHash;
            DescriptionHash = siteModel.DescriptionHash;
            CheckHash = siteModel.CheckHash;
            AttachmentsHash = siteModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.RequestDataString.Deserialize<SiteApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.Title != null) Title = new Title(SiteId, data.Title);
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.GridGuide != null) GridGuide = data.GridGuide.ToString().ToString();
            if (data.EditorGuide != null) EditorGuide = data.EditorGuide.ToString().ToString();
            if (data.ReferenceType != null) ReferenceType = data.ReferenceType.ToString().ToString();
            if (data.InheritPermission != null) InheritPermission = data.InheritPermission.ToLong().ToLong();
            if (data.Publish != null) Publish = data.Publish.ToBool().ToBool();
            if (data.ApiCountDate != null) ApiCountDate = data.ApiCountDate.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.ApiCount != null) ApiCount = data.ApiCount.ToInt().ToInt();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash?.ForEach(o => Class(
                columnName: o.Key,
                value: o.Value));
            data.NumHash?.ForEach(o => Num(
                columnName: o.Key,
                value: o.Value));
            data.DateHash?.ForEach(o => Date(
                columnName: o.Key,
                value: o.Value.ToDateTime().ToUniversal(context: context)));
            data.DescriptionHash?.ForEach(o => Description(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash?.ForEach(o => Check(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash?.ForEach(o =>
            {
                string columnName = o.Key;
                Attachments newAttachments = o.Value;
                Attachments oldAttachments = AttachmentsHash.Get(columnName);
                if (oldAttachments != null)
                {
                    var newGuidSet = new HashSet<string>(newAttachments.Select(x => x.Guid).Distinct());
                    newAttachments.AddRange(oldAttachments.Where((oldvalue) => !newGuidSet.Contains(oldvalue.Guid)));
                }
                Attachments(columnName: columnName, value: newAttachments);
            });
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
                        case "UpdatedTime":
                            match = UpdatedTime.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Body":
                            match = Body.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "CreatedTime":
                            match = CreatedTime.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(filter.Key))
                            {
                                case "Class":
                                    match = Class(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Num":
                                    match = Num(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Date":
                                    match = Date(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Description":
                                    match = Description(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Check":
                                    match = Check(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                            }
                            break;
                    }
                    if (!match) return false;
                }
            }
            return true;
        }

        private void SetBySession(Context context)
        {
            if (!context.Forms.Exists("Sites_SiteSettings")) SiteSettings = Session_SiteSettings(context: context);
            if (!context.Forms.Exists("Sites_MonitorChangesColumns")) MonitorChangesColumns = Session_MonitorChangesColumns(context: context);
            if (!context.Forms.Exists("Sites_TitleColumns")) TitleColumns = Session_TitleColumns(context: context);
            if (!context.Forms.Exists("Sites_Export")) Export = Session_Export(context: context);
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
                        case "GridGuide":
                            GridGuide = dataRow[column.ColumnName].ToString();
                            SavedGridGuide = GridGuide;
                            break;
                        case "EditorGuide":
                            EditorGuide = dataRow[column.ColumnName].ToString();
                            SavedEditorGuide = EditorGuide;
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
                        case "Publish":
                            Publish = dataRow[column.ColumnName].ToBool();
                            SavedPublish = Publish;
                            break;
                        case "LockedTime":
                            LockedTime = new Time(context, dataRow, column.ColumnName);
                            SavedLockedTime = LockedTime.Value;
                            break;
                        case "LockedUser":
                            LockedUser = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedLockedUser = LockedUser.Id;
                            break;
                        case "ApiCountDate":
                            ApiCountDate = dataRow[column.ColumnName].ToDateTime();
                            SavedApiCountDate = ApiCountDate;
                            break;
                        case "ApiCount":
                            ApiCount = dataRow[column.ColumnName].ToInt();
                            SavedApiCount = ApiCount;
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
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedClass(
                                        columnName: column.Name,
                                        value: Class(columnName: column.Name));
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDecimal());
                                    SavedNum(
                                        columnName: column.Name,
                                        value: Num(columnName: column.Name));
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SavedDate(
                                        columnName: column.Name,
                                        value: Date(columnName: column.Name));
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedDescription(
                                        columnName: column.Name,
                                        value: Description(columnName: column.Name));
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SavedCheck(
                                        columnName: column.Name,
                                        value: Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    Attachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SavedAttachments(
                                        columnName: column.Name,
                                        value: Attachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return Updated()
                || TenantId_Updated(context: context)
                || Ver_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || GridGuide_Updated(context: context)
                || EditorGuide_Updated(context: context)
                || ReferenceType_Updated(context: context)
                || ParentId_Updated(context: context)
                || InheritPermission_Updated(context: context)
                || SiteSettings_Updated(context: context)
                || Publish_Updated(context: context)
                || LockedTime_Updated(context: context)
                || LockedUser_Updated(context: context)
                || ApiCountDate_Updated(context: context)
                || ApiCount_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
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
        public ErrorData Create(Context context, bool otherInitValue = false)
        {
            if (!otherInitValue)
            {
                SiteSettings = new SiteSettings(context: context, referenceType: ReferenceType);
            }
            TenantId = context.TenantId;
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam()
                            .ReferenceType("Sites")
                            .Title(Title.Value.MaxLength(1024))),
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
            SiteId = response.Id ?? SiteId;
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
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SiteSettings GetSiteSettings(Context context, DataRow dataRow)
        {
            return dataRow.String("SiteSettings").DeserializeSiteSettings(context: context) ??
                new SiteSettings(context: context, referenceType: ReferenceType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsPropertiesBySession(Context context)
        {
            SiteSettings = Session_SiteSettings(context: context);
            SetSiteSettingsProperties(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsProperties(
            Context context,
            Dictionary<long, DataSet> linkedSsDataSetHash = null)
        {
            if (SiteSettings == null)
            {
                SiteSettings = SiteSettingsUtilities.SitesSiteSettings(
                    context: context, siteId: SiteId);
            }
            SiteSettings.SiteId = SiteId;
            SiteSettings.Title = Title.Value;
            SiteSettings.Body = Body;
            SiteSettings.ParentId = ParentId;
            SiteSettings.InheritPermission = InheritPermission;
            SiteSettings.AccessStatus = AccessStatus;
            SiteSettings.LinkedSsDataSetHash = linkedSsDataSetHash;
            SiteSettings.SetLinkedSiteSettings(context: context);
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
            switch (invalid.Type)
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
                value: SiteSettings.RecordingJson(context: context));
            return res
                .SetMemory("formChanged", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteSettings(Context context, ResponseCollection res)
        {
            var controlId = context.Forms.ControlId();
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
                case "OpenAggregationDetailsDialog":
                    OpenAggregationDetailsDialog(
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
                case "ToDisableEditorColumns":
                    DeleteEditorColumns(
                        context: context,
                        res: res);
                    break;
                case "EditorColumnsTabs":
                    SetEditorColumnsTabsSelectable(
                        context: context,
                        res: res);
                    break;
                case "EditorSourceColumnsType":
                    SetEditorSourceColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "MoveUpTabs":
                case "MoveDownTabs":
                    SetTabsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewTabDialog":
                case "EditTabDialog":
                    OpenTabDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddTab":
                    AddTab(
                        context: context,
                        res: res);
                    break;
                case "UpdateTab":
                    UpdateTab(
                        context: context,
                        res: res);
                    break;
                case "DeleteTabs":
                    DeleteTabs(
                        context: context,
                        res: res);
                    break;
                case "ToEnableEditorColumns":
                    AddEditorColumns(
                        context: context,
                        res: res);
                    break;
                case "UpdateSection":
                    UpdateSection(
                        context: context,
                        res: res);
                    break;
                case "UpdateLink":
                    UpdateLink(
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
                    DeleteViews(
                        context: context,
                        res: res);
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
                case "MoveUpServerScripts":
                case "MoveDownServerScripts":
                    SetServerScriptsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewServerScript":
                case "EditServerScript":
                    OpenServerScriptDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddServerScript":
                    AddServerScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateServerScript":
                    UpdateServerScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteServerScripts":
                    DeleteServerScripts(
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
                    if (controlId.Contains("_NumericRange"))
                    {
                        OpenSetNumericRangeDialog(
                            context: context,
                            res: res,
                            controlId: controlId);
                    }
                    else if (controlId.Contains("_DateRange"))
                    {
                        OpenSetDateRangeDialog(
                            context: context,
                            res: res,
                            controlId: controlId);
                    }
                    else
                    {
                        context.Forms
                            .Where(o => o.Key != controlId)
                            .ForEach(data =>
                                SiteSettings.Set(
                                    context: context,
                                    propertyName: data.Key,
                                    value: data.Value));
                    }
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
            var selectedColumns = context.Forms.List("GridColumns");
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
                    SiteSettings.GridColumns = context.Forms.List("GridColumnsAll");
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
            var columnName = context.Forms.Data("GridColumnName");
            var column = SiteSettings.GridColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest(context: context));
            }
            else
            {
                context.Forms.ForEach(control => SiteSettings.SetColumnProperty(
                    context: context,
                    column: column,
                    propertyName: control.Key,
                    value: GridColumnValue(
                        context: context,
                        name: control.Key,
                        value: control.Value)));
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
        private string GridColumnValue(Context context, string name, string value)
        {
            switch (name)
            {
                case "GridDesign":
                    return context.Forms.Bool("UseGridDesign")
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
            SiteSettings.GridColumns = context.Forms.List("GridColumnsAll");
            var listItemCollection = SiteSettings.GridSelectableOptions(
                context: context, enabled: false, join: context.Forms.Data("GridJoin"));
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
            var selectedColumns = context.Forms.List("FilterColumns");
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
                    SiteSettings.FilterColumns = context.Forms.List("FilterColumnsAll");
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
            var columnName = context.Forms.Data("FilterColumnName");
            var column = SiteSettings.FilterColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest(context: context));
            }
            else
            {
                context.Forms.ForEach(control => SiteSettings.SetColumnProperty(
                    context: context,
                    column: column,
                    propertyName: control.Key,
                    value: control.Value));
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
            SiteSettings.FilterColumns = context.Forms.List("FilterColumnsAll");
            var listItemCollection = SiteSettings.FilterSelectableOptions(
                context: context, enabled: false, join: context.Forms.Data("FilterJoin"));
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
        private void OpenAggregationDetailsDialog(Context context, ResponseCollection res)
        {
            var selectedColumns = context.Forms.List("AggregationDestination");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var aggregation = SiteSettings.Aggregations
                    .FirstOrDefault(o => o.Id == selectedColumns.First().ToLong());
                if (aggregation == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    res.Html(
                        "#AggregationDetailsDialog",
                        new HtmlBuilder().AggregationDetailsDialog(
                            context: context,
                            ss: SiteSettings,
                            aggregation: aggregation));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetAggregations(Context context, ResponseCollection res, string controlId)
        {
            var selectedColumns = context.Forms.List("AggregationDestination");
            var selectedSourceColumns = context.Forms.List("AggregationSource");
            if (selectedColumns.Any() || selectedSourceColumns.Any())
            {
                SiteSettings.Aggregations = SiteSettings.GetAggregations(context: context);
                selectedColumns = SiteSettings.SetAggregations(
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
            Enum.TryParse(context.Forms.Data("AggregationType"), out type);
            var target = type != Aggregation.Types.Count
                ? context.Forms.Data("AggregationTarget")
                : string.Empty;
            var id = context.Forms.Long("SelectedAggregation");
            var aggregation = SiteSettings.Aggregations
                .FirstOrDefault(o => o.Id == id);
            if (aggregation == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                aggregation.Type = type;
                aggregation.Target = type != Aggregation.Types.Count
                    ? target
                    : null;
                res
                    .Html("#AggregationDestination", new HtmlBuilder()
                        .SelectableItems(
                            listItemCollection: SiteSettings
                                .AggregationDestination(context: context),
                            selectedValueTextCollection: id.ToString().ToSingleList()))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenEditorColumnDialog(Context context, ResponseCollection res)
        {
            var selectedColumns = context.Forms.List("EditorColumns");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var column = SiteSettings.EditorColumn(selectedColumns.FirstOrDefault());
                var section = SiteSettings.Sections.Get(SiteSettings.SectionId(selectedColumns
                    .FirstOrDefault()));
                var linkId = SiteSettings.LinkId(selectedColumns.FirstOrDefault());
                if (column == null && section == null && linkId == 0)
                {
                    res.Message(Messages.InvalidRequest(context: context));
                }
                else
                {
                    var titleColumns = SiteSettings.TitleColumns;
                    if (column?.ColumnName == "Title")
                    {
                        Session_TitleColumns(
                            context: context,
                            value: titleColumns.ToJson());
                    }
                    AddOrUpdateEditorColumnHash(context: context);
                    if (column != null)
                    {
                        res.Html(
                            "#EditorColumnDialog",
                            SiteUtilities.EditorColumnDialog(
                                context: context,
                                ss: SiteSettings,
                                column: column,
                                titleColumns: titleColumns));
                    }
                    else if(section != null)
                    {
                        res.Html("#EditorColumnDialog", SiteUtilities.SectionDialog(
                            context: context,
                            ss: SiteSettings,
                            controlId: context.Forms.ControlId(),
                            section: section));
                    }
                    else if (linkId != 0)
                    {
                        res.Html("#EditorColumnDialog", SiteUtilities.LinkDialog(
                            context: context,
                            ss: SiteSettings,
                            controlId: context.Forms.ControlId()));
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorColumn(Context context, ResponseCollection res)
        {
            var columnName = context.Forms.Data("EditorColumnName");
            var column = SiteSettings.EditorColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest(context: context));
            }
            else
            {
                if (column.ColumnName == "Title")
                {
                    SiteSettings.TitleColumns = context.Forms.List("TitleColumnsAll");
                }
                context.Forms.ForEach(control =>
                    SiteSettings.SetColumnProperty(
                        context: context,
                        column: column,
                        propertyName: control.Key,
                        value: control.Value));
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
            var ss = new SiteSettings(
                context: context,
                referenceType: ReferenceType);
            res.Html(
                "#EditorColumnDialog",
                SiteUtilities.EditorColumnDialog(
                    context: context,
                    ss: SiteSettings,
                    column: ss.GetColumn(
                        context: context,
                        columnName: context.Forms.Data("EditorColumnName")),
                    titleColumns: ss.TitleColumns));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddOrUpdateEditorColumnHash(Context context)
        {
            SiteSettings
                .AddOrUpdateEditorColumnHash(
                    editorColumnsAll: context.Forms.List("EditorColumnsAll"),
                    editorColumnsTabsTarget: context
                        .Forms
                        .Data("EditorColumnsTabsTarget"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteEditorColumns(Context context, ResponseCollection res)
        {
            AddOrUpdateEditorColumnHash(context: context);
            var selected = context.Forms.List("EditorColumns");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.EditorColumnHash?.ForEach(o => o
                    .Value
                    ?.RemoveAll(columnName => selected.Contains(columnName)));
                res.EditorColumnsResponses(
                    context: context,
                    ss: SiteSettings);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorColumnsTabsSelectable(Context context, ResponseCollection res)
        {
            AddOrUpdateEditorColumnHash(context: context);
            res.Html("#EditorColumns", new HtmlBuilder().SelectableItems(
                listItemCollection: SiteSettings.EditorSelectableOptions(
                    context: context,
                    tabId: context
                        .Forms
                        .Data(key: "EditorColumnsTabs")
                        .ToInt())))
                .Val(
                    "#EditorColumnsTabsTarget",
                    context.Forms.Data("EditorColumnsTabs"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorSourceColumnsSelectable(Context context, ResponseCollection res)
        {
            AddOrUpdateEditorColumnHash(context: context);
            res.EditorSourceColumnsResponses(
                context: context,
                ss: SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetTabsOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("Tabs");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Tabs?.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res
                    .TabResponses(
                        context: context,
                        ss: SiteSettings,
                        selected: selected)
                    .EditorColumnsResponses(
                        context: context,
                        ss: SiteSettings);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenTabDialog(Context context, ResponseCollection res, string controlId)
        {
            Libraries.Settings.Tab tab;
            if (controlId == "NewTabDialog")
            {
                OpenTabDialog(context: context, res: res, tab: null);
            }
            else
            {
                var idList = context.Forms.IntList("Tabs");
                if (idList.Count() != 1)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    tab = idList.First() == 0
                        ? new Tab
                        {
                            Id = 0,
                            LabelText = SiteSettings.GeneralTabLabelText
                        }
                        : SiteSettings.Tabs?.Get(idList.First());
                    if (tab == null)
                    {
                        OpenDialogError(res, Messages.SelectOne(context: context));
                    }
                    else
                    {
                        SiteSettings.Tabs = SiteSettings.Tabs?.Join(
                            context
                                .Forms
                                .List("TabsAll")
                                .Select((val, key) => new
                                    {
                                        Key = key,
                                        Val = val
                                    }),
                            v => v.Id, l => l.Val.ToInt(),
                            (v, l) => new { Tabs = v, OrderNo = l.Key })
                                .OrderBy(v => v.OrderNo)
                                .Select(v => v.Tabs)
                                .Aggregate(
                                    new SettingList<Tab>(),
                                    (list, data) => { list.Add(data); return list; });
                        OpenTabDialog(context: context, res: res, tab: tab);
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenTabDialog(
            Context context,
            ResponseCollection res,
            Libraries.Settings.Tab tab)
        {
            AddOrUpdateEditorColumnHash(context: context);
            SiteSettings.SetChoiceHash(context: context);
            res.Html("#TabDialog", SiteUtilities.Tab(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                tab: tab));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddTab(Context context, ResponseCollection res)
        {
            SiteSettings
                .AddTab(new Libraries.Settings.Tab(
                    context: context,
                    ss: SiteSettings));
            res
                .TabResponses(
                    context: context,
                    ss: SiteSettings,
                    selected: new List<int>
                    {
                        SiteSettings.TabLatestId.ToInt()
                    })
                .CloseDialog()
                .EditorColumnsResponses(
                    context: context,
                    ss: SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateTab(Context context, ResponseCollection res)
        {
            var selected = context.Forms.Int("TabId");
            var tab = SiteSettings.Tabs?.Get(selected);
            if(selected == 0)
            {
                SiteSettings.GeneralTabLabelText = context.Forms.Data("LabelText");
                res
                    .TabResponses(
                        context: context,
                        ss: SiteSettings,
                        selected: new List<int> { selected })
                    .CloseDialog()
                    .EditorColumnsResponses(
                        context: context,
                        ss: SiteSettings);
            }
            else if (tab == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                tab.SetByForm(
                    context: context,
                    ss: SiteSettings);
                res
                    .TabResponses(
                        context: context,
                        ss: SiteSettings,
                        selected: new List<int> { selected })
                    .CloseDialog()
                    .EditorColumnsResponses(
                        context: context,
                        ss: SiteSettings);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteTabs(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("Tabs");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else if (selected.Contains(0))
            {
                res.Message(Messages.CanNotDelete(
                    context: context,
                    Displays.General(context:context))).ToJson();
            }
            else
            {
                SiteSettings.Tabs?.RemoveAll(o => selected.Contains(o.Id));
                SiteSettings.EditorColumnHash?.RemoveAll((key, value) => SiteSettings
                    .TabId(key) != 0
                        && selected.Contains(SiteSettings.TabId(key)));
                res
                    .TabResponses(
                        context: context,
                        ss: SiteSettings)
                    .EditorColumnsResponses(
                        context: context,
                        ss: SiteSettings);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddEditorColumns(Context context, ResponseCollection res)
        {
            switch (context.Forms.Data("EditorSourceColumnsType"))
            {
                case "Others":
                    if (context.Forms.List("EditorSourceColumns")?.FirstOrDefault()?.StartsWith("_Section-") == true)
                    {
                        var sectionName = SiteSettings.SectionName(SiteSettings.AddSection(new Section
                        {
                            LabelText = Displays.Section(context: context)
                        }).Id);
                        var tab = SiteSettings
                            .EditorColumnHash
                            .Get(SiteSettings.TabName(context.Forms.Int("EditorColumnsTabsTarget")));
                        if (tab == null)
                        {
                            tab = new List<string>();
                            SiteSettings.AddOrUpdateEditorColumnHash(
                                editorColumnsAll: tab,
                                editorColumnsTabsTarget: context
                                    .Forms
                                    .Int("EditorColumnsTabsTarget")
                                    .ToStr());
                        }
                        tab.Add(sectionName);
                        res.Html(
                            "#EditorColumns",
                            new HtmlBuilder().SelectableItems(
                                listItemCollection: SiteSettings
                                    .EditorSelectableOptions(
                                        context: context,
                                        tabId: context.Forms.Int("EditorColumnsTabs")),
                                selectedValueTextCollection: new List<string> { sectionName }));
                    }
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateSection(Context context, ResponseCollection res)
        {
            var selected = context.Forms.Int("SectionId");
            var section = SiteSettings.Sections.Get(selected);
            var sectionName = SiteSettings.SectionName(section?.Id);
            if (section == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                section.SetByForm(
                    context: context,
                    ss: SiteSettings);
                res.Html(
                    "#EditorColumns",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings
                            .EditorSelectableOptions(
                                context: context,
                                tabId: SiteSettings
                                    .TabId(SiteSettings
                                        .EditorColumnHash
                                        .Where(o => o
                                            .Value?
                                            .Contains(sectionName) == true)
                                        .Select(o => o.Key)
                                        .FirstOrDefault()))))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateLink(Context context, ResponseCollection res)
        {
            res.CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummariesOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditSummary");
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
                        summary: new Summary(SiteSettings
                            .Destinations
                            .Values
                            .FirstOrDefault()
                            .SiteId));
                }
                else
                {
                    var summary = SiteSettings.Summaries?.Get(context.Forms.Int("SummaryId"));
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
                controlId: context.Forms.ControlId(),
                summary: summary));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummarySiteId(Context context, ResponseCollection res)
        {
            var siteId = context.Forms.Long("SummarySiteId");
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
                    type: context.Forms.Data("SummaryType")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddSummary(Context context, ResponseCollection res)
        {
            var siteId = context.Forms.Long("SummarySiteId");
            var destinationSs = SiteSettings.Destinations.Get(siteId);
            int? destinationCondition = context.Forms.Int("SummaryDestinationCondition");
            int? sourceCondition = context.Forms.Int("SummarySourceCondition");
            var error = SiteSettings.AddSummary(
                siteId,
                new SiteModel(context: context, siteId: context.Forms.Long("SummarySiteId")).ReferenceType,
                context.Forms.Data("SummaryDestinationColumn"),
                destinationSs?.Views?.Get(destinationCondition)?.Id,
                context.Forms.Bool("SummarySetZeroWhenOutOfCondition"),
                context.Forms.Data("SummaryLinkColumn"),
                context.Forms.Data("SummaryType"),
                context.Forms.Data("SummarySourceColumn"),
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
            var siteId = context.Forms.Long("SummarySiteId");
            var destinationSs = SiteSettings.Destinations.Get(siteId);
            int? destinationCondition = context.Forms.Int("SummaryDestinationCondition");
            int? sourceCondition = context.Forms.Int("SummarySourceCondition");
            var outOfCondition = context.Forms.Data("SummaryOutOfCondition").Trim();
            var error = SiteSettings.UpdateSummary(
                context.Forms.Int("SummaryId"),
                siteId,
                new SiteModel(context: context, siteId: context.Forms.Long("SummarySiteId")).ReferenceType,
                context.Forms.Data("SummaryDestinationColumn"),
                destinationSs?.Views?.Get(destinationCondition)?.Id,
                context.Forms.Bool("SummarySetZeroWhenOutOfCondition"),
                context.Forms.Data("SummaryLinkColumn"),
                context.Forms.Data("SummaryType"),
                context.Forms.Data("SummarySourceColumn"),
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
            var selected = context.Forms.IntList("EditSummary");
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
            var selected = context.Forms.IntList("EditFormula");
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
                var formulaSet = SiteSettings.Formulas?.Get(context.Forms.Int("FormulaId"));
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
                controlId: context.Forms.ControlId(),
                formulaSet: formulaSet));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddFormula(Context context, ResponseCollection res)
        {
            var outOfCondition = context.Forms.Data("FormulaOutOfCondition").Trim();
            var error = SiteSettings.AddFormula(
                context.Forms.Data("FormulaTarget"),
                context.Forms.Int("FormulaCondition"),
                context.Forms.Data("Formula"),
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
            var id = context.Forms.Int("FormulaId");
            var outOfCondition = context.Forms.Data("FormulaOutOfCondition").Trim();
            var error = SiteSettings.UpdateFormula(
                id,
                context.Forms.Data("FormulaTarget"),
                context.Forms.Int("FormulaCondition"),
                context.Forms.Data("Formula"),
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
            var selected = context.Forms.IntList("EditFormula");
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
                var idList = context.Forms.IntList("Views");
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
                            context.Forms.List("ViewsAll").Select((val, key) => new { Key = key, Val = val }), v => v.Id, l => l.Val.ToInt(),
                                (v, l) => new { Views = v, OrderNo = l.Key })
                                .OrderBy(v => v.OrderNo)
                                .Select(v => v.Views)
                                .ToList();
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
            SiteSettings.SetChoiceHash(context: context);
            res.Html("#ViewDialog", SiteUtilities.ViewDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                view: view));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddViewFilter(Context context, ResponseCollection res)
        {
            SiteSettings.SetChoiceHash(context: context);
            var column = SiteSettings.GetColumn(
                context: context,
                columnName: context.Forms.Data("ViewFilterSelector"));
            if (column != null)
            {
                res
                    .Append(
                        "#ViewFiltersTab .items",
                        new HtmlBuilder().ViewFilter(
                            context: context,
                            ss: SiteSettings,
                            column: column))
                    .Remove("#ViewFilterSelector option:selected");
            }
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
            var selected = context.Forms.Int("ViewId");
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
        private void DeleteViews(Context context, ResponseCollection res)
        {
            SiteSettings.Views?.RemoveAll(o =>
                context.Forms.IntList("Views").Contains(o.Id));
            res.ViewResponses(SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetViewGridColumnsSelectable(Context context, ResponseCollection res)
        {
            var gridColumns = context.Forms.List("ViewGridColumnsAll");
            var listItemCollection = SiteSettings.ViewGridSelectableOptions(
                context: context,
                gridColumns: gridColumns,
                enabled: false,
                join: context.Forms.Data("ViewGridJoin"));
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
                            .Where(o => SiteSettings.GetEditorColumnNames().Contains(o)
                                || o == "Comments")
                            .ToList()));
            }
            else
            {
                var notification = SiteSettings.Notifications?.Get(context.Forms.Int("NotificationId"));
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
                    value: notification.MonitorChangesColumns.ToJson());
                res.Html("#NotificationDialog", SiteUtilities.NotificationDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: context.Forms.ControlId(),
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
                    id: SiteSettings.Notifications.MaxOrDefault(o => o.Id) + 1,
                    type: (Notification.Types)context.Forms.Int("NotificationType"),
                    prefix: context.Forms.Data("NotificationPrefix"),
                    address: context.Forms.Data("NotificationAddress"),
                    token: context.Forms.Data("NotificationToken"),
                    useCustomFormat: context.Forms.Bool("NotificationUseCustomFormat"),
                    format: SiteSettings.LabelTextToColumnName(
                        context.Forms.Data("NotificationFormat")),
                    monitorChangesColumns: context.Forms.List("MonitorChangesColumnsAll"),
                    beforeCondition: context.Forms.Int("BeforeCondition"),
                    afterCondition: context.Forms.Int("AfterCondition"),
                    expression: (Notification.Expressions)context.Forms.Int("Expression"),
                    disabled: context.Forms.Bool("NotificationDisabled")));
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
                var notification = SiteSettings.Notifications.Get(context.Forms.Int("NotificationId"));
                if (notification == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    notification.Update(
                        type: (Notification.Types)context.Forms.Int("NotificationType"),
                        prefix: context.Forms.Data("NotificationPrefix"),
                        address: context.Forms.Data("NotificationAddress"),
                        token: context.Forms.Data("NotificationToken"),
                        useCustomFormat: context.Forms.Bool("NotificationUseCustomFormat"),
                        format: SiteSettings.LabelTextToColumnName(
                            context.Forms.Data("NotificationFormat")),
                        monitorChangesColumns: context.Forms.List("MonitorChangesColumnsAll"),
                        beforeCondition: context.Forms.Int("BeforeCondition"),
                        afterCondition: context.Forms.Int("AfterCondition"),
                        expression: (Notification.Expressions)context.Forms.Int("Expression"),
                        disabled: context.Forms.Bool("NotificationDisabled"));
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
                var selected = context.Forms.IntList("EditNotification");
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
                var selected = context.Forms.IntList("EditNotification");
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
                var reminder = SiteSettings.Reminders?.Get(context.Forms.Int("ReminderId"));
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
                    controlId: context.Forms.ControlId(),
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
                var invalid = SiteValidators.SetReminder(
                    context: context,
                    ss: SiteSettings);
                switch (invalid.Type)
                {
                    case Error.Types.None:
                        SiteSettings.Reminders.Add(new Reminder(
                            id: SiteSettings.Reminders.MaxOrDefault(o => o.Id) + 1,
                            subject: SiteSettings.LabelTextToColumnName(
                                context.Forms.Data("ReminderSubject")),
                            body: SiteSettings.LabelTextToColumnName(
                                context.Forms.Data("ReminderBody")),
                            line: SiteSettings.LabelTextToColumnName(
                                context.Forms.Data("ReminderLine")),
                            from: context.Forms.Data("ReminderFrom"),
                            to: SiteSettings.LabelTextToColumnName(
                                context.Forms.Data("ReminderTo")),
                            column: context.Forms.Data("ReminderColumn"),
                            startDateTime: context.Forms.DateTime("ReminderStartDateTime"),
                            type: (Times.RepeatTypes)context.Forms.Int("ReminderType"),
                            range: context.Forms.Int("ReminderRange"),
                            sendCompletedInPast: context.Forms.Bool("ReminderSendCompletedInPast"),
                            notSendIfNotApplicable: context.Forms.Bool("ReminderNotSendIfNotApplicable"),
                            notSendHyperLink: context.Forms.Bool("ReminderNotSendHyperLink"),
                            excludeOverdue: context.Forms.Bool("ReminderExcludeOverdue"),
                            condition: context.Forms.Int("ReminderCondition"),
                            disabled: context.Forms.Bool("ReminderDisabled")));
                        SetRemindersResponseCollection(context: context, res: res);
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
                var reminder = SiteSettings.Reminders.Get(context.Forms.Int("ReminderId"));
                if (reminder == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    var invalid = SiteValidators.SetReminder(
                        context: context,
                        ss: SiteSettings);
                    switch (invalid.Type)
                    {
                        case Error.Types.None:
                            reminder.Update(
                                subject: SiteSettings.LabelTextToColumnName(
                                    context.Forms.Data("ReminderSubject")),
                                body: SiteSettings.LabelTextToColumnName(
                                    context.Forms.Data("ReminderBody")),
                                line: SiteSettings.LabelTextToColumnName(
                                    context.Forms.Data("ReminderLine")),
                                from: context.Forms.Data("ReminderFrom"),
                                to: SiteSettings.LabelTextToColumnName(
                                    context.Forms.Data("ReminderTo")),
                                column: context.Forms.Data("ReminderColumn"),
                                startDateTime: context.Forms.DateTime("ReminderStartDateTime"),
                                type: (Times.RepeatTypes)context.Forms.Int("ReminderType"),
                                range: context.Forms.Int("ReminderRange"),
                                sendCompletedInPast: context.Forms.Bool("ReminderSendCompletedInPast"),
                                notSendIfNotApplicable: context.Forms.Bool("ReminderNotSendIfNotApplicable"),
                                notSendHyperLink: context.Forms.Bool("ReminderNotSendHyperLink"),
                                excludeOverdue: context.Forms.Bool("ReminderExcludeOverdue"),
                                condition: context.Forms.Int("ReminderCondition"),
                                disabled: context.Forms.Bool("ReminderDisabled"));
                            SetRemindersResponseCollection(context: context, res: res);
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
                var selected = context.Forms.IntList("EditReminder");
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
                var selected = context.Forms.IntList("EditReminder");
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
                var selected = context.Forms.IntList("EditReminder");
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
            if (controlId == "NewExport")
            {
                OpenExportDialog(context: context, res: res, export: new Export(SiteSettings
                    .DefaultExportColumns(context: context)));
            }
            else
            {
                var export = SiteSettings.Exports?.Get(context.Forms.Int("ExportId"));
                if (export == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    export.SetColumns(
                        context: context,
                        ss: SiteSettings);
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
                    value: export.ToJson());
                res.Html("#ExportDialog", SiteUtilities.ExportDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: context.Forms.ControlId(),
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
                Export = Session_Export(context: context);
                if (Export == null && controlId == "EditExport")
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    Export.SetColumns(
                        context: context,
                        ss: SiteSettings);
                    int id = 1;
                    var columns = new List<ExportColumn>();
                    context.Forms.List("ExportColumnsAll").ForEach(o =>
                    {
                        var exp = o.Deserialize<ExportColumn>()
                            ?? Session_Export(context: context)
                                .Columns
                                .Where(c => c.Id.ToString() == o)
                                .FirstOrDefault();
                        columns.Add(new ExportColumn()
                        {
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
                    Export.Name = context.Forms.Data("ExportName");
                    Export.Type = (Export.Types)context.Forms.Int("ExportType");
                    Export.Header = context.Forms.Bool("ExportHeader");
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
                var export = SiteSettings.Exports.Get(context.Forms.Int("ExportId"));
                if (export == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    int id = 1;
                    var columns = new List<ExportColumn>();
                    context.Forms.List("ExportColumnsAll").ForEach(o =>
                    {
                        var exp = o.Deserialize<ExportColumn>()
                            ?? Session_Export(context: context)
                                .Columns
                                .Where(c => c.Id.ToString() == o)
                                .FirstOrDefault();
                        columns.Add(new ExportColumn()
                        {
                            Id = id++,
                            ColumnName = exp.ColumnName,
                            LabelText = exp.LabelText,
                            Type = exp.Type,
                            Format = exp.Format,
                            SiteTitle = exp.SiteTitle,
                            Column = exp.Column
                        });
                    });
                    export.SetColumns(
                        context: context,
                        ss: SiteSettings);
                    export.Update(
                        name: context.Forms.Data("ExportName"),
                        type: (Export.Types)context.Forms.Int("ExportType"),
                        header: context.Forms.Bool("ExportHeader"),
                        columns: columns,
                        executionType: (Export.ExecutionTypes)context.Forms.Int("ExecutionType"));
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
                var selected = context.Forms.IntList("EditExport");
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
            var join = context.Forms.Data("ExportJoin");
            res
                .Html("#ExportSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: ExportUtilities
                        .ColumnOptions(columns: SiteSettings.ExportColumns(
                            context: context,
                            join: join))))
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
                var selected = context.Forms.IntList("EditExport");
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
            var selected = context.Forms.List("ExportColumns");
            if (selected.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                int id = 1;
                var columns = new List<ExportColumn>();
                var selectedNewId = "";
                context.Forms.List("ExportColumnsAll").ForEach(o =>
                {
                    var export = o.Deserialize<ExportColumn>()
                        ?? Export.Columns.Where(c => c.ToJson() == o).FirstOrDefault();
                    if (export.ToJson() == selected[0]) selectedNewId = id.ToString();
                    columns.Add(new ExportColumn()
                    {
                        Id = id++,
                        ColumnName = export.ColumnName,
                        LabelText = export.LabelText,
                        Type = export.Type,
                        Format = export.Format,
                        SiteTitle = export.SiteTitle,
                        Column = export.Column
                    });
                });
                Export.Columns = columns;
                Export.SetColumns(
                    context: context,
                    ss: SiteSettings);
                Session_Export(
                    context: context,
                    value: Export.ToJson());
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
                    controlId: context.Forms.ControlId(),
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
                Export.SetColumns(
                    context: context,
                    ss: SiteSettings);
                var column = Export.Columns.FirstOrDefault(o =>
                    o.Id == context.Forms.Int("ExportColumnId"));
                if (column == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    var selected = new List<string> { column.Id.ToString() };
                    column.Update(
                        context.Forms.Data("ExportColumnLabelText"),
                        (ExportColumn.Types)context.Forms.Int("ExportColumnType"),
                        context.Forms.Data("ExportFormat"));
                    Session_Export(
                        context: context,
                        value: Export.ToJson());
                    res
                        .Html("#ExportColumns", new HtmlBuilder().SelectableItems(
                            listItemCollection: ExportUtilities
                                .ColumnOptions(Export.Columns),
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
            var selected = context.Forms.IntList("EditStyle");
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
                var style = SiteSettings.Styles?.Get(context.Forms.Int("StyleId"));
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
                controlId: context.Forms.ControlId(),
                style: style));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddStyle(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.Styles.Add(new Style(
                id: SiteSettings.Styles.MaxOrDefault(o => o.Id) + 1,
                title: context.Forms.Data("StyleTitle"),
                all: context.Forms.Bool("StyleAll"),
                _new: context.Forms.Bool("StyleNew"),
                edit: context.Forms.Bool("StyleEdit"),
                index: context.Forms.Bool("StyleIndex"),
                calendar: context.Forms.Bool("StyleCalendar"),
                crosstab: context.Forms.Bool("StyleCrosstab"),
                gantt: context.Forms.Bool("StyleGantt"),
                burnDown: context.Forms.Bool("StyleBurnDown"),
                timeSeries: context.Forms.Bool("StyleTimeSeries"),
                kamban: context.Forms.Bool("StyleKamban"),
                imageLib: context.Forms.Bool("StyleImageLib"),
                body: context.Forms.Data("StyleBody")));
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
                .FirstOrDefault(o => o.Id == context.Forms.Int("StyleId"))?
                .Update(
                    title: context.Forms.Data("StyleTitle"),
                    all: context.Forms.Bool("StyleAll"),
                    _new: context.Forms.Bool("StyleNew"),
                    edit: context.Forms.Bool("StyleEdit"),
                    index: context.Forms.Bool("StyleIndex"),
                    calendar: context.Forms.Bool("StyleCalendar"),
                    crosstab: context.Forms.Bool("StyleCrosstab"),
                    gantt: context.Forms.Bool("StyleGantt"),
                    burnDown: context.Forms.Bool("StyleBurnDown"),
                    timeSeries: context.Forms.Bool("StyleTimeSeries"),
                    kamban: context.Forms.Bool("StyleKamban"),
                    imageLib: context.Forms.Bool("StyleImageLib"),
                    body: context.Forms.Data("StyleBody"));
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
            var selected = context.Forms.IntList("EditStyle");
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
            var selected = context.Forms.IntList("EditScript");
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
                var script = SiteSettings.Scripts?.Get(context.Forms.Int("ScriptId"));
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
                controlId: context.Forms.ControlId(),
                script: script));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddScript(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.Scripts.Add(new Script(
                id: SiteSettings.Scripts.MaxOrDefault(o => o.Id) + 1,
                title: context.Forms.Data("ScriptTitle"),
                all: context.Forms.Bool("ScriptAll"),
                _new: context.Forms.Bool("ScriptNew"),
                edit: context.Forms.Bool("ScriptEdit"),
                index: context.Forms.Bool("ScriptIndex"),
                calendar: context.Forms.Bool("ScriptCalendar"),
                crosstab: context.Forms.Bool("ScriptCrosstab"),
                gantt: context.Forms.Bool("ScriptGantt"),
                burnDown: context.Forms.Bool("ScriptBurnDown"),
                timeSeries: context.Forms.Bool("ScriptTimeSeries"),
                kamban: context.Forms.Bool("ScriptKamban"),
                imageLib: context.Forms.Bool("ScriptImageLib"),
                body: context.Forms.Data("ScriptBody")));
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
                .FirstOrDefault(o => o.Id == context.Forms.Int("ScriptId"))?
                .Update(
                    title: context.Forms.Data("ScriptTitle"),
                    all: context.Forms.Bool("ScriptAll"),
                    _new: context.Forms.Bool("ScriptNew"),
                    edit: context.Forms.Bool("ScriptEdit"),
                    index: context.Forms.Bool("ScriptIndex"),
                    calendar: context.Forms.Bool("ScriptCalendar"),
                    crosstab: context.Forms.Bool("ScriptCrosstab"),
                    gantt: context.Forms.Bool("ScriptGantt"),
                    burnDown: context.Forms.Bool("ScriptBurnDown"),
                    timeSeries: context.Forms.Bool("ScriptTimeSeries"),
                    kamban: context.Forms.Bool("ScriptKamban"),
                    imageLib: context.Forms.Bool("ScriptImageLib"),
                    body: context.Forms.Data("ScriptBody"));
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
            var selected = context.Forms.IntList("EditScript");
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
        private void SetServerScriptsOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditServerScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.ServerScripts.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditServerScript", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenServerScriptDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewServerScript")
            {
                var script = new ServerScript() { BeforeFormula = true };
                OpenServerScriptDialog(
                    context: context,
                    res: res,
                    script: script);
            }
            else
            {
                var script = SiteSettings.ServerScripts?.Get(context.Forms.Int("ServerScriptId"));
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
                    OpenServerScriptDialog(
                        context: context,
                        res: res,
                        script: script);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenServerScriptDialog(Context context, ResponseCollection res, ServerScript script)
        {
            res.Html("#ServerScriptDialog", SiteUtilities.ServerScriptDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                script: script));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddServerScript(Context context, ResponseCollection res, string controlId)
        {
            var script = new ServerScript(
                id: context.Forms.Int("ServerScriptId"),
                title: context.Forms.Data("ServerScriptTitle"),
                beforeOpeningPage: context.Forms.Bool("ServerScriptBeforeOpeningPage"),
                beforeFormula: context.Forms.Bool("ServerScriptBeforeFormula"),
                afterFormula: context.Forms.Bool("ServerScriptAfterFormula"),
                body: context.Forms.Data("ServerScriptBody"));
            var invalid = ServerScriptValidators.OnCreating(
                context: context,
                serverScript: script);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    res.Message(invalid.Message(context: context));
                    return;
            }
            SiteSettings.ServerScripts.Add(new ServerScript(
                id: SiteSettings.ServerScripts.MaxOrDefault(o => o.Id) + 1,
                title: script.Title,
                beforeOpeningPage: script.BeforeOpeningPage ?? default,
                beforeFormula: script.BeforeFormula ?? default,
                afterFormula: script.AfterFormula ?? default,
                body: script.Body));
            res
                .ReplaceAll("#EditServerScript", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateServerScript(Context context, ResponseCollection res, string controlId)
        {
            var script = new ServerScript(
                id: context.Forms.Int("ServerScriptId"),
                title: context.Forms.Data("ServerScriptTitle"),
                beforeOpeningPage: context.Forms.Bool("ServerScriptBeforeOpeningPage"),
                beforeFormula: context.Forms.Bool("ServerScriptBeforeFormula"),
                afterFormula: context.Forms.Bool("ServerScriptAfterFormula"),
                body: context.Forms.Data("ServerScriptBody"));
            var invalid = ServerScriptValidators.OnUpdating(
                context: context,
                serverScript: script);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    res.Message(invalid.Message(context: context));
                    return;
            }
            SiteSettings.ServerScripts?
                .FirstOrDefault(o => o.Id == script.Id)?
                .Update(
                    title: script.Title,
                    beforeOpeningPage: script.BeforeOpeningPage ?? default,
                    beforeFormula: script.BeforeFormula ?? default,
                    afterFormula: script.AfterFormula ?? default,
                    body: script.Body);
            res
                .Html("#EditServerScript", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteServerScripts(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditServerScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.ServerScripts.Delete(selected);
                res.ReplaceAll("#EditServerScript", new HtmlBuilder()
                    .EditServerScript(
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
                .CloseDialog()
                .Message(message);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetRelatingColumnsOrder(
            Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditRelatingColumns");
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
                var RelatingColumn = SiteSettings.RelatingColumns?.Get(context.Forms.Int("RelatingColumnId"));
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
                controlId: context.Forms.ControlId(),
                relatingColumn: relatingColumn));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddRelatingColumn(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.RelatingColumns.Add(new RelatingColumn(
                id: SiteSettings.RelatingColumns.MaxOrDefault(o => o.Id) + 1,
                title: context.Forms.Data("RelatingColumnTitle"),
                columns: context.Forms.List("RelatingColumnColumnsAll")));
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
                .FirstOrDefault(o => o.Id == context.Forms.Int("RelatingColumnId"))?
                .Update(
                    title: context.Forms.Data("RelatingColumnTitle"),
                    columns: context.Forms.List("RelatingColumnColumnsAll"));
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
            var selected = context.Forms.IntList("EditRelatingColumns");
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public string OpenSetNumericRangeDialog(
            Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.SetPermissions(
                context: context,
                referenceId: SiteId);
            if (context.CanRead(SiteSettings))
            {
                var columnName = controlId
                    .Replace("ViewFilters__", string.Empty)
                    .Replace("ViewFiltersOnGridHeader__", string.Empty)
                    .Replace("_NumericRange", string.Empty);
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: columnName);
                return res.Html(
                    target: "#SetNumericRangeDialog",
                    value: new HtmlBuilder().SetNumericRangeDialog(
                        context: context,
                        ss: SiteSettings,
                        column: column))
                            .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string OpenSetDateRangeDialog(
            Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.SetPermissions(
                context: context,
                referenceId: SiteId);
            if (context.CanRead(SiteSettings))
            {
                var columnName = controlId
                    .Replace("ViewFilters__", string.Empty)
                    .Replace("ViewFiltersOnGridHeader__", string.Empty)
                    .Replace("_DateRange", string.Empty);
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: columnName);
                return res.Html(
                    target: "#SetDateRangeDialog",
                    value: new HtmlBuilder().SetDateRangeDialog(
                        context: context,
                        ss: SiteSettings,
                        column: column))
                            .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool WithinApiLimits()
        {
            if (ApiCountDate.Date < DateTime.Now.Date)
            {
                ApiCountDate = DateTime.Now;
                ApiCount = 0;
            }
            return !(Parameters.Api.LimitPerSite != 0
                && ApiCount >= Parameters.Api.LimitPerSite);
        }
    }
}
