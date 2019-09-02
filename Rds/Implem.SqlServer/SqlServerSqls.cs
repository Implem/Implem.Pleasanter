using Implem.IRds;
namespace Implem.SqlServer
{
    internal class SqlServerSqls : ISqls
    {
        public string TrueString { get; } = "1";

        public string FalseString { get; } = "0";

        public object TrueValue { get; } = 1;

        public object FalseValue { get; } = 0;

        public string IsNotTrue { get; } = " <> 1 ";

        public string CurrentDateTime { get; } = " getdate() ";

        public string WhereLikeTemplateForward { get; } = "'%' + ";

        public string WhereLikeTemplate { get; } = "@SearchText#ParamCount#_#CommandCount# + '%')";

        public string GenerateIdentity { get; } = " identity({0}, 1)";

        public object DateTimeValue(object value)
        {
            return value;
        }

        public string BooleanString(string bit)
        {
            return bit;
        }
    }
}
