using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class UserPlugin : ExtendedBase
    {
        public LibraryTypes LibraryType;
        public string FileName;

        public enum LibraryTypes
        {
            Pdf
        }
    }
}