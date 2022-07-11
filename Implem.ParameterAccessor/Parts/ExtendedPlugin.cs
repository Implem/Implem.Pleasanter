using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedPlugin : ExtendedBase
    {
        public PluginTypes PluginType;
        public string LibraryPath;

        public enum PluginTypes
        {
            Pdf
        }
    }
}