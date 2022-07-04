using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedLibrary : ExtendedBase
    {
        public LibraryTypes LibraryType;
        public string FileName;

        public enum LibraryTypes
        {
            Print
        }
    }
}