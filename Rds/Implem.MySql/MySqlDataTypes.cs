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
                .Replace("nvarchar", "varchar")
                .Replace("bit", "tinyint(1)")
                .Replace("image", "blob");
        }

        public string ConvertBack(string name)
        {
            return name == "char"
                ? "nchar"
                : name
                    .Replace("varchar", "nvarchar")
                    .Replace("longtext", "nvarchar")
                    .Replace("text", "nvarchar")
                    .Replace("tinyint", "bit")
                    .Replace("blob", "image");
        }

        public string DefaultDefinition(object dbRawValue)
        {
            string s = dbRawValue.ToString();
            s = Regex.Replace(s, @"^(?<str>'.+')::.+$", "${str}");
            return s;
        }
    }
}
