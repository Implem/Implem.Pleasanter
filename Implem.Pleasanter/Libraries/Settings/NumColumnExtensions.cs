using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class NumColumnExtensions
    {
        public static Dictionary<string, ControlData> NumFilterOptions(
            this Column column, Context context)
        {
            var min = column.NumFilterMin;
            var max = column.NumFilterMax;
            var step = column.NumFilterStep;
            var data = new Dictionary<string, ControlData>();
            if (!column.Required)
            {
                data.Add("\t", new ControlData(Displays.NotSet(context: context)));
            }
            if (min < max && step > 0)
            {
                data.Add(
                    context: context,
                    column: column,
                    from: 0,
                    to: min,
                    lessThan: true);
                for (var num = min; num < max; num += step)
                {
                    if (data.Count < Parameters.General.DropDownSearchPageSize)
                    {
                        data.Add(
                            context: context,
                            column: column,
                            from: num,
                            to: num + step - Minimum(column));
                    }
                }
                if (data.Count < Parameters.General.DropDownSearchPageSize)
                {
                    data.Add(
                        context: context,
                        column: column,
                        from: max,
                        to: 0,
                        over: true);
                }
            }
            return data;
        }

        private static void Add(
            this Dictionary<string, ControlData> data,
            Context context,
            Column column,
            decimal? from,
            decimal? to,
            bool lessThan = false,
            bool over = false)
        {
            var fromText = column.Display(
                context: context,
                value: from ?? 0,
                unit: true);
            var toText = column.Display(
                context: context,
                value: to ?? 0,
                unit: true);
            if (lessThan)
            {
                data.Add(
                    "," + to.TrimEndZero(),
                    new ControlData(Displays.LessThan(
                        context: context,
                        data: fromText)));
            }
            else if (over)
            {
                data.Add(
                    from.TrimEndZero() + ",",
                    new ControlData(Displays.Over(
                        context: context,
                        data: fromText)));
            }
            else
            {
                data.Add(
                    from.TrimEndZero() + "," + to.TrimEndZero(),
                    new ControlData(Displays.Over(
                        context: context,
                        data: fromText)));
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