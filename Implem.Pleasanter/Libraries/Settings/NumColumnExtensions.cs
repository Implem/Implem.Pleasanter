using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class NumColumnExtensions
    {
        public static Dictionary<string, ControlData> NumFilterOptions(this Column column)
        {
            var min = column.NumFilterMin;
            var max = column.NumFilterMax;
            var step = column.NumFilterStep;
            var data = new Dictionary<string, ControlData>();
            if (!column.Required)
            {
                data.Add("\t", new ControlData(Displays.NotSet()));
            }
            if (min < max && step > 0)
            {
                data.Add(column, 0, min, lessThan: true);
                for (var num = min; num < max; num += step)
                {
                    data.Add(column, num, num + step - Minimum(column));
                }
                data.Add(column, max, 0, over: true);
            }
            return data;
        }

        private static void Add(
            this Dictionary<string, ControlData> data,
            Column column,
            decimal? from,
            decimal? to,
            bool lessThan = false,
            bool over = false)
        {
            var fromText = column.Display(from ?? 0, unit: true);
            var toText = column.Display(to ?? 0, unit: true);
            if (lessThan)
            {
                data.Add(
                    "," + to.TrimEndZero(),
                    new ControlData(Displays.LessThan(fromText)));
            }
            else if (over)
            {
                data.Add(
                    from.TrimEndZero() + ",",
                    new ControlData(Displays.Over(fromText)));
            }
            else
            {
                data.Add(
                    from.TrimEndZero() + "," + to.TrimEndZero(),
                    new ControlData(Displays.Over(fromText)));
            }
        }

        private static decimal Minimum(Column column)
        {
            return column.DecimalPlaces == 0
                ? 1
                : ("0." + new string('0', column.DecimalPlaces.ToInt() - 1) + 1).ToDecimal();
        }
    }
}