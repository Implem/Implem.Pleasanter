using Implem.IRds;
namespace Implem.PostgreSql
{
    class PostgreSqlSqls : ISqls
    {
        public string TrueString { get; } = "true";
        public string FalseString { get; } = "false";
    }
}
