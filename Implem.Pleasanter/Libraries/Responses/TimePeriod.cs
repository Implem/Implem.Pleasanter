using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class TimePeriod
    {
        public static Dictionary<string, ControlData> Get(bool recordedTime)
        {
            var hash = new Dictionary<string, ControlData>();
            var min = Min();
            var max = Max();
            for (var m = min; m <= max; m += 12)
            {
                SetFy(hash, DateTime.Now.AddMonths(m), recordedTime);
            }
            for (var m = min; m <= max; m += 6)
            {
                SetHalf(hash, DateTime.Now.AddMonths(m), recordedTime);
            }
            for (var m = min; m <= max; m += 3)
            {
                SetQuarter(hash, DateTime.Now.AddMonths(m), recordedTime);
            }
            for (var m = min; m <= max; m++)
            {
                SetMonth(hash, DateTime.Now.AddMonths(m), recordedTime);
            }
            return hash;
        }

        private static int Min()
        {
            return (DateTime.Now.AddYears(Parameters.General.FilterMinSpan).FyFrom() -
                DateTime.Now).Months();
        }

        private static int Max()
        {
            return (DateTime.Now.AddYears(Parameters.General.FilterMaxSpan + 1).FyFrom() -
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