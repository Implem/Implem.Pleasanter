using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
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
                Name = columnName.Split(',').Skip(1).Join(string.Empty);
                TableAlias = columnName.Split_1st();
                SiteId = ColumnUtilities.GetSiteIdByTableAlias(TableAlias);
                Joined = true;
            }
            else
            {
                Name = columnName;
            }
        }

        public bool Exists(SiteSettings ss, Dictionary<long, SiteSettings> joinedSsHash)
        {
            if (!ss.ColumnDefinitionHash.ContainsKey(Name))
            {
                return false;
            }
            if (!joinedSsHash.ContainsKey(SiteId))
            {
                return false;
            }
            foreach (var part in TableAlias.Split('-'))
            {
                var columnName = part.Split('~').First();
                var siteId = part.Split('~').Skip(1).Join(string.Empty);
                if (!siteId.All(char.IsDigit))
                {
                    return false;
                }
                var currentSs = joinedSsHash.Get(siteId.ToLong());
                if (currentSs == null)
                {
                    return false;
                }
                if (!currentSs.ColumnDefinitionHash.ContainsKey(columnName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}