using Implem.IRds;
namespace Implem.PostgreSql
{
    internal class PostgreSqlDataTypes : ISqlDataTypes
    {
        public string Convert(string name)
        {
            return name
                .Replace("nchar", "char")
                .Replace("nvarchar(max)", "text")
                .Replace("nvarchar", "varchar")
                .Replace("bit", "boolean")
                .Replace("image", "bytea")
                .Replace("datetime", "timestamp(3)");
        }

        public string ConvertBack(string name)
        {
            return name
                .Replace("int4", "int")
                .Replace("int8", "bigint")
                .Replace("float8", "float")
                .Replace("numeric", "decimal")
                .Replace("bpchar", "nchar")
                .Replace("varchar", "nvarchar")
                .Replace("text", "nvarchar")
                .Replace("bool", "bit")
                .Replace("bytea", "image")
                .Replace("timestamp", "datetime");
        }
    }
}
