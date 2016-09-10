namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlOrderBy
    {
        public string ColumnBracket;
        public Types OrderType;

        public enum Types
        {
            asc,
            desc,
            release
        }

        public SqlOrderBy(string columnBracket, Types orderType = Types.asc)
        {
            ColumnBracket = columnBracket;
            OrderType = orderType;
        }
    }
}
