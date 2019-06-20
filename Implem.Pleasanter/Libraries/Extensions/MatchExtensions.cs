using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ViewUtilities
    {
        public static bool Matched(this bool value, Column column, string condition)
        {
            switch (column.CheckFilterControlType)
            {
                case ColumnUtilities.CheckFilterControlTypes.OnOnly:
                    return value;
                case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                    switch ((ColumnUtilities.CheckFilterTypes)condition.ToInt())
                    {
                        case ColumnUtilities.CheckFilterTypes.On:
                            return value;
                        case ColumnUtilities.CheckFilterTypes.Off:
                            return !value;
                    }
                    break;
            }
            return true;
        }

        public static bool Matched(this int value, Column column, string condition)
        {
            return value.ToDecimal().Matched(column, condition);
        }

        public static bool Matched(this long value, Column column, string condition)
        {
            return value.ToDecimal().Matched(column, condition);
        }

        public static bool Matched(this decimal value, Column column, string condition)
        {
            var param = condition.Deserialize<List<string>>();
            if (param.Any())
            {
                if (param.All(o => o.RegexExists(@"^[0-9\.]*,[0-9\.]*$")))
                {
                    return param.Any(o =>
                        o.Split_1st().ToDecimal() <= value &&
                        o.Split_2nd().ToDecimal() >= value);
                }
                else
                {
                    return param.Any(o => o.ToDecimal() == value);
                }
            }
            return true;
        }

        public static bool Matched(this DateTime value, Column column, string condition)
        {
            var param = condition.Deserialize<List<string>>();
            if (param.Any())
            {
                return value.InRange()
                    ? param.Any(o =>
                        o.Split_1st().ToDateTime() <= value &&
                        o.Split_2nd().ToDateTime() >= value)
                    : param.Any(o => o == "\n");
            }
            return true;
        }

        public static bool Matched(this string value, Column column, string condition)
        {
            var param = condition.Deserialize<List<string>>();
            return param.Contains(value != string.Empty
                ? value
                : "\t");
        }
    }
}