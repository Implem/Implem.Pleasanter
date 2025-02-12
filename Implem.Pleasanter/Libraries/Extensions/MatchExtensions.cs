using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
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

        public static bool Matched(this int value, Context context, Column column, string condition)
        {
            if (column.HasChoices())
            {
                return value.ToString().Matched(
                    context: context,
                    column: column,
                    condition: condition);
            }
            else
            {
                return value.ToDecimal().Matched(
                    column: column,
                    condition: condition);
            }
        }

        public static bool Matched(this long value, Column column, string condition)
        {
            return value.ToDecimal().Matched(column, condition);
        }

        public static bool Matched(this Num value, Column column, string condition)
        {
            if (column.Nullable == true && value.Value == null)
            {
                return condition == "[\"\\t\"]";
            }
            return value.Value.ToDecimal().Matched(column, condition);
        }

        public static bool Matched(this decimal value, Column column, string condition)
        {
            var param = condition.Deserialize<List<string>>();
            if (param.Any())
            {
                if (param.All(o => o.RegexExists(@"^[0-9\.]*,[0-9\.]*$")))
                {
                    return param.Any(o =>
                        (o.Split_1st().IsNullOrEmpty() || o.Split_1st().ToDecimal() <= value) &&
                        (o.Split_2nd().IsNullOrEmpty() || o.Split_2nd().ToDecimal() >= value));
                }
                else
                {
                    return param.Any(o => o.ToDecimal() == value);
                }
            }
            return true;
        }

        public static bool Matched(this DateTime value, Context context, Column column, string condition)
        {
            var param = condition.Deserialize<List<string>>();
            if (param.Where(o => !o.IsNullOrEmpty()).Any())
            {
                if (value.InRange())
                {
                    return param.Any(o => DateTimeMatched(
                        context: context,
                        value: value,
                        column: column,
                        o: o));
                }
                else
                {
                    return param.Any(o => o == "\t");
                }
            }
            return true;
        }

        private static bool DateTimeMatched(Context context, DateTime value, Column column, string o)
        {
            var today = DateTime.Now.ToDateTime().ToLocal(context: context).Date;
            switch (o)
            {
                case "Today":
                    return column.ConvertDateTime(
                        context: context,
                        dt: today) <= value
                            && column.ConvertDateTime(
                                context: context,
                                dt: today.AddDays(1)) > value;
                case "ThisMonth":
                    return column.ConvertDateTime(
                        context: context,
                        dt: new DateTime(today.Year, today.Month, 1)) <= value
                            && column.ConvertDateTime(
                                context: context,
                                dt: new DateTime(today.Year, today.Month, 1).AddMonths(1)) > value;
                case "ThisYear":
                    return column.ConvertDateTime(
                        context: context,
                        dt: new DateTime(today.Year, 1, 1)) <= value
                            && column.ConvertDateTime(
                                context: context,
                                dt: new DateTime(today.Year, 1, 1).AddYears(1)) > value;
                case "\t":
                    return !value.InRange();
                default:
                    return (o.Split_1st().IsNullOrEmpty()
                        || column.ConvertDateTime(
                            context: context,
                            dt: o.Split_1st().ToDateTime()) <= value)
                                && (o.Split_2nd().IsNullOrEmpty()
                                    || column.ConvertDateTime(
                                        context: context,
                                        dt: o.Split_2nd().ToDateTime()) >= value);
            }
        }

        public static bool Matched(this string value, Context context, Column column, string condition)
        {
            var param = GetMatchedParam(
                context: context,
                column: column,
                condition: condition);
            if (column.HasChoices())
            {
                if (column.MultipleSelections == true)
                {
                    switch (column.SearchType)
                    {
                        case Column.SearchTypes.ExactMatch:
                        case Column.SearchTypes.ExactMatchMultiple:
                            if (param?.Count() == 1 && param.FirstOrDefault() == "\t")
                            {
                                return value?.ToJson()?.Any() != true;
                            }
                            else
                            {
                                return value == condition;
                            }
                        case Column.SearchTypes.ForwardMatch:
                        case Column.SearchTypes.ForwardMatchMultiple:
                            if (param?.Count() == 1 && param.FirstOrDefault() == "\t")
                            {
                                return value?.ToJson()?.Any() != true;
                            }
                            else
                            {
                                return value?.IndexOf(condition) == 0;
                            }
                        default:
                            if (param?.Count() == 1 && param.FirstOrDefault() == "\t")
                            {
                                return value == "[]";
                            }
                            else if (param?.Any() == true)
                            {
                                return param.All(o =>
                                    value?.Contains(o.StringInJson()) == true);
                            }
                            return true;
                    }
                }
                else if (param?.Any() == true)
                {
                    if (column.TypeName.CsTypeSummary() == Types.CsNumeric
                        && column.Nullable == false
                        && value == "0")
                    {
                        return param.Any(o => o == "\t");
                    }
                    switch (column.SearchType)
                    {
                        case Column.SearchTypes.ExactMatch:
                        case Column.SearchTypes.ExactMatchMultiple:
                            return param.Any(o => !value.IsNullOrEmpty()
                                ? o == value
                                : o == "\t");
                        case Column.SearchTypes.ForwardMatch:
                        case Column.SearchTypes.ForwardMatchMultiple:
                            return param.Any(o => !value.IsNullOrEmpty()
                                ? value.IndexOf(o) == 0
                                : o == "\t");
                        default:
                            return param.Any(o => !value.IsNullOrEmpty()
                                ? value.Contains(o)
                                : o == "\t");
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (condition == " " || condition == "　")
                {
                    return value.IsNullOrEmpty();
                }
                else
                {
                    switch (column.SearchType)
                    {
                        case Column.SearchTypes.ExactMatch:
                            return value == condition;
                        case Column.SearchTypes.ForwardMatch:
                            return value?.IndexOf(condition) == 0;
                        case Column.SearchTypes.PartialMatch:
                            return value?.Contains(condition) == true;
                        case Column.SearchTypes.ExactMatchMultiple:
                            if (param?.Any() == true)
                            {
                                return param.Any(o => o == (!value.IsNullOrEmpty()
                                    ? value
                                    : "\t"));
                            }
                            return true;
                        case Column.SearchTypes.ForwardMatchMultiple:
                            if (param?.Any() == true)
                            {
                                return param.Any(o => !value.IsNullOrEmpty()
                                    ? value.IndexOf(o) == 0
                                    : o == "\t");
                            }
                            return true;
                        case Column.SearchTypes.PartialMatchMultiple:
                            if (param?.Any() == true)
                            {
                                return param.Any(o => !value.IsNullOrEmpty()
                                    ? value.Contains(o)
                                    : o == "\t");
                            }
                            return true;
                        default:
                            return true;
                    }
                }
            }
        }

        private static List<string> GetMatchedParam(Context context, Column column, string condition)
        {
            var param = condition.Deserialize<List<string>>();
            switch (column.Type)
            {
                case Column.Types.User:
                    return param?.Select(o => o == "Own"
                        ? context.UserId.ToString()
                        : o).ToList();
                case Column.Types.Dept:
                    return param?.Select(o => o == "Own"
                        ? context.DeptId.ToString()
                        : o).ToList();
                default:
                    return param?.ToList();
            }
        }
    }
}