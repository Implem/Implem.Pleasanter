namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlColumn
    {
        public string ColumnBracket;
        public bool AdHoc;

        public SqlColumn()
        {
        }

        public SqlColumn(string columnBracket, bool adHoc = false)
        {
            ColumnBracket = columnBracket;
            AdHoc = adHoc;
        }
    }
}
