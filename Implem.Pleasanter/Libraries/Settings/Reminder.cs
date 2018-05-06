using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
            Body = BodyPlaceholder;
            Line = Parameters.Reminder.DefaultLine;
            StartDateTime = DateTime.Now.ToLocal().Date.AddDays(1);
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

        public void Remind(SiteSettings ss, bool test = false)
        {
            ss.SetChoiceHash(withLink: true, all: true);
            var dataSet = GetDataSet(ss);
            if (Rds.Count(dataSet) > 0 || NotSendIfNotApplicable != true)
            {
                new OutgoingMailModel()
                {
                    Title = GetSubject(test),
                    Body = GetBody(ss, dataSet),
                    From = new MailAddress(From),
                    To = To
                }.Send();
            }
            if (!test)
            {
                Rds.ExecuteNonQuery(statements:
                    Rds.UpdateReminderSchedules(
                        param: Rds.ReminderSchedulesParam()
                            .ScheduledTime(StartDateTime.Next(Type)),
                        where: Rds.ReminderSchedulesWhere()
                            .SiteId(ss.SiteId)
                            .Id(Id)));
            }
        }

        private Title GetSubject(bool test)
        {
            return new Title((test
                ? "(" + Displays.Test() + ")"
                : string.Empty)
                    + Subject);
        }

        private string GetBody(SiteSettings ss, DataSet dataSet)
        {
            var sb = new StringBuilder();
            var timeGroups = dataSet.Tables["Main"]
                .AsEnumerable()
                .GroupBy(dataRow => dataRow.DateTime(Column).Date)
                .ToList();
            timeGroups.ForEach(timeGroup =>
            {
                var date = timeGroup.First().DateTime(Column).ToLocal().Date;
                switch (Column)
                {
                    case "CompletionTime":
                        date = date.AddDifferenceOfDates(
                            ss.GetColumn("CompletionTime")?.EditorFormat, minus: true);
                        break;
                }
                sb.Append("{0} ({1})\n".Params(
                    date.ToString(Displays.Get("YmdaFormat"), Sessions.CultureInfo()),
                    Relative(date)));
                timeGroup
                    .OrderBy(dataRow => dataRow.Int("Status"))
                    .ForEach(dataRow =>
                        sb.Append(
                            "\t",
                            ReplacedLine(ss, dataRow),
                            "\n\t",
                            Locations.ItemEditAbsoluteUri(
                                dataRow.Long(Rds.IdColumn(ss.ReferenceType))),
                            "\n"));
                sb.Append("\n");
            });
            if (!timeGroups.Any())
            {
                sb.Append(Displays.NoTargetRecord(), "\r\n");
            }
            return Body.Contains(BodyPlaceholder)
                ? Body.Replace(BodyPlaceholder, sb.ToString())
                : Body + "\n" + sb.ToString();
        }

        private DataSet GetDataSet(SiteSettings ss)
        {
            var orderByColumn = ss.GetColumn(Column);
            var column = new SqlColumnCollection()
                .Add(ss, ss.GetColumn(Rds.IdColumn(ss.ReferenceType)))
                .Add(ss, orderByColumn)
                .ItemTitle(ss.ReferenceType, Rds.IdColumn(ss.ReferenceType));
            var columns = ss.IncludedColumns(Line).ToList();
            columns.ForEach(o => column.Add(ss, o));
            if (columns.Any(o => o.ColumnName == "Status"))
            {
                columns.Add(ss.GetColumn("Status"));
            }
            var view = ss.Views?.Get(Condition) ?? new View();
            var orderBy = new SqlOrderByCollection()
                .Add(ss, orderByColumn, SqlOrderBy.Types.desc);
            var dataSet = Rds.ExecuteDataSet(statements:
                Rds.Select(
                    tableName: ss.ReferenceType,
                    dataTableName: "Main",
                    column: column,
                    where: view.Where(ss, checkPermission: false)
                        .Add(
                            tableName: ss.ReferenceType,
                            columnBrackets: new string[] { orderByColumn.ColumnName },
                            _operator: "<'{0}'".Params(DateTime.Now.ToLocal().Date.AddDays(Range)))
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
                                _operator: "<'{0}'".Params(DateTime.Now.ToLocal().Date),
                                _using: SendCompletedInPast == true)),
                    orderBy: new SqlOrderByCollection().Add(ss, ss.GetColumn(Column)),
                    pageSize: Parameters.Reminder.Limit,
                    countRecord: true));
            return dataSet;
        }

        private string Relative(DateTime time)
        {
            var diff = DateTime.Now.ToLocal().Date - time;
            if (diff.TotalDays == 0)
            {
                return Displays.Today();
            }
            else if (diff.TotalDays < 0)
            {
                return Displays.LimitAfterDays((diff.Days * -1).ToString());
            }
            else
            {
                return Displays.LimitBeforeDays(diff.Days.ToString());
            }
        }

        public Reminder GetRecordingData()
        {
            var reminder = new Reminder();
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

        private string ReplacedLine(SiteSettings ss, DataRow dataRow)
        {
            var line = Line;
            switch (ss.ReferenceType)
            {
                case "Issues":
                    var issueModel = new IssueModel(ss, dataRow);
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
                                    issueModel.SiteId.ToExport(column));
                                break;
                            case "UpdatedTime":
                                line = line.Replace(
                                    "[UpdatedTime]",
                                    issueModel.UpdatedTime.ToExport(column));
                                break;
                            case "IssueId":
                                line = line.Replace(
                                    "[IssueId]",
                                    issueModel.IssueId.ToExport(column));
                                break;
                            case "Ver":
                                line = line.Replace(
                                    "[Ver]",
                                    issueModel.Ver.ToExport(column));
                                break;
                            case "Body":
                                line = line.Replace(
                                    "[Body]",
                                    issueModel.Body.ToExport(column));
                                break;
                            case "StartTime":
                                line = line.Replace(
                                    "[StartTime]",
                                    issueModel.StartTime.ToExport(column));
                                break;
                            case "CompletionTime":
                                line = line.Replace(
                                    "[CompletionTime]",
                                    issueModel.CompletionTime.ToExport(column));
                                break;
                            case "WorkValue":
                                line = line.Replace(
                                    "[WorkValue]",
                                    issueModel.WorkValue.ToExport(column));
                                break;
                            case "ProgressRate":
                                line = line.Replace(
                                    "[ProgressRate]",
                                    issueModel.ProgressRate.ToExport(column));
                                break;
                            case "Status":
                                line = line.Replace(
                                    "[Status]",
                                    issueModel.Status.ToExport(column));
                                break;
                            case "Manager":
                                line = line.Replace(
                                    "[Manager]",
                                    issueModel.Manager.ToExport(column));
                                break;
                            case "Owner":
                                line = line.Replace(
                                    "[Owner]",
                                    issueModel.Owner.ToExport(column));
                                break;
                            case "ClassA":
                                line = line.Replace(
                                    "[ClassA]",
                                    issueModel.ClassA.ToExport(column));
                                break;
                            case "ClassB":
                                line = line.Replace(
                                    "[ClassB]",
                                    issueModel.ClassB.ToExport(column));
                                break;
                            case "ClassC":
                                line = line.Replace(
                                    "[ClassC]",
                                    issueModel.ClassC.ToExport(column));
                                break;
                            case "ClassD":
                                line = line.Replace(
                                    "[ClassD]",
                                    issueModel.ClassD.ToExport(column));
                                break;
                            case "ClassE":
                                line = line.Replace(
                                    "[ClassE]",
                                    issueModel.ClassE.ToExport(column));
                                break;
                            case "ClassF":
                                line = line.Replace(
                                    "[ClassF]",
                                    issueModel.ClassF.ToExport(column));
                                break;
                            case "ClassG":
                                line = line.Replace(
                                    "[ClassG]",
                                    issueModel.ClassG.ToExport(column));
                                break;
                            case "ClassH":
                                line = line.Replace(
                                    "[ClassH]",
                                    issueModel.ClassH.ToExport(column));
                                break;
                            case "ClassI":
                                line = line.Replace(
                                    "[ClassI]",
                                    issueModel.ClassI.ToExport(column));
                                break;
                            case "ClassJ":
                                line = line.Replace(
                                    "[ClassJ]",
                                    issueModel.ClassJ.ToExport(column));
                                break;
                            case "ClassK":
                                line = line.Replace(
                                    "[ClassK]",
                                    issueModel.ClassK.ToExport(column));
                                break;
                            case "ClassL":
                                line = line.Replace(
                                    "[ClassL]",
                                    issueModel.ClassL.ToExport(column));
                                break;
                            case "ClassM":
                                line = line.Replace(
                                    "[ClassM]",
                                    issueModel.ClassM.ToExport(column));
                                break;
                            case "ClassN":
                                line = line.Replace(
                                    "[ClassN]",
                                    issueModel.ClassN.ToExport(column));
                                break;
                            case "ClassO":
                                line = line.Replace(
                                    "[ClassO]",
                                    issueModel.ClassO.ToExport(column));
                                break;
                            case "ClassP":
                                line = line.Replace(
                                    "[ClassP]",
                                    issueModel.ClassP.ToExport(column));
                                break;
                            case "ClassQ":
                                line = line.Replace(
                                    "[ClassQ]",
                                    issueModel.ClassQ.ToExport(column));
                                break;
                            case "ClassR":
                                line = line.Replace(
                                    "[ClassR]",
                                    issueModel.ClassR.ToExport(column));
                                break;
                            case "ClassS":
                                line = line.Replace(
                                    "[ClassS]",
                                    issueModel.ClassS.ToExport(column));
                                break;
                            case "ClassT":
                                line = line.Replace(
                                    "[ClassT]",
                                    issueModel.ClassT.ToExport(column));
                                break;
                            case "ClassU":
                                line = line.Replace(
                                    "[ClassU]",
                                    issueModel.ClassU.ToExport(column));
                                break;
                            case "ClassV":
                                line = line.Replace(
                                    "[ClassV]",
                                    issueModel.ClassV.ToExport(column));
                                break;
                            case "ClassW":
                                line = line.Replace(
                                    "[ClassW]",
                                    issueModel.ClassW.ToExport(column));
                                break;
                            case "ClassX":
                                line = line.Replace(
                                    "[ClassX]",
                                    issueModel.ClassX.ToExport(column));
                                break;
                            case "ClassY":
                                line = line.Replace(
                                    "[ClassY]",
                                    issueModel.ClassY.ToExport(column));
                                break;
                            case "ClassZ":
                                line = line.Replace(
                                    "[ClassZ]",
                                    issueModel.ClassZ.ToExport(column));
                                break;
                            case "NumA":
                                line = line.Replace(
                                    "[NumA]",
                                    issueModel.NumA.ToExport(column));
                                break;
                            case "NumB":
                                line = line.Replace(
                                    "[NumB]",
                                    issueModel.NumB.ToExport(column));
                                break;
                            case "NumC":
                                line = line.Replace(
                                    "[NumC]",
                                    issueModel.NumC.ToExport(column));
                                break;
                            case "NumD":
                                line = line.Replace(
                                    "[NumD]",
                                    issueModel.NumD.ToExport(column));
                                break;
                            case "NumE":
                                line = line.Replace(
                                    "[NumE]",
                                    issueModel.NumE.ToExport(column));
                                break;
                            case "NumF":
                                line = line.Replace(
                                    "[NumF]",
                                    issueModel.NumF.ToExport(column));
                                break;
                            case "NumG":
                                line = line.Replace(
                                    "[NumG]",
                                    issueModel.NumG.ToExport(column));
                                break;
                            case "NumH":
                                line = line.Replace(
                                    "[NumH]",
                                    issueModel.NumH.ToExport(column));
                                break;
                            case "NumI":
                                line = line.Replace(
                                    "[NumI]",
                                    issueModel.NumI.ToExport(column));
                                break;
                            case "NumJ":
                                line = line.Replace(
                                    "[NumJ]",
                                    issueModel.NumJ.ToExport(column));
                                break;
                            case "NumK":
                                line = line.Replace(
                                    "[NumK]",
                                    issueModel.NumK.ToExport(column));
                                break;
                            case "NumL":
                                line = line.Replace(
                                    "[NumL]",
                                    issueModel.NumL.ToExport(column));
                                break;
                            case "NumM":
                                line = line.Replace(
                                    "[NumM]",
                                    issueModel.NumM.ToExport(column));
                                break;
                            case "NumN":
                                line = line.Replace(
                                    "[NumN]",
                                    issueModel.NumN.ToExport(column));
                                break;
                            case "NumO":
                                line = line.Replace(
                                    "[NumO]",
                                    issueModel.NumO.ToExport(column));
                                break;
                            case "NumP":
                                line = line.Replace(
                                    "[NumP]",
                                    issueModel.NumP.ToExport(column));
                                break;
                            case "NumQ":
                                line = line.Replace(
                                    "[NumQ]",
                                    issueModel.NumQ.ToExport(column));
                                break;
                            case "NumR":
                                line = line.Replace(
                                    "[NumR]",
                                    issueModel.NumR.ToExport(column));
                                break;
                            case "NumS":
                                line = line.Replace(
                                    "[NumS]",
                                    issueModel.NumS.ToExport(column));
                                break;
                            case "NumT":
                                line = line.Replace(
                                    "[NumT]",
                                    issueModel.NumT.ToExport(column));
                                break;
                            case "NumU":
                                line = line.Replace(
                                    "[NumU]",
                                    issueModel.NumU.ToExport(column));
                                break;
                            case "NumV":
                                line = line.Replace(
                                    "[NumV]",
                                    issueModel.NumV.ToExport(column));
                                break;
                            case "NumW":
                                line = line.Replace(
                                    "[NumW]",
                                    issueModel.NumW.ToExport(column));
                                break;
                            case "NumX":
                                line = line.Replace(
                                    "[NumX]",
                                    issueModel.NumX.ToExport(column));
                                break;
                            case "NumY":
                                line = line.Replace(
                                    "[NumY]",
                                    issueModel.NumY.ToExport(column));
                                break;
                            case "NumZ":
                                line = line.Replace(
                                    "[NumZ]",
                                    issueModel.NumZ.ToExport(column));
                                break;
                            case "DateA":
                                line = line.Replace(
                                    "[DateA]",
                                    issueModel.DateA.ToExport(column));
                                break;
                            case "DateB":
                                line = line.Replace(
                                    "[DateB]",
                                    issueModel.DateB.ToExport(column));
                                break;
                            case "DateC":
                                line = line.Replace(
                                    "[DateC]",
                                    issueModel.DateC.ToExport(column));
                                break;
                            case "DateD":
                                line = line.Replace(
                                    "[DateD]",
                                    issueModel.DateD.ToExport(column));
                                break;
                            case "DateE":
                                line = line.Replace(
                                    "[DateE]",
                                    issueModel.DateE.ToExport(column));
                                break;
                            case "DateF":
                                line = line.Replace(
                                    "[DateF]",
                                    issueModel.DateF.ToExport(column));
                                break;
                            case "DateG":
                                line = line.Replace(
                                    "[DateG]",
                                    issueModel.DateG.ToExport(column));
                                break;
                            case "DateH":
                                line = line.Replace(
                                    "[DateH]",
                                    issueModel.DateH.ToExport(column));
                                break;
                            case "DateI":
                                line = line.Replace(
                                    "[DateI]",
                                    issueModel.DateI.ToExport(column));
                                break;
                            case "DateJ":
                                line = line.Replace(
                                    "[DateJ]",
                                    issueModel.DateJ.ToExport(column));
                                break;
                            case "DateK":
                                line = line.Replace(
                                    "[DateK]",
                                    issueModel.DateK.ToExport(column));
                                break;
                            case "DateL":
                                line = line.Replace(
                                    "[DateL]",
                                    issueModel.DateL.ToExport(column));
                                break;
                            case "DateM":
                                line = line.Replace(
                                    "[DateM]",
                                    issueModel.DateM.ToExport(column));
                                break;
                            case "DateN":
                                line = line.Replace(
                                    "[DateN]",
                                    issueModel.DateN.ToExport(column));
                                break;
                            case "DateO":
                                line = line.Replace(
                                    "[DateO]",
                                    issueModel.DateO.ToExport(column));
                                break;
                            case "DateP":
                                line = line.Replace(
                                    "[DateP]",
                                    issueModel.DateP.ToExport(column));
                                break;
                            case "DateQ":
                                line = line.Replace(
                                    "[DateQ]",
                                    issueModel.DateQ.ToExport(column));
                                break;
                            case "DateR":
                                line = line.Replace(
                                    "[DateR]",
                                    issueModel.DateR.ToExport(column));
                                break;
                            case "DateS":
                                line = line.Replace(
                                    "[DateS]",
                                    issueModel.DateS.ToExport(column));
                                break;
                            case "DateT":
                                line = line.Replace(
                                    "[DateT]",
                                    issueModel.DateT.ToExport(column));
                                break;
                            case "DateU":
                                line = line.Replace(
                                    "[DateU]",
                                    issueModel.DateU.ToExport(column));
                                break;
                            case "DateV":
                                line = line.Replace(
                                    "[DateV]",
                                    issueModel.DateV.ToExport(column));
                                break;
                            case "DateW":
                                line = line.Replace(
                                    "[DateW]",
                                    issueModel.DateW.ToExport(column));
                                break;
                            case "DateX":
                                line = line.Replace(
                                    "[DateX]",
                                    issueModel.DateX.ToExport(column));
                                break;
                            case "DateY":
                                line = line.Replace(
                                    "[DateY]",
                                    issueModel.DateY.ToExport(column));
                                break;
                            case "DateZ":
                                line = line.Replace(
                                    "[DateZ]",
                                    issueModel.DateZ.ToExport(column));
                                break;
                            case "DescriptionA":
                                line = line.Replace(
                                    "[DescriptionA]",
                                    issueModel.DescriptionA.ToExport(column));
                                break;
                            case "DescriptionB":
                                line = line.Replace(
                                    "[DescriptionB]",
                                    issueModel.DescriptionB.ToExport(column));
                                break;
                            case "DescriptionC":
                                line = line.Replace(
                                    "[DescriptionC]",
                                    issueModel.DescriptionC.ToExport(column));
                                break;
                            case "DescriptionD":
                                line = line.Replace(
                                    "[DescriptionD]",
                                    issueModel.DescriptionD.ToExport(column));
                                break;
                            case "DescriptionE":
                                line = line.Replace(
                                    "[DescriptionE]",
                                    issueModel.DescriptionE.ToExport(column));
                                break;
                            case "DescriptionF":
                                line = line.Replace(
                                    "[DescriptionF]",
                                    issueModel.DescriptionF.ToExport(column));
                                break;
                            case "DescriptionG":
                                line = line.Replace(
                                    "[DescriptionG]",
                                    issueModel.DescriptionG.ToExport(column));
                                break;
                            case "DescriptionH":
                                line = line.Replace(
                                    "[DescriptionH]",
                                    issueModel.DescriptionH.ToExport(column));
                                break;
                            case "DescriptionI":
                                line = line.Replace(
                                    "[DescriptionI]",
                                    issueModel.DescriptionI.ToExport(column));
                                break;
                            case "DescriptionJ":
                                line = line.Replace(
                                    "[DescriptionJ]",
                                    issueModel.DescriptionJ.ToExport(column));
                                break;
                            case "DescriptionK":
                                line = line.Replace(
                                    "[DescriptionK]",
                                    issueModel.DescriptionK.ToExport(column));
                                break;
                            case "DescriptionL":
                                line = line.Replace(
                                    "[DescriptionL]",
                                    issueModel.DescriptionL.ToExport(column));
                                break;
                            case "DescriptionM":
                                line = line.Replace(
                                    "[DescriptionM]",
                                    issueModel.DescriptionM.ToExport(column));
                                break;
                            case "DescriptionN":
                                line = line.Replace(
                                    "[DescriptionN]",
                                    issueModel.DescriptionN.ToExport(column));
                                break;
                            case "DescriptionO":
                                line = line.Replace(
                                    "[DescriptionO]",
                                    issueModel.DescriptionO.ToExport(column));
                                break;
                            case "DescriptionP":
                                line = line.Replace(
                                    "[DescriptionP]",
                                    issueModel.DescriptionP.ToExport(column));
                                break;
                            case "DescriptionQ":
                                line = line.Replace(
                                    "[DescriptionQ]",
                                    issueModel.DescriptionQ.ToExport(column));
                                break;
                            case "DescriptionR":
                                line = line.Replace(
                                    "[DescriptionR]",
                                    issueModel.DescriptionR.ToExport(column));
                                break;
                            case "DescriptionS":
                                line = line.Replace(
                                    "[DescriptionS]",
                                    issueModel.DescriptionS.ToExport(column));
                                break;
                            case "DescriptionT":
                                line = line.Replace(
                                    "[DescriptionT]",
                                    issueModel.DescriptionT.ToExport(column));
                                break;
                            case "DescriptionU":
                                line = line.Replace(
                                    "[DescriptionU]",
                                    issueModel.DescriptionU.ToExport(column));
                                break;
                            case "DescriptionV":
                                line = line.Replace(
                                    "[DescriptionV]",
                                    issueModel.DescriptionV.ToExport(column));
                                break;
                            case "DescriptionW":
                                line = line.Replace(
                                    "[DescriptionW]",
                                    issueModel.DescriptionW.ToExport(column));
                                break;
                            case "DescriptionX":
                                line = line.Replace(
                                    "[DescriptionX]",
                                    issueModel.DescriptionX.ToExport(column));
                                break;
                            case "DescriptionY":
                                line = line.Replace(
                                    "[DescriptionY]",
                                    issueModel.DescriptionY.ToExport(column));
                                break;
                            case "DescriptionZ":
                                line = line.Replace(
                                    "[DescriptionZ]",
                                    issueModel.DescriptionZ.ToExport(column));
                                break;
                            case "CheckA":
                                line = line.Replace(
                                    "[CheckA]",
                                    issueModel.CheckA.ToExport(column));
                                break;
                            case "CheckB":
                                line = line.Replace(
                                    "[CheckB]",
                                    issueModel.CheckB.ToExport(column));
                                break;
                            case "CheckC":
                                line = line.Replace(
                                    "[CheckC]",
                                    issueModel.CheckC.ToExport(column));
                                break;
                            case "CheckD":
                                line = line.Replace(
                                    "[CheckD]",
                                    issueModel.CheckD.ToExport(column));
                                break;
                            case "CheckE":
                                line = line.Replace(
                                    "[CheckE]",
                                    issueModel.CheckE.ToExport(column));
                                break;
                            case "CheckF":
                                line = line.Replace(
                                    "[CheckF]",
                                    issueModel.CheckF.ToExport(column));
                                break;
                            case "CheckG":
                                line = line.Replace(
                                    "[CheckG]",
                                    issueModel.CheckG.ToExport(column));
                                break;
                            case "CheckH":
                                line = line.Replace(
                                    "[CheckH]",
                                    issueModel.CheckH.ToExport(column));
                                break;
                            case "CheckI":
                                line = line.Replace(
                                    "[CheckI]",
                                    issueModel.CheckI.ToExport(column));
                                break;
                            case "CheckJ":
                                line = line.Replace(
                                    "[CheckJ]",
                                    issueModel.CheckJ.ToExport(column));
                                break;
                            case "CheckK":
                                line = line.Replace(
                                    "[CheckK]",
                                    issueModel.CheckK.ToExport(column));
                                break;
                            case "CheckL":
                                line = line.Replace(
                                    "[CheckL]",
                                    issueModel.CheckL.ToExport(column));
                                break;
                            case "CheckM":
                                line = line.Replace(
                                    "[CheckM]",
                                    issueModel.CheckM.ToExport(column));
                                break;
                            case "CheckN":
                                line = line.Replace(
                                    "[CheckN]",
                                    issueModel.CheckN.ToExport(column));
                                break;
                            case "CheckO":
                                line = line.Replace(
                                    "[CheckO]",
                                    issueModel.CheckO.ToExport(column));
                                break;
                            case "CheckP":
                                line = line.Replace(
                                    "[CheckP]",
                                    issueModel.CheckP.ToExport(column));
                                break;
                            case "CheckQ":
                                line = line.Replace(
                                    "[CheckQ]",
                                    issueModel.CheckQ.ToExport(column));
                                break;
                            case "CheckR":
                                line = line.Replace(
                                    "[CheckR]",
                                    issueModel.CheckR.ToExport(column));
                                break;
                            case "CheckS":
                                line = line.Replace(
                                    "[CheckS]",
                                    issueModel.CheckS.ToExport(column));
                                break;
                            case "CheckT":
                                line = line.Replace(
                                    "[CheckT]",
                                    issueModel.CheckT.ToExport(column));
                                break;
                            case "CheckU":
                                line = line.Replace(
                                    "[CheckU]",
                                    issueModel.CheckU.ToExport(column));
                                break;
                            case "CheckV":
                                line = line.Replace(
                                    "[CheckV]",
                                    issueModel.CheckV.ToExport(column));
                                break;
                            case "CheckW":
                                line = line.Replace(
                                    "[CheckW]",
                                    issueModel.CheckW.ToExport(column));
                                break;
                            case "CheckX":
                                line = line.Replace(
                                    "[CheckX]",
                                    issueModel.CheckX.ToExport(column));
                                break;
                            case "CheckY":
                                line = line.Replace(
                                    "[CheckY]",
                                    issueModel.CheckY.ToExport(column));
                                break;
                            case "CheckZ":
                                line = line.Replace(
                                    "[CheckZ]",
                                    issueModel.CheckZ.ToExport(column));
                                break;
                            case "Comments":
                                line = line.Replace(
                                    "[Comments]",
                                    issueModel.Comments.ToExport(column));
                                break;
                            case "Creator":
                                line = line.Replace(
                                    "[Creator]",
                                    issueModel.Creator.ToExport(column));
                                break;
                            case "Updator":
                                line = line.Replace(
                                    "[Updator]",
                                    issueModel.Updator.ToExport(column));
                                break;
                            case "CreatedTime":
                                line = line.Replace(
                                    "[CreatedTime]",
                                    issueModel.CreatedTime.ToExport(column));
                                break;
                        }
                    });
                    break;
                case "Results":
                    var resultModel = new ResultModel(ss, dataRow);
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
                                    resultModel.SiteId.ToExport(column));
                                break;
                            case "UpdatedTime":
                                line = line.Replace(
                                    "[UpdatedTime]",
                                    resultModel.UpdatedTime.ToExport(column));
                                break;
                            case "ResultId":
                                line = line.Replace(
                                    "[ResultId]",
                                    resultModel.ResultId.ToExport(column));
                                break;
                            case "Ver":
                                line = line.Replace(
                                    "[Ver]",
                                    resultModel.Ver.ToExport(column));
                                break;
                            case "Body":
                                line = line.Replace(
                                    "[Body]",
                                    resultModel.Body.ToExport(column));
                                break;
                            case "Status":
                                line = line.Replace(
                                    "[Status]",
                                    resultModel.Status.ToExport(column));
                                break;
                            case "Manager":
                                line = line.Replace(
                                    "[Manager]",
                                    resultModel.Manager.ToExport(column));
                                break;
                            case "Owner":
                                line = line.Replace(
                                    "[Owner]",
                                    resultModel.Owner.ToExport(column));
                                break;
                            case "ClassA":
                                line = line.Replace(
                                    "[ClassA]",
                                    resultModel.ClassA.ToExport(column));
                                break;
                            case "ClassB":
                                line = line.Replace(
                                    "[ClassB]",
                                    resultModel.ClassB.ToExport(column));
                                break;
                            case "ClassC":
                                line = line.Replace(
                                    "[ClassC]",
                                    resultModel.ClassC.ToExport(column));
                                break;
                            case "ClassD":
                                line = line.Replace(
                                    "[ClassD]",
                                    resultModel.ClassD.ToExport(column));
                                break;
                            case "ClassE":
                                line = line.Replace(
                                    "[ClassE]",
                                    resultModel.ClassE.ToExport(column));
                                break;
                            case "ClassF":
                                line = line.Replace(
                                    "[ClassF]",
                                    resultModel.ClassF.ToExport(column));
                                break;
                            case "ClassG":
                                line = line.Replace(
                                    "[ClassG]",
                                    resultModel.ClassG.ToExport(column));
                                break;
                            case "ClassH":
                                line = line.Replace(
                                    "[ClassH]",
                                    resultModel.ClassH.ToExport(column));
                                break;
                            case "ClassI":
                                line = line.Replace(
                                    "[ClassI]",
                                    resultModel.ClassI.ToExport(column));
                                break;
                            case "ClassJ":
                                line = line.Replace(
                                    "[ClassJ]",
                                    resultModel.ClassJ.ToExport(column));
                                break;
                            case "ClassK":
                                line = line.Replace(
                                    "[ClassK]",
                                    resultModel.ClassK.ToExport(column));
                                break;
                            case "ClassL":
                                line = line.Replace(
                                    "[ClassL]",
                                    resultModel.ClassL.ToExport(column));
                                break;
                            case "ClassM":
                                line = line.Replace(
                                    "[ClassM]",
                                    resultModel.ClassM.ToExport(column));
                                break;
                            case "ClassN":
                                line = line.Replace(
                                    "[ClassN]",
                                    resultModel.ClassN.ToExport(column));
                                break;
                            case "ClassO":
                                line = line.Replace(
                                    "[ClassO]",
                                    resultModel.ClassO.ToExport(column));
                                break;
                            case "ClassP":
                                line = line.Replace(
                                    "[ClassP]",
                                    resultModel.ClassP.ToExport(column));
                                break;
                            case "ClassQ":
                                line = line.Replace(
                                    "[ClassQ]",
                                    resultModel.ClassQ.ToExport(column));
                                break;
                            case "ClassR":
                                line = line.Replace(
                                    "[ClassR]",
                                    resultModel.ClassR.ToExport(column));
                                break;
                            case "ClassS":
                                line = line.Replace(
                                    "[ClassS]",
                                    resultModel.ClassS.ToExport(column));
                                break;
                            case "ClassT":
                                line = line.Replace(
                                    "[ClassT]",
                                    resultModel.ClassT.ToExport(column));
                                break;
                            case "ClassU":
                                line = line.Replace(
                                    "[ClassU]",
                                    resultModel.ClassU.ToExport(column));
                                break;
                            case "ClassV":
                                line = line.Replace(
                                    "[ClassV]",
                                    resultModel.ClassV.ToExport(column));
                                break;
                            case "ClassW":
                                line = line.Replace(
                                    "[ClassW]",
                                    resultModel.ClassW.ToExport(column));
                                break;
                            case "ClassX":
                                line = line.Replace(
                                    "[ClassX]",
                                    resultModel.ClassX.ToExport(column));
                                break;
                            case "ClassY":
                                line = line.Replace(
                                    "[ClassY]",
                                    resultModel.ClassY.ToExport(column));
                                break;
                            case "ClassZ":
                                line = line.Replace(
                                    "[ClassZ]",
                                    resultModel.ClassZ.ToExport(column));
                                break;
                            case "NumA":
                                line = line.Replace(
                                    "[NumA]",
                                    resultModel.NumA.ToExport(column));
                                break;
                            case "NumB":
                                line = line.Replace(
                                    "[NumB]",
                                    resultModel.NumB.ToExport(column));
                                break;
                            case "NumC":
                                line = line.Replace(
                                    "[NumC]",
                                    resultModel.NumC.ToExport(column));
                                break;
                            case "NumD":
                                line = line.Replace(
                                    "[NumD]",
                                    resultModel.NumD.ToExport(column));
                                break;
                            case "NumE":
                                line = line.Replace(
                                    "[NumE]",
                                    resultModel.NumE.ToExport(column));
                                break;
                            case "NumF":
                                line = line.Replace(
                                    "[NumF]",
                                    resultModel.NumF.ToExport(column));
                                break;
                            case "NumG":
                                line = line.Replace(
                                    "[NumG]",
                                    resultModel.NumG.ToExport(column));
                                break;
                            case "NumH":
                                line = line.Replace(
                                    "[NumH]",
                                    resultModel.NumH.ToExport(column));
                                break;
                            case "NumI":
                                line = line.Replace(
                                    "[NumI]",
                                    resultModel.NumI.ToExport(column));
                                break;
                            case "NumJ":
                                line = line.Replace(
                                    "[NumJ]",
                                    resultModel.NumJ.ToExport(column));
                                break;
                            case "NumK":
                                line = line.Replace(
                                    "[NumK]",
                                    resultModel.NumK.ToExport(column));
                                break;
                            case "NumL":
                                line = line.Replace(
                                    "[NumL]",
                                    resultModel.NumL.ToExport(column));
                                break;
                            case "NumM":
                                line = line.Replace(
                                    "[NumM]",
                                    resultModel.NumM.ToExport(column));
                                break;
                            case "NumN":
                                line = line.Replace(
                                    "[NumN]",
                                    resultModel.NumN.ToExport(column));
                                break;
                            case "NumO":
                                line = line.Replace(
                                    "[NumO]",
                                    resultModel.NumO.ToExport(column));
                                break;
                            case "NumP":
                                line = line.Replace(
                                    "[NumP]",
                                    resultModel.NumP.ToExport(column));
                                break;
                            case "NumQ":
                                line = line.Replace(
                                    "[NumQ]",
                                    resultModel.NumQ.ToExport(column));
                                break;
                            case "NumR":
                                line = line.Replace(
                                    "[NumR]",
                                    resultModel.NumR.ToExport(column));
                                break;
                            case "NumS":
                                line = line.Replace(
                                    "[NumS]",
                                    resultModel.NumS.ToExport(column));
                                break;
                            case "NumT":
                                line = line.Replace(
                                    "[NumT]",
                                    resultModel.NumT.ToExport(column));
                                break;
                            case "NumU":
                                line = line.Replace(
                                    "[NumU]",
                                    resultModel.NumU.ToExport(column));
                                break;
                            case "NumV":
                                line = line.Replace(
                                    "[NumV]",
                                    resultModel.NumV.ToExport(column));
                                break;
                            case "NumW":
                                line = line.Replace(
                                    "[NumW]",
                                    resultModel.NumW.ToExport(column));
                                break;
                            case "NumX":
                                line = line.Replace(
                                    "[NumX]",
                                    resultModel.NumX.ToExport(column));
                                break;
                            case "NumY":
                                line = line.Replace(
                                    "[NumY]",
                                    resultModel.NumY.ToExport(column));
                                break;
                            case "NumZ":
                                line = line.Replace(
                                    "[NumZ]",
                                    resultModel.NumZ.ToExport(column));
                                break;
                            case "DateA":
                                line = line.Replace(
                                    "[DateA]",
                                    resultModel.DateA.ToExport(column));
                                break;
                            case "DateB":
                                line = line.Replace(
                                    "[DateB]",
                                    resultModel.DateB.ToExport(column));
                                break;
                            case "DateC":
                                line = line.Replace(
                                    "[DateC]",
                                    resultModel.DateC.ToExport(column));
                                break;
                            case "DateD":
                                line = line.Replace(
                                    "[DateD]",
                                    resultModel.DateD.ToExport(column));
                                break;
                            case "DateE":
                                line = line.Replace(
                                    "[DateE]",
                                    resultModel.DateE.ToExport(column));
                                break;
                            case "DateF":
                                line = line.Replace(
                                    "[DateF]",
                                    resultModel.DateF.ToExport(column));
                                break;
                            case "DateG":
                                line = line.Replace(
                                    "[DateG]",
                                    resultModel.DateG.ToExport(column));
                                break;
                            case "DateH":
                                line = line.Replace(
                                    "[DateH]",
                                    resultModel.DateH.ToExport(column));
                                break;
                            case "DateI":
                                line = line.Replace(
                                    "[DateI]",
                                    resultModel.DateI.ToExport(column));
                                break;
                            case "DateJ":
                                line = line.Replace(
                                    "[DateJ]",
                                    resultModel.DateJ.ToExport(column));
                                break;
                            case "DateK":
                                line = line.Replace(
                                    "[DateK]",
                                    resultModel.DateK.ToExport(column));
                                break;
                            case "DateL":
                                line = line.Replace(
                                    "[DateL]",
                                    resultModel.DateL.ToExport(column));
                                break;
                            case "DateM":
                                line = line.Replace(
                                    "[DateM]",
                                    resultModel.DateM.ToExport(column));
                                break;
                            case "DateN":
                                line = line.Replace(
                                    "[DateN]",
                                    resultModel.DateN.ToExport(column));
                                break;
                            case "DateO":
                                line = line.Replace(
                                    "[DateO]",
                                    resultModel.DateO.ToExport(column));
                                break;
                            case "DateP":
                                line = line.Replace(
                                    "[DateP]",
                                    resultModel.DateP.ToExport(column));
                                break;
                            case "DateQ":
                                line = line.Replace(
                                    "[DateQ]",
                                    resultModel.DateQ.ToExport(column));
                                break;
                            case "DateR":
                                line = line.Replace(
                                    "[DateR]",
                                    resultModel.DateR.ToExport(column));
                                break;
                            case "DateS":
                                line = line.Replace(
                                    "[DateS]",
                                    resultModel.DateS.ToExport(column));
                                break;
                            case "DateT":
                                line = line.Replace(
                                    "[DateT]",
                                    resultModel.DateT.ToExport(column));
                                break;
                            case "DateU":
                                line = line.Replace(
                                    "[DateU]",
                                    resultModel.DateU.ToExport(column));
                                break;
                            case "DateV":
                                line = line.Replace(
                                    "[DateV]",
                                    resultModel.DateV.ToExport(column));
                                break;
                            case "DateW":
                                line = line.Replace(
                                    "[DateW]",
                                    resultModel.DateW.ToExport(column));
                                break;
                            case "DateX":
                                line = line.Replace(
                                    "[DateX]",
                                    resultModel.DateX.ToExport(column));
                                break;
                            case "DateY":
                                line = line.Replace(
                                    "[DateY]",
                                    resultModel.DateY.ToExport(column));
                                break;
                            case "DateZ":
                                line = line.Replace(
                                    "[DateZ]",
                                    resultModel.DateZ.ToExport(column));
                                break;
                            case "DescriptionA":
                                line = line.Replace(
                                    "[DescriptionA]",
                                    resultModel.DescriptionA.ToExport(column));
                                break;
                            case "DescriptionB":
                                line = line.Replace(
                                    "[DescriptionB]",
                                    resultModel.DescriptionB.ToExport(column));
                                break;
                            case "DescriptionC":
                                line = line.Replace(
                                    "[DescriptionC]",
                                    resultModel.DescriptionC.ToExport(column));
                                break;
                            case "DescriptionD":
                                line = line.Replace(
                                    "[DescriptionD]",
                                    resultModel.DescriptionD.ToExport(column));
                                break;
                            case "DescriptionE":
                                line = line.Replace(
                                    "[DescriptionE]",
                                    resultModel.DescriptionE.ToExport(column));
                                break;
                            case "DescriptionF":
                                line = line.Replace(
                                    "[DescriptionF]",
                                    resultModel.DescriptionF.ToExport(column));
                                break;
                            case "DescriptionG":
                                line = line.Replace(
                                    "[DescriptionG]",
                                    resultModel.DescriptionG.ToExport(column));
                                break;
                            case "DescriptionH":
                                line = line.Replace(
                                    "[DescriptionH]",
                                    resultModel.DescriptionH.ToExport(column));
                                break;
                            case "DescriptionI":
                                line = line.Replace(
                                    "[DescriptionI]",
                                    resultModel.DescriptionI.ToExport(column));
                                break;
                            case "DescriptionJ":
                                line = line.Replace(
                                    "[DescriptionJ]",
                                    resultModel.DescriptionJ.ToExport(column));
                                break;
                            case "DescriptionK":
                                line = line.Replace(
                                    "[DescriptionK]",
                                    resultModel.DescriptionK.ToExport(column));
                                break;
                            case "DescriptionL":
                                line = line.Replace(
                                    "[DescriptionL]",
                                    resultModel.DescriptionL.ToExport(column));
                                break;
                            case "DescriptionM":
                                line = line.Replace(
                                    "[DescriptionM]",
                                    resultModel.DescriptionM.ToExport(column));
                                break;
                            case "DescriptionN":
                                line = line.Replace(
                                    "[DescriptionN]",
                                    resultModel.DescriptionN.ToExport(column));
                                break;
                            case "DescriptionO":
                                line = line.Replace(
                                    "[DescriptionO]",
                                    resultModel.DescriptionO.ToExport(column));
                                break;
                            case "DescriptionP":
                                line = line.Replace(
                                    "[DescriptionP]",
                                    resultModel.DescriptionP.ToExport(column));
                                break;
                            case "DescriptionQ":
                                line = line.Replace(
                                    "[DescriptionQ]",
                                    resultModel.DescriptionQ.ToExport(column));
                                break;
                            case "DescriptionR":
                                line = line.Replace(
                                    "[DescriptionR]",
                                    resultModel.DescriptionR.ToExport(column));
                                break;
                            case "DescriptionS":
                                line = line.Replace(
                                    "[DescriptionS]",
                                    resultModel.DescriptionS.ToExport(column));
                                break;
                            case "DescriptionT":
                                line = line.Replace(
                                    "[DescriptionT]",
                                    resultModel.DescriptionT.ToExport(column));
                                break;
                            case "DescriptionU":
                                line = line.Replace(
                                    "[DescriptionU]",
                                    resultModel.DescriptionU.ToExport(column));
                                break;
                            case "DescriptionV":
                                line = line.Replace(
                                    "[DescriptionV]",
                                    resultModel.DescriptionV.ToExport(column));
                                break;
                            case "DescriptionW":
                                line = line.Replace(
                                    "[DescriptionW]",
                                    resultModel.DescriptionW.ToExport(column));
                                break;
                            case "DescriptionX":
                                line = line.Replace(
                                    "[DescriptionX]",
                                    resultModel.DescriptionX.ToExport(column));
                                break;
                            case "DescriptionY":
                                line = line.Replace(
                                    "[DescriptionY]",
                                    resultModel.DescriptionY.ToExport(column));
                                break;
                            case "DescriptionZ":
                                line = line.Replace(
                                    "[DescriptionZ]",
                                    resultModel.DescriptionZ.ToExport(column));
                                break;
                            case "CheckA":
                                line = line.Replace(
                                    "[CheckA]",
                                    resultModel.CheckA.ToExport(column));
                                break;
                            case "CheckB":
                                line = line.Replace(
                                    "[CheckB]",
                                    resultModel.CheckB.ToExport(column));
                                break;
                            case "CheckC":
                                line = line.Replace(
                                    "[CheckC]",
                                    resultModel.CheckC.ToExport(column));
                                break;
                            case "CheckD":
                                line = line.Replace(
                                    "[CheckD]",
                                    resultModel.CheckD.ToExport(column));
                                break;
                            case "CheckE":
                                line = line.Replace(
                                    "[CheckE]",
                                    resultModel.CheckE.ToExport(column));
                                break;
                            case "CheckF":
                                line = line.Replace(
                                    "[CheckF]",
                                    resultModel.CheckF.ToExport(column));
                                break;
                            case "CheckG":
                                line = line.Replace(
                                    "[CheckG]",
                                    resultModel.CheckG.ToExport(column));
                                break;
                            case "CheckH":
                                line = line.Replace(
                                    "[CheckH]",
                                    resultModel.CheckH.ToExport(column));
                                break;
                            case "CheckI":
                                line = line.Replace(
                                    "[CheckI]",
                                    resultModel.CheckI.ToExport(column));
                                break;
                            case "CheckJ":
                                line = line.Replace(
                                    "[CheckJ]",
                                    resultModel.CheckJ.ToExport(column));
                                break;
                            case "CheckK":
                                line = line.Replace(
                                    "[CheckK]",
                                    resultModel.CheckK.ToExport(column));
                                break;
                            case "CheckL":
                                line = line.Replace(
                                    "[CheckL]",
                                    resultModel.CheckL.ToExport(column));
                                break;
                            case "CheckM":
                                line = line.Replace(
                                    "[CheckM]",
                                    resultModel.CheckM.ToExport(column));
                                break;
                            case "CheckN":
                                line = line.Replace(
                                    "[CheckN]",
                                    resultModel.CheckN.ToExport(column));
                                break;
                            case "CheckO":
                                line = line.Replace(
                                    "[CheckO]",
                                    resultModel.CheckO.ToExport(column));
                                break;
                            case "CheckP":
                                line = line.Replace(
                                    "[CheckP]",
                                    resultModel.CheckP.ToExport(column));
                                break;
                            case "CheckQ":
                                line = line.Replace(
                                    "[CheckQ]",
                                    resultModel.CheckQ.ToExport(column));
                                break;
                            case "CheckR":
                                line = line.Replace(
                                    "[CheckR]",
                                    resultModel.CheckR.ToExport(column));
                                break;
                            case "CheckS":
                                line = line.Replace(
                                    "[CheckS]",
                                    resultModel.CheckS.ToExport(column));
                                break;
                            case "CheckT":
                                line = line.Replace(
                                    "[CheckT]",
                                    resultModel.CheckT.ToExport(column));
                                break;
                            case "CheckU":
                                line = line.Replace(
                                    "[CheckU]",
                                    resultModel.CheckU.ToExport(column));
                                break;
                            case "CheckV":
                                line = line.Replace(
                                    "[CheckV]",
                                    resultModel.CheckV.ToExport(column));
                                break;
                            case "CheckW":
                                line = line.Replace(
                                    "[CheckW]",
                                    resultModel.CheckW.ToExport(column));
                                break;
                            case "CheckX":
                                line = line.Replace(
                                    "[CheckX]",
                                    resultModel.CheckX.ToExport(column));
                                break;
                            case "CheckY":
                                line = line.Replace(
                                    "[CheckY]",
                                    resultModel.CheckY.ToExport(column));
                                break;
                            case "CheckZ":
                                line = line.Replace(
                                    "[CheckZ]",
                                    resultModel.CheckZ.ToExport(column));
                                break;
                            case "Comments":
                                line = line.Replace(
                                    "[Comments]",
                                    resultModel.Comments.ToExport(column));
                                break;
                            case "Creator":
                                line = line.Replace(
                                    "[Creator]",
                                    resultModel.Creator.ToExport(column));
                                break;
                            case "Updator":
                                line = line.Replace(
                                    "[Updator]",
                                    resultModel.Updator.ToExport(column));
                                break;
                            case "CreatedTime":
                                line = line.Replace(
                                    "[CreatedTime]",
                                    resultModel.CreatedTime.ToExport(column));
                                break;
                        }
                    });
                    break;
            }
            return line;
        }
    }
}
