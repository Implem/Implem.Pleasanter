using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Views;
using System;
using System.Data;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Time : IConvertable
    {
        public DateTime Value;
        public DateTime DisplayValue;

        public Time()
        {
        }

        public Time(DataRow dataRow, string name)
        {
            Value = dataRow.DateTime(name);
            DisplayValue = Value.ToLocal();
        }

        public Time(DateTime value, bool byForm = false)
        {
            Value = byForm
                ? value.ToUniversal()
                : value;
            DisplayValue = value;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            DisplayValue = Value.ToUniversal();
        }

        public virtual string ToView(Column column)
        {
            return Value.ToText(column);
        }

        public virtual string ToControl(Column column)
        {
            return Value.NotZero()
                ? DisplayValue.ToControl(column)
                : string.Empty;
        }

        public virtual string ToResponse()
        {
            return Value.NotZero()
                ? DisplayValue.ToString()
                : string.Empty;
        }

        public override string ToString()
        {
            return Value.NotZero() 
                ? Value.ToString() 
                : string.Empty;
        }

        public virtual string ToViewText(string format = "")
        {
            return Value.NotZero() 
                ? DisplayValue.ToString(format)
                : string.Empty;
        }

        public bool DifferentDate()
        {
            return 
                DisplayValue.ToShortDateString() !=
                DateTime.Now.ToLocal().ToShortDateString();
        }

        public virtual HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => hb
                .P(css: "time", action: () => hb
                    .Text(DisplayValue.ToText(column))));
        }

        public string ToExport(Column column)
        {
            return ToViewText();
        }
    }

    public class CompletionTime : Time
    {
        public Status Status;

        public CompletionTime() : base()
        {
        }

        public CompletionTime(DataRow dataRow, string name)
        {
            Value = dataRow.DateTime(name);
            DisplayValue = Value.ToLocal().AddDays(-1);
            Status = new Status(dataRow, "Status");
        }

        public CompletionTime(DateTime value, Status status, bool byForm = false) : base(value, byForm)
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
                    .Text(DisplayValue.ToText(column)));
                if (Status?.Value != Def.Parameters.CompletionCode)
                {
                    LimitText(hb);
                }
            });
        }

        private HtmlBuilder LimitText(HtmlBuilder hb)
        {
            if (!Times.InRange(Value))
            {
                return hb;
            }
            var now = DateTime.Now.ToLocal();
            var css = LimitCss(now, Value);
            var years = Times.DateDiff(Times.Types.Years, now, Value);
            if (Math.Abs(years) >= 2)
            {
                return years > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterYears(years.ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeYears((years * -1).ToString()));
            }
            var months = Times.DateDiff(Times.Types.Months, now, Value);
            if (Math.Abs(months) >= 2)
            {
                return months > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterMonths(months.ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeMonths((months * -1).ToString()));
            }
            var days = Times.DateDiff(Times.Types.Days, now, Value);
            if ((days >= 0 && days >= 2) || (days < 0))
            {
                return days > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterDays((days - 1).ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeDays(((days * -1) + 1).ToString()));
            }
            var hours = Times.DateDiff(Times.Types.Hours, now, Value);
            if (Math.Abs(hours) >= 3)
            {
                return hours > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterHours(hours.ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeHours((hours * -1).ToString()));
            }
            var minutes = Times.DateDiff(Times.Types.Minutes, now, Value);
            if (Math.Abs(minutes) >= 3)
            {
                return minutes > 0
                    ? hb.P(css: css, action: () => hb
                        .Displays_LimitAfterMinutes(minutes.ToString()))
                    : hb.P(css: css, action: () => hb
                        .Displays_LimitBeforeMinutes((minutes * -1).ToString()));
            }
            var seconds = Times.DateDiff(Times.Types.Seconds, now, Value);
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
            if (diff < Def.Parameters.LimitWarning3)
            {
                return "limit-warning3";
            }
            else if (diff < Def.Parameters.LimitWarning2)
            {
                return "limit-warning2";
            }
            else if (diff < Def.Parameters.LimitWarning1)
            {
                return "limit-warning1";
            }
            else
            {
                return "limit-normal";
            }
        }
    }
}