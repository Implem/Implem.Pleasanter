using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedPlugin : ExtendedBase
    {
        public LibraryTypes LibraryType;
        public string FileName;

        public enum LibraryTypes
        {
            Pdf
        }
    }
}