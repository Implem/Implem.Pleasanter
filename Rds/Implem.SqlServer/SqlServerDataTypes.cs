using Implem.IRds;
using System.Data;
using System.Text.RegularExpressions;

namespace Implem.SqlServer
{
    internal class SqlServerDataType : ISqlDataType
    {
        public string Convert(string name)
        {
            return name;
        }

        public string ConvertBack(string name, string definitionTypeName, bool isQrtzTable)
        {
            return name;
        }

        public string DefaultValue(DataRow dataRow)
        {
            var columnDefault = dataRow["column_default"].ToString();
            while (IsEnclosedInParentheses(value: columnDefault))
            {
                columnDefault = columnDefault.Substring(1, columnDefault.Length - 2);
            }
            var match = Regex.Match(
                columnDefault,
                @"^N?'(?<value>(?:[^']|'')*)'$");
            return match.Success
                ? match.Groups["value"].Value.Replace("''", "'")
                : columnDefault;
        }

        public string StringLiteral(string value)
        {
            return $"N'{value.Replace("'", "''")}'";
        }

        private static bool IsEnclosedInParentheses(string value)
        {
            if (value.Length < 2
                || value[0] != '('
                || value[^1] != ')')
            {
                return false;
            }
            var depth = 0;
            var inQuote = false;
            for (var i = 0; i < value.Length - 1; i++)
            {
                if (value[i] == '\'')
                {
                    inQuote = inQuote == false;
                }
                else if (inQuote)
                {
                    continue;
                }
                else if (value[i] == '(')
                {
                    depth++;
                }
                else if (value[i] == ')')
                {
                    depth--;
                }
                if (depth == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
