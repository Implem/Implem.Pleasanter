using Implem.IRds;
namespace Implem.SqlServer
{
    class SqlServerSqls : ISqls
    {
        public string TrueString { get; } = "1";
        public string FalseString { get; } = "0";
    }
}
