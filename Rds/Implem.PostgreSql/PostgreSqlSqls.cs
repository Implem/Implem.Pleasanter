using Implem.IRds;
using System;
namespace Implem.PostgreSql
{
    internal class PostgreSqlSqls : ISqls
    {
        public string TrueString { get; } = "true";

        public string FalseString { get; } = "false";

        public object TrueValue { get; } = true;

        public object FalseValue { get; } = false;

        public string IsNotTrue { get; } = " is not true ";

        public string CurrentDateTime { get; } = " CURRENT_TIMESTAMP ";

        public string WhereLikeTemplateForward { get; } = "'%' || ";

        public string WhereLikeTemplate { get; } = "@SearchText#ParamCount#_#CommandCount# || '%')";

        public object DateTimeValue(object value)
        {
            return value != null &&
                !(value is DateTime) &&
                DateTime.TryParse(value.ToString(), out var data)
                ? data
                : value;
        }
    }
}
