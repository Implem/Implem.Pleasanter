using Implem.IRds;
namespace Implem.SqlServer
{
    internal class SqlServerSqls : ISqls
    {
        public string TrueString { get; } = "1";

        public string FalseString { get; } = "0";

        public string IsNotTrue { get; } = " <> 1 ";

        public string CurrentDateTime { get; } = " getdate() ";

        public string WhereLikeTemplateForward { get; } = "'%' + ";

        public string WhereLikeTemplate { get; } = "#ParamCount#_#CommandCount# + '%')";

        public string GenerateIdentity { get; } = " identity({0}, 1)";

        public object DateTimeValue(object value)
        {
            return value;
        }

        public string BooleanString(string bit)
        {
            return bit == "1" ? TrueString : FalseString;
        }

        public string IntegerColumnLike(string tableName, string columnName)
        {
            return "(\"" + tableName + "\".\"" + columnName + "\" like ";
        }

        public string DateAddHour(int hour, string columnBracket)
        {
            return $"dateadd(hour,{hour},{columnBracket})";
        }

        public string DateGroupYearly { get; } = "substring(convert(varchar,{0},111),1,4)";

        public string DateGroupMonthly { get; } = "substring(convert(varchar,{0},111),1,7)";

        public string DateGroupWeeklyPart { get; } = "case datepart(weekday,{0}) when 1 then dateadd(day,-6,{0}) else dateadd(day,(2-datepart(weekday,{0})),{0}) end";

        public string DateGroupWeekly { get; } = "datepart(year,{0}) * 100 + datepart(week,{0})";

        public string DateGroupDaily { get; } = "convert(varchar,{0},111)";
    }
}
