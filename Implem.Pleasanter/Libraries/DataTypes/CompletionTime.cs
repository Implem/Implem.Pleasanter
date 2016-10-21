using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
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
    public class CompletionTime : Time
    {
        public Status Status;
        public DateTime UpdatedTime;
        public Versions.VerTypes VerType = Versions.VerTypes.Latest;

        public CompletionTime() : base()
        {
        }

        public CompletionTime(DataRow dataRow, string name)
        {
            Value = dataRow.DateTime(name);
            DisplayValue = Value.ToLocal().AddDays(-1);
            Status = new Status(dataRow, "Status");
            UpdatedTime = dataRow["UpdatedTime"].ToDateTime();
            if (dataRow.Table.Columns.Contains("IsHistory"))
            {
                VerType = dataRow["IsHistory"].ToBool()
                    ? Versions.VerTypes.History
                    : Versions.VerTypes.Latest;
            }
        }

        public CompletionTime(
            DateTime value, Status status, bool byForm = false) : base(value, byForm)
        {
            Value = byForm
                ? value.ToUniversal().AddDays(1)
                : value;
            DisplayValue = value.ToUniversal();
            Status = status;
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
                        .Displays_LimitAfterYears(years.ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeYears((years * -1).ToString()));
            }
            var months = Times.DateDiff(Times.Types.Months, now, value);
            if (Math.Abs(months) >= 2)
            {
                return months > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterMonths(months.ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeMonths((months * -1).ToString()));
            }
            var days = Times.DateDiff(Times.Types.Days, now, value);
            if ((days >= 0 && days >= 2) || (days < 0))
            {
                return days > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterDays((days - 1).ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeDays(((days * -1) + 1).ToString()));
            }
            var hours = Times.DateDiff(Times.Types.Hours, now, value);
            if (Math.Abs(hours) >= 3)
            {
                return hours > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterHours(hours.ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeHours((hours * -1).ToString()));
            }
            var minutes = Times.DateDiff(Times.Types.Minutes, now, value);
            if (Math.Abs(minutes) >= 3)
            {
                return minutes > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterMinutes(minutes.ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeMinutes((minutes * -1).ToString()));
            }
            var seconds = Times.DateDiff(Times.Types.Seconds, now, value);
            if (Math.Abs(seconds) >= 1)
            {
                return seconds > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterSeconds(seconds.ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeSeconds((seconds * -1).ToString()));
            }
            return hb.P(css: "Display-just", action: () => hb
                .Displays_LimitJust());
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

        public override string ToNotice(
            DateTime saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.DisplayExport(DisplayValue).ToNoticeLine(
                column.DisplayExport(saved.ToLocal().AddDays(-1)),
                column,
                updated,
                update);
        }
    }
}