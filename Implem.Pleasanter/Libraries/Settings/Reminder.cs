using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
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
        public int Condition;

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
            int condition)
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
            Condition = condition;
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
            int condition)
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
            Condition = condition;
        }

        public string DisplayLine(SiteSettings ss)
        {
            return ss.ColumnNameToLabelText(Line);
        }

        public string GetColumn(SiteSettings ss)
        {
            return Column ??
                (ss.ColumnHash.ContainsKey("CompletionTime")
                    ? "CompletionTime"
                    : null);
        }

        public void Remind(Context context, SiteSettings ss, bool test = false)
        {
            ss.SetChoiceHash(context: context, withLink: true, all: true);
            var dataSet = GetDataSet(context: context, ss: ss);
            if (Rds.Count(dataSet) > 0 || NotSendIfNotApplicable != true)
            {
                new OutgoingMailModel()
                {
                    Title = GetSubject(context: context, test: test),
                    Body = GetBody(context: context, ss: ss, dataSet: dataSet),
                    From = new MailAddress(From),
                    To = To
                }.Send(context: context, ss: ss);
            }
            if (!test)
            {
                Rds.ExecuteNonQuery(
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

        private Title GetSubject(Context context, bool test)
        {
            return new Title((test
                ? "(" + Displays.Test(context: context) + ")"
                : string.Empty)
                    + Subject);
        }

        private string GetBody(Context context, SiteSettings ss, DataSet dataSet)
        {
            var sb = new StringBuilder();
            var timeGroups = dataSet.Tables["Main"]
                .AsEnumerable()
                .GroupBy(dataRow => dataRow.DateTime(Column).Date)
                .ToList();
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
                        sb.Append(
                            "\t",
                            ReplacedLine(context: context, ss: ss, dataRow: dataRow),
                            "\n\t",
                            Locations.ItemEditAbsoluteUri(
                                context: context,
                                id: dataRow.Long(Rds.IdColumn(ss.ReferenceType))),
                            "\n"));
                sb.Append("\n");
            });
            if (!timeGroups.Any())
            {
                sb.Append(Displays.NoTargetRecord(context: context), "\r\n");
            }
            return Body.Contains(BodyPlaceholder)
                ? Body.Replace(BodyPlaceholder, sb.ToString())
                : Body + "\n" + sb.ToString();
        }

        private DataSet GetDataSet(Context context, SiteSettings ss)
        {
            var orderByColumn = ss.GetColumn(context: context, columnName: Column);
            var column = new SqlColumnCollection()
                .Add(
                    context: context,
                    ss: ss,
                    column: ss.GetColumn(
                        context: context,
                        columnName: Rds.IdColumn(ss.ReferenceType)))
                .Add(
                    context: context,
                    ss: ss,
                    column: orderByColumn)
                .ItemTitle(ss.ReferenceType);
            var columns = ss.IncludedColumns(Line).ToList();
            columns.ForEach(o => column.Add(
                context: context,
                ss: ss,
                column: o));
            if (columns.Any(o => o.ColumnName == "Status"))
            {
                columns.Add(ss.GetColumn(context: context, columnName: "Status"));
            }
            var view = ss.Views?.Get(Condition) ?? new View();
            var orderBy = new SqlOrderByCollection()
                .Add(ss, orderByColumn, SqlOrderBy.Types.desc);
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                statements: Rds.Select(
                    tableName: ss.ReferenceType,
                    dataTableName: "Main",
                    column: column,
                    join: new SqlJoinCollection().ItemJoin(
                        tableType: Sqls.TableTypes.Normal,
                        tableName: ss.ReferenceType),
                    where: view.Where(context: context, ss: ss, checkPermission: false)
                        .Add(
                            tableName: ss.ReferenceType,
                            columnBrackets: new string[] { orderByColumn.ColumnName },
                            _operator: "<'{0}'".Params(
                                DateTime.Now.ToLocal(context: context).Date.AddDays(Range)))
                        .Or(new SqlWhereCollection()
                            .Add(
                                tableName: ss.ReferenceType,
                                columnBrackets: new string[] { "[Status]" },
                                _operator: "<{0}".Params(Parameters.General.CompletionCode))
                            .Add(
                                tableName: ss.ReferenceType,
                                columnBrackets: new string[]
                                {
                                    "[" + orderByColumn.ColumnName + "]"
                                },
                                _operator: "<'{0}'".Params(
                                    DateTime.Now.ToLocal(context: context).Date),
                                _using: SendCompletedInPast == true)),
                    orderBy: new SqlOrderByCollection().Add(ss, ss.GetColumn(
                        context: context, columnName: Column)),
                    pageSize: Parameters.Reminder.Limit,
                    countRecord: true));
            return dataSet;
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
            reminder.Condition = Condition;
            return reminder;
        }

        private string ReplacedLine(Context context, SiteSettings ss, DataRow dataRow)
        {
            var line = Line;
            switch (ss.ReferenceType)
            {
                case "Issues":
                    var issueModel = new IssueModel(
                        context: context, ss: ss, dataRow: dataRow);
                    ss.IncludedColumns(Line).ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "Title":
                                line = line.Replace("[Title]", dataRow.String("ItemTitle"));
                                break;
                            case "SiteId":
                                line = line.Replace(
                                    "[SiteId]",
                                    issueModel.SiteId.ToExport(context: context, column: column));
                                break;
                            case "UpdatedTime":
                                line = line.Replace(
                                    "[UpdatedTime]",
                                    issueModel.UpdatedTime.ToExport(context: context, column: column));
                                break;
                            case "IssueId":
                                line = line.Replace(
                                    "[IssueId]",
                                    issueModel.IssueId.ToExport(context: context, column: column));
                                break;
                            case "Ver":
                                line = line.Replace(
                                    "[Ver]",
                                    issueModel.Ver.ToExport(context: context, column: column));
                                break;
                            case "Body":
                                line = line.Replace(
                                    "[Body]",
                                    issueModel.Body.ToExport(context: context, column: column));
                                break;
                            case "StartTime":
                                line = line.Replace(
                                    "[StartTime]",
                                    issueModel.StartTime.ToExport(context: context, column: column));
                                break;
                            case "CompletionTime":
                                line = line.Replace(
                                    "[CompletionTime]",
                                    issueModel.CompletionTime.ToExport(context: context, column: column));
                                break;
                            case "WorkValue":
                                line = line.Replace(
                                    "[WorkValue]",
                                    issueModel.WorkValue.ToExport(context: context, column: column));
                                break;
                            case "ProgressRate":
                                line = line.Replace(
                                    "[ProgressRate]",
                                    issueModel.ProgressRate.ToExport(context: context, column: column));
                                break;
                            case "Status":
                                line = line.Replace(
                                    "[Status]",
                                    issueModel.Status.ToExport(context: context, column: column));
                                break;
                            case "Manager":
                                line = line.Replace(
                                    "[Manager]",
                                    issueModel.Manager.ToExport(context: context, column: column));
                                break;
                            case "Owner":
                                line = line.Replace(
                                    "[Owner]",
                                    issueModel.Owner.ToExport(context: context, column: column));
                                break;
                            case "ClassA":
                                line = line.Replace(
                                    "[ClassA]",
                                    issueModel.ClassA.ToExport(context: context, column: column));
                                break;
                            case "ClassB":
                                line = line.Replace(
                                    "[ClassB]",
                                    issueModel.ClassB.ToExport(context: context, column: column));
                                break;
                            case "ClassC":
                                line = line.Replace(
                                    "[ClassC]",
                                    issueModel.ClassC.ToExport(context: context, column: column));
                                break;
                            case "ClassD":
                                line = line.Replace(
                                    "[ClassD]",
                                    issueModel.ClassD.ToExport(context: context, column: column));
                                break;
                            case "ClassE":
                                line = line.Replace(
                                    "[ClassE]",
                                    issueModel.ClassE.ToExport(context: context, column: column));
                                break;
                            case "ClassF":
                                line = line.Replace(
                                    "[ClassF]",
                                    issueModel.ClassF.ToExport(context: context, column: column));
                                break;
                            case "ClassG":
                                line = line.Replace(
                                    "[ClassG]",
                                    issueModel.ClassG.ToExport(context: context, column: column));
                                break;
                            case "ClassH":
                                line = line.Replace(
                                    "[ClassH]",
                                    issueModel.ClassH.ToExport(context: context, column: column));
                                break;
                            case "ClassI":
                                line = line.Replace(
                                    "[ClassI]",
                                    issueModel.ClassI.ToExport(context: context, column: column));
                                break;
                            case "ClassJ":
                                line = line.Replace(
                                    "[ClassJ]",
                                    issueModel.ClassJ.ToExport(context: context, column: column));
                                break;
                            case "ClassK":
                                line = line.Replace(
                                    "[ClassK]",
                                    issueModel.ClassK.ToExport(context: context, column: column));
                                break;
                            case "ClassL":
                                line = line.Replace(
                                    "[ClassL]",
                                    issueModel.ClassL.ToExport(context: context, column: column));
                                break;
                            case "ClassM":
                                line = line.Replace(
                                    "[ClassM]",
                                    issueModel.ClassM.ToExport(context: context, column: column));
                                break;
                            case "ClassN":
                                line = line.Replace(
                                    "[ClassN]",
                                    issueModel.ClassN.ToExport(context: context, column: column));
                                break;
                            case "ClassO":
                                line = line.Replace(
                                    "[ClassO]",
                                    issueModel.ClassO.ToExport(context: context, column: column));
                                break;
                            case "ClassP":
                                line = line.Replace(
                                    "[ClassP]",
                                    issueModel.ClassP.ToExport(context: context, column: column));
                                break;
                            case "ClassQ":
                                line = line.Replace(
                                    "[ClassQ]",
                                    issueModel.ClassQ.ToExport(context: context, column: column));
                                break;
                            case "ClassR":
                                line = line.Replace(
                                    "[ClassR]",
                                    issueModel.ClassR.ToExport(context: context, column: column));
                                break;
                            case "ClassS":
                                line = line.Replace(
                                    "[ClassS]",
                                    issueModel.ClassS.ToExport(context: context, column: column));
                                break;
                            case "ClassT":
                                line = line.Replace(
                                    "[ClassT]",
                                    issueModel.ClassT.ToExport(context: context, column: column));
                                break;
                            case "ClassU":
                                line = line.Replace(
                                    "[ClassU]",
                                    issueModel.ClassU.ToExport(context: context, column: column));
                                break;
                            case "ClassV":
                                line = line.Replace(
                                    "[ClassV]",
                                    issueModel.ClassV.ToExport(context: context, column: column));
                                break;
                            case "ClassW":
                                line = line.Replace(
                                    "[ClassW]",
                                    issueModel.ClassW.ToExport(context: context, column: column));
                                break;
                            case "ClassX":
                                line = line.Replace(
                                    "[ClassX]",
                                    issueModel.ClassX.ToExport(context: context, column: column));
                                break;
                            case "ClassY":
                                line = line.Replace(
                                    "[ClassY]",
                                    issueModel.ClassY.ToExport(context: context, column: column));
                                break;
                            case "ClassZ":
                                line = line.Replace(
                                    "[ClassZ]",
                                    issueModel.ClassZ.ToExport(context: context, column: column));
                                break;
                            case "NumA":
                                line = line.Replace(
                                    "[NumA]",
                                    issueModel.NumA.ToExport(context: context, column: column));
                                break;
                            case "NumB":
                                line = line.Replace(
                                    "[NumB]",
                                    issueModel.NumB.ToExport(context: context, column: column));
                                break;
                            case "NumC":
                                line = line.Replace(
                                    "[NumC]",
                                    issueModel.NumC.ToExport(context: context, column: column));
                                break;
                            case "NumD":
                                line = line.Replace(
                                    "[NumD]",
                                    issueModel.NumD.ToExport(context: context, column: column));
                                break;
                            case "NumE":
                                line = line.Replace(
                                    "[NumE]",
                                    issueModel.NumE.ToExport(context: context, column: column));
                                break;
                            case "NumF":
                                line = line.Replace(
                                    "[NumF]",
                                    issueModel.NumF.ToExport(context: context, column: column));
                                break;
                            case "NumG":
                                line = line.Replace(
                                    "[NumG]",
                                    issueModel.NumG.ToExport(context: context, column: column));
                                break;
                            case "NumH":
                                line = line.Replace(
                                    "[NumH]",
                                    issueModel.NumH.ToExport(context: context, column: column));
                                break;
                            case "NumI":
                                line = line.Replace(
                                    "[NumI]",
                                    issueModel.NumI.ToExport(context: context, column: column));
                                break;
                            case "NumJ":
                                line = line.Replace(
                                    "[NumJ]",
                                    issueModel.NumJ.ToExport(context: context, column: column));
                                break;
                            case "NumK":
                                line = line.Replace(
                                    "[NumK]",
                                    issueModel.NumK.ToExport(context: context, column: column));
                                break;
                            case "NumL":
                                line = line.Replace(
                                    "[NumL]",
                                    issueModel.NumL.ToExport(context: context, column: column));
                                break;
                            case "NumM":
                                line = line.Replace(
                                    "[NumM]",
                                    issueModel.NumM.ToExport(context: context, column: column));
                                break;
                            case "NumN":
                                line = line.Replace(
                                    "[NumN]",
                                    issueModel.NumN.ToExport(context: context, column: column));
                                break;
                            case "NumO":
                                line = line.Replace(
                                    "[NumO]",
                                    issueModel.NumO.ToExport(context: context, column: column));
                                break;
                            case "NumP":
                                line = line.Replace(
                                    "[NumP]",
                                    issueModel.NumP.ToExport(context: context, column: column));
                                break;
                            case "NumQ":
                                line = line.Replace(
                                    "[NumQ]",
                                    issueModel.NumQ.ToExport(context: context, column: column));
                                break;
                            case "NumR":
                                line = line.Replace(
                                    "[NumR]",
                                    issueModel.NumR.ToExport(context: context, column: column));
                                break;
                            case "NumS":
                                line = line.Replace(
                                    "[NumS]",
                                    issueModel.NumS.ToExport(context: context, column: column));
                                break;
                            case "NumT":
                                line = line.Replace(
                                    "[NumT]",
                                    issueModel.NumT.ToExport(context: context, column: column));
                                break;
                            case "NumU":
                                line = line.Replace(
                                    "[NumU]",
                                    issueModel.NumU.ToExport(context: context, column: column));
                                break;
                            case "NumV":
                                line = line.Replace(
                                    "[NumV]",
                                    issueModel.NumV.ToExport(context: context, column: column));
                                break;
                            case "NumW":
                                line = line.Replace(
                                    "[NumW]",
                                    issueModel.NumW.ToExport(context: context, column: column));
                                break;
                            case "NumX":
                                line = line.Replace(
                                    "[NumX]",
                                    issueModel.NumX.ToExport(context: context, column: column));
                                break;
                            case "NumY":
                                line = line.Replace(
                                    "[NumY]",
                                    issueModel.NumY.ToExport(context: context, column: column));
                                break;
                            case "NumZ":
                                line = line.Replace(
                                    "[NumZ]",
                                    issueModel.NumZ.ToExport(context: context, column: column));
                                break;
                            case "DateA":
                                line = line.Replace(
                                    "[DateA]",
                                    issueModel.DateA.ToExport(context: context, column: column));
                                break;
                            case "DateB":
                                line = line.Replace(
                                    "[DateB]",
                                    issueModel.DateB.ToExport(context: context, column: column));
                                break;
                            case "DateC":
                                line = line.Replace(
                                    "[DateC]",
                                    issueModel.DateC.ToExport(context: context, column: column));
                                break;
                            case "DateD":
                                line = line.Replace(
                                    "[DateD]",
                                    issueModel.DateD.ToExport(context: context, column: column));
                                break;
                            case "DateE":
                                line = line.Replace(
                                    "[DateE]",
                                    issueModel.DateE.ToExport(context: context, column: column));
                                break;
                            case "DateF":
                                line = line.Replace(
                                    "[DateF]",
                                    issueModel.DateF.ToExport(context: context, column: column));
                                break;
                            case "DateG":
                                line = line.Replace(
                                    "[DateG]",
                                    issueModel.DateG.ToExport(context: context, column: column));
                                break;
                            case "DateH":
                                line = line.Replace(
                                    "[DateH]",
                                    issueModel.DateH.ToExport(context: context, column: column));
                                break;
                            case "DateI":
                                line = line.Replace(
                                    "[DateI]",
                                    issueModel.DateI.ToExport(context: context, column: column));
                                break;
                            case "DateJ":
                                line = line.Replace(
                                    "[DateJ]",
                                    issueModel.DateJ.ToExport(context: context, column: column));
                                break;
                            case "DateK":
                                line = line.Replace(
                                    "[DateK]",
                                    issueModel.DateK.ToExport(context: context, column: column));
                                break;
                            case "DateL":
                                line = line.Replace(
                                    "[DateL]",
                                    issueModel.DateL.ToExport(context: context, column: column));
                                break;
                            case "DateM":
                                line = line.Replace(
                                    "[DateM]",
                                    issueModel.DateM.ToExport(context: context, column: column));
                                break;
                            case "DateN":
                                line = line.Replace(
                                    "[DateN]",
                                    issueModel.DateN.ToExport(context: context, column: column));
                                break;
                            case "DateO":
                                line = line.Replace(
                                    "[DateO]",
                                    issueModel.DateO.ToExport(context: context, column: column));
                                break;
                            case "DateP":
                                line = line.Replace(
                                    "[DateP]",
                                    issueModel.DateP.ToExport(context: context, column: column));
                                break;
                            case "DateQ":
                                line = line.Replace(
                                    "[DateQ]",
                                    issueModel.DateQ.ToExport(context: context, column: column));
                                break;
                            case "DateR":
                                line = line.Replace(
                                    "[DateR]",
                                    issueModel.DateR.ToExport(context: context, column: column));
                                break;
                            case "DateS":
                                line = line.Replace(
                                    "[DateS]",
                                    issueModel.DateS.ToExport(context: context, column: column));
                                break;
                            case "DateT":
                                line = line.Replace(
                                    "[DateT]",
                                    issueModel.DateT.ToExport(context: context, column: column));
                                break;
                            case "DateU":
                                line = line.Replace(
                                    "[DateU]",
                                    issueModel.DateU.ToExport(context: context, column: column));
                                break;
                            case "DateV":
                                line = line.Replace(
                                    "[DateV]",
                                    issueModel.DateV.ToExport(context: context, column: column));
                                break;
                            case "DateW":
                                line = line.Replace(
                                    "[DateW]",
                                    issueModel.DateW.ToExport(context: context, column: column));
                                break;
                            case "DateX":
                                line = line.Replace(
                                    "[DateX]",
                                    issueModel.DateX.ToExport(context: context, column: column));
                                break;
                            case "DateY":
                                line = line.Replace(
                                    "[DateY]",
                                    issueModel.DateY.ToExport(context: context, column: column));
                                break;
                            case "DateZ":
                                line = line.Replace(
                                    "[DateZ]",
                                    issueModel.DateZ.ToExport(context: context, column: column));
                                break;
                            case "DescriptionA":
                                line = line.Replace(
                                    "[DescriptionA]",
                                    issueModel.DescriptionA.ToExport(context: context, column: column));
                                break;
                            case "DescriptionB":
                                line = line.Replace(
                                    "[DescriptionB]",
                                    issueModel.DescriptionB.ToExport(context: context, column: column));
                                break;
                            case "DescriptionC":
                                line = line.Replace(
                                    "[DescriptionC]",
                                    issueModel.DescriptionC.ToExport(context: context, column: column));
                                break;
                            case "DescriptionD":
                                line = line.Replace(
                                    "[DescriptionD]",
                                    issueModel.DescriptionD.ToExport(context: context, column: column));
                                break;
                            case "DescriptionE":
                                line = line.Replace(
                                    "[DescriptionE]",
                                    issueModel.DescriptionE.ToExport(context: context, column: column));
                                break;
                            case "DescriptionF":
                                line = line.Replace(
                                    "[DescriptionF]",
                                    issueModel.DescriptionF.ToExport(context: context, column: column));
                                break;
                            case "DescriptionG":
                                line = line.Replace(
                                    "[DescriptionG]",
                                    issueModel.DescriptionG.ToExport(context: context, column: column));
                                break;
                            case "DescriptionH":
                                line = line.Replace(
                                    "[DescriptionH]",
                                    issueModel.DescriptionH.ToExport(context: context, column: column));
                                break;
                            case "DescriptionI":
                                line = line.Replace(
                                    "[DescriptionI]",
                                    issueModel.DescriptionI.ToExport(context: context, column: column));
                                break;
                            case "DescriptionJ":
                                line = line.Replace(
                                    "[DescriptionJ]",
                                    issueModel.DescriptionJ.ToExport(context: context, column: column));
                                break;
                            case "DescriptionK":
                                line = line.Replace(
                                    "[DescriptionK]",
                                    issueModel.DescriptionK.ToExport(context: context, column: column));
                                break;
                            case "DescriptionL":
                                line = line.Replace(
                                    "[DescriptionL]",
                                    issueModel.DescriptionL.ToExport(context: context, column: column));
                                break;
                            case "DescriptionM":
                                line = line.Replace(
                                    "[DescriptionM]",
                                    issueModel.DescriptionM.ToExport(context: context, column: column));
                                break;
                            case "DescriptionN":
                                line = line.Replace(
                                    "[DescriptionN]",
                                    issueModel.DescriptionN.ToExport(context: context, column: column));
                                break;
                            case "DescriptionO":
                                line = line.Replace(
                                    "[DescriptionO]",
                                    issueModel.DescriptionO.ToExport(context: context, column: column));
                                break;
                            case "DescriptionP":
                                line = line.Replace(
                                    "[DescriptionP]",
                                    issueModel.DescriptionP.ToExport(context: context, column: column));
                                break;
                            case "DescriptionQ":
                                line = line.Replace(
                                    "[DescriptionQ]",
                                    issueModel.DescriptionQ.ToExport(context: context, column: column));
                                break;
                            case "DescriptionR":
                                line = line.Replace(
                                    "[DescriptionR]",
                                    issueModel.DescriptionR.ToExport(context: context, column: column));
                                break;
                            case "DescriptionS":
                                line = line.Replace(
                                    "[DescriptionS]",
                                    issueModel.DescriptionS.ToExport(context: context, column: column));
                                break;
                            case "DescriptionT":
                                line = line.Replace(
                                    "[DescriptionT]",
                                    issueModel.DescriptionT.ToExport(context: context, column: column));
                                break;
                            case "DescriptionU":
                                line = line.Replace(
                                    "[DescriptionU]",
                                    issueModel.DescriptionU.ToExport(context: context, column: column));
                                break;
                            case "DescriptionV":
                                line = line.Replace(
                                    "[DescriptionV]",
                                    issueModel.DescriptionV.ToExport(context: context, column: column));
                                break;
                            case "DescriptionW":
                                line = line.Replace(
                                    "[DescriptionW]",
                                    issueModel.DescriptionW.ToExport(context: context, column: column));
                                break;
                            case "DescriptionX":
                                line = line.Replace(
                                    "[DescriptionX]",
                                    issueModel.DescriptionX.ToExport(context: context, column: column));
                                break;
                            case "DescriptionY":
                                line = line.Replace(
                                    "[DescriptionY]",
                                    issueModel.DescriptionY.ToExport(context: context, column: column));
                                break;
                            case "DescriptionZ":
                                line = line.Replace(
                                    "[DescriptionZ]",
                                    issueModel.DescriptionZ.ToExport(context: context, column: column));
                                break;
                            case "CheckA":
                                line = line.Replace(
                                    "[CheckA]",
                                    issueModel.CheckA.ToExport(context: context, column: column));
                                break;
                            case "CheckB":
                                line = line.Replace(
                                    "[CheckB]",
                                    issueModel.CheckB.ToExport(context: context, column: column));
                                break;
                            case "CheckC":
                                line = line.Replace(
                                    "[CheckC]",
                                    issueModel.CheckC.ToExport(context: context, column: column));
                                break;
                            case "CheckD":
                                line = line.Replace(
                                    "[CheckD]",
                                    issueModel.CheckD.ToExport(context: context, column: column));
                                break;
                            case "CheckE":
                                line = line.Replace(
                                    "[CheckE]",
                                    issueModel.CheckE.ToExport(context: context, column: column));
                                break;
                            case "CheckF":
                                line = line.Replace(
                                    "[CheckF]",
                                    issueModel.CheckF.ToExport(context: context, column: column));
                                break;
                            case "CheckG":
                                line = line.Replace(
                                    "[CheckG]",
                                    issueModel.CheckG.ToExport(context: context, column: column));
                                break;
                            case "CheckH":
                                line = line.Replace(
                                    "[CheckH]",
                                    issueModel.CheckH.ToExport(context: context, column: column));
                                break;
                            case "CheckI":
                                line = line.Replace(
                                    "[CheckI]",
                                    issueModel.CheckI.ToExport(context: context, column: column));
                                break;
                            case "CheckJ":
                                line = line.Replace(
                                    "[CheckJ]",
                                    issueModel.CheckJ.ToExport(context: context, column: column));
                                break;
                            case "CheckK":
                                line = line.Replace(
                                    "[CheckK]",
                                    issueModel.CheckK.ToExport(context: context, column: column));
                                break;
                            case "CheckL":
                                line = line.Replace(
                                    "[CheckL]",
                                    issueModel.CheckL.ToExport(context: context, column: column));
                                break;
                            case "CheckM":
                                line = line.Replace(
                                    "[CheckM]",
                                    issueModel.CheckM.ToExport(context: context, column: column));
                                break;
                            case "CheckN":
                                line = line.Replace(
                                    "[CheckN]",
                                    issueModel.CheckN.ToExport(context: context, column: column));
                                break;
                            case "CheckO":
                                line = line.Replace(
                                    "[CheckO]",
                                    issueModel.CheckO.ToExport(context: context, column: column));
                                break;
                            case "CheckP":
                                line = line.Replace(
                                    "[CheckP]",
                                    issueModel.CheckP.ToExport(context: context, column: column));
                                break;
                            case "CheckQ":
                                line = line.Replace(
                                    "[CheckQ]",
                                    issueModel.CheckQ.ToExport(context: context, column: column));
                                break;
                            case "CheckR":
                                line = line.Replace(
                                    "[CheckR]",
                                    issueModel.CheckR.ToExport(context: context, column: column));
                                break;
                            case "CheckS":
                                line = line.Replace(
                                    "[CheckS]",
                                    issueModel.CheckS.ToExport(context: context, column: column));
                                break;
                            case "CheckT":
                                line = line.Replace(
                                    "[CheckT]",
                                    issueModel.CheckT.ToExport(context: context, column: column));
                                break;
                            case "CheckU":
                                line = line.Replace(
                                    "[CheckU]",
                                    issueModel.CheckU.ToExport(context: context, column: column));
                                break;
                            case "CheckV":
                                line = line.Replace(
                                    "[CheckV]",
                                    issueModel.CheckV.ToExport(context: context, column: column));
                                break;
                            case "CheckW":
                                line = line.Replace(
                                    "[CheckW]",
                                    issueModel.CheckW.ToExport(context: context, column: column));
                                break;
                            case "CheckX":
                                line = line.Replace(
                                    "[CheckX]",
                                    issueModel.CheckX.ToExport(context: context, column: column));
                                break;
                            case "CheckY":
                                line = line.Replace(
                                    "[CheckY]",
                                    issueModel.CheckY.ToExport(context: context, column: column));
                                break;
                            case "CheckZ":
                                line = line.Replace(
                                    "[CheckZ]",
                                    issueModel.CheckZ.ToExport(context: context, column: column));
                                break;
                            case "Comments":
                                line = line.Replace(
                                    "[Comments]",
                                    issueModel.Comments.ToExport(context: context, column: column));
                                break;
                            case "Creator":
                                line = line.Replace(
                                    "[Creator]",
                                    issueModel.Creator.ToExport(context: context, column: column));
                                break;
                            case "Updator":
                                line = line.Replace(
                                    "[Updator]",
                                    issueModel.Updator.ToExport(context: context, column: column));
                                break;
                            case "CreatedTime":
                                line = line.Replace(
                                    "[CreatedTime]",
                                    issueModel.CreatedTime.ToExport(context: context, column: column));
                                break;
                        }
                    });
                    break;
                case "Results":
                    var resultModel = new ResultModel(
                        context: context, ss: ss, dataRow: dataRow);
                    ss.IncludedColumns(Line).ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "Title":
                                line = line.Replace("[Title]", dataRow.String("ItemTitle"));
                                break;
                            case "SiteId":
                                line = line.Replace(
                                    "[SiteId]",
                                    resultModel.SiteId.ToExport(context: context, column: column));
                                break;
                            case "UpdatedTime":
                                line = line.Replace(
                                    "[UpdatedTime]",
                                    resultModel.UpdatedTime.ToExport(context: context, column: column));
                                break;
                            case "ResultId":
                                line = line.Replace(
                                    "[ResultId]",
                                    resultModel.ResultId.ToExport(context: context, column: column));
                                break;
                            case "Ver":
                                line = line.Replace(
                                    "[Ver]",
                                    resultModel.Ver.ToExport(context: context, column: column));
                                break;
                            case "Body":
                                line = line.Replace(
                                    "[Body]",
                                    resultModel.Body.ToExport(context: context, column: column));
                                break;
                            case "Status":
                                line = line.Replace(
                                    "[Status]",
                                    resultModel.Status.ToExport(context: context, column: column));
                                break;
                            case "Manager":
                                line = line.Replace(
                                    "[Manager]",
                                    resultModel.Manager.ToExport(context: context, column: column));
                                break;
                            case "Owner":
                                line = line.Replace(
                                    "[Owner]",
                                    resultModel.Owner.ToExport(context: context, column: column));
                                break;
                            case "ClassA":
                                line = line.Replace(
                                    "[ClassA]",
                                    resultModel.ClassA.ToExport(context: context, column: column));
                                break;
                            case "ClassB":
                                line = line.Replace(
                                    "[ClassB]",
                                    resultModel.ClassB.ToExport(context: context, column: column));
                                break;
                            case "ClassC":
                                line = line.Replace(
                                    "[ClassC]",
                                    resultModel.ClassC.ToExport(context: context, column: column));
                                break;
                            case "ClassD":
                                line = line.Replace(
                                    "[ClassD]",
                                    resultModel.ClassD.ToExport(context: context, column: column));
                                break;
                            case "ClassE":
                                line = line.Replace(
                                    "[ClassE]",
                                    resultModel.ClassE.ToExport(context: context, column: column));
                                break;
                            case "ClassF":
                                line = line.Replace(
                                    "[ClassF]",
                                    resultModel.ClassF.ToExport(context: context, column: column));
                                break;
                            case "ClassG":
                                line = line.Replace(
                                    "[ClassG]",
                                    resultModel.ClassG.ToExport(context: context, column: column));
                                break;
                            case "ClassH":
                                line = line.Replace(
                                    "[ClassH]",
                                    resultModel.ClassH.ToExport(context: context, column: column));
                                break;
                            case "ClassI":
                                line = line.Replace(
                                    "[ClassI]",
                                    resultModel.ClassI.ToExport(context: context, column: column));
                                break;
                            case "ClassJ":
                                line = line.Replace(
                                    "[ClassJ]",
                                    resultModel.ClassJ.ToExport(context: context, column: column));
                                break;
                            case "ClassK":
                                line = line.Replace(
                                    "[ClassK]",
                                    resultModel.ClassK.ToExport(context: context, column: column));
                                break;
                            case "ClassL":
                                line = line.Replace(
                                    "[ClassL]",
                                    resultModel.ClassL.ToExport(context: context, column: column));
                                break;
                            case "ClassM":
                                line = line.Replace(
                                    "[ClassM]",
                                    resultModel.ClassM.ToExport(context: context, column: column));
                                break;
                            case "ClassN":
                                line = line.Replace(
                                    "[ClassN]",
                                    resultModel.ClassN.ToExport(context: context, column: column));
                                break;
                            case "ClassO":
                                line = line.Replace(
                                    "[ClassO]",
                                    resultModel.ClassO.ToExport(context: context, column: column));
                                break;
                            case "ClassP":
                                line = line.Replace(
                                    "[ClassP]",
                                    resultModel.ClassP.ToExport(context: context, column: column));
                                break;
                            case "ClassQ":
                                line = line.Replace(
                                    "[ClassQ]",
                                    resultModel.ClassQ.ToExport(context: context, column: column));
                                break;
                            case "ClassR":
                                line = line.Replace(
                                    "[ClassR]",
                                    resultModel.ClassR.ToExport(context: context, column: column));
                                break;
                            case "ClassS":
                                line = line.Replace(
                                    "[ClassS]",
                                    resultModel.ClassS.ToExport(context: context, column: column));
                                break;
                            case "ClassT":
                                line = line.Replace(
                                    "[ClassT]",
                                    resultModel.ClassT.ToExport(context: context, column: column));
                                break;
                            case "ClassU":
                                line = line.Replace(
                                    "[ClassU]",
                                    resultModel.ClassU.ToExport(context: context, column: column));
                                break;
                            case "ClassV":
                                line = line.Replace(
                                    "[ClassV]",
                                    resultModel.ClassV.ToExport(context: context, column: column));
                                break;
                            case "ClassW":
                                line = line.Replace(
                                    "[ClassW]",
                                    resultModel.ClassW.ToExport(context: context, column: column));
                                break;
                            case "ClassX":
                                line = line.Replace(
                                    "[ClassX]",
                                    resultModel.ClassX.ToExport(context: context, column: column));
                                break;
                            case "ClassY":
                                line = line.Replace(
                                    "[ClassY]",
                                    resultModel.ClassY.ToExport(context: context, column: column));
                                break;
                            case "ClassZ":
                                line = line.Replace(
                                    "[ClassZ]",
                                    resultModel.ClassZ.ToExport(context: context, column: column));
                                break;
                            case "NumA":
                                line = line.Replace(
                                    "[NumA]",
                                    resultModel.NumA.ToExport(context: context, column: column));
                                break;
                            case "NumB":
                                line = line.Replace(
                                    "[NumB]",
                                    resultModel.NumB.ToExport(context: context, column: column));
                                break;
                            case "NumC":
                                line = line.Replace(
                                    "[NumC]",
                                    resultModel.NumC.ToExport(context: context, column: column));
                                break;
                            case "NumD":
                                line = line.Replace(
                                    "[NumD]",
                                    resultModel.NumD.ToExport(context: context, column: column));
                                break;
                            case "NumE":
                                line = line.Replace(
                                    "[NumE]",
                                    resultModel.NumE.ToExport(context: context, column: column));
                                break;
                            case "NumF":
                                line = line.Replace(
                                    "[NumF]",
                                    resultModel.NumF.ToExport(context: context, column: column));
                                break;
                            case "NumG":
                                line = line.Replace(
                                    "[NumG]",
                                    resultModel.NumG.ToExport(context: context, column: column));
                                break;
                            case "NumH":
                                line = line.Replace(
                                    "[NumH]",
                                    resultModel.NumH.ToExport(context: context, column: column));
                                break;
                            case "NumI":
                                line = line.Replace(
                                    "[NumI]",
                                    resultModel.NumI.ToExport(context: context, column: column));
                                break;
                            case "NumJ":
                                line = line.Replace(
                                    "[NumJ]",
                                    resultModel.NumJ.ToExport(context: context, column: column));
                                break;
                            case "NumK":
                                line = line.Replace(
                                    "[NumK]",
                                    resultModel.NumK.ToExport(context: context, column: column));
                                break;
                            case "NumL":
                                line = line.Replace(
                                    "[NumL]",
                                    resultModel.NumL.ToExport(context: context, column: column));
                                break;
                            case "NumM":
                                line = line.Replace(
                                    "[NumM]",
                                    resultModel.NumM.ToExport(context: context, column: column));
                                break;
                            case "NumN":
                                line = line.Replace(
                                    "[NumN]",
                                    resultModel.NumN.ToExport(context: context, column: column));
                                break;
                            case "NumO":
                                line = line.Replace(
                                    "[NumO]",
                                    resultModel.NumO.ToExport(context: context, column: column));
                                break;
                            case "NumP":
                                line = line.Replace(
                                    "[NumP]",
                                    resultModel.NumP.ToExport(context: context, column: column));
                                break;
                            case "NumQ":
                                line = line.Replace(
                                    "[NumQ]",
                                    resultModel.NumQ.ToExport(context: context, column: column));
                                break;
                            case "NumR":
                                line = line.Replace(
                                    "[NumR]",
                                    resultModel.NumR.ToExport(context: context, column: column));
                                break;
                            case "NumS":
                                line = line.Replace(
                                    "[NumS]",
                                    resultModel.NumS.ToExport(context: context, column: column));
                                break;
                            case "NumT":
                                line = line.Replace(
                                    "[NumT]",
                                    resultModel.NumT.ToExport(context: context, column: column));
                                break;
                            case "NumU":
                                line = line.Replace(
                                    "[NumU]",
                                    resultModel.NumU.ToExport(context: context, column: column));
                                break;
                            case "NumV":
                                line = line.Replace(
                                    "[NumV]",
                                    resultModel.NumV.ToExport(context: context, column: column));
                                break;
                            case "NumW":
                                line = line.Replace(
                                    "[NumW]",
                                    resultModel.NumW.ToExport(context: context, column: column));
                                break;
                            case "NumX":
                                line = line.Replace(
                                    "[NumX]",
                                    resultModel.NumX.ToExport(context: context, column: column));
                                break;
                            case "NumY":
                                line = line.Replace(
                                    "[NumY]",
                                    resultModel.NumY.ToExport(context: context, column: column));
                                break;
                            case "NumZ":
                                line = line.Replace(
                                    "[NumZ]",
                                    resultModel.NumZ.ToExport(context: context, column: column));
                                break;
                            case "DateA":
                                line = line.Replace(
                                    "[DateA]",
                                    resultModel.DateA.ToExport(context: context, column: column));
                                break;
                            case "DateB":
                                line = line.Replace(
                                    "[DateB]",
                                    resultModel.DateB.ToExport(context: context, column: column));
                                break;
                            case "DateC":
                                line = line.Replace(
                                    "[DateC]",
                                    resultModel.DateC.ToExport(context: context, column: column));
                                break;
                            case "DateD":
                                line = line.Replace(
                                    "[DateD]",
                                    resultModel.DateD.ToExport(context: context, column: column));
                                break;
                            case "DateE":
                                line = line.Replace(
                                    "[DateE]",
                                    resultModel.DateE.ToExport(context: context, column: column));
                                break;
                            case "DateF":
                                line = line.Replace(
                                    "[DateF]",
                                    resultModel.DateF.ToExport(context: context, column: column));
                                break;
                            case "DateG":
                                line = line.Replace(
                                    "[DateG]",
                                    resultModel.DateG.ToExport(context: context, column: column));
                                break;
                            case "DateH":
                                line = line.Replace(
                                    "[DateH]",
                                    resultModel.DateH.ToExport(context: context, column: column));
                                break;
                            case "DateI":
                                line = line.Replace(
                                    "[DateI]",
                                    resultModel.DateI.ToExport(context: context, column: column));
                                break;
                            case "DateJ":
                                line = line.Replace(
                                    "[DateJ]",
                                    resultModel.DateJ.ToExport(context: context, column: column));
                                break;
                            case "DateK":
                                line = line.Replace(
                                    "[DateK]",
                                    resultModel.DateK.ToExport(context: context, column: column));
                                break;
                            case "DateL":
                                line = line.Replace(
                                    "[DateL]",
                                    resultModel.DateL.ToExport(context: context, column: column));
                                break;
                            case "DateM":
                                line = line.Replace(
                                    "[DateM]",
                                    resultModel.DateM.ToExport(context: context, column: column));
                                break;
                            case "DateN":
                                line = line.Replace(
                                    "[DateN]",
                                    resultModel.DateN.ToExport(context: context, column: column));
                                break;
                            case "DateO":
                                line = line.Replace(
                                    "[DateO]",
                                    resultModel.DateO.ToExport(context: context, column: column));
                                break;
                            case "DateP":
                                line = line.Replace(
                                    "[DateP]",
                                    resultModel.DateP.ToExport(context: context, column: column));
                                break;
                            case "DateQ":
                                line = line.Replace(
                                    "[DateQ]",
                                    resultModel.DateQ.ToExport(context: context, column: column));
                                break;
                            case "DateR":
                                line = line.Replace(
                                    "[DateR]",
                                    resultModel.DateR.ToExport(context: context, column: column));
                                break;
                            case "DateS":
                                line = line.Replace(
                                    "[DateS]",
                                    resultModel.DateS.ToExport(context: context, column: column));
                                break;
                            case "DateT":
                                line = line.Replace(
                                    "[DateT]",
                                    resultModel.DateT.ToExport(context: context, column: column));
                                break;
                            case "DateU":
                                line = line.Replace(
                                    "[DateU]",
                                    resultModel.DateU.ToExport(context: context, column: column));
                                break;
                            case "DateV":
                                line = line.Replace(
                                    "[DateV]",
                                    resultModel.DateV.ToExport(context: context, column: column));
                                break;
                            case "DateW":
                                line = line.Replace(
                                    "[DateW]",
                                    resultModel.DateW.ToExport(context: context, column: column));
                                break;
                            case "DateX":
                                line = line.Replace(
                                    "[DateX]",
                                    resultModel.DateX.ToExport(context: context, column: column));
                                break;
                            case "DateY":
                                line = line.Replace(
                                    "[DateY]",
                                    resultModel.DateY.ToExport(context: context, column: column));
                                break;
                            case "DateZ":
                                line = line.Replace(
                                    "[DateZ]",
                                    resultModel.DateZ.ToExport(context: context, column: column));
                                break;
                            case "DescriptionA":
                                line = line.Replace(
                                    "[DescriptionA]",
                                    resultModel.DescriptionA.ToExport(context: context, column: column));
                                break;
                            case "DescriptionB":
                                line = line.Replace(
                                    "[DescriptionB]",
                                    resultModel.DescriptionB.ToExport(context: context, column: column));
                                break;
                            case "DescriptionC":
                                line = line.Replace(
                                    "[DescriptionC]",
                                    resultModel.DescriptionC.ToExport(context: context, column: column));
                                break;
                            case "DescriptionD":
                                line = line.Replace(
                                    "[DescriptionD]",
                                    resultModel.DescriptionD.ToExport(context: context, column: column));
                                break;
                            case "DescriptionE":
                                line = line.Replace(
                                    "[DescriptionE]",
                                    resultModel.DescriptionE.ToExport(context: context, column: column));
                                break;
                            case "DescriptionF":
                                line = line.Replace(
                                    "[DescriptionF]",
                                    resultModel.DescriptionF.ToExport(context: context, column: column));
                                break;
                            case "DescriptionG":
                                line = line.Replace(
                                    "[DescriptionG]",
                                    resultModel.DescriptionG.ToExport(context: context, column: column));
                                break;
                            case "DescriptionH":
                                line = line.Replace(
                                    "[DescriptionH]",
                                    resultModel.DescriptionH.ToExport(context: context, column: column));
                                break;
                            case "DescriptionI":
                                line = line.Replace(
                                    "[DescriptionI]",
                                    resultModel.DescriptionI.ToExport(context: context, column: column));
                                break;
                            case "DescriptionJ":
                                line = line.Replace(
                                    "[DescriptionJ]",
                                    resultModel.DescriptionJ.ToExport(context: context, column: column));
                                break;
                            case "DescriptionK":
                                line = line.Replace(
                                    "[DescriptionK]",
                                    resultModel.DescriptionK.ToExport(context: context, column: column));
                                break;
                            case "DescriptionL":
                                line = line.Replace(
                                    "[DescriptionL]",
                                    resultModel.DescriptionL.ToExport(context: context, column: column));
                                break;
                            case "DescriptionM":
                                line = line.Replace(
                                    "[DescriptionM]",
                                    resultModel.DescriptionM.ToExport(context: context, column: column));
                                break;
                            case "DescriptionN":
                                line = line.Replace(
                                    "[DescriptionN]",
                                    resultModel.DescriptionN.ToExport(context: context, column: column));
                                break;
                            case "DescriptionO":
                                line = line.Replace(
                                    "[DescriptionO]",
                                    resultModel.DescriptionO.ToExport(context: context, column: column));
                                break;
                            case "DescriptionP":
                                line = line.Replace(
                                    "[DescriptionP]",
                                    resultModel.DescriptionP.ToExport(context: context, column: column));
                                break;
                            case "DescriptionQ":
                                line = line.Replace(
                                    "[DescriptionQ]",
                                    resultModel.DescriptionQ.ToExport(context: context, column: column));
                                break;
                            case "DescriptionR":
                                line = line.Replace(
                                    "[DescriptionR]",
                                    resultModel.DescriptionR.ToExport(context: context, column: column));
                                break;
                            case "DescriptionS":
                                line = line.Replace(
                                    "[DescriptionS]",
                                    resultModel.DescriptionS.ToExport(context: context, column: column));
                                break;
                            case "DescriptionT":
                                line = line.Replace(
                                    "[DescriptionT]",
                                    resultModel.DescriptionT.ToExport(context: context, column: column));
                                break;
                            case "DescriptionU":
                                line = line.Replace(
                                    "[DescriptionU]",
                                    resultModel.DescriptionU.ToExport(context: context, column: column));
                                break;
                            case "DescriptionV":
                                line = line.Replace(
                                    "[DescriptionV]",
                                    resultModel.DescriptionV.ToExport(context: context, column: column));
                                break;
                            case "DescriptionW":
                                line = line.Replace(
                                    "[DescriptionW]",
                                    resultModel.DescriptionW.ToExport(context: context, column: column));
                                break;
                            case "DescriptionX":
                                line = line.Replace(
                                    "[DescriptionX]",
                                    resultModel.DescriptionX.ToExport(context: context, column: column));
                                break;
                            case "DescriptionY":
                                line = line.Replace(
                                    "[DescriptionY]",
                                    resultModel.DescriptionY.ToExport(context: context, column: column));
                                break;
                            case "DescriptionZ":
                                line = line.Replace(
                                    "[DescriptionZ]",
                                    resultModel.DescriptionZ.ToExport(context: context, column: column));
                                break;
                            case "CheckA":
                                line = line.Replace(
                                    "[CheckA]",
                                    resultModel.CheckA.ToExport(context: context, column: column));
                                break;
                            case "CheckB":
                                line = line.Replace(
                                    "[CheckB]",
                                    resultModel.CheckB.ToExport(context: context, column: column));
                                break;
                            case "CheckC":
                                line = line.Replace(
                                    "[CheckC]",
                                    resultModel.CheckC.ToExport(context: context, column: column));
                                break;
                            case "CheckD":
                                line = line.Replace(
                                    "[CheckD]",
                                    resultModel.CheckD.ToExport(context: context, column: column));
                                break;
                            case "CheckE":
                                line = line.Replace(
                                    "[CheckE]",
                                    resultModel.CheckE.ToExport(context: context, column: column));
                                break;
                            case "CheckF":
                                line = line.Replace(
                                    "[CheckF]",
                                    resultModel.CheckF.ToExport(context: context, column: column));
                                break;
                            case "CheckG":
                                line = line.Replace(
                                    "[CheckG]",
                                    resultModel.CheckG.ToExport(context: context, column: column));
                                break;
                            case "CheckH":
                                line = line.Replace(
                                    "[CheckH]",
                                    resultModel.CheckH.ToExport(context: context, column: column));
                                break;
                            case "CheckI":
                                line = line.Replace(
                                    "[CheckI]",
                                    resultModel.CheckI.ToExport(context: context, column: column));
                                break;
                            case "CheckJ":
                                line = line.Replace(
                                    "[CheckJ]",
                                    resultModel.CheckJ.ToExport(context: context, column: column));
                                break;
                            case "CheckK":
                                line = line.Replace(
                                    "[CheckK]",
                                    resultModel.CheckK.ToExport(context: context, column: column));
                                break;
                            case "CheckL":
                                line = line.Replace(
                                    "[CheckL]",
                                    resultModel.CheckL.ToExport(context: context, column: column));
                                break;
                            case "CheckM":
                                line = line.Replace(
                                    "[CheckM]",
                                    resultModel.CheckM.ToExport(context: context, column: column));
                                break;
                            case "CheckN":
                                line = line.Replace(
                                    "[CheckN]",
                                    resultModel.CheckN.ToExport(context: context, column: column));
                                break;
                            case "CheckO":
                                line = line.Replace(
                                    "[CheckO]",
                                    resultModel.CheckO.ToExport(context: context, column: column));
                                break;
                            case "CheckP":
                                line = line.Replace(
                                    "[CheckP]",
                                    resultModel.CheckP.ToExport(context: context, column: column));
                                break;
                            case "CheckQ":
                                line = line.Replace(
                                    "[CheckQ]",
                                    resultModel.CheckQ.ToExport(context: context, column: column));
                                break;
                            case "CheckR":
                                line = line.Replace(
                                    "[CheckR]",
                                    resultModel.CheckR.ToExport(context: context, column: column));
                                break;
                            case "CheckS":
                                line = line.Replace(
                                    "[CheckS]",
                                    resultModel.CheckS.ToExport(context: context, column: column));
                                break;
                            case "CheckT":
                                line = line.Replace(
                                    "[CheckT]",
                                    resultModel.CheckT.ToExport(context: context, column: column));
                                break;
                            case "CheckU":
                                line = line.Replace(
                                    "[CheckU]",
                                    resultModel.CheckU.ToExport(context: context, column: column));
                                break;
                            case "CheckV":
                                line = line.Replace(
                                    "[CheckV]",
                                    resultModel.CheckV.ToExport(context: context, column: column));
                                break;
                            case "CheckW":
                                line = line.Replace(
                                    "[CheckW]",
                                    resultModel.CheckW.ToExport(context: context, column: column));
                                break;
                            case "CheckX":
                                line = line.Replace(
                                    "[CheckX]",
                                    resultModel.CheckX.ToExport(context: context, column: column));
                                break;
                            case "CheckY":
                                line = line.Replace(
                                    "[CheckY]",
                                    resultModel.CheckY.ToExport(context: context, column: column));
                                break;
                            case "CheckZ":
                                line = line.Replace(
                                    "[CheckZ]",
                                    resultModel.CheckZ.ToExport(context: context, column: column));
                                break;
                            case "Comments":
                                line = line.Replace(
                                    "[Comments]",
                                    resultModel.Comments.ToExport(context: context, column: column));
                                break;
                            case "Creator":
                                line = line.Replace(
                                    "[Creator]",
                                    resultModel.Creator.ToExport(context: context, column: column));
                                break;
                            case "Updator":
                                line = line.Replace(
                                    "[Updator]",
                                    resultModel.Updator.ToExport(context: context, column: column));
                                break;
                            case "CreatedTime":
                                line = line.Replace(
                                    "[CreatedTime]",
                                    resultModel.CreatedTime.ToExport(context: context, column: column));
                                break;
                        }
                    });
                    break;
            }
            return line;
        }
    }
}
