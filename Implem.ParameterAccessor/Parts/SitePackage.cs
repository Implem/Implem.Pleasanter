namespace Implem.ParameterAccessor.Parts
{
    public class SitePackage
    {
        public enum OptionTypes : int
        {
            On = 0,
            Off = 1,
            Disabled = 2
        }

        public bool Import;
        public bool Export;
        public int ExportLimit;
        public OptionTypes IncludeDataOnImport;
        public OptionTypes IncludeSitePermissionOnImport;
        public OptionTypes IncludeRecordPermissionOnImport;
        public OptionTypes IncludeColumnPermissionOnImport;
        public OptionTypes IncludeNotificationsOnImport;
        public OptionTypes IncludeRemindersOnImport;
        public OptionTypes IncludeDataOnExport;
        public OptionTypes UseIndentOptionOnExport;
        public OptionTypes IncludeSitePermissionOnExport;
        public OptionTypes IncludeRecordPermissionOnExport;
        public OptionTypes IncludeColumnPermissionOnExport;
        public OptionTypes IncludeNotificationsOnExport;
        public OptionTypes IncludeRemindersOnExport;
    }
}