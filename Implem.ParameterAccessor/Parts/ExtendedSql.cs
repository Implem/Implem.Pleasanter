namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedSql : ExtendedBase
    {
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
        public bool OnUseSecondaryAuthentication;
        public string CommandText;
    }
}