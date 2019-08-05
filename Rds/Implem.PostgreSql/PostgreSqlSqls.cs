using Implem.IRds;
namespace Implem.PostgreSql
{
    class PostgreSqlSqls : ISqls
    {
        public string TrueString { get; } = "true";

        public string FalseString { get; } = "false";

        public object TrueValue { get; } = true;

        public object FalseValue { get; } = false;

        public string IsNotTrue { get; } = " is not true ";

        public string CurrentDateTime { get; } = " CURRENT_TIMESTAMP ";
    }
}
