namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlGroupBy
    {
        public string TableName;
        public string ColumnBracket;

        public SqlGroupBy(string columnBracket, string tableName)
        {
            TableName = tableName;
            ColumnBracket = columnBracket;
        }
    }
}
