﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Requests;
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
            Context context,
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
            var userNameText = SiteInfo.UserName(
                context: context,
                userId: owner,
                notSet: false);
            var statusText = statusColumn.Choice(status.ToString()).Text;
            Title = showProgressRate
                ? "{0} ({1}{2} * {3}{4}){5}{6}".Params(
                    title,
                    workValueColumn.Display(
                        context: context,
                        value: workValue),
                    workValueColumn.Unit,
                    progressRateColumn.Display(
                        context: context,
                        value: progressRate),
                    progressRateColumn.Unit,
                    !userNameText.IsNullOrEmpty()
                        ? " " + SiteInfo.UserName(
                            context: context,
                            userId: owner,
                            notSet: false)
                        : string.Empty,
                    !statusText.IsNullOrEmpty()
                        ? " : " + statusColumn.Choice(status.ToString()).Text
                        : string.Empty)
                : "{0}{1}{2}".Params(
                    title,
                    !userNameText.IsNullOrEmpty()
                        ? " (" + SiteInfo.UserName(
                            context: context,
                            userId: owner,
                            notSet: false) + ")"
                        : string.Empty,
                    !statusText.IsNullOrEmpty()
                        ? " : " + statusColumn.Choice(status.ToString()).Text
                        : string.Empty);
            StartTime = startTime.InRange()
                ? startTime.ToLocal(
                    context: context,
                    format: Displays.YmdFormat(context: context))
                : createdTime.ToLocal(
                    context: context,
                    format: Displays.YmdFormat(context: context));
            CompletionTime = completionTime.ToLocal(
                context: context,
                format: Displays.YmdFormat(context: context));
            DisplayCompletionTime = completionTime
                .AddDifferenceOfDates(completionTimeColumn.EditorFormat, minus: true)
                .ToLocal(
                    context: context,
                    format: Displays.YmdFormat(context: context));
            ProgressRate = progressRate;
            Completed = status >= Parameters.General.CompletionCode;
            GroupSummary = summary;
        }
    }
}