namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlGroupBy
    {
        public string ColumnBracket;
        public string TableName;

        public SqlGroupBy(string columnBracket, string tableName)
        {
            ColumnBracket = columnBracket;
            TableName = tableName;
        }
    }
}
