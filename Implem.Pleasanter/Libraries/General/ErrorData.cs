namespace Implem.Pleasanter.Libraries.General
{
    public class ErrorData
    {
        public Error.Types Type;
        public long Id;
        public string ColumnName;

        public ErrorData(Error.Types type, long id = 0, string columnName = null)
        {
            Type = type;
            Id = id;
            ColumnName = columnName;
        }
    }
}