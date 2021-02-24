﻿using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Reminder : ISettingListItem
    {
        const string BodyPlaceholder = "[[Records]]";
        public int Id { get; set; }
        public string Subject;
        public string Body;
        public string Line;
        public string From;
        public string To;
        public string Column;
        public DateTime StartDateTime;
        public Times.RepeatTypes Type;
        public int Range;
        public bool? SendCompletedInPast;
        public bool? NotSendIfNotApplicable;
        public bool? NotSendHyperLink;
        public bool? ExcludeOverdue;
        public int Condition;
        public bool? Disabled;

        public Reminder()
        {
        }

        public Reminder(Context context)
        {
            Body = BodyPlaceholder;
            Line = Parameters.Reminder.DefaultLine;
            StartDateTime = DateTime.Now.ToLocal(context: context).Date.AddDays(1);
            Range = Parameters.Reminder.DefaultRange;
        }

        public Reminder(
            int id,
            string subject,
            string body,
            string line,
            string from,
            string to,
            string column,
            DateTime startDateTime,
            Times.RepeatTypes type,
            int range,
            bool sendCompletedInPast,
            bool notSendIfNotApplicable,
            bool notSendHyperLink,
            bool excludeOverdue,
            int condition,
            bool disabled)
        {
            Id = id;
            Subject = subject;
            Body = body;
            Line = line;
            From = from;
            To = to;
            Column = column;
            StartDateTime = startDateTime;
            Type = type;
            Range = range;
            SendCompletedInPast = sendCompletedInPast;
            NotSendIfNotApplicable = notSendIfNotApplicable;
            NotSendHyperLink = notSendHyperLink;
            ExcludeOverdue = excludeOverdue;
            Condition = condition;
            Disabled = disabled;
        }

        public void Update(
            string subject,
            string body,
            string line,
            string from,
            string to,
            string column,
            DateTime startDateTime,
            Times.RepeatTypes type,
            int range,
            bool sendCompletedInPast,
            bool notSendIfNotApplicable,
            bool notSendHyperLink,
            bool excludeOverdue,
            int condition,
            bool disabled)
        {
            Subject = subject;
            Body = body;
            Line = line;
            From = from;
            To = to;
            Column = column;
            StartDateTime = startDateTime;
            Type = type;
            Range = range;
            SendCompletedInPast = sendCompletedInPast;
            NotSendIfNotApplicable = notSendIfNotApplicable;
            NotSendHyperLink = notSendHyperLink;
            ExcludeOverdue = excludeOverdue;
            Condition = condition;
            Disabled = disabled;
        }

        public string GetColumn(SiteSettings ss)
        {
            return Column
                ?? (ss.ColumnHash.ContainsKey("CompletionTime")
                    ? "CompletionTime"
                    : null);
        }

        public void Remind(Context context, SiteSettings ss, bool test = false)
        {
            if (Disabled == true && !test) return;
            try
            {
                ss.SetChoiceHash(
                    context: context,
                    withLink: true,
                    all: true);
                var toColumns = ss.IncludedColumns(To);
                var subjectColumns = ss.IncludedColumns(Subject);
                var bodyColumns = ss.IncludedColumns(Body);
                var fixedTo = GetFixedTo(
                    context: context,
                    toColumns: toColumns);
                GetDataHash(
                    context: context,
                    ss: ss,
                    toColumns: toColumns,
                    subjectColumns: subjectColumns,
                    bodyColumns: bodyColumns,
                    fixedTo: fixedTo)
                        .Where(data => !data.Key.IsNullOrEmpty())
                        .Where(data => data.Value.Count > 0
                            || NotSendIfNotApplicable != true)
                        .ForEach(data =>
                        {
                            new OutgoingMailModel()
                            {
                                Title = GetSubject(
                                    context: context,
                                    ss: ss,
                                    dataRows: data.Value.Values.ToList(),
                                    test: test),
                                Body = GetBody(
                                    context: context,
                                    ss: ss,
                                    dataRows: data.Value.Values.ToList()),
                                From = new MailAddress(From),
                                To = data.Key
                            }.Send(
                                context: context,
                                ss: ss);
                        });
                if (!test)
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.UpdateReminderSchedules(
                            param: Rds.ReminderSchedulesParam()
                                .ScheduledTime(StartDateTime.Next(
                                    context: context,
                                    type: Type)),
                            where: Rds.ReminderSchedulesWhere()
                                .SiteId(ss.SiteId)
                                .Id(Id)));
                }
            }
            catch (Exception e)
            {
                new SysLogModel(context: context, e: e);
                if (test)
                {
                    throw e;
                }
            }
        }

        private string GetFixedTo(Context context, List<Column> toColumns)
        {
            var to = To;
            toColumns.ForEach(toColumn =>
                to = to.Replace($"[{toColumn.ColumnName}]", string.Empty));
            to = Addresses.Get(
                context: context,
                addresses: to)
                    .Join(",");
            return to;
        }

        private Dictionary<string, Dictionary<long, DataRow>> GetDataHash(
            Context context,
            SiteSettings ss,
            List<Column> toColumns,
            List<Column> subjectColumns,
            List<Column> bodyColumns,
            string fixedTo)
        {
            var hash = new Dictionary<string, Dictionary<long, DataRow>>()
            {
                {
                    fixedTo,
                    new Dictionary<long, DataRow>()
                }
            };
            GetDataSet(
                context: context,
                ss: ss,
                toColumns: toColumns,
                subjectColumns: subjectColumns,
                bodyColumns: bodyColumns)
                    .AsEnumerable()
                    .ForEach(dataRow =>
                    {
                        var id = dataRow.Long(Rds.IdColumn(ss.ReferenceType));
                        hash[fixedTo].AddIfNotConainsKey(id, dataRow);
                        toColumns.ForEach(toColumn =>
                            Addresses.Get(
                                context: context,
                                addresses: toColumn.Type == Settings.Column.Types.User
                                    ? $"[User{dataRow.String(toColumn.ColumnName)}]"
                                    : dataRow.String(toColumn.ColumnName))
                                        .ForEach(mailAddress =>
                                        {
                                            if (!hash.ContainsKey(mailAddress))
                                            {
                                                hash.Add(
                                                    mailAddress,
                                                    new Dictionary<long, DataRow>());
                                            }
                                            hash[mailAddress].AddIfNotConainsKey(id, dataRow);
                                        }));
                    });
            return hash;
        }

        private Title GetSubject(Context context, SiteSettings ss, List<DataRow> dataRows, bool test)
        {
            var subject = ReplacedLine(
                context: context,
                ss: ss,
                dataRow: dataRows.FirstOrDefault(),
                line: Subject);
            return new Title((test
                ? "(" + Displays.Test(context: context) + ")"
                : string.Empty)
                    + subject);
        }

        private string GetBody(Context context, SiteSettings ss, List<DataRow> dataRows)
        {
            var sb = new StringBuilder();
            var timeGroups = dataRows
                .GroupBy(dataRow => dataRow.DateTime(Column).Date)
                .ToList();
            var body = ReplacedLine(
                context: context,
                ss: ss,
                dataRow: dataRows.FirstOrDefault(),
                line: Body);
            timeGroups.ForEach(timeGroup =>
            {
                var date = timeGroup.First().DateTime(Column).ToLocal(context: context).Date;
                switch (Column)
                {
                    case "CompletionTime":
                        date = date.AddDifferenceOfDates(
                            format: ss.GetColumn(
                                context: context,
                                columnName: "CompletionTime")?
                                    .EditorFormat,
                            minus: true);
                        break;
                }
                sb.Append("{0} ({1})\n".Params(
                    date.ToString(
                        Displays.Get(context: context, id: "YmdaFormat"),
                        context.CultureInfo()),
                    Relative(context: context, time: date)));
                timeGroup
                    .OrderBy(dataRow => dataRow.Int("Status"))
                    .ForEach(dataRow =>
                    {
                        sb.Append(
                            "\t",
                            ReplacedLine(
                                context: context,
                                ss: ss,
                                dataRow: dataRow,
                                line: Line));
                        if (NotSendHyperLink != true)
                        {
                            sb.Append(
                                "\n\t",
                                Locations.ItemEditAbsoluteUri(
                                    context: context,
                                    id: dataRow.Long(Rds.IdColumn(ss.ReferenceType))));
                        }
                        sb.Append("\n");
                    });
                sb.Append("\n");
            });
            if (!timeGroups.Any())
            {
                sb.Append(Displays.NoTargetRecord(context: context), "\r\n");
            }
            return body.Contains(BodyPlaceholder)
                ? body.Replace(BodyPlaceholder, sb.ToString())
                : body + "\n" + sb.ToString();
        }

        private DataTable GetDataSet(
            Context context,
            SiteSettings ss,
            List<Column> toColumns,
            List<Column> subjectColumns,
            List<Column> bodyColumns)
        {
            var orderByColumn = ss.GetColumn(
                context: context,
                columnName: Column);
            var column = new SqlColumnCollection()
                .Add(column: ss.GetColumn(
                    context: context,
                    columnName: Rds.IdColumn(ss.ReferenceType)))
                .Add(column: orderByColumn)
                .ItemTitle(ss.ReferenceType);
            toColumns.ForEach(toColumn =>
                column.Add(column: toColumn));
            subjectColumns.ForEach(subjectColumn =>
                column.Add(column: subjectColumn));
            bodyColumns.ForEach(bodyColumn =>
                column.Add(column: bodyColumn));
            var columns = ss.IncludedColumns(Line).ToList();
            columns.ForEach(o => column.Add(column: o));
            if (columns.Any(o => o.ColumnName == "Status"))
            {
                columns.Add(ss.GetColumn(
                    context: context,
                    columnName: "Status"));
            }
            var view = ss.Views?.Get(Condition) ?? new View();
            var where = view.Where(
                context: context,
                ss: ss,
                checkPermission: false)
                    .Add(
                        tableName: ss.ReferenceType,
                        columnBrackets: new string[]
                        {
                            "\"" + orderByColumn.ColumnName + "\""
                        },
                        _operator: "<'{0}'".Params(
                            DateTime.Now.ToLocal(context: context).Date.AddDays(Range)))
                    .Add(
                        tableName: ss.ReferenceType,
                        columnBrackets: new string[]
                        {
                            "\"" + orderByColumn.ColumnName + "\""
                        },
                        _operator: ">getdate()",
                        _using: ExcludeOverdue == true)
                    .Add(or: new SqlWhereCollection()
                        .Add(
                            tableName: ss.ReferenceType,
                            columnBrackets: new string[]
                            {
                                "\"Status\""
                            },
                            _operator: " is null")
                        .Add(
                            tableName: ss.ReferenceType,
                            columnBrackets: new string[]
                            {
                                "\"Status\""
                            },
                            _operator: "<{0}".Params(Parameters.General.CompletionCode))
                        .Add(
                            tableName: ss.ReferenceType,
                            columnBrackets: new string[]
                            {
                                "\"" + orderByColumn.ColumnName + "\""
                            },
                            _operator: "<'{0}'".Params(
                                DateTime.Now.ToLocal(context: context).Date),
                            _using: SendCompletedInPast == true));
            var orderBy = new SqlOrderByCollection()
                .Add(
                    column: orderByColumn,
                    orderType: SqlOrderBy.Types.desc);
            var dataTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.Select(
                    tableName: ss.ReferenceType,
                    column: column,
                    join: ss.Join(
                        context: context,
                        join: new IJoin[]
                        {
                            column,
                            where,
                            orderBy
                        }),
                    where: where,
                    orderBy: orderBy,
                    top: Parameters.Reminder.Limit));
            return dataTable;
        }

        private string Relative(Context context, DateTime time)
        {
            var diff = DateTime.Now.ToLocal(context: context).Date - time;
            if (diff.TotalDays == 0)
            {
                return Displays.Today(context: context);
            }
            else if (diff.TotalDays < 0)
            {
                return Displays.LimitAfterDays(
                    context: context,
                    data: (diff.Days * -1).ToString());
            }
            else
            {
                return Displays.LimitBeforeDays(
                    context: context,
                    data: diff.Days.ToString());
            }
        }

        public Reminder GetRecordingData(Context context)
        {
            var reminder = new Reminder(context: context);
            reminder.Id = Id;
            reminder.Subject = Subject;
            reminder.Body = Body;
            reminder.Line = Line;
            reminder.From = From;
            reminder.To = To;
            reminder.Column = Column;
            reminder.StartDateTime = StartDateTime;
            reminder.Type = Type;
            reminder.Range = Range;
            if (SendCompletedInPast == true)
            {
                reminder.SendCompletedInPast = SendCompletedInPast;
            }
            if (NotSendIfNotApplicable == true)
            {
                reminder.NotSendIfNotApplicable = NotSendIfNotApplicable;
            }
            if (NotSendHyperLink == true)
            {
                reminder.NotSendHyperLink = NotSendHyperLink;
            }
            if (ExcludeOverdue == true)
            {
                reminder.ExcludeOverdue = ExcludeOverdue;
            }
            if (Disabled == true)
            {
                reminder.Disabled = Disabled;
            }
            reminder.Condition = Condition;
            return reminder;
        }

        private string ReplacedLine(Context context, SiteSettings ss, DataRow dataRow, string line)
        {
            switch (ss.ReferenceType)
            {
                case "Issues":
                    var issueModel = new IssueModel(
                        context: context, ss: ss, dataRow: dataRow);
                    ss.IncludedColumns(line).ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "Title":
                                line = line.Replace("[Title]", dataRow.String("ItemTitle"));
                                break;
                            case "SiteId":
                                line = line.Replace(
                                    "[SiteId]",
                                    issueModel.SiteId.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "UpdatedTime":
                                line = line.Replace(
                                    "[UpdatedTime]",
                                    issueModel.UpdatedTime.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "IssueId":
                                line = line.Replace(
                                    "[IssueId]",
                                    issueModel.IssueId.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Ver":
                                line = line.Replace(
                                    "[Ver]",
                                    issueModel.Ver.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Body":
                                line = line.Replace(
                                    "[Body]",
                                    issueModel.Body.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "StartTime":
                                line = line.Replace(
                                    "[StartTime]",
                                    issueModel.StartTime.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "CompletionTime":
                                line = line.Replace(
                                    "[CompletionTime]",
                                    issueModel.CompletionTime.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "WorkValue":
                                line = line.Replace(
                                    "[WorkValue]",
                                    issueModel.WorkValue.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "ProgressRate":
                                line = line.Replace(
                                    "[ProgressRate]",
                                    issueModel.ProgressRate.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Status":
                                line = line.Replace(
                                    "[Status]",
                                    issueModel.Status.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Manager":
                                line = line.Replace(
                                    "[Manager]",
                                    issueModel.Manager.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Owner":
                                line = line.Replace(
                                    "[Owner]",
                                    issueModel.Owner.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Locked":
                                line = line.Replace(
                                    "[Locked]",
                                    issueModel.Locked.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Comments":
                                line = line.Replace(
                                    "[Comments]",
                                    issueModel.Comments.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Creator":
                                line = line.Replace(
                                    "[Creator]",
                                    issueModel.Creator.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Updator":
                                line = line.Replace(
                                    "[Updator]",
                                    issueModel.Updator.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "CreatedTime":
                                line = line.Replace(
                                    "[CreatedTime]",
                                    issueModel.CreatedTime.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column.Name))
                                {
                                    case "Class":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            issueModel.Class(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                    case "Num":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            issueModel.Num(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                    case "Date":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            issueModel.Date(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                    case "Description":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            issueModel.Description(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                    case "Check":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            issueModel.Check(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                }
                                break;
                        }
                    });
                    break;
                case "Results":
                    var resultModel = new ResultModel(
                        context: context, ss: ss, dataRow: dataRow);
                    ss.IncludedColumns(line).ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "Title":
                                line = line.Replace("[Title]", dataRow.String("ItemTitle"));
                                break;
                            case "SiteId":
                                line = line.Replace(
                                    "[SiteId]",
                                    resultModel.SiteId.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "UpdatedTime":
                                line = line.Replace(
                                    "[UpdatedTime]",
                                    resultModel.UpdatedTime.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "ResultId":
                                line = line.Replace(
                                    "[ResultId]",
                                    resultModel.ResultId.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Ver":
                                line = line.Replace(
                                    "[Ver]",
                                    resultModel.Ver.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Body":
                                line = line.Replace(
                                    "[Body]",
                                    resultModel.Body.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Status":
                                line = line.Replace(
                                    "[Status]",
                                    resultModel.Status.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Manager":
                                line = line.Replace(
                                    "[Manager]",
                                    resultModel.Manager.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Owner":
                                line = line.Replace(
                                    "[Owner]",
                                    resultModel.Owner.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Locked":
                                line = line.Replace(
                                    "[Locked]",
                                    resultModel.Locked.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Comments":
                                line = line.Replace(
                                    "[Comments]",
                                    resultModel.Comments.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Creator":
                                line = line.Replace(
                                    "[Creator]",
                                    resultModel.Creator.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Updator":
                                line = line.Replace(
                                    "[Updator]",
                                    resultModel.Updator.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "CreatedTime":
                                line = line.Replace(
                                    "[CreatedTime]",
                                    resultModel.CreatedTime.ToExport(
                                        context: context,
                                        column: column));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column.Name))
                                {
                                    case "Class":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            resultModel.Class(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                    case "Num":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            resultModel.Num(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                    case "Date":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            resultModel.Date(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                    case "Description":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            resultModel.Description(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                    case "Check":
                                        line = line.Replace(
                                            $"[{column.Name}]",
                                            resultModel.Check(column: column).ToExport(
                                                context: context,
                                                column: column));
                                        break;
                                }
                                break;
                        }
                    });
                    break;
            }
            return line;
        }
    }
}
