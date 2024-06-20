using Implem.IRds;
using System.Data;
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
                .Replace("image", "longblob");
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
                    .Replace("longblob", "image");
        }

        public string DefaultDefinition(DataRow dataRow)
        {
            switch (dataRow["data_type"].ToString())
            {
                case "char":
                case "varchar":
                case "text":
                case "longtext":
                    return "'" + dataRow["column_default"].ToString() + "'";
                case "decimal":
                    return dataRow["column_default"].ToString().TrimEnd('0').TrimEnd('.'); ;
                default:
                    return dataRow["column_default"].ToString();
            }
        }
    }
}
