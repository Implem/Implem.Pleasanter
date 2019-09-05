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
    public class IssueModel : BaseItemModel
    {
        public long IssueId = 0;
        public DateTime StartTime = 0.ToDateTime();
        public CompletionTime CompletionTime = new CompletionTime();
        public WorkValue WorkValue = new WorkValue();
        public ProgressRate ProgressRate = new ProgressRate();
        public decimal RemainingWorkValue = 0;
        public Status Status = new Status();
        public User Manager = new User();
        public User Owner = new User();

        public TitleBody TitleBody
        {
            get
            {
                return new TitleBody(IssueId, Ver, VerType == Versions.VerTypes.History, Title.Value, Title.DisplayValue, Body);
            }
        }

        public SiteTitle SiteTitle
        {
            get
            {
                return new SiteTitle(SiteId);
            }
        }

        public long SavedIssueId = 0;
        public DateTime SavedStartTime = 0.ToDateTime();
        public DateTime SavedCompletionTime = 0.ToDateTime();
        public decimal SavedWorkValue = 0;
        public decimal SavedProgressRate = 0;
        public decimal SavedRemainingWorkValue = 0;
        public int SavedStatus = 0;
        public int SavedManager = 0;
        public int SavedOwner = 0;

        public bool WorkValue_Updated(Context context, Column column = null)
        {
            return WorkValue.Value != SavedWorkValue &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != WorkValue.Value);
        }

        public bool ProgressRate_Updated(Context context, Column column = null)
        {
            return ProgressRate.Value != SavedProgressRate &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != ProgressRate.Value);
        }

        public bool Status_Updated(Context context, Column column = null)
        {
            return Status.Value != SavedStatus &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Status.Value);
        }

        public bool Manager_Updated(Context context, Column column = null)
        {
            return Manager.Id != SavedManager &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Manager.Id);
        }

        public bool Owner_Updated(Context context, Column column = null)
        {
            return Owner.Id != SavedOwner &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Owner.Id);
        }

        public bool StartTime_Updated(Context context, Column column = null)
        {
            return StartTime != SavedStartTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != StartTime.Date);
        }

        public bool CompletionTime_Updated(Context context, Column column = null)
        {
            return CompletionTime.Value != SavedCompletionTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != CompletionTime.Value.Date);
        }

        public string PropertyValue(Context context, string name)
        {
            switch (name)
            {
                case "SiteId": return SiteId.ToString();
                case "UpdatedTime": return UpdatedTime.Value.ToString();
                case "IssueId": return IssueId.ToString();
                case "Ver": return Ver.ToString();
                case "Title": return Title.Value;
                case "Body": return Body;
                case "TitleBody": return TitleBody.ToString();
                case "StartTime": return StartTime.ToString();
                case "CompletionTime": return CompletionTime.Value.ToString();
                case "WorkValue": return WorkValue.Value.ToString();
                case "ProgressRate": return ProgressRate.Value.ToString();
                case "RemainingWorkValue": return RemainingWorkValue.ToString();
                case "Status": return Status.Value.ToString();
                case "Manager": return Manager.Id.ToString();
                case "Owner": return Owner.Id.ToString();
                case "SiteTitle": return SiteTitle.SiteId.ToString();
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
                    case "SiteId":
                        hash.Add("SiteId", SiteId.ToString());
                        break;
                    case "UpdatedTime":
                        hash.Add("UpdatedTime", UpdatedTime.Value.ToString());
                        break;
                    case "IssueId":
                        hash.Add("IssueId", IssueId.ToString());
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
                    case "StartTime":
                        hash.Add("StartTime", StartTime.ToString());
                        break;
                    case "CompletionTime":
                        hash.Add("CompletionTime", CompletionTime.Value.ToString());
                        break;
                    case "WorkValue":
                        hash.Add("WorkValue", WorkValue.Value.ToString());
                        break;
                    case "ProgressRate":
                        hash.Add("ProgressRate", ProgressRate.Value.ToString());
                        break;
                    case "RemainingWorkValue":
                        hash.Add("RemainingWorkValue", RemainingWorkValue.ToString());
                        break;
                    case "Status":
                        hash.Add("Status", Status.Value.ToString());
                        break;
                    case "Manager":
                        hash.Add("Manager", Manager.Id.ToString());
                        break;
                    case "Owner":
                        hash.Add("Owner", Owner.Id.ToString());
                        break;
                    case "SiteTitle":
                        hash.Add("SiteTitle", SiteTitle.SiteId.ToString());
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

        public string CsvData(
            Context context,
            SiteSettings ss,
            Column column,
            ExportColumn exportColumn,
            List<string> mine)
        {
            var value = string.Empty;
            switch (column.Name)
            {
                case "SiteId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? SiteId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UpdatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? UpdatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "IssueId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? IssueId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Ver":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Ver.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Title":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Title.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Body":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Body.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "TitleBody":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? TitleBody.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "StartTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? StartTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CompletionTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CompletionTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "WorkValue":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? WorkValue.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ProgressRate":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ProgressRate.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "RemainingWorkValue":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? RemainingWorkValue.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Status":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Status.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Manager":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Manager.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Owner":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Owner.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SiteTitle":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? SiteTitle.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Comments":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Comments.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Creator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Creator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Updator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Updator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CreatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CreatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                default:
                    switch (Def.ExtendedColumnTypes.Get(column.Name))
                    {
                        case "Class":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Class(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Num":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Num(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Date":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Date(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Description":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Description(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Check":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Check(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Attachments":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Attachments(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        default: return string.Empty;
                    }
                    break;
            }
            return "\"" + value?.Replace("\"", "\"\"") + "\"";
        }

        public List<long> SwitchTargets;

        public IssueModel()
        {
        }

        public IssueModel(
            Context context,
            SiteSettings ss,
            IDictionary<string, string> formData = null,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            SiteId = ss.SiteId;
            if (IssueId == 0) SetDefault(context: context, ss: ss);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (setByApi) SetByApi(context: context, ss: ss);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public IssueModel(
            Context context,
            SiteSettings ss,
            long issueId,
            IDictionary<string, string> formData = null,
            bool setByApi = false,
            bool clearSessions = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            IssueId = issueId;
            SiteId = ss.SiteId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    where: Rds.IssuesWhereDefault(this)
                        .Issues_Ver(context.QueryStrings.Int("ver")), ss: ss);
            }
            else
            {
                Get(context: context, ss: ss);
            }
            if (clearSessions) ClearSessions(context: context);
            if (IssueId == 0) SetDefault(context: context, ss: ss);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (setByApi) SetByApi(context: context, ss: ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public IssueModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            IDictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    ss: ss,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public IssueModel Get(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(context, ss, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    tableType: tableType,
                    column: column ?? Rds.IssuesEditorColumns(ss),
                    join: join ??  Rds.IssuesJoinDefault(),
                    where: where ?? Rds.IssuesWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public IssueApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new IssueApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "SiteId": data.SiteId = SiteId; break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "IssueId": data.IssueId = IssueId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "Title": data.Title = Title.Value; break;
                    case "Body": data.Body = Body; break;
                    case "StartTime": data.StartTime = StartTime.ToLocal(context: context); break;
                    case "CompletionTime": data.CompletionTime = CompletionTime.Value.ToLocal(context: context); break;
                    case "WorkValue": data.WorkValue = WorkValue.Value; break;
                    case "ProgressRate": data.ProgressRate = ProgressRate.Value; break;
                    case "RemainingWorkValue": data.RemainingWorkValue = RemainingWorkValue; break;
                    case "Status": data.Status = Status.Value; break;
                    case "Manager": data.Manager = Manager.Id; break;
                    case "Owner": data.Owner = Owner.Id; break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                    default: 
                        data.Value(
                            context: context,
                            columnName: column.ColumnName,
                            value: Value(
                                context: context,
                                column: column,
                                toLocal: true));
                        break;
                }
            });
            data.ItemTitle = Title.DisplayValue;
            return data;
        }

        public string FullText(
            Context context,
            SiteSettings ss,
            bool backgroundTask = false,
            bool onCreating = false)
        {
            if (!Parameters.Search.CreateIndexes && !backgroundTask) return null;
            if (AccessStatus == Databases.AccessStatuses.NotFound) return null;
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
                    case "IssueId":
                        IssueId.FullText(context, fullText);
                        break;
                    case "Title":
                        Title.FullText(context, fullText);
                        break;
                    case "Body":
                        Body.FullText(context, fullText);
                        break;
                    case "StartTime":
                        StartTime.FullText(context, fullText);
                        break;
                    case "CompletionTime":
                        CompletionTime.FullText(context, fullText);
                        break;
                    case "WorkValue":
                        WorkValue.FullText(context, fullText);
                        break;
                    case "ProgressRate":
                        ProgressRate.FullText(context, fullText);
                        break;
                    case "Status":
                        Status.FullText(context, ss.GetColumn(context: context, columnName: "Status"), fullText);
                        break;
                    case "Manager":
                        Manager.FullText(context, fullText);
                        break;
                    case "Owner":
                        Owner.FullText(context, fullText);
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
                    referenceType: "Issues",
                    referenceId: IssueId);
            }
            return fullText
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Distinct()
                .Join(" ");
        }

        public ErrorData Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool extendedSqls = true,
            bool synchronizeSummary = true,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            bool otherInitValue = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            if (extendedSqls)
            {
                statements.OnCreatingExtendedSqls(SiteId);
            }
            statements.AddRange(CreateStatements(
                context: context,
                ss: ss,
                tableType: tableType,
                param: param,
                otherInitValue: otherInitValue));
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            if (response.Event == "Duplicated")
            {
                return new ErrorData(
                    type: Error.Types.Duplicated,
                    id: IssueId,
                    columnName: response.ColumnName);
            }
            IssueId = (response.Id ?? IssueId).ToLong();
            if (synchronizeSummary)
            {
                SynchronizeSummary(
                    context: context,
                    ss: ss,
                    force: forceSynchronizeSourceSummary);
            }
            if (context.ContractSettings.Notice != false && notice)
            {
                SetTitle(
                    context: context,
                    ss: ss);
                Notice(
                    context: context,
                    ss: ss,
                    notifications: GetNotifications(
                        context: context,
                        ss: ss,
                        notice: notice),
                    type: "Created");
            }
            if (get) Get(context: context, ss: ss);
            if (ss.PermissionForCreating != null)
            {
                ss.SetPermissions(
                    context: context,
                    referenceId: IssueId);
            }
            var fullText = FullText(context, ss: ss, onCreating: true);
            statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                param: Rds.ItemsParam()
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                where: Rds.ItemsWhere().ReferenceId(IssueId)));
            statements.Add(BinaryUtilities.UpdateReferenceId(
                context: context,
                ss: ss,
                referenceId: IssueId,
                values: fullText));
            if (extendedSqls)
            {
                statements.OnCreatedExtendedSqls(
                    siteId: SiteId,
                    id: IssueId);
            }
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (get && Rds.ExtendedSqls(
                siteId: SiteId,
                id: IssueId)
                    ?.Any(o => o.OnCreated) == true)
            {
                Get(
                    context: context,
                    ss: ss);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(IfDuplicatedStatements(ss: ss));
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertItems(
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Issues")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.InsertIssues(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    param: param ?? Rds.IssuesParamDefault(
                        context: context,
                        issueModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
                InsertLinks(ss, setIdentity: true),
            });
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            statements.AddRange(PermissionUtilities.InsertStatements(
                context: context,
                ss: ss,
                users: ss.Columns
                    .Where(o => o.UserColumn)
                    .ToDictionary(o =>
                        o.ColumnName,
                        o => SiteInfo.User(
                            context: context,
                            userId: PropertyValue(
                                context: context,
                                name: o.ColumnName).ToInt()))));
            return statements;
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            bool extendedSqls = true,
            bool synchronizeSummary = true,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            var notifications = GetNotifications(
                context: context,
                ss: ss,
                notice: notice,
                before: true);
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            if (extendedSqls)
            {
                statements.OnUpdatingExtendedSqls(
                    siteId: SiteId,
                    id: IssueId,
                    timestamp: Timestamp.ToDateTime());
            }
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                permissions: permissions,
                permissionChanged: permissionChanged,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements));
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Duplicated")
            {
                return new ErrorData(
                    type: Error.Types.Duplicated,
                    id: IssueId,
                    columnName: response.ColumnName);
            }
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: IssueId);
            }
            if (synchronizeSummary)
            {
                SynchronizeSummary(
                    context: context,
                    ss: ss,
                    force: forceSynchronizeSourceSummary);
            }
            if (context.ContractSettings.Notice != false && notice)
            {
                Notice(
                    context: context,
                    ss: ss,
                    notifications: NotificationUtilities.MeetConditions(
                        ss: ss,
                        before: notifications,
                        after: GetNotifications(
                            context: context,
                            ss: ss,
                            notice: notice)),
                    type: "Updated");
            }
            if (get)
            {
                Get(context: context, ss: ss);
            }
            UpdateRelatedRecords(
                context: context,
                ss: ss,
                extendedSqls: extendedSqls,
                get: get,
                addUpdatedTimeParam: true,
                addUpdatorParam: true,
                updateItems: true);
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
            var where = Rds.IssuesWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            statements.AddRange(IfDuplicatedStatements(ss: ss));
            if (VerUp)
            {
                statements.Add(Rds.IssuesCopyToStatement(
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
                statements.UpdatePermissions(context, ss, IssueId, permissions);
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
                Rds.UpdateIssues(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.IssuesParamDefault(
                        context: context,
                        issueModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(IssueId))
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
                        referenceId: IssueId));
            return statements;
        }

        public void UpdateRelatedRecords(
            Context context,
            SiteSettings ss,
            bool extendedSqls = false,
            bool get = false,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: UpdateRelatedRecordsStatements(
                    context: context,
                    ss: ss,
                    extendedSqls: extendedSqls,
                    addUpdatedTimeParam: addUpdatedTimeParam,
                    addUpdatorParam: addUpdatorParam,
                    updateItems: updateItems)
                        .ToArray());
            if (get && Rds.ExtendedSqls(
                siteId: SiteId,
                id: IssueId)
                    ?.Any(o => o.OnUpdated) == true)
            {
                Get(
                    context: context,
                    ss: ss);
            }
            if (ss.Sources?.Any() == true)
            {
                ItemUtilities.UpdateSourceTitles(
                    context: context,
                    ss: ss,
                    idList: IssueId.ToSingleList());
            }
        }

        public List<SqlStatement> UpdateRelatedRecordsStatements(
            Context context,
            SiteSettings ss,
            bool extendedSqls = false,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            var fullText = FullText(context, ss: ss);
            var statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                where: Rds.ItemsWhere().ReferenceId(IssueId),
                param: Rds.ItemsParam()
                    .SiteId(SiteId)
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                addUpdatedTimeParam: addUpdatedTimeParam,
                addUpdatorParam: addUpdatorParam,
                _using: updateItems));
            statements.Add(Rds.PhysicalDeleteLinks(
                where: Rds.LinksWhere().SourceId(IssueId)));
            statements.Add(InsertLinks(ss));
            if (extendedSqls)
            {
                statements.OnUpdatedExtendedSqls(
                    siteId: SiteId,
                    id: IssueId);
            }
            return statements;
        }

        private SqlInsert InsertLinks(SiteSettings ss, bool setIdentity = false)
        {
            var link = new Dictionary<long, long>();
            ss.Columns.Where(o => o.Link.ToBool()).ForEach(column =>
            {
                var id = Class(column).ToLong();
                if (id != 0 && !link.ContainsKey(id))
                {
                    link.Add(id, IssueId);
                }
            });
            return LinkUtilities.Insert(link, setIdentity);
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Issues")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.UpdateOrInsertIssues(
                    where: where ?? Rds.IssuesWhereDefault(this),
                    param: param ?? Rds.IssuesParamDefault(
                        context: context, issueModel: this, setDefault: true))
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            IssueId = (response.Id ?? IssueId).ToLong();
            Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Move(Context context, SiteSettings ss, SiteSettings targetSs)
        {
            SiteId = targetSs.SiteId;
            var statements = new List<SqlStatement>();
            var fullText = FullText(
                context: context,
                ss: targetSs);
            statements.AddRange(IfDuplicatedStatements(ss: targetSs));
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateItems(
                    where: Rds.ItemsWhere().ReferenceId(IssueId),
                    param: Rds.ItemsParam()
                        .SiteId(SiteId)
                        .FullText(fullText, _using: fullText != null)),
                Rds.UpdateIssues(
                    where: Rds.IssuesWhere().IssueId(IssueId),
                    param: Rds.IssuesParam().SiteId(SiteId))
            });
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Duplicated")
            {
                return new ErrorData(
                    type: Error.Types.Duplicated,
                    id: IssueId,
                    columnName: response.ColumnName);
            }
            SynchronizeSummary(
                context: context,
                ss: ss);
            Get(
                context: context,
                ss: targetSs);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var notifications = GetNotifications(
                context: context,
                ss: ss,
                notice: notice,
                before: true);
            var statements = new List<SqlStatement>();
            var where = Rds.IssuesWhere().SiteId(SiteId).IssueId(IssueId);
            statements.OnDeletingExtendedSqls(SiteId, IssueId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteItems(
                    where: Rds.ItemsWhere().ReferenceId(IssueId)),
                Rds.DeleteBinaries(
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId(IssueId)),
                Rds.DeleteIssues(where: where)
            });
            statements.OnDeletedExtendedSqls(SiteId, IssueId);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            SynchronizeSummary(context, ss);
            if (context.ContractSettings.Notice != false && notice)
            {
                Notice(
                    context: context,
                    ss: ss,
                    notifications: GetNotifications(
                        context: context,
                        ss: ss,
                        notice: notice,
                        tableTypes: Sqls.TableTypes.Deleted),
                    type: "Deleted");
            }
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,long issueId)
        {
            IssueId = issueId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(IssueId)),
                    Rds.RestoreIssues(
                        where: Rds.IssuesWhere().IssueId(IssueId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteIssues(
                    tableType: tableType,
                    param: Rds.IssuesParam().SiteId(SiteId).IssueId(IssueId)));
            return new ErrorData(type: Error.Types.None);
        }

        private List<SqlStatement> IfDuplicatedStatements(SiteSettings ss)
        {
            var statements = new List<SqlStatement>();
            var param = new Rds.IssuesParamCollection();
            ss.Columns
                .Where(column => column.NoDuplication == true)
                .ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "Title":
                            if (Title.Value != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Title(Title.Value.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "Body":
                            if (Body != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Body(Body), SiteId, IssueId));
                            break;
                        case "StartTime":
                            if (StartTime != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.StartTime(StartTime), SiteId, IssueId));
                            break;
                        case "CompletionTime":
                            if (CompletionTime.Value != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.CompletionTime(CompletionTime.Value), SiteId, IssueId));
                            break;
                        case "WorkValue":
                            if (WorkValue.Value != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.WorkValue(WorkValue.Value), SiteId, IssueId));
                            break;
                        case "ProgressRate":
                            if (ProgressRate.Value != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ProgressRate(ProgressRate.Value), SiteId, IssueId));
                            break;
                        case "Status":
                            if (Status.Value != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Status(Status.Value), SiteId, IssueId));
                            break;
                        case "Manager":
                            if (Manager.Id != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Manager(Manager.Id), SiteId, IssueId));
                            break;
                        case "Owner":
                            if (Owner.Id != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Owner(Owner.Id), SiteId, IssueId));
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.ColumnName))
                            {
                                case "Class":
                                    if (!Class(column: column).IsNullOrEmpty())
                                        statements.Add(column.IfDuplicatedStatement(
                                            param: param.Add(
                                                columnBracket: $"[{column.ColumnName}]",
                                                name: column.ColumnName,
                                                value: Class(column: column).MaxLength(1024)),
                                            siteId: SiteId,
                                            referenceId: IssueId));
                                    break;
                                case "Num":
                                    if (Num(column: column) != 0)
                                        statements.Add(column.IfDuplicatedStatement(
                                            param: param.Add(
                                                columnBracket: $"[{column.ColumnName}]",
                                                name: column.ColumnName,
                                                value: Num(column: column)),
                                            siteId: SiteId,
                                            referenceId: IssueId));
                                    break;
                                case "Date":
                                    if (Date(column: column) != 0.ToDateTime())
                                        statements.Add(column.IfDuplicatedStatement(
                                            param: param.Add(
                                                columnBracket: $"[{column.ColumnName}]",
                                                name: column.ColumnName,
                                                value: Date(column: column)),
                                            siteId: SiteId,
                                            referenceId: IssueId));
                                    break;
                                case "Description":
                                    if (!Description(column: column).IsNullOrEmpty())
                                        statements.Add(column.IfDuplicatedStatement(
                                            param: param.Add(
                                                columnBracket: $"[{column.ColumnName}]",
                                                name: column.ColumnName,
                                                value: Description(column: column)),
                                            siteId: SiteId,
                                            referenceId: IssueId));
                                    break;
                            }
                            break;
                    }
                });
            return statements;
        }

        public void SetDefault(Context context, SiteSettings ss)
        {
            ss.Columns
                .Where(o => !o.DefaultInput.IsNullOrEmpty())
                .ForEach(column => SetDefault(context: context, ss: ss, column: column));
        }

        public void SetCopyDefault(Context context, SiteSettings ss)
        {
            ss.Columns
                .Where(column => column.CopyByDefault == true
                    || column.TypeCs == "Attachments")
                .ForEach(column => SetDefault(
                    context: context,
                    ss: ss,
                    column: column));
        }

        public void SetDefault(Context context, SiteSettings ss, Column column)
        {
            switch (column.ColumnName)
            {
                case "IssueId":
                    IssueId = column.GetDefaultInput(context: context).ToLong();
                    break;
                case "Title":
                    Title.Value = column.GetDefaultInput(context: context).ToString();
                    break;
                case "Body":
                    Body = column.GetDefaultInput(context: context).ToString();
                    break;
                case "WorkValue":
                    WorkValue.Value = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "ProgressRate":
                    ProgressRate.Value = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "Status":
                    Status.Value = column.GetDefaultInput(context: context).ToInt();
                    break;
                case "Timestamp":
                    Timestamp = column.GetDefaultInput(context: context).ToString();
                    break;
                case "Manager":
                    Manager = SiteInfo.User(
                        context: context,
                        userId: column.GetDefaultInput(context: context).ToInt());
                    break;
                case "Owner":
                    Owner = SiteInfo.User(
                        context: context,
                        userId: column.GetDefaultInput(context: context).ToInt());
                    break;
                case "StartTime":
                    StartTime = column.DefaultTime();
                    break;
                case "CompletionTime":
                    CompletionTime = new CompletionTime(
                        context: context,
                        ss: ss,
                        value: column.DefaultTime(),
                        status: Status);
                    break;
                default:
                    switch (Def.ExtendedColumnTypes.Get(column.ColumnName))
                    {
                        case "Class":
                            Class(
                                column: column,
                                value: column.GetDefaultInput(context: context).ToString());
                            break;
                        case "Num":
                            Num(
                                column: column,
                                value: column.GetDefaultInput(context: context).ToDecimal());
                            break;
                        case "Date":
                            Date(
                                column: column,
                                value: column.DefaultTime());
                            break;
                        case "Description":
                            Description(
                                column: column,
                                value: column.GetDefaultInput(context: context).ToString());
                            break;
                        case "Check":
                            Check(
                                column: column,
                                value: column.GetDefaultInput(context: context).ToBool());
                            break;
                        case "Attachments":
                            Attachments(
                                column: column,
                                value: new Attachments());
                            break;
                    }
                    break;
            }
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            IDictionary<string, string> formData)
        {
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Issues_Title": Title = new Title(IssueId, value); break;
                    case "Issues_Body": Body = value.ToString(); break;
                    case "Issues_StartTime": StartTime = value.ToDateTime().ToUniversal(context: context); ProgressRate.StartTime = StartTime; break;
                    case "Issues_CompletionTime": CompletionTime = new CompletionTime(context: context, ss: ss, value: value.ToDateTime(), status: Status, byForm: true); ProgressRate.CompletionTime = CompletionTime.Value; break;
                    case "Issues_WorkValue": WorkValue = new WorkValue(ss.GetColumn(context: context, columnName: "WorkValue").Round(value.ToDecimal(context.CultureInfo())), ProgressRate.Value); break;
                    case "Issues_ProgressRate": ProgressRate = new ProgressRate(CreatedTime, StartTime, CompletionTime, ss.GetColumn(context: context, columnName: "ProgressRate").Round(value.ToDecimal(context.CultureInfo()))); WorkValue.ProgressRate = ProgressRate.Value; break;
                    case "Issues_Status": Status = new Status(value.ToInt()); CompletionTime.Status = Status; break;
                    case "Issues_Manager": Manager = SiteInfo.User(context: context, userId: value.ToInt()); break;
                    case "Issues_Owner": Owner = SiteInfo.User(context: context, userId: value.ToInt()); break;
                    case "Issues_Timestamp": Timestamp = value.ToString(); break;
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
            var fromSiteId = context.Forms.Long("FromSiteId");
            if (fromSiteId > 0)
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: ss.Links.FirstOrDefault(o => o.SiteId == fromSiteId).ColumnName);
                if (PropertyValue(context: context, name: column?.ColumnName) == context.Forms.Data("LinkId"))
                {
                    column.Linking = true;
                }
            }
            SetByFormula(context: context, ss: ss);
            SetChoiceHash(context: context, ss: ss);
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = formData.Get("ControlId")?
                    .Split(',')
                    ._2nd()
                    .ToInt() ?? 0;
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        public void SetByModel(IssueModel issueModel)
        {
            SiteId = issueModel.SiteId;
            UpdatedTime = issueModel.UpdatedTime;
            Title = issueModel.Title;
            Body = issueModel.Body;
            StartTime = issueModel.StartTime;
            CompletionTime = issueModel.CompletionTime;
            WorkValue = issueModel.WorkValue;
            ProgressRate = issueModel.ProgressRate;
            RemainingWorkValue = issueModel.RemainingWorkValue;
            Status = issueModel.Status;
            Manager = issueModel.Manager;
            Owner = issueModel.Owner;
            Comments = issueModel.Comments;
            Creator = issueModel.Creator;
            Updator = issueModel.Updator;
            CreatedTime = issueModel.CreatedTime;
            VerUp = issueModel.VerUp;
            Comments = issueModel.Comments;
            ClassHash = issueModel.ClassHash;
            NumHash = issueModel.NumHash;
            DateHash = issueModel.DateHash;
            DescriptionHash = issueModel.DescriptionHash;
            CheckHash = issueModel.CheckHash;
            AttachmentsHash = issueModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.RequestDataString.Deserialize<IssueApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.Title != null) Title = new Title(data.IssueId.ToLong(), data.Title);
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.StartTime != null) StartTime = data.StartTime.ToDateTime().ToUniversal(context: context); ProgressRate.StartTime = StartTime;
            if (data.CompletionTime != null) CompletionTime = new CompletionTime(context: context, ss: ss, value: data.CompletionTime.ToDateTime(), status: Status, byForm: true); ProgressRate.CompletionTime = CompletionTime.Value;
            if (data.WorkValue != null) WorkValue = new WorkValue(ss.GetColumn(context: context, columnName: "WorkValue").Round(data.WorkValue.ToDecimal()), ProgressRate.Value);
            if (data.ProgressRate != null) ProgressRate = new ProgressRate(CreatedTime, StartTime, CompletionTime, ss.GetColumn(context: context, columnName: "ProgressRate").Round(data.ProgressRate.ToDecimal())); WorkValue.ProgressRate = ProgressRate.Value;
            if (data.Status != null) Status = new Status(data.Status.ToInt()); CompletionTime.Status = Status;
            if (data.Manager != null) Manager = SiteInfo.User(context: context, userId: data.Manager.ToInt());
            if (data.Owner != null) Owner = SiteInfo.User(context: context, userId: data.Owner.ToInt());
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash.ForEach(o => Class(
                columnName: o.Key,
                value: o.Value));
            data.NumHash.ForEach(o => Num(
                columnName: o.Key,
                value: o.Value));
            data.DateHash.ForEach(o => Date(
                columnName: o.Key,
                value: o.Value.ToUniversal(context: context)));
            data.DescriptionHash.ForEach(o => Description(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash.ForEach(o => Check(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash.ForEach(o => Attachments(
                columnName: o.Key,
                value: o.Value));
            SetByFormula(context: context, ss: ss);
            SetChoiceHash(context: context, ss: ss);
        }

        public void SynchronizeSummary(Context context, SiteSettings ss, bool force = false)
        {
            ss.Summaries.ForEach(summary =>
            {
                var id = SynchronizeSummaryDestinationId(linkColumn: summary.LinkColumn);
                var savedId = SynchronizeSummaryDestinationId(
                    linkColumn: summary.LinkColumn,
                    saved: true);
                if (id != 0)
                {
                    SynchronizeSummary(
                        context: context,
                        ss: ss,
                        summary: summary,
                        id: id);
                }
                if (savedId != 0 && id != savedId)
                {
                    SynchronizeSummary(
                        context: context,
                        ss: ss,
                        summary: summary,
                        id: savedId);
                }
            });
            SynchronizeSourceSummary(
                context: context,
                ss: ss,
                force: force);
        }

        private void SynchronizeSummary(
            Context context, SiteSettings ss, Summary summary, long id)
        {
            var destinationSs = SiteSettingsUtilities.Get(
                context: context, siteId: summary.SiteId);
            if (destinationSs != null)
            {
                Summaries.Synchronize(
                    context: context,
                    ss: ss,
                    destinationSs: destinationSs,
                    destinationSiteId: summary.SiteId,
                    destinationReferenceType: summary.DestinationReferenceType,
                    destinationColumn: summary.DestinationColumn,
                    destinationCondition: destinationSs.Views?.Get(summary.DestinationCondition),
                    setZeroWhenOutOfCondition: summary.SetZeroWhenOutOfCondition == true,
                    sourceSiteId: SiteId,
                    sourceReferenceType: "Issues",
                    linkColumn: summary.LinkColumn,
                    type: summary.Type,
                    sourceColumn: summary.SourceColumn,
                    sourceCondition: ss.Views?.Get(summary.SourceCondition),
                    id: id);
            }
        }

        private void SynchronizeSourceSummary(
            Context context, SiteSettings ss, bool force = false)
        {
            ss.Sources.Values.ForEach(sourceSs =>
                sourceSs.Summaries
                    .Where(o => ss.Views?.Get(o.DestinationCondition) != null || force)
                    .ForEach(summary =>
                        Summaries.Synchronize(
                            context: context,
                            ss: sourceSs,
                            destinationSs: ss,
                            destinationSiteId: summary.SiteId,
                            destinationReferenceType: summary.DestinationReferenceType,
                            destinationColumn: summary.DestinationColumn,
                            destinationCondition: ss.Views?.Get(summary.DestinationCondition),
                            setZeroWhenOutOfCondition: summary.SetZeroWhenOutOfCondition == true,
                            sourceSiteId: sourceSs.SiteId,
                            sourceReferenceType: sourceSs.ReferenceType,
                            linkColumn: summary.LinkColumn,
                            type: summary.Type,
                            sourceColumn: summary.SourceColumn,
                            sourceCondition: sourceSs.Views?.Get(summary.SourceCondition),
                            id: IssueId)));
        }

        private long SynchronizeSummaryDestinationId(string linkColumn, bool saved = false)
        {
            return saved
                ? SavedClass(linkColumn).ToLong()
                : Class(linkColumn).ToLong();
        }

        public void UpdateFormulaColumns(
            Context context, SiteSettings ss, IEnumerable<int> selected = null)
        {
            SetByFormula(context: context, ss: ss);
            var param = Rds.IssuesParam();
            ss.Formulas?
                .Where(o => selected == null || selected.Contains(o.Id))
                .ForEach(formulaSet =>
                {
                    switch (formulaSet.Target)
                    {
                        case "WorkValue": param.WorkValue(WorkValue.Value); break;
                        case "ProgressRate": param.ProgressRate(ProgressRate.Value); break;
                        default:
                            if (Def.ExtendedColumnTypes.ContainsKey(formulaSet.Target))
                            {
                                param.Add(
                                    columnBracket: $"[{formulaSet.Target}]",
                                    name: formulaSet.Target,
                                    value: Num(formulaSet.Target));
                            }
                            break;
                    }
                });
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateIssues(
                    param: param,
                    where: Rds.IssuesWhereDefault(this),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        public void SetByFormula(Context context, SiteSettings ss)
        {
            ss.Formulas?.ForEach(formulaSet =>
            {
                var columnName = formulaSet.Target;
                var formula = formulaSet.Formula;
                var view = ss.Views?.Get(formulaSet.Condition);
                if (view != null && !Matched(context: context, ss: ss, view: view))
                {
                    if (formulaSet.OutOfCondition != null)
                    {
                        formula = formulaSet.OutOfCondition;
                    }
                    else
                    {
                        return;
                    }
                }
                var data = new Dictionary<string, decimal>
                {
                    { "WorkValue", WorkValue.Value },
                    { "ProgressRate", ProgressRate.Value },
                    { "RemainingWorkValue", RemainingWorkValue }
                };
                data.AddRange(NumHash);
                var value = formula?.GetResult(
                    data: data,
                    column: ss.GetColumn(
                        context: context,
                        columnName: columnName)) ?? 0;
                switch (columnName)
                {
                    case "WorkValue":
                        WorkValue.Value = value;
                        break;
                    case "ProgressRate":
                        ProgressRate.Value = value;
                        break;
                    default:
                        Num(
                            columnName: columnName,
                            value: value);
                        break;
                }
            });
        }

        public void SetTitle(Context context, SiteSettings ss)
        {
            Title = new Title(
                context: context,
                ss: ss,
                id: IssueId,
                ver: Ver,
                isHistory: VerType == Versions.VerTypes.History,
                data: PropertyValues(
                    context: context,
                    names: ss.TitleColumns));
        }

        private bool Matched(Context context, SiteSettings ss, View view)
        {
            if (view.Incomplete == true && !Status.Incomplete())
            {
                return false;
            }
            var userId = context.UserId;
            if (view.Own == true && !(Manager.Id == userId || Owner.Id == userId))
            {
                return false;
            }
            if (view.NearCompletionTime == true && !CompletionTime.Near(
                context: context,
                ss: ss))
            {
                return false;
            }
            if (view.Delay == true && !ProgressRate.Delay(
                context: context,
                status: Status))
            {
                return false;
            }
            if (view.Overdue == true && CompletionTime.Overdue())
            {
                return false;
            }
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
                        case "Title":
                            match = Title.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Body":
                            match = Body.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "StartTime":
                            match = StartTime.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "CompletionTime":
                            match = CompletionTime.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "WorkValue":
                            match = WorkValue.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "ProgressRate":
                            match = ProgressRate.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Status":
                            match = Status.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Manager":
                            match = Manager.Id.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Owner":
                            match = Owner.Id.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "SiteTitle":
                            match = SiteTitle.SiteId.Matched(
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

        public List<Notification> GetNotifications(
            Context context,
            SiteSettings ss,
            bool notice,
            bool before = false,
            Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            if (context.ContractSettings.Notice == false || !notice)
            {
                return null;
            }
            var notifications = NotificationUtilities.Get(
                context: context,
                ss: ss);
            if (notifications?.Any() == true)
            {
                var dataSet = Rds.ExecuteDataSet(
                    context: context,
                    statements: notifications.Select(notification =>
                        Rds.SelectIssues(
                            dataTableName: notification.Index.ToString(),
                            tableType: tableTypes,
                            column: Rds.IssuesColumn().IssueId(),
                            where: ss.Views?.Get(before
                                ? notification.BeforeCondition
                                : notification.AfterCondition)
                                    ?.Where(
                                        context: context,
                                        ss: ss,
                                        where: Rds.IssuesWhere().IssueId(IssueId))
                                            ?? Rds.IssuesWhere().IssueId(IssueId)))
                                                .ToArray());
                return notifications
                    .Where(notification =>
                        dataSet.Tables[notification.Index.ToString()].Rows.Count == 1 )
                    .ToList();
            }
            else
            {
                return null;
            }
        }

        public void Notice(
            Context context,
            SiteSettings ss,
            List<Notification> notifications,
            string type)
        {
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: IssueId);
            notifications?.ForEach(notification =>
            {
                if (notification.HasRelatedUsers())
                {
                    var users = new List<long>();
                    Rds.ExecuteTable(
                        context: context,
                        statements: Rds.SelectIssues(
                            tableType: Sqls.TableTypes.All,
                            distinct: true,
                            column: Rds.IssuesColumn()
                                .Manager()
                                .Owner()
                                .Creator()
                                .Updator(),
                            where: Rds.IssuesWhere().IssueId(IssueId)))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                {
                                    users.Add(dataRow.Long("Manager"));
                                    users.Add(dataRow.Long("Owner"));
                                    users.Add(dataRow.Long("Creator"));
                                    users.Add(dataRow.Long("Updator"));
                                });
                    notification.ReplaceRelatedUsers(
                        context: context,
                        users: users);
                }
                switch (type)
                {
                    case "Created":
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Created(
                                context: context,
                                data: Title.DisplayValue).ToString(),
                            url: url,
                            body: NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification));
                        break;
                    case "Updated":
                        var body = NoticeBody(
                            context: context,
                            ss: ss,
                            notification: notification, update: true);
                        if (body.Length > 0)
                        {
                            notification.Send(
                                context: context,
                                ss: ss,
                                title: Displays.Updated(
                                    context: context,
                                    data: Title.DisplayValue).ToString(),
                                url: url,
                                body: body);
                        }
                        break;
                    case "Deleted":
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Deleted(
                                context: context,
                                data: Title.DisplayValue).ToString(),
                            url: url,
                            body: NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification));
                        break;
                }
            });
        }

        private string NoticeBody(
            Context context, SiteSettings ss, Notification notification, bool update = false)
        {
            var body = new System.Text.StringBuilder();
            notification.ColumnCollection(context, ss, update)?.ForEach(column =>
            {
                switch (column.Name)
                {
                    case "Title":
                        body.Append(Title.ToNotice(
                            context: context,
                            saved: SavedTitle,
                            column: column,
                            updated: Title_Updated(context: context),
                            update: update));
                        break;
                    case "Body":
                        body.Append(Body.ToNotice(
                            context: context,
                            saved: SavedBody,
                            column: column,
                            updated: Body_Updated(context: context),
                            update: update));
                        break;
                    case "StartTime":
                        body.Append(StartTime.ToNotice(
                            context: context,
                            saved: SavedStartTime,
                            column: column,
                            updated: StartTime_Updated(context: context),
                            update: update));
                        break;
                    case "CompletionTime":
                        body.Append(CompletionTime.ToNotice(
                            context: context,
                            saved: SavedCompletionTime,
                            column: column,
                            updated: CompletionTime_Updated(context: context),
                            update: update));
                        break;
                    case "WorkValue":
                        body.Append(WorkValue.ToNotice(
                            context: context,
                            saved: SavedWorkValue,
                            column: column,
                            updated: WorkValue_Updated(context: context),
                            update: update));
                        break;
                    case "ProgressRate":
                        body.Append(ProgressRate.ToNotice(
                            context: context,
                            saved: SavedProgressRate,
                            column: column,
                            updated: ProgressRate_Updated(context: context),
                            update: update));
                        break;
                    case "Status":
                        body.Append(Status.ToNotice(
                            context: context,
                            saved: SavedStatus,
                            column: column,
                            updated: Status_Updated(context: context),
                            update: update));
                        break;
                    case "Manager":
                        body.Append(Manager.ToNotice(
                            context: context,
                            saved: SavedManager,
                            column: column,
                            updated: Manager_Updated(context: context),
                            update: update));
                        break;
                    case "Owner":
                        body.Append(Owner.ToNotice(
                            context: context,
                            saved: SavedOwner,
                            column: column,
                            updated: Owner_Updated(context: context),
                            update: update));
                        break;
                    case "Comments":
                        body.Append(Comments.ToNotice(
                            context: context,
                            saved: SavedComments,
                            column: column,
                            updated: Comments_Updated(context: context),
                            update: update));
                        break;
                    case "Creator":
                        body.Append(Creator.ToNotice(
                            context: context,
                            saved: SavedCreator,
                            column: column,
                            updated: Creator_Updated(context: context),
                            update: update));
                        break;
                    case "Updator":
                        body.Append(Updator.ToNotice(
                            context: context,
                            saved: SavedUpdator,
                            column: column,
                            updated: Updator_Updated(context: context),
                            update: update));
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                body.Append(Class(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedClass(columnName: column.Name),
                                    column: column,
                                    updated: Class_Updated(columnName: column.Name),
                                    update: update));
                                break;
                            case "Num":
                                body.Append(Num(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedNum(columnName: column.Name),
                                    column: column,
                                    updated: Num_Updated(columnName: column.Name),
                                    update: update));
                                break;
                            case "Date":
                                body.Append(Date(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedDate(columnName: column.Name),
                                    column: column,
                                    updated: Date_Updated(columnName: column.Name),
                                    update: update));
                                break;
                            case "Description":
                                body.Append(Description(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedDescription(columnName: column.Name),
                                    column: column,
                                    updated: Description_Updated(columnName: column.Name),
                                    update: update));
                                break;
                            case "Check":
                                body.Append(Check(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedCheck(columnName: column.Name),
                                    column: column,
                                    updated: Check_Updated(columnName: column.Name),
                                    update: update));
                                break;
                        }
                        break;
                }
            });
            return body.ToString();
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
            SetChoiceHash(context: context, ss: ss);
        }

        public void SetChoiceHash(Context context, SiteSettings ss)
        {
            if (!ss.SetAllChoices)
            {
                ss.GetUseSearchLinks(context: context).ForEach(link =>
                {
                    var value = PropertyValue(
                        context: context,
                        name: link.ColumnName);
                    var column = ss.GetColumn(
                        context: context,
                        columnName: link.ColumnName);
                    if (!value.IsNullOrEmpty() 
                        && column?.ChoiceHash.Any(o => o.Value.Value == value) != true)
                    {
                        ss.SetChoiceHash(
                            context: context,
                            columnName: column.ColumnName,
                            selectedValues: value.ToSingleList());
                    }
                });
            }
            SetTitle(context: context, ss: ss);
        }

        private void Set(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
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
                        case "IssueId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                IssueId = dataRow[column.ColumnName].ToLong();
                                SavedIssueId = IssueId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Title":
                            Title = new Title(context: context, ss: ss, dataRow: dataRow, column: column);
                            SavedTitle = Title.Value;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "StartTime":
                            StartTime = dataRow[column.ColumnName].ToDateTime();
                            SavedStartTime = StartTime;
                            break;
                        case "CompletionTime":
                            CompletionTime = new CompletionTime(context: context, ss: ss, dataRow: dataRow, column: column);
                            SavedCompletionTime = CompletionTime.Value;
                            break;
                        case "WorkValue":
                            WorkValue = new WorkValue(dataRow, column);
                            SavedWorkValue = WorkValue.Value;
                            break;
                        case "ProgressRate":
                            ProgressRate = new ProgressRate(dataRow, column);
                            SavedProgressRate = ProgressRate.Value;
                            break;
                        case "RemainingWorkValue":
                            RemainingWorkValue = dataRow[column.ColumnName].ToDecimal();
                            SavedRemainingWorkValue = RemainingWorkValue;
                            break;
                        case "Status":
                            Status = new Status(dataRow, column);
                            SavedStatus = Status.Value;
                            break;
                        case "Manager":
                            Manager = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedManager = Manager.Id;
                            break;
                        case "Owner":
                            Owner = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedOwner = Owner.Id;
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
            SetTitle(context: context, ss: ss);
        }

        public bool Updated(Context context)
        {
            return Updated()
                || SiteId_Updated(context: context)
                || Ver_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || StartTime_Updated(context: context)
                || CompletionTime_Updated(context: context)
                || WorkValue_Updated(context: context)
                || ProgressRate_Updated(context: context)
                || Status_Updated(context: context)
                || Manager_Updated(context: context)
                || Owner_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public List<string> Mine(Context context)
        {
            var mine = new List<string>();
            var userId = context.UserId;
            if (SavedManager == userId) mine.Add("Manager");
            if (SavedOwner == userId) mine.Add("Owner");
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }

        public string IdSuffix()
        {
            return $"_{SiteId}_{(IssueId == 0 ? -1 : IssueId)}";
        }
    }
}
