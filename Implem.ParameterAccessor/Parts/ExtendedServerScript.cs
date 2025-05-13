﻿namespace Implem.ParameterAccessor.Parts
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
        public bool? BeforeBulkDelete;
        public bool? AfterDelete;
        public bool? AfterBulkDelete;
        public bool? BeforeOpeningPage;
        public bool? BeforeOpeningRow;
        public bool? Shared;
        public bool? Functionalize;
        public bool? TryCatch;
        public string Body;
    }
}