using Implem.Libraries.Utilities;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class ColumnNameInfo
    {
        public string ColumnName;
        public string Name;
        public string TableAlias;
        public long SiteId;
        public bool Joined;

        public ColumnNameInfo(Column column)
        {
            Set(column.ColumnName);
        }

        public ColumnNameInfo(string columnName)
        {
            Set(columnName);
        }

        private void Set(string columnName)
        {
            ColumnName = columnName;
            if (columnName.Contains(","))
            {
                Name = columnName.Split_2nd();
                TableAlias = columnName.Split_1st();
                SiteId = ColumnUtilities.GetSiteIdByTableAlias(TableAlias);
                Joined = true;
            }
            else
            {
                Name = columnName;
            }
        }
    }
}