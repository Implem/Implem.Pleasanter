using Implem.IRds;
using System.Text.RegularExpressions;
namespace Implem.MySql
{
    internal class MySqlDataType : ISqlDataType
    {
        public string Convert(string name)
        {
            return name
                .Replace("nchar", "char")
                .Replace("nvarchar(max)", "longtext")
                .Replace("nvarchar(1024)", "text")
                .Replace("nvarchar", "varchar")
                .Replace("bit", "tinyint(1)")
                .Replace("image", "blob");
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
