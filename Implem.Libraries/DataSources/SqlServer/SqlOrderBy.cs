using Implem.Libraries.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data.SqlClient;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlOrderBy
    {
        public string TableName;
        public string ColumnBracket;
        public Types OrderType;
        public string IsNullValue;
        public Sqls.Functions Function;
        public SqlStatement Sub;
        public string Raw;

        [JsonConverter(typeof(StringEnumConverter))]
        public enum Types
        {
            asc,
            desc,
            release
        }

        public SqlOrderBy(
            string columnBracket = null,
            Types orderType = Types.asc,
            string tableName = null,
            string isNullValue = null,
            Sqls.Functions function = Sqls.Functions.None,
            SqlStatement sub = null,
            string raw = null)
        {
            ColumnBracket = columnBracket;
            OrderType = orderType;
            TableName = tableName;
            IsNullValue = isNullValue;
            Function = function;
            Sub = sub;
            Raw = raw;
        }

        public string Sql(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            string tableBracket,
            Sqls.TableTypes tableType)
        {
            var orderType = " " + OrderType.ToString().ToLower();
            if (!Raw.IsNullOrEmpty())
            {
                return Raw;
            }
            else if (Sub != null)
            {
                return Sql_Sub(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    orderType: orderType);
            }
            else
            {
                string columnBracket = Sqls.TableAndColumnBracket(
                    tableBracket: tableBracket,
                    columnBracket: ColumnBracket);
                switch (Function)
                {
                    case Sqls.Functions.Count:
                    case Sqls.Functions.Sum:
                    case Sqls.Functions.Min:
                    case Sqls.Functions.Max:
                        return
                            Function.ToString().ToLower() +
                            "(" +
                            columnBracket +
                            ")" +
                            orderType;
                    case Sqls.Functions.Avg:
                        return $"avg(isnull({columnBracket}, 0)) {orderType}";
                    default:
                        return IsNullValue.IsNullOrEmpty()
                            ? columnBracket + orderType
                            : $"isnull({columnBracket}, {IsNullValue}) {orderType}";
                }
            }
        }

        private string Sql_Sub(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            string orderType)
        {
            return "(" + Sub.GetCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand) + ")"
                    + orderType;
        }
    }
}
