using Implem.IRds;
using System.Data;
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
                .Replace("varbinary", "bytea")
                .Replace("image", "bytea")
                .Replace("datetime", "timestamp(3)");
        }

        public string ConvertBack(string name, string definitionTypeName, bool isQrtzTable)
        {
            if (isQrtzTable)
            {
                switch (name)
                {
                    case "bytea":
                        return "varbinary";
                    case "numeric":
                        return "numeric";
                    default:
                        break;
                }
            }
            switch (name)
            {
                case "varchar":
                    return definitionTypeName == "varchar"
                        ? "varchar"
                        : "nvarchar";
                case "bpchar":
                    return definitionTypeName == "char"
                        ? "char"
                        : "nchar";
                default:
                    return name
                        .Replace("int4", "int")
                        .Replace("int8", "bigint")
                        .Replace("float8", "float")
                        .Replace("numeric", "decimal")
                        .Replace("text", "nvarchar")
                        .Replace("bool", "bit")
                        .Replace("bytea", "image")
                        .Replace("timestamp", "datetime");
            }
        }

        public string DefaultDefinition(DataRow dataRow)
        {
            string s = dataRow["column_default"].ToString();
            s = Regex.Replace(s, @"^(?<str>'.+')::.+$", "${str}");
            return s;
        }
    }
}
