using Implem.DefinitionAccessor;
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
using System.Runtime.Serialization;
using System.Text;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Reminder : ISettingListItem
    {
        const string BodyPlaceholder = "[[Records]]";
        public int Id { get; set; }
        public ReminderTypes ReminderType;
        public string Subject;
        public string Body;
        public string Line;
        public string From;
        public string To;
        public string Token;
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

        public enum ReminderTypes : int
        {
            Mail = 1,
            Slack = 2,
            ChatWork = 3,
            Line = 4,
            LineGroup = 5,
            Teams = 6,
            RocketChat = 7,
            InCircle = 8
        }

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
            ReminderTypes reminderType,
            string subject,
            string body,
            string line,
            string from,
            string to,
            string token,
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
            ReminderType = reminderType;
            Subject = subject;
            Body = body;
            Line = line;
            From = from;
            To = to;
            Token = token;
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
            ReminderTypes reminderType,
            string subject,
            string body,
            string line,
            string from,
            string to,
            string token,
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
            ReminderType = reminderType;
            Subject = subject;
            Body = body;
            Line = line;
            From = from;
            To = to;
            Token = token;
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

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (ReminderType == 0)
            {
                ReminderType = ReminderTypes.Mail;
            }
        }

        public string GetColumn(SiteSettings ss)
        {
            return Column
                ?? (ss.ColumnHash.ContainsKey("CompletionTime")
                    ? "CompletionTime"
                    : null);
        }

        public void Remind(
            Context context,
            SiteSettings ss,
            DateTime scheduledTime,
            bool test = false)
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
                var dataTable = GetDataTable(
                    context: context,
                    ss: ss,
                    toColumns: toColumns,
                    subjectColumns: subjectColumns,
                    bodyColumns: bodyColumns,
                    scheduledTime: scheduledTime);
                var reminderType = ReminderType.ToInt() == 0
                    ? ReminderTypes.Mail
                    : ReminderType;
                var title = reminderType != ReminderTypes.Mail
                    ? GetSubject(
                        context: context,
                        ss: ss,
                        dataRows: dataTable.AsEnumerable().ToList(),
                        test: test)
                    : null;
                var dataRows = dataTable.AsEnumerable().ToList();
                var notSend = dataRows.Any()
                    ? false
                    : NotSendIfNotApplicable == true;
                var body = reminderType != ReminderTypes.Mail
                    ? GetBody(
                        context: context,
                        ss: ss,
                        dataRows: dataRows)
                    : null;
                switch (reminderType)
                {
                    case ReminderTypes.Mail:
                        if (Parameters.Reminder.Mail)
                        {
                            GetDataHash(
                                context: context,
                                ss: ss,
                                dataTable: dataTable,
                                toColumns: toColumns,
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
                                            From = MimeKit.MailboxAddress.Parse(Strings.CoalesceEmpty(
                                                Parameters.Mail.FixedFrom,
                                                From)),
                                            To = data.Key
                                        }.Send(
                                            context: context,
                                            ss: ss);
                                    });
                        }
                        break;
                    case ReminderTypes.Slack:
                        if (Parameters.Reminder.Slack && !notSend)
                        {
                            new Slack(
                                _context: context,
                                _text: $"*{title}*\n{body}",
                                _username: From)
                                    .Send(To);
                        }
                        break;
                    case ReminderTypes.ChatWork:
                        if (Parameters.Reminder.ChatWork && !notSend)
                        {
                            new ChatWork(
                                _context: context,
                                _text: $"*{title}*\n{body}",
                                _username: From,
                                _token: Token)
                                    .Send(To);
                        }
                        break;
                    case ReminderTypes.Line:
                    case ReminderTypes.LineGroup:
                        if (Parameters.Reminder.Line && !notSend)
                        {
                            new Line(
                                _context: context,
                                _text: $"*{title}*\n{body}",
                                _username: From,
                                _token: Token)
                                    .Send(To, ReminderType == ReminderTypes.LineGroup);
                        }
                        break;
                    case ReminderTypes.Teams:
                        if (Parameters.Reminder.Teams && !notSend)
                        {
                            new Teams(
                                _context: context,
                                _text: $"*{title}*\n{body}")
                                    .Send(To);
                        }
                        break;
                    case ReminderTypes.RocketChat:
                        if (Parameters.Reminder.RocketChat && !notSend)
                        {
                            new RocketChat(
                                _context: context,
                                _text: $"*{title}*\n{body}",
                                _username: From)
                                    .Send(To);
                        }
                        break;
                    case ReminderTypes.InCircle:
                        if (Parameters.Reminder.InCircle && !notSend)
                        {
                            new InCircle(
                                _context: context,
                                _text: $"*{title}*\n{body}",
                                _username: From,
                                _token: Token)
                                    .Send(To);
                        }
                        break;
                    default:
                        break;
                }
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
                new SysLogModel(
                    context: context,
                    e: e);
                var reminder = ss.Reminders.Where(o => o.Id == Id)
                    .FirstOrDefault();
                if (reminder != null)
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: new[]
                        {
                            Rds.PhysicalDeleteReminderSchedules(
                                where: Rds.ReminderSchedulesWhere()
                                    .SiteId(ss.SiteId)
                                    .Id(Id))
                        });
                    var title = new Title(value: Displays.ReminderErrorTitle(
                        context: context,
                        data: Displays.ProductName(context)));
                    var body = Displays.ReminderErrorContent(
                        context: context,
                        data: new string[]
                        {
                            ss.Title,
                            ss.SiteId.ToString(),
                            reminder.Subject,
                            Id.ToString(),
                            e.Message,
                            e.StackTrace
                        });
                    new OutgoingMailModel()
                    {
                        Title = title,
                        Body = body,
                        From = MimeKit.MailboxAddress.Parse(Strings.CoalesceEmpty(
                            Parameters.Mail.FixedFrom,
                            From)),
                        To = reminder.To
                    }.Send(
                        context: context,
                        ss: ss);
                }
                if (test)
                {
                    throw;
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
            DataTable dataTable,
            List<Column> toColumns,
            string fixedTo)
        {
            var hash = new Dictionary<string, Dictionary<long, DataRow>>()
            {
                {
                    fixedTo,
                    new Dictionary<long, DataRow>()
                }
            };
            dataTable
                .AsEnumerable()
                .ForEach(dataRow =>
                {
                    var id = dataRow.Long(Rds.IdColumn(ss.ReferenceType));
                    hash[fixedTo].AddIfNotConainsKey(id, dataRow);
                    toColumns.ForEach(toColumn =>
                        Addresses.Get(
                            context: context,
                            addresses: Addresses.ReplacedAddress(
                                                context: context,
                                                column: toColumn,
                                                value: dataRow.String(toColumn.ColumnName)))
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

        private string GetBody(
            Context context,
            SiteSettings ss,
            List<DataRow> dataRows)
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

        private DataTable GetDataTable(
            Context context,
            SiteSettings ss,
            List<Column> toColumns,
            List<Column> subjectColumns,
            List<Column> bodyColumns,
            DateTime scheduledTime)
        {
            var orderByColumn = ss.GetColumn(
                context: context,
                columnName: Column);
            var convertedScheduledTime = scheduledTime.ToDateTime().ToUniversal(context: context).ToString("yyyy/M/d H:m:s.fff");
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
                // リマインダーはログインユーザが存在しないためアクセス権のチェックをバイパスする
                checkPermission: false,
                requestSearchCondition: false)
                    .Add(
                        tableName: ss.ReferenceType,
                        columnBrackets: new string[]
                        {
                            "\"" + orderByColumn.ColumnName + "\""
                        },
                        _operator: "<'{0}'".Params(
                            DateTime.Now.ToLocal(context: context).Date.AddDays(Range)))
                    .Add(
                        //【完了項目】
                        //   ■時刻なし(エディタの書式が"年月日")
                        //     完了項目は期限切れかどうかを判定するために設計された項目
                        //     項目に"2022/12/01"が設定されていて、リマインドするタイミングが"2022/12/01 12:00"であった場合、
                        //     当日中として扱うために(期限内とするために)データベース上では値を"+1日"して登録している
                        //     ・リマインド対象
                        //       「データベース上の値 > リマインドするタイミング」に当てはまるレコード
                        //   ■時刻あり(エディタの書式が"年月日"以外)
                        //     時刻の指定がある場合、当日中という判定は行わないためリマインドするタイミングとそのまま比較する
                        //     項目の値が"2022/12/01 12:00"、リマインドするタイミングが"2022/12/01 12:00"のように
                        //     項目の値とリマインドするタイミングが同日同時刻の場合もリマインド対象に含める
                        //     ・リマインド対象
                        //       「データベース上の値 >= リマインドするタイミング」に当てはまるレコード
                        //【日付項目】
                        //   ■時刻なし(エディタの書式が"年月日")
                        //     日付項目は期限切れを判定する項目として使用するために、
                        //     データベース上の値に"+1日"して期限に含まれているか判定する
                        //     ・リマインド対象
                        //       「データベース上の値 + 1日 > リマインドするタイミング」に当てはまるレコード
                        //   ■時刻あり(エディタの書式が"年月日"以外)
                        //     時刻の指定がある場合、当日中という判定は行わないためリマインドするタイミングとそのまま比較する
                        //     項目の値が"2022/12/01 12:00"、リマインドするタイミングが"2022/12/01 12:00"のように
                        //     項目の値とリマインドするタイミングが同日同時刻の場合もリマインド対象に含める
                        //     ・リマインド対象
                        //       「データベース上の値 >= リマインドするタイミング」に当てはまるレコード
                        tableName: ss.ReferenceType,
                        columnBrackets: new string[]
                        {
                            orderByColumn.ColumnName == "CompletionTime" || ContainsTimeSettings(orderByColumn)
                                ? "\"" + orderByColumn.ColumnName + "\""
                                : context.Sqls.DateAddDay(1, orderByColumn.ColumnName)
                        },
                        _operator: ContainsTimeSettings(orderByColumn)
                            ? $">='{convertedScheduledTime}'"
                            : $">'{convertedScheduledTime}'",
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

        private bool ContainsTimeSettings(Column column)
        {
            return column.EditorFormat == "Ymdhm" || column.EditorFormat == "Ymdhms";
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
            reminder.ReminderType = ReminderType;
            reminder.Subject = Subject;
            reminder.Body = Body;
            reminder.Line = Line;
            reminder.From = From;
            reminder.To = To;
            reminder.Token = Token;
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
                    line = issueModel.ReplaceLineByIssueModel(context, ss, line, dataRow.String("ItemTitle"));
                    break;
                case "Results":
                    var resultModel = new ResultModel(
                        context: context, ss: ss, dataRow: dataRow);
                    line = resultModel.ReplaceLineByResultModel(context, ss, line, dataRow.String("ItemTitle"));
                    break;
            }
            return line;
        }
    }
}
