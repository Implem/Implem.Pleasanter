using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedSql
    {
        public string Name;
        public string Path;
        public string Description;
        public bool Disabled;
        public List<long> SiteIdList;
        public List<long> IdList;
        public List<string> Controllers;
        public List<string> Actions;
        public bool Api;
        public bool Html;
        public bool OnCreating;
        public bool OnCreated;
        public bool OnUpdating;
        public bool OnUpdated;
        public bool OnDeleting;
        public bool OnDeleted;
        public bool OnBulkUpdating;
        public bool OnBulkUpdated;
        public bool OnBulkDeleting;
        public bool OnBulkDeleted;
        public bool OnImporting;
        public bool OnImported;
        public bool OnSelectingWhere;
        public string CommandText;
    }
}