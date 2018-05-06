using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
using System.Runtime.Serialization;
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

        public CompletionTime(SiteSettings ss, DataRow dataRow, ColumnNameInfo column = null)
        {
            column = column ?? new ColumnNameInfo("CompletionTime");
            Value = dataRow.DateTime(Rds.DataColumnName(column, "CompletionTime"));
            DisplayValue = Value
                .ToLocal()
                .AddDifferenceOfDates(ss.GetColumn("CompletionTime")?.EditorFormat, minus: true);
            Status = new Status(dataRow, column);
            UpdatedTime = dataRow.DateTime(Rds.DataColumnName(column, "UpdatedTime"));
            VerType = dataRow.Bool(Rds.DataColumnName(column, "IsHistory"))
                ? Versions.VerTypes.History
                : Versions.VerTypes.Latest;
        }

        public CompletionTime(
            SiteSettings ss,
            DateTime value,
            Status status,
            bool byForm = false) : base(value, byForm)
        {
            Value = byForm
                ? value
                    .ToUniversal()
                    .AddDifferenceOfDates(ss.GetColumn("CompletionTime")?.EditorFormat)
                : value;
            DisplayValue = value.ToLocal();
            Status = status;
        }

        public CompletionTime(SiteSettings ss, DateTime value)
        {
            Value = value;
            DisplayValue = Value
                .ToLocal()
                .AddDifferenceOfDates(ss.GetColumn("CompletionTime")?.EditorFormat, minus: true);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            DisplayValue = Value.ToUniversal();
        }

        public override HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () =>
            {
                hb.P(css: "time", action: () => hb
                    .Text(column.DisplayGrid(DisplayValue)));
                if (Status?.Value < Parameters.General.CompletionCode)
                {
                    LimitText(hb);
                }
            });
        }

        public bool Near(SiteSettings ss)
        {
            return
                DateTime.Now.ToLocal().Date.AddDays(
                    ss.NearCompletionTimeBeforeDays.ToInt() * (-1))
                        <= DisplayValue &&
                DateTime.Now.ToLocal().Date.AddDays(
                    ss.NearCompletionTimeAfterDays.ToInt() + 1).AddMilliseconds(-1)
                        >= DisplayValue;
        }

        public bool Overdue()
        {
            return Status.Incomplete() && Value < DateTime.Now;
        }

        private HtmlBuilder LimitText(HtmlBuilder hb)
        {
            var value = Value.ToLocal();
            if (!Times.InRange(value))
            {
                return hb;
            }
            var now = VerType == Versions.VerTypes.Latest
                ? DateTime.Now.ToLocal()
                : UpdatedTime.ToLocal();
            var css = LimitCss(now, value);
            var years = Times.DateDiff(Times.Types.Years, now, value);
            if (Math.Abs(years) >= 2)
            {
                return years > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterYears(years.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeYears((years * -1).ToString())));
            }
            var months = Times.DateDiff(Times.Types.Months, now, value);
            if (Math.Abs(months) >= 2)
            {
                return months > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterMonths(months.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeMonths((months * -1).ToString())));
            }
            var days = Times.DateDiff(Times.Types.Days, now, value);
            if ((days >= 0 && days >= 2) || (days < 0))
            {
                return days > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterDays((days - 1).ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeDays(((days * -1) + 1).ToString())));
            }
            var hours = Times.DateDiff(Times.Types.Hours, now, value);
            if (Math.Abs(hours) >= 3)
            {
                return hours > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterHours(hours.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeHours((hours * -1).ToString())));
            }
            var minutes = Times.DateDiff(Times.Types.Minutes, now, value);
            if (Math.Abs(minutes) >= 3)
            {
                return minutes > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterMinutes(minutes.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeMinutes((minutes * -1).ToString())));
            }
            var seconds = Times.DateDiff(Times.Types.Seconds, now, value);
            if (Math.Abs(seconds) >= 1)
            {
                return seconds > 0
                    ? hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitAfterSeconds(seconds.ToString())))
                    : hb.P(css: css, action: () => hb
                        .Text(text: Displays.LimitBeforeSeconds((seconds * -1).ToString())));
            }
            return hb.P(css: "Display-just", action: () => hb
                .Text(text: Displays.LimitJust()));
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

        public override string GridText(Column column)
        {
            return column.DisplayGrid(DisplayValue);
        }

        public override string ToNotice(DateTime saved, Column column, bool updated, bool update)
        {
            return column.DisplayControl(DisplayValue).ToNoticeLine(
                column.DisplayControl(saved
                    .ToLocal()
                    .AddDifferenceOfDates(column.EditorFormat, minus: true)),
                column,
                updated,
                update);
        }
    }
}