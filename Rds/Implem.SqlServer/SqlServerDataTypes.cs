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

        public string ConvertBack(string name)
        {
            return name;
        }

        public string DefaultDefinition(DataRow dataRow)
        {
            string s = dataRow["column_default"].ToString();
            s = Regex.Replace(s, @"^\(\((?<num>.+)\)\)$", "${num}");
            s = Regex.Replace(s, @"^\((?<str>'.+')\)$", "${str}");
            s = Regex.Replace(s, @"^\((?<other>.+)\)$", "${other}");
            return s;
        }
    }
}
