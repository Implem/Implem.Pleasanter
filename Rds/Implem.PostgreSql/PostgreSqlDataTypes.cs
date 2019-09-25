using Implem.IRds;
using System.Text.RegularExpressions;
namespace Implem.PostgreSql
{
    internal class PostgreSqlDataType : ISqlDataType
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

        public string DefaultDefinition(object dbRawValue)
        {
            string s = dbRawValue.ToString();
            s = Regex.Replace(s, @"^(?<str>'.+')::.+$", "${str}");
            return s;
        }
    }
}
