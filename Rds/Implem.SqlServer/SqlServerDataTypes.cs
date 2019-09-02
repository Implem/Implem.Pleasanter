using Implem.IRds;
namespace Implem.SqlServer
{
    internal class SqlServerDataTypes : ISqlDataTypes
    {
        public string Convert(string name)
        {
            return name;
        }

        public string ConvertBack(string name)
        {
            return name;
        }
    }
}
