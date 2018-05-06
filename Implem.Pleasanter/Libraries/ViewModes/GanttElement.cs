using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class GanttElement
    {
        public string GroupBy;
        public object SortBy;
        public long Id;
        public string Title;
        public string StartTime;
        public string CompletionTime;
        public string DisplayCompletionTime;
        public decimal ProgressRate;
        public bool Completed;
        public bool? GroupSummary;

        public GanttElement(
            string groupBy,
            object sortBy,
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
            Column completionTimeColumn,
            Column workValueColumn,
            Column progressRateColumn,
            bool showProgressRate,
            bool? summary = null)
        {
            GroupBy = groupBy;
            SortBy = sortBy;
            Id = id;
            var userNameText = SiteInfo.UserName(owner, notSet: false);
            var statusText = statusColumn.Choice(status.ToString()).Text;
            Title = showProgressRate
                ? "{0} ({1}{2} * {3}{4}){5}{6}".Params(
                    title,
                    workValueColumn.Display(workValue),
                    workValueColumn.Unit,
                    progressRateColumn.Display(progressRate),
                    progressRateColumn.Unit,
                    !userNameText.IsNullOrEmpty()
                        ? " " + SiteInfo.UserName(owner, notSet: false)
                        : string.Empty,
                    !statusText.IsNullOrEmpty()
                        ? " : " + statusColumn.Choice(status.ToString()).Text
                        : string.Empty)
                : "{0}{1}{2}".Params(
                    title,
                    !userNameText.IsNullOrEmpty()
                        ? " (" + SiteInfo.UserName(owner, notSet: false) + ")"
                        : string.Empty,
                    !statusText.IsNullOrEmpty()
                        ? " : " + statusColumn.Choice(status.ToString()).Text
                        : string.Empty);
            StartTime = startTime.InRange()
                ? startTime.ToLocal(Displays.YmdFormat())
                : createdTime.ToLocal(Displays.YmdFormat());
            CompletionTime = completionTime.ToLocal(Displays.YmdFormat());
            DisplayCompletionTime = completionTime
                .AddDifferenceOfDates(completionTimeColumn.EditorFormat, minus: true)
                .ToLocal(Displays.YmdFormat());
            ProgressRate = progressRate;
            Completed = status >= Parameters.General.CompletionCode;
            GroupSummary = summary;
        }
    }
}