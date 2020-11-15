using Implem.IRds;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlOrderBy
    {
        public string TableName;
        public string ColumnBracket;
        public Types OrderType;
        public Sqls.Functions Function;
        public SqlStatement Sub;

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
            Sqls.Functions function = Sqls.Functions.None,
            SqlStatement sub = null)
        {
            TableName = tableName;
            ColumnBracket = columnBracket;
            OrderType = orderType;
            Function = function;
            Sub = sub;
        }

        public string Sql(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            string tableBracket,
            Sqls.TableTypes tableType)
        {
            var orderType = " " + OrderType.ToString().ToLower();
            if (Sub != null)
            {
                return Sql_Sub(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    orderType: orderType);
            }
            else
            {
                string columnBracket = Sqls.TableAndColumnBracket(
                    tableBracket: tableType == Sqls.TableTypes.NormalAndHistory
                        ? string.Empty
                        : tableBracket,
                    columnBracket: ColumnBracket);
                switch (Function)
                {
                    case Sqls.Functions.Count:
                    case Sqls.Functions.Sum:
                    case Sqls.Functions.Min:
                    case Sqls.Functions.Max:
                    case Sqls.Functions.Avg:
                        return
                            Function.ToString().ToLower() +
                            "(" +
                            columnBracket +
                            ")" +
                            orderType;
                    default:
                        return columnBracket + orderType;
                }
            }
        }

        private string Sql_Sub(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            string orderType)
        {
            return "(" + Sub.GetCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand) + ")"
                    + orderType;
        }
    }
}
