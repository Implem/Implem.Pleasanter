using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Analysis
{
    public class GanttElement
    {
        public long Id;
        public string Title;
        public string StartTime;
        public string CompletionTime;
        public string DisplayCompletionTime;
        public decimal ProgressRate;
        public bool Completed;

        public GanttElement(
            long id,
            string title,
            decimal workValue,
            DateTime startTime,
            DateTime completionTime,
            decimal progressRate,
            int status,
            int owner,
            int updatorId,
            DateTime createdTime,
            DateTime updatedTime,
            Column statusColumn,
            Column workValueColumn)
        {
            Id = id;
            Title = "{0} ({1}{2} * {3}%) {4} : {5}".Params(
                title,
                workValueColumn.Format(workValue),
                workValueColumn.Unit,
                progressRate,
                owner != RdsUser.UserTypes.Anonymous.ToInt()
                    ? SiteInfo.UserFullName(owner)
                    : Displays.NotSet(),
                statusColumn.Choice(status.ToString()).Text());
            StartTime = startTime.NotZero()
                ? startTime.ToLocal(Displays.YmdFormat())
                : createdTime.ToLocal(Displays.YmdFormat());
            CompletionTime = completionTime.ToLocal(Displays.YmdFormat());
            DisplayCompletionTime = completionTime.AddDays(-1).ToLocal(Displays.YmdFormat());
            ProgressRate = progressRate;
            Completed = status == Parameters.General.CompletionCode;
        }
    }
}