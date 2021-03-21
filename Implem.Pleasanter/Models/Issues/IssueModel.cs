﻿using Implem.DefinitionAccessor;
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
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
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
        public Num RemainingWorkValue = new Num();
        public Status Status = new Status();
        public User Manager = new User();
        public User Owner = new User();
        public bool Locked = false;

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
        public decimal? SavedWorkValue = 0;
        public decimal? SavedProgressRate = 0;
        public decimal? SavedRemainingWorkValue = 0;
        public int SavedStatus = 0;
        public int SavedManager = 0;
        public int SavedOwner = 0;
        public bool SavedLocked = false;

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

        public bool Locked_Updated(Context context, Column column = null)
        {
            return Locked != SavedLocked &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != Locked);
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

        public string PropertyValue(Context context, Column column)
        {
            switch (column?.ColumnName)
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
                case "RemainingWorkValue": return RemainingWorkValue.Value.ToString();
                case "Status": return Status.Value.ToString();
                case "Manager": return Manager.Id.ToString();
                case "Owner": return Owner.Id.ToString();
                case "Locked": return Locked.ToString();
                case "SiteTitle": return SiteTitle.SiteId.ToString();
                case "Comments": return Comments.ToJson();
                case "Creator": return Creator.Id.ToString();
                case "Updator": return Updator.Id.ToString();
                case "CreatedTime": return CreatedTime.Value.ToString();
                case "VerUp": return VerUp.ToString();
                case "Timestamp": return Timestamp;
                default: return Value(
                    context: context,
                    column: column);
            }
        }

        public Dictionary<string, string> PropertyValues(Context context, List<Column> columns)
        {
            var hash = new Dictionary<string, string>();
            columns?
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.ColumnName)
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
                            hash.Add("RemainingWorkValue", RemainingWorkValue.Value.ToString());
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
                        case "Locked":
                            hash.Add("Locked", Locked.ToString());
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
                            hash.Add(column.ColumnName, Value(
                                context: context,
                                column: column));
                            break;
                    }
                });
            return hash;
        }

        public bool PropertyUpdated(Context context, string name)
        {
            switch (name)
            {
                case "SiteId": return SiteId_Updated(context: context);
                case "Ver": return Ver_Updated(context: context);
                case "Title": return Title_Updated(context: context);
                case "Body": return Body_Updated(context: context);
                case "StartTime": return StartTime_Updated(context: context);
                case "CompletionTime": return CompletionTime_Updated(context: context);
                case "WorkValue": return WorkValue_Updated(context: context);
                case "ProgressRate": return ProgressRate_Updated(context: context);
                case "Status": return Status_Updated(context: context);
                case "Manager": return Manager_Updated(context: context);
                case "Owner": return Owner_Updated(context: context);
                case "Locked": return Locked_Updated(context: context);
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
                        mine: mine)
                            ? Owner.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Locked":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Locked.ToExport(
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
            Dictionary<string, string> formData = null,
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
            Dictionary<string, string> formData = null,
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
                    where: Rds.IssuesWhereDefault(
                        context: context,
                        issueModel: this)
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
            if (SavedLocked)
            {
                ss.SetLockedRecord(
                    context: context,
                    time: UpdatedTime,
                    user: Updator);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public IssueModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
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
            where = where ?? Rds.IssuesWhereDefault(
                context: context,
                issueModel: this);
            new View().SetColumnsWhere(
                context: context,
                ss: ss,
                where: where);
            Set(context, ss, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    tableType: tableType,
                    column: column ?? Rds.IssuesEditorColumns(ss),
                    join: join ??  Rds.IssuesJoinDefault(),
                    where: where,
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
            ss.ReadableColumns(context: context, noJoined: true).ForEach(column =>
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
                    case "WorkValue": data.WorkValue = WorkValue.Value; break;
                    case "ProgressRate": data.ProgressRate = ProgressRate.Value; break;
                    case "RemainingWorkValue": data.RemainingWorkValue = RemainingWorkValue.Value; break;
                    case "Status": data.Status = Status.Value; break;
                    case "Manager": data.Manager = Manager.Id; break;
                    case "Owner": data.Owner = Owner.Id; break;
                    case "Locked": data.Locked = Locked; break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "CompletionTime": data.CompletionTime = CompletionTime.DisplayValue.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                    default: 
                        data.Value(
                            context: context,
                            column: column,
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
            var fullText = new System.Text.StringBuilder();
            SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu.Breadcrumb(
                    context: context,
                    siteId: SiteId)
                .FullText(
                    context: context,
                    fullText: fullText);
            SiteId.FullText(
                context: context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "SiteId"),
                fullText: fullText);
            ss.GetEditorColumnNames(
                context: context,
                columnOnly: true)
                    .Select(columnName => ss.GetColumn(
                        context: context,
                        columnName: columnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "IssueId":
                                IssueId.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Title":
                                Title.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Body":
                                Body.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "StartTime":
                                StartTime.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "CompletionTime":
                                CompletionTime.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "WorkValue":
                                WorkValue.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "ProgressRate":
                                ProgressRate.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Status":
                                Status.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Manager":
                                Manager.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Owner":
                                Owner.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Comments":
                                Comments.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            default:
                                BaseFullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                        }
                    });
            Creator.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "Creator"),
                fullText);
            Updator.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "Updator"),
                fullText);
            CreatedTime.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "CreatedTime"),
                fullText);
            UpdatedTime.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "UpdatedTime"),
                fullText);
            if (!onCreating)
            {
                FullTextExtensions.OutgoingMailsFullText(
                    context: context,
                    fullText: fullText,
                    referenceType: "Issues",
                    referenceId: IssueId);
            }
            return fullText
                .ToString()
                .Replace("　", " ")
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Split(' ')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
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
            SetByBeforeCreateServerScript(
                context: context,
                ss: ss);
            var statements = new List<SqlStatement>();
            if (extendedSqls)
            {
                statements.OnCreatingExtendedSqls(
                    context: context,
                    siteId: SiteId);
            }
            statements.AddRange(CreateStatements(
                context: context,
                ss: ss,
                tableType: tableType,
                param: param,
                otherInitValue: otherInitValue));
            var response = Repository.ExecuteScalar_response(
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
                    context: context,
                    siteId: SiteId,
                    id: IssueId);
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (get && Rds.ExtendedSqls(
                context: context,
                siteId: SiteId,
                id: IssueId)
                    ?.Any(o => o.OnCreated) == true)
            {
                Get(
                    context: context,
                    ss: ss);
            }
            SetByAfterCreateServerScript(
                context: context,
                ss: ss);
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
                InsertLinks(
                    context: context,
                    ss: ss,
                    setIdentity: true),
            });
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            statements.AddRange(PermissionUtilities.InsertStatements(
                context: context,
                ss: ss,
                users: ss.Columns
                    .Where(o => o.Type == Column.Types.User)
                    .ToDictionary(o =>
                        o.ColumnName,
                        o => SiteInfo.User(
                            context: context,
                            userId: PropertyValue(
                                context: context,
                                column: o).ToInt()))));
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
            string previousTitle = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            SetByBeforeUpdateServerScript(
                context: context,
                ss: ss);
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
                    context: context,
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
            var response = Repository.ExecuteScalar_response(
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
                previousTitle: previousTitle,
                get: get,
                addUpdatedTimeParam: true,
                addUpdatorParam: true,
                updateItems: true);
            SetByAfterUpdateServerScript(
                context: context,
                ss: ss);
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
            var where = Rds.IssuesWhereDefault(
                context: context,
                issueModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange());
            statements.AddRange(IfDuplicatedStatements(ss: ss));
            if (Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp))
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
                new SqlStatement(Def.Sql.IfConflicted.Params(IssueId)) {
                    IfConflicted = true,
                    Id = IssueId
                }
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
            string previousTitle = null,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            Repository.ExecuteNonQuery(
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
            var titleUpdated = Title_Updated(context: context);
            if (get && Rds.ExtendedSqls(
                context: context,
                siteId: SiteId,
                id: IssueId)
                    ?.Any(o => o.OnUpdated) == true)
            {
                Get(
                    context: context,
                    ss: ss);
            }
            if (previousTitle != null
                && previousTitle != Title.DisplayValue
                && ss.Sources?.Any() == true)
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
            statements.Add(InsertLinks(
                context: context,
                ss: ss));
            if (extendedSqls)
            {
                statements.OnUpdatedExtendedSqls(
                    context: context,
                    siteId: SiteId,
                    id: IssueId);
            }
            return statements;
        }

        private SqlInsert InsertLinks(Context context, SiteSettings ss, bool setIdentity = false)
        {
            var link = ss.Links
                .Where(o => ss.Destinations.ContainsKey(o.SiteId))
                .Select(o => ss.GetColumn(
                    context: context,
                    columnName: o.ColumnName))
                .Where(o => o != null)
                .SelectMany(column => column.MultipleSelections == true
                    ? Class(column).Deserialize<List<long>>()
                        ?? new List<long>()
                    : Class(column).ToLong().ToSingleList())
                .Where(id => id > 0)
                .Distinct()
                .ToDictionary(id => id, id => IssueId);
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
                    where: where ?? Rds.IssuesWhereDefault(
                        context: context,
                        issueModel: this),
                    param: param ?? Rds.IssuesParamDefault(
                        context: context, issueModel: this, setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
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
            var response = Repository.ExecuteScalar_response(
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
            SetByBeforeDeleteServerScript(
                context: context,
                ss: ss);
            var notifications = context.ContractSettings.Notice != false && notice
                ? GetNotifications(
                    context: context,
                    ss: ss,
                    notice: notice)
                : null;
            var statements = new List<SqlStatement>();
            var where = Rds.IssuesWhere().SiteId(SiteId).IssueId(IssueId);
            statements.OnDeletingExtendedSqls(
                context: context,
                siteId: SiteId,
                id: IssueId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteItems(
                    factory: context,
                    where: Rds.ItemsWhere().ReferenceId(IssueId)),
                Rds.DeleteBinaries(
                    factory: context,
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId(IssueId)),
                Rds.DeleteIssues(
                    factory: context,
                    where: where)
            });
            statements.OnDeletedExtendedSqls(
                context: context,
                siteId: SiteId,
                id: IssueId);
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            SynchronizeSummary(context, ss);
            if (context.ContractSettings.Notice != false && notice)
            {
                Notice(
                    context: context,
                    ss: ss,
                    notifications: notifications,
                    type: "Deleted");
            }
            SetByAfterDeleteServerScript(
                context: context,
                ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,long issueId)
        {
            IssueId = issueId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        factory: context,
                        where: Rds.ItemsWhere().ReferenceId(IssueId)),
                    Rds.RestoreIssues(
                        factory: context,
                        where: Rds.IssuesWhere().IssueId(IssueId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
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
                                                columnBracket: $"\"{column.ColumnName}\"",
                                                name: column.ColumnName,
                                                value: Class(column: column).MaxLength(1024)),
                                            siteId: SiteId,
                                            referenceId: IssueId));
                                    break;
                                case "Num":
                                    var num = Num(column: column);
                                    if (column.Nullable == true)
                                    {
                                        if (num?.Value != null)
                                            statements.Add(column.IfDuplicatedStatement(
                                                param: param.Add(
                                                    columnBracket: $"\"{column.ColumnName}\"",
                                                    name: column.ColumnName,
                                                    value: num.Value),
                                                siteId: SiteId,
                                                referenceId: IssueId));
                                    }
                                    else
                                    {
                                        if (num?.Value != null && num?.Value != 0)
                                            statements.Add(column.IfDuplicatedStatement(
                                                param: param.Add(
                                                    columnBracket: $"\"{column.ColumnName}\"",
                                                    name: column.ColumnName,
                                                    value: num.Value),
                                                siteId: SiteId,
                                                referenceId: IssueId));
                                    }
                                    break;
                                case "Date":
                                    if (Date(column: column) != 0.ToDateTime())
                                        statements.Add(column.IfDuplicatedStatement(
                                            param: param.Add(
                                                columnBracket: $"\"{column.ColumnName}\"",
                                                name: column.ColumnName,
                                                value: Date(column: column)),
                                            siteId: SiteId,
                                            referenceId: IssueId));
                                    break;
                                case "Description":
                                    if (!Description(column: column).IsNullOrEmpty())
                                        statements.Add(column.IfDuplicatedStatement(
                                            param: param.Add(
                                                columnBracket: $"\"{column.ColumnName}\"",
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
            var defaultInput = column.GetDefaultInput(context: context);
            switch (column.ColumnName)
            {
                case "IssueId":
                    IssueId = defaultInput.ToLong();
                    break;
                case "Title":
                    Title.Value = defaultInput.ToString();
                    break;
                case "Body":
                    Body = defaultInput.ToString();
                    break;
                case "WorkValue":
                    WorkValue.Value = defaultInput.ToDecimal();
                    break;
                case "ProgressRate":
                    ProgressRate.Value = defaultInput.ToDecimal();
                    break;
                case "Status":
                    Status.Value = defaultInput.ToInt();
                    break;
                case "Locked":
                    Locked = defaultInput.ToBool();
                    break;
                case "Timestamp":
                    Timestamp = defaultInput.ToString();
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
                                value: defaultInput);
                            break;
                        case "Num":
                            Num(
                                column: column,
                                value: new Num(
                                    context: context,
                                    column: column,
                                    value: defaultInput));
                            break;
                        case "Date":
                            Date(
                                column: column,
                                value: column.DefaultTime());
                            break;
                        case "Description":
                            Description(
                                column: column,
                                value: defaultInput.ToString());
                            break;
                        case "Check":
                            Check(
                                column: column,
                                value: defaultInput.ToBool());
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
            Dictionary<string, string> formData)
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
                    case "Issues_Locked": Locked = value.ToBool(); break;
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
                                        value: new Num(
                                            context: context,
                                            column: column,
                                            value: value));
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
                var value = PropertyValue(
                    context: context,
                    column: column);
                column.Linking = column.MultipleSelections == true
                    ? value.Deserialize<List<string>>()?.Contains(context.Forms.Data("LinkId")) == true
                    : value == context.Forms.Data("LinkId");
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
            Locked = issueModel.Locked;
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
            if (data.Locked != null) Locked = data.Locked.ToBool().ToBool();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash?.ForEach(o => Class(
                columnName: o.Key,
                value: o.Value));
            data.NumHash?.ForEach(o => Num(
                columnName: o.Key,
                value: new Num(o.Value)));
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
                                    columnBracket: $"\"{formulaSet.Target}\"",
                                    name: formulaSet.Target,
                                    value: Num(formulaSet.Target).Value);
                            }
                            break;
                    }
                });
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateIssues(
                    param: param,
                    where: Rds.IssuesWhereDefault(
                        context: context,
                        issueModel: this),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        public void SetByFormula(Context context, SiteSettings ss)
        {
            SetByBeforeFormulaServerScript(
                context: context,
                ss: ss);
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
                    { "WorkValue", WorkValue.Value.ToDecimal() },
                    { "ProgressRate", ProgressRate.Value.ToDecimal() },
                    { "RemainingWorkValue", RemainingWorkValue.Value.ToDecimal() }
                };
                data.AddRange(NumHash.ToDictionary(
                    o => o.Key,
                    o => o.Value?.Value?.ToDecimal() ?? 0));
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
                            value: new Num(value));
                        break;
                }
                if (ss.OutputFormulaLogs == true)
                {
                    context.LogBuilder?.AppendLine($"formulaSet: {formulaSet.GetRecordingData().ToJson()}");
                    context.LogBuilder?.AppendLine($"formulaSource: {data.ToJson()}");
                    context.LogBuilder?.AppendLine($"formulaResult: {{\"{columnName}\":{value}}}");
                }
            });
            SetByAfterFormulaServerScript(
                context: context,
                ss: ss);
        }

        public void SetTitle(Context context, SiteSettings ss)
        {
            if (Title?.ItemTitle != true)
            {
                Title = new Title(
                    context: context,
                    ss: ss,
                    id: IssueId,
                    ver: Ver,
                    isHistory: VerType == Versions.VerTypes.History,
                    data: PropertyValues(
                        context: context,
                        columns: ss.GetTitleColumns(context: context)));
            }
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
                        case "IssueId":
                            match = IssueId.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Ver":
                            match = Ver.Matched(
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
                        case "Locked":
                            match = Locked.Matched(
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
                var dataSet = Repository.ExecuteDataSet(
                    context: context,
                    statements: notifications.Select(notification =>
                    {
                        var where = ss.Views?.Get(before
                            ? notification.BeforeCondition
                            : notification.AfterCondition)
                                ?.Where(
                                    context: context,
                                    ss: ss,
                                    where: Rds.IssuesWhere().IssueId(IssueId))
                                        ?? Rds.IssuesWhere().IssueId(IssueId);
                        return Rds.SelectIssues(
                            dataTableName: notification.Index.ToString(),
                            tableType: tableTypes,
                            column: Rds.IssuesColumn().IssueId(),
                            join: ss.Join(
                                context: context,
                                join: where),
                            where: where);
                    }).ToArray());
                return notifications
                    .Where(notification =>
                        dataSet.Tables[notification.Index.ToString()].Rows.Count == 1)
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
            notifications?.ForEach(notification =>
            {
                if (notification.HasRelatedUsers())
                {
                    var users = new List<int>();
                    Repository.ExecuteTable(
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
                                    users.Add(dataRow.Int("Manager"));
                                    users.Add(dataRow.Int("Owner"));
                                    users.Add(dataRow.Int("Creator"));
                                    users.Add(dataRow.Int("Updator"));
                                });
                    notification.ReplaceRelatedUsers(
                        context: context,
                        users: users);
                }
                var values = ss.IncludedColumns(notification.Address)
                    .ToDictionary(
                        column => column,
                        column => PropertyValue(
                            context: context,
                            column: column));
                switch (type)
                {
                    case "Created":
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Created(
                                context: context,
                                data: Title.DisplayValue).ToString(),
                            body: NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification),
                            values: values);
                        break;
                    case "Updated":
                        if (notification.MonitorChangesColumns.Any(columnName => PropertyUpdated(
                            context: context,
                            name: columnName)))
                        {
                            var body = NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification,
                                update: true);
                            notification.Send(
                                context: context,
                                ss: ss,
                                title: Displays.Updated(
                                    context: context,
                                    data: Title.DisplayValue).ToString(),
                                body: body,
                                values: values);
                        }
                        break;
                    case "Deleted":
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Deleted(
                                context: context,
                                data: Title.DisplayValue).ToString(),
                            body: NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification),
                            values: values);
                        break;
                }
            });
        }

        private string NoticeBody(
            Context context, SiteSettings ss, Notification notification, bool update = false)
        {
            var body = new System.Text.StringBuilder();
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: IssueId);
            var mailAddress = MailAddressUtilities.Get(
                context: context,
                userId: context.UserId);
            notification.GetFormat(
                context,
                ss)
                    .Split('\n')
                    .Select(line => new
                    {
                        Line = line.Trim(),
                        Format = line.Trim().Deserialize<NotificationColumnFormat>()
                    })
                    .ForEach(data =>
                    {
                        var column = ss.IncludedColumns(data.Format?.Name)?.FirstOrDefault();
                        if (column == null)
                        {
                            body.Append(data.Line
                                .Replace("{Url}", url)
                                .Replace("{LoginId}", context.User.LoginId)
                                .Replace("{UserName}", context.User.Name)
                                .Replace("{MailAddress}", mailAddress));
                            body.Append("\n");
                        }
                        else
                        {
                            switch (column.Name)
                            {
                                case "Title":
                                    body.Append(Title.ToNotice(
                                        context: context,
                                        saved: SavedTitle,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Title_Updated(context: context),
                                        update: update));
                                    break;
                                case "Body":
                                    body.Append(Body.ToNotice(
                                        context: context,
                                        saved: SavedBody,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Body_Updated(context: context),
                                        update: update));
                                    break;
                                case "StartTime":
                                    body.Append(StartTime.ToNotice(
                                        context: context,
                                        saved: SavedStartTime,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: StartTime_Updated(context: context),
                                        update: update));
                                    break;
                                case "CompletionTime":
                                    body.Append(CompletionTime.ToNotice(
                                        context: context,
                                        saved: SavedCompletionTime,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: CompletionTime_Updated(context: context),
                                        update: update));
                                    break;
                                case "WorkValue":
                                    body.Append(WorkValue.ToNotice(
                                        context: context,
                                        saved: SavedWorkValue,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: WorkValue_Updated(context: context),
                                        update: update));
                                    break;
                                case "ProgressRate":
                                    body.Append(ProgressRate.ToNotice(
                                        context: context,
                                        saved: SavedProgressRate,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: ProgressRate_Updated(context: context),
                                        update: update));
                                    break;
                                case "Status":
                                    body.Append(Status.ToNotice(
                                        context: context,
                                        saved: SavedStatus,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Status_Updated(context: context),
                                        update: update));
                                    break;
                                case "Manager":
                                    body.Append(Manager.ToNotice(
                                        context: context,
                                        saved: SavedManager,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Manager_Updated(context: context),
                                        update: update));
                                    break;
                                case "Owner":
                                    body.Append(Owner.ToNotice(
                                        context: context,
                                        saved: SavedOwner,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Owner_Updated(context: context),
                                        update: update));
                                    break;
                                case "Locked":
                                    body.Append(Locked.ToNotice(
                                        context: context,
                                        saved: SavedLocked,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Locked_Updated(context: context),
                                        update: update));
                                    break;
                                case "Comments":
                                    body.Append(Comments.ToNotice(
                                        context: context,
                                        saved: SavedComments,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Comments_Updated(context: context),
                                        update: update));
                                    break;
                                case "Creator":
                                    body.Append(Creator.ToNotice(
                                        context: context,
                                        saved: SavedCreator,
                                        column: column,
                                        notificationColumnFormat: data.Format,
                                        updated: Creator_Updated(context: context),
                                        update: update));
                                    break;
                                case "Updator":
                                    body.Append(Updator.ToNotice(
                                        context: context,
                                        saved: SavedUpdator,
                                        column: column,
                                        notificationColumnFormat: data.Format,
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
                                                notificationColumnFormat: data.Format,
                                                updated: Class_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Num":
                                            body.Append(Num(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: SavedNum(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Num_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Date":
                                            body.Append(Date(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: SavedDate(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Date_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Description":
                                            body.Append(Description(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: SavedDescription(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Description_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Check":
                                            body.Append(Check(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: SavedCheck(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Check_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                        case "Attachments":
                                            body.Append(Attachments(columnName: column.Name).ToNotice(
                                                context: context,
                                                saved: SavedAttachments(columnName: column.Name),
                                                column: column,
                                                notificationColumnFormat: data.Format,
                                                updated: Attachments_Updated(columnName: column.Name),
                                                update: update));
                                            break;
                                    }
                                    break;
                            }
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
                    var column = ss.GetColumn(
                        context: context,
                        columnName: link.ColumnName);
                    var value = PropertyValue(
                        context: context,
                        column: column);
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
                            RemainingWorkValue = new Num(dataRow, "RemainingWorkValue");
                            SavedRemainingWorkValue = RemainingWorkValue.Value;
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
                        case "Locked":
                            Locked = dataRow[column.ColumnName].ToBool();
                            SavedLocked = Locked;
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
                                        value: new Num(
                                            dataRow: dataRow,
                                            name: column.ColumnName));
                                    SavedNum(
                                        columnName: column.Name,
                                        value: Num(columnName: column.Name).Value);
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
            SetByWhenloadingRecordServerScript(
                context: context,
                ss: ss);
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
                || Locked_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public override List<string> Mine(Context context)
        {
            if (MineCache == null)
            {
                var mine = new List<string>();
                var userId = context.UserId;
                if (SavedManager == userId) mine.Add("Manager");
                if (SavedOwner == userId) mine.Add("Owner");
                if (SavedCreator == userId) mine.Add("Creator");
                if (SavedUpdator == userId) mine.Add("Updator");
                MineCache = mine;
            }
            return MineCache;
        }

        public string IdSuffix()
        {
            return $"_{SiteId}_{(IssueId == 0 ? -1 : IssueId)}";
        }
    }
}
