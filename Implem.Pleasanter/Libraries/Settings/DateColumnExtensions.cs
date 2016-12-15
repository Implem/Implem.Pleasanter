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
            var min = Min(column);
            var max = Max(column);
            if (column.DateFilterFy.ToBool())
            {
                for (var m = min; m <= max; m += 12)
                {
                    SetFy(hash, DateTime.Now.AddMonths(m), column.RecordedTime);
                }
            }
            if (column.DateFilterHalf.ToBool())
            {
                for (var m = min; m <= max; m += 6)
                {
                    SetHalf(hash, DateTime.Now.AddMonths(m), column.RecordedTime);
                }
            }
            if (column.DateFilterQuarter.ToBool())
            {
                for (var m = min; m <= max; m += 3)
                {
                    SetQuarter(hash, DateTime.Now.AddMonths(m), column.RecordedTime);
                }
            }
            if (column.DateFilterMonth.ToBool())
            {
                for (var m = min; m <= max; m++)
                {
                    SetMonth(hash, DateTime.Now.AddMonths(m), column.RecordedTime);
                }
            }
            return hash;
        }

        private static int Min(Column column)
        {
            return (DateTime.Now.AddYears(column.DateFilterMinSpan.ToInt()).FyFrom() -
                DateTime.Now).Months();
        }

        private static int Max(Column column)
        {
            return (DateTime.Now.AddYears(column.DateFilterMaxSpan.ToInt() + 1).FyFrom() -
                DateTime.Now).Months();
        }

        private static void SetMonth(
            Dictionary<string, ControlData> hash, DateTime current, bool recordedTime)
        {
            var timePeriod = new Implem.Libraries.Classes.TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Month, current);
            if (!recordedTime || timePeriod.From <= DateTime.Now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData(current.ToString("y", Sessions.CultureInfo()) +
                        InRange(timePeriod)));
            }
        }

        private static void SetQuarter(
            Dictionary<string, ControlData> hash, DateTime current, bool recordedTime)
        {
            var timePeriod = new Implem.Libraries.Classes.TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Quarter, current);
            if (!recordedTime || timePeriod.From <= DateTime.Now)
            {
                hash.Add(
                   Period(timePeriod),
                    new ControlData(Displays.Quarter(
                        current.Fy().ToString(), current.Quarter().ToString()) +
                            InRange(timePeriod)));
            }
        }

        private static void SetHalf(
            Dictionary<string, ControlData> hash, DateTime current, bool recordedTime)
        {
            var timePeriod = new Implem.Libraries.Classes.TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Half, current);
            if (!recordedTime || timePeriod.From <= DateTime.Now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData((current.Half() == 1
                        ? Displays.Half1(current.Fy().ToString())
                        : Displays.Half2(current.Fy().ToString())) +
                            InRange(timePeriod)));
            }
        }

        private static void SetFy(
            Dictionary<string, ControlData> hash, DateTime current, bool recordedTime)
        {
            var timePeriod = new Implem.Libraries.Classes.TimePeriod(
                Implem.Libraries.Classes.TimePeriod.Types.Fy, current);
            if (!recordedTime || timePeriod.From <= DateTime.Now)
            {
                hash.Add(
                    Period(timePeriod),
                    new ControlData(Displays.Fy(current.Fy().ToString()) + InRange(timePeriod)));
            }
        }

        private static string Period(Implem.Libraries.Classes.TimePeriod timePeriod)
        {
            return
                timePeriod.From.ToString() + "," +
                timePeriod.To.ToString("yyyy/M/d H:m:s.fff");
        }

        private static string InRange(Implem.Libraries.Classes.TimePeriod timePeriod)
        {
            return timePeriod.InRange(DateTime.Now)
                ? " *"
                : string.Empty;
        }
    }
}