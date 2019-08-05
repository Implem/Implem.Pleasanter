using Implem.IRds;
namespace Implem.SqlServer
{
    class SqlServerSqls : ISqls
    {
        public string TrueString { get; } = "1";

        public string FalseString { get; } = "0";

        public object TrueValue { get; } = 1;

        public object FalseValue { get; } = 0;

        public string IsNotTrue { get; } = " <> 1 ";

        public string CurrentDateTime { get; } = " getdate() ";
    }
}
