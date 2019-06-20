using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedSql
    {
        public string Path;
        public string Description;
        public bool Disabled;
        public List<long> SiteIdList;
        public List<long> IdList;
        public bool OnCreating;
        public bool OnCreated;
        public bool OnUpdating;
        public bool OnUpdated;
        public bool OnDeleting;
        public bool OnDeleted;
        public bool OnBulkDeleting;
        public bool OnBulkDeleted;
        public bool OnImporting;
        public bool OnImported;
        public string CommandText;
    }
}