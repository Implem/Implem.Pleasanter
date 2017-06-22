using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class DateColumnExtensions
    {
        public static Dictionary<string, ControlData> DateFilterOptions(this Column column)
        {
            var hash = new Dictionary<string, ControlData>();
            var now = DateTime.Now.ToLocal();
            var min = Min(column, now);
            var max = Max(column, now);
            var diff = 0;
            switch (column.ColumnName)
            {
                case "CompletionTime": diff = 1; break;
            }
            if (!column.Required)
            {
                hash.Add("\t", new ControlData(Displays.NotSet()));
            }
            if (column.DateFilterFy == true)
            {
                for (var m = min; m <= max; m += 12)
                {
                    SetFy(
                        hash,
                        now,
                        now.AddMonths(m),
                        column.RecordedTime,
                        diff);
                }
            }
            if (column.DateFilterHalf == true)
            {
                for (var m = min; m <= max; m += 6)
                {
                    SetHalf(
                        hash,
                        now,
                        now.AddMonths(m),
                        column.RecordedTime,
                        diff);
                }
            }
            if (column.DateFilterQuarter == true)
            {
                for (var m = min; m <= max; m += 3)
                {
                    SetQuarter(
                        hash,
                        now,
                        now.AddMonths(m),
                        column.RecordedTime,
                        diff);
                }
            }
            if (column.DateFilterMonth == true)
            {
                for (var m = min; m <= max; m++)
                {
                    SetMonth(
                        hash,
                        now,
                        now.AddMonths(m),
                        column.RecordedTime,
                        diff);
                }
            }
            return hash;
        }

        private static int Min(Column column, DateTime now)
        {
            return (now.AddYears(column.DateFilterMinSpan.ToInt()) - now).Months();
        }

        private static int Max(Column column, DateTime now)
        {
            return (now.AddYears(column.DateFilterMaxSpan.ToInt() + 1) - now).Months();
        }

        private static void SetMonth(
            Dictionary<string, ControlData> hash,
            DateTime now,
            DateTime current,
            bool recordedTime,
            int diff)
        {
            var timePeriod = new Implem.Libraries.Classes.TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Month, current, diff);
            if (!recordedTime || timePeriod.From <= now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData(current.ToString("y", Sessions.CultureInfo()) +
                        InRange(timePeriod, now)));
            }
        }

        private static void SetQuarter(
            Dictionary<string, ControlData> hash,
            DateTime now,
            DateTime current,
            bool recordedTime,
            int diff)
        {
            var timePeriod = new Implem.Libraries.Classes.TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Quarter, current, diff);
            if (!recordedTime || timePeriod.From <= now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData(Displays.Quarter(
                        current.Fy().ToString(), current.Quarter().ToString()) +
                            InRange(timePeriod, now)));
            }
        }

        private static void SetHalf(
            Dictionary<string, ControlData> hash,
            DateTime now,
            DateTime current,
            bool recordedTime,
            int diff)
        {
            var timePeriod = new Implem.Libraries.Classes.TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Half, current, diff);
            if (!recordedTime || timePeriod.From <= now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData((current.Half() == 0
                        ? Displays.Half1(current.Fy().ToString())
                        : Displays.Half2(current.Fy().ToString())) +
                            InRange(timePeriod, now)));
            }
        }

        private static void SetFy(
            Dictionary<string, ControlData> hash,
            DateTime now,
            DateTime current,
            bool recordedTime,
            int diff)
        {
            var timePeriod = new Implem.Libraries.Classes.TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Fy, current, diff);
            if (!recordedTime || timePeriod.From <= now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData(Displays.Fy(current.Fy().ToString()) + InRange(timePeriod, now)));
            }
        }

        private static string Period(Implem.Libraries.Classes.TimePeriod timePeriod)
        {
            return
                timePeriod.From.ToString() + "," +
                timePeriod.To.ToString("yyyy/M/d H:m:s.fff");
        }

        private static string InRange(Implem.Libraries.Classes.TimePeriod timePeriod, DateTime now)
        {
            return timePeriod.InRange(now)
                ? " *"
                : string.Empty;
        }
    }
}