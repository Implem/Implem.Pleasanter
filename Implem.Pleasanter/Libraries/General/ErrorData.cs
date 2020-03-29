namespace Implem.Pleasanter.Libraries.General
{
    public class ErrorData
    {
        public Error.Types Type;
        public long Id;
        public string ColumnName;
        public string[] Data;

        public ErrorData(
            Error.Types type,
            long id = 0,
            string columnName = null,
            params string[] data)
        {
            Type = type;
            Id = id;
            ColumnName = columnName;
            Data = data;
        }
    }
}