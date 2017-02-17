namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlOrderBy
    {
        public string ColumnBracket;
        public Types OrderType;
        public string TableName;
        public Sqls.Functions Function;

        public enum Types
        {
            asc,
            desc,
            release
        }

        public SqlOrderBy(
            string columnBracket,
            Types orderType = Types.asc,
            string tableName = null,
            Sqls.Functions function = Sqls.Functions.None)
        {
            ColumnBracket = columnBracket;
            OrderType = orderType;
            TableName = tableName;
            Function = function;
        }

        public string Sql(Sqls.TableTypes tableType, Sqls.UnionTypes unionType)
        {
            var columnBracket = Sqls.TableAndColumnBracket(
                tableName: unionType == Sqls.UnionTypes.None
                    ? TableName
                    : null,
                tableType: tableType,
                columnBracket: ColumnBracket);
            var orderType = " " + OrderType.ToString().ToLower();
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
}
