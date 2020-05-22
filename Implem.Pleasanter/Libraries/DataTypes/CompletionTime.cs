using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class CompletionTime : Time
    {
        public Status Status;
        public DateTime UpdatedTime;
        [NonSerialized]
        public Versions.VerTypes VerType = Versions.VerTypes.Latest;

        public CompletionTime() : base()
        {
        }

        public CompletionTime(
            Context context, SiteSettings ss, DataRow dataRow, ColumnNameInfo column = null)
        {
            column = column ?? new ColumnNameInfo("CompletionTime");
            Value = dataRow.DateTime(Rds.DataColumnName(column, "CompletionTime"));
            DisplayValue = Value
                .ToLocal(context: context)
                .AddDifferenceOfDates(ss.GetColumn(
                    context: context,
                    columnName: "CompletionTime")?.EditorFormat, minus: true);
            Status = new Status(dataRow, column);
            UpdatedTime = dataRow.DateTime(Rds.DataColumnName(column, "UpdatedTime"));
            VerType = dataRow.Bool(Rds.DataColumnName(column, "IsHistory"))
                ? Versions.VerTypes.History
                : Versions.VerTypes.Latest;
        }

        public CompletionTime(
            Context context,
            SiteSettings ss,
            DateTime value,
            Status status,
            bool byForm = false) : base(context, value, byForm)
        {
            if (byForm)
            {
                Value = value
                    .ToUniversal(context: context)
                    .AddDifferenceOfDates(ss.GetColumn(
                        context: context,
                        columnName: "CompletionTime")?.EditorFormat);
                DisplayValue = value;
            }
            else
            {
                Value = value
                    .AddDifferenceOfDates(ss.GetColumn(
                        context: context,
                        columnName: "CompletionTime")?.EditorFormat);
                DisplayValue = value.ToLocal(context: context);
            }
            Status = status;
        }

        public CompletionTime(Context context, SiteSettings ss, DateTime value)
        {
            Value = value;
            DisplayValue = Value
                .ToLocal(context: context)
                .AddDifferenceOfDates(ss.GetColumn(
                    context: context,
                    columnName: "CompletionTime")?.EditorFormat, minus: true);
        }

        public override HtmlBuilder Td(HtmlBuilder hb, Context context, Column column)
        {
            return hb.Td(
                css: column.TextAlign == SiteSettings.TextAlignTypes.Right
                    ? " right-align "
                    : string.Empty,
                action: () =>
                {
                    hb.P(css: "time", action: () => hb
                        .Text(column.DisplayGrid(
                            context: context,
                            value: DisplayValue)));
                    if (Status?.Value < Parameters.General.CompletionCode)
                    {
                        LimitText(hb, context);
                    }
                });
        }

        public bool Near(Context context, SiteSettings ss)
        {
            return
                DateTime.Now.ToLocal(context: context).Date.AddDays(
                    ss.NearCompletionTimeBeforeDays.ToInt() * (-1))
                        <= DisplayValue &&
                DateTime.Now.ToLocal(context: context).Date.AddDays(
                    ss.NearCompletionTimeAfterDays.ToInt() + 1).AddMilliseconds(-1)
                        >= DisplayValue;
        }

        public bool Overdue()
        {
            return Status.Incomplete() && Value < DateTime.Now;
        }

        private HtmlBuilder LimitText(HtmlBuilder hb, Context context)
        {
            var value = Value.ToLocal(context: context);
            if (!Times.InRange(value))
            {
                return hb;
            }
            var now = VerType == Versions.VerTypes.Latest
                ? DateTime.Now.ToLocal(context: context)
                : UpdatedTime.ToLocal(context: context);
            var css = LimitCss(now, value);
            var years = Times.DateDiff(Times.Types.Years, now, value);
            if (Math.Abs(years) >= 2)
            {
                return years > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterYears(
                            context: context,
                            data: years.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeYears(
                            context: context,
                            data: (years * -1).ToString())));
            }
            var months = Times.DateDiff(Times.Types.Months, now, value);
            if (Math.Abs(months) >= 2)
            {
                return months > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterMonths(
                            context: context,
                            data: months.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeMonths(
                            context: context,
                            data: (months * -1).ToString())));
            }
            var days = Times.DateDiff(Times.Types.Days, now, value);
            if ((days >= 0 && days >= 2) || (days < 0))
            {
                return days > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterDays(
                            context: context,
                            data: (days - 1).ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeDays(
                            context: context,
                            data: ((days * -1) + 1).ToString())));
            }
            var hours = Times.DateDiff(Times.Types.Hours, now, value);
            if (Math.Abs(hours) >= 3)
            {
                return hours > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterHours(
                            context: context,
                            data: hours.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeHours(
                            context: context,
                            data: (hours * -1).ToString())));
            }
            var minutes = Times.DateDiff(Times.Types.Minutes, now, value);
            if (Math.Abs(minutes) >= 3)
            {
                return minutes > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterMinutes(
                            context: context,
                            data: minutes.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeMinutes(
                            context: context,
                            data: (minutes * -1).ToString())));
            }
            var seconds = Times.DateDiff(Times.Types.Seconds, now, value);
            if (Math.Abs(seconds) >= 1)
            {
                return seconds > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterSeconds(
                            context: context,
                            data: seconds.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeSeconds(
                            context: context,
                            data: (seconds * -1).ToString())));
            }
            return hb.P(css: "Display-just", action: () => hb
                .Text(text: Displays.LimitJust(context: context)));
        }

        private static string LimitCss(DateTime now, DateTime limit)
        {
            var diff = (limit - now).TotalSeconds;
            if (diff < Parameters.General.LimitWarning3)
            {
                return "limit-warning3";
            }
            else if (diff < Parameters.General.LimitWarning2)
            {
                return "limit-warning2";
            }
            else if (diff < Parameters.General.LimitWarning1)
            {
                return "limit-warning1";
            }
            else
            {
                return "limit-normal";
            }
        }

        public override string GridText(Context context, Column column)
        {
            return column.DisplayGrid(
                context: context,
                value: DisplayValue);
        }

        public override string ToNotice(
            Context context,
            DateTime saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: column.DisplayControl(
                    context: context,
                    value: DisplayValue),
                saved: column.DisplayControl(
                    context: context,
                    value: saved
                        .ToLocal(context: context)
                        .AddDifferenceOfDates(column.EditorFormat, minus: true)),
                column: column,
                updated: updated,
                update: update);
        }
    }
}