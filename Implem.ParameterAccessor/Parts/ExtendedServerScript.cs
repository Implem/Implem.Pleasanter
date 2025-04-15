namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedServerScript : ExtendedBase
    {
        public bool? WhenloadingSiteSettings;
        public bool? WhenViewProcessing;
        public bool? WhenloadingRecord;
        public bool? BeforeFormula;
        public bool? AfterFormula;
        public bool? BeforeCreate;
        public bool? AfterCreate;
        public bool? BeforeUpdate;
        public bool? AfterUpdate;
        public bool? BeforeDelete;
        public bool? AfterDelete;
        public bool? BeforeOpeningPage;
        public bool? BeforeOpeningRow;
        public bool? BeforeBulkDelete;
        public bool? AfterBulkDelete;
        public bool? Shared;
        public string Body;
    }
}