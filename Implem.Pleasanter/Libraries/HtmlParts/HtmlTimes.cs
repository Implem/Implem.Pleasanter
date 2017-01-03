using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTimes
    {
        public static HtmlBuilder ElapsedTime(this HtmlBuilder hb, DateTime value)
        {
            if (!Times.InRange(value))
            {
                return hb;
            }
            var now = DateTime.Now.ToLocal();
            var css = "elapsed-time" +
                ((DateTime.Now - value).Days > Parameters.General.SiteMenuHotSpan
                    ? " old"
                    : string.Empty);
            var displayTime = Displays.UpdatedTime() + " " +
                value.ToString(Sessions.CultureInfo());
            var years = Times.DateDiff(Times.Types.Years, value, now);
            if (years >= 2)
            {
                return hb.Span(
                    attributes: new HtmlAttributes()
                        .Class(css)
                        .Title(displayTime),
                    action: () => hb
                        .Text(text: Displays.YearsAgo(years.ToString())));
            }
            var months = Times.DateDiff(Times.Types.Months, value, now);
            if (months >= 2)
            {
                return hb.Span(
                    attributes: new HtmlAttributes()
                        .Class(css)
                        .Title(displayTime),
                    action: () => hb
                        .Text(text: Displays.MonthsAgo(months.ToString())));
            }
            var days = Times.DateDiff(Times.Types.Days, value, now);
            if (days >= 3)
            {
                return hb.Span(
                    attributes: new HtmlAttributes()
                        .Class(css)
                        .Title(displayTime),
                    action: () => hb
                        .Text(text: Displays.DaysAgo(days.ToString())));
            }
            var hours = Times.DateDiff(Times.Types.Hours, value, now);
            if (hours >= 3)
            {
                return hb.Span(
                    attributes: new HtmlAttributes()
                        .Class(css)
                        .Title(displayTime),
                    action: () => hb
                        .Text(text: Displays.HoursAgo(hours.ToString())));
            }
            var minutes = Times.DateDiff(Times.Types.Minutes, value, now);
            if (minutes >= 3)
            {
                return hb.Span(
                    attributes: new HtmlAttributes()
                        .Class(css)
                        .Title(displayTime),
                    action: () => hb
                        .Text(text: Displays.MinutesAgo(minutes.ToString())));
            }
            var seconds = Times.DateDiff(Times.Types.Seconds, value, now);
            if (seconds >= 1)
            {
                return hb.Span(
                    attributes: new HtmlAttributes()
                        .Class(css)
                        .Title(displayTime),
                    action: () => hb
                        .Text(text: Displays.SecondsAgo(seconds.ToString())));
            }
            return hb.Span(css: css, action: () => hb
                .Text(text: Displays.LimitJust()));
        }
    }
}