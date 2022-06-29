namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedLibrary : ExtendedBase
    {
        public int ReportId;
        public LibraryTypes LibraryType;
        public string LibraryName;

        public enum LibraryTypes
        {
            Print
        }
    }
}